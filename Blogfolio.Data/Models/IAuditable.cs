using Blogfolio.Data.Identity;

namespace Blogfolio.Data.Models;

public interface IAuditable
{
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset LastUpdatedAt { get; set; }
    string? CreatedByUserId { get; set; }
    string? LastUpdatedByUserId { get; set; }
}
