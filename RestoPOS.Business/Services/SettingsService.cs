using Microsoft.EntityFrameworkCore;
using RestoPOS.Data;
using RestoPOS.Data.Entities;

namespace RestoPOS.Business.Services;

/// <summary>
/// Sistem ayarları servisi
/// </summary>
public class SettingsService
{
    private readonly Func<RestoPosContext> _contextFactory;

    public SettingsService(Func<RestoPosContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<string?> GetValueAsync(string key)
    {
        using var context = _contextFactory();
        var setting = await context.AppSettings.FirstOrDefaultAsync(s => s.Key == key);
        return setting?.Value;
    }

    public async Task<int> GetIntValueAsync(string key, int defaultValue = 0)
    {
        var value = await GetValueAsync(key);
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    public async Task<decimal> GetDecimalValueAsync(string key, decimal defaultValue = 0)
    {
        var value = await GetValueAsync(key);
        return decimal.TryParse(value, out var result) ? result : defaultValue;
    }

    public async Task<List<int>> GetAvailableTaxRatesAsync()
    {
        var value = await GetValueAsync("AvailableTaxRates");
        if (string.IsNullOrEmpty(value)) return new List<int> { 0, 1, 10, 20 };

        return value.Split(',')
            .Where(v => int.TryParse(v.Trim(), out _))
            .Select(v => int.Parse(v.Trim()))
            .ToList();
    }

    public async Task<bool> SetValueAsync(string key, string value, string? description = null)
    {
        using var context = _contextFactory();
        var setting = await context.AppSettings.FirstOrDefaultAsync(s => s.Key == key);
        
        if (setting == null)
        {
            setting = new AppSetting
            {
                Key = key,
                Value = value,
                Description = description,
                UpdatedAt = DateTime.UtcNow
            };
            context.AppSettings.Add(setting);
        }
        else
        {
            setting.Value = value;
            if (description != null) setting.Description = description;
            setting.UpdatedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<AppSetting>> GetAllAsync()
    {
        using var context = _contextFactory();
        return await context.AppSettings
            .OrderBy(s => s.Key)
            .ToListAsync();
    }

    public async Task<string> GetRestaurantNameAsync()
    {
        return await GetValueAsync("RestaurantName") ?? "RestoPOS";
    }

    public async Task<string> GetCurrencyAsync()
    {
        return await GetValueAsync("Currency") ?? "TL";
    }

    public async Task<int> GetDefaultTaxRateAsync()
    {
        return await GetIntValueAsync("DefaultTaxRate", 10);
    }
}
