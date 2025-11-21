using System;
using System.Data;
using System.Windows.Forms;

namespace rentalsystem.Controls
{
    public partial class MessagesControl : UserControl
    {
        public int CurrentUserId { get; set; }

        public MessagesControl()
        {
            InitializeComponent();
            // Do not auto-load here; allow parent to set CurrentUserId then call LoadMessages
            LoadMessages();
        }

        public void LoadMessages()
        {
            DataTable dt;
            if (CurrentUserId > 0)
            {
                dt = DataAccess.GetMessagesForUser(CurrentUserId);
            }
            else
            {
                dt = DataAccess.GetAllMessages();
            }

            dgvMessages.DataSource = dt;
            if (dgvMessages.Columns.Contains("message_id")) dgvMessages.Columns["message_id"].HeaderText = "ID";
            if (dgvMessages.Columns.Contains("sender_username")) dgvMessages.Columns["sender_username"].HeaderText = "Sender";
            if (dgvMessages.Columns.Contains("receiver_username")) dgvMessages.Columns["receiver_username"].HeaderText = "Receiver";
            if (dgvMessages.Columns.Contains("sent_at")) dgvMessages.Columns["sent_at"].HeaderText = "Sent At";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadMessages();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (CurrentUserId == 0)
            {
                MessageBox.Show("Current user not set. Cannot send message.");
                return;
            }

            var toUsername = Prompt.ShowDialog("Enter recipient username:", "Send Message", "");
            if (string.IsNullOrWhiteSpace(toUsername)) return;
            var receiverId = DataAccess.GetUserIdByUsername(toUsername);
            if (receiverId == 0) { MessageBox.Show("Recipient not found."); return; }
            var content = Prompt.ShowDialog("Message content:", "Send Message", "");
            if (content == null) return;
            if (string.IsNullOrWhiteSpace(content)) { MessageBox.Show("Message cannot be empty."); return; }

            var msgId = DataAccess.SendMessage(CurrentUserId, receiverId, content);
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

        private void btnReply_Click(object sender, EventArgs e)
        {
            if (dgvMessages.CurrentRow == null) { MessageBox.Show("Select a message to reply to."); return; }
            if (CurrentUserId == 0) { MessageBox.Show("Current user not set. Cannot reply."); return; }

            var row = dgvMessages.CurrentRow;
            int senderId = 0, receiverId = 0;
            try { senderId = row.Cells["sender_id"].Value != DBNull.Value ? Convert.ToInt32(row.Cells["sender_id"].Value) : 0; } catch { }
            try { receiverId = row.Cells["receiver_id"].Value != DBNull.Value ? Convert.ToInt32(row.Cells["receiver_id"].Value) : 0; } catch { }

            int recipientId = 0;
            if (senderId == CurrentUserId && receiverId != 0) recipientId = receiverId;
            else if (receiverId == CurrentUserId && senderId != 0) recipientId = senderId;
            else
            {
                // fallback: ask for username
                var toUsername = Prompt.ShowDialog("Enter recipient username:", "Reply Message", "");
                if (string.IsNullOrWhiteSpace(toUsername)) return;
                recipientId = DataAccess.GetUserIdByUsername(toUsername);
                if (recipientId == 0) { MessageBox.Show("Recipient not found."); return; }
            }

            var content = Prompt.ShowDialog("Message content:", "Reply Message", "");
            if (content == null) return;
            if (string.IsNullOrWhiteSpace(content)) { MessageBox.Show("Message cannot be empty."); return; }

            var msgId = DataAccess.SendMessage(CurrentUserId, recipientId, content);
            if (msgId > 0)
            {
                MessageBox.Show("Reply sent.");
                LoadMessages();
            }
            else
            {
                MessageBox.Show("Failed to send reply.");
            }
        }
    }
}