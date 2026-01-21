using RestoPOS.Business.Services;

namespace RestoPOS.Presentation.Forms;

/// <summary>
/// Raporlar formu
/// </summary>
public partial class ReportsForm : Form
{
    private DateTimePicker dtpDate = null!;
    private Button btnRefresh = null!;
    private TabControl tabReports = null!;
    private DataGridView dgvDailySales = null!;
    private DataGridView dgvProductSales = null!;
    private DataGridView dgvCategorySales = null!;
    private Panel pnlSummary = null!;

    public ReportsForm()
    {
        InitializeComponent();
        LoadReports();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        this.BackColor = Color.FromArgb(35, 35, 45);

        // Title
        var lblTitle = new Label
        {
            Text = "📊 Raporlar",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = false,
            Size = new Size(200, 50),
            Location = new Point(20, 10),
            TextAlign = ContentAlignment.MiddleLeft
        };
        this.Controls.Add(lblTitle);

        // Date picker
        var lblDate = new Label
        {
            Text = "Tarih:",
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.White,
            Location = new Point(20, 70),
            AutoSize = true
        };
        this.Controls.Add(lblDate);

        dtpDate = new DateTimePicker
        {
            Location = new Point(80, 67),
            Size = new Size(200, 30),
            Font = new Font("Segoe UI", 11),
            Format = DateTimePickerFormat.Short,
            Value = DateTime.Today
        };
        this.Controls.Add(dtpDate);

        btnRefresh = new Button
        {
            Text = "🔄 Yenile",
            Font = new Font("Segoe UI", 11),
            Size = new Size(120, 35),
            Location = new Point(300, 65),
            BackColor = Color.FromArgb(50, 180, 100),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnRefresh.FlatAppearance.BorderSize = 0;
        btnRefresh.Click += (s, e) => LoadReports();
        this.Controls.Add(btnRefresh);

        // Summary panel
        pnlSummary = new Panel
        {
            Location = new Point(20, 110),
            Size = new Size(this.ClientSize.Width - 40, 100),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            BackColor = Color.FromArgb(45, 45, 60)
        };
        this.Controls.Add(pnlSummary);

        // Tab control
        tabReports = new TabControl
        {
            Location = new Point(20, 220),
            Size = new Size(this.ClientSize.Width - 40, this.ClientSize.Height - 240),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            Font = new Font("Segoe UI", 10)
        };

        // Daily sales tab
        var tabDaily = new TabPage("Günlük Özet");
        tabDaily.BackColor = Color.FromArgb(40, 40, 55);
        dgvDailySales = CreateDataGrid();
        dgvDailySales.Dock = DockStyle.Fill;
        tabDaily.Controls.Add(dgvDailySales);
        tabReports.TabPages.Add(tabDaily);

        // Product sales tab
        var tabProducts = new TabPage("Ürün Satışları");
        tabProducts.BackColor = Color.FromArgb(40, 40, 55);
        dgvProductSales = CreateDataGrid();
        dgvProductSales.Columns.Add("Product", "Ürün");
        dgvProductSales.Columns.Add("Category", "Kategori");
        dgvProductSales.Columns.Add("Quantity", "Adet");
        dgvProductSales.Columns.Add("Revenue", "Gelir");
        dgvProductSales.Dock = DockStyle.Fill;
        tabProducts.Controls.Add(dgvProductSales);
        tabReports.TabPages.Add(tabProducts);

        // Category sales tab
        var tabCategories = new TabPage("Kategori Analizi");
        tabCategories.BackColor = Color.FromArgb(40, 40, 55);
        dgvCategorySales = CreateDataGrid();
        dgvCategorySales.Columns.Add("Category", "Kategori");
        dgvCategorySales.Columns.Add("Products", "Ürün Sayısı");
        dgvCategorySales.Columns.Add("Quantity", "Satış Adedi");
        dgvCategorySales.Columns.Add("Revenue", "Gelir");
        dgvCategorySales.Dock = DockStyle.Fill;
        tabCategories.Controls.Add(dgvCategorySales);
        tabReports.TabPages.Add(tabCategories);

        this.Controls.Add(tabReports);
        this.ResumeLayout(false);
    }

    private DataGridView CreateDataGrid()
    {
        return new DataGridView
        {
            BackgroundColor = Color.FromArgb(40, 40, 55),
            BorderStyle = BorderStyle.None,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(45, 45, 60),
                ForeColor = Color.White,
                SelectionBackColor = Color.FromArgb(70, 70, 90),
                SelectionForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Padding = new Padding(5)
            },
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(55, 55, 75),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Padding = new Padding(5)
            },
            EnableHeadersVisualStyles = false,
            RowTemplate = { Height = 35 }
        };
    }

    private async void LoadReports()
    {
        try
        {
            var date = dtpDate.Value.Date;
            var report = await Program.ReportService.GetDailySalesReportAsync(date);

            // Update summary panel
            pnlSummary.Controls.Clear();
            AddSummaryCard("💰 Toplam Gelir", $"₺{report.TotalRevenue:N2}", Color.FromArgb(50, 180, 100), 0);
            AddSummaryCard("🧾 Siparişler", $"{report.CompletedOrders}/{report.TotalOrders}", Color.FromArgb(50, 150, 200), 1);
            AddSummaryCard("💵 Nakit", $"₺{report.CashPayments:N2}", Color.FromArgb(100, 180, 100), 2);
            AddSummaryCard("💳 Kart", $"₺{report.CardPayments:N2}", Color.FromArgb(100, 150, 220), 3);
            AddSummaryCard("📋 KDV", $"₺{report.TotalTax:N2}", Color.FromArgb(255, 165, 0), 4);
            AddSummaryCard("❌ İptal", report.CancelledOrders.ToString(), Color.FromArgb(220, 80, 80), 5);

            // Load product sales
            var productReport = await Program.ReportService.GetProductSalesReportAsync(date, date);
            dgvProductSales.Rows.Clear();
            foreach (var item in productReport)
            {
                dgvProductSales.Rows.Add(item.ProductName, item.CategoryName, item.TotalQuantity, $"₺{item.TotalRevenue:N2}");
            }

            // Load category sales
            var categoryReport = await Program.ReportService.GetCategorySalesReportAsync(date, date);
            dgvCategorySales.Rows.Clear();
            foreach (var item in categoryReport)
            {
                dgvCategorySales.Rows.Add(item.CategoryName, item.TotalProducts, item.TotalQuantity, $"₺{item.TotalRevenue:N2}");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Raporlar yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AddSummaryCard(string title, string value, Color color, int index)
    {
        var width = (pnlSummary.Width - 60) / 6;
        var card = new Panel
        {
            Size = new Size(width, 80),
            Location = new Point(10 + (index * (width + 10)), 10),
            BackColor = Color.FromArgb(55, 55, 75)
        };

        var lblTitle = new Label
        {
            Text = title,
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(180, 180, 180),
            AutoSize = false,
            Size = new Size(width, 25),
            Location = new Point(0, 10),
            TextAlign = ContentAlignment.MiddleCenter
        };
        card.Controls.Add(lblTitle);

        var lblValue = new Label
        {
            Text = value,
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = color,
            AutoSize = false,
            Size = new Size(width, 35),
            Location = new Point(0, 35),
            TextAlign = ContentAlignment.MiddleCenter
        };
        card.Controls.Add(lblValue);

        pnlSummary.Controls.Add(card);
    }
}
