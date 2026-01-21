using RestoPOS.Common.Enums;

namespace RestoPOS.Presentation.Forms;

/// <summary>
/// Ana dashboard formu
/// </summary>
public partial class MainForm : Form
{
    private Panel pnlSidebar = null!;
    private Panel pnlContent = null!;
    private Label lblTitle = null!;
    private Label lblUser = null!;
    private Button btnTables = null!;
    private Button btnProducts = null!;
    private Button btnKitchen = null!;
    private Button btnReports = null!;
    private Button btnAdmin = null!;
    private Button btnLogout = null!;
    private Form? currentChildForm;

    public MainForm()
    {
        InitializeComponent();
        LoadTableLayout();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        // Form settings
        this.Text = "RestoPOS - Ana Ekran";
        this.Size = new Size(1400, 850);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(30, 30, 40);
        this.MinimumSize = new Size(1200, 700);
        this.WindowState = FormWindowState.Maximized;

        // Sidebar panel
        pnlSidebar = new Panel
        {
            Dock = DockStyle.Left,
            Width = 220,
            BackColor = Color.FromArgb(25, 25, 35)
        };

        // Title label
        lblTitle = new Label
        {
            Text = "🍽️ RestoPOS",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 165, 0),
            AutoSize = false,
            Size = new Size(220, 60),
            Location = new Point(0, 15),
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlSidebar.Controls.Add(lblTitle);

        // User info
        lblUser = new Label
        {
            Text = $"👤 {Program.CurrentUser?.FullName ?? "Kullanıcı"}",
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(180, 180, 180),
            AutoSize = false,
            Size = new Size(220, 30),
            Location = new Point(0, 75),
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlSidebar.Controls.Add(lblUser);

        // Role label
        var lblRole = new Label
        {
            Text = GetRoleName(Program.CurrentUser?.Role ?? UserRole.Waiter),
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(255, 165, 0),
            AutoSize = false,
            Size = new Size(220, 20),
            Location = new Point(0, 100),
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlSidebar.Controls.Add(lblRole);

        int btnY = 150;
        int btnHeight = 50;
        int btnMargin = 10;

        // Tables button
        btnTables = CreateMenuButton("🪑  Masalar", btnY);
        btnTables.Click += (s, e) => LoadTableLayout();
        pnlSidebar.Controls.Add(btnTables);
        btnY += btnHeight + btnMargin;

        // Products button
        btnProducts = CreateMenuButton("📦  Ürünler", btnY);
        btnProducts.Click += (s, e) => LoadProductManagement();
        pnlSidebar.Controls.Add(btnProducts);
        btnY += btnHeight + btnMargin;

        // Kitchen button (only for Kitchen and Admin)
        if (Program.CurrentUser?.Role == UserRole.Kitchen || Program.CurrentUser?.Role == UserRole.Admin)
        {
            btnKitchen = CreateMenuButton("👨‍🍳  Mutfak", btnY);
            btnKitchen.Click += (s, e) => LoadKitchenDisplay();
            pnlSidebar.Controls.Add(btnKitchen);
            btnY += btnHeight + btnMargin;
        }

        // Reports button (only for Cashier and Admin)
        if (Program.CurrentUser?.Role == UserRole.Cashier || Program.CurrentUser?.Role == UserRole.Admin)
        {
            btnReports = CreateMenuButton("📊  Raporlar", btnY);
            btnReports.Click += (s, e) => LoadReports();
            pnlSidebar.Controls.Add(btnReports);
            btnY += btnHeight + btnMargin;
        }

        // Admin button (only for Admin)
        if (Program.CurrentUser?.Role == UserRole.Admin)
        {
            btnAdmin = CreateMenuButton("⚙️  Yönetim", btnY);
            btnAdmin.Click += (s, e) => LoadAdminPanel();
            pnlSidebar.Controls.Add(btnAdmin);
            btnY += btnHeight + btnMargin;
        }

        // Logout button
        btnLogout = CreateMenuButton("🚪  Çıkış", 0);
        btnLogout.BackColor = Color.FromArgb(180, 50, 50);
        btnLogout.Dock = DockStyle.Bottom;
        btnLogout.Click += BtnLogout_Click;
        pnlSidebar.Controls.Add(btnLogout);

        this.Controls.Add(pnlSidebar);

        // Content panel
        pnlContent = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(35, 35, 45),
            Padding = new Padding(20)
        };
        this.Controls.Add(pnlContent);

        // Bring sidebar to front
        pnlSidebar.BringToFront();

        this.ResumeLayout(false);
    }

    private Button CreateMenuButton(string text, int y)
    {
        var btn = new Button
        {
            Text = text,
            Font = new Font("Segoe UI", 12),
            Size = new Size(200, 50),
            Location = new Point(10, y),
            BackColor = Color.FromArgb(45, 45, 60),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(15, 0, 0, 0)
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(60, 60, 80);
        btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(45, 45, 60);
        return btn;
    }

    private string GetRoleName(UserRole role) => role switch
    {
        UserRole.Admin => "Yönetici",
        UserRole.Waiter => "Garson",
        UserRole.Cashier => "Kasiyer",
        UserRole.Kitchen => "Mutfak",
        _ => "Kullanıcı"
    };

    private void LoadTableLayout()
    {
        pnlContent.Controls.Clear();
        var tableLayout = new TableLayoutPanel();
        tableLayout.Dock = DockStyle.Fill;
        
        var form = new TableLayoutForm();
        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(form);
        form.Show();
        currentChildForm = form;
    }

    private void LoadProductManagement()
    {
        pnlContent.Controls.Clear();
        var form = new ProductManagementForm();
        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(form);
        form.Show();
        currentChildForm = form;
    }

    private void LoadKitchenDisplay()
    {
        pnlContent.Controls.Clear();
        var form = new KitchenDisplayForm();
        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(form);
        form.Show();
        currentChildForm = form;
    }

    private void LoadReports()
    {
        pnlContent.Controls.Clear();
        var form = new ReportsForm();
        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(form);
        form.Show();
        currentChildForm = form;
    }

    private void LoadAdminPanel()
    {
        pnlContent.Controls.Clear();
        var form = new AdminPanelForm();
        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(form);
        form.Show();
        currentChildForm = form;
    }

    private void BtnLogout_Click(object? sender, EventArgs e)
    {
        var result = MessageBox.Show("Çıkış yapmak istediğinizden emin misiniz?", 
            "Çıkış", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        
        if (result == DialogResult.Yes)
        {
            Program.CurrentUser = null;
            this.Close();
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        currentChildForm?.Close();
        base.OnFormClosing(e);
    }
}
