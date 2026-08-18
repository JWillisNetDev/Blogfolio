using Blogfolio.Data.Identity;
using Blogfolio.Data.Services;
using Microsoft.AspNetCore.Identity;

namespace Blogfolio.Test.Services;

public class TodoServiceTests : DatabaseTestFixture
{
    [Fact]
    public async Task It_CreatesNewTodoItems()
    {
        string title = "Test Title";
        string desc = "Test Description";
        BlogfolioUser user = new()
        {
            Email = "TestUser",
        };

        using var db = CreateDbContext();
        db.Add(user);

        TodoService sut = new TodoService(this);
        var created = await sut.CreateTodoAsync(title, desc, user);

        var found = await db.TodoItems.FindAsync(created.Id);
        Assert.NotNull(found);
        Assert.Equal(title, found.Name);
        Assert.Equal(desc, found.Description);
        Assert.Equal(user, found.CreatedBy);
        Assert.Equal(user, found.LastUpdatedBy);
    }
}