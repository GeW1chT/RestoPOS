using Microsoft.EntityFrameworkCore;
using RestoPOS.Common.Enums;
using RestoPOS.Data;
using RestoPOS.Data.Entities;

namespace RestoPOS.Business.Services;

/// <summary>
/// Raporlama servisi
/// </summary>
public class ReportService
{
    private readonly Func<RestoPosContext> _contextFactory;

    public ReportService(Func<RestoPosContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    /// <summary>
    /// Günlük satış raporu
    /// </summary>
    public async Task<DailySalesReport> GetDailySalesReportAsync(DateTime date)
    {
        using var context = _contextFactory();
        var startDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        var endDate = startDate.AddDays(1);

        var payments = await context.Payments
            .Include(p => p.Order)
            .Where(p => p.PaidAt >= startDate && p.PaidAt < endDate)
            .ToListAsync();

        var orders = await context.Orders
            .Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate)
            .ToListAsync();

        return new DailySalesReport
        {
            Date = date,
            TotalOrders = orders.Count,
            CompletedOrders = orders.Count(o => o.Status == OrderStatus.Paid),
            CancelledOrders = orders.Count(o => o.Status == OrderStatus.Cancelled),
            TotalRevenue = payments.Sum(p => p.TotalAmount),
            TotalTax = payments.Sum(p => p.TaxAmount),
            TotalDiscount = payments.Sum(p => p.DiscountAmount),
            CashPayments = payments.Where(p => p.Method == PaymentMethod.Cash).Sum(p => p.TotalAmount),
            CardPayments = payments.Where(p => p.Method == PaymentMethod.CreditCard).Sum(p => p.TotalAmount)
        };
    }

    /// <summary>
    /// Ürün satış analizi
    /// </summary>
    public async Task<List<ProductSalesReport>> GetProductSalesReportAsync(DateTime startDate, DateTime endDate)
    {
        using var context = _contextFactory();
        var start = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
        var end = DateTime.SpecifyKind(endDate.Date, DateTimeKind.Utc).AddDays(1);

        var orderItems = await context.OrderItems
            .Include(oi => oi.Product)
                .ThenInclude(p => p.Category)
            .Include(oi => oi.Order)
            .Where(oi => oi.Order.Status == OrderStatus.Paid && 
                        oi.CreatedAt >= start && oi.CreatedAt < end)
            .ToListAsync();

        return orderItems
            .GroupBy(oi => oi.ProductId)
            .Select(g => new ProductSalesReport
            {
                ProductId = g.Key,
                ProductName = g.First().Product.Name,
                CategoryName = g.First().Product.Category.Name,
                TotalQuantity = g.Sum(oi => oi.Quantity),
                TotalRevenue = g.Sum(oi => oi.TotalPrice),
                AveragePrice = g.Average(oi => oi.UnitPrice)
            })
            .OrderByDescending(p => p.TotalRevenue)
            .ToList();
    }

    /// <summary>
    /// Kategori satış analizi
    /// </summary>
    public async Task<List<CategorySalesReport>> GetCategorySalesReportAsync(DateTime startDate, DateTime endDate)
    {
        var productReport = await GetProductSalesReportAsync(startDate, endDate);

        return productReport
            .GroupBy(p => p.CategoryName)
            .Select(g => new CategorySalesReport
            {
                CategoryName = g.Key,
                TotalProducts = g.Count(),
                TotalQuantity = g.Sum(p => p.TotalQuantity),
                TotalRevenue = g.Sum(p => p.TotalRevenue)
            })
            .OrderByDescending(c => c.TotalRevenue)
            .ToList();
    }

    /// <summary>
    /// Saatlik satış dağılımı
    /// </summary>
    public async Task<List<HourlySalesReport>> GetHourlySalesReportAsync(DateTime date)
    {
        using var context = _contextFactory();
        var startDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        var endDate = startDate.AddDays(1);

        var payments = await context.Payments
            .Where(p => p.PaidAt >= startDate && p.PaidAt < endDate)
            .ToListAsync();

        return Enumerable.Range(0, 24)
            .Select(hour => new HourlySalesReport
            {
                Hour = hour,
                OrderCount = payments.Count(p => p.PaidAt.Hour == hour),
                TotalRevenue = payments.Where(p => p.PaidAt.Hour == hour).Sum(p => p.TotalAmount)
            })
            .ToList();
    }
}

#region Report Models

public class DailySalesReport
{
    public DateTime Date { get; set; }
    public int TotalOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int CancelledOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal CashPayments { get; set; }
    public decimal CardPayments { get; set; }
}

public class ProductSalesReport
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AveragePrice { get; set; }
}

public class CategorySalesReport
{
    public string CategoryName { get; set; } = string.Empty;
    public int TotalProducts { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class HourlySalesReport
{
    public int Hour { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalRevenue { get; set; }
}

#endregion
