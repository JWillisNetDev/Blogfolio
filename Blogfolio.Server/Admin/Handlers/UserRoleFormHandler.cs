using Blogfolio.Server.Components.Admin;
using Microsoft.AspNetCore.Identity;

namespace Blogfolio.Server.Admin.Handlers;

public class UserRoleFormHandler : AdminCustomFormHandler<IdentityRole, IdentityRoleForm, UserRoleFormView>
{
    private readonly RoleManager<IdentityRole> _roles;
    private readonly ILogger<UserRoleFormHandler> _logger;

    public UserRoleFormHandler(RoleManager<IdentityRole> roles, ILogger<UserRoleFormHandler> logger)
    {
        _roles = roles ?? throw new ArgumentNullException(nameof(roles));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task<IdentityRoleForm> LoadAsync(string key)
    {
        if (await _roles.FindByIdAsync(key) is { } found)
        {
            return new IdentityRoleForm()
            {
                Name = found.Name ?? "",
                Claims = (await _roles.GetClaimsAsync(found)).Select(c => c.Type).ToArray(),
            };
        }

        _logger.LogError("Failed to find role given key: {}", key);
        throw new InvalidOperationException();
    }

    public override async Task DeleteAsync(string key)
    {
        if (await _roles.FindByIdAsync(key) is { } found)
        {
            _logger.LogTrace("Deleting role of ID {}", found.Id);
            var res = await _roles.DeleteAsync(found);
            Check(res);
            return;
        }
        _logger.LogError("Failed to find role given key: {}", key);
        throw new InvalidOperationException();
    }

    protected override async Task SaveAsync(string? key, IdentityRoleForm form)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            IdentityRole role = new(form.Name);
            var res = await _roles.CreateAsync(role);
            Check(res);
            return;
        }
        
        if (await _roles.FindByIdAsync(key) is { } found)
        {
            found.Name = form.Name;
            Check(await _roles.UpdateAsync(found));
            return;
        }

        throw new InvalidOperationException();
    }

    private static void Check(IdentityResult res)
    {
        if (!res.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", res.Errors.Select(e => e.Description)));
        }
    }
}
