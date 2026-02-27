
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace ShoppingList.Core.Domain.Entities;

[DebuggerDisplay("{Name}: {Status}")]
public class Item : IEntity
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = String.Empty;

    [Required]
    [StringLength(200)]
    public string Description { get; set; } = String.Empty;

    [Range(0, 100000)]
    public int Price { get; set; }

    public ItemStatus Status { get; set; } = ItemStatus.NotPurchased;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
