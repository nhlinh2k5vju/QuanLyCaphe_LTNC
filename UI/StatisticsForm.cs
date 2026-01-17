using System;
using System.Linq;
using System.Windows.Forms;
using BLL.Interface;
using DAL.Models;

namespace UI
{
    public partial class StatisticsForm : Form
    {
        private readonly IOrderService _orderService;

        public StatisticsForm(IOrderService orderService)
        {
            InitializeComponent();
            _orderService = orderService;

            dtFrom.Value = DateTime.Today;
            dtTo.Value = DateTime.Today;
        }

        private async void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime from = dtFrom.Value.Date;
                DateTime to = dtTo.Value.Date.AddDays(1).AddSeconds(-1);

                var result = await _orderService.GetStatisticsAsync(from, to);

                lblOrderCount.Text = $"Số hóa đơn: {result.OrderCount}";
                lblRevenue.Text = $"Doanh thu: {result.TotalRevenue:N0} đ";

                lblTop1.Text = "1. -";
                lblTop2.Text = "2. -";
                lblTop3.Text = "3. -";

                if (result.TopProducts.Any())
                {
                    if (result.TopProducts.Count > 0)
                        lblTop1.Text = $"1. {result.TopProducts[0].MenuItemName} ({result.TopProducts[0].Quantity})";

                    if (result.TopProducts.Count > 1)
                        lblTop2.Text = $"2. {result.TopProducts[1].MenuItemName} ({result.TopProducts[1].Quantity})";

                    if (result.TopProducts.Count > 2)
                        lblTop3.Text = $"3. {result.TopProducts[2].MenuItemName} ({result.TopProducts[2].Quantity})";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thống kê: " + ex.Message);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Chức năng xuất Excel sẽ dùng ClosedXML.\nBạn có thể bổ sung sau.",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
