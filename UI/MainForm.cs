using System;
using System.Windows.Forms;
using DAL.Entities;
using BLL.Interface;

namespace UI
{
    public partial class MainForm : Form
    {
        private readonly User _currentUser;
        private readonly IOrderService _orderService;
        private readonly IPromotionService _promotionService;
        private readonly IMenuService _menuService;
        private readonly IUserService _userService;

        private Form _currentForm;

        public MainForm(
            User currentUser,
            IOrderService orderService,
            IPromotionService promotionService,
            IMenuService menuService,
            IUserService userService)
        {
            InitializeComponent();

            _currentUser = currentUser;
            _orderService = orderService;
            _promotionService = promotionService;
            _menuService = menuService;
            _userService = userService;

            DisplayUserInfo();
            RegisterMenuEvents();
            ApplyAuthorization();
        }

        // ================= USER INFO =================

        private void DisplayUserInfo()
        {
            lblUserInfo.Text =
                $"Nhân viên: {_currentUser.FullName} | Quyền: {_currentUser.Role}";
        }

        // ================= PHÂN QUYỀN =================

        private void ApplyAuthorization()
        {
            bool isAdmin = _currentUser.Role == "ADMIN";

            // Ai cũng dùng
            menuCreateOrder.Visible = true;
            menuHistory.Visible = true;
            menuLogout.Visible = true;

            // Admin only
            menuMenu.Visible = isAdmin;
            menuPromotion.Visible = isAdmin;
            menuUser.Visible = isAdmin;
            menuStatistics.Visible = isAdmin;
        }

        // ================= MENU EVENTS =================

        private void RegisterMenuEvents()
        {
            menuCreateOrder.Click += (s, e) =>
                LoadForm(new OrderForm(
                    _currentUser,
                    _orderService,
                    _promotionService,
                    _menuService));

            menuHistory.Click += (s, e) =>
                LoadForm(new OrderHistoryForm(_orderService));

            menuMenu.Click += (s, e) =>
            {
                if (_currentUser.Role != "ADMIN")
                {
                    MessageBox.Show("Bạn không có quyền");
                    return;
                }
                LoadForm(new MenuManagementForm(_menuService));
            };

            menuPromotion.Click += (s, e) =>
            {
                if (_currentUser.Role != "ADMIN")
                {
                    MessageBox.Show("Bạn không có quyền");
                    return;
                }
                LoadForm(new PromotionManagementForm(_promotionService));
            };

            menuUser.Click += (s, e) =>
            {
                if (_currentUser.Role != "ADMIN")
                {
                    MessageBox.Show("Bạn không có quyền");
                    return;
                }
                LoadForm(new UserManagementForm(_userService));
            };

            menuStatistics.Click += (s, e) =>
            {
                if (_currentUser.Role != "ADMIN")
                {
                    MessageBox.Show("Bạn không có quyền");
                    return;
                }
                LoadForm(new StatisticsForm(_orderService));
            };

            menuLogout.Click += MenuLogout_Click;
        }

        // ================= LOAD CHILD FORM =================

        private void LoadForm(Form form)
        {
            if (_currentForm != null)
                _currentForm.Close();

            _currentForm = form;

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            panelContent.Controls.Clear();
            panelContent.Controls.Add(form);
            form.Show();
        }

        // ================= LOGOUT =================

        private void MenuLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "Bạn có chắc muốn đăng xuất?",
                "Xác nhận",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Hide();
                new LoginForm(
                    _userService,
                    _orderService,
                    _promotionService,
                    _menuService
                ).Show();
            }
        }
    }
}
