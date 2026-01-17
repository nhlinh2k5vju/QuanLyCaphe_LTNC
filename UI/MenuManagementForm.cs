using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BLL.Interface;
using DAL.Entities;

using MenuEntity = DAL.Entities.MenuItem;

namespace UI
{
    public partial class MenuManagementForm : Form
    {
        private readonly IMenuService _menuService;

        private MenuEntity? _selectedItem;
        private string? _imagePath;

        // UI
        private FlowLayoutPanel flowMenu;
        private Panel panelEditor;

        private TextBox txtName;
        private CheckBox chkStatus;
        private PictureBox picImage;
        private Button btnChooseImage;

        private DataGridView dgvSizes;
        private Button btnAddSize;

        private Button btnAdd, btnEdit, btnHide, btnSave, btnCancel;

        public MenuManagementForm(IMenuService menuService)
        {
            _menuService = menuService;

            InitializeUI();
            _ = LoadMenuAsync();
        }

        // ================= UI =================
        private void InitializeUI()
        {
            this.Size = new Size(1000, 600);
            this.Text = "Quản lý thực đơn";
            this.Dock = DockStyle.Fill; 

            var top = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40
            };

            btnAdd = new Button { Text = "Thêm" };
            btnEdit = new Button { Text = "Sửa" };
            btnHide = new Button { Text = "Ẩn / Hiện" };

            top.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnHide });
            Controls.Add(top);

            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
            };

            Controls.Add(split);
            split.BringToFront(); 

            split.FixedPanel = FixedPanel.Panel2;
            split.Panel2MinSize = 360;

            split.SplitterDistance = split.Width - 360;

            flowMenu = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White 
            };
            split.Panel1.Controls.Add(flowMenu);

            panelEditor = new Panel
            {
                Dock = DockStyle.Fill,
                Enabled = false,
                Padding = new Padding(10),
                BackColor = SystemColors.Control 
            };
            split.Panel2.Controls.Add(panelEditor);

            BuildEditor();

            // EVENTS
            btnAdd.Click += (_, __) => EnterCreateMode();
            btnEdit.Click += (_, __) => EnterEditMode();
            btnHide.Click += async (_, __) => await ToggleStatusAsync();
            btnSave.Click += async (_, __) => await SaveAsync();
            btnCancel.Click += (_, __) => ExitEditor();
        }

        private void BuildEditor()
        {
            panelEditor.Controls.Clear();

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Name
            layout.Controls.Add(new Label { Text = "Tên món" }, 0, 0);
            txtName = new TextBox();
            layout.Controls.Add(txtName, 1, 0);

            // Image
            layout.Controls.Add(new Label { Text = "Hình ảnh" }, 0, 1);
            var imgPanel = new FlowLayoutPanel { AutoSize = true };
            picImage = new PictureBox
            {
                Width = 80,
                Height = 80,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            btnChooseImage = new Button { Text = "Chọn ảnh" };
            btnChooseImage.Click += ChooseImage;
            imgPanel.Controls.AddRange(new Control[] { picImage, btnChooseImage });
            layout.Controls.Add(imgPanel, 1, 1);

            // Sizes
            layout.Controls.Add(new Label { Text = "Size / Giá" }, 0, 2);
            dgvSizes = new DataGridView
            {
                Height = 150,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false
            };

            dgvSizes.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Size",
                Name = "Size"
            });
            dgvSizes.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Giá",
                Name = "Price"
            });
            dgvSizes.Columns.Add(new DataGridViewButtonColumn
            {
                Text = "X",
                UseColumnTextForButtonValue = true
            });

            dgvSizes.CellClick += (s, e) =>
            {
                if (e.ColumnIndex == 2 && e.RowIndex >= 0)
                    dgvSizes.Rows.RemoveAt(e.RowIndex);
            };

            var sizePanel = new FlowLayoutPanel { AutoSize = true };
            btnAddSize = new Button { Text = "Thêm size" };
            btnAddSize.Click += (_, __) =>
                dgvSizes.Rows.Add("", 0);

            sizePanel.Controls.AddRange(new Control[] { dgvSizes, btnAddSize });
            layout.Controls.Add(sizePanel, 1, 2);

            // Status
            chkStatus = new CheckBox { Text = "Còn bán", Checked = true };
            layout.Controls.Add(chkStatus, 1, 3);

            // Buttons
            var btnPanel = new FlowLayoutPanel { AutoSize = true };
            btnSave = new Button { Text = "OK" };
            btnCancel = new Button { Text = "Hủy" };
            btnPanel.Controls.AddRange(new Control[] { btnSave, btnCancel });
            layout.Controls.Add(btnPanel, 1, 4);

            panelEditor.Controls.Add(layout);
        }

        // ================= LOGIC =================
        private async System.Threading.Tasks.Task LoadMenuAsync()
        {
            flowMenu.Controls.Clear();

            var items = await _menuService.GetAllAsync();

            foreach (var item in items)
            {
                var btn = new Button
                {
                    Text = item.Name,
                    Width = 200,
                    Height = 60, // Tăng chiều cao một chút để hiển thị rõ
                    Tag = item // Lưu object vào Tag để dùng sau này nếu cần
                };

                // ===== LOGIC HIỂN THỊ MỜ / RÕ =====
                if (item.Status)
                {
                    // Trạng thái HIỆN: Màu sáng, chữ đậm
                    btn.BackColor = Color.AliceBlue;
                    btn.ForeColor = Color.Black;
                    btn.Font = new Font(this.Font, FontStyle.Regular);
                }
                else
                {
                    // Trạng thái ẨN: Màu xám, chữ nhạt, thêm chữ (Ẩn)
                    btn.BackColor = Color.LightGray;
                    btn.ForeColor = Color.DimGray;
                    btn.Font = new Font(this.Font, FontStyle.Italic); // Chữ nghiêng
                    btn.Text += "\n(Đang ẩn)"; // Thêm dòng chú thích
                }

                // Sự kiện Click giữ nguyên
                btn.Click += (_, __) =>
                {
                    _selectedItem = item;
                    EnterEditMode();
                };

                flowMenu.Controls.Add(btn);
            }
        }

        private void EnterCreateMode()
        {
            _selectedItem = null;
            panelEditor.Enabled = true;
            txtName.Clear();
            dgvSizes.Rows.Clear();
            chkStatus.Checked = true;
            picImage.Image = null;
            _imagePath = null;
        }

        private void EnterEditMode()
        {
            if (_selectedItem == null) return;

            panelEditor.Enabled = true;
            txtName.Text = _selectedItem.Name;
            chkStatus.Checked = _selectedItem.Status;
            _imagePath = _selectedItem.ImagePath;

            dgvSizes.Rows.Clear();
            foreach (var s in _selectedItem.MenuItemSizes)
                dgvSizes.Rows.Add(s.Size, s.Price);

            if (!string.IsNullOrEmpty(_imagePath) && System.IO.File.Exists(_imagePath))
                picImage.Image = Image.FromFile(_imagePath);
        }

        private async System.Threading.Tasks.Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Tên món không hợp lệ");
                return;
            }

            var sizes = dgvSizes.Rows.Cast<DataGridViewRow>()
                .Select(r => new MenuItemSize
                {
                    Size = r.Cells[0].Value?.ToString() ?? "",
                    Price = Convert.ToDecimal(r.Cells[1].Value)
                }).ToList();

            if (_selectedItem == null)
            {
                var menu = new MenuEntity
                {
                    Name = txtName.Text,
                    Status = chkStatus.Checked,
                    ImagePath = _imagePath
                };

                await _menuService.CreateMenuAsync(menu, sizes);
            }
            else
            {
                _selectedItem.Name = txtName.Text;
                _selectedItem.Status = chkStatus.Checked;
                _selectedItem.ImagePath = _imagePath;

                await _menuService.UpdateMenuAsync(_selectedItem, sizes);
            }

            ExitEditor();
            await LoadMenuAsync();
        }

        private async System.Threading.Tasks.Task ToggleStatusAsync()
        {
            if (_selectedItem == null) return;
            await _menuService.SetMenuStatusAsync(
                _selectedItem.MenuItemId,
                !_selectedItem.Status);
            await LoadMenuAsync();
        }

        private void ExitEditor()
        {
            panelEditor.Enabled = false;
        }

        private void ChooseImage(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Filter = "Image|*.jpg;*.png"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _imagePath = dlg.FileName;
                picImage.Image = Image.FromFile(_imagePath);
            }
        }
    }
}
