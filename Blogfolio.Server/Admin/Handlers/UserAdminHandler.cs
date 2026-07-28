using Blogfolio.Data.Identity;
using Microsoft.AspNetCore.Identity;

namespace Blogfolio.Server.Admin.Handlers;

// TODO: Could we generify this dynamic code?
public sealed class UserAdminHandler(UserManager<BlogfolioUser> users) : IAdminHandler
{
    public Type ClrType => typeof(BlogfolioUser);

    public async Task SaveAsync(object?[]? keys, IReadOnlyDictionary<string, object?> values)
    {
        var email = values.GetValueOrDefault("Email")?.ToString() ?? "";

        if (keys is null)
        {
            var user = new BlogfolioUser { UserName = email, Email = email };
            var password = values.GetValueOrDefault("Password")?.ToString() ?? "";
            Check(await users.CreateAsync(user, password));
        }
        else
        {
            if (await users.FindByIdAsync(keys[0]!.ToString()!) is not { } user)
            {
                return;
            }
            user.Email = email;
            user.UserName =email;
            Check(await users.UpdateAsync(user));
        }
    }

    public async Task DeleteAsync(object?[] keys)
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