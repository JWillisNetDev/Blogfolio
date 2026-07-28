using System.Globalization;
using System.Reflection;
using Blogfolio.Data;
using Microsoft.EntityFrameworkCore;

namespace Blogfolio.Server.Admin;

public class AdminRepository(IDbContextFactory<BlogfolioDbContext> dbFactory, AdminSchema schema)
{
    private static MethodInfo SetOf = typeof(DbContext).GetMethods()
        .First(m => m.Name == nameof(DbContext.Set) && m.IsGenericMethod && m.GetParameters().Length == 0);

    public async Task<IReadOnlyList<object>> ListAsync(AdminEntity e)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        if (SetOf.MakeGenericMethod(e.ClrType).Invoke(db, null) is IQueryable set)
        {
            return await set.Cast<object>().ToListAsync();
        }
        
        throw new InvalidOperationException($"No valid database Set for generic type `{e.ClrType.FullName}` exists or failed to be created.");
    }

    public async Task<object?> FindAsync(AdminEntity entity, object?[] keys)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.FindAsync(entity.ClrType, keys);
    }

    public async Task SaveAsync(AdminEntity entity, object?[]? keys, IReadOnlyDictionary<string, object?> values)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        object inst;
        if (keys is null)
        {
            inst = Activator.CreateInstance(entity.ClrType)!;
            db.Add(inst);
        }
        else
        {
            inst = await db.FindAsync(entity.ClrType, keys)
                ?? throw new InvalidOperationException($"{entity.Name} not found.");
        }

        foreach (var f in entity.EditFields.Where(f => f.Property is not null))
        {
            if (values.TryGetValue(f.Name, out var v))
            {
                // Used LINQ to ensure non-null reference here.
                f.Property!.SetValue(inst, Coerce(v, f.Property.PropertyType));
            }
        }

        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(AdminEntity entity, object?[] keys)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var existing = await db.FindAsync(entity.ClrType, keys);
        if (existing is null)
        {
            return;
        }

        db.Remove(existing);
        await db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<AdminOption>> OptionsAsync(AdminField field)
    {
        if (field.PrincipalType is not { } principalType
            || schema.ByType(principalType) is not { } principal)
        {
            return [];
        }

        var rows = await ListAsync(principal);
        return rows.Select(r => new AdminOption(KeyOf(principal, r), LabelOf(principal, r))).ToList();
    }

    public object?[] KeyValues(AdminEntity entity, object row)
        => entity.KeyFields.Select(k => k.Property!.GetValue(row)).ToArray();
    private static object? KeyOf(AdminEntity e, object row)
        => e.KeyFields.FirstOrDefault()?.Property?.GetValue(row);

    private static string LabelOf(AdminEntity e, object row)
    {
        foreach (string name in new string[] { "Email", "UserName", "Name", "Title", "DisplayName" })
        {
            string? v = e.Fields.FirstOrDefault(f => f.Name == name)?.Property?.GetValue(row)?.ToString();
            if (!string.IsNullOrWhiteSpace(v))
            {
                return v;
            }
        }

        return KeyOf(e, row)?.ToString() ?? e.Name;
    }

    private static object? Coerce(object? v, Type targetType)
    {
        if (v is null)
        {
            return null;
        }

        var target = Nullable.GetUnderlyingType(targetType) ?? targetType;
        
        if (target.IsInstanceOfType(v))
        {
            return v;
        }
        
        if (target.IsEnum)
        {
            return Enum.Parse(targetType, v.ToString()!);
        }
        
        if (target == typeof(Guid))
        {
            return v is Guid g ? g : Guid.Parse(v.ToString()!);
        }

        return Convert.ChangeType(v, target, CultureInfo.InvariantCulture);
    }
}