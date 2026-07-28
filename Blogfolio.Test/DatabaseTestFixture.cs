using Blogfolio.Data;
using Blogfolio.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Blogfolio.Test;

public abstract class DatabaseTestFixture : IDbContextFactory<BlogfolioDbContext>, IDisposable
{
    private readonly TestBlogfolioDbContextFactory _dbContextFactory = new();
    protected IDbContextFactory<BlogfolioDbContext> DbContextFactory => _dbContextFactory;
    private bool _disposed;

    public BlogfolioDbContext CreateDbContext() => _dbContextFactory.CreateDbContext();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _dbContextFactory?.Dispose();
        }

        _disposed = true;
    }

    private sealed class TestBlogfolioDbContextFactory : IDbContextFactory<BlogfolioDbContext>, IDisposable
    {
        private readonly SqliteConnection _connection;
        private bool _disposed;

        public TestBlogfolioDbContextFactory(Action<BlogfolioDbContext>? seeding = null)
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            using var db = CreateDbContext();
            db.Database.EnsureCreated();
        }

        public BlogfolioDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<BlogfolioDbContext>().UseSqlite(_connection);
            return new BlogfolioDbContext(optionsBuilder.Options);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _connection?.Dispose();
            _disposed = true;
        }
    }
}