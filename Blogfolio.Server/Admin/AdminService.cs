using Blogfolio.Server.Admin.Handlers;

namespace Blogfolio.Server.Admin;

public sealed class AdminService(AdminRepository repo, IEnumerable<IAdminCustomFormHandler> customForms)
{
    public AdminRepository Repository => repo;

    public Task<IReadOnlyList<object>> ListAsync(AdminEntity entity)
        => repo.ListAsync(entity);

    public Task<object?> FindAsync(AdminEntity e, object?[] keys)
        => repo.FindAsync(e, keys);

    public Task<IReadOnlyList<AdminOption>> OptionsAsync(AdminField field)
        => repo.OptionsAsync(field);

    public Task SaveAsync(AdminEntity e, object?[]? keys, IReadOnlyDictionary<string, object?> values)
        => FormFor(e) is { } handler ? handler.SaveFormAsync(AssumeKey(keys), values) : repo.SaveAsync(e, keys, values);

    public Task DeleteAsync(AdminEntity e, object?[] keys)
        => FormFor(e) is { } handler ? handler.DeleteAsync(AssumeKey(keys)) : repo.DeleteAsync(e, keys);

    public IAdminCustomFormHandler? FormFor(AdminEntity e)
        => customForms.OfType<IAdminCustomFormHandler>().FirstOrDefault(h => h.ClrType == e.ClrType);

    private static string AssumeKey(object?[]? keys)
    {
        if (keys is [string { Length: >0 } id, ..])
        {
           return id;
        }
        throw new InvalidOperationException($"Failed to assume key from {keys}");
    }
}