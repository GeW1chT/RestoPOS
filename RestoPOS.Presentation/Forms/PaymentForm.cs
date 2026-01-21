using RestoPOS.Common.Enums;
using RestoPOS.Data.Entities;

namespace RestoPOS.Presentation.Forms;

/// <summary>
/// Ödeme formu
/// </summary>
public partial class PaymentForm : Form
{
    private readonly Order _order;
    
    private Label lblSubTotal = null!;
    private Label lblTax = null!;
    private Label lblDiscount = null!;
    private Label lblTotal = null!;
    private NumericUpDown numDiscount = null!;
    private NumericUpDown numReceived = null!;
    private Label lblChange = null!;
    private RadioButton rbCash = null!;
    private RadioButton rbCard = null!;
    private Button btnPay = null!;
    
    private decimal discountAmount = 0;

    public PaymentForm(Order order)
    {
        _order = order;
        InitializeComponent();
        CalculateTotal();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        this.Text = "Ödeme";
        this.Size = new Size(500, 600);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.FromArgb(30, 30, 40);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        int y = 20;
        int labelWidth = 150;
        int valueX = 180;

        // Header
        var lblHeader = new Label
        {
            Text = "💳 Ödeme İşlemi",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 165, 0),
            Location = new Point(20, y),
            AutoSize = true
        };
        this.Controls.Add(lblHeader);
        y += 60;

        // Order items summary
        var pnlItems = new Panel
        {
            Location = new Point(20, y),
            Size = new Size(440, 150),
            BackColor = Color.FromArgb(40, 40, 55),
            AutoScroll = true
        };

        int itemY = 10;
        foreach (var item in _order.OrderItems)
        {
            var lbl = new Label
            {
                Text = $"{item.Product?.Name ?? "Ürün"} x{item.Quantity} = ₺{item.TotalPrice:N2}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                Location = new Point(10, itemY),
                AutoSize = true
            };
            pnlItems.Controls.Add(lbl);
            itemY += 25;
        }
        this.Controls.Add(pnlItems);
        y += 170;

        // Subtotal
        AddLabel("Ara Toplam:", 20, y);
        lblSubTotal = AddValueLabel($"₺{_order.SubTotal:N2}", valueX, y);
        y += 35;

        // Tax
        AddLabel("KDV:", 20, y);
        lblTax = AddValueLabel($"₺{_order.TaxAmount:N2}", valueX, y);
        y += 35;

        // Discount
        AddLabel("İndirim (₺):", 20, y);
        numDiscount = new NumericUpDown
        {
            Location = new Point(valueX, y),
            Size = new Size(120, 30),
            Font = new Font("Segoe UI", 12),
            Minimum = 0,
            Maximum = _order.TotalAmount,
            DecimalPlaces = 2,
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White
        };
        numDiscount.ValueChanged += (s, e) => CalculateTotal();
        this.Controls.Add(numDiscount);
        y += 40;

        // Divider
        var divider = new Panel
        {
            Location = new Point(20, y),
            Size = new Size(440, 2),
            BackColor = Color.FromArgb(80, 80, 100)
        };
        this.Controls.Add(divider);
        y += 15;

        // Total
        AddLabel("TOPLAM:", 20, y, true);
        lblTotal = AddValueLabel($"₺{_order.TotalAmount:N2}", valueX, y, true);
        lblTotal.ForeColor = Color.FromArgb(50, 180, 100);
        y += 50;

        // Payment method
        AddLabel("Ödeme Yöntemi:", 20, y);
        y += 30;

        rbCash = new RadioButton
        {
            Text = "💵 Nakit",
            Font = new Font("Segoe UI", 12),
            ForeColor = Color.White,
            Location = new Point(20, y),
            AutoSize = true,
            Checked = true
        };
        rbCash.CheckedChanged += (s, e) => { if (rbCash.Checked) numReceived.Enabled = true; };
        this.Controls.Add(rbCash);

        rbCard = new RadioButton
        {
            Text = "💳 Kredi Kartı",
            Font = new Font("Segoe UI", 12),
            ForeColor = Color.White,
            Location = new Point(180, y),
            AutoSize = true
        };
        rbCard.CheckedChanged += (s, e) => { 
            if (rbCard.Checked) 
            {
                numReceived.Value = _order.TotalAmount - discountAmount;
                numReceived.Enabled = false;
            }
        };
        this.Controls.Add(rbCard);
        y += 40;

        // Received amount
        AddLabel("Alınan Tutar (₺):", 20, y);
        numReceived = new NumericUpDown
        {
            Location = new Point(valueX, y),
            Size = new Size(150, 30),
            Font = new Font("Segoe UI", 12),
            Minimum = 0,
            Maximum = 100000,
            DecimalPlaces = 2,
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White,
            Value = _order.TotalAmount
        };
        numReceived.ValueChanged += (s, e) => CalculateChange();
        this.Controls.Add(numReceived);
        y += 40;

        // Change
        AddLabel("Para Üstü:", 20, y);
        lblChange = AddValueLabel("₺0,00", valueX, y);
        lblChange.ForeColor = Color.FromArgb(255, 200, 100);
        y += 50;

        // Pay button
        btnPay = new Button
        {
            Text = "✅ ÖDEME YAP",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Size = new Size(440, 55),
            Location = new Point(20, y),
            BackColor = Color.FromArgb(50, 180, 100),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnPay.FlatAppearance.BorderSize = 0;
        btnPay.Click += BtnPay_Click;
        this.Controls.Add(btnPay);

        this.ResumeLayout(false);
    }

    private Label AddLabel(string text, int x, int y, bool bold = false)
    {
        var lbl = new Label
        {
            Text = text,
            Font = new Font("Segoe UI", bold ? 14 : 11, bold ? FontStyle.Bold : FontStyle.Regular),
            ForeColor = Color.White,
            Location = new Point(x, y),
            AutoSize = true
        };
        this.Controls.Add(lbl);
        return lbl;
    }

    private Label AddValueLabel(string text, int x, int y, bool bold = false)
    {
        var lbl = new Label
        {
            Text = text,
            Font = new Font("Segoe UI", bold ? 16 : 12, bold ? FontStyle.Bold : FontStyle.Regular),
            ForeColor = Color.White,
            Location = new Point(x, y),
            AutoSize = true
        };
        this.Controls.Add(lbl);
        return lbl;
    }

    private void CalculateTotal()
    {
        discountAmount = numDiscount.Value;
        var total = _order.TotalAmount - discountAmount;
        if (total < 0) total = 0;

        lblDiscount?.Invoke(() => lblDiscount.Text = $"-₺{discountAmount:N2}");
        lblTotal.Text = $"₺{total:N2}";
        
        if (rbCard?.Checked == true)
        {
            numReceived.Value = total;
        }

        CalculateChange();
    }

    private void CalculateChange()
    {
        var total = _order.TotalAmount - discountAmount;
        var change = numReceived.Value - total;
        if (change < 0) change = 0;
        lblChange.Text = $"₺{change:N2}";
    }

    private async void BtnPay_Click(object? sender, EventArgs e)
    {
        var total = _order.TotalAmount - discountAmount;
        
        if (numReceived.Value < total)
        {
            MessageBox.Show("Alınan tutar toplam tutardan az olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var method = rbCash.Checked ? PaymentMethod.Cash : PaymentMethod.CreditCard;
            var payment = await Program.PaymentService.ProcessPaymentAsync(
                _order.Id, method, numReceived.Value, discountAmount);

            if (payment != null)
            {
                MessageBox.Show(
                    $"Ödeme başarıyla alındı!\n\n" +
                    $"Fatura No: {payment.InvoiceNumber}\n" +
                    $"Toplam: ₺{payment.TotalAmount:N2}\n" +
                    $"Alınan: ₺{payment.ReceivedAmount:N2}\n" +
                    $"Para Üstü: ₺{payment.ChangeAmount:N2}",
                    "Ödeme Tamamlandı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ödeme işlenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
