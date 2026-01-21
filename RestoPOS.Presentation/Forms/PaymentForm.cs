using RestoPOS.Common.Enums;
using RestoPOS.Data.Entities;

namespace RestoPOS.Presentation.Forms;

/// <summary>
/// Ödeme formu - Parçalı ödeme destekli
/// </summary>
public partial class PaymentForm : Form
{
    private readonly Order _order;
    
    private Label lblSubTotal = null!;
    private Label lblTax = null!;
    private Label lblTotal = null!;
    private Label lblRemaining = null!;
    private NumericUpDown numDiscount = null!;
    private NumericUpDown numCash = null!;
    private NumericUpDown numCard = null!;
    private Label lblChange = null!;
    private CheckBox chkSplitPayment = null!;
    private Panel pnlSplitPayment = null!;
    private Button btnPay = null!;
    
    private decimal discountAmount = 0;
    private decimal totalToPay = 0;

    public PaymentForm(Order order)
    {
        _order = order;
        totalToPay = order.TotalAmount;
        InitializeComponent();
        CalculateTotal();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        this.Text = "Ödeme";
        this.Size = new Size(520, 750);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.FromArgb(30, 30, 40);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        int y = 20;
        int valueX = 180;

        // Header
        var lblHeader = new Label
        {
            Text = "Ödeme İşlemi",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 165, 0),
            Location = new Point(20, y),
            AutoSize = true
        };
        this.Controls.Add(lblHeader);
        y += 50;

        // Order items summary
        var pnlItems = new Panel
        {
            Location = new Point(20, y),
            Size = new Size(460, 120),
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
            itemY += 22;
        }
        this.Controls.Add(pnlItems);
        y += 135;

        // Subtotal
        AddLabel("Ara Toplam:", 20, y);
        lblSubTotal = AddValueLabel($"₺{_order.SubTotal:N2}", valueX, y);
        y += 30;

        // Tax
        AddLabel("KDV:", 20, y);
        lblTax = AddValueLabel($"₺{_order.TaxAmount:N2}", valueX, y);
        y += 30;

        // Discount
        AddLabel("İndirim (₺):", 20, y);
        numDiscount = new NumericUpDown
        {
            Location = new Point(valueX, y - 3),
            Size = new Size(120, 28),
            Font = new Font("Segoe UI", 11),
            Minimum = 0,
            Maximum = _order.TotalAmount,
            DecimalPlaces = 2,
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White
        };
        numDiscount.ValueChanged += (s, e) => CalculateTotal();
        this.Controls.Add(numDiscount);
        y += 35;

        // Divider
        AddDivider(y);
        y += 15;

        // Total
        AddLabel("TOPLAM:", 20, y, true);
        lblTotal = AddValueLabel($"₺{_order.TotalAmount:N2}", valueX, y, true);
        lblTotal.ForeColor = Color.FromArgb(50, 180, 100);
        y += 45;

        // Split payment checkbox
        chkSplitPayment = new CheckBox
        {
            Text = "Parçalı Ödeme (Nakit + Kart)",
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.FromArgb(255, 200, 100),
            Location = new Point(20, y),
            AutoSize = true
        };
        chkSplitPayment.CheckedChanged += ChkSplitPayment_CheckedChanged;
        this.Controls.Add(chkSplitPayment);
        y += 35;

        // Split payment panel
        pnlSplitPayment = new Panel
        {
            Location = new Point(20, y),
            Size = new Size(460, 180),
            BackColor = Color.FromArgb(40, 40, 55),
            Visible = false
        };

        // Cash amount
        var lblCash = new Label
        {
            Text = "💵 Nakit Tutar:",
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.White,
            Location = new Point(15, 15),
            AutoSize = true
        };
        pnlSplitPayment.Controls.Add(lblCash);

        numCash = new NumericUpDown
        {
            Location = new Point(160, 12),
            Size = new Size(140, 28),
            Font = new Font("Segoe UI", 11),
            Minimum = 0,
            Maximum = 100000,
            DecimalPlaces = 2,
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White,
            Value = 0
        };
        numCash.ValueChanged += (s, e) => CalculateSplitPayment();
        pnlSplitPayment.Controls.Add(numCash);

        // Card amount
        var lblCard = new Label
        {
            Text = "💳 Kart Tutar:",
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.White,
            Location = new Point(15, 55),
            AutoSize = true
        };
        pnlSplitPayment.Controls.Add(lblCard);

        numCard = new NumericUpDown
        {
            Location = new Point(160, 52),
            Size = new Size(140, 28),
            Font = new Font("Segoe UI", 11),
            Minimum = 0,
            Maximum = 100000,
            DecimalPlaces = 2,
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White,
            Value = 0
        };
        numCard.ValueChanged += (s, e) => CalculateSplitPayment();
        pnlSplitPayment.Controls.Add(numCard);

        // Remaining amount
        var lblRemainingTitle = new Label
        {
            Text = "Kalan Tutar:",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(15, 95),
            AutoSize = true
        };
        pnlSplitPayment.Controls.Add(lblRemainingTitle);

        lblRemaining = new Label
        {
            Text = $"₺{totalToPay:N2}",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.FromArgb(220, 80, 80),
            Location = new Point(160, 92),
            AutoSize = true
        };
        pnlSplitPayment.Controls.Add(lblRemaining);

        // Change for split payment
        var lblChangeTitle = new Label
        {
            Text = "Para Üstü:",
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.White,
            Location = new Point(15, 130),
            AutoSize = true
        };
        pnlSplitPayment.Controls.Add(lblChangeTitle);

        lblChange = new Label
        {
            Text = "₺0,00",
            Font = new Font("Segoe UI", 12),
            ForeColor = Color.FromArgb(255, 200, 100),
            Location = new Point(160, 128),
            AutoSize = true
        };
        pnlSplitPayment.Controls.Add(lblChange);

        this.Controls.Add(pnlSplitPayment);

        // Single payment panel (when split is off)
        var pnlSinglePayment = new Panel
        {
            Name = "pnlSinglePayment",
            Location = new Point(20, y),
            Size = new Size(460, 130),
            BackColor = Color.Transparent,
            Visible = true
        };

        // Payment method
        var lblMethod = new Label
        {
            Text = "Ödeme Yöntemi:",
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.White,
            Location = new Point(0, 5),
            AutoSize = true
        };
        pnlSinglePayment.Controls.Add(lblMethod);

        var rbCash = new RadioButton
        {
            Name = "rbCash",
            Text = "💵 Nakit",
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.White,
            Location = new Point(0, 30),
            AutoSize = true,
            Checked = true
        };
        pnlSinglePayment.Controls.Add(rbCash);

        var rbCard = new RadioButton
        {
            Name = "rbCard",
            Text = "💳 Kredi Kartı",
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.White,
            Location = new Point(140, 30),
            AutoSize = true
        };
        pnlSinglePayment.Controls.Add(rbCard);

        // Received amount for single payment
        var lblReceived = new Label
        {
            Text = "Alınan Tutar (₺):",
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.White,
            Location = new Point(0, 65),
            AutoSize = true
        };
        pnlSinglePayment.Controls.Add(lblReceived);

        var numReceived = new NumericUpDown
        {
            Name = "numReceived",
            Location = new Point(160, 62),
            Size = new Size(140, 28),
            Font = new Font("Segoe UI", 11),
            Minimum = 0,
            Maximum = 100000,
            DecimalPlaces = 2,
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White,
            Value = _order.TotalAmount
        };
        numReceived.ValueChanged += (s, e) => CalculateSinglePaymentChange();
        pnlSinglePayment.Controls.Add(numReceived);

        // Change for single payment
        var lblChangeSingle = new Label
        {
            Text = "Para Üstü:",
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.White,
            Location = new Point(0, 100),
            AutoSize = true
        };
        pnlSinglePayment.Controls.Add(lblChangeSingle);

        var lblChangeSingleValue = new Label
        {
            Name = "lblChangeSingle",
            Text = "₺0,00",
            Font = new Font("Segoe UI", 12),
            ForeColor = Color.FromArgb(255, 200, 100),
            Location = new Point(160, 98),
            AutoSize = true
        };
        pnlSinglePayment.Controls.Add(lblChangeSingleValue);

        rbCard.CheckedChanged += (s, e) => {
            if (rbCard.Checked)
            {
                numReceived.Value = totalToPay;
                numReceived.Enabled = false;
            }
            else
            {
                numReceived.Enabled = true;
            }
        };

        this.Controls.Add(pnlSinglePayment);
        y += 190;

        // Pay button
        btnPay = new Button
        {
            Text = "✅ ÖDEME YAP",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Size = new Size(460, 55),
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

    private void ChkSplitPayment_CheckedChanged(object? sender, EventArgs e)
    {
        pnlSplitPayment.Visible = chkSplitPayment.Checked;
        var pnlSingle = this.Controls.Find("pnlSinglePayment", false).FirstOrDefault();
        if (pnlSingle != null)
        {
            pnlSingle.Visible = !chkSplitPayment.Checked;
        }

        if (chkSplitPayment.Checked)
        {
            numCash.Value = 0;
            numCard.Value = 0;
            CalculateSplitPayment();
        }
    }

    private void CalculateSplitPayment()
    {
        var total = totalToPay;
        var paid = numCash.Value + numCard.Value;
        var remaining = total - paid;
        
        if (remaining < 0)
        {
            lblRemaining.Text = "₺0,00";
            lblRemaining.ForeColor = Color.FromArgb(50, 180, 100);
            lblChange.Text = $"₺{Math.Abs(remaining):N2}";
        }
        else if (remaining == 0)
        {
            lblRemaining.Text = "₺0,00";
            lblRemaining.ForeColor = Color.FromArgb(50, 180, 100);
            lblChange.Text = "₺0,00";
        }
        else
        {
            lblRemaining.Text = $"₺{remaining:N2}";
            lblRemaining.ForeColor = Color.FromArgb(220, 80, 80);
            lblChange.Text = "₺0,00";
        }
    }

    private void CalculateSinglePaymentChange()
    {
        var numReceived = this.Controls.Find("pnlSinglePayment", false).FirstOrDefault()?.Controls.Find("numReceived", false).FirstOrDefault() as NumericUpDown;
        var lblChangeSingle = this.Controls.Find("pnlSinglePayment", false).FirstOrDefault()?.Controls.Find("lblChangeSingle", false).FirstOrDefault() as Label;
        
        if (numReceived != null && lblChangeSingle != null)
        {
            var change = numReceived.Value - totalToPay;
            if (change < 0) change = 0;
            lblChangeSingle.Text = $"₺{change:N2}";
        }
    }

    private Label AddLabel(string text, int x, int y, bool bold = false)
    {
        var lbl = new Label
        {
            Text = text,
            Font = new Font("Segoe UI", bold ? 13 : 11, bold ? FontStyle.Bold : FontStyle.Regular),
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

    private void AddDivider(int y)
    {
        var divider = new Panel
        {
            Location = new Point(20, y),
            Size = new Size(460, 2),
            BackColor = Color.FromArgb(80, 80, 100)
        };
        this.Controls.Add(divider);
    }

    private void CalculateTotal()
    {
        discountAmount = numDiscount.Value;
        totalToPay = _order.TotalAmount - discountAmount;
        if (totalToPay < 0) totalToPay = 0;

        lblTotal.Text = $"₺{totalToPay:N2}";
        
        // Update single payment received amount
        var numReceived = this.Controls.Find("pnlSinglePayment", false).FirstOrDefault()?.Controls.Find("numReceived", false).FirstOrDefault() as NumericUpDown;
        if (numReceived != null)
        {
            numReceived.Value = totalToPay;
        }

        CalculateSplitPayment();
        CalculateSinglePaymentChange();
    }

    private async void BtnPay_Click(object? sender, EventArgs e)
    {
        try
        {
            if (chkSplitPayment.Checked)
            {
                // Split payment
                var cashAmount = numCash.Value;
                var cardAmount = numCard.Value;
                var totalPaid = cashAmount + cardAmount;

                if (totalPaid < totalToPay)
                {
                    MessageBox.Show($"Ödeme tutarı yetersiz!\n\nToplam: ₺{totalToPay:N2}\nÖdenen: ₺{totalPaid:N2}\nKalan: ₺{(totalToPay - totalPaid):N2}", 
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Process split payment - we'll use the primary method based on which is higher
                var primaryMethod = cashAmount >= cardAmount ? PaymentMethod.Cash : PaymentMethod.CreditCard;
                var payment = await Program.PaymentService.ProcessPaymentAsync(
                    _order.Id, primaryMethod, totalPaid, discountAmount);

                if (payment != null)
                {
                    var changeAmount = totalPaid - totalToPay;
                    MessageBox.Show(
                        $"Parçalı ödeme başarıyla alındı!\n\n" +
                        $"Fatura No: {payment.InvoiceNumber}\n" +
                        $"Toplam: ₺{totalToPay:N2}\n" +
                        $"Nakit: ₺{cashAmount:N2}\n" +
                        $"Kart: ₺{cardAmount:N2}\n" +
                        $"Para Üstü: ₺{changeAmount:N2}",
                        "Ödeme Tamamlandı",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            else
            {
                // Single payment
                var pnlSingle = this.Controls.Find("pnlSinglePayment", false).FirstOrDefault();
                var rbCash = pnlSingle?.Controls.Find("rbCash", false).FirstOrDefault() as RadioButton;
                var numReceived = pnlSingle?.Controls.Find("numReceived", false).FirstOrDefault() as NumericUpDown;

                if (numReceived == null) return;

                if (numReceived.Value < totalToPay)
                {
                    MessageBox.Show("Alınan tutar toplam tutardan az olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var method = rbCash?.Checked == true ? PaymentMethod.Cash : PaymentMethod.CreditCard;
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
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ödeme işlenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
