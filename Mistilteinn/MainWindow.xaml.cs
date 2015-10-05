using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Mistilteinn.Texts;
using Mistilteinn.Unit;
using Mistilteinn.ViewModels;

namespace Mistilteinn
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public void SetProject(Project project)
        {
            SoundUnit.Initialize();
            var vm = new MainWindowViewModel()
            {
                IsMute = project.IsMute,
                IsMusicMute = project.IsMusicMute,
                IsVoiceMute = project.IsVoiceMute,
                FontSize = project.FontSize
            };

            foreach (var file in Directory.EnumerateFiles(project.TextPath, project.FileExtension))
            {
                var textFile = TextFile.FromFile(file);
                textFile.FileIndex = vm.TextFiles.Count + 1;
                vm.TextFiles.Add(textFile);
            }

            if (String.IsNullOrEmpty(project.NameTablePath))
            {
                NameTableUnit.LoadNameTable(project.NameTablePath);
            }

            vm.PreviewVisibility = project.IsPreviewEnable ? Visibility.Visible : Visibility.Collapsed;


            this.DataContext = vm;

            FileList.SelectedIndex = Project.Current.FileIndex == -1 ? 0 : Project.Current.FileIndex;
            TextList.SelectedIndex = Project.Current.TextIndex == -1 ? 0 : Project.Current.FileIndex;
        }
        
        protected override void OnClosing(CancelEventArgs e)
        {

            var result = MessageDialog.Show("保存了文本吗？确定要退出吗？", "退出", Char.ConvertFromUtf32(0xe84f),
                MessageDialogButtons.OkCancel);
            if (result == MessageDialogResultButton.Ok)
            {
                base.OnClosing(e);
            }
            else
            {
                e.Cancel = true;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            if (this.DataContext == null)
            {
                DataContext = new MainWindowViewModel();
            }
        }
        
        void AddNameTable(int index, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                if ((DataContext as MainWindowViewModel).ActiveNameTable.Count >= index)
                {
                    if (ClassicTextEditer.Visibility == Visibility.Visible)
                    {
                        ClassicTextEditer.AddText((DataContext as MainWindowViewModel).ActiveNameTable[index - 1].TranslatedText);
                        return;
                    }
                    if (GamePreview.Visibility == Visibility.Visible)
                    {
                        GamePreview.AddText((DataContext as MainWindowViewModel).ActiveNameTable[index - 1].TranslatedText);
                        return;
                    }
                }
            }
        }

        private void GamePreview_KeyPressed(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    PreviousText();
                    break;
                case Key.Down:
                case Key.Enter:
                    NextText();
                    break;
                case Key.Left:
                    PreviousTextFile();
                    break;
                case Key.Right:
                    NextTextFile();
                    break;
                case Key.D1:
                case Key.NumPad1:
                    AddNameTable(1, e);
                    break;
                case Key.D2:
                case Key.NumPad2:
                    AddNameTable(2, e);
                    break;
                case Key.D3:
                case Key.NumPad3:
                    AddNameTable(3, e);
                    break;
                case Key.D4:
                case Key.NumPad4:
                    AddNameTable(4, e);
                    break;
                case Key.D5:
                case Key.NumPad5:
                    AddNameTable(5, e);
                    break;
                case Key.D6:
                case Key.NumPad6:
                    AddNameTable(6, e);
                    break;
                case Key.D7:
                case Key.NumPad7:
                    AddNameTable(7, e);
                    break;
                case Key.D8:
                case Key.NumPad8:
                    AddNameTable(8, e);
                    break;
                case Key.D9:
                case Key.NumPad9:
                    AddNameTable(9, e);
                    break;
            }
        }

        bool NextText()
        {
            if (TextList.SelectedIndex == TextList.Items.Count - 1)
            {
                if (NextTextFile())
                {
                    TextList.SelectedIndex = 0;
                    TextList.ScrollIntoView(TextList.SelectedItem);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                TextList.SelectedIndex++;
                TextList.ScrollIntoView(TextList.SelectedItem);
                return true;
            }
        }

        bool NextTextFile()
        {
            if (FileList.SelectedIndex == FileList.Items.Count - 1)
            {
                MessageDialog.Show("已到最后一个文本文件。", "文本跳转", Char.ConvertFromUtf32(0xe886));
                return false;
            }
            else
            {
                FileList.SelectedIndex++;
                TextList.SelectedIndex = 0;
                TextList.ScrollIntoView(TextList.SelectedItem);
                return true;
            }
        }

        bool PreviousText()
        {
            if (TextList.SelectedIndex == 0)
            {
                if (PreviousTextFile())
                {
                    TextList.SelectedIndex = TextList.Items.Count - 1;
                    TextList.ScrollIntoView(TextList.SelectedItem);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                TextList.SelectedIndex--;
                TextList.ScrollIntoView(TextList.SelectedItem);
                return true;
            }
        }

        bool PreviousTextFile()
        {
            if (FileList.SelectedIndex == 0)
            {
                MessageDialog.Show("已到第一个一个文本文件。", "文本跳转", Char.ConvertFromUtf32(0xe886));
                return false;
            }
            else
            {
                FileList.SelectedIndex--;
                TextList.SelectedIndex = 0;
                TextList.ScrollIntoView(TextList.SelectedItem);
                return true;
            }
        }
        
        private void FileList_SelectionChanged_1(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (FileList.SelectedIndex != Project.Current.FileIndex)
            {
                TextList.SelectedIndex = 0;
            }

            if (TextList.SelectedItem != null)
            {
                TextList.ScrollIntoView(TextList.SelectedItem);
            }

            Project.Current.FileIndex = FileList.SelectedIndex;
        }
        
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Project.Current == null)
            {
                //await Task.Delay(100);
                var project = await WelcomeWindow.ShowWelcomeWindow();
                if (project != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        SetProject(project);
                    });
                }
            }
            else
            {
                SetProject(Project.Current);
            }
        }

        private async void NewProjectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var project = await NewProjectWindow.CreateProject();
            if (project != null)
            {
                SetProject(project);
            }
        }

        private void OpenProjectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var project = Project.OpenProject(this);
            if (project != null)
            {
                SetProject(project);
            }
        }


        private void SaveProjectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Project.SaveProject();
        }

        private void SaveAsProjectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Project.SaveProject(true);
        }

        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
        
        private void EditRawCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void EditInfoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new TextInfoEditWindow()
            {
                DataContext = FileList.SelectedItem,
                Owner = this
            }.ShowDialog();
        }

        private void FastFindCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (FindInOriginalWindow.Current != null)
            {
                FindInOriginalWindow.Current.Close();
            }
            if (FindInTranslatedWindow.Current != null)
            {
                FindInTranslatedWindow.Current.Close();
            }
            if (FastFindWindow.Current == null)
            {
                new FastFindWindow() {Owner = this}.Show();
            }
            else
            {
                FastFindWindow.Current.Activate();
            }
        }

        private void FindInOriginalCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (FastFindWindow.Current != null)
            {
                FastFindWindow.Current.Close();
            }
            if (FindInTranslatedWindow.Current != null)
            {
                FindInTranslatedWindow.Current.Close();
            }
            if (FindInOriginalWindow.Current == null)
            {
                new FindInOriginalWindow() { Owner = this }.Show();
            }
            else
            {
                FindInOriginalWindow.Current.Activate();
            }
        }

        private void FindInTranslatedCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (FastFindWindow.Current != null)
            {
                FastFindWindow.Current.Close();
            }
            if (FindInOriginalWindow.Current != null)
            {
                FindInOriginalWindow.Current.Close();
            }
            if (FindInTranslatedWindow.Current == null)
            {
                new FindInTranslatedWindow() { Owner = this }.Show();
            }
            else
            {
                FindInTranslatedWindow.Current.Activate();
            }
        }

        private void NameTableCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void ProjectSettingCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new SettingsWindow() {DataContext = Project.Current,Owner = this}.ShowDialog();
            SetProject(Project.Current);
        }

        private void CheckCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new CheckTextWindow() {DataContext = new CheckTextWindowViewModel(), Owner = this}.Show();
        }

        private void MuteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as MainWindowViewModel).IsMute = !(DataContext as MainWindowViewModel).IsMute;
        }

        private void CloseMusicCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as MainWindowViewModel).IsMusicMute = !(DataContext as MainWindowViewModel).IsMusicMute;
        }

        private void CloseVoiceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as MainWindowViewModel).IsVoiceMute = !(DataContext as MainWindowViewModel).IsVoiceMute;
        }

        private void ReplayVoiceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SoundUnit.ReplayVoice();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid lv = sender as DataGrid;

            if (lv.SelectedIndex > 0 && (DataContext as MainWindowViewModel).ActiveNameTable.Count >= lv.SelectedIndex + 1)
            {
                GamePreview.AddText((DataContext as MainWindowViewModel).ActiveNameTable[lv.SelectedIndex].TranslatedText);
            }
        }

        private void DataGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;

            double width = e.NewSize.Width - dataGrid.Columns[0].ActualWidth;
            width = width/(dataGrid.Columns.Count - 1);

            for (int i = 1; i < dataGrid.Columns.Count; i++)
            {
                dataGrid.Columns[i].Width = width - 9;
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;

            if (dataGrid.SelectedItem != null)
            {
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);
            }

            Project.Current.TextIndex = dataGrid.SelectedIndex;
            var text = dataGrid.SelectedItem as Text;
            var removeText = e.RemovedItems.Count > 0 ? e.RemovedItems[0] as Text : null;
            if (removeText != null)
            {
                removeText.CheckResult.IsSelected = false;
            }
            if (text != null)
            {
                if (text.CheckResult != null)
                {
                    text.CheckResult.IsSelected = true;
                }
                (DataContext as MainWindowViewModel).ActiveNameTable = NameTableUnit.NameTable.GetActiveCollection(text.OriginalText);
                SoundUnit.ReadScript(text);
            }
        }

        public void SetTextIndex(int fileIndex, int textIndex)
        {
            FileList.SelectedIndex = fileIndex;
            TextList.SelectedIndex = textIndex;
        }

        public void GetTextIndex(out int fileIndex, out int textIndex)
        {
            fileIndex = FileList.SelectedIndex;
            textIndex = TextList.SelectedIndex;
        }

        private void NextInfomationTextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GotoNextText(t => t.CheckResult.Importance.HasFlag(ErrorImportance.Infomation));
        }

        private void NextWarnningTextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GotoNextText(t => t.CheckResult.Importance.HasFlag(ErrorImportance.Warnning));
        }

        private void NextErrorTextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GotoNextText(t => t.CheckResult.Importance.HasFlag(ErrorImportance.Error));
        }

        private void PreviousInfomationTextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GotoPreviousText(t => t.CheckResult.Importance.HasFlag(ErrorImportance.Infomation));
        }

        private void PreviousWarnningTextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GotoPreviousText(t => t.CheckResult.Importance.HasFlag(ErrorImportance.Warnning));
        }

        private void PreviousErrorTextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GotoPreviousText(t => t.CheckResult.Importance.HasFlag(ErrorImportance.Error));
        }

        bool GotoNextText(Func<Text,bool> filter)
        {
            var viewModel = DataContext as MainWindowViewModel;

            for (int fileIndex = FileList.SelectedIndex; fileIndex < viewModel.TextFiles.Count; fileIndex++)
            {
                var textFile = viewModel.TextFiles[fileIndex];
                var textStart = 0;
                if (fileIndex == FileList.SelectedIndex)
                {
                    textStart = TextList.SelectedIndex + 1;
                }
                else
                {
                    textStart = 0;
                }
                for (int textIndex = textStart; textIndex < textFile.Texts.Count; textIndex++)
                {
                    var text = textFile.Texts[textIndex];
                    if (filter(text))
                    {
                        SetTextIndex(fileIndex, textIndex);
                        return true;
                    }
                }
            }

            return false;
        }

        bool GotoPreviousText(Func<Text, bool> filter)
        {
            var viewModel = DataContext as MainWindowViewModel;

            for (int fileIndex = FileList.SelectedIndex; fileIndex >= 0; fileIndex--)
            {
                var textFile = viewModel.TextFiles[fileIndex];
                var textStart = 0;
                if (fileIndex == FileList.SelectedIndex)
                {
                    textStart = TextList.SelectedIndex - 1;
                }
                else
                {
                    textStart = textFile.Texts.Count - 1;
                }
                for (int textIndex = textStart; textIndex >= 0; textIndex--)
                {
                    var text = textFile.Texts[textIndex];
                    if (filter(text))
                    {
                        SetTextIndex(fileIndex, textIndex);
                        return true;
                    }
                }
            }

            return false;
        }

        private void ClassicViewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as MainWindowViewModel).IsClassicViewMode =
                !(DataContext as MainWindowViewModel).IsClassicViewMode;
        }

        private void PreviousCommentTextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GotoPreviousText(t => !String.IsNullOrEmpty(t.Comment));
        }

        private void NextCommentTextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GotoNextText(t => !String.IsNullOrEmpty(t.Comment));
        }

        public bool IsProjectValid => Project.Current != null;
        public bool IsTextFileValid => FileList?.SelectedItem != null;
        public bool IsTextValid => TextList?.SelectedItem != null;
        public bool IsNameTableValid => NameTableUnit.NameTable != null;
        public bool IsVoiceCanReplay => IsProjectValid && IsTextValid && Project.Current.IsVoiceEnable && !Project.Current.IsVoiceMute && !String.IsNullOrEmpty(SoundUnit.Voice);

        private void SaveProjectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid;
        }

        private void SaveAsProjectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid;
        }

        private void EditInfoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid && IsTextFileValid;
        }

        private void EditRawCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid && IsTextFileValid;
        }

        private void FastFindCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid && IsTextFileValid && IsTextValid;
        }

        private void FindInOriginalCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid && IsTextFileValid && IsTextValid;
        }

        private void FindInTranslatedCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid && IsTextFileValid && IsTextValid;
        }

        private void CheckCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid;
        }

        private void NameTableCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid && IsNameTableValid;
        }

        private void ProjectSettingCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid;
        }

        private void MuteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid;
        }

        private void CloseMusicCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid;
        }

        private void CloseVoiceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid;
        }

        private void ReplayVoiceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsVoiceCanReplay;
        }

        private void NextInfomationTextCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid && IsTextFileValid && IsTextValid;
        }

        private void NextWarnningTextCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid && IsTextFileValid && IsTextValid;
        }

        private void NextErrorTextCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid && IsTextFileValid && IsTextValid;
        }

        private void PreviousInfomationTextCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid && IsTextFileValid && IsTextValid;
        }

        private void PreviousWarnningTextCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid && IsTextFileValid && IsTextValid;
        }

        private void PreviousErrorTextCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid && IsTextFileValid && IsTextValid;
        }

        private void PreviousCommentTextCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid && IsTextFileValid && IsTextValid;
        }

        private void NextCommentTextCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid && IsTextFileValid && IsTextValid;
        }

        private void ClassicViewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsProjectValid;
        }
    }
}
