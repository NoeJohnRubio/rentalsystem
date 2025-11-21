using System;
using System.Data;
using System.Windows.Forms;

namespace rentalsystem.Controls
{
    public partial class UsersControl : UserControl
    {        public UsersControl()        {            InitializeComponent();            LoadUsers();        }        public void LoadUsers()        {            var dt = DataAccess.GetUsers();            dgvUsers.DataSource = dt;        }        private void btnRefresh_Click(object sender, EventArgs e)        {            LoadUsers();        }        private void btnAdd_Click(object sender, EventArgs e)        {            using (var f = new AddUserForm())            {                if (f.ShowDialog() == DialogResult.OK)                {                    LoadUsers();                }            }        }    }}