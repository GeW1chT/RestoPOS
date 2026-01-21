using RestoPOS.Common.Enums;
using RestoPOS.Data.Entities;

namespace RestoPOS.Presentation.Forms;

/// <summary>
/// Mutfak ekranı formu
/// </summary>
public partial class KitchenDisplayForm : Form
{
    private FlowLayoutPanel pnlOrders = null!;
    private Label lblTitle = null!;
    private System.Windows.Forms.Timer refreshTimer = null!;

    public KitchenDisplayForm()
    {
        InitializeComponent();
        LoadOrders();
        StartAutoRefresh();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        this.BackColor = Color.FromArgb(35, 35, 45);

        // Title
        lblTitle = new Label
        {
            Text = "Mutfak Ekranı",
            Font = new Font("Segoe UI", 24, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 165, 0),
            AutoSize = false,
            Size = new Size(500, 60),
            Location = new Point(30, 10),
            TextAlign = ContentAlignment.MiddleLeft
        };
        this.Controls.Add(lblTitle);

        // Refresh button
        var btnRefresh = new Button
        {
            Text = "🔄 Yenile",
            Font = new Font("Segoe UI", 12),
            Size = new Size(120, 40),
            Location = new Point(this.ClientSize.Width - 150, 20),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            BackColor = Color.FromArgb(60, 60, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnRefresh.FlatAppearance.BorderSize = 0;
        btnRefresh.Click += (s, e) => LoadOrders();
        this.Controls.Add(btnRefresh);

        // Orders panel
        pnlOrders = new FlowLayoutPanel
        {
            Location = new Point(20, 80),
            Size = new Size(this.ClientSize.Width - 40, this.ClientSize.Height - 100),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            AutoScroll = true,
            BackColor = Color.Transparent,
            Padding = new Padding(10)
        };
        this.Controls.Add(pnlOrders);

        this.ResumeLayout(false);
    }

    private async void LoadOrders()
    {
        try
        {
            var orders = await Program.OrderService.GetPendingKitchenOrdersAsync();
            
            pnlOrders.Controls.Clear();

            if (orders.Count == 0)
            {
                var lblEmpty = new Label
                {
                    Text = "✅ Bekleyen sipariş yok",
                    Font = new Font("Segoe UI", 16),
                    ForeColor = Color.FromArgb(100, 180, 100),
                    AutoSize = true,
                    Margin = new Padding(20)
                };
                pnlOrders.Controls.Add(lblEmpty);
                return;
            }

            foreach (var order in orders)
            {
                var orderCard = CreateOrderCard(order);
                pnlOrders.Controls.Add(orderCard);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Siparişler yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Panel CreateOrderCard(Order order)
    {
        var color = order.Status switch
        {
            OrderStatus.Pending => Color.FromArgb(220, 120, 50),
            OrderStatus.Preparing => Color.FromArgb(50, 150, 200),
            _ => Color.Gray
        };

        var panel = new Panel
        {
            Size = new Size(320, 300),
            BackColor = Color.FromArgb(50, 50, 65),
            Margin = new Padding(10),
            Tag = order
        };

        // Header with table number
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = color
        };

        var lblTable = new Label
        {
            Text = $"🪑 Masa {order.Table.TableNumber}",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = false,
            Size = new Size(200, 50),
            Location = new Point(10, 0),
            TextAlign = ContentAlignment.MiddleLeft
        };
        pnlHeader.Controls.Add(lblTable);

        var lblTime = new Label
        {
            Text = GetTimeAgo(order.CreatedAt),
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(230, 230, 230),
            AutoSize = false,
            Size = new Size(100, 50),
            Location = new Point(210, 0),
            TextAlign = ContentAlignment.MiddleRight
        };
        pnlHeader.Controls.Add(lblTime);

        panel.Controls.Add(pnlHeader);

        // Items list
        var pnlItems = new Panel
        {
            Location = new Point(0, 50),
            Size = new Size(320, 180),
            AutoScroll = true,
            BackColor = Color.Transparent
        };

        int itemY = 10;
        foreach (var item in order.OrderItems)
        {
            var lblItem = new Label
            {
                Text = $"• {item.Quantity}x {item.Product?.Name ?? "Ürün"}",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.White,
                Location = new Point(15, itemY),
                AutoSize = true
            };
            pnlItems.Controls.Add(lblItem);
            itemY += 25;

            if (!string.IsNullOrEmpty(item.Note))
            {
                var lblNote = new Label
                {
                    Text = $"   📝 {item.Note}",
                    Font = new Font("Segoe UI", 9, FontStyle.Italic),
                    ForeColor = Color.FromArgb(255, 200, 100),
                    Location = new Point(15, itemY),
                    AutoSize = true
                };
                pnlItems.Controls.Add(lblNote);
                itemY += 22;
            }
        }

        panel.Controls.Add(pnlItems);

        // Action buttons
        var pnlActions = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 60,
            BackColor = Color.FromArgb(40, 40, 55)
        };

        if (order.Status == OrderStatus.Pending)
        {
            var btnStart = new Button
            {
                Text = "🔥 Hazırla",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(140, 45),
                Location = new Point(10, 7),
                BackColor = Color.FromArgb(50, 150, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Tag = order
            };
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.Click += BtnStart_Click;
            pnlActions.Controls.Add(btnStart);
        }

        var btnReady = new Button
        {
            Text = "✅ Hazır",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Size = new Size(140, 45),
            Location = new Point(order.Status == OrderStatus.Pending ? 160 : 90, 7),
            BackColor = Color.FromArgb(50, 180, 100),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Tag = order
        };
        btnReady.FlatAppearance.BorderSize = 0;
        btnReady.Click += BtnReady_Click;
        pnlActions.Controls.Add(btnReady);

        panel.Controls.Add(pnlActions);

        return panel;
    }

    private string GetTimeAgo(DateTime dateTime)
    {
        var diff = DateTime.UtcNow - dateTime;
        if (diff.TotalMinutes < 1) return "Az önce";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} dk";
        return $"{(int)diff.TotalHours} sa {diff.Minutes} dk";
    }

    private async void BtnStart_Click(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.Tag is Order order)
        {
            await Program.OrderService.UpdateStatusAsync(order.Id, OrderStatus.Preparing);
            LoadOrders();
        }
    }

    private async void BtnReady_Click(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.Tag is Order order)
        {
            await Program.OrderService.UpdateStatusAsync(order.Id, OrderStatus.Ready);
            LoadOrders();
        }
    }

    private void StartAutoRefresh()
    {
        refreshTimer = new System.Windows.Forms.Timer
        {
            Interval = 15000 // 15 seconds
        };
        refreshTimer.Tick += (s, e) => LoadOrders();
        refreshTimer.Start();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        refreshTimer?.Stop();
        refreshTimer?.Dispose();
        base.OnFormClosing(e);
    }
}
