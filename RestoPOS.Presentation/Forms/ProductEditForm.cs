using RestoPOS.Data.Entities;

namespace RestoPOS.Presentation.Forms;

/// <summary>
/// Ürün düzenleme formu
/// </summary>
public partial class ProductEditForm : Form
{
    private readonly int? _productId;
    private Product? _product;
    
    private TextBox txtName = null!;
    private TextBox txtDescription = null!;
    private ComboBox cmbCategory = null!;
    private NumericUpDown numPrice = null!;
    private ComboBox cmbTaxRate = null!;
    private NumericUpDown numStock = null!;
    private Button btnSave = null!;
    private Button btnCancel = null!;

    public ProductEditForm(int? productId)
    {
        _productId = productId;
        InitializeComponent();
        LoadCategories();
        if (_productId.HasValue)
        {
            LoadProduct();
        }
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        this.Text = _productId.HasValue ? "Ürün Düzenle" : "Yeni Ürün";
        this.Size = new Size(450, 500);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.FromArgb(30, 30, 40);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        int y = 20;
        int labelX = 20;
        int inputX = 150;
        int inputWidth = 260;

        // Name
        AddLabel("Ürün Adı:", labelX, y);
        txtName = new TextBox
        {
            Location = new Point(inputX, y),
            Size = new Size(inputWidth, 30),
            Font = new Font("Segoe UI", 11),
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };
        this.Controls.Add(txtName);
        y += 45;

        // Description
        AddLabel("Açıklama:", labelX, y);
        txtDescription = new TextBox
        {
            Location = new Point(inputX, y),
            Size = new Size(inputWidth, 60),
            Font = new Font("Segoe UI", 10),
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Multiline = true
        };
        this.Controls.Add(txtDescription);
        y += 75;

        // Category
        AddLabel("Kategori:", labelX, y);
        cmbCategory = new ComboBox
        {
            Location = new Point(inputX, y),
            Size = new Size(inputWidth, 30),
            Font = new Font("Segoe UI", 11),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White
        };
        this.Controls.Add(cmbCategory);
        y += 45;

        // Price
        AddLabel("Fiyat (₺):", labelX, y);
        numPrice = new NumericUpDown
        {
            Location = new Point(inputX, y),
            Size = new Size(inputWidth, 30),
            Font = new Font("Segoe UI", 11),
            Minimum = 0,
            Maximum = 100000,
            DecimalPlaces = 2,
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White
        };
        this.Controls.Add(numPrice);
        y += 45;

        // Tax Rate
        AddLabel("KDV Oranı:", labelX, y);
        cmbTaxRate = new ComboBox
        {
            Location = new Point(inputX, y),
            Size = new Size(inputWidth, 30),
            Font = new Font("Segoe UI", 11),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White
        };
        cmbTaxRate.Items.AddRange(new object[] { "%0", "%1", "%10", "%20" });
        cmbTaxRate.SelectedIndex = 2; // Default %10
        this.Controls.Add(cmbTaxRate);
        y += 45;

        // Stock
        AddLabel("Stok:", labelX, y);
        numStock = new NumericUpDown
        {
            Location = new Point(inputX, y),
            Size = new Size(inputWidth, 30),
            Font = new Font("Segoe UI", 11),
            Minimum = 0,
            Maximum = 100000,
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White,
            Value = 100
        };
        this.Controls.Add(numStock);
        y += 60;

        // Buttons
        btnSave = new Button
        {
            Text = "💾 Kaydet",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Size = new Size(130, 45),
            Location = new Point(inputX, y),
            BackColor = Color.FromArgb(50, 180, 100),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.Click += BtnSave_Click;
        this.Controls.Add(btnSave);

        btnCancel = new Button
        {
            Text = "❌ İptal",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Size = new Size(120, 45),
            Location = new Point(inputX + 140, y),
            BackColor = Color.FromArgb(100, 100, 120),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.Click += (s, e) => this.Close();
        this.Controls.Add(btnCancel);

        this.ResumeLayout(false);
    }

    private void AddLabel(string text, int x, int y)
    {
        var lbl = new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.White,
            Location = new Point(x, y + 5),
            AutoSize = true
        };
        this.Controls.Add(lbl);
    }

    private async void LoadCategories()
    {
        try
        {
            var categories = await Program.CategoryService.GetAllAsync();
            cmbCategory.Items.Clear();
            
            foreach (var cat in categories)
            {
                cmbCategory.Items.Add(new CategoryItem { Id = cat.Id, Name = cat.Name });
            }
            
            if (cmbCategory.Items.Count > 0)
                cmbCategory.SelectedIndex = 0;
            
            cmbCategory.DisplayMember = "Name";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Kategoriler yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void LoadProduct()
    {
        try
        {
            _product = await Program.ProductService.GetByIdAsync(_productId!.Value);
            if (_product == null)
            {
                MessageBox.Show("Ürün bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            txtName.Text = _product.Name;
            txtDescription.Text = _product.Description;
            numPrice.Value = _product.Price;
            numStock.Value = _product.Stock;

            // Select category
            for (int i = 0; i < cmbCategory.Items.Count; i++)
            {
                if (cmbCategory.Items[i] is CategoryItem cat && cat.Id == _product.CategoryId)
                {
                    cmbCategory.SelectedIndex = i;
                    break;
                }
            }

            // Select tax rate
            var taxIndex = _product.TaxRate switch
            {
                0 => 0,
                1 => 1,
                10 => 2,
                20 => 3,
                _ => 2
            };
            cmbTaxRate.SelectedIndex = taxIndex;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ürün yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Ürün adı gereklidir.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cmbCategory.SelectedItem is not CategoryItem category)
        {
            MessageBox.Show("Lütfen bir kategori seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var taxRate = cmbTaxRate.SelectedIndex switch
        {
            0 => 0,
            1 => 1,
            2 => 10,
            3 => 20,
            _ => 10
        };

        try
        {
            if (_productId.HasValue)
            {
                await Program.ProductService.UpdateAsync(
                    _productId.Value,
                    txtName.Text.Trim(),
                    category.Id,
                    numPrice.Value,
                    taxRate,
                    (int)numStock.Value,
                    txtDescription.Text.Trim(),
                    null);
            }
            else
            {
                await Program.ProductService.CreateAsync(
                    txtName.Text.Trim(),
                    category.Id,
                    numPrice.Value,
                    taxRate,
                    (int)numStock.Value,
                    txtDescription.Text.Trim(),
                    null);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Kayıt sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private class CategoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public override string ToString() => Name;
    }
}
