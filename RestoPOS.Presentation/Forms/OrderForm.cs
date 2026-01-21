using RestoPOS.Common.Enums;
using RestoPOS.Data.Entities;

namespace RestoPOS.Presentation.Forms;

/// <summary>
/// Sipariş alma formu
/// </summary>
public partial class OrderForm : Form
{
    private readonly RestaurantTable _table;
    private Order _order;
    
    private Panel pnlLeft = null!;
    private Panel pnlRight = null!;
    private FlowLayoutPanel pnlCategories = null!;
    private FlowLayoutPanel pnlProducts = null!;
    private DataGridView dgvOrderItems = null!;
    private Label lblTotal = null!;
    private TextBox txtNote = null!;
    private Button btnPayment = null!;
    private Button btnCancel = null!;
    private Button btnClose = null!;
    
    private int? selectedCategoryId = null;

    public OrderForm(RestaurantTable table, Order? order)
    {
        _table = table;
        _order = order ?? new Order();
        InitializeComponent();
        LoadCategories();
        LoadOrderItems();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        this.Text = $"Sipariş - Masa {_table.TableNumber}";
        this.Size = new Size(1200, 750);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.FromArgb(30, 30, 40);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        // Left panel (products)
        pnlLeft = new Panel
        {
            Location = new Point(10, 10),
            Size = new Size(700, 690),
            BackColor = Color.FromArgb(40, 40, 55)
        };

        // Categories panel
        var lblCategories = new Label
        {
            Text = "Kategoriler",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(10, 10),
            AutoSize = true
        };
        pnlLeft.Controls.Add(lblCategories);

        pnlCategories = new FlowLayoutPanel
        {
            Location = new Point(10, 40),
            Size = new Size(680, 60),
            BackColor = Color.Transparent,
            AutoScroll = true
        };
        pnlLeft.Controls.Add(pnlCategories);

        // Products panel
        var lblProducts = new Label
        {
            Text = "Ürünler",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(10, 110),
            AutoSize = true
        };
        pnlLeft.Controls.Add(lblProducts);

        pnlProducts = new FlowLayoutPanel
        {
            Location = new Point(10, 140),
            Size = new Size(680, 540),
            BackColor = Color.FromArgb(35, 35, 50),
            AutoScroll = true,
            Padding = new Padding(5)
        };
        pnlLeft.Controls.Add(pnlProducts);

        this.Controls.Add(pnlLeft);

        // Right panel (order)
        pnlRight = new Panel
        {
            Location = new Point(720, 10),
            Size = new Size(460, 690),
            BackColor = Color.FromArgb(40, 40, 55)
        };

        // Order header
        var lblOrder = new Label
        {
            Text = $"🧾 Masa {_table.TableNumber} - Sipariş",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 165, 0),
            Location = new Point(10, 10),
            AutoSize = true
        };
        pnlRight.Controls.Add(lblOrder);

        // Order items grid
        dgvOrderItems = new DataGridView
        {
            Location = new Point(10, 50),
            Size = new Size(440, 380),
            BackgroundColor = Color.FromArgb(50, 50, 65),
            BorderStyle = BorderStyle.None,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
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
                BackColor = Color.FromArgb(60, 60, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            },
            EnableHeadersVisualStyles = false
        };

        dgvOrderItems.Columns.Add("Id", "ID");
        dgvOrderItems.Columns["Id"].Visible = false;
        dgvOrderItems.Columns.Add("Product", "Ürün");
        dgvOrderItems.Columns["Product"].FillWeight = 40;
        dgvOrderItems.Columns.Add("Qty", "Adet");
        dgvOrderItems.Columns["Qty"].FillWeight = 15;
        dgvOrderItems.Columns.Add("Price", "Fiyat");
        dgvOrderItems.Columns["Price"].FillWeight = 20;
        dgvOrderItems.Columns.Add("Total", "Toplam");
        dgvOrderItems.Columns["Total"].FillWeight = 25;

        // Add delete button column
        var btnColumn = new DataGridViewButtonColumn
        {
            Name = "Delete",
            Text = "🗑️",
            UseColumnTextForButtonValue = true,
            FillWeight = 15
        };
        dgvOrderItems.Columns.Add(btnColumn);
        dgvOrderItems.CellClick += DgvOrderItems_CellClick;
        dgvOrderItems.CellValueChanged += DgvOrderItems_CellValueChanged;

        pnlRight.Controls.Add(dgvOrderItems);

        // Note
        var lblNote = new Label
        {
            Text = "Not:",
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.White,
            Location = new Point(10, 440),
            AutoSize = true
        };
        pnlRight.Controls.Add(lblNote);

        txtNote = new TextBox
        {
            Location = new Point(10, 465),
            Size = new Size(440, 60),
            Multiline = true,
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 10),
            Text = _order.Notes ?? ""
        };
        pnlRight.Controls.Add(txtNote);

        // Total label
        lblTotal = new Label
        {
            Text = "Toplam: ₺0,00",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.FromArgb(50, 180, 100),
            Location = new Point(10, 535),
            Size = new Size(440, 40),
            TextAlign = ContentAlignment.MiddleRight
        };
        pnlRight.Controls.Add(lblTotal);

        // Buttons panel
        var pnlButtons = new FlowLayoutPanel
        {
            Location = new Point(10, 580),
            Size = new Size(440, 100),
            BackColor = Color.Transparent,
            FlowDirection = FlowDirection.LeftToRight
        };

        btnPayment = new Button
        {
            Text = "💳 Ödeme",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Size = new Size(140, 50),
            BackColor = Color.FromArgb(50, 180, 100),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(5)
        };
        btnPayment.FlatAppearance.BorderSize = 0;
        btnPayment.Click += BtnPayment_Click;
        pnlButtons.Controls.Add(btnPayment);

        btnCancel = new Button
        {
            Text = "❌ İptal",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Size = new Size(140, 50),
            BackColor = Color.FromArgb(220, 80, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(5)
        };
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.Click += BtnCancel_Click;
        pnlButtons.Controls.Add(btnCancel);

        btnClose = new Button
        {
            Text = "✖ Kapat",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Size = new Size(140, 50),
            BackColor = Color.FromArgb(100, 100, 120),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(5)
        };
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.Click += (s, e) => this.Close();
        pnlButtons.Controls.Add(btnClose);

        pnlRight.Controls.Add(pnlButtons);

        this.Controls.Add(pnlRight);
        this.ResumeLayout(false);
    }

    private async void LoadCategories()
    {
        try
        {
            var categories = await Program.CategoryService.GetAllAsync();
            pnlCategories.Controls.Clear();

            // All categories button
            var allBtn = CreateCategoryButton("Tümü", null, selectedCategoryId == null);
            pnlCategories.Controls.Add(allBtn);

            foreach (var category in categories)
            {
                var btn = CreateCategoryButton(category.Name, category.Id, selectedCategoryId == category.Id);
                pnlCategories.Controls.Add(btn);
            }

            LoadProducts();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Kategoriler yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Button CreateCategoryButton(string name, int? id, bool selected)
    {
        var btn = new Button
        {
            Text = name,
            Font = new Font("Segoe UI", 10, selected ? FontStyle.Bold : FontStyle.Regular),
            Size = new Size(100, 40),
            BackColor = selected ? Color.FromArgb(255, 165, 0) : Color.FromArgb(60, 60, 80),
            ForeColor = selected ? Color.Black : Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Tag = id,
            Margin = new Padding(3)
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.Click += CategoryButton_Click;
        return btn;
    }

    private void CategoryButton_Click(object? sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            selectedCategoryId = btn.Tag as int?;
            LoadCategories();
        }
    }

    private async void LoadProducts()
    {
        try
        {
            var products = selectedCategoryId.HasValue
                ? await Program.ProductService.GetByCategoryAsync(selectedCategoryId.Value)
                : await Program.ProductService.GetAllAsync();

            pnlProducts.Controls.Clear();

            foreach (var product in products.Where(p => p.Stock > 0))
            {
                var productPanel = CreateProductPanel(product);
                pnlProducts.Controls.Add(productPanel);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ürünler yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Panel CreateProductPanel(Product product)
    {
        var panel = new Panel
        {
            Size = new Size(155, 100),
            BackColor = Color.FromArgb(60, 60, 80),
            Margin = new Padding(5),
            Cursor = Cursors.Hand,
            Tag = product
        };

        var lblName = new Label
        {
            Text = product.Name,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = false,
            Size = new Size(155, 40),
            Location = new Point(0, 10),
            TextAlign = ContentAlignment.MiddleCenter
        };
        panel.Controls.Add(lblName);

        var lblPrice = new Label
        {
            Text = $"₺{product.Price:N2}",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(50, 180, 100),
            AutoSize = false,
            Size = new Size(155, 30),
            Location = new Point(0, 50),
            TextAlign = ContentAlignment.MiddleCenter
        };
        panel.Controls.Add(lblPrice);

        var lblStock = new Label
        {
            Text = $"Stok: {product.Stock}",
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(180, 180, 180),
            AutoSize = false,
            Size = new Size(155, 20),
            Location = new Point(0, 78),
            TextAlign = ContentAlignment.MiddleCenter
        };
        panel.Controls.Add(lblStock);

        panel.Click += ProductPanel_Click;
        foreach (Control c in panel.Controls) c.Click += ProductPanel_Click;

        return panel;
    }

    private async void ProductPanel_Click(object? sender, EventArgs e)
    {
        Control? control = sender as Control;
        Panel? panel = control as Panel ?? control?.Parent as Panel;
        
        if (panel?.Tag is not Product product) return;

        try
        {
            await Program.OrderService.AddItemAsync(_order.Id, product.Id, 1);
            LoadOrderItems();
            LoadProducts();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ürün eklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void LoadOrderItems()
    {
        try
        {
            _order = await Program.OrderService.GetByIdAsync(_order.Id) ?? _order;
            
            dgvOrderItems.Rows.Clear();
            
            foreach (var item in _order.OrderItems)
            {
                dgvOrderItems.Rows.Add(
                    item.Id,
                    item.Product?.Name ?? "Ürün",
                    item.Quantity,
                    $"₺{item.UnitPrice:N2}",
                    $"₺{item.TotalPrice:N2}"
                );
            }

            UpdateTotal();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Sipariş yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void UpdateTotal()
    {
        lblTotal.Text = $"Toplam: ₺{_order.TotalAmount:N2}";
    }

    private async void DgvOrderItems_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;

        if (e.ColumnIndex == dgvOrderItems.Columns["Delete"].Index)
        {
            var itemId = (int)dgvOrderItems.Rows[e.RowIndex].Cells["Id"].Value;
            var result = MessageBox.Show("Bu ürünü sipariştenn kaldırmak istiyor musunuz?",
                "Ürün Kaldır", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                await Program.OrderService.RemoveItemAsync(itemId);
                LoadOrderItems();
                LoadProducts();
            }
        }
    }

    private async void DgvOrderItems_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex != dgvOrderItems.Columns["Qty"].Index) return;

        try
        {
            var itemId = (int)dgvOrderItems.Rows[e.RowIndex].Cells["Id"].Value;
            var newQty = int.Parse(dgvOrderItems.Rows[e.RowIndex].Cells["Qty"].Value?.ToString() ?? "1");
            
            await Program.OrderService.UpdateItemQuantityAsync(itemId, newQty);
            LoadOrderItems();
        }
        catch { }
    }

    private async void BtnPayment_Click(object? sender, EventArgs e)
    {
        if (_order.OrderItems.Count == 0)
        {
            MessageBox.Show("Siparişte ürün bulunmuyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Update order status to delivered
        await Program.OrderService.UpdateStatusAsync(_order.Id, OrderStatus.Delivered);

        var paymentForm = new PaymentForm(_order);
        if (paymentForm.ShowDialog() == DialogResult.OK)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        else
        {
            LoadOrderItems();
        }
    }

    private async void BtnCancel_Click(object? sender, EventArgs e)
    {
        var result = MessageBox.Show("Siparişi iptal etmek istiyor musunuz?",
            "Sipariş İptal", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        
        if (result == DialogResult.Yes)
        {
            await Program.OrderService.CancelOrderAsync(_order.Id);
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
