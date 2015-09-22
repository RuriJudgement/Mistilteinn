using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mistilteinn.Texts;

namespace Mistilteinn.Unit
{
    public static class NameTableUnit
    {
        public static NameTable NameTable { get; private set; } = new NameTable();

        public static void LoadNameTable(String path)
        {
            NameTable = NameTable.LoadNameTable(path);
        }
        
    }
}
