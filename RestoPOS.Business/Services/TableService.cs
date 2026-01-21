using Microsoft.EntityFrameworkCore;
using RestoPOS.Common.Enums;
using RestoPOS.Data;
using RestoPOS.Data.Entities;

namespace RestoPOS.Business.Services;

/// <summary>
/// Masa yönetimi servisi
/// </summary>
public class TableService
{
    private readonly Func<RestoPosContext> _contextFactory;

    public TableService(Func<RestoPosContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<RestaurantTable>> GetAllAsync()
    {
        using var context = _contextFactory();
        return await context.Tables
            .Where(t => t.IsActive)
            .OrderBy(t => t.TableNumber)
            .ToListAsync();
    }

    public async Task<RestaurantTable?> GetByIdAsync(int id)
    {
        using var context = _contextFactory();
        return await context.Tables.FindAsync(id);
    }

    public async Task<RestaurantTable?> GetByNumberAsync(int tableNumber)
    {
        using var context = _contextFactory();
        return await context.Tables
            .FirstOrDefaultAsync(t => t.TableNumber == tableNumber && t.IsActive);
    }

    public async Task<List<RestaurantTable>> GetAvailableAsync()
    {
        using var context = _contextFactory();
        return await context.Tables
            .Where(t => t.IsActive && t.Status == TableStatus.Empty)
            .OrderBy(t => t.TableNumber)
            .ToListAsync();
    }

    public async Task<bool> UpdateStatusAsync(int id, TableStatus status)
    {
        using var context = _contextFactory();
        var table = await context.Tables.FindAsync(id);
        if (table == null) return false;

        table.Status = status;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<RestaurantTable> CreateAsync(int tableNumber, int capacity = 4, string? tableName = null)
    {
        using var context = _contextFactory();
        var table = new RestaurantTable
        {
            TableNumber = tableNumber,
            TableName = tableName ?? $"Masa {tableNumber}",
            Capacity = capacity,
            Status = TableStatus.Empty,
            IsActive = true
        };

        context.Tables.Add(table);
        await context.SaveChangesAsync();
        return table;
    }

    public async Task<RestaurantTable?> UpdateAsync(int id, int tableNumber, int capacity, string? tableName)
    {
        using var context = _contextFactory();
        var table = await context.Tables.FindAsync(id);
        if (table == null) return null;

        table.TableNumber = tableNumber;
        table.Capacity = capacity;
        table.TableName = tableName;
        await context.SaveChangesAsync();
        return table;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var context = _contextFactory();
        var table = await context.Tables.FindAsync(id);
        if (table == null) return false;

        table.IsActive = false;
        await context.SaveChangesAsync();
        return true;
    }
}
