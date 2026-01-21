using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RestoPOS.Common.Enums;

namespace RestoPOS.Data.Entities;

/// <summary>
/// Ödeme kaydı
/// </summary>
public class Payment
{
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal SubTotal { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal TaxAmount { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal DiscountAmount { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal ReceivedAmount { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal ChangeAmount { get; set; }
    
    public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
    
    [MaxLength(100)]
    public string? InvoiceNumber { get; set; }
    
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public virtual Order Order { get; set; } = null!;
}
