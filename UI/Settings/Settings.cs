using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StorefrontApps_Outlook.UI.Settings
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.username = txtUsername.Text.Trim();
            Properties.Settings.Default.password = txtPassword.Text.Trim();
            Properties.Settings.Default.domain = txtDomain.Text.Trim();

            if (txtStorefrontURL.Text.Trim().EndsWith("/"))
            {
                Properties.Settings.Default.storefronturl = txtStorefrontURL.Text.Trim();
            }
            else
            {
                Properties.Settings.Default.storefronturl = txtStorefrontURL.Text.Trim() + @"/";
            }
            try
            {
                Properties.Settings.Default.Save();
            }
            catch ( System.Exception err)
            {
                Console.WriteLine(err.Message);
            }
            ThisAddIn.TaskPaneHost.elementHost1.Child = new UI.MyApps();

            Close();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            txtStorefrontURL.Text = Properties.Settings.Default.storefronturl.Trim();
            txtUsername.Text = Properties.Settings.Default.username.Trim();
            txtPassword.Text = Properties.Settings.Default.password.Trim();
            txtDomain.Text = Properties.Settings.Default.domain.Trim();
        }
    }
}
