using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestoPOS.Data.Entities;

/// <summary>
/// Ürün
/// </summary>
public class Product
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public int CategoryId { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }
    
    public int Stock { get; set; } = 0;
    
    [MaxLength(500)]
    public string? ImagePath { get; set; }
    
    /// <summary>
    /// KDV oranı (0, 1, 10, 20)
    /// </summary>
    public int TaxRate { get; set; } = 10;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
