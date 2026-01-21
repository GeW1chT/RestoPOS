using RestoPOS.Common.Enums;
using RestoPOS.Data.Entities;

namespace RestoPOS.Presentation.Forms;

/// <summary>
/// Kullanıcı düzenleme formu
/// </summary>
public partial class UserEditForm : Form
{
    private readonly int? _userId;
    private User? _user;

    private TextBox txtUsername = null!;
    private TextBox txtFullName = null!;
    private TextBox txtPassword = null!;
    private ComboBox cmbRole = null!;
    private Button btnSave = null!;

    public UserEditForm(int? userId)
    {
        _userId = userId;
        InitializeComponent();
        if (_userId.HasValue)
        {
            LoadUser();
        }
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        this.Text = _userId.HasValue ? "Kullanıcı Düzenle" : "Yeni Kullanıcı";
        this.Size = new Size(400, 380);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.FromArgb(30, 30, 40);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        int y = 20;
        int labelX = 20;
        int inputX = 140;
        int inputWidth = 220;

        // Username
        AddLabel("Kullanıcı Adı:", labelX, y);
        txtUsername = CreateTextBox(inputX, y, inputWidth);
        this.Controls.Add(txtUsername);
        y += 45;

        // Full Name
        AddLabel("Ad Soyad:", labelX, y);
        txtFullName = CreateTextBox(inputX, y, inputWidth);
        this.Controls.Add(txtFullName);
        y += 45;

        // Password
        AddLabel(_userId.HasValue ? "Yeni Şifre:" : "Şifre:", labelX, y);
        txtPassword = CreateTextBox(inputX, y, inputWidth);
        txtPassword.UseSystemPasswordChar = true;
        if (_userId.HasValue)
        {
            txtPassword.PlaceholderText = "(Boş bırakılırsa değişmez)";
        }
        this.Controls.Add(txtPassword);
        y += 45;

        // Role
        AddLabel("Rol:", labelX, y);
        cmbRole = new ComboBox
        {
            Location = new Point(inputX, y),
            Size = new Size(inputWidth, 30),
            Font = new Font("Segoe UI", 11),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White
        };
        cmbRole.Items.Add(new RoleItem { Role = UserRole.Admin, Name = "Yönetici" });
        cmbRole.Items.Add(new RoleItem { Role = UserRole.Waiter, Name = "Garson" });
        cmbRole.Items.Add(new RoleItem { Role = UserRole.Cashier, Name = "Kasiyer" });
        cmbRole.Items.Add(new RoleItem { Role = UserRole.Kitchen, Name = "Mutfak" });
        cmbRole.SelectedIndex = 1;
        cmbRole.DisplayMember = "Name";
        this.Controls.Add(cmbRole);
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

        var btnCancel = new Button
        {
            Text = "❌ İptal",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Size = new Size(100, 45),
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

    private TextBox CreateTextBox(int x, int y, int width)
    {
        return new TextBox
        {
            Location = new Point(x, y),
            Size = new Size(width, 30),
            Font = new Font("Segoe UI", 11),
            BackColor = Color.FromArgb(50, 50, 65),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };
    }

    private async void LoadUser()
    {
        try
        {
            _user = await Program.UserService.GetByIdAsync(_userId!.Value);
            if (_user == null)
            {
                MessageBox.Show("Kullanıcı bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            txtUsername.Text = _user.Username;
            txtFullName.Text = _user.FullName;

            for (int i = 0; i < cmbRole.Items.Count; i++)
            {
                if (cmbRole.Items[i] is RoleItem item && item.Role == _user.Role)
                {
                    cmbRole.SelectedIndex = i;
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Kullanıcı yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtUsername.Text))
        {
            MessageBox.Show("Kullanıcı adı gereklidir.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(txtFullName.Text))
        {
            MessageBox.Show("Ad soyad gereklidir.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!_userId.HasValue && string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            MessageBox.Show("Şifre gereklidir.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cmbRole.SelectedItem is not RoleItem roleItem)
        {
            MessageBox.Show("Lütfen bir rol seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            if (_userId.HasValue)
            {
                await Program.UserService.UpdateAsync(_userId.Value, txtFullName.Text.Trim(), roleItem.Role);
                
                if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    await Program.UserService.ResetPasswordAsync(_userId.Value, txtPassword.Text);
                }
            }
            else
            {
                await Program.UserService.CreateAsync(
                    txtUsername.Text.Trim(),
                    txtPassword.Text,
                    txtFullName.Text.Trim(),
                    roleItem.Role);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Kayıt sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private class RoleItem
    {
        public UserRole Role { get; set; }
        public string Name { get; set; } = string.Empty;
        public override string ToString() => Name;
    }
}
