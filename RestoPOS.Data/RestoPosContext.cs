using Microsoft.EntityFrameworkCore;
using RestoPOS.Data.Entities;

namespace RestoPOS.Data;

/// <summary>
/// RestoPOS veritabanı context'i
/// </summary>
public class RestoPosContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<RestaurantTable> Tables { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<AppSetting> AppSettings { get; set; }

    private readonly string _connectionString;

    public RestoPosContext()
    {
        _connectionString = "Host=localhost;Port=5432;Database=restpos;Username=postgres;Password=1234";
    }

    public RestoPosContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public RestoPosContext(DbContextOptions<RestoPosContext> options) : base(options)
    {
        _connectionString = string.Empty;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(_connectionString))
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // RestaurantTable configuration
        modelBuilder.Entity<RestaurantTable>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TableNumber).IsUnique();
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Table)
                  .WithMany(t => t.Orders)
                  .HasForeignKey(e => e.TableId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Orders)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // OrderItem configuration
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Order)
                  .WithMany(o => o.OrderItems)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Product)
                  .WithMany(p => p.OrderItems)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Payment configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Order)
                  .WithOne(o => o.Payment)
                  .HasForeignKey<Payment>(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // AppSetting configuration
        modelBuilder.Entity<AppSetting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Key).IsUnique();
        });

        // Seed default data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Ana Yemekler", Description = "Sıcak ana yemekler", IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 2, Name = "Başlangıçlar", Description = "Aperatifler ve mezeler", IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 3, Name = "Salatalar", Description = "Taze salatalar", IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 4, Name = "İçecekler", Description = "Soğuk ve sıcak içecekler", IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 5, Name = "Tatlılar", Description = "Tatlılar ve dondurma", IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        // Seed Products
        modelBuilder.Entity<Product>().HasData(
            // Ana Yemekler
            new Product { Id = 1, Name = "Izgara Köfte", CategoryId = 1, Price = 180.00m, TaxRate = 10, Stock = 100, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 2, Name = "Tavuk Şiş", CategoryId = 1, Price = 160.00m, TaxRate = 10, Stock = 100, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 3, Name = "Kuzu Pirzola", CategoryId = 1, Price = 320.00m, TaxRate = 10, Stock = 50, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 4, Name = "Karışık Izgara", CategoryId = 1, Price = 280.00m, TaxRate = 10, Stock = 80, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            
            // Başlangıçlar
            new Product { Id = 5, Name = "Humus", CategoryId = 2, Price = 45.00m, TaxRate = 10, Stock = 100, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 6, Name = "Sigara Böreği", CategoryId = 2, Price = 55.00m, TaxRate = 10, Stock = 100, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 7, Name = "Patlıcan Salatası", CategoryId = 2, Price = 50.00m, TaxRate = 10, Stock = 100, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            
            // Salatalar
            new Product { Id = 8, Name = "Çoban Salatası", CategoryId = 3, Price = 40.00m, TaxRate = 10, Stock = 100, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 9, Name = "Sezar Salata", CategoryId = 3, Price = 75.00m, TaxRate = 10, Stock = 100, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            
            // İçecekler
            new Product { Id = 10, Name = "Ayran", CategoryId = 4, Price = 15.00m, TaxRate = 10, Stock = 200, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 11, Name = "Kola", CategoryId = 4, Price = 25.00m, TaxRate = 10, Stock = 200, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 12, Name = "Çay", CategoryId = 4, Price = 10.00m, TaxRate = 10, Stock = 500, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 13, Name = "Türk Kahvesi", CategoryId = 4, Price = 30.00m, TaxRate = 10, Stock = 300, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            
            // Tatlılar
            new Product { Id = 14, Name = "Künefe", CategoryId = 5, Price = 90.00m, TaxRate = 10, Stock = 50, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 15, Name = "Sütlaç", CategoryId = 5, Price = 45.00m, TaxRate = 10, Stock = 80, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 16, Name = "Baklava (Porsiyon)", CategoryId = 5, Price = 85.00m, TaxRate = 10, Stock = 60, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        // Seed 20 Tables
        var tables = new List<RestaurantTable>();
        for (int i = 1; i <= 20; i++)
        {
            tables.Add(new RestaurantTable 
            { 
                Id = i, 
                TableNumber = i, 
                TableName = $"Masa {i}",
                Capacity = i <= 10 ? 4 : 6,
                Status = RestoPOS.Common.Enums.TableStatus.Empty,
                IsActive = true 
            });
        }
        modelBuilder.Entity<RestaurantTable>().HasData(tables);

        // Seed Default Admin User (password: admin123)
        modelBuilder.Entity<User>().HasData(
            new User 
            { 
                Id = 1, 
                Username = "admin", 
                PasswordHash = "$2a$11$5rCFB1hBEreVW6.pCZxYjOF7CnPx3YLyMBKXvVbYnixr06g9tvDJG", // admin123
                FullName = "Sistem Yöneticisi",
                Role = RestoPOS.Common.Enums.UserRole.Admin,
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User 
            { 
                Id = 2, 
                Username = "garson1", 
                PasswordHash = "$2a$11$5rCFB1hBEreVW6.pCZxYjOF7CnPx3YLyMBKXvVbYnixr06g9tvDJG", // admin123
                FullName = "Ahmet Garson",
                Role = RestoPOS.Common.Enums.UserRole.Waiter,
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User 
            { 
                Id = 3, 
                Username = "kasa1", 
                PasswordHash = "$2a$11$5rCFB1hBEreVW6.pCZxYjOF7CnPx3YLyMBKXvVbYnixr06g9tvDJG", // admin123
                FullName = "Mehmet Kasa",
                Role = RestoPOS.Common.Enums.UserRole.Cashier,
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User 
            { 
                Id = 4, 
                Username = "mutfak1", 
                PasswordHash = "$2a$11$5rCFB1hBEreVW6.pCZxYjOF7CnPx3YLyMBKXvVbYnixr06g9tvDJG", // admin123
                FullName = "Ali Mutfak",
                Role = RestoPOS.Common.Enums.UserRole.Kitchen,
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // Seed Default Settings
        modelBuilder.Entity<AppSetting>().HasData(
            new AppSetting { Id = 1, Key = "DefaultTaxRate", Value = "10", Description = "Varsayılan KDV oranı (%)", UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new AppSetting { Id = 2, Key = "Currency", Value = "TL", Description = "Para birimi", UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new AppSetting { Id = 3, Key = "RestaurantName", Value = "RestoPOS Demo", Description = "Restoran adı", UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new AppSetting { Id = 4, Key = "AvailableTaxRates", Value = "0,1,10,20", Description = "Kullanılabilir KDV oranları", UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
