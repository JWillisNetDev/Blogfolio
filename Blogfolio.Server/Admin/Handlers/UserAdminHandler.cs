using Blogfolio.Data.Identity;
using Blogfolio.Server.Components.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Blogfolio.Server.Admin.Handlers;

public sealed class UserAdminHandler : AdminCustomFormHandler<BlogfolioUser, UserAdminForm, UserAdminFormView>
{
    private readonly UserManager<BlogfolioUser> _users;

    public UserAdminHandler(UserManager<BlogfolioUser> users)
    {
        _users = users ?? throw new ArgumentNullException(nameof(users));
    }

    protected override async Task<UserAdminForm> LoadAsync(string key)
    {
        if (await _users.FindByIdAsync(key) is { } usr)
        {
            return new UserAdminForm()
            {
                Email = usr.Email ?? "",
                EmailConfirmed = usr.EmailConfirmed,
            };
       }
       throw new InvalidOperationException();
    }

    protected override async Task SaveAsync(string? key, UserAdminForm form)
    {
        if (string.IsNullOrEmpty(key))
        {
            var usr = new BlogfolioUser
            {
                UserName = form.Email,
                Email = form.Email,
            };
            Check(await _users.CreateAsync(usr, form.Password!));
            return;
        }
        
        if (await _users.FindByIdAsync(key) is { } found)
        {
            found.Email = found.UserName = form.Email;
            found.EmailConfirmed = form.EmailConfirmed;
            Check(await _users.UpdateAsync(found));
            return;
        }

        throw new InvalidOperationException("Failed to save.");
    }

    public override async Task DeleteAsync(string key)
    {
        if (await _users.FindByIdAsync(key) is { } user)
        {
            Check(await _users.DeleteAsync(user));
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
