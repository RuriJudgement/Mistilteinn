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
using Microsoft.VisualBasic;
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
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new TextCheckFilterSelectWindow()
            {
                Owner = this,
                DataContext = DataContext
            }.Show();
        }

        private void DataGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;

            double width = e.NewSize.Width - dataGrid.Columns[0].ActualWidth;
            width = width / (dataGrid.Columns.Count - 1);

            for (int i = 1; i < dataGrid.Columns.Count; i++)
            {
                dataGrid.Columns[i].Width = width - 9;
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            Text text = dataGrid.SelectedItem as Text;

            if (text != null)
            {
                (Application.Current.MainWindow as MainWindow).SetTextIndex(text.FileIndex - 1, text.TextIndex - 1);
            }
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

            if (NameTableUnit.NameTable.Where(n => text.OriginalText.ToLower().Contains(n.Key.ToLower())).Any(n => text.TranslatedText.ToLower().Contains(n.Key.ToLower()) && n.Key != n.Value.TranslatedText && !String.IsNullOrEmpty(n.Value.TranslatedText.Trim("　。？！…「」『』，、".ToCharArray()))))
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

            if (Microsoft.VisualBasic.Strings.StrConv(text.TranslatedText, VbStrConv.SimplifiedChinese) !=
                text.TranslatedText)
            {
                result.Result |= TextCheckResultType.TraditionalChinese;
                result.Importance |= ErrorImportance.Warnning;

            }
            else
            {
                var matches = Regex.Matches(text.TranslatedText, @"[\u4e00-\u9fa5]+");
                foreach (Match chinese in matches)
                {
                    if (chinese.Success)
                    {
                        Encoding gb = Encoding.GetEncoding("gb2312");
                        foreach (var ch in chinese.Value)
                        {
                            var bytes = gb.GetBytes(ch.ToString());

                            if (bytes.Length == 2)
                            {
                                if (bytes[0] >= 0xB0
                                    && bytes[0] <= 0xF7
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
                }
            }
            
            TraditionalChinese:;

            int okLenght = Math.Max(5, (int)Math.Round(text.OriginalText.Length * 0.45));

            if (text.OriginalText.Length - text.TranslatedText.Length > okLenght)
            {
                result.Result |= TextCheckResultType.TooShort;
                result.Importance |= ErrorImportance.Infomation;
            }

            if (text.OriginalText.Count(c => "　。？！…「」『』".Contains(c)) != text.TranslatedText.Count(c => "　。？！…「」『』".Contains(c)))
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
                if (Importance.HasFlag(ErrorImportance.Error))
                {
                    return new SolidColorBrush(Color.FromRgb(0xFF, 0x17, 0x44));
                }
                if (Importance.HasFlag(ErrorImportance.Warnning))
                {
                    return new SolidColorBrush(Color.FromRgb(0xFF, 0x94, 0x00));
                }
                if (Importance.HasFlag(ErrorImportance.Infomation))
                {
                    return new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50));
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

                    if (Importance.HasFlag(ErrorImportance.Error))
                    {
                        result.AppendLine("错误");
                    }
                    if (Importance.HasFlag(ErrorImportance.Warnning))
                    {
                        result.AppendLine("警告");
                    }
                    if (Importance.HasFlag(ErrorImportance.Infomation))
                    {
                        result.AppendLine("提示");
                    }
                    if (Importance == ErrorImportance.Normal)
                    {
                        result.AppendLine("一般");
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
        Warnning = 0x2,
        Error = 0x4
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
