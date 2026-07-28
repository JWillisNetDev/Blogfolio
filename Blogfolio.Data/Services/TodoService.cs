using System.Security.Claims;
using Blogfolio.Data.Identity;
using Blogfolio.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Blogfolio.Data.Services;

public class TodoService : ITodoService
{
    private readonly IDbContextFactory<BlogfolioDbContext> _dbFactory;

    public TodoService(IDbContextFactory<BlogfolioDbContext> dbFactory)
    {
        _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
    }

    public async Task<TodoItem> CreateTodoAsync(string title, string description, BlogfolioUser user)
    {
        TodoItem todo = new()
        {
            Name = title,
            Description = description,
            CreatedBy = user,
            LastUpdatedBy = user,
        };

        using var db = await _dbFactory.CreateDbContextAsync();
        await db.TodoItems.AddAsync(todo);
        await db.SaveChangesAsync();

        return todo;
    }

    public async Task<bool> DeleteTodoAsync(string? todoId, BlogfolioUser user)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var found = await db.TodoItems.FindAsync(todoId);



        if (found is not null)
        {
            db.TodoItems.Remove(found);
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<ICollection<TodoItem>> GetTodosForUserAsync(BlogfolioUser user)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.TodoItems.Where(t => t.CreatedByUserId == user.Id).ToListAsync();
    }

    public async Task<TodoItem?> GetTodoItemAsync(string todoItemId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.TodoItems.FindAsync(todoItemId);
    }

    public static bool CanDelete(ClaimsPrincipal user, TodoItem item)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return user.HasClaim(Constants.Claims.DeleteAnyTodo, "true")
            || (userId is not null && item.CreatedByUserId == userId);
    }
}