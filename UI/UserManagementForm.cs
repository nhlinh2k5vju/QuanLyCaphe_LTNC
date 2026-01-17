using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL.Interface;
using DAL.Entities;

namespace UI
{
    public partial class UserManagementForm : Form
    {
        private readonly IUserService _userService;
        private User? _selectedUser;

        // ===== Controls =====
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtFullName;
        private TextBox txtPhone;
        private ComboBox cboRole;
        private CheckBox chkStatus;

        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;

        private DataGridView dgvUsers;

        public UserManagementForm(IUserService userService)
        {
            _userService = userService;

            InitializeUI();
            _ = LoadUsersAsync();
        }

        // ================= UI =================
        private void InitializeUI()
        {
            Dock = DockStyle.Fill;
            Text = "Quản lý nhân viên";

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3
            };
            Controls.Add(mainLayout);

            // ===== INPUT PANEL =====
            var input = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 4,
                AutoSize = true,
                Padding = new Padding(10)
            };

            // Username
            input.Controls.Add(new Label { Text = "Username" }, 0, 0);
            txtUsername = new TextBox();
            input.Controls.Add(txtUsername, 1, 0);

            // Role
            input.Controls.Add(new Label { Text = "Role" }, 2, 0);
            cboRole = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboRole.Items.AddRange(new[] { "ADMIN", "STAFF" });
            cboRole.SelectedIndex = 0;
            input.Controls.Add(cboRole, 3, 0);

            // Password
            input.Controls.Add(new Label { Text = "Password" }, 0, 1);
            txtPassword = new TextBox { UseSystemPasswordChar = true };
            input.Controls.Add(txtPassword, 1, 1);

            // Status
            chkStatus = new CheckBox { Text = "Hoạt động", Checked = true };
            input.Controls.Add(chkStatus, 3, 1);

            // Full name
            input.Controls.Add(new Label { Text = "Họ tên" }, 0, 2);
            txtFullName = new TextBox();
            input.Controls.Add(txtFullName, 1, 2);

            // Phone
            input.Controls.Add(new Label { Text = "SĐT" }, 0, 3);
            txtPhone = new TextBox();
            input.Controls.Add(txtPhone, 1, 3);

            mainLayout.Controls.Add(input);

            // ===== BUTTON PANEL =====
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Padding = new Padding(10)
            };

            btnAdd = new Button { Text = "Thêm" };
            btnEdit = new Button { Text = "Sửa" };
            btnDelete = new Button { Text = "Xoá" };

            btnPanel.Controls.AddRange(new Control[]
            {
                btnAdd, btnEdit, btnDelete
            });

            mainLayout.Controls.Add(btnPanel);

            // ===== GRID =====
            dgvUsers = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoGenerateColumns = false
            };

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Username",
                DataPropertyName = "Username"
            });

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Họ tên",
                DataPropertyName = "FullName"
            });

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "SĐT",
                DataPropertyName = "PhoneNumber"
            });

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Role",
                DataPropertyName = "Role"
            });

            dgvUsers.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText = "Active",
                DataPropertyName = "Status"
            });

            dgvUsers.CellClick += DgvUsers_CellClick;
            mainLayout.Controls.Add(dgvUsers);

            // ===== EVENTS =====
            btnAdd.Click += async (_, __) => await AddUserAsync();
            btnEdit.Click += async (_, __) => await UpdateUserAsync();
            btnDelete.Click += async (_, __) => await DeleteUserAsync();
        }

        // ================= DATA =================
        private async Task LoadUsersAsync()
        {
            dgvUsers.DataSource = (await _userService.GetAllAsync()).ToList();
            _selectedUser = null;
            ClearInputs();
        }

        private void DgvUsers_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            _selectedUser = dgvUsers.Rows[e.RowIndex].DataBoundItem as User;
            if (_selectedUser == null) return;

            txtUsername.Text = _selectedUser.Username;
            txtUsername.Enabled = false; // không cho sửa username
            txtPassword.Clear(); // không hiển thị password cũ
            txtFullName.Text = _selectedUser.FullName;
            txtPhone.Text = _selectedUser.PhoneNumber;
            cboRole.SelectedItem = _selectedUser.Role;
            chkStatus.Checked = _selectedUser.Status;
        }

        // ================= ACTIONS =================
        private async Task AddUserAsync()
        {
            try
            {
                var user = new User
                {
                    Username = txtUsername.Text.Trim(),
                    PasswordHash = txtPassword.Text,
                    FullName = txtFullName.Text.Trim(),
                    PhoneNumber = txtPhone.Text.Trim(),
                    Role = cboRole.SelectedItem?.ToString(),
                    Status = chkStatus.Checked
                };

                await _userService.AddAsync(user);
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task UpdateUserAsync()
        {
            if (_selectedUser == null) return;

            try
            {
                _selectedUser.FullName = txtFullName.Text.Trim();
                _selectedUser.PhoneNumber = txtPhone.Text.Trim();
                _selectedUser.Role = cboRole.SelectedItem?.ToString();
                _selectedUser.Status = chkStatus.Checked;

                // nếu nhập password mới
                if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                    _selectedUser.PasswordHash = txtPassword.Text;

                await _userService.UpdateAsync(_selectedUser);
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task DeleteUserAsync()
        {
            if (_selectedUser == null) return;

            var confirm = MessageBox.Show(
                "Bạn có chắc muốn xoá nhân viên này?",
                "Xác nhận",
                MessageBoxButtons.YesNo);

            if (confirm == DialogResult.Yes)
            {
                _selectedUser.Status = false;
                await _userService.UpdateAsync(_selectedUser);
                await LoadUsersAsync();
            }
        }

        // ================= UTILS =================
        private void ClearInputs()
        {
            txtUsername.Clear();
            txtUsername.Enabled = true;
            txtPassword.Clear();
            txtFullName.Clear();
            txtPhone.Clear();
            chkStatus.Checked = true;
            cboRole.SelectedIndex = 0;
        }
    }
}
