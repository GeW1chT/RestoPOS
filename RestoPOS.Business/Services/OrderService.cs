using Microsoft.EntityFrameworkCore;
using RestoPOS.Common.Enums;
using RestoPOS.Data;
using RestoPOS.Data.Entities;

namespace RestoPOS.Business.Services;

/// <summary>
/// Sipariş yönetimi servisi
/// </summary>
public class OrderService
{
    private readonly Func<RestoPosContext> _contextFactory;

    public OrderService(Func<RestoPosContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Order>> GetActiveOrdersAsync()
    {
        using var context = _contextFactory();
        return await context.Orders
            .Include(o => o.Table)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.Status != OrderStatus.Paid && o.Status != OrderStatus.Cancelled)
            .OrderBy(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Order>> GetPendingKitchenOrdersAsync()
    {
        using var context = _contextFactory();
        return await context.Orders
            .Include(o => o.Table)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Preparing)
            .OrderBy(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        using var context = _contextFactory();
        return await context.Orders
            .Include(o => o.Table)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order?> GetActiveOrderByTableAsync(int tableId)
    {
        using var context = _contextFactory();
        return await context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.TableId == tableId && 
                o.Status != OrderStatus.Paid && 
                o.Status != OrderStatus.Cancelled);
    }

    public async Task<Order> CreateOrderAsync(int tableId, int? userId = null, string? notes = null)
    {
        using var context = _contextFactory();
        var order = new Order
        {
            TableId = tableId,
            UserId = userId,
            Status = OrderStatus.Pending,
            Notes = notes,
            SubTotal = 0,
            TaxAmount = 0,
            TotalAmount = 0,
            CreatedAt = DateTime.UtcNow
        };

        context.Orders.Add(order);
        
        // Update table status
        var table = await context.Tables.FindAsync(tableId);
        if (table != null)
        {
            table.Status = TableStatus.Occupied;
        }
        
        await context.SaveChangesAsync();
        return order;
    }

    public async Task<OrderItem?> AddItemAsync(int orderId, int productId, int quantity = 1, string? note = null)
    {
        using var context = _contextFactory();
        var order = await context.Orders.FindAsync(orderId);
        var product = await context.Products.FindAsync(productId);
        
        if (order == null || product == null) return null;

        // Mevcut item varsa güncelle
        var existingItem = await context.OrderItems
            .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.ProductId == productId && oi.Note == note);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            existingItem.TaxAmount = (existingItem.UnitPrice * existingItem.Quantity * existingItem.TaxRate) / 100;
            existingItem.TotalPrice = (existingItem.UnitPrice * existingItem.Quantity) + existingItem.TaxAmount;
        }
        else
        {
            var taxAmount = (product.Price * quantity * product.TaxRate) / 100;
            existingItem = new OrderItem
            {
                OrderId = orderId,
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = product.Price,
                TaxRate = product.TaxRate,
                TaxAmount = taxAmount,
                TotalPrice = (product.Price * quantity) + taxAmount,
                Note = note,
                CreatedAt = DateTime.UtcNow
            };
            context.OrderItems.Add(existingItem);
        }

        await context.SaveChangesAsync();
        await RecalculateOrderTotalInternalAsync(context, orderId);
        
        return existingItem;
    }

    public async Task<bool> RemoveItemAsync(int orderItemId)
    {
        using var context = _contextFactory();
        var item = await context.OrderItems.FindAsync(orderItemId);
        if (item == null) return false;

        var orderId = item.OrderId;
        context.OrderItems.Remove(item);
        await context.SaveChangesAsync();
        await RecalculateOrderTotalInternalAsync(context, orderId);
        return true;
    }

    public async Task<bool> UpdateItemQuantityAsync(int orderItemId, int newQuantity)
    {
        using var context = _contextFactory();
        var item = await context.OrderItems
            .Include(oi => oi.Product)
            .FirstOrDefaultAsync(oi => oi.Id == orderItemId);
            
        if (item == null) return false;

        if (newQuantity <= 0)
        {
            context.OrderItems.Remove(item);
            await context.SaveChangesAsync();
            await RecalculateOrderTotalInternalAsync(context, item.OrderId);
            return true;
        }

        item.Quantity = newQuantity;
        item.TaxAmount = (item.UnitPrice * item.Quantity * item.TaxRate) / 100;
        item.TotalPrice = (item.UnitPrice * item.Quantity) + item.TaxAmount;
        
        await context.SaveChangesAsync();
        await RecalculateOrderTotalInternalAsync(context, item.OrderId);
        return true;
    }

    private async Task RecalculateOrderTotalInternalAsync(RestoPosContext context, int orderId)
    {
        var order = await context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) return;

        order.SubTotal = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);
        order.TaxAmount = order.OrderItems.Sum(oi => oi.TaxAmount);
        order.TotalAmount = order.SubTotal + order.TaxAmount;
        order.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }

    public async Task RecalculateOrderTotalAsync(int orderId)
    {
        using var context = _contextFactory();
        await RecalculateOrderTotalInternalAsync(context, orderId);
    }

    public async Task<bool> UpdateStatusAsync(int orderId, OrderStatus status)
    {
        using var context = _contextFactory();
        var order = await context.Orders.FindAsync(orderId);
        if (order == null) return false;

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        if (status == OrderStatus.Paid || status == OrderStatus.Cancelled)
        {
            var table = await context.Tables.FindAsync(order.TableId);
            if (table != null) table.Status = TableStatus.Empty;
        }
        else if (status == OrderStatus.Delivered)
        {
            var table = await context.Tables.FindAsync(order.TableId);
            if (table != null) table.Status = TableStatus.WaitingPayment;
        }

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelOrderAsync(int orderId)
    {
        return await UpdateStatusAsync(orderId, OrderStatus.Cancelled);
    }

    public async Task<List<Order>> GetOrdersByDateAsync(DateTime date)
    {
        using var context = _contextFactory();
        var startDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        var endDate = startDate.AddDays(1);

        return await context.Orders
            .Include(o => o.Table)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.Payment)
            .Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate)
            .OrderBy(o => o.CreatedAt)
            .ToListAsync();
    }
}
