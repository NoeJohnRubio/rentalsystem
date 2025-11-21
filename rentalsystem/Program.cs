using System;
using System.Windows.Forms;

namespace rentalsystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var login = new Form1())
            {
                login.StartPosition = FormStartPosition.CenterScreen;
                var dr = login.ShowDialog();
                if (dr == DialogResult.OK && login.IsAuthenticated)
                {
                    Form dashboard;
                    if (string.Equals(login.AuthRole, "ADMIN", StringComparison.OrdinalIgnoreCase) || string.Equals(login.AuthRole, "admin", StringComparison.OrdinalIgnoreCase))
                        dashboard = new AdminForm(login.AuthUsername);
                    else
                        dashboard = new TenantForm(login.AuthUsername);

                    dashboard.StartPosition = FormStartPosition.CenterScreen;
                    Application.Run(dashboard);
                }
            }
        }
    }
}
