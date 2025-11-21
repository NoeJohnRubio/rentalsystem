using System;
using System.Data;
using System.Windows.Forms;

namespace rentalsystem.Controls
{
    public partial class NotificationsControl : UserControl
    {
        public int CurrentUserId { get; set; }

        public NotificationsControl()
        {
            InitializeComponent();
        }

        public void LoadNotifications()
        {
            if (CurrentUserId > 0)
            {
                // show notifications specific to this user
                var dt = DataAccess.GetNotificationsForUser(CurrentUserId);
                // replace label with grid if needed
                var dgv = new DataGridView { Dock = DockStyle.Fill, DataSource = dt, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
                var btnMark = new Button { Text = "Mark Read", Dock = DockStyle.Bottom, Height = 30 };
                btnMark.Click += (s, e) =>
                {
                    if (dgv.CurrentRow == null) { MessageBox.Show("Select a notification first."); return; }
                    var idObj = dgv.CurrentRow.Cells["notification_id"].Value;
                    if (idObj == null) { MessageBox.Show("Notification id not found."); return; }
                    var id = Convert.ToInt32(idObj);
                    var ok = DataAccess.MarkNotificationRead(id);
                    MessageBox.Show(ok ? "Marked read." : "Failed to mark.");
                    if (ok) dgv.DataSource = DataAccess.GetNotificationsForUser(CurrentUserId);
                };

                this.Controls.Clear();
                this.Controls.Add(dgv);
                this.Controls.Add(btnMark);
            }
            else
            {
                // show admin-wide notifications via existing query
                var dt = DataAccess.GetNotificationsForUser(0);
                var dgv = new DataGridView { Dock = DockStyle.Fill, DataSource = dt, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
                this.Controls.Clear();
                this.Controls.Add(dgv);
            }
        }
    }
}