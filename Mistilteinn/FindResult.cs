using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mistilteinn.Texts;

namespace Mistilteinn
{
    public class FindResult
    {
        public Text Text { get; set; }
        public int FileIndex { get; set; }
        public int TextIndex { get; set; }
    }
}
