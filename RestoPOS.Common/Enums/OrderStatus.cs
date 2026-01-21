namespace RestoPOS.Common.Enums;

/// <summary>
/// Sipariş durumları
/// </summary>
public enum OrderStatus
{
    /// <summary>Beklemede</summary>
    Pending = 0,
    
    /// <summary>Hazırlanıyor</summary>
    Preparing = 1,
    
    /// <summary>Hazır</summary>
    Ready = 2,
    
    /// <summary>Teslim edildi</summary>
    Delivered = 3,
    
    /// <summary>Ödeme yapıldı</summary>
    Paid = 4,
    
    /// <summary>İptal edildi</summary>
    Cancelled = 5
}
