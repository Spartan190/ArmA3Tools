using ArmA3PresetList;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArmA_3_Server_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string LastOpenedFile {
            get;
            private set;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SettingsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SettingsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(this);
            settingsWindow.ShowDialog();
        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ArmA 3 Preset File|*.html";
            if (openFileDialog.ShowDialog() == true)
            {
                LastOpenedFile = openFileDialog.FileName;
                LoadModInfos(LastOpenedFile);
            }
        }

        public void LoadModInfos(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                ArmA3PresetFile armA3PresetFile = new ArmA3PresetFile(fileName);
                StringBuilder modNamesStringBuilder = new StringBuilder();
                StringBuilder modIdsStringBuilder = new StringBuilder();
                StringBuilder checkRegexStringBuilder = new StringBuilder();

                var settings = Properties.Settings.Default;
                string modNamesSeperator = settings.ModNamesSeperator;
                string modNamesPrefix = settings.ModNamesPrefix;
                string modIdsSeperator = settings.ModIdsSeperator;

                bool isFirst = true;
                foreach (var armaMod in armA3PresetFile.armA3Mods)
                {

                    string modNameToSave = modNamesPrefix + HttpUtility.HtmlDecode(armaMod.displayName);
                    string modIdToSave = armaMod.workshopId;

                    bool modAlreadyAdded = modNamesStringBuilder.ToString().Contains(modNameToSave);

                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        if (!modAlreadyAdded)
                        {
                            modNamesStringBuilder.Append(modNamesSeperator);
                        }

                        modIdsStringBuilder.Append(modIdsSeperator);
                        checkRegexStringBuilder.Append("|");
                    }

                    if (!modAlreadyAdded)
                    {
                        modNamesStringBuilder.Append(modNameToSave);
                    }

                    modIdsStringBuilder.Append(modIdToSave);
                    checkRegexStringBuilder.Append($"({armaMod.displayName.Replace("(", "\\(").Replace(")", "\\)").Replace(".", "\\.")}\\n)");

                }

                displayNamesRichTextBox.Document.Blocks.Clear();
                displayNamesRichTextBox.Document.Blocks.Add(new Paragraph(new Run(modNamesStringBuilder.ToString())));

                modIdsRichTextBox.Document.Blocks.Clear();
                modIdsRichTextBox.Document.Blocks.Add(new Paragraph(new Run(modIdsStringBuilder.ToString())));

                regexRichTextBox.Document.Blocks.Clear();
                regexRichTextBox.Document.Blocks.Add(new Paragraph(new Run(checkRegexStringBuilder.ToString())));
            }
        }
    }
}
