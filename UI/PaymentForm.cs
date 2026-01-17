using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using UI.Models;

namespace UI
{
    public partial class PaymentForm : Form
    {
        private readonly PaymentSummary _summary;

        // Controls
        private TableLayoutPanel tlpMain;
        private Panel pnlLeft, pnlRight, pnlFooter;
        private Label lblStaff, lblTime, lblSubTotal, lblDiscount, lblTotal;
        private DataGridView dgvInvoice;
        private PictureBox picQR;
        private Button btnEdit, btnCancel, btnConfirm;

        public PaymentForm(PaymentSummary summary)
        {
            // Kiểm tra dữ liệu đầu vào để tránh crash
            _summary = summary ?? new PaymentSummary();

            // 1. Dựng giao diện trước
            InitializeComponentManual();

            // 2. Load dữ liệu
            LoadInvoice();

            // 3. Load QR
            GenerateDynamicQR();
        }

        private void InitializeComponentManual()
        {
            this.Text = "Thanh toán & Xuất hóa đơn";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            // Layout
            tlpMain = new TableLayoutPanel();
            tlpMain.Dock = DockStyle.Fill;
            tlpMain.ColumnCount = 2;
            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75F));
            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            this.Controls.Add(tlpMain);

            // Left Panel
            pnlLeft = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            tlpMain.Controls.Add(pnlLeft, 0, 0);

            // Labels Header
            lblStaff = new Label { AutoSize = true, Font = new Font("Segoe UI", 11), Location = new Point(20, 10) };
            lblTime = new Label { AutoSize = true, Font = new Font("Segoe UI", 11), Location = new Point(20, 35) };
            pnlLeft.Controls.Add(lblStaff);
            pnlLeft.Controls.Add(lblTime);

            // GridView
            dgvInvoice = new DataGridView
            {
                Location = new Point(20, 70),
                Width = 760,
                Height = 350,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 11)
            };
            dgvInvoice.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên món", FillWeight = 50 });
            dgvInvoice.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Size", FillWeight = 15 });
            dgvInvoice.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "SL", FillWeight = 10 });
            var colPrice = new DataGridViewTextBoxColumn { HeaderText = "Thành tiền", FillWeight = 25 };
            colPrice.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvInvoice.Columns.Add(colPrice);
            pnlLeft.Controls.Add(dgvInvoice);

            // Footer
            pnlFooter = new Panel
            {
                Height = 260,
                Dock = DockStyle.Bottom,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlLeft.Controls.Add(pnlFooter);

            lblSubTotal = new Label { AutoSize = true, Font = new Font("Segoe UI", 11), Location = new Point(20, 15) };
            lblDiscount = new Label { AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Italic), ForeColor = Color.Red, Location = new Point(20, 45) };
            lblTotal = new Label { AutoSize = true, Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.Blue, Location = new Point(20, 80) };

            pnlFooter.Controls.Add(lblSubTotal);
            pnlFooter.Controls.Add(lblDiscount);
            pnlFooter.Controls.Add(lblTotal);

            picQR = new PictureBox
            {
                Size = new Size(180, 180),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top
            };
            picQR.Location = new Point((pnlFooter.Width - picQR.Width) / 2, 40);
            pnlFooter.Controls.Add(picQR);
            pnlFooter.Resize += (s, e) => { picQR.Left = (pnlFooter.Width - picQR.Width) / 2; };

            // Right Panel (Buttons)
            pnlRight = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10), BackColor = Color.WhiteSmoke };
            tlpMain.Controls.Add(pnlRight, 1, 0);

            var btnFont = new Font("Segoe UI", 10, FontStyle.Bold);
            btnEdit = new Button { Text = "QUAY LẠI SỬA", Size = new Size(220, 55), Location = new Point(15, 70), Font = btnFont, BackColor = Color.LightYellow };
            btnCancel = new Button { Text = "HỦY HÓA ĐƠN", Size = new Size(220, 55), Location = new Point(15, 145), Font = btnFont, BackColor = Color.MistyRose, ForeColor = Color.DarkRed };
            btnConfirm = new Button { Text = "THANH TOÁN (IN)", Size = new Size(220, 80), Location = new Point(15, 220), Font = new Font("Segoe UI", 12, FontStyle.Bold), BackColor = Color.LightGreen, ForeColor = Color.DarkGreen };

            pnlRight.Controls.Add(btnEdit);
            pnlRight.Controls.Add(btnCancel);
            pnlRight.Controls.Add(btnConfirm);

            btnEdit.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            btnCancel.Click += BtnCancel_Click;
            btnConfirm.Click += (s, e) => { DialogResult = DialogResult.OK; Close(); };
        }

        private void LoadInvoice()
        {
            // Hiển thị thông tin Header
            // Nếu StaffName null thì hiện "N/A"
            lblStaff.Text = $"Nhân viên: {_summary.StaffName ?? "N/A"}";

            // Format thời gian
            lblTime.Text = $"Thời gian: {_summary.CreatedTime:HH:mm - dd/MM/yyyy}";

            // Đổ dữ liệu vào Grid
            dgvInvoice.Rows.Clear();
            if (_summary.Items != null)
            {
                foreach (var item in _summary.Items)
                {
                    dgvInvoice.Rows.Add(
                        item.MenuItemName,
                        item.Size,
                        item.Quantity,
                        item.Total.ToString("N0")
                    );
                }
            }

            // Hiển thị Footer
            lblSubTotal.Text = $"Tạm tính:   {_summary.SubTotal:N0} đ";
            lblDiscount.Text = $"Giảm giá:  -{_summary.Discount:N0} đ";

            // Tính tổng tiền cần trả (SubTotal - Discount)
            decimal finalTotal = _summary.SubTotal - _summary.Discount;
            if (finalTotal < 0) finalTotal = 0;

            lblTotal.Text = $"TỔNG CỘNG: {finalTotal:N0} đ";
        }

        private void GenerateDynamicQR()
        {
            try
            {
                // Thông tin chuyển khoản
                string bankCode = "MB";
                string accountNo = "9090905210906";
                string accountName = "NGUYEN HAI LINH";
                decimal finalTotal = _summary.SubTotal - _summary.Discount;
                if (finalTotal < 0) finalTotal = 0;

                string amount = finalTotal.ToString("0");
                string content = $"HD {_summary.CreatedTime:HHmm}";

                string url = $"https://img.vietqr.io/image/{bankCode}-{accountNo}-compact2.png" +
                             $"?amount={amount}" +
                             $"&addInfo={Uri.EscapeDataString(content)}" +
                             $"&accountName={Uri.EscapeDataString(accountName)}";

                picQR.Load(url);
            }
            catch
            {
                picQR.BackColor = Color.LightGray; // Lỗi mạng thì hiện màu xám
            }
        }

        public string SaveQrTempImage()
        {
            if (picQR.Image == null) return "";
            try
            {
                string path = Path.Combine(Path.GetTempPath(), $"qr_{DateTime.Now.Ticks}.png");
                picQR.Image.Save(path);
                return path;
            }
            catch { return ""; }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Hủy đơn hàng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DialogResult = DialogResult.Abort;
                Close();
            }
        }
    }
}