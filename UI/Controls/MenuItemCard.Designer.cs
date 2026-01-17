namespace UI.Controls
{
    partial class MenuItemCard
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.PictureBox picImage;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.FlowLayoutPanel pnlSizes;
        private System.Windows.Forms.Button btnAdd;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            picImage = new PictureBox();
            lblName = new Label();
            lblStatus = new Label();
            pnlSizes = new FlowLayoutPanel();
            btnAdd = new Button();
            ((System.ComponentModel.ISupportInitialize)picImage).BeginInit();
            SuspendLayout();
            // 
            // picImage
            // 
            picImage.Location = new Point(23, 21);
            picImage.Name = "picImage";
            picImage.Size = new Size(120, 90);
            picImage.SizeMode = PictureBoxSizeMode.Zoom;
            picImage.TabIndex = 0;
            picImage.TabStop = false;
            // 
            // lblName
            // 
            lblName.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblName.Location = new Point(10, 105);
            lblName.Name = "lblName";
            lblName.Size = new Size(160, 20);
            lblName.TabIndex = 1;
            lblName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblStatus
            // 
            lblStatus.Location = new Point(10, 125);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(160, 18);
            lblStatus.TabIndex = 2;
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlSizes
            // 
            pnlSizes.Location = new Point(10, 145);
            pnlSizes.Name = "pnlSizes";
            pnlSizes.Size = new Size(160, 45);
            pnlSizes.TabIndex = 3;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(60, 195);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(50, 30);
            btnAdd.TabIndex = 4;
            btnAdd.Text = "+";
            btnAdd.Click += btnAdd_Click;
            // 
            // MenuItemCard
            // 
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(picImage);
            Controls.Add(lblName);
            Controls.Add(lblStatus);
            Controls.Add(pnlSizes);
            Controls.Add(btnAdd);
            Name = "MenuItemCard";
            Size = new Size(180, 235);
            ((System.ComponentModel.ISupportInitialize)picImage).EndInit();
            ResumeLayout(false);
        }
    }
}
