using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Mistilteinn.Helper;
using Mistilteinn.ViewModels;
using Application = System.Windows.Application;

namespace Mistilteinn
{
    public partial class NewProjectWindow : Window
    {
        public static async Task<Project> CreateProject()
        {
            var vm = new NewProjectWindowViewModel(new EventWaitHandle(false, EventResetMode.AutoReset));
            var window = new NewProjectWindow(vm)
            {
                Owner = Application.Current.MainWindow
            };

            window.ShowDialog();

            await Task.Run(() => vm.WaitHandle.WaitOne());

            return vm.CreatedProject;
        }

        private readonly NewProjectWindowViewModel _viewModel;

        public NewProjectWindow(NewProjectWindowViewModel vm)
        {
            InitializeComponent();
            DataContext = _viewModel = vm;
        }

        private void WelcomeNext_Click(object sender, RoutedEventArgs e)
        {
            ContentTabControl.SelectedIndex++;
        }

        private void TextPrevious_Click(object sender, RoutedEventArgs e)
        {
            ContentTabControl.SelectedIndex--;
        }

        private void TextNext_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(_viewModel.TextFileFolder))
            {
                MessageDialog.Show(this, "文本文件目录不存在!", "文件夹不存在", Char.ConvertFromUtf32(0xe886));
                return;
            }
            else
            {
                if (!Directory.EnumerateFiles(_viewModel.TextFileFolder, _viewModel.TextFileEx).Any())
                {
                    MessageDialog.Show(this, "文本文件目录不包含任何文本文件!", "文件夹为空", Char.ConvertFromUtf32(0xe886));
                    return;
                }
            }
            ContentTabControl.SelectedIndex++;
        }

        private void ExtensionPrevious_Click(object sender, RoutedEventArgs e)
        {
            ContentTabControl.SelectedIndex--;
        }

        private void ExtensionNext_Click(object sender, RoutedEventArgs e)
        {
            var next = 1;

            if (!_viewModel.IsVoiceEnable)
            {
                next++;

                if (!_viewModel.IsMusicEnable)
                {
                    next++;
                    if (!_viewModel.IsPreviewEnable)
                    {
                        next++;

                    }
                }
            }

            ContentTabControl.SelectedIndex += next;
        }

        private void VoicePrevious_Click(object sender, RoutedEventArgs e)
        {
            ContentTabControl.SelectedIndex--;
        }

        private void VoiceNext_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(_viewModel.VoiceFileFolder))
            {
                MessageDialog.Show(this, "语音文件目录不存在!", "文件夹不存在", Char.ConvertFromUtf32(0xe886));
                return;
            }
            else
            {
                if (!Directory.EnumerateFiles(_viewModel.VoiceFileFolder).Any())
                {
                    MessageDialog.Show(this, "语音目录不包含任何文件!", "文件夹为空", Char.ConvertFromUtf32(0xe886));
                    return;
                }
            }

            var next = 1;

            if (!_viewModel.IsMusicEnable)
            {
                next++;
                if (!_viewModel.IsPreviewEnable)
                {
                    next++;

                }
            }

            ContentTabControl.SelectedIndex += next;
        }

        private void MusicPrevious_Click(object sender, RoutedEventArgs e)
        {
            var previous = 1;

            if (!_viewModel.IsVoiceEnable)
            {
                previous++;
            }

            ContentTabControl.SelectedIndex -= previous;
        }

        private void MusicNext_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(_viewModel.MusicFileFolder))
            {
                MessageDialog.Show(this, "音乐文件目录不存在!", "文件夹不存在", Char.ConvertFromUtf32(0xe886));
                return;
            }
            else
            {
                if (!Directory.EnumerateFiles(_viewModel.MusicFileFolder).Any())
                {
                    MessageDialog.Show(this, "音乐目录不包含任何文件!", "文件夹为空", Char.ConvertFromUtf32(0xe886));
                    return;
                }
            }

            var next = 1;

            if (!_viewModel.IsPreviewEnable)
            {
                next++;

            }

            ContentTabControl.SelectedIndex += next;
        }

        private void PreviewPrevious_Click(object sender, RoutedEventArgs e)
        {
            var previous = 1;

            if (!_viewModel.IsMusicEnable)
            {
                previous++;
                if (!_viewModel.IsVoiceEnable)
                {
                    previous++;

                }
            }

            ContentTabControl.SelectedIndex -= previous;
        }

        private void PreviewNext_Click(object sender, RoutedEventArgs e)
        {

            if (!Directory.Exists(_viewModel.CharaFileFolder))
            {
                MessageDialog.Show(this, "立绘文件目录不存在!", "文件夹不存在", Char.ConvertFromUtf32(0xe886));
                return;
            }
            else
            {
                if (!Directory.EnumerateFiles(_viewModel.CharaFileFolder).Any())
                {
                    MessageDialog.Show(this, "立绘目录不包含任何文件!", "文件夹为空", Char.ConvertFromUtf32(0xe886));
                    return;
                }
            }

            if (!Directory.Exists(_viewModel.FaceFileFolder))
            {
                MessageDialog.Show(this, "Face 文件目录不存在!", "文件夹不存在", Char.ConvertFromUtf32(0xe886));
                return;
            }
            else
            {
                if (!Directory.EnumerateFiles(_viewModel.FaceFileFolder).Any())
                {
                    MessageDialog.Show(this, "Face 目录不包含任何文件!", "文件夹为空", Char.ConvertFromUtf32(0xe886));
                    return;
                }
            }

            if (!Directory.Exists(_viewModel.BackgroundFileFolder))
            {
                MessageDialog.Show(this, "背景文件目录不存在!", "文件夹不存在", Char.ConvertFromUtf32(0xe886));
                return;
            }
            else
            {
                if (!Directory.EnumerateFiles(_viewModel.BackgroundFileFolder).Any())
                {
                    MessageDialog.Show(this, "背景目录不包含任何文件!", "文件夹为空", Char.ConvertFromUtf32(0xe886));
                    return;
                }
            }
            ContentTabControl.SelectedIndex ++;
        }
        
        private void Window_Closed(object sender, System.EventArgs e)
        {
            _viewModel.WaitHandle.Set();
        }

        private void Complete_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Create();
            Close();
        }

        private void TextFolderSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedFolder = DialogHelper.ShowFolderBrowserDialog("选择文本文件夹");
            if (selectedFolder != null)
            {
                TextFolderTextBox.Text = selectedFolder;
            }
        }

        private void NameTableSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedFile = DialogHelper.ShowOpenFileDialog(this,"选择名词表", "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*");
            if (selectedFile != null)
            {
                NameTableTextBox.Text = selectedFile;
            }
        }

        private void VoiceSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedFolder = DialogHelper.ShowFolderBrowserDialog("选择语音文件夹");
            if (selectedFolder != null)
            {
                VoiceTextBox.Text = selectedFolder;
            }
        }

        private void MusicSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedFolder = DialogHelper.ShowFolderBrowserDialog("选择背景音乐文件夹");
            if (selectedFolder != null)
            {
                MusicTextBox.Text = selectedFolder;
            }
        }

        private void CharaSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedFolder = DialogHelper.ShowFolderBrowserDialog("选择立绘文件夹");
            if (selectedFolder != null)
            {
                CharaTextBox.Text = selectedFolder;
            }
        }

        private void FaceSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedFolder = DialogHelper.ShowFolderBrowserDialog("选择 Face 文件夹");
            if (selectedFolder != null)
            {
                FaceTextBox.Text = selectedFolder;
            }
        }

        private void BackgroundSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedFolder = DialogHelper.ShowFolderBrowserDialog("选择背景图片文件夹");
            if (selectedFolder != null)
            {
                BackgroundTextBox.Text = selectedFolder;
            }
        }
    }
}
