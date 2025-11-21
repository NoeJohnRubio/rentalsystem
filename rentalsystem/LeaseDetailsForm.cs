using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace rentalsystem
{
    // Simple programmatic form to show lease details to tenant
    public class LeaseDetailsForm : Form
    {
        private int _leaseId;
        private DataRow _leaseRow;

        public LeaseDetailsForm(int leaseId)
        {
            _leaseId = leaseId;
            InitializeComponents();
            LoadLease();
        }

        private void InitializeComponents()
        {
            this.Text = "Lease Details";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.AutoScaleMode = AutoScaleMode.Font;

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            var table = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2 };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Labels
            AddRow(table, "Lease ID:", "");
            AddRow(table, "Property:", "");
            AddRow(table, "Tenant:", "");
            AddRow(table, "Rent Amount:", "");
            AddRow(table, "Start Date:", "");
            AddRow(table, "End Date:", "");
            AddRow(table, "Status:", "");

            var btnClose = new Button { Text = "Close", Anchor = AnchorStyles.Right, AutoSize = true };
            btnClose.Click += (s, e) => this.Close();

            panel.Controls.Add(table);
            panel.Controls.Add(btnClose);
            btnClose.Top = table.Bottom + 10;
            btnClose.Left = panel.Width - btnClose.Width - 20;
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            this.Controls.Add(panel);
        }

        private void AddRow(TableLayoutPanel table, string label, string value)
        {
            var lbl = new Label { Text = label, AutoSize = true, Padding = new Padding(0, 6, 0, 6) };
            var val = new Label { Text = value, AutoSize = true, Padding = new Padding(6, 6, 0, 6) };
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.Controls.Add(lbl);
            table.Controls.Add(val);
        }

        private void LoadLease()
        {
            _leaseRow = DataAccess.GetLeaseById(_leaseId);
            if (_leaseRow == null)
            {
                MessageBox.Show("Lease not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // Populate fields by finding controls in TableLayoutPanel
            var table = this.Controls[0].Controls[0] as TableLayoutPanel;
            if (table == null) return;

            // Expected order of values added in InitializeComponents
            SetCellText(table, 0, 1, _leaseRow["lease_id"].ToString());
            SetCellText(table, 1, 1, _leaseRow["property_name"]?.ToString() ?? "");
            SetCellText(table, 2, 1, _leaseRow["tenant_username"]?.ToString() ?? "");
            SetCellText(table, 3, 1, Convert.ToDecimal(_leaseRow["rent_amount"]).ToString("C"));
            SetCellText(table, 4, 1, _leaseRow["start_date"] != DBNull.Value ? Convert.ToDateTime(_leaseRow["start_date"]).ToShortDateString() : "");
            SetCellText(table, 5, 1, _leaseRow["end_date"] != DBNull.Value ? Convert.ToDateTime(_leaseRow["end_date"]).ToShortDateString() : "");
            SetCellText(table, 6, 1, _leaseRow["status"]?.ToString() ?? "");
        }

        private void SetCellText(TableLayoutPanel table, int row, int col, string text)
        {
            var index = row * table.ColumnCount + col;
            if (index < table.Controls.Count)
            {
                var ctrl = table.Controls[index] as Label;
                if (ctrl != null) ctrl.Text = text;
            }
        }
    }
}
