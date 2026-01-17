using System.Drawing;
using System.Windows.Forms;

namespace UI
{
    partial class MainForm
    {
        private MenuStrip menuStrip;
        private ToolStripMenuItem menuCreateOrder;
        private ToolStripMenuItem menuHistory;
        private ToolStripMenuItem menuMenu;
        private ToolStripMenuItem menuPromotion;
        private ToolStripMenuItem menuUser;
        private ToolStripMenuItem menuStatistics;
        private ToolStripMenuItem menuLogout;

        private Panel panelTop;
        private Label lblUserInfo;
        private Panel panelContent;

        private void InitializeComponent()
        {
            this.Text = "Coffee Management";
            this.WindowState = FormWindowState.Maximized;

            // ===== MENU =====
            menuStrip = new MenuStrip();

            menuCreateOrder = new ToolStripMenuItem("Tạo hóa đơn");
            menuHistory = new ToolStripMenuItem("Lịch sử");
            menuMenu = new ToolStripMenuItem("Quản lý menu");
            menuPromotion = new ToolStripMenuItem("Khuyến mãi");
            menuUser = new ToolStripMenuItem("Nhân viên");
            menuStatistics = new ToolStripMenuItem("Thống kê");
            menuLogout = new ToolStripMenuItem("Đăng xuất");

            menuStrip.Items.AddRange(new ToolStripItem[]
            {
                menuCreateOrder,
                menuHistory,
                menuMenu,
                menuPromotion,
                menuUser,
                menuStatistics,
                menuLogout
            });

            // ===== TOP PANEL =====
            panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.WhiteSmoke
            };

            lblUserInfo = new Label
            {
                Dock = DockStyle.Right,
                Width = 400,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 10, 0)
            };

            panelTop.Controls.Add(lblUserInfo);

            // ===== CONTENT =====
            panelContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // ===== FORM =====
            this.Controls.Add(panelContent);
            this.Controls.Add(panelTop);
            this.Controls.Add(menuStrip);
            this.MainMenuStrip = menuStrip;
        }
    }
}
