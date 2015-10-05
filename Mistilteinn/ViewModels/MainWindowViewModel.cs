using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Mistilteinn.Texts;
using Mistilteinn.Unit;

namespace Mistilteinn.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public static MainWindowViewModel Current { get; set; }
        public MainWindowViewModel()
        {
            TextFiles = new ObservableCollection<TextFile>();

            if (DesignerProperties.GetIsInDesignMode(Application.Current.MainWindow))
            {
                
            }

            Current = this;
        }

        private ObservableCollection<TextFile> _textFiles;

        public ObservableCollection<TextFile> TextFiles
        {
            get { return _textFiles; }
            set
            {
                if (_textFiles == value) return;
                _textFiles = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<NameTableItem> _activeNameTable;

        public ObservableCollection<NameTableItem> ActiveNameTable
        {
            get { return _activeNameTable; }
            set
            {
                if (_activeNameTable == value) return;
                _activeNameTable = value;
                OnPropertyChanged();
            }
        }

        private Visibility _previewVisibility = Visibility.Collapsed;

        public Visibility PreviewVisibility
        {
            get { return _previewVisibility;}
            set
            {
                if (_previewVisibility == value) return;
                _previewVisibility = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ClassicVisibility));
            }
        }
        public Visibility ClassicVisibility => PreviewVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

        public bool IsClassicViewMode
        {
            get { return _isClassicViewMode; }
            set
            {
                if (value == _isClassicViewMode) return;
                _isClassicViewMode = value;
                OnPropertyChanged();
                Project.Current.IsClassicViewModeOn = value;
                PreviewVisibility = (!Project.Current.IsPreviewEnable || value) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private bool _isMute;
        private bool _isMusicMute;
        private bool _isVoiceMute;
        private bool _isClassicViewMode;
        private double _fontSize;

        public bool IsMute
        {
            get { return _isMute; }
            set
            {
                if (value == _isMute) return;
                _isMute = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MuteIcon));
                Project.Current.IsMute = value;
                SoundUnit.SetMute();
            }
        }

        public String MuteIcon => Char.ConvertFromUtf32(IsMute ? 0xecc4 : 0xecc5);

        public bool IsMusicMute
        {
            get { return _isMusicMute; }
            set
            {
                if (value == _isMusicMute) return;
                _isMusicMute = value;
                OnPropertyChanged();
                Project.Current.IsMusicMute = value;
                SoundUnit.SetMute();
            }
        }

        public bool IsVoiceMute
        {
            get { return _isVoiceMute; }
            set
            {
                if (value == _isVoiceMute) return;
                _isVoiceMute = value;
                OnPropertyChanged();
                Project.Current.IsVoiceMute = value;
                SoundUnit.SetMute();
            }
        }

        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                if (value.Equals(_fontSize)) return;
                _fontSize = value;
                OnPropertyChanged();
            }
        }
    }
}
