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
using Mistilteinn.Helper;
using Mistilteinn.Texts;
using Mistilteinn.Unit;
using VisualTreeHelper = Mistilteinn.Helper.VisualTreeHelper;

namespace Mistilteinn.Controls
{
    public class GamePreview : Control
    {
        static GamePreview()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GamePreview), new FrameworkPropertyMetadata(typeof(GamePreview)));
        }

        public GamePreview()
        {
            this.Loaded += GamePreview_Loaded;
        }

        private TextBox _nameBoard, _content;

        public void AddText(String text)
        {
            _content.SelectedText = text;
            _content.SelectionStart += _content.SelectionLength;
            _content.SelectionLength = 0;
        }

        private void GamePreview_Loaded(object sender, RoutedEventArgs e)
        {
            _nameBoard = VisualTreeHelper.FindVisualElementFormName(this, "NameBoard") as TextBox;
            _content = VisualTreeHelper.FindVisualElementFormName(this, "Content") as TextBox;

            _content.PreviewKeyDown += _content_PreviewKeyDown;
            _nameBoard.PreviewKeyDown += _nameBoard_PreviewKeyDown;
        }

        private void _nameBoard_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                case Key.Enter:
                    e.Handled = true;
                    _content.Focus();
                    _content.SelectionStart = _content.Text.Length;
                    break;
            }
        }

        private void _content_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                    e.Handled = true;
                    _nameBoard.Focus();
                    _nameBoard.SelectionStart = _nameBoard.Text.Length;
                    break;
                case Key.Enter:
                    if (e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
                    {
                        e.Handled = true;
                        _nameBoard.Focus();
                        _content.Focus();
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

        public static readonly DependencyProperty GameBackgroundProperty = DependencyProperty.Register(
            "GameBackground", typeof(ImageSource), typeof(GamePreview), new PropertyMetadata(default(ImageSource)));

        public ImageSource GameBackground
        {
            get { return (ImageSource)GetValue(GameBackgroundProperty); }
            set { SetValue(GameBackgroundProperty, value); }
        }

        public static readonly DependencyProperty FaceImageProperty = DependencyProperty.Register(
            "FaceImage", typeof(ImageSource), typeof(GamePreview), new PropertyMetadata(default(ImageSource)));

        public ImageSource FaceImage
        {
            get { return (ImageSource)GetValue(FaceImageProperty); }
            set { SetValue(FaceImageProperty, value); }
        }

        public static readonly DependencyProperty NameBoardProperty = DependencyProperty.Register(
            "NameBoard", typeof(String), typeof(GamePreview), new PropertyMetadata(default(String)));

        public String NameBoard
        {
            get { return (String)GetValue(NameBoardProperty); }
            set { SetValue(NameBoardProperty, value); }
        }

        public static readonly DependencyProperty GameTextProperty = DependencyProperty.Register(
            "GameText", typeof(Text), typeof(GamePreview), new PropertyMetadata(default(Text), (o, args) =>
            {
                if (!Project.Current.IsPreviewEnable)
                {
                    return;
                }
                if (args.NewValue != args.OldValue)
                {
                    if ((args.NewValue as Text) != null)
                    {
                        ((Text)args.NewValue).TextChanged += (o as GamePreview).GamePreview_TextChanged;
                    }
                    if ((args.OldValue as Text) != null)
                    {
                        ((Text)args.OldValue).TextChanged += (o as GamePreview).GamePreview_TextChanged;
                    }
                }

                (o as GamePreview).UpdateText(args.NewValue != args.OldValue);

            }));

        private void GamePreview_TextChanged(object sender, EventArgs e)
        {
            if (!Project.Current.IsPreviewEnable)
            {
                return;
            }
            UpdateText(false);
        }

        public Text GameText
        {
            get { return (Text)GetValue(GameTextProperty); }
            set { SetValue(GameTextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(String), typeof(GamePreview), new PropertyMetadata(default(String), (o, args) =>
          {
              if ((o as GamePreview).GameText.OriginalText != args.NewValue.ToString())
              {
                  (o as GamePreview).GameText.TranslatedText = args.NewValue.ToString();
              }
          }));

        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        async void UpdateText(bool updateImage)
        {
            if (GameText != null)
            {
                if (String.IsNullOrEmpty(GameText.TranslatedText.Trim("　「」".ToCharArray())))
                {
                    Text = GameText.OriginalText;
                }
                else
                {
                    Text = GameText.TranslatedText;
                }
                if (updateImage)
                {
                    GameBackground = await GraphicalUnit.ReadScript(GameText);
                    if (!String.IsNullOrEmpty(GameText?.Face?.Path))
                    {
                        FaceImage = new BitmapImage(new Uri(FileHelper.FindFile(Project.Current.FacePath, GameText.Face.Path)));
                    }
                    else
                    {
                        //if (GameText?.Voice?.Path != null)
                        //{
                        //    var chara =
                        //        GameText?.Characters.FirstOrDefault(
                        //            c => c.Path.ToLower().Contains(GameText?.Voice?.Path.ToLower().Replace("fro", "flo").Split('_')[1].TrimEnd("0123456789".ToCharArray())));
                        //    if (chara?.Path != null)
                        //    {
                        //        var face = chara?.Path.Split('_');
                        //        face[1] = "f";
                        //        var file = FileHelper.FindFile(Project.Current.FacePath, String.Join("_", face));
                        //        if (file != null)
                        //        {
                        //            FaceImage = new BitmapImage(new Uri(file));
                        //            return;
                        //        }
                        //    }
                        //}
                        FaceImage = null;
                    }
                }
            }
        }

        public event EventHandler<KeyEventArgs> KeyPressed;

        void SelectNeedTranslate()
        {
            int startCount, endCount;
            var trimChars = "　。？！…「」『』".ToCharArray();

            startCount = _content.Text.Length - _content.Text.TrimStart(trimChars).Length;
            endCount = _content.Text.Length - _content.Text.TrimEnd(trimChars).Length;

            _content.SelectionStart = startCount;
            _content.SelectionLength = _content.Text.Length - startCount - endCount;
        }
    }
}
