using Blogfolio.Data.Identity;
using Blogfolio.Data.Models;

namespace Blogfolio.Data.Services;

public interface ITodoService
{
    Task<TodoItem> CreateTodoAsync(string title, string desription, BlogfolioUser user);
    Task<bool> DeleteTodoAsync(string? todoId, BlogfolioUser user);
    Task<ICollection<TodoItem>> GetTodosForUserAsync(BlogfolioUser user);
    Task<TodoItem?> GetTodoItemAsync(string todoItemId);
}
