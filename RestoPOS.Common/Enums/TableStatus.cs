namespace RestoPOS.Common.Enums;

/// <summary>
/// Masa durumları
/// </summary>
public enum TableStatus
{
    /// <summary>Boş masa</summary>
    Empty = 0,
    
    /// <summary>Dolu masa - sipariş var</summary>
    Occupied = 1,
    
    /// <summary>Ödeme bekleniyor</summary>
    WaitingPayment = 2
}
