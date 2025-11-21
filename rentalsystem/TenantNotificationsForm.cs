using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace rentalsystem
{
    public class TenantNotificationsForm : Form
    {
        private int _userId;
        private DataGridView dgv;
        private Button btnMarkRead;
        private Button btnRefresh;

        public TenantNotificationsForm(int userId)
        {
            _userId = userId;
            InitializeComponents();
            LoadNotifications();
        }

        private void InitializeComponents()
        {
            this.Text = "Notifications";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            dgv = new DataGridView { Dock = DockStyle.Top, Height = 380, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            btnMarkRead = new Button { Text = "Mark Read", Width = 100, Left = 10, Top = 390 };
            btnRefresh = new Button { Text = "Refresh", Width = 80, Left = 120, Top = 390 };

            btnMarkRead.Click += BtnMarkRead_Click;
            btnRefresh.Click += (s, e) => LoadNotifications();

            this.Controls.Add(dgv);
            this.Controls.Add(btnMarkRead);
            this.Controls.Add(btnRefresh);
        }

        private void LoadNotifications()
        {
            var dt = DataAccess.GetNotificationsForUser(_userId);
            dgv.DataSource = dt;
        }

        private void BtnMarkRead_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Select a notification first."); return; }
            var idObj = dgv.CurrentRow.Cells["notification_id"].Value;
            if (idObj == null) { MessageBox.Show("Notification id not found."); return; }
            var id = Convert.ToInt32(idObj);
            var ok = DataAccess.MarkNotificationRead(id);
            MessageBox.Show(ok ? "Marked read." : "Failed to mark.");
            if (ok) LoadNotifications();
        }
    }
}
