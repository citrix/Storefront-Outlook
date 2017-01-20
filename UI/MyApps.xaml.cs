using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StorefrontApps_Outlook.UI
{
    /// <summary>
    /// Interaction logic for MyApps.xaml
    /// </summary>
    public partial class MyApps : UserControl
    {       
        Classes.CitrixAuthCredential _authCreds = null;
        string username = null;
        string password = null;
        string domain = null;

        string storefrontURL = null;
        public MyApps()
        {
            InitializeComponent();
           
        }
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (Properties.Settings.Default.username.Trim() == "" ||
                Properties.Settings.Default.password.Trim() == "" ||
                Properties.Settings.Default.storefronturl.Trim() == "" ||
                Properties.Settings.Default.domain.Trim() == "")
            {

            }
            else
            {
                username = Properties.Settings.Default.username.Trim();
                password = Properties.Settings.Default.password.Trim();
                storefrontURL = Properties.Settings.Default.storefronturl.Trim();
                domain = Properties.Settings.Default.domain.Trim();
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            RetrieveApplications();
        }

        async private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Classes.CitrixApplicationInfo appInfo = (Classes.CitrixApplicationInfo)(sender as Border).Tag;

            string ica = await Classes.CitrixHelper.Storefront.RetreiveICA(storefrontURL, _authCreds, appInfo);

            //write out to the file system.
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filename = string.Format(@"{0}\{1}.ica", appDataPath, Guid.NewGuid().ToString("N"));
            File.WriteAllText(filename, ica);
            Process.Start(filename);
            Console.WriteLine(appInfo.AppLaunchURL);
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UI.Settings.Settings settings = new Settings.Settings();
            settings.ShowDialog();
        }

        private void RefreshAppsClick(object sender, MouseButtonEventArgs e)
        {
            RetrieveApplications();
        }

        async public void RetrieveApplications()
        {
            ApplicationListing.ItemsSource = null;
            //Authenticate to the Storefront API
            _authCreds = await Classes.CitrixHelper.Storefront.AuthenticateWithPost(
              storefrontURL,
              username, password, domain);

            var _apps = await Classes.CitrixHelper.Storefront.GetResources(
                storefrontURL,
                _authCreds,
                false);
            ApplicationListing.ItemsSource = _apps;
            foreach (var app in _apps)
            {
                Console.WriteLine(app.AppTitle);
            }

            ApplicationListing.ItemsSource = _apps;
        }
    }
}
