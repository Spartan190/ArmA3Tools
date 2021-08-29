using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ArmA_3_Server_Tool
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly MainWindow mainWindow;

        private UIHelper uiHelper = new UIHelper();

        public SettingsWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            LoadFTPPassword();
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFTPPassword();
            Properties.Settings.Default.Save();
            mainWindow.LoadModInfos(mainWindow.LastOpenedFile);
            uiHelper.ShowLabel(settingsSavedLabel, MainWindow.CopyLabelVisibiltyTime, Dispatcher);
            
        }

        private void SaveFTPPassword()
        {
            // Data to protect. Convert a string to a byte[] using Encoding.UTF8.GetBytes().
            byte[] plaintext = Encoding.UTF8.GetBytes(ftpPasswordTextBox.Password);

            // Generate additional entropy (will be used as the Initialization vector)
            byte[] entropy = new byte[20];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(entropy);
            }

            byte[] ciphertext = ProtectedData.Protect(plaintext, null,
                DataProtectionScope.CurrentUser);

            Properties.Settings.Default.FTPPassword = Convert.ToBase64String(ciphertext);
        }

        private void LoadFTPPassword()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.FTPPassword)) {
                // Data to protect. Convert a string to a byte[] using Encoding.UTF8.GetBytes().
                byte[] ciphertext = Convert.FromBase64String(Properties.Settings.Default.FTPPassword);

                // Generate additional entropy (will be used as the Initialization vector)
                byte[] entropy = new byte[20];
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(entropy);
                }

                try
                {
                    ftpPasswordTextBox.Password = Encoding.UTF8.GetString(ProtectedData.Unprotect(ciphertext, null,
                        DataProtectionScope.CurrentUser));
                } catch
                {
                    
                }
            }

        }
    }
}
