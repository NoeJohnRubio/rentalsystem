namespace rentalsystem
{
    partial class AddPropertyForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.Label lblDescription;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblName = new System.Windows.Forms.Label();
            this.lblAddress = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(20, 5);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(20, 20);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(300, 20);
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(20, 45);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(48, 13);
            this.lblAddress.Text = "Address:";
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(20, 60);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(400, 20);
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(20, 85);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.Text = "Description:";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(20, 100);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(400, 80);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(20, 190);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // AddPropertyForm
            // 
            this.ClientSize = new System.Drawing.Size(460, 220);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblAddress);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.btnSave);
            this.Name = "AddPropertyForm";
            this.Text = "Add Property";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
