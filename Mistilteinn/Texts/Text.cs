using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Mistilteinn.ViewModels;

namespace Mistilteinn.Texts
{
    public class Text : ViewModelBase
    {
        private readonly WeakReference<TextFile> _textFile;

        public Text()
        {

        }

        public Text(TextFile file)
        {
            _textFile = new WeakReference<TextFile>(file);
            Characters = new ObservableCollection<Character>();
        }

        private bool _creating = true;
        private int _index;
        private ObservableCollection<Character> _characters;
        private Background _background;
        private Music _music;
        private Voice _voice;
        private int _inputTag;
        private Face _face;
        private string _nameBoard;
        private string _originalText;
        private string _translatedText;
        private string _comment;
        private TextCheckResult _checkResult;

        public void CreateOver()
        {
            _creating = false;
        }

        public int Index
        {
            get { return _index; }
            set
            {
                if (value == _index) return;
                _index = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Character> Characters
        {
            get { return _characters; }
            set
            {
                if (Equals(value, _characters)) return;
                _characters = value;
                OnPropertyChanged();
            }
        }

        public Background Background
        {
            get { return _background; }
            set
            {
                if (Equals(value, _background)) return;
                _background = value;
                OnPropertyChanged();
            }
        }

        public Music Music
        {
            get { return _music; }
            set
            {
                if (Equals(value, _music)) return;
                _music = value;
                OnPropertyChanged();
            }
        }

        public Voice Voice
        {
            get { return _voice; }
            set
            {
                if (Equals(value, _voice)) return;
                _voice = value;
                OnPropertyChanged();
            }
        }

        public int InputTag
        {
            get { return _inputTag; }
            set
            {
                if (value == _inputTag) return;
                _inputTag = value;
                OnPropertyChanged();
            }
        }

        public Face Face
        {
            get { return _face; }
            set
            {
                if (Equals(value, _face)) return;
                _face = value;
                OnPropertyChanged();
            }
        }

        public String NameBoard
        {
            get { return _nameBoard; }
            set
            {
                if (value == _nameBoard) return;
                _nameBoard = value;
                OnPropertyChanged();

                TextFile file;
                if (!_creating && _textFile.TryGetTarget(out file))
                {
                    file.Save();
                }
            }
        }

        public String OriginalText
        {
            get { return _originalText; }
            set
            {
                if (value == _originalText) return;
                _originalText = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEmpty));
            }
        }


        public String TranslatedText
        {
            get { return _translatedText; }
            set
            {
                if (value == _translatedText) return;
                _translatedText = value;
                OnPropertyChanged();

                if (OriginalText.Length > 0 && TranslatedText.Length > 0)
                {
                    if (OriginalText.First() == '　' && TranslatedText.First() != '　')
                    {
                        TranslatedText = '　' + TranslatedText;
                    }
                    if (OriginalText.First() == '「' && TranslatedText.First() != '「')
                    {
                        TranslatedText = '「' + TranslatedText;
                    }
                    if (OriginalText.Last() == '」' && TranslatedText.Last() != '」')
                    {
                        TranslatedText = TranslatedText + '」';
                    }
                }
                TextChanged?.Invoke(this, new EventArgs());

                TextFile file;
                if (!_creating && _textFile.TryGetTarget(out file))
                {
                    file.Save();
                }
                TextChecker.Check(this);
            }
        }

        public String Comment
        {
            get { return _comment; }
            set
            {
                if (value == _comment) return;
                _comment = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CommentDisplay));
            }
        }

        public String CommentDisplay
            => String.IsNullOrEmpty(Comment) ? Char.ConvertFromUtf32(0xe87f) : Char.ConvertFromUtf32(0xe87d);

        public int FileIndex
        {
            get
            {
                TextFile file;
                if (_textFile.TryGetTarget(out file))
                {
                    return file.FileIndex;
                }
                return -1;
            }
        }

        public int TextIndex => Index;
        public TextCheckResult CheckResult
        {
            get { return _checkResult; }
            set
            {
                if (Equals(value, _checkResult)) return;
                _checkResult = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler TextChanged;

        public bool IsEmpty => String.IsNullOrEmpty(OriginalText);

        public override string ToString()
        {
            String info = "";
            info += InputTag.ToString("D8");
            if (!String.IsNullOrEmpty(NameBoard))
            {
                info += $",${NameBoard}";
            }
            if (!String.IsNullOrEmpty(Voice?.Path))
            {
                info += $",%{Voice.Path}";
            }
            if (!String.IsNullOrEmpty(Background?.Path))
            {
                info += $",^{Background?.Path}";
            }
            if (!String.IsNullOrEmpty(Music?.Path))
            {
                info += $",@{Music?.Path}";
            }
            if (!String.IsNullOrEmpty(Face?.Path))
            {
                info += $",#{Face.Path}";
            }
            if ((Characters?.Count ?? 0) > 0)
            {
                info = Characters.Where(chara => !String.IsNullOrEmpty(chara.Path)).Aggregate(info, (current, chara) => current + $",&{chara.Path}");
            }

            return $"[{info}]\r\n>{OriginalText.Replace("\r\n","\r\n>")}\r\n<{TranslatedText.Replace("\r\n", "\r\n<")}\r\n#{Comment.Replace("\r\n", "\r\n#")}";
        }
    }
}
