using ArmA_3_Server_Tool.ViewModel;
using ArmA3PresetList;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace ArmA_3_Server_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public const int CopyLabelVisibiltyTime = 2 * 1000;

        public string LastOpenedFile {
            get;
            private set;
        }

        public ObservableCollection<ArmA3ModViewModel> A3Mods { get; } = new ObservableCollection<ArmA3ModViewModel>();

        private string title = "";

        private UIHelper uiHelper = new UIHelper();

        public MainWindow()
        {
            InitializeComponent();
            title = Title;
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
                LoadModInfos(openFileDialog.FileName);
            }
        }
        public void LoadModInfos(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                LastOpenedFile = fileName;
                Title = $"{title} ({Path.GetFileName(LastOpenedFile)})";


                A3Mods.Clear();

                ArmA3PresetFile armA3PresetFile = new ArmA3PresetFile(fileName);
                StringBuilder modNamesStringBuilder = new StringBuilder();
                StringBuilder modIdsStringBuilder = new StringBuilder();
                StringBuilder checkRegexStringBuilder = new StringBuilder();

                var settings = Properties.Settings.Default;
                List<string> addedMods = new List<string>();
                string modNamesSeperator = settings.ModNamesSeperator;
                string modNamesPrefix = settings.ModNamesPrefix;
                string modIdsSeperator = settings.ModIdsSeperator;

                int modNamesCount = 0;
                int modIdsCount = 0;
                int regexCount = 0;

                bool isFirst = true;
                foreach (var armaMod in armA3PresetFile.armA3Mods)
                {

                    string modNameToSave = modNamesPrefix + HttpUtility.HtmlDecode(armaMod.DisplayName);
                    string modIdToSave = armaMod.WorkshopId;

                    bool modAlreadyAdded = addedMods.Contains(modNameToSave);

                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        if (!modAlreadyAdded)
                        {
                            modNamesStringBuilder.Append(modNamesSeperator);
                            checkRegexStringBuilder.Append("|");
                        }

                        modIdsStringBuilder.Append(modIdsSeperator);
                    }

                    if (!modAlreadyAdded)
                    {
                        modNamesStringBuilder.Append(modNameToSave);
                        modNamesCount++;
                        checkRegexStringBuilder.Append($"({armaMod.DisplayName.Replace("(", "\\(").Replace(")", "\\)").Replace(".", "\\.")}\\n)");
                        regexCount++;
                        addedMods.Add(modNameToSave);
                    }

                    modIdsStringBuilder.Append(modIdToSave);
                    modIdsCount++;
                    A3Mods.Add(new ArmA3ModViewModel(armaMod));
                }


                modNamesCountLabel.Content = $"({modNamesCount})";

                modIdsCountLabel.Content = $"({modIdsCount})";
            }
        }

        private string GetRichTextBoxText(ref RichTextBox richTextBox)
        {
            if (richTextBox != null)
            {
                TextRange textRange = new TextRange(
                // TextPointer to the start of content in the RichTextBox.
                richTextBox.Document.ContentStart,
                // TextPointer to the end of content in the RichTextBox.
                richTextBox.Document.ContentEnd
                );

                // The Text property on a TextRange object returns a string
                // representing the plain text content of the TextRange.
                return textRange.Text.Trim();
            }

            return "";

        }

        private void SetRichTextBoxText(ref RichTextBox richTextBox, string text)
        {
            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(new Paragraph(new Run(text)));
        }

        private void CopyModNamesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;// check if list is set
        }

        private void CopyModNamesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
            uiHelper.ShowLabel(modIdsCopiedLabel, CopyLabelVisibiltyTime, Dispatcher);
        }

        private void CopyModIdsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true; // check if list is set
        }

        private void CopyModIdsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
            uiHelper.ShowLabel(modIdsCopiedLabel, CopyLabelVisibiltyTime, Dispatcher);
        }

        private void VerifyCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            
        }

        private void VerifyCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
        }
    }
}
