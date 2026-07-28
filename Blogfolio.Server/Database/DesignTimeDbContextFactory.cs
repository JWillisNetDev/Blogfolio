using Blogfolio.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BlogfolioDbContext>
{
    public BlogfolioDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<BlogfolioDbContext>();
        optionsBuilder.UseSqlite(config.GetConnectionString("Default"), b => b.MigrationsAssembly("Blogfolio.Server"));
        return new BlogfolioDbContext(optionsBuilder.Options);
    }
}