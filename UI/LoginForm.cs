using System;
using System.Windows.Forms;
using BLL.Interface;
using DAL.Entities;

namespace UI
{
    public partial class LoginForm : Form
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly IPromotionService _promotionService;
        private readonly IMenuService _menuService;

        public LoginForm(
            IUserService userService,
            IOrderService orderService,
            IPromotionService promotionService,
            IMenuService menuService)
        {
            InitializeComponent();
            _userService = userService;
            _orderService = orderService;
            _promotionService = promotionService;
            _menuService = menuService;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            try
            {
                User? loggedInUser =
                    await _userService.LoginAsync(username, password);

                if (loggedInUser == null)
                {
                    MessageBox.Show(
                        "Sai tài khoản hoặc mật khẩu",
                        "Login failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show(
                    $"Xin chào {loggedInUser.FullName}",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                MainForm mainForm = new MainForm(
                    loggedInUser,
                    _orderService,
                    _promotionService,
                    _menuService,
                    _userService
                );

                this.Hide();
                mainForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
