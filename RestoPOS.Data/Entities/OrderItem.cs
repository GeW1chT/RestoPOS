using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestoPOS.Data.Entities;

/// <summary>
/// Sipariş kalemi
/// </summary>
public class OrderItem
{
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    
    public int ProductId { get; set; }
    
    public int Quantity { get; set; } = 1;
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal UnitPrice { get; set; }
    
    /// <summary>
    /// Ürünün KDV oranı (sipariş anında)
    /// </summary>
    public int TaxRate { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal TaxAmount { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalPrice { get; set; }
    
    [MaxLength(300)]
    public string? Note { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Order Order { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
