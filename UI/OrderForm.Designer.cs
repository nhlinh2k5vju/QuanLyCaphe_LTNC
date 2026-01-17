using System.Windows.Forms;
using System.Drawing;

namespace UI
{
    partial class OrderForm
    {
        private Panel panelTop;
        private TextBox txtSearch;
        private ComboBox cboCategory;
        private ComboBox cboPromotion;
        private Button btnClear;
        private Button btnCheckout;

        private SplitContainer splitMain;

        private Panel panelMenuScroll;
        private TableLayoutPanel tblMenu;

        private DataGridView dgvBill;
        private Label lblSubTotal;
        private Label lblDiscount;
        private Label lblTotal;

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;

            #region TOP BAR
            panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = Color.WhiteSmoke
            };

            txtSearch = new TextBox { Left = 10, Width = 180, PlaceholderText = "Tìm món..." };
            cboCategory = new ComboBox { Left = 200, Width = 140 };
            cboPromotion = new ComboBox { Left = 350, Width = 180 };

            btnClear = new Button { Text = "HỦY", Left = 540, Width = 90 };
            btnCheckout = new Button { Text = "THANH TOÁN", Left = 640, Width = 120 };

            panelTop.Controls.AddRange(new Control[]
            {
                txtSearch, cboCategory, cboPromotion, btnClear, btnCheckout
            });
            #endregion

            #region MAIN SPLIT
            splitMain = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = (int)(Screen.PrimaryScreen.Bounds.Width * 0.6),
                IsSplitterFixed = true
            };
            #endregion

            #region LEFT - MENU LIST
            panelMenuScroll = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            tblMenu = new TableLayoutPanel
            {
                ColumnCount = 3,
                Dock = DockStyle.Top,
                AutoSize = true
            };

            for (int i = 0; i < 3; i++)
                tblMenu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

            panelMenuScroll.Controls.Add(tblMenu);
            splitMain.Panel1.Controls.Add(panelMenuScroll);
            #endregion

            #region RIGHT - BILL
            dgvBill = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 320,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvBill.Columns.Add("Name", "Tên món");
            dgvBill.Columns.Add("Size", "Size");
            dgvBill.Columns.Add("Qty", "SL");
            dgvBill.Columns.Add("Price", "Giá");

            var colRemove = new DataGridViewButtonColumn
            {
                Text = "X",
                UseColumnTextForButtonValue = true,
                Width = 40
            };
            dgvBill.Columns.Add(colRemove);

            lblSubTotal = new Label { Dock = DockStyle.Top, Height = 30 };
            lblDiscount = new Label { Dock = DockStyle.Top, Height = 30 };
            lblTotal = new Label
            {
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };

            splitMain.Panel2.Controls.AddRange(new Control[]
            {
                lblTotal, lblDiscount, lblSubTotal, dgvBill
            });
            #endregion

            this.Controls.Add(splitMain);
            this.Controls.Add(panelTop);
        }
    }
}
