using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Mistilteinn.Helper
{
    public static class DialogHelper
    {
        public static String ShowFolderBrowserDialog(String title, bool showNewFolderButton = false, String selectedPath = "")
        {
            FolderBrowserDialog fbg = new FolderBrowserDialog
            {
                Description = title,
                ShowNewFolderButton = showNewFolderButton,
                SelectedPath = selectedPath
            };

            if (fbg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return fbg.SelectedPath;
            }
            else
            {
                return null;
            }
        }

        public static String ShowOpenFileDialog(Window owner,String title, String filter, bool multiselect = false)
        {
            OpenFileDialog fd = new OpenFileDialog()
            {
                Filter = filter,
                Multiselect = false,
                Title = title
            };

            if ((bool) fd.ShowDialog(owner))
            {
                return fd.FileName;
            }
            else
            {
                return null;
            }
            
        }
    }
}
