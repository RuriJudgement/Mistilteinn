using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mistilteinn.ViewModels
{
    public class MessageWindowViewModel : ViewModelBase
    {
        public MessageWindowViewModel(String title, String text, String icon, MessageDialogButtons buttons)
        {
            Title = title;
            Text = text;
            Icon = icon;
            switch (buttons)
            {
                case MessageDialogButtons.Ok:
                    OkVisibility = Visibility.Visible;
                    break;
                case MessageDialogButtons.OkCancel:
                    OkVisibility = Visibility.Visible;
                    CancelVisibility = Visibility.Visible;
                    break;
                case MessageDialogButtons.YesNo:
                    YesVisibility = Visibility.Visible;
                    NoVisibility = Visibility.Visible;
                    break;
                case MessageDialogButtons.YesNoCancel:
                    YesVisibility = Visibility.Visible;
                    NoVisibility = Visibility.Visible;
                    CancelVisibility = Visibility.Visible;
                    break;

            }
        }

        private string _title;
        private string _text;
        private string _icon;
        private Visibility _cancelVisibility = Visibility.Collapsed;
        private Visibility _okVisibility = Visibility.Collapsed;
        private Visibility _noVisibility = Visibility.Collapsed;
        private Visibility _yesVisibility = Visibility.Collapsed;

        public String Title
        {
            get { return _title; }
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public String Text
        {
            get { return _text; }
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        public String Icon
        {
            get { return _icon; }
            set
            {
                if (value == _icon) return;
                _icon = value;
                OnPropertyChanged();
            }
        }

        public Visibility YesVisibility
        {
            get { return _yesVisibility; }
            set
            {
                if (value == _yesVisibility) return;
                _yesVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility NoVisibility
        {
            get { return _noVisibility; }
            set
            {
                if (value == _noVisibility) return;
                _noVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility OkVisibility
        {
            get { return _okVisibility; }
            set
            {
                if (value == _okVisibility) return;
                _okVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility CancelVisibility
        {
            get { return _cancelVisibility; }
            set
            {
                if (value == _cancelVisibility) return;
                _cancelVisibility = value;
                OnPropertyChanged();
            }
        }

        public MessageDialogResultButton ResultButton { get; set; } = MessageDialogResultButton.Cancel;
    }
}
