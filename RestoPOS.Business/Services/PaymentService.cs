using Microsoft.EntityFrameworkCore;
using RestoPOS.Common.Enums;
using RestoPOS.Data;
using RestoPOS.Data.Entities;

namespace RestoPOS.Business.Services;

/// <summary>
/// Ödeme işlemleri servisi
/// </summary>
public class PaymentService
{
    private readonly Func<RestoPosContext> _contextFactory;
    private static int _invoiceCounter = 1;

    public PaymentService(Func<RestoPosContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<Payment?> ProcessPaymentAsync(int orderId, PaymentMethod method, decimal receivedAmount, decimal discountAmount = 0)
    {
        using var context = _contextFactory();
        var order = await context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) return null;

        var subTotal = order.SubTotal;
        var taxAmount = order.TaxAmount;
        var totalAfterDiscount = order.TotalAmount - discountAmount;

        if (receivedAmount < totalAfterDiscount)
        {
            throw new InvalidOperationException($"Yetersiz ödeme. Toplam: {totalAfterDiscount:C}, Alınan: {receivedAmount:C}");
        }

        var payment = new Payment
        {
            OrderId = orderId,
            SubTotal = subTotal,
            TaxAmount = taxAmount,
            DiscountAmount = discountAmount,
            TotalAmount = totalAfterDiscount,
            ReceivedAmount = receivedAmount,
            ChangeAmount = receivedAmount - totalAfterDiscount,
            Method = method,
            InvoiceNumber = GenerateInvoiceNumber(),
            PaidAt = DateTime.UtcNow
        };

        context.Payments.Add(payment);
        
        // Update order status
        order.Status = OrderStatus.Paid;
        order.UpdatedAt = DateTime.UtcNow;
        
        // Update table status
        var table = await context.Tables.FindAsync(order.TableId);
        if (table != null)
        {
            table.Status = TableStatus.Empty;
        }
        
        await context.SaveChangesAsync();

        return payment;
    }

    public async Task<Payment?> GetByOrderIdAsync(int orderId)
    {
        using var context = _contextFactory();
        return await context.Payments
            .Include(p => p.Order)
                .ThenInclude(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
            .Include(p => p.Order)
                .ThenInclude(o => o.Table)
            .FirstOrDefaultAsync(p => p.OrderId == orderId);
    }

    public async Task<List<Payment>> GetPaymentsByDateAsync(DateTime date)
    {
        using var context = _contextFactory();
        var startDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        var endDate = startDate.AddDays(1);

        return await context.Payments
            .Include(p => p.Order)
                .ThenInclude(o => o.Table)
            .Where(p => p.PaidAt >= startDate && p.PaidAt < endDate)
            .OrderBy(p => p.PaidAt)
            .ToListAsync();
    }

    public async Task<decimal> GetDailyTotalAsync(DateTime date)
    {
        var payments = await GetPaymentsByDateAsync(date);
        return payments.Sum(p => p.TotalAmount);
    }

    public async Task<(decimal cash, decimal card)> GetDailyTotalByMethodAsync(DateTime date)
    {
        var payments = await GetPaymentsByDateAsync(date);
        var cash = payments.Where(p => p.Method == PaymentMethod.Cash).Sum(p => p.TotalAmount);
        var card = payments.Where(p => p.Method == PaymentMethod.CreditCard).Sum(p => p.TotalAmount);
        return (cash, card);
    }

    private string GenerateInvoiceNumber()
    {
        var date = DateTime.Now;
        var number = System.Threading.Interlocked.Increment(ref _invoiceCounter);
        return $"INV-{date:yyyyMMdd}-{number:D4}";
    }
}
