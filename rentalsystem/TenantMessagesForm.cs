using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace rentalsystem
{
    public class TenantMessagesForm : Form
    {
        private int _userId;
        private DataGridView dgv;
        private Button btnSend;
        private Button btnRefresh;

        public TenantMessagesForm(int userId)
        {
            _userId = userId;
            InitializeComponents();
            LoadMessages();
        }

        private void InitializeComponents()
        {
            this.Text = "Messages";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            dgv = new DataGridView { Dock = DockStyle.Top, Height = 380, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            btnSend = new Button { Text = "Send Message", Width = 120, Left = 10, Top = 390 };
            btnRefresh = new Button { Text = "Refresh", Width = 80, Left = 140, Top = 390 };

            btnSend.Click += BtnSend_Click;
            btnRefresh.Click += (s, e) => LoadMessages();

            this.Controls.Add(dgv);
            this.Controls.Add(btnSend);
            this.Controls.Add(btnRefresh);
        }

        private void LoadMessages()
        {
            var dt = DataAccess.GetMessagesForUser(_userId);
            dgv.DataSource = dt;
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            var toUsername = Prompt.ShowDialog("Enter recipient username:", "Send Message", "");
            if (string.IsNullOrWhiteSpace(toUsername)) return;
            var receiverId = DataAccess.GetUserIdByUsername(toUsername);
            if (receiverId == 0)
            {
                MessageBox.Show("Recipient not found.");
                return;
            }

            var content = Prompt.ShowDialog("Message content:", "Send Message", "");
            if (content == null) return;
            if (string.IsNullOrWhiteSpace(content)) { MessageBox.Show("Message cannot be empty."); return; }

            var msgId = DataAccess.SendMessage(_userId, receiverId, content);
            if (msgId > 0)
            {
                MessageBox.Show("Message sent.");
                LoadMessages();
            }
            else
            {
                MessageBox.Show("Failed to send message.");
            }
        }
    }
}
