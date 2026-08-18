using Blogfolio.Server.Admin;
using Blogfolio.Server.Admin.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Moq.AutoMock;

namespace Blogfolio.Test.Admin;

public class AdminDependencyInjectionTests
{
    private class TestEntity
    {
        public string? Name { get; set; }
        public int Payload { get; set; }
    }

    private class TestAdminHandler : IAdminHandler
    {
        public Type ClrType => throw new NotImplementedException();
        public Task DeleteAsync(object?[] keys) => throw new NotImplementedException();
        public Task SaveAsync(object?[]? keys, IReadOnlyDictionary<string, object?> values) => throw new NotImplementedException();
    }

    [Fact]
    public void It_RegistersByTypeAndHandler()
    {
        

    }

    
}