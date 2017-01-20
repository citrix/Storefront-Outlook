using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.Threading;

namespace StorefrontApps_Outlook
{
    public partial class ThisAddIn
    {
        private StorefrontUIHost _storefrontUIHost;
        private Microsoft.Office.Tools.CustomTaskPane _storefrontTaskPane;
        internal static StorefrontUIHost TaskPaneHost
        {
            get
            {
                return ThisAddIn.TaskPaneHost;
            }
        }

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            //create the host control
            this._storefrontUIHost = new StorefrontUIHost();
            if (Properties.Settings.Default.username.Trim() == "" ||
                Properties.Settings.Default.password.Trim() == "" ||
                Properties.Settings.Default.storefronturl.Trim() == "" ||
                Properties.Settings.Default.domain.Trim() == "")
            {
                _storefrontUIHost.elementHost1.Child = new UI.Notification();
            }
            else
            {
                _storefrontUIHost.elementHost1.Child = new UI.MyApps();
            }


            //create the task pane
            this._storefrontTaskPane = this.CustomTaskPanes.Add(this._storefrontUIHost, "My Citrix Apps");
            this._storefrontTaskPane.Visible = true;
            
        }
        
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see http://go.microsoft.com/fwlink/?LinkId=506785
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
