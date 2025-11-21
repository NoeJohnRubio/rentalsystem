using System;
using System.Data;
using System.Windows.Forms;

namespace rentalsystem.Controls
{
    public partial class MaintenanceControl : UserControl
    {        public MaintenanceControl()        {            InitializeComponent();            LoadRequests();        }        public void LoadRequests()        {            var dt = DataAccess.GetAllMaintenanceRequests();            dgvRequests.DataSource = dt;        }        private void btnRefresh_Click(object sender, EventArgs e)        {            LoadRequests();        }        private void btnSetStatus_Click(object sender, EventArgs e)        {            if (dgvRequests.CurrentRow == null) { MessageBox.Show("Select a request."); return; }            var id = Convert.ToInt32(dgvRequests.CurrentRow.Cells["request_id"].Value);            var status = cmbStatus.SelectedItem?.ToString();            if (string.IsNullOrEmpty(status)) { MessageBox.Show("Select status."); return; }            var ok = DataAccess.UpdateMaintenanceRequestStatus(id, status);            MessageBox.Show(ok ? "Status updated." : "Failed.");            LoadRequests();        }    }}