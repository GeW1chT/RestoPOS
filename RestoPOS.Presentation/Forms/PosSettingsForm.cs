using RestoPOS.Business.PosIntegration;
using RestoPOS.Common.Enums;

namespace RestoPOS.Presentation.Forms;

/// <summary>
/// POS Cihaz Ayarları Formu
/// </summary>
public partial class PosSettingsForm : Form
{
    private ComboBox cmbDeviceType = null!;
    private ComboBox cmbPort = null!;
    private TextBox txtIpAddress = null!;
    private TextBox txtPortNumber = null!;
    private TextBox txtBaudRate = null!;
    private Label lblStatus = null!;
    private Button btnConnect = null!;
    private Button btnTest = null!;
    private Button btnDisconnect = null!;
    private Panel pnlSerialSettings = null!;
    private Panel pnlTcpSettings = null!;

    public PosSettingsForm()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        this.Text = "POS Cihaz Ayarları";
        this.Size = new Size(500, 500);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.FromArgb(30, 30, 40);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        int y = 20;

        // Title
        var lblTitle = new Label
        {
            Text = "POS Cihaz Ayarları",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 165, 0),
            Location = new Point(20, y),
            AutoSize = true
        };
        this.Controls.Add(lblTitle);
        y += 50;

        // Device Type
        AddLabel("Cihaz Türü:", 20, y);
        cmbDeviceType = new ComboBox
        {
            Location = new Point(160, y - 3),
            Size = new Size(280, 28),
            Font = new Font("Segoe UI", 11),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White
        };
        cmbDeviceType.Items.Add(new DeviceTypeItem(PosDeviceType.None, "Seçiniz..."));
        cmbDeviceType.Items.Add(new DeviceTypeItem(PosDeviceType.Pavo, "Pavo POS (Serial)"));
        cmbDeviceType.Items.Add(new DeviceTypeItem(PosDeviceType.InPos, "InPOS (TCP/IP)"));
        cmbDeviceType.Items.Add(new DeviceTypeItem(PosDeviceType.Hugin, "Hugin POS (Serial)"));
        cmbDeviceType.DisplayMember = "Name";
        cmbDeviceType.SelectedIndex = 0;
        cmbDeviceType.SelectedIndexChanged += CmbDeviceType_SelectedIndexChanged;
        this.Controls.Add(cmbDeviceType);
        y += 45;

        // Serial Port Settings Panel
        pnlSerialSettings = new Panel
        {
            Location = new Point(20, y),
            Size = new Size(440, 100),
            BackColor = Color.FromArgb(40, 40, 55),
            Visible = false
        };

        var lblPort = new Label
        {
            Text = "COM Port:",
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.White,
            Location = new Point(15, 20),
            AutoSize = true
        };
        pnlSerialSettings.Controls.Add(lblPort);

        cmbPort = new ComboBox
        {
            Location = new Point(140, 17),
            Size = new Size(120, 28),
            Font = new Font("Segoe UI", 11),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White
        };
        pnlSerialSettings.Controls.Add(cmbPort);

        var btnRefreshPorts = new Button
        {
            Text = "🔄",
            Font = new Font("Segoe UI", 10),
            Size = new Size(35, 28),
            Location = new Point(265, 17),
            BackColor = Color.FromArgb(60, 60, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnRefreshPorts.FlatAppearance.BorderSize = 0;
        btnRefreshPorts.Click += (s, e) => RefreshComPorts();
        pnlSerialSettings.Controls.Add(btnRefreshPorts);

        var lblBaud = new Label
        {
            Text = "Baud Rate:",
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.White,
            Location = new Point(15, 60),
            AutoSize = true
        };
        pnlSerialSettings.Controls.Add(lblBaud);

        txtBaudRate = new TextBox
        {
            Location = new Point(140, 57),
            Size = new Size(120, 28),
            Font = new Font("Segoe UI", 11),
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White,
            Text = "9600"
        };
        pnlSerialSettings.Controls.Add(txtBaudRate);

        this.Controls.Add(pnlSerialSettings);

        // TCP/IP Settings Panel
        pnlTcpSettings = new Panel
        {
            Location = new Point(20, y),
            Size = new Size(440, 100),
            BackColor = Color.FromArgb(40, 40, 55),
            Visible = false
        };

        var lblIp = new Label
        {
            Text = "IP Adresi:",
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.White,
            Location = new Point(15, 20),
            AutoSize = true
        };
        pnlTcpSettings.Controls.Add(lblIp);

        txtIpAddress = new TextBox
        {
            Location = new Point(140, 17),
            Size = new Size(180, 28),
            Font = new Font("Segoe UI", 11),
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White,
            Text = "192.168.1.100"
        };
        pnlTcpSettings.Controls.Add(txtIpAddress);

        var lblTcpPort = new Label
        {
            Text = "Port:",
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.White,
            Location = new Point(15, 60),
            AutoSize = true
        };
        pnlTcpSettings.Controls.Add(lblTcpPort);

        txtPortNumber = new TextBox
        {
            Location = new Point(140, 57),
            Size = new Size(100, 28),
            Font = new Font("Segoe UI", 11),
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White,
            Text = "8080"
        };
        pnlTcpSettings.Controls.Add(txtPortNumber);

        this.Controls.Add(pnlTcpSettings);
        y += 120;

        // Status
        var lblStatusTitle = new Label
        {
            Text = "Durum:",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(20, y),
            AutoSize = true
        };
        this.Controls.Add(lblStatusTitle);

        lblStatus = new Label
        {
            Text = "Bağlı değil",
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.FromArgb(220, 80, 80),
            Location = new Point(100, y),
            AutoSize = true
        };
        this.Controls.Add(lblStatus);
        y += 50;

        // Buttons
        btnConnect = new Button
        {
            Text = "🔌 Bağlan",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Size = new Size(140, 45),
            Location = new Point(20, y),
            BackColor = Color.FromArgb(50, 180, 100),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnConnect.FlatAppearance.BorderSize = 0;
        btnConnect.Click += BtnConnect_Click;
        this.Controls.Add(btnConnect);

        btnTest = new Button
        {
            Text = "🧪 Test",
            Font = new Font("Segoe UI", 12),
            Size = new Size(120, 45),
            Location = new Point(170, y),
            BackColor = Color.FromArgb(255, 165, 0),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Enabled = false
        };
        btnTest.FlatAppearance.BorderSize = 0;
        btnTest.Click += BtnTest_Click;
        this.Controls.Add(btnTest);

        btnDisconnect = new Button
        {
            Text = "❌ Bağlantıyı Kes",
            Font = new Font("Segoe UI", 12),
            Size = new Size(140, 45),
            Location = new Point(300, y),
            BackColor = Color.FromArgb(220, 80, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Enabled = false
        };
        btnDisconnect.FlatAppearance.BorderSize = 0;
        btnDisconnect.Click += BtnDisconnect_Click;
        this.Controls.Add(btnDisconnect);
        y += 70;

        // Info
        var lblInfo = new Label
        {
            Text = "ℹ️ Pavo ve Hugin için COM port, InPOS için IP adresi gereklidir.",
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(150, 150, 150),
            Location = new Point(20, y),
            AutoSize = true
        };
        this.Controls.Add(lblInfo);
        y += 25;

        var lblInfo2 = new Label
        {
            Text = "📌 Bağlantı ayarları kaydedilir ve uygulama yeniden başlatıldığında hatırlanır.",
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(150, 150, 150),
            Location = new Point(20, y),
            AutoSize = true
        };
        this.Controls.Add(lblInfo2);

        // Close button
        var btnClose = new Button
        {
            Text = "Kapat",
            Font = new Font("Segoe UI", 11),
            Size = new Size(100, 40),
            Location = new Point(380, this.ClientSize.Height - 60),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            BackColor = Color.FromArgb(60, 60, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.Click += (s, e) => this.Close();
        this.Controls.Add(btnClose);

        this.ResumeLayout(false);
    }

    private void LoadSettings()
    {
        RefreshComPorts();
        UpdateConnectionStatus();
    }

    private void RefreshComPorts()
    {
        cmbPort.Items.Clear();
        var ports = PosDeviceManager.GetAvailableComPorts();
        foreach (var port in ports)
        {
            cmbPort.Items.Add(port);
        }
        if (cmbPort.Items.Count > 0)
        {
            cmbPort.SelectedIndex = 0;
        }
    }

    private void CmbDeviceType_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var selectedItem = cmbDeviceType.SelectedItem as DeviceTypeItem;
        if (selectedItem == null) return;

        var deviceType = selectedItem.Type;

        // Show appropriate settings panel
        pnlSerialSettings.Visible = deviceType == PosDeviceType.Pavo || deviceType == PosDeviceType.Hugin;
        pnlTcpSettings.Visible = deviceType == PosDeviceType.InPos;

        // Set default baud rate
        if (deviceType == PosDeviceType.Hugin)
        {
            txtBaudRate.Text = "115200";
        }
        else if (deviceType == PosDeviceType.Pavo)
        {
            txtBaudRate.Text = "9600";
        }
    }

    private void UpdateConnectionStatus()
    {
        if (PosDeviceManager.Instance.HasActiveDevice)
        {
            lblStatus.Text = $"✅ {PosDeviceManager.Instance.ActiveDevice?.DeviceName} - Bağlı";
            lblStatus.ForeColor = Color.FromArgb(50, 180, 100);
            btnConnect.Enabled = false;
            btnTest.Enabled = true;
            btnDisconnect.Enabled = true;
        }
        else
        {
            lblStatus.Text = "❌ Bağlı değil";
            lblStatus.ForeColor = Color.FromArgb(220, 80, 80);
            btnConnect.Enabled = true;
            btnTest.Enabled = false;
            btnDisconnect.Enabled = false;
        }
    }

    private async void BtnConnect_Click(object? sender, EventArgs e)
    {
        var selectedItem = cmbDeviceType.SelectedItem as DeviceTypeItem;
        if (selectedItem == null || selectedItem.Type == PosDeviceType.None)
        {
            MessageBox.Show("Lütfen bir cihaz türü seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string connectionString;
        if (selectedItem.Type == PosDeviceType.InPos)
        {
            connectionString = $"{txtIpAddress.Text}:{txtPortNumber.Text}";
        }
        else
        {
            if (cmbPort.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir COM port seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            connectionString = $"{cmbPort.SelectedItem};{txtBaudRate.Text}";
        }

        lblStatus.Text = "⏳ Bağlanıyor...";
        lblStatus.ForeColor = Color.FromArgb(255, 200, 100);
        btnConnect.Enabled = false;

        try
        {
            var success = await PosDeviceManager.Instance.ConnectAsync(selectedItem.Type, connectionString);
            
            if (success)
            {
                MessageBox.Show($"{selectedItem.Name} cihazına başarıyla bağlanıldı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Save settings
                await Program.SettingsService.SetValueAsync("PosDeviceType", selectedItem.Type.ToString());
                await Program.SettingsService.SetValueAsync("PosConnectionString", connectionString);
            }
            else
            {
                MessageBox.Show("Cihaza bağlanılamadı. Ayarları kontrol edin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Bağlantı hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        UpdateConnectionStatus();
    }

    private async void BtnTest_Click(object? sender, EventArgs e)
    {
        lblStatus.Text = "⏳ Test ediliyor...";
        lblStatus.ForeColor = Color.FromArgb(255, 200, 100);

        try
        {
            var success = await PosDeviceManager.Instance.TestConnectionAsync();
            
            if (success)
            {
                MessageBox.Show("Bağlantı testi başarılı! ✅", "Test Sonucu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Bağlantı testi başarısız. Cihazı kontrol edin.", "Test Sonucu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Test hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        UpdateConnectionStatus();
    }

    private async void BtnDisconnect_Click(object? sender, EventArgs e)
    {
        await PosDeviceManager.Instance.DisconnectAsync();
        MessageBox.Show("Bağlantı kesildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        UpdateConnectionStatus();
    }

    private Label AddLabel(string text, int x, int y)
    {
        var lbl = new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.White,
            Location = new Point(x, y),
            AutoSize = true
        };
        this.Controls.Add(lbl);
        return lbl;
    }

    private class DeviceTypeItem
    {
        public PosDeviceType Type { get; }
        public string Name { get; }

        public DeviceTypeItem(PosDeviceType type, string name)
        {
            Type = type;
            Name = name;
        }

        public override string ToString() => Name;
    }
}
