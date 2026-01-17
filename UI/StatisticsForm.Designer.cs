using System.Drawing;
using System.Windows.Forms;

namespace UI
{
    partial class StatisticsForm
    {
        private Panel panelTop;
        private Panel panelContent;

        private DateTimePicker dtFrom;
        private DateTimePicker dtTo;
        private Button btnQuery;
        private Button btnExport;

        private Label lblTitle;
        private Label lblOrderCount;
        private Label lblRevenue;
        private Label lblTop1;
        private Label lblTop2;
        private Label lblTop3;

        private void InitializeComponent()
        {
            panelTop = new Panel();
            dtFrom = new DateTimePicker();
            dtTo = new DateTimePicker();
            btnQuery = new Button();
            btnExport = new Button();
            panelContent = new Panel();
            lblTitle = new Label();
            lblOrderCount = new Label();
            lblRevenue = new Label();
            lblTop1 = new Label();
            lblTop2 = new Label();
            lblTop3 = new Label();
            panelTop.SuspendLayout();
            panelContent.SuspendLayout();
            SuspendLayout();
            // 
            // panelTop
            // 
            panelTop.Controls.Add(dtFrom);
            panelTop.Controls.Add(dtTo);
            panelTop.Controls.Add(btnQuery);
            panelTop.Controls.Add(btnExport);
            panelTop.Location = new Point(0, 0);
            panelTop.Name = "panelTop";
            panelTop.Size = new Size(200, 100);
            panelTop.TabIndex = 1;
            // 
            // dtFrom
            // 
            dtFrom.Location = new Point(0, 0);
            dtFrom.Name = "dtFrom";
            dtFrom.Size = new Size(200, 23);
            dtFrom.TabIndex = 0;
            // 
            // dtTo
            // 
            dtTo.Location = new Point(0, 0);
            dtTo.Name = "dtTo";
            dtTo.Size = new Size(200, 23);
            dtTo.TabIndex = 1;
            // 
            // btnQuery
            // 
            btnQuery.Location = new Point(0, 0);
            btnQuery.Name = "btnQuery";
            btnQuery.Size = new Size(75, 23);
            btnQuery.TabIndex = 2;
            btnQuery.Click += btnQuery_Click;
            // 
            // btnExport
            // 
            btnExport.Location = new Point(0, 0);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(75, 23);
            btnExport.TabIndex = 3;
            btnExport.Click += btnExport_Click;
            // 
            // panelContent
            // 
            panelContent.Controls.Add(lblTitle);
            panelContent.Controls.Add(lblOrderCount);
            panelContent.Controls.Add(lblRevenue);
            panelContent.Controls.Add(lblTop1);
            panelContent.Controls.Add(lblTop2);
            panelContent.Controls.Add(lblTop3);
            panelContent.Location = new Point(0, 0);
            panelContent.Name = "panelContent";
            panelContent.Size = new Size(200, 100);
            panelContent.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.Location = new Point(0, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(100, 23);
            lblTitle.TabIndex = 0;
            // 
            // lblOrderCount
            // 
            lblOrderCount.Location = new Point(0, 0);
            lblOrderCount.Name = "lblOrderCount";
            lblOrderCount.Size = new Size(100, 23);
            lblOrderCount.TabIndex = 1;
            // 
            // lblRevenue
            // 
            lblRevenue.Location = new Point(0, 0);
            lblRevenue.Name = "lblRevenue";
            lblRevenue.Size = new Size(100, 23);
            lblRevenue.TabIndex = 2;
            // 
            // lblTop1
            // 
            lblTop1.Location = new Point(0, 0);
            lblTop1.Name = "lblTop1";
            lblTop1.Size = new Size(100, 23);
            lblTop1.TabIndex = 3;
            // 
            // lblTop2
            // 
            lblTop2.Location = new Point(0, 0);
            lblTop2.Name = "lblTop2";
            lblTop2.Size = new Size(100, 23);
            lblTop2.TabIndex = 4;
            // 
            // lblTop3
            // 
            lblTop3.Location = new Point(0, 0);
            lblTop3.Name = "lblTop3";
            lblTop3.Size = new Size(100, 23);
            lblTop3.TabIndex = 5;
            // 
            // StatisticsForm
            // 
            BackColor = Color.White;
            ClientSize = new Size(1482, 373);
            Controls.Add(panelContent);
            Controls.Add(panelTop);
            Name = "StatisticsForm";
            Text = "Thống kê";
            panelTop.ResumeLayout(false);
            panelContent.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
