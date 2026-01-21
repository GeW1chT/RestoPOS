using RestoPOS.Data.Entities;

namespace RestoPOS.Presentation.Forms;

/// <summary>
/// Ürün yönetimi formu
/// </summary>
public partial class ProductManagementForm : Form
{
    private DataGridView dgvProducts = null!;
    private ComboBox cmbCategory = null!;
    private Button btnAdd = null!;
    private Button btnEdit = null!;
    private Button btnDelete = null!;

    public ProductManagementForm()
    {
        InitializeComponent();
        LoadCategories();
        LoadProducts();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        this.BackColor = Color.FromArgb(35, 35, 45);

        // Title
        var lblTitle = new Label
        {
            Text = "Ürün Yönetimi",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(20, 15),
            TextAlign = ContentAlignment.MiddleLeft
        };
        this.Controls.Add(lblTitle);

        // Category filter
        var lblCategory = new Label
        {
            Text = "Kategori:",
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.White,
            Location = new Point(20, 70),
            AutoSize = true
        };
        this.Controls.Add(lblCategory);

        cmbCategory = new ComboBox
        {
            Location = new Point(100, 67),
            Size = new Size(200, 30),
            Font = new Font("Segoe UI", 11),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White
        };
        cmbCategory.SelectedIndexChanged += (s, e) => LoadProducts();
        this.Controls.Add(cmbCategory);

        // Buttons panel
        var pnlButtons = new FlowLayoutPanel
        {
            Location = new Point(320, 60),
            Size = new Size(500, 50),
            BackColor = Color.Transparent
        };

        btnAdd = CreateButton("➕ Yeni Ürün", Color.FromArgb(50, 180, 100));
        btnAdd.Click += BtnAdd_Click;
        pnlButtons.Controls.Add(btnAdd);

        btnEdit = CreateButton("✏️ Düzenle", Color.FromArgb(255, 165, 0));
        btnEdit.Click += BtnEdit_Click;
        pnlButtons.Controls.Add(btnEdit);

        btnDelete = CreateButton("🗑️ Sil", Color.FromArgb(220, 80, 80));
        btnDelete.Click += BtnDelete_Click;
        pnlButtons.Controls.Add(btnDelete);

        this.Controls.Add(pnlButtons);

        // Products grid
        dgvProducts = new DataGridView
        {
            Location = new Point(20, 120),
            Size = new Size(this.ClientSize.Width - 40, this.ClientSize.Height - 140),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
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
            RowTemplate = { Height = 40 }
        };

        dgvProducts.Columns.Add("Id", "ID");
        dgvProducts.Columns["Id"].Visible = false;
        dgvProducts.Columns.Add("Name", "Ürün Adı");
        dgvProducts.Columns["Name"].FillWeight = 25;
        dgvProducts.Columns["Name"].MinimumWidth = 150;
        dgvProducts.Columns.Add("Category", "Kategori");
        dgvProducts.Columns["Category"].FillWeight = 18;
        dgvProducts.Columns["Category"].MinimumWidth = 100;
        dgvProducts.Columns.Add("Price", "Fiyat");
        dgvProducts.Columns["Price"].FillWeight = 12;
        dgvProducts.Columns["Price"].MinimumWidth = 80;
        dgvProducts.Columns.Add("TaxRate", "KDV %");
        dgvProducts.Columns["TaxRate"].FillWeight = 10;
        dgvProducts.Columns["TaxRate"].MinimumWidth = 60;
        dgvProducts.Columns.Add("Stock", "Stok");
        dgvProducts.Columns["Stock"].FillWeight = 10;
        dgvProducts.Columns["Stock"].MinimumWidth = 60;
        dgvProducts.Columns.Add("Status", "Durum");
        dgvProducts.Columns["Status"].FillWeight = 12;
        dgvProducts.Columns["Status"].MinimumWidth = 80;

        dgvProducts.DoubleClick += (s, e) => BtnEdit_Click(s, e);

        this.Controls.Add(dgvProducts);
        this.ResumeLayout(false);
    }

    private Button CreateButton(string text, Color color)
    {
        var btn = new Button
        {
            Text = text,
            Font = new Font("Segoe UI", 11),
            Size = new Size(140, 40),
            BackColor = color,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(5)
        };
        btn.FlatAppearance.BorderSize = 0;
        return btn;
    }

    private async void LoadCategories()
    {
        try
        {
            var categories = await Program.CategoryService.GetAllAsync();
            
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add(new CategoryItem { Id = null, Name = "Tüm Kategoriler" });
            
            foreach (var cat in categories)
            {
                cmbCategory.Items.Add(new CategoryItem { Id = cat.Id, Name = cat.Name });
            }
            
            cmbCategory.SelectedIndex = 0;
            cmbCategory.DisplayMember = "Name";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Kategoriler yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void LoadProducts()
    {
        try
        {
            var selectedCategory = cmbCategory.SelectedItem as CategoryItem;
            var products = selectedCategory?.Id.HasValue == true
                ? await Program.ProductService.GetByCategoryAsync(selectedCategory.Id.Value)
                : await Program.ProductService.GetAllAsync();

            dgvProducts.Rows.Clear();
            
            foreach (var product in products)
            {
                dgvProducts.Rows.Add(
                    product.Id,
                    product.Name,
                    product.Category?.Name ?? "-",
                    $"₺{product.Price:N2}",
                    $"%{product.TaxRate}",
                    product.Stock,
                    product.IsActive ? "✅ Aktif" : "❌ Pasif"
                );
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ürünler yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        var form = new ProductEditForm(null);
        if (form.ShowDialog() == DialogResult.OK)
        {
            LoadProducts();
        }
    }

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        if (dgvProducts.SelectedRows.Count == 0)
        {
            MessageBox.Show("Lütfen düzenlemek için bir ürün seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var productId = (int)dgvProducts.SelectedRows[0].Cells["Id"].Value;
        var form = new ProductEditForm(productId);
        if (form.ShowDialog() == DialogResult.OK)
        {
            LoadProducts();
        }
    }

    private async void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (dgvProducts.SelectedRows.Count == 0)
        {
            MessageBox.Show("Lütfen silmek için bir ürün seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var productName = dgvProducts.SelectedRows[0].Cells["Name"].Value?.ToString();
        var result = MessageBox.Show($"'{productName}' ürününü silmek istediğinizden emin misiniz?",
            "Ürün Sil", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (result == DialogResult.Yes)
        {
            var productId = (int)dgvProducts.SelectedRows[0].Cells["Id"].Value;
            await Program.ProductService.DeleteAsync(productId);
            LoadProducts();
        }
    }

    private class CategoryItem
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public override string ToString() => Name;
    }
}
