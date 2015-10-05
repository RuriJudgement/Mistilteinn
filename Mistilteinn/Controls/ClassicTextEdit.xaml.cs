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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mistilteinn.Texts;

namespace Mistilteinn.Controls
{
    /// <summary>
    /// ClassicTextEdit.xaml 的交互逻辑
    /// </summary>
    public partial class ClassicTextEdit : UserControl, ITextEditer
    {
        public ClassicTextEdit()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                    e.Handled = true;
                    NameBoard.Focus();
                    NameBoard.SelectionStart = NameBoard.Text.Length;
                    break;
                case Key.Enter:
                    if (e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
                    {
                        e.Handled = true;
                        NameBoard.Focus();
                        TranslatedText.Focus();
                        KeyPressed?.Invoke(this, e);
                        SelectNeedTranslate();
                    }
                    break;
                case Key.Up:
                case Key.Down:
                case Key.Left:
                case Key.Right:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    {
                        e.Handled = true;
                        KeyPressed?.Invoke(this, e);
                        SelectNeedTranslate();
                    }
                    break;
                case Key.D1:
                case Key.NumPad1:
                case Key.D2:
                case Key.NumPad2:
                case Key.D3:
                case Key.NumPad3:
                case Key.D4:
                case Key.NumPad4:
                case Key.D5:
                case Key.NumPad5:
                case Key.D6:
                case Key.NumPad6:
                case Key.D7:
                case Key.NumPad7:
                case Key.D8:
                case Key.NumPad8:
                case Key.D9:
                case Key.NumPad9:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    {
                        e.Handled = true;
                        KeyPressed?.Invoke(this, e);
                    }
                    break;
            }
        }

        public event EventHandler<KeyEventArgs> KeyPressed;
        public void AddText(string text)
        {

            TranslatedText.SelectedText = text;
            TranslatedText.SelectionStart += TranslatedText.SelectionLength;
            TranslatedText.SelectionLength = 0;
        }

        void SelectNeedTranslate()
        {
            int startCount, endCount;
            var trimChars = "　。？！…「」『』".ToCharArray();

            startCount = TranslatedText.Text.Length - TranslatedText.Text.TrimStart(trimChars).Length;
            endCount = TranslatedText.Text.Length - TranslatedText.Text.TrimEnd(trimChars).Length;

            TranslatedText.SelectionStart = startCount;
            TranslatedText.SelectionLength = (TranslatedText.Text.Length - startCount - endCount) < 0 ? 0 : (TranslatedText.Text.Length - startCount - endCount);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof (Text), typeof (ClassicTextEdit), new PropertyMetadata(default(Text)));

        public Text Text
        {
            get { return (Text) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}
