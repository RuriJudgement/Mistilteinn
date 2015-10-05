using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Mistilteinn.Texts;

namespace Mistilteinn.ViewModels
{
    public class CheckTextWindowViewModel : ViewModelBase
    {
        public CheckTextWindowViewModel()
        {
            _hasComment = Project.Current.TextCheckFilterHasComment;
            _errorImportance = Project.Current.TextCheckFilterErrorImportance;
            _textCheckResultType = Project.Current.TextCheckFilterResultType;
            UpdateCheckTextResults();
        }

        private ObservableCollection<Text> _checkTextResults = new ObservableCollection<Text>();
        private bool _hasComment;
        private ErrorImportance _errorImportance;
        private TextCheckResultType _textCheckResultType;

        public ObservableCollection<Text> CheckTextResults
        {
            get { return _checkTextResults; }
            set
            {
                if (Equals(value, _checkTextResults)) return;
                _checkTextResults = value;
                OnPropertyChanged();
            }
        }

        async void UpdateCheckTextResults()
        {
            TextFile[] files = null;

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var vm = Application.Current.MainWindow.DataContext as MainWindowViewModel;
                files = vm.TextFiles.ToArray();
            });

            await Task.Run(() =>
            {

                List<Text> buffer = new List<Text>(1024);

                foreach (var textFile in files)
                {
                    foreach (var text in textFile.Texts)
                    {
                        if ((text.CheckResult.Importance & ErrorImportance) != ErrorImportance.Normal)
                        {
                            buffer.Add(text);
                            continue;
                        }
                        if ((text.CheckResult.Result & TextCheckResultType) != TextCheckResultType.Pass)
                        {
                            buffer.Add(text);
                            continue;
                        }
                        if (HasComment && !String.IsNullOrEmpty(text.Comment))
                        {
                            buffer.Add(text);
                        }
                    }
                }

                CheckTextResults = new ObservableCollection<Text>(buffer);
            });
        }

        public bool HasComment
        {
            get { return _hasComment; }
            set
            {
                if (value == _hasComment) return;
                _hasComment = value;
                OnPropertyChanged();
                Project.Current.TextCheckFilterHasComment = value;
                UpdateCheckTextResults();
            }
        }

        public ErrorImportance ErrorImportance
        {
            get { return _errorImportance; }
            set
            {
                if (value == _errorImportance) return;
                _errorImportance = value;
                OnPropertyChanged();
                Project.Current.TextCheckFilterErrorImportance = value;
                UpdateCheckTextResults();
            }
        }

        public bool IsInfomationChecked
        {
            get { return ErrorImportance.HasFlag(ErrorImportance.Infomation); }
            set
            {
                ErrorImportance ^= ErrorImportance.Infomation;
                OnPropertyChanged();
            }
        }

        public bool IsWarningChecked
        {
            get { return ErrorImportance.HasFlag(ErrorImportance.Warnning); }
            set
            {
                ErrorImportance ^= ErrorImportance.Warnning;
                OnPropertyChanged();
            }
        }

        public bool IsErrorChecked
        {
            get { return ErrorImportance.HasFlag(ErrorImportance.Error); }
            set
            {
                ErrorImportance ^= ErrorImportance.Error;
                OnPropertyChanged();
            }
        }

        public TextCheckResultType TextCheckResultType
        {
            get { return _textCheckResultType; }
            set
            {
                if (value == _textCheckResultType) return;
                _textCheckResultType = value;
                OnPropertyChanged();
                Project.Current.TextCheckFilterResultType = value;
                UpdateCheckTextResults();
            }
        }
        
        public bool IsNotTranslatedChecked
        {
            get { return TextCheckResultType.HasFlag(TextCheckResultType.NotTranslated); }
            set
            {
                TextCheckResultType ^= TextCheckResultType.NotTranslated;
                OnPropertyChanged();
            }
        }


        public bool IsHalfWidthCharacterChecked
        {
            get { return TextCheckResultType.HasFlag(TextCheckResultType.HalfWidthCharacter); }
            set
            {
                TextCheckResultType ^= TextCheckResultType.HalfWidthCharacter;
                OnPropertyChanged();
            }
        }

        public bool IsPhoneticMarkerChecked
        {
            get { return TextCheckResultType.HasFlag(TextCheckResultType.PhoneticMarker); }
            set
            {
                TextCheckResultType ^= TextCheckResultType.PhoneticMarker;
                OnPropertyChanged();
            }
        }

        public bool IsLineNotMatchChecked
        {
            get { return TextCheckResultType.HasFlag(TextCheckResultType.LineNotMatch); }
            set
            {
                TextCheckResultType ^= TextCheckResultType.LineNotMatch;
                OnPropertyChanged();
            }
        }

        public bool IsFormatNotMatchChecked
        {
            get { return TextCheckResultType.HasFlag(TextCheckResultType.FormatNotMatch); }
            set
            {
                TextCheckResultType ^= TextCheckResultType.FormatNotMatch;
                OnPropertyChanged();
            }
        }

        public bool IsNounNotReplacedChecked
        {
            get { return TextCheckResultType.HasFlag(TextCheckResultType.NounNotReplaced); }
            set
            {
                TextCheckResultType ^= TextCheckResultType.NounNotReplaced;
                OnPropertyChanged();
            }
        }

        public bool IsJapaneseChecked
        {
            get { return TextCheckResultType.HasFlag(TextCheckResultType.Japanese); }
            set
            {
                TextCheckResultType ^= TextCheckResultType.Japanese;
                OnPropertyChanged();
            }
        }

        public bool IsTraditionalChineseChecked
        {
            get { return TextCheckResultType.HasFlag(TextCheckResultType.TraditionalChinese); }
            set
            {
                TextCheckResultType ^= TextCheckResultType.TraditionalChinese;
                OnPropertyChanged();
            }
        }

        public bool IsTooShortChecked
        {
            get { return TextCheckResultType.HasFlag(TextCheckResultType.TooShort); }
            set
            {
                TextCheckResultType ^= TextCheckResultType.TooShort;
                OnPropertyChanged();
            }
        }

        public bool IsPunctuationsNotMatchChecked
        {
            get { return TextCheckResultType.HasFlag(TextCheckResultType.PunctuationsNotMatch); }
            set
            {
                TextCheckResultType ^= TextCheckResultType.PunctuationsNotMatch;
                OnPropertyChanged();
            }
        }
    }
}
