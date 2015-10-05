using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mistilteinn.Tools.ViewModels
{
    public class UnhandledExceptionWindowViewModel : ViewModelBase
    {
        public UnhandledExceptionWindowViewModel(String info)
        {
            Infomation = Uri.UnescapeDataString(info);
        }

        private String _infomation;

        public String Infomation
        {
            get { return _infomation; }
            set
            {
                if (_infomation == value) return;
                _infomation = value;
                OnPropertyChanged();
            }
        }
    }
}
