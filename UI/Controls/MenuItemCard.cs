using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using DALMenuItem = DAL.Entities.MenuItem;
using DALMenuItemSize = DAL.Entities.MenuItemSize;

namespace UI.Controls
{
    public partial class MenuItemCard : UserControl
    {
        private readonly DALMenuItem _menuItem;
        private DALMenuItemSize? _selectedSize;
        private readonly MenuItemCardMode _mode;

        // ===== EVENTS =====
        // Dùng cho OrderForm
        public event Action<DALMenuItem, DALMenuItemSize>? AddClicked;

        // Dùng cho MenuManagementForm
        public event Action<DALMenuItem>? CardSelected;

        // ===== CONSTRUCTOR =====
        public MenuItemCard(DALMenuItem menuItem, MenuItemCardMode mode)
        {
            InitializeComponent();

            _menuItem = menuItem;
            _mode = mode;

            LoadData();
            ApplyMode();
        }

        // ===== LOAD DATA =====
        private void LoadData()
        {
            lblName.Text = _menuItem.Name;

            // trạng thái
            lblStatus.Text = _menuItem.Status ? "Đang bán" : "Đã ẩn";
            lblStatus.ForeColor = _menuItem.Status ? Color.Green : Color.Red;

            // ảnh
            if (!string.IsNullOrEmpty(_menuItem.ImagePath)
                && System.IO.File.Exists(_menuItem.ImagePath))
            {
                picImage.Image = Image.FromFile(_menuItem.ImagePath);
            }

            // sizes
            if (_menuItem.MenuItemSizes != null && _menuItem.MenuItemSizes.Any())
            {
                _selectedSize = _menuItem.MenuItemSizes.First();
                pnlSizes.Controls.Clear();

                foreach (var size in _menuItem.MenuItemSizes)
                {
                    var btnSize = new Button
                    {
                        Text = $"{size.Size}\n{size.Price:N0}đ",
                        Width = 50,
                        Height = 40,
                        Font = new Font("Segoe UI", 8)
                    };

                    btnSize.Click += (s, e) =>
                    {
                        _selectedSize = size;
                        HighlightSelectedSize(btnSize);
                    };

                    pnlSizes.Controls.Add(btnSize);
                }
            }
        }

        // ===== APPLY MODE =====
        private void ApplyMode()
        {
            if (_mode == MenuItemCardMode.Management)
            {
                // Ẩn phần order
                pnlSizes.Visible = false;
                btnAdd.Visible = false;

                // click cả card để chọn
                this.Cursor = Cursors.Hand;
                this.Click += Card_Click;

                // propagate click cho control con
                foreach (Control c in this.Controls)
                    c.Click += Card_Click;
            }
        }

        private void Card_Click(object? sender, EventArgs e)
        {
            CardSelected?.Invoke(_menuItem);
            HighlightCard();
        }

        // ===== UI EFFECT =====
        private void HighlightSelectedSize(Button selectedButton)
        {
            foreach (Control ctrl in pnlSizes.Controls)
            {
                if (ctrl is Button btn)
                    btn.BackColor = SystemColors.Control;
            }

            selectedButton.BackColor = Color.LightBlue;
        }

        private void HighlightCard()
        {
            this.BackColor = Color.AliceBlue;
        }

        public void UnHighlight()
        {
            this.BackColor = SystemColors.Control;
        }

        // ===== ORDER BUTTON =====
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (_selectedSize == null) return;
            AddClicked?.Invoke(_menuItem, _selectedSize);
        }
    }
}
