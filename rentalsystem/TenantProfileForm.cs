using System;
using System.Data;
using System.Windows.Forms;

namespace rentalsystem
{
    public partial class TenantProfileForm : Form
    {
        private int _userId;
        public TenantProfileForm(int userId)
        {
            _userId = userId;
            InitializeComponent();
            LoadProfile();
        }
        private void LoadProfile()        {            var row = DataAccess.GetUserProfile(_userId);            if (row == null) return;            txtFullName.Text = row["full_name"].ToString();            txtEmail.Text = row["email"].ToString();            txtContact.Text = row["contact"].ToString();            txtUsername.Text = row["username"].ToString();        }        private void btnSave_Click(object sender, EventArgs e)        {            var fullName = txtFullName.Text.Trim();            var email = txtEmail.Text.Trim();            var contact = txtContact.Text.Trim();            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email))            {                MessageBox.Show("Full name and email are required.");                return;            }            var ok = DataAccess.UpdateUserProfile(_userId, fullName, email, contact);            MessageBox.Show(ok ? "Profile updated." : "Failed to update profile.");            if (ok) this.Close();        }    }}