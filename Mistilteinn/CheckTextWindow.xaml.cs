using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Mistilteinn.Texts;
using Mistilteinn.Unit;
using Mistilteinn.ViewModels;
using Newtonsoft.Json;

namespace Mistilteinn
{
    /// <summary>
    /// CheckTextWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CheckTextWindow : Window
    {
        public CheckTextWindow()
        {
            InitializeComponent();
            this.DataContext = new CheckTextWindowViewModel();
        }

        private bool isChecking = false;

        private async void StartCheck_Click(object sender, RoutedEventArgs e)
        {
            if (isChecking)
            {
                return;
            }
            textBlock.Text = "检查";
            (this.Resources["BeginCheck"] as Storyboard).Begin();

            var vm = (DataContext as CheckTextWindowViewModel);



            List<TextFile> files = null;
            List<TextCheckResult> buffer = new List<TextCheckResult>(1024);



            await Application.Current.MainWindow.Dispatcher.InvokeAsync(() =>
            {
                files = new List<TextFile>((Application.Current.MainWindow.DataContext as MainWindowViewModel).TextFiles);
            });


            await Task.Run(async () =>
            {
                for (int index = 0; index < files.Count; index++)
                {
                    var textFile = files[index];
                    for (int i = 0; i < textFile.Texts.Count; i++)
                    {
                        var text = textFile.Texts[i];

                        var checkResult = TextChecker.Check(text);
                        if (checkResult.Result != TextCheckResultType.Pass)
                        {
                            checkResult.Index = buffer.Count + 1;
                            buffer.Add(checkResult);
                        }
                    }
                }

                await Dispatcher.InvokeAsync(() =>
                {
                    vm.CheckTextResults = new ObservableCollection<TextCheckResult>(buffer);
                    (this.Resources["EndCheck"] as Storyboard).Begin();
                    textBlock.Text = "开始";
                });
            });
        }

        private async void Storyboard_Completed(object sender, EventArgs e)
        {
        }

    }

    public static class TextChecker
    {
        public static TextCheckResult Check(Text text)
        {
            TextCheckResult result = new TextCheckResult()
            {
                Text = new WeakReference<Text>(text),
            };

            if (text.CheckResult != null)
            {
                if (text.CheckResult.IsChecking)
                {
                    return null;
                }
            }
            else
            {
                text.CheckResult = result;
            }

            text.CheckResult.IsChecking = true;

            if (String.IsNullOrEmpty(text.TranslatedText))
            {
                result.Result |= TextCheckResultType.NotTranslated;
                result.Importance |= ErrorImportance.Error;
            }

            if (text.TranslatedText.Any(c => c < 0xFF))
            {
                result.Result |= TextCheckResultType.HalfWidthCharacter;
                result.Importance |= ErrorImportance.Infomation;
            }

            var phoneticMarker = Regex.Match(text.TranslatedText, @"\[rb,[\s\S]*,[\s\S]*\]");
            if (phoneticMarker.Success)
            {
                result.Result |= TextCheckResultType.PhoneticMarker;
                result.Importance |= ErrorImportance.Infomation;
            }

            if (text.OriginalText.Split("\r\n".ToCharArray(), StringSplitOptions.None).Length !=
                text.TranslatedText.Split("\r\n".ToCharArray(), StringSplitOptions.None).Length)
            {
                result.Result |= TextCheckResultType.LineNotMatch;
                result.Importance |= ErrorImportance.Warnning;
            }

            var oStartChars = text.OriginalText.Substring(0,
                text.OriginalText.Length - text.OriginalText.TrimStart("　。？！…「」『』".ToCharArray()).Length);
            var tStartChars = text.TranslatedText.Substring(0,
                text.TranslatedText.Length - text.TranslatedText.TrimStart("　。？！…「」『』".ToCharArray()).Length);
            if (oStartChars != tStartChars)
            {
                result.Result |= TextCheckResultType.FormatNotMatch;
                result.Importance |= ErrorImportance.Warnning;
            }
            else
            {
                var oEndChars =
                    text.OriginalText.Substring(text.OriginalText.TrimEnd("　。？！…「」『』".ToCharArray()).Length,
                        text.OriginalText.Length - text.OriginalText.TrimEnd("　。？！…「」『』".ToCharArray()).Length);
                var tEndChars =
                    text.TranslatedText.Substring(text.TranslatedText.TrimEnd("　。？！…「」『』".ToCharArray()).Length,
                        text.TranslatedText.Length - text.TranslatedText.TrimEnd("　。？！…「」『』".ToCharArray()).Length);
                if (oEndChars != tEndChars)
                {
                    result.Result |= TextCheckResultType.FormatNotMatch;
                    result.Importance |= ErrorImportance.Warnning;
                }
            }

            if (NameTableUnit.NameTable.Any(n => text.TranslatedText.ToLower().Contains(n.Key.ToLower())))
            {
                result.Result |= TextCheckResultType.NounNotReplaced;
                result.Importance |= ErrorImportance.Infomation;
            }

            var japanese = Regex.Match(text.TranslatedText, @"[\u3040-\u30FF\u31F0-\u31FF]+");
            if (japanese.Success)
            {
                result.Result |= TextCheckResultType.Japanese;
                result.Importance |= ErrorImportance.Infomation;
            }
            else
            {
                var chinese = Regex.Match(text.TranslatedText, @"[\u4e00-\u9fa5]*");

                if (chinese.Success)
                {
                    Encoding gb = Encoding.GetEncoding("gb2312");
                    foreach (Group @group in chinese.Groups)
                    {
                        foreach (var ch in @group.Value)
                        {
                            var bytes = gb.GetBytes(ch.ToString());

                            if (bytes.Length == 2)
                            {
                                if (bytes[0] >= 0xB0
                                    && bytes[1] <= 0xF7
                                    && bytes[1] >= 0xA1
                                    && bytes[1] <= 0xFE)
                                {

                                }
                                else
                                {
                                    result.Result |= TextCheckResultType.TraditionalChinese;
                                    result.Importance |= ErrorImportance.Warnning;
                                    goto TraditionalChinese;
                                }
                            }
                        }
                    }

                    TraditionalChinese:;
                }
            }

            int okLenght = Math.Max(5, (int)Math.Round(text.OriginalText.Length * 0.3));

            if (text.OriginalText.Length - text.TranslatedText.Length > okLenght)
            {
                result.Result |= TextCheckResultType.TooShort;
                result.Importance |= ErrorImportance.Infomation;
            }
            
            if (text.OriginalText.Count(c=> "　。？！…「」『』".Contains(c)) != text.TranslatedText.Count(c => "　。？！…「」『』".Contains(c)))
            {
                result.Result |= TextCheckResultType.PunctuationsNotMatch;
                result.Importance |= ErrorImportance.Infomation;
            }

            text.CheckResult.IsChecking = false;
            if (text.CheckResult != result)
            {
                text.CheckResult.Importance = result.Importance;
                text.CheckResult.Result = result.Result;
            }
            return text.CheckResult;
        }
    }


    public class TextCheckResult : ViewModelBase
    {
        private bool _isChecking = true;
        private int _index;
        private WeakReference<Text> _text;
        private TextCheckResultType _result;
        private ErrorImportance _importance;
        private bool _isSelected;

        public bool IsChecking
        {
            get { return _isChecking; }
            set
            {
                if (value == _isChecking) return;
                _isChecking = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public int Index
        {
            get { return _index; }
            set
            {
                if (value == _index) return;
                _index = value;
                OnPropertyChanged();
            }
        }

        public WeakReference<Text> Text
        {
            get { return _text; }
            set
            {
                if (Equals(value, _text)) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        public TextCheckResultType Result
        {
            get { return _result; }
            set
            {
                if (value == _result) return;
                _result = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CheckResultInfo));
            }
        }

        public ErrorImportance Importance
        {
            get { return _importance; }
            set
            {
                if (value == _importance) return;
                _importance = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProgressBarForeground));
                OnPropertyChanged(nameof(CheckResultInfo));
            }
        }
       
        public SolidColorBrush ProgressBarForeground
        {
            get
            {
                switch (Importance)
                {
                    case ErrorImportance.Infomation:
                        return new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50));
                    case ErrorImportance.Warnning:
                        return new SolidColorBrush(Color.FromRgb(0xFF, 0x94, 0x00));
                    case ErrorImportance.Error:
                        return new SolidColorBrush(Color.FromRgb(0xFF, 0x17, 0x44));
                }
                return Application.Current.Resources["PrimaryHueDarkBrush"] as SolidColorBrush;
            }
        }
        public String CheckResultInfo
        {
            get
            {
                if (Result == TextCheckResultType.Pass)
                {
                    return "通过文本检查.";
                }
                else
                {
                    var result = new StringBuilder(512);
                    int index = 1;
                    result.Append("检查结果:");
                    switch (Importance)
                    {
                        case ErrorImportance.Normal:
                            result.AppendLine("一般");
                            break;
                        case ErrorImportance.Infomation:
                            result.AppendLine("提示");
                            break;
                        case ErrorImportance.Warnning:
                            result.AppendLine("警告");
                            break;
                        case ErrorImportance.Error:
                            result.AppendLine("错误");
                            break;
                    }

                    if (Result.HasFlag(TextCheckResultType.NotTranslated))
                    {
                        result.AppendLine($"{index++:00}.未提供翻译.");
                    }
                    if (Result.HasFlag(TextCheckResultType.HalfWidthCharacter))
                    {
                        result.AppendLine($"{index++:00}.包含半角字符.");
                    }
                    if (Result.HasFlag(TextCheckResultType.PhoneticMarker))
                    {
                        result.AppendLine($"{index++:00}.包含注音标记,请确保注音标记格式正确.");
                    }
                    if (Result.HasFlag(TextCheckResultType.LineNotMatch))
                    {
                        result.AppendLine($"{index++:00}.行数不匹配,请与原文保证相同的函数.");
                    }
                    if (Result.HasFlag(TextCheckResultType.FormatNotMatch))
                    {
                        result.AppendLine($"{index++:00}.格式和原文不匹配,前后标点与原文不匹配.");
                    }
                    if (Result.HasFlag(TextCheckResultType.NounNotReplaced))
                    {
                        result.AppendLine($"{index++:00}.含有未替换的名词表项目,请替换名词表项文本.");
                    }
                    if (Result.HasFlag(TextCheckResultType.Japanese))
                    {
                        result.AppendLine($"{index++:00}.包含日语,请确保日语是必须的.");
                    }
                    if (Result.HasFlag(TextCheckResultType.TraditionalChinese))
                    {
                        result.AppendLine($"{index++:00}.包含非简体汉字,请确保非简体汉字是必须的.");
                    }
                    if (Result.HasFlag(TextCheckResultType.TooShort))
                    {
                        result.AppendLine($"{index++:00}.译文太短，请确保已经完成翻译.");
                    }
                    if (Result.HasFlag(TextCheckResultType.PunctuationsNotMatch))
                    {
                        result.AppendLine($"{index++:00}.标点不匹配,标点数目不一致,请确保已经完成翻译.");
                    }

                    return result.ToString().Trim();
                }
            }
        }
    }

    [Flags]
    public enum ErrorImportance
    {
        Normal = 0,
        Infomation = 0x1,
        Warnning = 0x3,
        Error = 0x7

    }

    [Flags]
    public enum TextCheckResultType
    {
        /// <summary>
        /// 通过
        /// </summary>
        Pass = 0,
        /// <summary>
        /// 未提供翻译
        /// </summary>
        NotTranslated = 0x1,
        /// <summary>
        /// 包含半角字符
        /// </summary>
        HalfWidthCharacter = 0x2,
        /// <summary>
        /// 注音标记
        /// </summary>
        PhoneticMarker = 0x4,
        /// <summary>
        /// 行数不匹配
        /// </summary>
        LineNotMatch = 0x8,
        /// <summary>
        /// 格式和原文不匹配
        /// </summary>
        FormatNotMatch = 0x10,
        /// <summary>
        /// 含有未替换的名词表项目
        /// </summary>
        NounNotReplaced = 0x20,
        /// <summary>
        /// 包含日语
        /// </summary>
        Japanese = 0x40,
        /// <summary>
        /// 包含非简体汉字
        /// </summary>
        TraditionalChinese = 0x80,
        /// <summary>
        /// 文本长度太短
        /// </summary>
        TooShort = 0x100,
        /// <summary>
        /// 标点不匹配
        /// </summary>
        PunctuationsNotMatch = 0x200
    }
}
