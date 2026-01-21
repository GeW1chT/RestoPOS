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
    private Button btnToggle = null!;
    private Button btnTables = null!;
    private Button btnProducts = null!;
    private Button btnKitchen = null!;
    private Button btnReports = null!;
    private Button btnAdmin = null!;
    private Button btnLogout = null!;
    private Form? currentChildForm;
    
    private bool isSidebarExpanded = true;
    private const int SIDEBAR_EXPANDED_WIDTH = 200;
    private const int SIDEBAR_COLLAPSED_WIDTH = 60;

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
            Width = SIDEBAR_EXPANDED_WIDTH,
            BackColor = Color.FromArgb(25, 25, 35)
        };

        // Toggle button (hamburger menu)
        btnToggle = new Button
        {
            Text = "☰",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Size = new Size(50, 50),
            Location = new Point(5, 5),
            BackColor = Color.FromArgb(45, 45, 60),
            ForeColor = Color.FromArgb(255, 165, 0),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnToggle.FlatAppearance.BorderSize = 0;
        btnToggle.Click += BtnToggle_Click;
        pnlSidebar.Controls.Add(btnToggle);

        // Title label
        lblTitle = new Label
        {
            Text = "RestoPOS",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 165, 0),
            AutoSize = false,
            Size = new Size(130, 50),
            Location = new Point(60, 5),
            TextAlign = ContentAlignment.MiddleLeft
        };
        pnlSidebar.Controls.Add(lblTitle);

        // User info
        lblUser = new Label
        {
            Text = Program.CurrentUser?.FullName ?? "Kullanıcı",
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(180, 180, 180),
            AutoSize = false,
            Size = new Size(190, 20),
            Location = new Point(5, 60),
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlSidebar.Controls.Add(lblUser);

        // Role label
        var lblRole = new Label
        {
            Name = "lblRole",
            Text = GetRoleName(Program.CurrentUser?.Role ?? UserRole.Waiter),
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(255, 165, 0),
            AutoSize = false,
            Size = new Size(190, 18),
            Location = new Point(5, 80),
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlSidebar.Controls.Add(lblRole);

        int btnY = 110;
        int btnHeight = 45;
        int btnMargin = 8;

        // Tables button
        btnTables = CreateMenuButton("Masalar", "🪑", btnY);
        btnTables.Click += (s, e) => LoadTableLayout();
        pnlSidebar.Controls.Add(btnTables);
        btnY += btnHeight + btnMargin;

        // Products button
        btnProducts = CreateMenuButton("Ürünler", "📦", btnY);
        btnProducts.Click += (s, e) => LoadProductManagement();
        pnlSidebar.Controls.Add(btnProducts);
        btnY += btnHeight + btnMargin;

        // Kitchen button (only for Kitchen and Admin)
        if (Program.CurrentUser?.Role == UserRole.Kitchen || Program.CurrentUser?.Role == UserRole.Admin)
        {
            btnKitchen = CreateMenuButton("Mutfak", "👨‍🍳", btnY);
            btnKitchen.Click += (s, e) => LoadKitchenDisplay();
            pnlSidebar.Controls.Add(btnKitchen);
            btnY += btnHeight + btnMargin;
        }

        // Reports button (only for Cashier and Admin)
        if (Program.CurrentUser?.Role == UserRole.Cashier || Program.CurrentUser?.Role == UserRole.Admin)
        {
            btnReports = CreateMenuButton("Raporlar", "📊", btnY);
            btnReports.Click += (s, e) => LoadReports();
            pnlSidebar.Controls.Add(btnReports);
            btnY += btnHeight + btnMargin;
        }

        // Admin button (only for Admin)
        if (Program.CurrentUser?.Role == UserRole.Admin)
        {
            btnAdmin = CreateMenuButton("Yönetim", "⚙️", btnY);
            btnAdmin.Click += (s, e) => LoadAdminPanel();
            pnlSidebar.Controls.Add(btnAdmin);
            btnY += btnHeight + btnMargin;
        }

        // Logout button
        btnLogout = CreateMenuButton("Çıkış", "🚪", 0);
        btnLogout.BackColor = Color.FromArgb(150, 50, 50);
        btnLogout.Dock = DockStyle.Bottom;
        btnLogout.Click += BtnLogout_Click;
        pnlSidebar.Controls.Add(btnLogout);

        this.Controls.Add(pnlSidebar);

        // Content panel
        pnlContent = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(35, 35, 45),
            Padding = new Padding(10)
        };
        this.Controls.Add(pnlContent);

        // Bring sidebar to front
        pnlSidebar.BringToFront();

        this.ResumeLayout(false);
    }

    private Button CreateMenuButton(string text, string icon, int y)
    {
        var btn = new Button
        {
            Text = $"{icon}  {text}",
            Tag = new string[] { icon, text }, // Store icon and text separately
            Font = new Font("Segoe UI", 11),
            Size = new Size(190, 45),
            Location = new Point(5, y),
            BackColor = Color.FromArgb(45, 45, 60),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(10, 0, 0, 0)
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(60, 60, 80);
        btn.MouseLeave += (s, e) => 
        {
            if (btn != btnLogout)
                btn.BackColor = Color.FromArgb(45, 45, 60);
            else
                btn.BackColor = Color.FromArgb(150, 50, 50);
        };
        return btn;
    }

    private void BtnToggle_Click(object? sender, EventArgs e)
    {
        isSidebarExpanded = !isSidebarExpanded;
        
        if (isSidebarExpanded)
        {
            // Expand
            pnlSidebar.Width = SIDEBAR_EXPANDED_WIDTH;
            lblTitle.Visible = true;
            lblUser.Visible = true;
            
            // Show role label
            var lblRole = pnlSidebar.Controls.Find("lblRole", false).FirstOrDefault();
            if (lblRole != null) lblRole.Visible = true;
            
            // Update buttons to show text
            foreach (Control ctrl in pnlSidebar.Controls)
            {
                if (ctrl is Button btn && btn.Tag is string[] parts && parts.Length == 2)
                {
                    btn.Text = $"{parts[0]}  {parts[1]}";
                    btn.Size = new Size(190, 45);
                    btn.TextAlign = ContentAlignment.MiddleLeft;
                }
            }
        }
        else
        {
            // Collapse
            pnlSidebar.Width = SIDEBAR_COLLAPSED_WIDTH;
            lblTitle.Visible = false;
            lblUser.Visible = false;
            
            // Hide role label
            var lblRole = pnlSidebar.Controls.Find("lblRole", false).FirstOrDefault();
            if (lblRole != null) lblRole.Visible = false;
            
            // Update buttons to show only icons
            foreach (Control ctrl in pnlSidebar.Controls)
            {
                if (ctrl is Button btn && btn.Tag is string[] parts && parts.Length == 2)
                {
                    btn.Text = parts[0];
                    btn.Size = new Size(50, 45);
                    btn.TextAlign = ContentAlignment.MiddleCenter;
                    btn.Padding = new Padding(0);
                }
            }
        }
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
