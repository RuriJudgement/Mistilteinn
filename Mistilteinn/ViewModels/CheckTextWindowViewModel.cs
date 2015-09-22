using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mistilteinn.ViewModels
{
    public class CheckTextWindowViewModel : ViewModelBase
    {
        private ObservableCollection<TextCheckResult> _checkTextResults = new ObservableCollection<TextCheckResult>();

        public ObservableCollection<TextCheckResult> CheckTextResults
        {
            get { return _checkTextResults; }
            set
            {
                if (Equals(value, _checkTextResults)) return;
                _checkTextResults = value;
                OnPropertyChanged();
            }
        }
    }
}
