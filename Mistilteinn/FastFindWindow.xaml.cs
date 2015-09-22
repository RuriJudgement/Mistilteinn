using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Mistilteinn.ViewModels;

namespace Mistilteinn
{
    /// <summary>
    /// FastFindWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FastFindWindow : Window
    {
        public static FastFindWindow Current { get; private set; }

        public FastFindWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            Current = this;
            base.OnInitialized(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            Current = null;
            base.OnClosed(e);
        }

        private int preFileIndex = -1, preTextIndex = -1;
        
        private void FindNext_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(SearchText.Text))
            {
                MessageDialog.Show(this, "查找文本为空,请输入查找内容.", "空的查找文本", Char.ConvertFromUtf32(0xe886));
                return;
            }

            var searchText = (IsCaseSensitive.IsChecked ?? false) ? SearchText.Text : SearchText.Text.ToLower();
            var nameText = (IsCaseSensitive.IsChecked ?? false) ? NameText.Text : NameText.Text.ToLower();

            var main = Owner as MainWindow;
            var mainVm = Owner.DataContext as MainWindowViewModel;
            if (main != null && mainVm != null)
            {
                int fi, ti;
                main.GetTextIndex(out fi, out ti);
                if (fi == preFileIndex && ti == preTextIndex)
                {
                    ti++;
                }

                RESEARCH:

                for (int i = fi; i < mainVm.TextFiles.Count; i++)
                {
                    var file = mainVm.TextFiles[i];

                    for (int j = ti; j < file.Texts.Count; j++)
                    {
                        var text = file.Texts[j];

                        if (!String.IsNullOrEmpty(NameText.Text))
                        {
                            if (String.IsNullOrEmpty(text.NameBoard))
                            {
                                continue;
                            }
                            var nameBoard = (IsCaseSensitive.IsChecked ?? false)
                                ? text.NameBoard
                                : text.NameBoard.ToLower();

                            if (!nameBoard.Contains(nameText))
                            {
                                continue;
                            }
                        }

                        var originalText = (IsCaseSensitive.IsChecked ?? false)
                                ? text.OriginalText
                                : text.OriginalText.ToLower();
                        var translatedText = (IsCaseSensitive.IsChecked ?? false)
                                ? text.TranslatedText
                                : text.TranslatedText.ToLower();

                        if (originalText.Contains(searchText) || translatedText.Contains(searchText))
                        {
                            main.SetTextIndex(preFileIndex = i, preTextIndex = j);
                            return;
                        }
                    }
                }

                if (MessageDialog.Show(this, "本次搜索已经到文本结尾，是否要从头开始搜索?", "搜索完毕", Char.ConvertFromUtf32(0xe84f),
                        MessageDialogButtons.YesNo) == MessageDialogResultButton.Yes)
                {
                    fi = 0;
                    ti = 0;
                    goto RESEARCH;
                }
            }
        }
    }
}
