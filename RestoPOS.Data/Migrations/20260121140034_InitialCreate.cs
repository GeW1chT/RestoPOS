using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RestoPOS.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TableNumber = table.Column<int>(type: "integer", nullable: false),
                    TableName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false),
                    ImagePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TaxRate = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TableId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SubTotal = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    TaxRate = table.Column<int>(type: "integer", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    SubTotal = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    ReceivedAmount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    ChangeAmount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AppSettings",
                columns: new[] { "Id", "Description", "Key", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { 1, "Varsayılan KDV oranı (%)", "DefaultTaxRate", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "10" },
                    { 2, "Para birimi", "Currency", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TL" },
                    { 3, "Restoran adı", "RestaurantName", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "RestoPOS Demo" },
                    { 4, "Kullanılabilir KDV oranları", "AvailableTaxRates", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "0,1,10,20" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sıcak ana yemekler", true, "Ana Yemekler" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Aperatifler ve mezeler", true, "Başlangıçlar" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Taze salatalar", true, "Salatalar" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Soğuk ve sıcak içecekler", true, "İçecekler" },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tatlılar ve dondurma", true, "Tatlılar" }
                });

            migrationBuilder.InsertData(
                table: "Tables",
                columns: new[] { "Id", "Capacity", "IsActive", "Status", "TableName", "TableNumber" },
                values: new object[,]
                {
                    { 1, 4, true, 0, "Masa 1", 1 },
                    { 2, 4, true, 0, "Masa 2", 2 },
                    { 3, 4, true, 0, "Masa 3", 3 },
                    { 4, 4, true, 0, "Masa 4", 4 },
                    { 5, 4, true, 0, "Masa 5", 5 },
                    { 6, 4, true, 0, "Masa 6", 6 },
                    { 7, 4, true, 0, "Masa 7", 7 },
                    { 8, 4, true, 0, "Masa 8", 8 },
                    { 9, 4, true, 0, "Masa 9", 9 },
                    { 10, 4, true, 0, "Masa 10", 10 },
                    { 11, 6, true, 0, "Masa 11", 11 },
                    { 12, 6, true, 0, "Masa 12", 12 },
                    { 13, 6, true, 0, "Masa 13", 13 },
                    { 14, 6, true, 0, "Masa 14", 14 },
                    { 15, 6, true, 0, "Masa 15", 15 },
                    { 16, 6, true, 0, "Masa 16", 16 },
                    { 17, 6, true, 0, "Masa 17", 17 },
                    { 18, 6, true, 0, "Masa 18", 18 },
                    { 19, 6, true, 0, "Masa 19", 19 },
                    { 20, 6, true, 0, "Masa 20", 20 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "FullName", "IsActive", "LastLoginAt", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sistem Yöneticisi", true, null, "$2a$11$5rCFB1hBEreVW6.pCZxYjOF7CnPx3YLyMBKXvVbYnixr06g9tvDJG", 0, "admin" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ahmet Garson", true, null, "$2a$11$5rCFB1hBEreVW6.pCZxYjOF7CnPx3YLyMBKXvVbYnixr06g9tvDJG", 1, "garson1" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mehmet Kasa", true, null, "$2a$11$5rCFB1hBEreVW6.pCZxYjOF7CnPx3YLyMBKXvVbYnixr06g9tvDJG", 2, "kasa1" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ali Mutfak", true, null, "$2a$11$5rCFB1hBEreVW6.pCZxYjOF7CnPx3YLyMBKXvVbYnixr06g9tvDJG", 3, "mutfak1" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "ImagePath", "IsActive", "Name", "Price", "Stock", "TaxRate" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, "Izgara Köfte", 180.00m, 100, 10 },
                    { 2, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, "Tavuk Şiş", 160.00m, 100, 10 },
                    { 3, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, "Kuzu Pirzola", 320.00m, 50, 10 },
                    { 4, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, "Karışık Izgara", 280.00m, 80, 10 },
                    { 5, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, "Humus", 45.00m, 100, 10 },
                    { 6, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, "Sigara Böreği", 55.00m, 100, 10 },
                    { 7, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, "Patlıcan Salatası", 50.00m, 100, 10 },
                    { 8, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, "Çoban Salatası", 40.00m, 100, 10 },
                    { 9, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, "Sezar Salata", 75.00m, 100, 10 },
                    { 10, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, "Ayran", 15.00m, 200, 10 },
                    { 11, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, "Kola", 25.00m, 200, 10 },
                    { 12, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, "Çay", 10.00m, 500, 10 },
                    { 13, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, "Türk Kahvesi", 30.00m, 300, 10 },
                    { 14, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, "Künefe", 90.00m, 50, 10 },
                    { 15, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, "Sütlaç", 45.00m, 80, 10 },
                    { 16, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, "Baklava (Porsiyon)", 85.00m, 60, 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_Key",
                table: "AppSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TableId",
                table: "Orders",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_TableNumber",
                table: "Tables",
                column: "TableNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
