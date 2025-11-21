using System;
using System.Windows.Forms;

namespace rentalsystem
{
    public partial class AddPropertyForm : Form
    {
        public AddPropertyForm()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var name = txtName.Text.Trim();
            var addr = txtAddress.Text.Trim();
            var desc = txtDescription.Text.Trim();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(addr)) { MessageBox.Show("Name and address required"); return; }
            var id = DataAccess.AddProperty(name, addr, desc);
            if (id > 0) { MessageBox.Show("Property added"); this.DialogResult = DialogResult.OK; this.Close(); }
            else MessageBox.Show("Failed to add property");
        }
    }
}
