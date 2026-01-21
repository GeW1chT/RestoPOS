using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using RestoPOS.Common.Enums;
using RestoPOS.Data;
using RestoPOS.Data.Entities;

namespace RestoPOS.Business.Services;

/// <summary>
/// Kullanıcı yönetimi servisi
/// </summary>
public class UserService
{
    private readonly Func<RestoPosContext> _contextFactory;

    public UserService(Func<RestoPosContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<User?> LoginAsync(string username, string password)
    {
        using var context = _contextFactory();
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

        if (user == null) return null;

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return null;
        }

        user.LastLoginAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<List<User>> GetAllAsync()
    {
        using var context = _contextFactory();
        return await context.Users
            .Where(u => u.IsActive)
            .OrderBy(u => u.FullName)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var context = _contextFactory();
        return await context.Users.FindAsync(id);
    }

    public async Task<List<User>> GetByRoleAsync(UserRole role)
    {
        using var context = _contextFactory();
        return await context.Users
            .Where(u => u.Role == role && u.IsActive)
            .OrderBy(u => u.FullName)
            .ToListAsync();
    }

    public async Task<User> CreateAsync(string username, string password, string fullName, UserRole role)
    {
        using var context = _contextFactory();
        var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Bu kullanıcı adı zaten kullanımda.");
        }

        var user = new User
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            FullName = fullName,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> UpdateAsync(int id, string fullName, UserRole role)
    {
        using var context = _contextFactory();
        var user = await context.Users.FindAsync(id);
        if (user == null) return null;

        user.FullName = fullName;
        user.Role = role;
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> ChangePasswordAsync(int id, string currentPassword, string newPassword)
    {
        using var context = _contextFactory();
        var user = await context.Users.FindAsync(id);
        if (user == null) return false;

        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
        {
            throw new InvalidOperationException("Mevcut şifre yanlış.");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ResetPasswordAsync(int id, string newPassword)
    {
        using var context = _contextFactory();
        var user = await context.Users.FindAsync(id);
        if (user == null) return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var context = _contextFactory();
        var user = await context.Users.FindAsync(id);
        if (user == null) return false;

        user.IsActive = false;
        await context.SaveChangesAsync();
        return true;
    }
}
