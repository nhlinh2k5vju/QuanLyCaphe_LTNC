namespace UI
{
    partial class OrderHistoryForm
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }

        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(1100, 600);
            this.Text = "Lịch sử giao dịch";
            this.ResumeLayout(false);
        }
    }
}
