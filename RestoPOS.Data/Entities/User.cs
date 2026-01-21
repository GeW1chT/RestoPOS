using System.ComponentModel.DataAnnotations;
using RestoPOS.Common.Enums;

namespace RestoPOS.Data.Entities;

/// <summary>
/// Kullanıcı
/// </summary>
public class User
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
    
    public UserRole Role { get; set; } = UserRole.Waiter;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastLoginAt { get; set; }
    
    // Navigation property
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
