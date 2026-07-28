using Blogfolio.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blogfolio.Data;

public static class DependencyInjection
{
    public static T AddBlogfolioDb<T>(this T collection, IConfiguration config) where T : IServiceCollection
    {
        var connectionString = config.GetConnectionString("Default");
        collection.AddDbContextFactory<BlogfolioDbContext>((options) =>
        {
            options.UseSqlite(connectionString)
                .UseSeeding((db, _) =>
                {
                    db.Set<TodoItem>().Add(new TodoItem
                    {
                        Name = "Example Todo 1",
                        Description = "Take out the trash",
                    });
                    db.Set<TodoItem>().Add(new TodoItem
                    {
                        Name = "Example Todo 2",
                        Description = "Mow the lawn",
                    });
                    db.Set<TodoItem>().Add(new TodoItem
                    {
                        Name = "Completed Todo",
                        Description = "Wake up this morning",
                        IsDone = true,
                    });
                    db.SaveChanges();
                });
        });
        return collection;
    }
}