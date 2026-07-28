namespace Blogfolio.Server.Admin;

public sealed record AdminEntity(string Name, string Slug, Type ClrType, IReadOnlyList<AdminField> Fields)
{
    public IEnumerable<AdminField> ListFields => Fields.Where(f => !f.Hidden);
    public IEnumerable<AdminField> EditFields => Fields.Where(f => !f.Hidden && !f.ReadOnly);
    public IReadOnlyList<AdminField> KeyFields => Fields.Where(f => f.IsKey).ToList();
}
