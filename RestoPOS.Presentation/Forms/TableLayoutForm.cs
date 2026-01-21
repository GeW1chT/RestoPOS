using RestoPOS.Common.Enums;
using RestoPOS.Data.Entities;

namespace RestoPOS.Presentation.Forms;

/// <summary>
/// Masa düzeni formu
/// </summary>
public partial class TableLayoutForm : Form
{
    private FlowLayoutPanel pnlTables = null!;
    private Label lblTitle = null!;
    private System.Windows.Forms.Timer refreshTimer = null!;

    public TableLayoutForm()
    {
        InitializeComponent();
        LoadTables();
        StartAutoRefresh();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        this.BackColor = Color.FromArgb(35, 35, 45);

        // Title - positioned more to the right to avoid sidebar overlap
        lblTitle = new Label
        {
            Text = "Masa Durumu",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(20, 15),
            TextAlign = ContentAlignment.MiddleLeft
        };
        this.Controls.Add(lblTitle);

        // Legend panel - positioned next to title
        var pnlLegend = new FlowLayoutPanel
        {
            Location = new Point(250, 20),
            Size = new Size(600, 35),
            BackColor = Color.Transparent
        };

        AddLegendItem(pnlLegend, "Boş", Color.FromArgb(50, 180, 100));
        AddLegendItem(pnlLegend, "Dolu", Color.FromArgb(255, 165, 0));
        AddLegendItem(pnlLegend, "Ödeme Bekleniyor", Color.FromArgb(220, 80, 80));

        this.Controls.Add(pnlLegend);

        // Tables panel
        pnlTables = new FlowLayoutPanel
        {
            Location = new Point(10, 60),
            Size = new Size(this.ClientSize.Width - 20, this.ClientSize.Height - 80),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            AutoScroll = true,
            BackColor = Color.Transparent,
            Padding = new Padding(5)
        };
        this.Controls.Add(pnlTables);

        this.ResumeLayout(false);
    }

    private void AddLegendItem(FlowLayoutPanel panel, string text, Color color)
    {
        var pnl = new Panel
        {
            Size = new Size(20, 20),
            BackColor = color,
            Margin = new Padding(5, 5, 5, 5)
        };
        panel.Controls.Add(pnl);

        var lbl = new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(200, 200, 200),
            AutoSize = true,
            Margin = new Padding(0, 5, 20, 5)
        };
        panel.Controls.Add(lbl);
    }

    private async void LoadTables()
    {
        try
        {
            var tables = await Program.TableService.GetAllAsync();
            
            pnlTables.Controls.Clear();

            foreach (var table in tables.OrderBy(t => t.TableNumber))
            {
                var tableButton = CreateTableButton(table);
                pnlTables.Controls.Add(tableButton);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Masalar yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Panel CreateTableButton(RestaurantTable table)
    {
        var color = table.Status switch
        {
            TableStatus.Empty => Color.FromArgb(50, 180, 100),
            TableStatus.Occupied => Color.FromArgb(255, 165, 0),
            TableStatus.WaitingPayment => Color.FromArgb(220, 80, 80),
            _ => Color.Gray
        };

        var panel = new Panel
        {
            Size = new Size(150, 120),
            BackColor = color,
            Margin = new Padding(10),
            Cursor = Cursors.Hand,
            Tag = table
        };

        // Table number
        var lblNumber = new Label
        {
            Text = $"Masa {table.TableNumber}",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = false,
            Size = new Size(150, 35),
            Location = new Point(0, 15),
            TextAlign = ContentAlignment.MiddleCenter
        };
        panel.Controls.Add(lblNumber);

        // Capacity
        var lblCapacity = new Label
        {
            Text = $"👥 {table.Capacity} Kişilik",
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(240, 240, 240),
            AutoSize = false,
            Size = new Size(150, 25),
            Location = new Point(0, 50),
            TextAlign = ContentAlignment.MiddleCenter
        };
        panel.Controls.Add(lblCapacity);

        // Status
        var statusText = table.Status switch
        {
            TableStatus.Empty => "Boş",
            TableStatus.Occupied => "Dolu",
            TableStatus.WaitingPayment => "Ödeme Bekleniyor",
            _ => ""
        };

        var lblStatus = new Label
        {
            Text = statusText,
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(230, 230, 230),
            AutoSize = false,
            Size = new Size(150, 25),
            Location = new Point(0, 80),
            TextAlign = ContentAlignment.MiddleCenter
        };
        panel.Controls.Add(lblStatus);

        // Click events
        panel.Click += TablePanel_Click;
        foreach (Control control in panel.Controls)
        {
            control.Click += TablePanel_Click;
        }

        return panel;
    }

    private async void TablePanel_Click(object? sender, EventArgs e)
    {
        Control? control = sender as Control;
        Panel? panel = control as Panel ?? control?.Parent as Panel;
        
        if (panel?.Tag is not RestaurantTable table) return;

        // If table has active order, open order form
        var activeOrder = await Program.OrderService.GetActiveOrderByTableAsync(table.Id);

        if (activeOrder != null || table.Status != TableStatus.Empty)
        {
            var orderForm = new OrderForm(table, activeOrder);
            orderForm.ShowDialog();
            LoadTables();
        }
        else
        {
            // Create new order
            var result = MessageBox.Show($"Masa {table.TableNumber} için yeni sipariş oluşturmak istiyor musunuz?",
                "Yeni Sipariş", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var newOrder = await Program.OrderService.CreateOrderAsync(table.Id, Program.CurrentUser?.Id);
                var orderForm = new OrderForm(table, newOrder);
                orderForm.ShowDialog();
                LoadTables();
            }
        }
    }

    private void StartAutoRefresh()
    {
        refreshTimer = new System.Windows.Forms.Timer
        {
            Interval = 30000 // 30 seconds
        };
        refreshTimer.Tick += (s, e) => LoadTables();
        refreshTimer.Start();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        refreshTimer?.Stop();
        refreshTimer?.Dispose();
        base.OnFormClosing(e);
    }
}
