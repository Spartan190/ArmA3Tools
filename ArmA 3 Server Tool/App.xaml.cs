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
            SetLanguageDictionary();
        }

        private void SetLanguageDictionary()
        {
            ArmA_3_Server_Tool.Properties.Resources.Culture = new System.Globalization.CultureInfo("en");
            /*switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "nl-NL":
                    MyProject.Language.Resources.Culture = new System.Globalization.CultureInfo("nl-NL");
                    break;
                case "en-GB":
                    MyProject.Language.Resources.Culture = new System.Globalization.CultureInfo("en-GB");
                    break;
                default://default english because there can be so many different system language, we rather fallback on english in this case.
                    MyProject.Language.Resources.Culture = new System.Globalization.CultureInfo("en-GB");
                    break;
            }*/

        }
    }
}
