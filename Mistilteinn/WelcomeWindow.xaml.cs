using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Mistilteinn.ViewModels;

namespace Mistilteinn
{
    /// <summary>
    /// WelcomeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WelcomeWindow : Window
    {
        public static async Task<Project> ShowWelcomeWindow()
        {
            return await Application.Current.MainWindow.Dispatcher.Invoke(async () =>
            {
                var vm = new WelcomeWindowViewModel(new EventWaitHandle(false, EventResetMode.AutoReset));
                var window = new WelcomeWindow(vm) { Owner = Application.Current.MainWindow };
                window.ShowDialog();

                await Task.Run(() => vm.WaitHandle.WaitOne());

                return vm.Project;
            });
        }

        private readonly WelcomeWindowViewModel _viewModel;

        public WelcomeWindow(WelcomeWindowViewModel vm)
        {
            _viewModel = vm;
            InitializeComponent();
        }

        private async void CreateNew_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            _viewModel.Project = await NewProjectWindow.CreateProject();
            if (_viewModel.Project != null)
            {
                Close();
            }
            else
            {
                Show();
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            _viewModel.WaitHandle.Set();
        }

        private void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            var project = Project.OpenProject(this);
            if (project != null)
            {
                if ((Application.Current.MainWindow as MainWindow) != null)
                {
                    ((MainWindow)Application.Current.MainWindow).SetProject(project);
                }
                Close();
            }
        }
    }
}
