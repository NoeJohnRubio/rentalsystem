using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rentalsystem
{
    public partial class TenantForm : Form
    {
        private string _username;
        private int _tenantId;
        public TenantForm(string username)
        {
            _username = username;
            InitializeComponent();
            this.Shown += TenantForm_Shown;
        }

        private async void TenantForm_Shown(object sender, EventArgs e)
        {
            // Run loading on a background thread so the form can appear immediately
            try
            {
                await LoadTenantDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load tenant data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Task LoadTenantDataAsync()
        {
            return Task.Run(() =>
            {
                // Get tenant id and leases on background thread
                var id = DataAccess.GetUserIdByUsername(_username);
                DataTable dt = null;
                if (id != 0)
                {
                    dt = DataAccess.GetLeasesForTenantId(id);
                }

                // Marshal UI update back to UI thread
                if (!this.IsDisposed && !this.Disposing)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        lblWelcome.Text = $"Welcome, {_username}";
                        _tenantId = id;
                        dgvLeases.DataSource = dt;
                    }));
                }
            });
        }

        public void btnPayRent_Click(object sender, EventArgs e)
        {
            if (dgvLeases.CurrentRow == null) { MessageBox.Show("Select a lease first."); return; }
            var leaseIdObj = dgvLeases.CurrentRow.Cells["lease_id"].Value;
            if (leaseIdObj == null) { MessageBox.Show("Selected lease has no id."); return; }
            var leaseId = Convert.ToInt32(leaseIdObj);

            // Try to get the current rent amount from the grid as default
            decimal defaultAmount = 0m;
            try
            {
                var rentObj = dgvLeases.CurrentRow.Cells["rent_amount"].Value;
                if (rentObj != null && rentObj != DBNull.Value) decimal.TryParse(rentObj.ToString(), out defaultAmount);
            }
            catch { }

            var amountStr = Prompt.ShowDialog("Enter amount to pay:", "Pay Rent", defaultAmount > 0 ? defaultAmount.ToString("F2") : "0.00");
            if (amountStr == null) return; // cancelled
            if (!decimal.TryParse(amountStr, out var amount)) { MessageBox.Show("Invalid amount."); return; }
            var paymentId = DataAccess.AddPayment(leaseId, amount, "Manual");
            if (paymentId > 0) MessageBox.Show("Payment recorded. Awaiting confirmation."); else MessageBox.Show("Failed to record payment.");
        }

        public void btnRequestMaintenance_Click(object sender, EventArgs e)
        {
            if (dgvLeases.CurrentRow == null) { MessageBox.Show("Select a lease first."); return; }
            var propertyName = dgvLeases.CurrentRow.Cells["property_name"].Value.ToString();
            var propertyId = GetPropertyIdByName(propertyName);
            if (propertyId == 0) { MessageBox.Show("Property not found."); return; }
            var title = Prompt.ShowDialog("Title:", "Maintenance Request", "Leaky faucet");
            if (title == null) return;
            var desc = Prompt.ShowDialog("Description:", "Maintenance Request", "Describe the issue...");
            if (desc == null) return;
            var reqId = DataAccess.AddMaintenanceRequest(_tenantId, propertyId, title, desc);
            if (reqId > 0) MessageBox.Show("Maintenance request submitted.");
        }

        private int GetPropertyIdByName(string name)
        {
            var dt = DataAccess.GetProperties();
            foreach (DataRow r in dt.Rows)
            {
                if (r["property_name"].ToString() == name) return Convert.ToInt32(r["property_id"]);
            }
            return 0;
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            if (_tenantId == 0) { MessageBox.Show("Tenant not loaded yet."); return; }
            var f = new TenantProfileForm(_tenantId);
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog(this);
        }

        // New stubs for additional buttons
        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Change Password feature not yet implemented.");
        }

        private void btnLeaseDetails_Click(object sender, EventArgs e)
        {
            if (dgvLeases.CurrentRow == null) { MessageBox.Show("Select a lease first."); return; }
            var leaseIdObj = dgvLeases.CurrentRow.Cells["lease_id"].Value;
            if (leaseIdObj == null) { MessageBox.Show("Selected lease has no id."); return; }
            var leaseId = Convert.ToInt32(leaseIdObj);
            var leaseRow = DataAccess.GetLeaseById(leaseId);
            if (leaseRow == null) { MessageBox.Show("Lease details not found."); return; }

            var f = new LeaseDetailsForm(leaseId);
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog(this);
        }

        private void btnPayments_Click(object sender, EventArgs e)
        {
            // This button was removed from designer; keep stub for safety
            MessageBox.Show("Payments are handled by the admin. Please contact support.");
        }

        private void btnPaymentHistory_Click(object sender, EventArgs e)
        {
            if (_tenantId == 0) { MessageBox.Show("Tenant not loaded yet."); return; }
            var dt = DataAccess.GetPaymentsForTenantId(_tenantId);
            var f = new System.Windows.Forms.Form();
            var dgv = new System.Windows.Forms.DataGridView { Dock = DockStyle.Fill, DataSource = dt };
            f.Controls.Add(dgv);
            f.StartPosition = FormStartPosition.CenterParent;
            f.Size = new System.Drawing.Size(700, 400);
            f.Text = "Payment History";
            f.ShowDialog(this);
        }

        private void btnMessages_Click(object sender, EventArgs e)
        {
            if (_tenantId == 0) { MessageBox.Show("Tenant not loaded yet."); return; }
            var f = new TenantMessagesForm(_tenantId);
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog(this);
        }

        private void btnNotifications_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Notifications feature has been removed from the dashboard.");
        }

        private void btnMaintenanceList_Click(object sender, EventArgs e)
        {
            if (_tenantId == 0) { MessageBox.Show("Tenant not loaded yet."); return; }
            var dt = DataAccess.GetMaintenanceRequestsForTenantId(_tenantId);
            var f = new System.Windows.Forms.Form();
            var dgv = new System.Windows.Forms.DataGridView { Dock = DockStyle.Fill, DataSource = dt };
            f.Controls.Add(dgv);
            f.StartPosition = FormStartPosition.CenterParent;
            f.Size = new System.Drawing.Size(800, 500);
            f.Text = "Maintenance Requests";
            f.ShowDialog(this);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Close tenant dashboard and restart login flow
            this.Hide();
            var login = new Form1();
            var dr = login.ShowDialog();
            if (dr == DialogResult.OK && login.IsAuthenticated)
            {
                // Determine new dashboard based on role
                Form dashboard;
                if (string.Equals(login.AuthRole, "ADMIN", StringComparison.OrdinalIgnoreCase))
                    dashboard = new AdminForm(login.AuthUsername);
                else
                    dashboard = new TenantForm(login.AuthUsername);

                dashboard.StartPosition = FormStartPosition.CenterScreen;
                dashboard.Show();
                this.Close();
            }
            else
            {
                // if login cancelled show login again then close current
                try { login.Dispose(); } catch { }
                Application.Exit();
            }
        }
    }
}