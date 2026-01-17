using System;
using System.Collections.Generic;
using System.Drawing; // Cần cho UI
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL.Interface;
using ClosedXML.Excel;
using DAL.Models;

namespace UI
{
    public partial class OrderHistoryForm : Form
    {
        private readonly IOrderService _orderService;

  
        private List<OrderHistoryDto> _currentData = new();

  
        private DateTimePicker dtFrom;
        private DateTimePicker dtTo;
        private Button btnQuery;
        private Button btnExport;
        private DataGridView dgvHistory;

        public OrderHistoryForm(IOrderService orderService)
        {
            _orderService = orderService;

          
            InitializeUI();

          
            dtFrom.Value = DateTime.Now;
            dtTo.Value = DateTime.Now;
        }

      
        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;
            this.Text = "Lịch sử giao dịch";
            this.Font = new Font("Segoe UI", 10);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            this.Controls.Add(mainLayout);

            var filterPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                AutoSize = true,
                BackColor = Color.WhiteSmoke
            };

            filterPanel.Controls.Add(new Label { Text = "Từ ngày:", AutoSize = true, Margin = new Padding(0, 8, 0, 0) });
            dtFrom = new DateTimePicker { Width = 130, Format = DateTimePickerFormat.Short };
            filterPanel.Controls.Add(dtFrom);

            filterPanel.Controls.Add(new Label { Text = "  Đến ngày:", AutoSize = true, Margin = new Padding(0, 8, 0, 0) });
            dtTo = new DateTimePicker { Width = 130, Format = DateTimePickerFormat.Short };
            filterPanel.Controls.Add(dtTo);

            btnQuery = new Button { Text = "Truy vấn", Width = 100, BackColor = Color.LightBlue };
            btnExport = new Button { Text = "Xuất Excel", Width = 120, BackColor = Color.LightGreen };

            filterPanel.Controls.Add(btnQuery);
            filterPanel.Controls.Add(btnExport);

            mainLayout.Controls.Add(filterPanel, 0, 0);

            // ===== 2. GRID VIEW =====
            dgvHistory = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoGenerateColumns = false,
                BackgroundColor = Color.White,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };

            // Cấu hình cột (DataPropertyName phải trùng khớp với DAL.Models.OrderHistoryDto)
            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            { HeaderText = "Mã HĐ", DataPropertyName = "OrderId", Width = 80 });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            { HeaderText = "Thời gian", DataPropertyName = "OrderTime", Width = 140, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            { HeaderText = "Nhân viên", DataPropertyName = "StaffName", Width = 150 });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            { HeaderText = "Tên món", DataPropertyName = "MenuItemName", Width = 180 });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            { HeaderText = "Size", DataPropertyName = "Size", Width = 60 });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            { HeaderText = "SL", DataPropertyName = "Quantity", Width = 60 });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            { HeaderText = "Đơn giá", DataPropertyName = "UnitPrice", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            { HeaderText = "Thành tiền", DataPropertyName = "LineTotal", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            { HeaderText = "Tổng HĐ", DataPropertyName = "OrderTotal", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 10, FontStyle.Bold) } });

            mainLayout.Controls.Add(dgvHistory, 0, 1);

            // ===== EVENTS =====
            btnQuery.Click += async (_, __) => await QueryAsync();
            btnExport.Click += (_, __) => ExportExcel();
        }

        // ================= LOGIC =================
        private async Task QueryAsync()
        {
            if (dtFrom.Value.Date > dtTo.Value.Date)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnQuery.Enabled = false; // Chặn spam nút
                btnQuery.Text = "Đang tải...";

                // Gọi Service (Lưu ý: Service phải có hàm GetOrdersByDateAsync trả về List<OrderHistoryDto>)
                _currentData = await _orderService.GetOrdersByDateAsync(
                    dtFrom.Value.Date,
                    dtTo.Value.Date.AddDays(1).AddTicks(-1) // Lấy hết cuối ngày
                );

                dgvHistory.DataSource = _currentData;

                if (!_currentData.Any())
                {
                    MessageBox.Show("Không tìm thấy giao dịch nào trong khoảng thời gian này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi truy vấn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnQuery.Enabled = true;
                btnQuery.Text = "Truy vấn";
            }
        }

        // ================= EXPORT =================
        private void ExportExcel()
        {
            if (_currentData == null || !_currentData.Any())
            {
                MessageBox.Show("Không có dữ liệu để xuất file.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var sfd = new SaveFileDialog
                {
                    Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                    FileName = $"BaoCao_GiaoDich_{dtFrom.Value:yyyyMMdd}_{dtTo.Value:yyyyMMdd}.xlsx"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using var wb = new XLWorkbook();
                    var ws = wb.Worksheets.Add("Lịch sử giao dịch");

                    // Xuất tiêu đề
                    ws.Cell(1, 1).Value = "BÁO CÁO LỊCH SỬ GIAO DỊCH";
                    ws.Range(1, 1, 1, 9).Merge().Style.Font.Bold = true;
                    ws.Range(1, 1, 1, 9).Merge().Style.Font.FontSize = 14;
                    ws.Range(1, 1, 1, 9).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(2, 1).Value = $"Từ ngày: {dtFrom.Value:dd/MM/yyyy} - Đến ngày: {dtTo.Value:dd/MM/yyyy}";
                    ws.Range(2, 1, 2, 9).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

     
                    var table = ws.Cell(4, 1).InsertTable(_currentData);

                    ws.Columns().AdjustToContents();

                    wb.SaveAs(sfd.FileName);
                    MessageBox.Show("Xuất file Excel thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = sfd.FileName, UseShellExecute = true }); } catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}