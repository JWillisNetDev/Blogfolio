using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Blogfolio.Data.Identity;

namespace Blogfolio.Data.Models;

public abstract class BaseEntity : IAuditable
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid(); 
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastUpdatedAt { get; set; }
    public string? CreatedByUserId { get; set; }
    [ForeignKey(nameof(CreatedByUserId))]
    public virtual BlogfolioUser? CreatedBy { get; set; }
    public string? LastUpdatedByUserId { get; set; }
    [ForeignKey(nameof(LastUpdatedByUserId))]
    public BlogfolioUser? LastUpdatedBy { get; set; }
}