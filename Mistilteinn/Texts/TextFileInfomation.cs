using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mistilteinn.Texts
{
    public class TextFileInfomation : ObservableCollection<String>
    {
        public TextFileInfomation() : base()
        {
            
        }

        public TextFileInfomation(IEnumerable<String> collection) : base(collection)
        {

        }

        public TextFileInfomation(List<String> list) : base(list)
        {

        }

        public override string ToString()
        {
            return $"*[{String.Join(",", this)}]";
        }
    }
}
