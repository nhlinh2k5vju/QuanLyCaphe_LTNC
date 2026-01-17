using BLL.Interface;
using BLL.Validators;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UI.Models;
using UI.Services;

using DALMenuItem = DAL.Entities.MenuItem;
using DALPromotion = DAL.Entities.Promotion;
using DALOrderDetail = DAL.Entities.OrderDetail;
using DALUser = DAL.Entities.User;

namespace UI
{
    public partial class OrderForm : Form
    {
        private readonly DALUser _currentUser;
        private readonly IOrderService _orderService;
        private readonly IPromotionService _promotionService;
        private readonly IMenuService _menuService;

        private readonly List<OrderItemTemp> _orderItems = new();
        private decimal _discount = 0;
        private DALPromotion? _selectedPromotion;

        public OrderForm(
            DALUser currentUser,
            IOrderService orderService,
            IPromotionService promotionService,
            IMenuService menuService)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _orderService = orderService;
            _promotionService = promotionService;
            _menuService = menuService;

            BindEvents();
            //LoadMenuFromDb();
            //LoadPromotions();
            //RefreshBill();
            this.Load += OrderForm_Load;
        }

        private async void OrderForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Dùng 'await' để bắt chúng xếp hàng, chạy xong cái này mới chạy cái kia
                await LoadMenuFromDb();
                await LoadPromotions();

                RefreshBill();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo: " + ex.Message);
            }
        }
        #region INIT
        private void BindEvents()
        {
            btnClear.Click += BtnClear_Click;
            btnCheckout.Click += BtnCheckout_Click;
            dgvBill.CellClick += DgvBill_CellClick;
            cboPromotion.SelectedIndexChanged += CboPromotion_SelectedIndexChanged;
        }
        #endregion

        #region LOAD MENU + PROMOTION
        private async Task LoadMenuFromDb()
        {
            try
            {
                tblMenu.Controls.Clear();
                tblMenu.RowStyles.Clear();
                tblMenu.RowCount = 0;

                var menuItems = await _menuService.GetAllActiveAsync();
                int index = 0;

                foreach (var menuItem in menuItems)
                {
                    var card = CreateMenuCard(menuItem);
                    int row = index / 3;

                    if (tblMenu.RowCount <= row)
                    {
                        tblMenu.RowCount++;
                        tblMenu.RowStyles.Add(new RowStyle(SizeType.Absolute, 180));
                    }

                    tblMenu.Controls.Add(card, index % 3, row);
                    index++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải menu: " + ex.Message);
            }
        }

        private async Task LoadPromotions()
        {
            try
            {
                var promotions = await _promotionService.GetActivePromotionsAsync();

                cboPromotion.SelectedIndexChanged -= CboPromotion_SelectedIndexChanged;
                cboPromotion.DataSource = promotions;
                cboPromotion.DisplayMember = "Code";
                cboPromotion.SelectedIndex = -1;

                cboPromotion.SelectedIndexChanged += CboPromotion_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải khuyến mãi: " + ex.Message);
            }
        }
        #endregion

        #region MENU CARD
        //private Control CreateMenuCard(DALMenuItem menuItem)
        //{
        //    var panel = new Panel
        //    {
        //        Width = 200,
        //        Height = 170,
        //        BorderStyle = BorderStyle.FixedSingle,
        //        Margin = new Padding(10)
        //    };

        //    var lblName = new Label
        //    {
        //        Text = menuItem.Name,
        //        Dock = DockStyle.Top,
        //        Height = 30,
        //        TextAlign = ContentAlignment.MiddleCenter,
        //        Font = new Font("Segoe UI", 9, FontStyle.Bold)
        //    };

        //    string selectedSize = menuItem.MenuItemSizes.First().Size;

        //    FlowLayoutPanel flow = new FlowLayoutPanel
        //    {
        //        Dock = DockStyle.Bottom,
        //        Height = 40
        //    };

        //    foreach (var size in menuItem.MenuItemSizes)
        //    {
        //        var btnSize = new Button { Text = size.Size, Width = 40 };
        //        btnSize.Click += (s, e) => selectedSize = size.Size;
        //        flow.Controls.Add(btnSize);
        //    }

        //    var btnAdd = new Button
        //    {
        //        Text = "+",
        //        Width = 40,
        //        BackColor = Color.LightGreen
        //    };

        //    btnAdd.Click += (s, e) =>
        //    {
        //        var size = menuItem.MenuItemSizes.First(x => x.Size == selectedSize);
        //        AddItem(menuItem.MenuItemId, menuItem.Name, size.Size, size.Price);
        //    };

        //    flow.Controls.Add(btnAdd);
        //    panel.Controls.Add(flow);
        //    panel.Controls.Add(lblName);
        //    return panel;
        //}

        private Control CreateMenuCard(DALMenuItem menuItem)
        {
            // 1. Panel bao ngoài
            var panel = new Panel
            {
                Width = 240,
                Height = 380, // Tăng chiều cao để chứa ảnh
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(10),
                BackColor = Color.White
            };

            // 2. PictureBox (Hiển thị ảnh)
            var picBox = new PictureBox
            {
                Width = 200,
                Height = 100, // Chiều cao ảnh
                Location = new Point(10, 10),
                SizeMode = PictureBoxSizeMode.Zoom, // Co giãn ảnh vừa khung
                BorderStyle = BorderStyle.FixedSingle
            };

            // Load ảnh từ file
            if (!string.IsNullOrEmpty(menuItem.ImagePath) && File.Exists(menuItem.ImagePath))
            {
                try
                {
                    picBox.Image = Image.FromFile(menuItem.ImagePath);
                }
                catch { /* Lỗi ảnh thì kệ, để trống */ }
            }

            // 3. Label Tên món
            var lblName = new Label
            {
                Text = menuItem.Name,
                Location = new Point(0, 135), // Nằm dưới ảnh
                Width = 200,
                Height = 20,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkSlateGray
            };

            // 4. Panel chọn Size (FlowLayoutPanel)
            string selectedSize = menuItem.MenuItemSizes.FirstOrDefault()?.Size ?? "";
            decimal currentPrice = menuItem.MenuItemSizes.FirstOrDefault()?.Price ?? 0;

            FlowLayoutPanel flowSizes = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(5)
            };

            // Nút thêm (+)
            var btnAdd = new Button
            {
                Text = "+",
                Width = 25,
                Height = 25,
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.Click += (s, e) =>
            {
                // Tìm lại giá của size đang chọn
                var sizeObj = menuItem.MenuItemSizes.FirstOrDefault(x => x.Size == selectedSize);
                if (sizeObj != null)
                {
                    AddItem(menuItem.MenuItemId, menuItem.Name, sizeObj.Size, sizeObj.Price);
                }
            };

            // Các nút Size (S, M, L)
            foreach (var size in menuItem.MenuItemSizes)
            {
                var btnSize = new Button
                {
                    Text = $"{size.Size}\n{size.Price:N0}", // Hiện Size + Giá
                    Width = 30,
                    Height = 30,
                    Font = new Font("Segoe UI", 7),
                    BackColor = (size.Size == selectedSize) ? Color.LightBlue : Color.WhiteSmoke
                };

                btnSize.Click += (s, e) =>
                {
                    selectedSize = size.Size;
                    // Đổi màu nút để biết đang chọn size nào
                    foreach (Control c in flowSizes.Controls)
                        if (c is Button b && b != btnAdd) b.BackColor = Color.WhiteSmoke;
                    btnSize.BackColor = Color.LightBlue;
                };

                flowSizes.Controls.Add(btnSize);
            }

            // Thêm nút + vào cuối
            flowSizes.Controls.Add(btnAdd);

            // Add vào Panel cha
            panel.Controls.Add(picBox);
            panel.Controls.Add(lblName);
            panel.Controls.Add(flowSizes);

            return panel;
        }
        #endregion

        #region ORDER LOGIC
        private void AddItem(int id, string name, string size, decimal price)
        {
            var orderItem = _orderItems.FirstOrDefault(
                x => x.MenuItemId == id && x.Size == size);

            if (orderItem == null)
            {
                _orderItems.Add(new OrderItemTemp
                {
                    MenuItemId = id,
                    MenuItemName = name,
                    Size = size,
                    UnitPrice = price,
                    Quantity = 1
                });
            }
            else
            {
                orderItem.Quantity++;
            }

            RefreshBill();
        }

        private void RefreshBill()
        {
            dgvBill.Rows.Clear();
            decimal subTotal = 0;

            foreach (var orderItem in _orderItems)
            {
                dgvBill.Rows.Add(
                    orderItem.MenuItemName,
                    orderItem.Size,
                    orderItem.Quantity,
                    orderItem.Total.ToString("N0") + " đ");

                subTotal += orderItem.Total;
            }

            lblSubTotal.Text = $"TỔNG TIỀN: {subTotal:N0} đ";
            lblDiscount.Text = $"GIẢM GIÁ: -{_discount:N0} đ";
            lblTotal.Text = $"PHẢI TRẢ: {(subTotal - _discount):N0} đ";
        }
        #endregion

        #region EVENT HANDLERS

        private void BtnClear_Click(object sender, EventArgs e)
        {
            _orderItems.Clear();
            _discount = 0;
            _selectedPromotion = null;
            cboPromotion.SelectedIndex = -1;
            RefreshBill();
        }

        private async void BtnCheckout_Click(object sender, EventArgs e)
        {
            if (!_orderItems.Any())
            {
                MessageBox.Show("Chưa có món nào!");
                return;
            }

            // 1️⃣ TÍNH TOÁN
            decimal subTotal = _orderItems.Sum(x => x.Total);
            decimal discount = _discount;
            decimal total = subTotal - discount;

            // 2️⃣ MAP SANG PAYMENT SUMMARY (CỰC KỲ QUAN TRỌNG)
            var summary = new PaymentSummary
            {
                StaffName = _currentUser.FullName,
                CreatedTime = DateTime.Now,

                Items = _orderItems.Select(x => new PaymentItem
                {
                    MenuItemName = x.MenuItemName,
                    Size = x.Size,
                    Quantity = x.Quantity,
                    Total = x.Total
                }).ToList(),

                SubTotal = subTotal,
                Discount = discount,
                Total = total
            };

            // 3️⃣ MỞ PAYMENT FORM
            using var paymentForm = new PaymentForm(summary);
            var result = paymentForm.ShowDialog();

            // ===== XỬ LÝ KẾT QUẢ =====

            // CASE A: Quay lại sửa
            if (result == DialogResult.Cancel)
                return;

            // CASE B: Hủy hóa đơn
            if (result == DialogResult.Abort)
            {
                BtnClear_Click(null, EventArgs.Empty);
                return;
            }

            // CASE C: Thanh toán
            if (result == DialogResult.OK)
            {
                try
                {
                    string qrPath = paymentForm.SaveQrTempImage();

                    var orderDetails = _orderItems.Select(x => new DALOrderDetail
                    {
                        MenuItemId = x.MenuItemId,
                        Size = x.Size,
                        Quantity = x.Quantity,
                        UnitPrice = x.UnitPrice
                    }).ToList();

                    await _orderService.CreatePaidOrderAsync(
                        _currentUser.UserId,
                        orderDetails,
                        _selectedPromotion
                    );

                    string pdfPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        $"Bill_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

                    InvoicePdfService.Export(summary, pdfPath, qrPath);

                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = pdfPath,
                            UseShellExecute = true
                        });
                    }
                    catch { }

                    MessageBox.Show("Thanh toán thành công!");
                    BtnClear_Click(null, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi thanh toán: " + ex.Message);
                }
            }
        }



        private void DgvBill_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvBill.Columns.Count - 1)
            {
                _orderItems.RemoveAt(e.RowIndex);
                RefreshBill();
            }
        }

        private void CboPromotion_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 1. Nếu chưa chọn món thì không tính
            if (!_orderItems.Any())
            {
                _discount = 0;
                RefreshBill();
                return;
            }

            // 2. Lấy object Promotion từ ComboBox
            // Lưu ý: DataSource là List<Promotion> nên ép kiểu được
            if (cboPromotion.SelectedItem is not DALPromotion promo)
            {
                // Trường hợp người dùng xóa chọn hoặc null
                _discount = 0;
                _selectedPromotion = null;
                RefreshBill();
                return;
            }

            // 3. Tính toán
            decimal subTotal = _orderItems.Sum(x => x.Total);
            decimal discountAmount = 0;

            // Logic tính toán khớp với Service
            if (promo.DiscountType == "PERCENT")
            {
                // Giảm theo % (VD: 10 là 10%)
                discountAmount = subTotal * (promo.DiscountValue / 100m);
            }
            else
            {
                // Giảm tiền mặt (VD: 20000 là 20k)
                discountAmount = promo.DiscountValue;
            }

            // Không được giảm quá tổng tiền (tránh âm tiền)
            if (discountAmount > subTotal)
                discountAmount = subTotal;

            // 4. Lưu vào biến toàn cục để dùng lúc Thanh toán
            _discount = discountAmount;
            _selectedPromotion = promo;

            // 5. Cập nhật giao diện
            RefreshBill();
        }

        //private async void CboPromotion_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_orderItems.Any())
        //    {
        //        cboPromotion.SelectedIndex = -1;
        //        return;
        //    }

        //    if (cboPromotion.SelectedItem is not DALPromotion promo)
        //        return;

        //    decimal subTotal = _orderItems.Sum(x => x.Total);

        //    var validPromo =
        //        await _promotionService.ValidatePromotionAsync(promo.Code, subTotal);

        //    if (validPromo == null)
        //    {
        //        MessageBox.Show("Mã giảm giá không hợp lệ!");
        //        _discount = 0;
        //        _selectedPromotion = null;
        //        cboPromotion.SelectedIndex = -1;
        //    }
        //    else
        //    {
        //        _selectedPromotion = validPromo;
        //        _discount = OrderValidator.CalculateDiscount(validPromo, subTotal);
        //    }

        //    RefreshBill();
        //}

        #endregion
    }
}
