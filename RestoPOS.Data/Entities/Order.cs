using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RestoPOS.Common.Enums;

namespace RestoPOS.Data.Entities;

/// <summary>
/// Sipariş
/// </summary>
public class Order
{
    public int Id { get; set; }
    
    public int TableId { get; set; }
    
    public int? UserId { get; set; }
    
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal SubTotal { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal TaxAmount { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual RestaurantTable Table { get; set; } = null!;
    public virtual User? User { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual Payment? Payment { get; set; }
}
