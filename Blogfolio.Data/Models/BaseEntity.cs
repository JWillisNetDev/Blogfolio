using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Blogfolio.Data.Identity;

namespace Blogfolio.Data.Models;

public abstract class BaseEntity : IAuditable
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid(); 

    [ReadOnly(true)]
    public DateTimeOffset CreatedAt { get; set; }
    [ReadOnly(true)]
    public DateTimeOffset LastUpdatedAt { get; set; }

    [ReadOnly(true)]
    public string? CreatedByUserId { get; set; }
    [ForeignKey(nameof(CreatedByUserId))]
    public virtual BlogfolioUser? CreatedBy { get; set; }

    [ReadOnly(true)]
    public string? LastUpdatedByUserId { get; set; }
    [ForeignKey(nameof(LastUpdatedByUserId))]
    public BlogfolioUser? LastUpdatedBy { get; set; }
}