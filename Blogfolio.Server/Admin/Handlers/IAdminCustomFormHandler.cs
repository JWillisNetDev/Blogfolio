namespace Blogfolio.Server.Admin.Handlers;

public interface IAdminCustomFormHandler
{
    Type ClrType { get; }
    Type Component { get; }
    Task<object> LoadFormAsync(string key);
    Task SaveFormAsync(string key, object form);
    Task DeleteAsync(string key);
}
