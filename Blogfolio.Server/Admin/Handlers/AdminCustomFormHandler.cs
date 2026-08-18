using System.ComponentModel.DataAnnotations;

namespace Blogfolio.Server.Admin.Handlers;

public abstract class AdminCustomFormHandler<TEntity, TForm, TComponent> : IAdminCustomFormHandler
    where TForm : notnull, new()
    where TComponent : AdminFormBase<TForm>
{
    public Type ClrType => typeof(TEntity);
    public Type Component => typeof(TComponent);
    public abstract Task DeleteAsync(string key);
    protected abstract Task<TForm> LoadAsync(string key);
    protected abstract Task SaveAsync(string key, TForm form);
    
    async Task<object> IAdminCustomFormHandler.LoadFormAsync(string? key)
    {
        return key is null ? new TForm() : await LoadAsync(key);
    }

    Task IAdminCustomFormHandler.SaveFormAsync(string? key, object form)
    {
        return SaveAsync(key, (TForm) form);
    }
}

public sealed class UserAdminForm
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    public bool EmailConfirmed { get; set; }

    [MinLength(8)]
    public string? Password { get; set; }
}

public class IdentityRoleForm
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string[] Claims { get; set; } = null!;
}