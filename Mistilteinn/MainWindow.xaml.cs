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
            var vm = new MainWindowViewModel()
            {
                IsMute = project.IsMute,
                IsMusicMute = project.IsMusicMute,
                IsVoiceMute = project.IsVoiceMute
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

            SoundUnit.Initialize();

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
                    GamePreview.AddText((DataContext as MainWindowViewModel).ActiveNameTable[index - 1].TranslatedText);
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
            await Task.Delay(500);

            var project = await WelcomeWindow.ShowWelcomeWindow();
            if (project != null)
            {
                Dispatcher.Invoke(() =>
                {
                    SetProject(project);
                });
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

        }

        private void CheckCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //new CheckTextWindow() { Owner = this }.Show();
        }

        private void MuteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as MainWindowViewModel).IsMute = !(DataContext as MainWindowViewModel).IsMute;
        }

        private void CloseMusicCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void CloseVoiceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {

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
    }
}
