using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ArmA_3_Server_Tool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //SetLanguageDictionary();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow wnd = new MainWindow();
            
            wnd.Show();
            if (e.Args.Length == 1)
            {
                wnd.LoadModInfos(e.Args[0]);
            }
        }

        private void SetLanguageDictionary()
        {
            ArmA_3_Server_Tool.Properties.Resources.Culture = new System.Globalization.CultureInfo("en");

        }
    }
}
