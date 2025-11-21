using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace rentalsystem.Controls
{
    public partial class PropertiesControl : UserControl
    {
        public PropertiesControl()
        {
            InitializeComponent();
            LoadProperties();
        }

        public void LoadProperties()
        {
            var dt = DataAccess.GetPropertiesWithStatus();
            // Optionally limit to 10 units if you want fixed total units; for now show all
            dgvProperties.DataSource = dt;
            // Adjust column headers for readability if present
            if (dgvProperties.Columns.Contains("property_id")) dgvProperties.Columns["property_id"].HeaderText = "ID";
            if (dgvProperties.Columns.Contains("property_name")) dgvProperties.Columns["property_name"].HeaderText = "Unit";
            if (dgvProperties.Columns.Contains("availability")) dgvProperties.Columns["availability"].HeaderText = "Availability";
            if (dgvProperties.Columns.Contains("rent_amount")) dgvProperties.Columns["rent_amount"].HeaderText = "Rent";
            if (dgvProperties.Columns.Contains("lease_id")) dgvProperties.Columns["lease_id"].Visible = false; // hide internal id by default
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadProperties();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var f = new AddPropertyForm())
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    LoadProperties();
                }
            }
        }

        private void btnAssign_Click(object sender, EventArgs e)
        {
            if (dgvProperties.CurrentRow == null)
            {
                MessageBox.Show("Select a property first.");
                return;
            }

            var propIdObj = dgvProperties.CurrentRow.Cells["property_id"].Value;
            if (propIdObj == null) { MessageBox.Show("Selected property id not found."); return; }
            var propId = Convert.ToInt32(propIdObj);
            var propName = dgvProperties.CurrentRow.Cells["property_name"].Value?.ToString() ?? "";

            // If already occupied, prevent assigning
            var availability = dgvProperties.CurrentRow.Cells["availability"].Value?.ToString();
            if (!string.IsNullOrEmpty(availability) && availability.Equals("Occupied", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Property is already occupied. Please release lease before assigning.");
                return;
            }

            using (var f = new rentalsystem.AssignTenantForm(propId, propName))
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    LoadProperties();
                    MessageBox.Show("Tenant assigned to property.");
                }
            }
        }

        private void btnRelease_Click(object sender, EventArgs e)
        {
            if (dgvProperties.CurrentRow == null) { MessageBox.Show("Select a property."); return; }
            var leaseObj = dgvProperties.CurrentRow.Cells["lease_id"].Value;
            if (leaseObj == null || leaseObj == DBNull.Value) { MessageBox.Show("No active lease on selected property."); return; }
            var leaseId = Convert.ToInt32(leaseObj);
            var ok = DataAccess.ReleaseLease(leaseId);
            MessageBox.Show(ok ? "Lease released." : "Failed to release lease.");
            if (ok) LoadProperties();
        }

        private void btnAdjustRent_Click(object sender, EventArgs e)
        {
            if (dgvProperties.CurrentRow == null) { MessageBox.Show("Select a property."); return; }
            var leaseObj = dgvProperties.CurrentRow.Cells["lease_id"].Value;
            if (leaseObj == null || leaseObj == DBNull.Value) { MessageBox.Show("No active lease on selected property."); return; }
            var leaseId = Convert.ToInt32(leaseObj);
            var currentRentObj = dgvProperties.CurrentRow.Cells["rent_amount"].Value;
            decimal current = 0;
            if (currentRentObj != null && currentRentObj != DBNull.Value) decimal.TryParse(currentRentObj.ToString(), out current);

            var input = Prompt.ShowDialog("Enter new rent amount:", "Adjust Rent", current.ToString());
            if (input == null) return;
            if (!decimal.TryParse(input, out var newRent)) { MessageBox.Show("Invalid amount."); return; }

            var ok = DataAccess.UpdateLeaseRent(leaseId, newRent);
            MessageBox.Show(ok ? "Rent updated." : "Failed to update rent.");
            if (ok) LoadProperties();
        }
    }
}