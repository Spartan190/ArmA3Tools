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

        private void SetLanguageDictionary()
        {
            ArmA_3_Server_Tool.Properties.Resources.Culture = new System.Globalization.CultureInfo("en");

        }
    }
}
