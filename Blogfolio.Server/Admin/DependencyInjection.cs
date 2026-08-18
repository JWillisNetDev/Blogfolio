using Blogfolio.Server.Admin.Handlers;

namespace Blogfolio.Server.Admin;

public static class DependencyInjection
{
    public static T AddAdminServices<T>(this T collection)
        where T : IServiceCollection
    {
        // TODO: These all need interfaces for testing and such.
        collection.AddScoped<AdminSchema>();
        collection.AddScoped<AdminRepository>();
        collection.AddScoped<AdminService>();
        collection.AddScoped<IAdminCustomFormHandler, UserAdminHandler>();
        collection.AddScoped<IAdminCustomFormHandler, UserRoleFormHandler>();
        return collection;
    }
}