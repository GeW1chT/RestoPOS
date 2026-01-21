namespace RestoPOS.Presentation.Forms;

/// <summary>
/// Giriş formu
/// </summary>
public partial class LoginForm : Form
{
    private TextBox txtUsername = null!;
    private TextBox txtPassword = null!;
    private Button btnLogin = null!;
    private Label lblTitle = null!;
    private Label lblError = null!;
    private Panel pnlMain = null!;

    public LoginForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        
        // Form settings
        this.Text = "RestoPOS - Giriş";
        this.Size = new Size(450, 400);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.BackColor = Color.FromArgb(30, 30, 40);
        this.Icon = SystemIcons.Application;

        // Main panel
        pnlMain = new Panel
        {
            Size = new Size(350, 300),
            Location = new Point(50, 40),
            BackColor = Color.FromArgb(45, 45, 60)
        };

        // Title label
        lblTitle = new Label
        {
            Text = "🍽️ RestoPOS",
            Font = new Font("Segoe UI", 24, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 165, 0),
            AutoSize = false,
            Size = new Size(350, 50),
            Location = new Point(0, 20),
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlMain.Controls.Add(lblTitle);

        // Username label
        var lblUsername = new Label
        {
            Text = "Kullanıcı Adı",
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.White,
            Location = new Point(30, 90),
            AutoSize = true
        };
        pnlMain.Controls.Add(lblUsername);

        // Username textbox
        txtUsername = new TextBox
        {
            Font = new Font("Segoe UI", 12),
            Size = new Size(290, 30),
            Location = new Point(30, 115),
            BackColor = Color.FromArgb(60, 60, 80),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };
        pnlMain.Controls.Add(txtUsername);

        // Password label
        var lblPassword = new Label
        {
            Text = "Şifre",
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.White,
            Location = new Point(30, 155),
            AutoSize = true
        };
        pnlMain.Controls.Add(lblPassword);

        // Password textbox
        txtPassword = new TextBox
        {
            Font = new Font("Segoe UI", 12),
            Size = new Size(290, 30),
            Location = new Point(30, 180),
            BackColor = Color.FromArgb(60, 60, 80),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            UseSystemPasswordChar = true
        };
        txtPassword.KeyDown += TxtPassword_KeyDown;
        pnlMain.Controls.Add(txtPassword);

        // Error label
        lblError = new Label
        {
            Text = "",
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(255, 100, 100),
            Location = new Point(30, 220),
            Size = new Size(290, 20),
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlMain.Controls.Add(lblError);

        // Login button
        btnLogin = new Button
        {
            Text = "GİRİŞ YAP",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Size = new Size(290, 45),
            Location = new Point(30, 245),
            BackColor = Color.FromArgb(255, 165, 0),
            ForeColor = Color.Black,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.Click += BtnLogin_Click;
        pnlMain.Controls.Add(btnLogin);

        this.Controls.Add(pnlMain);
        this.ResumeLayout(false);
        
        // Set focus to username
        this.Shown += (s, e) => txtUsername.Focus();
    }

    private void TxtPassword_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            BtnLogin_Click(sender, e);
            e.SuppressKeyPress = true;
        }
    }

    private async void BtnLogin_Click(object? sender, EventArgs e)
    {
        lblError.Text = "";
        
        if (string.IsNullOrWhiteSpace(txtUsername.Text))
        {
            lblError.Text = "Kullanıcı adı gereklidir.";
            txtUsername.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            lblError.Text = "Şifre gereklidir.";
            txtPassword.Focus();
            return;
        }

        btnLogin.Enabled = false;
        btnLogin.Text = "Giriş yapılıyor...";

        try
        {
            var user = await Program.UserService.LoginAsync(txtUsername.Text, txtPassword.Text);
            
            if (user == null)
            {
                lblError.Text = "Kullanıcı adı veya şifre hatalı.";
                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            Program.CurrentUser = user;
            
            this.Hide();
            var mainForm = new MainForm();
            mainForm.FormClosed += (s, args) => this.Close();
            mainForm.Show();
        }
        catch (Exception ex)
        {
            lblError.Text = $"Hata: {ex.Message}";
        }
        finally
        {
            btnLogin.Enabled = true;
            btnLogin.Text = "GİRİŞ YAP";
        }
    }
}
