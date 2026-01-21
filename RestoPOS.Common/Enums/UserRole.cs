namespace RestoPOS.Common.Enums;

/// <summary>
/// Kullanıcı rolleri
/// </summary>
public enum UserRole
{
    /// <summary>Yönetici - tüm yetkiler</summary>
    Admin = 0,
    
    /// <summary>Garson - sipariş alma</summary>
    Waiter = 1,
    
    /// <summary>Kasa - ödeme işlemleri</summary>
    Cashier = 2,
    
    /// <summary>Mutfak - sipariş hazırlama</summary>
    Kitchen = 3
}
