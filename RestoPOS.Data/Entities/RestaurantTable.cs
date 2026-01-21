using System.ComponentModel.DataAnnotations;
using RestoPOS.Common.Enums;

namespace RestoPOS.Data.Entities;

/// <summary>
/// Restoran masası
/// </summary>
public class RestaurantTable
{
    public int Id { get; set; }
    
    [Required]
    public int TableNumber { get; set; }
    
    [MaxLength(50)]
    public string? TableName { get; set; }
    
    public int Capacity { get; set; } = 4;
    
    public TableStatus Status { get; set; } = TableStatus.Empty;
    
    public bool IsActive { get; set; } = true;
    
    // Navigation property
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
