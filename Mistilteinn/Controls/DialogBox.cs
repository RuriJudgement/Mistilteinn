using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mistilteinn.Helper;
using VisualTreeHelper = Mistilteinn.Helper.VisualTreeHelper;

namespace Mistilteinn.Controls
{
    public class DialogBox : Control
    {
        public DialogBox()
        {
            this.Loaded += DialogBox_Loaded;
        }

        private TextBox _nameBoard, _content;
        private void DialogBox_Loaded(object sender, RoutedEventArgs e)
        {
            SetStates();

            _nameBoard = VisualTreeHelper.FindVisualElementFormName(this, nameof(NameBoard)) as TextBox;
            _content = VisualTreeHelper.FindVisualElementFormName(this, nameof(Content)) as TextBox;
        }

        static DialogBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogBox), new FrameworkPropertyMetadata(typeof(DialogBox)));
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof (String), typeof (DialogBox), new PropertyMetadata(default(String)));

        public String Content
        {
            get { return (String) GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty NameBoardProperty = DependencyProperty.Register(
            "NameBoard", typeof (String), typeof (DialogBox), new PropertyMetadata(default(String), (o, args) =>
            {
                //(o as DialogBox).SetStates();
            }));

        public String NameBoard
        {
            get { return (String) GetValue(NameBoardProperty); }
            set { SetValue(NameBoardProperty, value); }
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly", typeof (bool), typeof (DialogBox), new PropertyMetadata(default(bool)));

        public bool IsReadOnly
        {
            get { return (bool) GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        private void SetStates()
        {
            if (String.IsNullOrEmpty(NameBoard))
            {
                VisualStateManager.GoToState(this, "Monologue", true);
            }
            else if(NameBoard.Contains("天城恵介"))
            {
                VisualStateManager.GoToState(this, "Hero", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Other", true);
            }
        }

        public void NameBoardGetFocus()
        {
            _nameBoard.Focus();
        }

        public void ContentGet()
        {
            _content.Focus();
        }
    }
}
