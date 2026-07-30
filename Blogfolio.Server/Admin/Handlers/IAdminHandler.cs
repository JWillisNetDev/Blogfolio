using System.ComponentModel.DataAnnotations;

namespace Blogfolio.Server.Admin.Handlers;

public interface IAdminHandler
{
    Type ClrType { get; }
    Task DeleteAsync(object?[] keys);
}

public interface IAdminFieldHandler : IAdminHandler
{
    Task SaveAsync(object?[]? keys, IReadOnlyDictionary<string, object?> values);
}

public interface IAdminFormHandler : IAdminHandler
{
    Type Component { get; }
    Task<object> LoadFormAsync(object?[]? keys);
    Task SaveFormAsync(object?[]? keys, object model);
}

public abstract class AdminFormHandler<TEntity, TForm> : IAdminFormHandler
    where TForm: new()
{
    public Type ClrType => typeof(TEntity);
    public abstract Type Component { get; }
    public abstract Task DeleteAsync(object?[] keys);

    protected abstract Task<TForm> LoadAsync(object?[] keys);
    protected abstract Task SaveAsync(object?[]? keys, TForm form);

    async Task<object> IAdminFormHandler.LoadFormAsync(object?[]? keys)
        => keys is null ? new TForm() : (await LoadAsync(keys))!;

    Task IAdminFormHandler.SaveFormAsync(object?[]? keys, object model)
        => SaveAsync(keys, (TForm)model);
}

public sealed class UserAdminForm
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    public bool EmailConfirmed { get; set; }

    [MinLength(8)]
    public string? Password { get; set; }
}