using System.ComponentModel.DataAnnotations;

namespace RestoPOS.Data.Entities;

/// <summary>
/// Sistem ayarları (key-value)
/// </summary>
public class AppSetting
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Key { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Value { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? Description { get; set; }
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
