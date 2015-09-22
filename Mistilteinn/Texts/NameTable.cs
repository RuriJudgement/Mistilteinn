using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mistilteinn.Texts
{
    public class NameTable : Dictionary<String, NameTableItem>
    {
        public static NameTable LoadNameTable(String path)
        {
            if (String.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return new NameTable();
            }

            var lines = File.ReadAllLines(path);
            var result = new NameTable();

            foreach (var s in lines)
            {
                var data = s.Replace(" ", "").Replace("　", "").Split('=');
                if (data.Length == 2)
                {
                    if (result.ContainsKey(data[0]))
                    {
                        result[data[0]] = new NameTableItem(data[0], data[1]);

                    }
                    else
                    {
                        result.Add(data[0], new NameTableItem(data[0], data[1]));
                    }
                }
            }

            return result;
        }
        public ObservableCollection<NameTableItem> GetActiveCollection(String text)
        {
            var result = new ObservableCollection<NameTableItem>();

            foreach (var item in this)
            {
                if (text.Contains(item.Key))
                {
                    item.Value.Index = result.Count+ 1;
                    result.Add(item.Value);
                }
            }

            return result;
        }
        public String ReplaceText(String text)
        {
            foreach (var item in this)
            {
                if (text.Contains(item.Key))
                {
                    text = text.Replace(item.Key, item.Value.TranslatedText);
                }
            }
            return text;
        }
    }
}
