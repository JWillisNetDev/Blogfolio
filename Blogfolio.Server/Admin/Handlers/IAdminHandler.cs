namespace Blogfolio.Server.Admin.Handlers;

public interface IAdminHandler
{
    Type ClrType { get; }
    Task SaveAsync(object?[]? keys, IReadOnlyDictionary<string, object?> values);
    Task DeleteAsync(object?[] keys);
}
