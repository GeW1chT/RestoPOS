using RestoPOS.Business.Services;
using RestoPOS.Data;
using Microsoft.EntityFrameworkCore;

namespace RestoPOS.Presentation;

internal static class Program
{
    private static readonly string ConnectionString = "Host=localhost;Port=5432;Database=restpos;Username=postgres;Password=1234";
    
    // Factory method to create new context for each operation
    public static RestoPosContext CreateDbContext() => new RestoPosContext(ConnectionString);
    
    public static UserService UserService { get; private set; } = null!;
    public static TableService TableService { get; private set; } = null!;
    public static ProductService ProductService { get; private set; } = null!;
    public static CategoryService CategoryService { get; private set; } = null!;
    public static OrderService OrderService { get; private set; } = null!;
    public static PaymentService PaymentService { get; private set; } = null!;
    public static ReportService ReportService { get; private set; } = null!;
    public static SettingsService SettingsService { get; private set; } = null!;
    
    public static Data.Entities.User? CurrentUser { get; set; }

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        
        // Ensure database is created and seeded
        using (var context = CreateDbContext())
        {
            context.Database.EnsureCreated();
        }
        
        // Initialize services with factory method
        CategoryService = new CategoryService(CreateDbContext);
        ProductService = new ProductService(CreateDbContext);
        TableService = new TableService(CreateDbContext);
        OrderService = new OrderService(CreateDbContext);
        PaymentService = new PaymentService(CreateDbContext);
        UserService = new UserService(CreateDbContext);
        ReportService = new ReportService(CreateDbContext);
        SettingsService = new SettingsService(CreateDbContext);
        
        // Start with login form
        Application.Run(new Forms.LoginForm());
    }
}