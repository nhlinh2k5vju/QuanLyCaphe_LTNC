using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL.Interface;
using DAL.Entities;

namespace UI
{
    public partial class PromotionManagementForm : Form
    {
        private readonly IPromotionService _promotionService;
        private Promotion? _selectedPromotion;

        // ===== Controls =====
        private ComboBox cboDiscountType;
        private TextBox txtDiscountValue;
        private DateTimePicker dtStart;
        private DateTimePicker dtEnd;
        private CheckBox chkStatus;

        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDisable;

        private DataGridView dgvPromotions;

        public PromotionManagementForm(IPromotionService promotionService)
        {
            _promotionService = promotionService;

            InitializeUI();
            _ = LoadPromotionsAsync();
        }

        // ================= UI =================
        private void InitializeUI()
        {
            Dock = DockStyle.Fill;
            Text = "Quản lý mã giảm giá";

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
                ColumnCount = 6,
                AutoSize = true,
                Padding = new Padding(10)
            };

            // Discount type
            input.Controls.Add(new Label { Text = "Loại giảm" }, 0, 0);
            cboDiscountType = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboDiscountType.Items.AddRange(new[] { "PERCENT", "FIXED" });
            cboDiscountType.SelectedIndex = 0;
            input.Controls.Add(cboDiscountType, 1, 0);

            // Discount value
            input.Controls.Add(new Label { Text = "Giá trị" }, 2, 0);
            txtDiscountValue = new TextBox();
            input.Controls.Add(txtDiscountValue, 3, 0);

            // Status
            chkStatus = new CheckBox
            {
                Text = "Hoạt động",
                Checked = true
            };
            input.Controls.Add(chkStatus, 5, 0);

            // Start date
            input.Controls.Add(new Label { Text = "Bắt đầu" }, 0, 1);
            dtStart = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short
            };
            input.Controls.Add(dtStart, 1, 1);

            // End date
            input.Controls.Add(new Label { Text = "Kết thúc" }, 2, 1);
            dtEnd = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short
            };
            input.Controls.Add(dtEnd, 3, 1);

            mainLayout.Controls.Add(input);

            // ===== BUTTON PANEL =====
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Padding = new Padding(10)
            };

            btnAdd = new Button { Text = "Thêm" };
            btnUpdate = new Button { Text = "Sửa" };
            btnDisable = new Button { Text = "Vô hiệu" };

            btnPanel.Controls.AddRange(new Control[]
            {
                btnAdd, btnUpdate, btnDisable
            });

            mainLayout.Controls.Add(btnPanel);

            // ===== GRID =====
            dgvPromotions = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoGenerateColumns = false
            };

            dgvPromotions.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Loại",
                DataPropertyName = "DiscountType"
            });
            dgvPromotions.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Giá trị",
                DataPropertyName = "DiscountValue"
            });
            dgvPromotions.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Bắt đầu",
                DataPropertyName = "StartDate"
            });
            dgvPromotions.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Kết thúc",
                DataPropertyName = "EndDate"
            });
            dgvPromotions.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText = "Active",
                DataPropertyName = "Status"
            });

            dgvPromotions.CellClick += DgvPromotions_CellClick;

            mainLayout.Controls.Add(dgvPromotions);

            // ===== EVENTS =====
            btnAdd.Click += async (_, __) => await AddPromotionAsync();
            btnUpdate.Click += async (_, __) => await UpdatePromotionAsync();
            btnDisable.Click += async (_, __) => await DisablePromotionAsync();
        }

        // ================= DATA =================
        private async Task LoadPromotionsAsync()
        {
            dgvPromotions.DataSource =
                (await _promotionService.GetActivePromotionsAsync()).ToList();

            _selectedPromotion = null;
            ClearInputs();
        }

        private void DgvPromotions_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            _selectedPromotion =
                dgvPromotions.Rows[e.RowIndex].DataBoundItem as Promotion;

            if (_selectedPromotion == null) return;

            cboDiscountType.SelectedItem = _selectedPromotion.DiscountType;
            txtDiscountValue.Text = _selectedPromotion.DiscountValue.ToString();
            dtStart.Value = _selectedPromotion.StartDate;
            dtEnd.Value = _selectedPromotion.EndDate;
            chkStatus.Checked = _selectedPromotion.Status;
        }

        // ================= ACTIONS =================
        private async Task AddPromotionAsync()
        {
            try
            {
                var promo = new Promotion
                {
                    DiscountType = cboDiscountType.SelectedItem?.ToString(),
                    DiscountValue = decimal.Parse(txtDiscountValue.Text),
                    StartDate = dtStart.Value,
                    EndDate = dtEnd.Value,
                    Status = chkStatus.Checked
                };

                await _promotionService.AddAsync(promo);
                await LoadPromotionsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task UpdatePromotionAsync()
        {
            if (_selectedPromotion == null) return;

            try
            {
                _selectedPromotion.DiscountType =
                    cboDiscountType.SelectedItem?.ToString();
                _selectedPromotion.DiscountValue =
                    decimal.Parse(txtDiscountValue.Text);
                _selectedPromotion.StartDate = dtStart.Value;
                _selectedPromotion.EndDate = dtEnd.Value;
                _selectedPromotion.Status = chkStatus.Checked;

                await _promotionService.UpdateAsync(_selectedPromotion);
                await LoadPromotionsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task DisablePromotionAsync()
        {
            if (_selectedPromotion == null) return;

            _selectedPromotion.Status = false;
            await _promotionService.UpdateAsync(_selectedPromotion);
            await LoadPromotionsAsync();
        }

        // ================= UTILS =================
        private void ClearInputs()
        {
            cboDiscountType.SelectedIndex = 0;
            txtDiscountValue.Clear();
            chkStatus.Checked = true;
            dtStart.Value = DateTime.Now;
            dtEnd.Value = DateTime.Now.AddDays(7);
        }
    }
}
