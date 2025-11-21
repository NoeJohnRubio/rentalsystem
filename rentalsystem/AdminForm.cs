using System;
using System.Windows.Forms;
using rentalsystem.Controls;

namespace rentalsystem
{
    public partial class AdminForm : Form
    {
        private string _username;
        public AdminForm(string username)
        {
            _username = username;
            InitializeComponent();
            LoadUsersView();
        }

        private void ClearMain()
        {
            foreach (Control c in panelMain.Controls) c.Dispose();
            panelMain.Controls.Clear();
        }

        private void LoadUsersView()
        {
            ClearMain();
            var uc = new UsersControl { Dock = DockStyle.Fill };
            panelMain.Controls.Add(uc);
        }

        private void LoadPropertiesView()
        {
            ClearMain();
            var uc = new PropertiesControl { Dock = DockStyle.Fill };
            panelMain.Controls.Add(uc);
        }

        private void LoadPaymentsView()
        {
            ClearMain();
            var uc = new PaymentsControl { Dock = DockStyle.Fill };
            panelMain.Controls.Add(uc);
        }

        private void LoadMessagesView()
        {
            ClearMain();
            var uc = new MessagesControl { Dock = DockStyle.Fill };
            // set current admin user id so messages show admin's inbox/sent and reply uses correct sender id
            var uid = DataAccess.GetUserIdByUsername(_username);
            if (uid > 0) uc.CurrentUserId = uid;
            uc.LoadMessages();
            panelMain.Controls.Add(uc);
        }

        private void LoadMaintenanceView()
        {
            ClearMain();
            var uc = new MaintenanceControl { Dock = DockStyle.Fill };
            panelMain.Controls.Add(uc);
        }

        private void LoadNotificationsView()
        {
            ClearMain();
            var uc = new NotificationsControl { Dock = DockStyle.Fill };
            var uid = DataAccess.GetUserIdByUsername(_username);
            if (uid > 0) uc.CurrentUserId = uid;
            uc.LoadNotifications();
            panelMain.Controls.Add(uc);
        }

        private void btnUsers_Click(object sender, EventArgs e) => LoadUsersView();
        private void btnProperties_Click(object sender, EventArgs e) => LoadPropertiesView();
        private void btnPayments_Click(object sender, EventArgs e) => LoadPaymentsView();
        private void btnMessages_Click(object sender, EventArgs e) => LoadMessagesView();
        private void btnMaintenance_Click(object sender, EventArgs e) => LoadMaintenanceView();
        private void btnNotifications_Click(object sender, EventArgs e) => LoadNotificationsView();

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Close admin dashboard and restart login flow
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