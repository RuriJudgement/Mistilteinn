using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Mistilteinn.ViewModels;

namespace Mistilteinn.Texts
{
    public class Background : ViewModelBase
    {
        private string _path;

        public Background(String path)
        {
            Path = path;
        }

        public String Path
        {
            get { return _path; }
            set
            {
                if (value == _path) return;
                _path = value;
                OnPropertyChanged();
            }
        }
    }
}
