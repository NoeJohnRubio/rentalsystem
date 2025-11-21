using System;
using System.Windows.Forms;

namespace rentalsystem
{
    public class AuthEventArgs : EventArgs
    {
        public string Role { get; }
        public string Username { get; }
        public int UserId { get; }

        public AuthEventArgs(string role, string username, int userId)
        {
            Role = role;
            Username = username;
            UserId = userId;
        }
    }

    public partial class Form1 : Form
    {
        // authentication result exposed for Program/MainApplicationContext
        public bool IsAuthenticated { get; private set; }
        public string AuthRole { get; private set; }
        public string AuthUsername { get; private set; }
        public int AuthUserId { get; private set; }

        // Event raised when user successfully authenticates (used by application context)
        public event EventHandler<AuthEventArgs> Authenticated;

        public Form1()
        {
            InitializeComponent();
            IsAuthenticated = false;
            // make Enter key activate login and ensure handler is attached
            this.AcceptButton = this.btnLogin;
            this.btnLogin.Click -= btnLogin_Click; // remove duplicate if any
            this.btnLogin.Click += btnLogin_Click;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text;
            var selectedRole = cmbRole.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(selectedRole))
            {
                MessageBox.Show("Enter username, password and select role.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var (success, role, uid, storedHash) = DataAccess.ValidateUserCredentials(username, password);
                if (!success)
                {
                    MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Compare role case-insensitively; DB may store upper/lowercase
                if (!string.Equals(role, selectedRole, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show($"Selected role does not match account role ({role}).", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // mark authenticated and raise event for ApplicationContext
                IsAuthenticated = true;
                AuthRole = role;
                AuthUsername = username;
                AuthUserId = uid;

                // raise event for backward compatibility
                Authenticated?.Invoke(this, new AuthEventArgs(AuthRole, AuthUsername, AuthUserId));

                // Always set DialogResult and close so ShowDialog returns to Program.Main
                try { this.DialogResult = DialogResult.OK; } catch { }
                try { this.Close(); } catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during login:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
