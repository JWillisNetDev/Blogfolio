using Microsoft.AspNetCore.Identity;

namespace Blogfolio.Server.Admin.Handlers;

public sealed class RoleAdminHandler(RoleManager<IdentityRole> roles) : IAdminHandler
{
    public Type ClrType => typeof(IdentityRole);

    public async Task SaveAsync(object?[]? keys, IReadOnlyDictionary<string, object?> values)
    {
        var name = values.GetValueOrDefault("Name")?.ToString() ?? "";
        if (keys is null)
        {
            await roles.CreateAsync(new IdentityRole(name));
        }
        else
        {
            if (await roles.FindByIdAsync(keys[0]!.ToString()!) is not { } role)
            {
                return;
            }
            role.Name = name;
            await roles.UpdateAsync(role);
        }
    }

    public async Task DeleteAsync(object?[] keys)
    {
        if (await roles.FindByIdAsync(keys[0]!.ToString()!) is { } role)
        {
            await roles.DeleteAsync(role);
        }
    }
}