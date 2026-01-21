using RestoPOS.Common.Enums;
using RestoPOS.Data.Entities;

namespace RestoPOS.Presentation.Forms;

/// <summary>
/// Yönetici paneli formu
/// </summary>
public partial class AdminPanelForm : Form
{
    private TabControl tabAdmin = null!;
    private DataGridView dgvUsers = null!;
    private DataGridView dgvSettings = null!;
    private DataGridView dgvCategories = null!;
    private DataGridView dgvTables = null!;

    public AdminPanelForm()
    {
        InitializeComponent();
        LoadData();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        this.BackColor = Color.FromArgb(35, 35, 45);

        // Title
        var lblTitle = new Label
        {
            Text = "⚙️ Yönetim Paneli",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = false,
            Size = new Size(300, 50),
            Location = new Point(20, 10),
            TextAlign = ContentAlignment.MiddleLeft
        };
        this.Controls.Add(lblTitle);

        // Tab control
        tabAdmin = new TabControl
        {
            Location = new Point(20, 70),
            Size = new Size(this.ClientSize.Width - 40, this.ClientSize.Height - 90),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            Font = new Font("Segoe UI", 10)
        };

        // Users tab
        var tabUsers = new TabPage("👥 Kullanıcılar");
        tabUsers.BackColor = Color.FromArgb(40, 40, 55);
        var pnlUsersButtons = CreateButtonPanel();
        var btnAddUser = CreateButton("➕ Ekle", Color.FromArgb(50, 180, 100));
        btnAddUser.Click += BtnAddUser_Click;
        pnlUsersButtons.Controls.Add(btnAddUser);
        var btnEditUser = CreateButton("✏️ Düzenle", Color.FromArgb(255, 165, 0));
        btnEditUser.Click += BtnEditUser_Click;
        pnlUsersButtons.Controls.Add(btnEditUser);
        var btnDeleteUser = CreateButton("🗑️ Sil", Color.FromArgb(220, 80, 80));
        btnDeleteUser.Click += BtnDeleteUser_Click;
        pnlUsersButtons.Controls.Add(btnDeleteUser);
        tabUsers.Controls.Add(pnlUsersButtons);
        
        dgvUsers = CreateDataGrid();
        dgvUsers.Columns.Add("Id", "ID");
        dgvUsers.Columns["Id"].Visible = false;
        dgvUsers.Columns.Add("Username", "Kullanıcı Adı");
        dgvUsers.Columns.Add("FullName", "Ad Soyad");
        dgvUsers.Columns.Add("Role", "Rol");
        dgvUsers.Columns.Add("Status", "Durum");
        dgvUsers.Location = new Point(10, 60);
        dgvUsers.Size = new Size(tabAdmin.Width - 40, tabAdmin.Height - 110);
        tabUsers.Controls.Add(dgvUsers);
        tabAdmin.TabPages.Add(tabUsers);

        // Categories tab
        var tabCategories = new TabPage("📁 Kategoriler");
        tabCategories.BackColor = Color.FromArgb(40, 40, 55);
        var pnlCatButtons = CreateButtonPanel();
        var btnAddCat = CreateButton("➕ Ekle", Color.FromArgb(50, 180, 100));
        btnAddCat.Click += BtnAddCategory_Click;
        pnlCatButtons.Controls.Add(btnAddCat);
        tabCategories.Controls.Add(pnlCatButtons);
        
        dgvCategories = CreateDataGrid();
        dgvCategories.Columns.Add("Id", "ID");
        dgvCategories.Columns["Id"].Visible = false;
        dgvCategories.Columns.Add("Name", "Kategori Adı");
        dgvCategories.Columns.Add("Description", "Açıklama");
        dgvCategories.Columns.Add("Products", "Ürün Sayısı");
        dgvCategories.Location = new Point(10, 60);
        dgvCategories.Size = new Size(tabAdmin.Width - 40, tabAdmin.Height - 110);
        tabCategories.Controls.Add(dgvCategories);
        tabAdmin.TabPages.Add(tabCategories);

        // Tables tab
        var tabTables = new TabPage("🪑 Masalar");
        tabTables.BackColor = Color.FromArgb(40, 40, 55);
        var pnlTableButtons = CreateButtonPanel();
        var btnAddTable = CreateButton("➕ Ekle", Color.FromArgb(50, 180, 100));
        btnAddTable.Click += BtnAddTable_Click;
        pnlTableButtons.Controls.Add(btnAddTable);
        tabTables.Controls.Add(pnlTableButtons);
        
        dgvTables = CreateDataGrid();
        dgvTables.Columns.Add("Id", "ID");
        dgvTables.Columns["Id"].Visible = false;
        dgvTables.Columns.Add("Number", "Masa No");
        dgvTables.Columns.Add("Name", "Masa Adı");
        dgvTables.Columns.Add("Capacity", "Kapasite");
        dgvTables.Columns.Add("Status", "Durum");
        dgvTables.Location = new Point(10, 60);
        dgvTables.Size = new Size(tabAdmin.Width - 40, tabAdmin.Height - 110);
        tabTables.Controls.Add(dgvTables);
        tabAdmin.TabPages.Add(tabTables);

        // Settings tab
        var tabSettings = new TabPage("🔧 Ayarlar");
        tabSettings.BackColor = Color.FromArgb(40, 40, 55);
        dgvSettings = CreateDataGrid();
        dgvSettings.Columns.Add("Key", "Ayar");
        dgvSettings.Columns.Add("Value", "Değer");
        dgvSettings.Columns.Add("Description", "Açıklama");
        dgvSettings.Location = new Point(10, 10);
        dgvSettings.Size = new Size(tabAdmin.Width - 40, tabAdmin.Height - 60);
        tabSettings.Controls.Add(dgvSettings);
        tabAdmin.TabPages.Add(tabSettings);

        this.Controls.Add(tabAdmin);
        this.ResumeLayout(false);
    }

    private FlowLayoutPanel CreateButtonPanel()
    {
        return new FlowLayoutPanel
        {
            Location = new Point(10, 10),
            Size = new Size(800, 45),
            BackColor = Color.Transparent
        };
    }

    private Button CreateButton(string text, Color color)
    {
        var btn = new Button
        {
            Text = text,
            Font = new Font("Segoe UI", 10),
            Size = new Size(120, 38),
            BackColor = color,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(5)
        };
        btn.FlatAppearance.BorderSize = 0;
        return btn;
    }

    private DataGridView CreateDataGrid()
    {
        return new DataGridView
        {
            BackgroundColor = Color.FromArgb(45, 45, 60),
            BorderStyle = BorderStyle.None,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(50, 50, 65),
                ForeColor = Color.White,
                SelectionBackColor = Color.FromArgb(70, 70, 90),
                SelectionForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            },
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(55, 55, 75),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            },
            EnableHeadersVisualStyles = false,
            RowTemplate = { Height = 35 }
        };
    }

    private async void LoadData()
    {
        await LoadUsers();
        await LoadCategories();
        await LoadTables();
        await LoadSettings();
    }

    private async Task LoadUsers()
    {
        try
        {
            var users = await Program.UserService.GetAllAsync();
            dgvUsers.Rows.Clear();
            foreach (var user in users)
            {
                var roleName = user.Role switch
                {
                    UserRole.Admin => "Yönetici",
                    UserRole.Waiter => "Garson",
                    UserRole.Cashier => "Kasiyer",
                    UserRole.Kitchen => "Mutfak",
                    _ => "Bilinmiyor"
                };
                dgvUsers.Rows.Add(user.Id, user.Username, user.FullName, roleName, user.IsActive ? "✅ Aktif" : "❌ Pasif");
            }
        }
        catch { }
    }

    private async Task LoadCategories()
    {
        try
        {
            var categories = await Program.CategoryService.GetAllAsync();
            dgvCategories.Rows.Clear();
            foreach (var cat in categories)
            {
                dgvCategories.Rows.Add(cat.Id, cat.Name, cat.Description ?? "-", cat.Products?.Count ?? 0);
            }
        }
        catch { }
    }

    private async Task LoadTables()
    {
        try
        {
            var tables = await Program.TableService.GetAllAsync();
            dgvTables.Rows.Clear();
            foreach (var table in tables)
            {
                var status = table.Status switch
                {
                    TableStatus.Empty => "Boş",
                    TableStatus.Occupied => "Dolu",
                    TableStatus.WaitingPayment => "Ödeme Bekleniyor",
                    _ => "-"
                };
                dgvTables.Rows.Add(table.Id, table.TableNumber, table.TableName ?? $"Masa {table.TableNumber}", table.Capacity, status);
            }
        }
        catch { }
    }

    private async Task LoadSettings()
    {
        try
        {
            var settings = await Program.SettingsService.GetAllAsync();
            dgvSettings.Rows.Clear();
            foreach (var setting in settings)
            {
                dgvSettings.Rows.Add(setting.Key, setting.Value, setting.Description ?? "-");
            }
        }
        catch { }
    }

    private void BtnAddUser_Click(object? sender, EventArgs e)
    {
        var form = new UserEditForm(null);
        if (form.ShowDialog() == DialogResult.OK)
        {
            _ = LoadUsers();
        }
    }

    private void BtnEditUser_Click(object? sender, EventArgs e)
    {
        if (dgvUsers.SelectedRows.Count == 0)
        {
            MessageBox.Show("Lütfen bir kullanıcı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var userId = (int)dgvUsers.SelectedRows[0].Cells["Id"].Value;
        var form = new UserEditForm(userId);
        if (form.ShowDialog() == DialogResult.OK)
        {
            _ = LoadUsers();
        }
    }

    private async void BtnDeleteUser_Click(object? sender, EventArgs e)
    {
        if (dgvUsers.SelectedRows.Count == 0)
        {
            MessageBox.Show("Lütfen bir kullanıcı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var userId = (int)dgvUsers.SelectedRows[0].Cells["Id"].Value;
        var userName = dgvUsers.SelectedRows[0].Cells["FullName"].Value?.ToString();

        if (userId == Program.CurrentUser?.Id)
        {
            MessageBox.Show("Kendi hesabınızı silemezsiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var result = MessageBox.Show($"'{userName}' kullanıcısını silmek istediğinizden emin misiniz?",
            "Kullanıcı Sil", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (result == DialogResult.Yes)
        {
            await Program.UserService.DeleteAsync(userId);
            await LoadUsers();
        }
    }

    private async void BtnAddCategory_Click(object? sender, EventArgs e)
    {
        var name = Microsoft.VisualBasic.Interaction.InputBox("Kategori adı:", "Yeni Kategori", "");
        if (!string.IsNullOrWhiteSpace(name))
        {
            await Program.CategoryService.CreateAsync(name.Trim());
            await LoadCategories();
        }
    }

    private async void BtnAddTable_Click(object? sender, EventArgs e)
    {
        var tables = await Program.TableService.GetAllAsync();
        var nextNumber = tables.Count > 0 ? tables.Max(t => t.TableNumber) + 1 : 1;
        
        await Program.TableService.CreateAsync(nextNumber, 4);
        await LoadTables();
        MessageBox.Show($"Masa {nextNumber} eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
