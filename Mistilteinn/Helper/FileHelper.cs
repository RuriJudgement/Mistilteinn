using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mistilteinn.Helper
{
    public static class FileHelper
    {
        public static String FindFile(String baseFolder, String fileName)
        {
            baseFolder = baseFolder.Replace('/', '\\');
            if (baseFolder.Last() != '\\')
            {
                baseFolder += '\\';
            }

            var path = Path.Combine(baseFolder, fileName);
            path = path.Replace('/', '\\');

            var folder = Path.GetDirectoryName(fileName);
            var file = Path.GetFileName(path);

            return Directory.GetFiles(Path.GetDirectoryName(path))
                .FirstOrDefault(f => f.ToLower().Contains(Path.GetFileName(path).ToLower()));
        }
    }
}
