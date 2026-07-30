using Blogfolio.Data.Identity;
using Blogfolio.Server.Components.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace Blogfolio.Server.Admin.Handlers;

public sealed class UserAdminHandler(UserManager<BlogfolioUser> users) : AdminFormHandler<BlogfolioUser, UserAdminForm>
{
    public override Type Component => typeof(UserFormView);

    protected override async Task<UserAdminForm> LoadAsync(object?[] keys)
    {
        return await users.FindByIdAsync(keys[0]!.ToString()!) is { } usr
            ? new UserAdminForm { Email = usr.Email ?? "", EmailConfirmed = usr.EmailConfirmed }
            : new UserAdminForm();
    }

    protected override async Task SaveAsync(object?[]? keys, UserAdminForm form)
    {
        if (keys is null)
        {
            var usr = new BlogfolioUser
            {
                UserName = form.Email,
                Email = form.Email,
            };
            Check(await users.CreateAsync(usr, form.Password!));
            return;
        }

        if (await users.FindByIdAsync(keys[0]!.ToString()!) is not { } existing)
        {
            return;
        }
        existing.Email = existing.UserName = form.Email;
        existing.EmailConfirmed = form.EmailConfirmed;
        Check(await users.UpdateAsync(existing));
    }

    public override async Task DeleteAsync(object?[] keys)
    {
        if (await users.FindByIdAsync(keys[0]!.ToString()!) is { } user)
        {
            Check(await users.DeleteAsync(user));
        }
    }

    private static void Check(IdentityResult res)
    {
        if (!res.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", res.Errors.Select(e => e.Description)));
        }
    }
}

public abstract class AdminFormBase<TForm> : ComponentBase
{
    [Parameter, EditorRequired]
    public TForm Model { get; set; } = default!;
    
    [Parameter]
    public EventCallback OnSubmit { get; set; }
}