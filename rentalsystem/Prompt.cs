using System;
using System.Windows.Forms;

namespace rentalsystem
{
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption, string defaultValue = "")
        {
            using (var form = new Form())
            {
                form.Width = 420;
                form.Height = 150;
                form.Text = caption;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterParent;
                form.MinimizeBox = false;
                form.MaximizeBox = false;

                var lbl = new Label() { Left = 10, Top = 10, Text = text, AutoSize = true };
                var txt = new TextBox() { Left = 10, Top = 35, Width = 380, Text = defaultValue };
                var ok = new Button() { Text = "OK", Left = 220, Width = 75, Top = 70, DialogResult = DialogResult.OK };
                var cancel = new Button() { Text = "Cancel", Left = 305, Width = 75, Top = 70, DialogResult = DialogResult.Cancel };

                form.AcceptButton = ok;
                form.CancelButton = cancel;
                form.Controls.Add(lbl);
                form.Controls.Add(txt);
                form.Controls.Add(ok);
                form.Controls.Add(cancel);
                var result = form.ShowDialog();
                return result == DialogResult.OK ? txt.Text : null;
            }
        }
    }
}
