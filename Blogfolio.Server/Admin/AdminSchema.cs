using Blogfolio.Data;
using Blogfolio.Data.Identity;
using Blogfolio.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Blogfolio.Server.Admin;

public sealed class AdminSchema
{
    // TODO: Turn this into an attribute `SecretAttribute`
    private static readonly HashSet<string> SecretFields = [
        "PasswordHash", // ...
    ];

    // TODO: Turn this into an attribute `AuditFieldAttribute`
    private static readonly HashSet<string> SystemManaged = [
        "CreatedAt", "LastUpdatedAt", "CreatedByUserId", "LastUpdatedByUserId",
    ];

    private readonly Dictionary<string, AdminEntity> _bySlug;

    public AdminSchema(IDbContextFactory<BlogfolioDbContext> factory)
    {
        using var db = factory.CreateDbContext();
        List<AdminEntity> entities = [];

        foreach (var entType in db.Model.GetEntityTypes().Where(IsEntityTypeSupported))
        {
            List<AdminField> fields = entType.GetProperties()
                .Where(p => !p.IsShadowProperty())
                .Select(p =>
                {
                    var underlying = Nullable.GetUnderlyingType(p.ClrType) ?? p.ClrType;
                    var generated = p.ValueGenerated != ValueGenerated.Never;
                    var fk = p.GetContainingForeignKeys().FirstOrDefault(); 
                    
                    return new AdminField(
                        Name: p.Name,
                        Label: Humanize(p.Name),
                        ClrType: p.ClrType,
                        Property: p.PropertyInfo,
                        IsKey: p.IsPrimaryKey(),
                        ReadOnly: p.IsPrimaryKey() || generated || SystemManaged.Contains(p.Name),
                        Hidden: SecretFields.Contains(p.Name),
                        IsForeignKey: fk is not null,
                        PrincipalType: fk?.PrincipalEntityType.ClrType,
                        IsEnum: underlying.IsEnum);
                })
                .ToList();

            if (entType.ClrType == typeof(BlogfolioUser))
            {
                fields.Add(new AdminField("Password", "Password", typeof(string), null, false, false, false, false, null, false));
            }

            var name = entType.ClrType.Name;
            entities.Add(new AdminEntity(name, name.ToLowerInvariant(), entType.ClrType, fields));
        }

        _bySlug = entities.OrderBy(e => e.Name).ToDictionary(e => e.Slug);
    }

    public IReadOnlyCollection<AdminEntity> Entities => _bySlug.Values;
    public AdminEntity? Find(string slug) => _bySlug.GetValueOrDefault(slug.ToLowerInvariant());
    public AdminEntity? ByType(Type type) => _bySlug.Values.FirstOrDefault(e => e.ClrType == type);

    private static string Humanize(string name)
    {
        return string.Concat(name.Select((c, i) => i > 0 && char.IsUpper(c) ? " " + c : c.ToString())).Trim();
    }

    private static bool IsEntityTypeSupported(IEntityType entType)
    {
        if (entType.IsOwned() || entType.FindPrimaryKey() is null)
        {
            return false;
        }

        if (entType.ClrType == typeof(IdentityRole))
        {
            return true;
        }

        return entType.ClrType.Assembly == typeof(BaseEntity).Assembly;
    }
}