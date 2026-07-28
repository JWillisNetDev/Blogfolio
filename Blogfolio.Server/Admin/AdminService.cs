using Blogfolio.Server.Admin.Handlers;

namespace Blogfolio.Server.Admin;

public sealed  class AdminService(AdminRepository repo, IEnumerable<IAdminHandler> handlers)
{
    public AdminRepository Repository => repo;
    
    public Task<IReadOnlyList<object>> ListAsync(AdminEntity entity)
        => repo.ListAsync(entity);

    public Task<object?> FindAsync(AdminEntity e, object?[] keys)
        => repo.FindAsync(e, keys);

    public Task<IReadOnlyList<AdminOption>> OptionsAsync(AdminField field)
        => repo.OptionsAsync(field);

    public Task SaveAsync(AdminEntity e, object?[]? keys, IReadOnlyDictionary<string, object?> values)
        => Handler(e) is { } h ? h.SaveAsync(keys, values) : repo.SaveAsync(e, keys, values);

    public Task DeleteAsync(AdminEntity e, object?[] keys)
        => Handler(e) is { } handler ? handler.DeleteAsync(keys) : repo.DeleteAsync(e, keys);

    private IAdminHandler? Handler(AdminEntity e)
        => handlers.FirstOrDefault(h => h.ClrType == e.ClrType);
}