using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mistilteinn.Controls
{
    interface ITextEditer
    {
        event EventHandler<KeyEventArgs> KeyPressed;
        void AddText(String text);
    }
}
