using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mistilteinn.ViewModels;

namespace Mistilteinn.Texts
{
    public class NameTableItem : ViewModelBase
    {
        public NameTableItem(String originalText, String translatedText)
        {
            OriginalText = originalText;
            TranslatedText = translatedText;
        }

        private int _index;
        private String _originalText;
        private String _translatedText;

        public int Index
        {
            get { return _index; }
            set
            {
                if (_index == value) return;
                _index = value;
                OnPropertyChanged();
            }
        }

        public String TranslatedText
        {
            get { return _translatedText; }
            set
            {
                if (_translatedText == value) return;
                _translatedText = value;
                OnPropertyChanged();
            }
        }

        public string OriginalText
        {
            get { return _originalText; }
            set
            {
                if (_originalText == value) return;
                _originalText = value;
                OnPropertyChanged();
            }
        }
    }
}
