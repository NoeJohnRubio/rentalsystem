namespace rentalsystem
{
    partial class TenantForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.DataGridView dgvLeases;
        private System.Windows.Forms.Button btnPayRent;
        private System.Windows.Forms.Button btnRequestMaintenance;
        private System.Windows.Forms.Button btnProfile;
        private System.Windows.Forms.Button btnChangePassword;
        private System.Windows.Forms.Button btnLeaseDetails;
        private System.Windows.Forms.Button btnPaymentHistory;
        private System.Windows.Forms.Button btnMessages;
        private System.Windows.Forms.Button btnNotifications;
        private System.Windows.Forms.Button btnMaintenanceList;
        private System.Windows.Forms.Button btnLogout;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.dgvLeases = new System.Windows.Forms.DataGridView();
            this.btnPayRent = new System.Windows.Forms.Button();
            this.btnRequestMaintenance = new System.Windows.Forms.Button();
            this.btnProfile = new System.Windows.Forms.Button();
            this.btnChangePassword = new System.Windows.Forms.Button();
            this.btnLeaseDetails = new System.Windows.Forms.Button();
            this.btnPaymentHistory = new System.Windows.Forms.Button();
            this.btnMessages = new System.Windows.Forms.Button();
            this.btnNotifications = new System.Windows.Forms.Button();
            this.btnMaintenanceList = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLeases)).BeginInit();
            this.SuspendLayout();
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Location = new System.Drawing.Point(20, 20);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(46, 17);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "label1";
            // 
            // dgvLeases
            // 
            this.dgvLeases.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLeases.Location = new System.Drawing.Point(23, 50);
            this.dgvLeases.Name = "dgvLeases";
            this.dgvLeases.Size = new System.Drawing.Size(740, 300);
            this.dgvLeases.TabIndex = 1;
            // 
            // btnPayRent
            // 
            this.btnPayRent.Location = new System.Drawing.Point(23, 370);
            this.btnPayRent.Name = "btnPayRent";
            this.btnPayRent.Size = new System.Drawing.Size(100, 30);
            this.btnPayRent.TabIndex = 2;
            this.btnPayRent.Text = "Pay Rent";
            this.btnPayRent.UseVisualStyleBackColor = true;
            this.btnPayRent.Click += new System.EventHandler(this.btnPayRent_Click);
            // 
            // btnRequestMaintenance
            // 
            this.btnRequestMaintenance.Location = new System.Drawing.Point(140, 370);
            this.btnRequestMaintenance.Name = "btnRequestMaintenance";
            this.btnRequestMaintenance.Size = new System.Drawing.Size(160, 30);
            this.btnRequestMaintenance.TabIndex = 3;
            this.btnRequestMaintenance.Text = "Request Maintenance";
            this.btnRequestMaintenance.UseVisualStyleBackColor = true;
            this.btnRequestMaintenance.Click += new System.EventHandler(this.btnRequestMaintenance_Click);
            // 
            // btnProfile
            // 
            this.btnProfile.Location = new System.Drawing.Point(320, 370);
            this.btnProfile.Name = "btnProfile";
            this.btnProfile.Size = new System.Drawing.Size(100, 30);
            this.btnProfile.TabIndex = 4;
            this.btnProfile.Text = "Profile";
            this.btnProfile.UseVisualStyleBackColor = true;
            this.btnProfile.Click += new System.EventHandler(this.btnProfile_Click);
            // 
            // btnChangePassword
            // 
            this.btnChangePassword.Location = new System.Drawing.Point(440, 370);
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.Size = new System.Drawing.Size(120, 30);
            this.btnChangePassword.TabIndex = 5;
            this.btnChangePassword.Text = "Change Password";
            this.btnChangePassword.UseVisualStyleBackColor = true;
            this.btnChangePassword.Click += new System.EventHandler(this.btnChangePassword_Click);
            // 
            // btnLeaseDetails
            // 
            this.btnLeaseDetails.Location = new System.Drawing.Point(580, 370);
            this.btnLeaseDetails.Name = "btnLeaseDetails";
            this.btnLeaseDetails.Size = new System.Drawing.Size(100, 30);
            this.btnLeaseDetails.TabIndex = 6;
            this.btnLeaseDetails.Text = "Lease Details";
            this.btnLeaseDetails.UseVisualStyleBackColor = true;
            this.btnLeaseDetails.Click += new System.EventHandler(this.btnLeaseDetails_Click);
            // 
            // btnPaymentHistory
            // 
            this.btnPaymentHistory.Location = new System.Drawing.Point(140, 410);
            this.btnPaymentHistory.Name = "btnPaymentHistory";
            this.btnPaymentHistory.Size = new System.Drawing.Size(120, 30);
            this.btnPaymentHistory.TabIndex = 8;
            this.btnPaymentHistory.Text = "Payment History";
            this.btnPaymentHistory.UseVisualStyleBackColor = true;
            this.btnPaymentHistory.Click += new System.EventHandler(this.btnPaymentHistory_Click);
            // 
            // btnMessages
            // 
            this.btnMessages.Location = new System.Drawing.Point(280, 410);
            this.btnMessages.Name = "btnMessages";
            this.btnMessages.Size = new System.Drawing.Size(100, 30);
            this.btnMessages.TabIndex = 9;
            this.btnMessages.Text = "Messages";
            this.btnMessages.UseVisualStyleBackColor = true;
            this.btnMessages.Click += new System.EventHandler(this.btnMessages_Click);
            // 
            // btnNotifications
            // 
            this.btnNotifications.Location = new System.Drawing.Point(400, 410);
            this.btnNotifications.Name = "btnNotifications";
            this.btnNotifications.Size = new System.Drawing.Size(120, 30);
            this.btnNotifications.TabIndex = 10;
            this.btnNotifications.Text = "Notifications";
            this.btnNotifications.UseVisualStyleBackColor = true;
            this.btnNotifications.Click += new System.EventHandler(this.btnNotifications_Click);
            // 
            // btnMaintenanceList
            // 
            this.btnMaintenanceList.Location = new System.Drawing.Point(540, 410);
            this.btnMaintenanceList.Name = "btnMaintenanceList";
            this.btnMaintenanceList.Size = new System.Drawing.Size(140, 30);
            this.btnMaintenanceList.TabIndex = 11;
            this.btnMaintenanceList.Text = "Maintenance Requests";
            this.btnMaintenanceList.UseVisualStyleBackColor = true;
            this.btnMaintenanceList.Click += new System.EventHandler(this.btnMaintenanceList_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(700, 12);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(75, 23);
            this.btnLogout.TabIndex = 12;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // TenantForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 460);
            this.Controls.Add(this.lblWelcome);
            this.Controls.Add(this.dgvLeases);
            this.Controls.Add(this.btnPayRent);
            this.Controls.Add(this.btnRequestMaintenance);
            this.Controls.Add(this.btnProfile);
            this.Controls.Add(this.btnChangePassword);
            this.Controls.Add(this.btnLeaseDetails);
            this.Controls.Add(this.btnPaymentHistory);
            this.Controls.Add(this.btnMessages);
            this.Controls.Add(this.btnNotifications);
            this.Controls.Add(this.btnMaintenanceList);
            this.Controls.Add(this.btnLogout);
            this.Name = "TenantForm";
            this.Text = "Tenant Dashboard";
            ((System.ComponentModel.ISupportInitialize)(this.dgvLeases)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}