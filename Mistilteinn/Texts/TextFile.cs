using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Mistilteinn.ViewModels;

namespace Mistilteinn.Texts
{
    public class TextFile : ViewModelBase
    {
        private TextFile()
        {
            Texts = new ObservableCollection<Text>();
            Infomations = new ObservableCollection<TextFileInfomation>();
        }

        public static TextFile FromFile(String path)
        {
            var result = new TextFile();

            var lines = File.ReadAllLines(path);
            var currentText = new Text(result);

            for (int index = 0; index < lines.Length; index++)
            {
                var line = lines[index];
                if (line.Length > 0)
                {
                    switch (line[0])
                    {
                        case '*':
                            var infomation = new TextFileInfomation(line.Substring(2, line.Length - 3).Split(','));
                            result.Infomations.Add(infomation);
                            break;
                        case '[':
                            var values = line.Trim('[', ']').Split(',');
                            if (values.Length > 0)
                            {
                                try
                                {
                                    currentText.InputTag = int.Parse(values[0]);
                                }
                                catch (Exception ex)
                                {
                                    throw new FormatException($"遇到不可读的导入标志,在{path},行{index},{line}", ex);
                                }
                                for (int i = 1; i < values.Length; i++)
                                {
                                    var value = values[i];
                                    switch (value[0])
                                    {
                                        case '$':
                                            currentText.NameBoard = value.Substring(1);
                                            break;
                                        case '%':
                                            currentText.Voice = new Voice(value.Substring(1));
                                            break;
                                        case '^':
                                            currentText.Background = new Background(value.Substring(1));
                                            break;
                                        case '@':
                                            currentText.Music = new Music(value.Substring(1));
                                            break;
                                        case '&':
                                            currentText.Characters.Add(new Character(value.Substring(1)));
                                            break;
                                        case '#':
                                            currentText.Face = new Face(value.Substring(1));
                                            break;
                                    }
                                }
                            }
                            break;
                        case '>':
                            if (String.IsNullOrEmpty(currentText.OriginalText))
                            {
                                currentText.OriginalText = line.Substring(1);
                            }
                            else
                            {
                                currentText.OriginalText += $"\r\n{line.Substring(1)}";
                            }
                            break;
                        case '<':
                            if (String.IsNullOrEmpty(currentText.TranslatedText))
                            {
                                currentText.TranslatedText = line.Substring(1);
                            }
                            else
                            {
                                currentText.TranslatedText += $"\r\n{line.Substring(1)}";
                            }
                            break;
                        case '#':
                            if (String.IsNullOrEmpty(currentText.Comment))
                            {
                                currentText.Comment = line.Substring(1);
                            }
                            else
                            {
                                currentText.Comment += $"\r\n{line.Substring(1)}";
                            }
                            break;
                        default:
                            Debug.WriteLine($"遇到未知标识符,在{path},行{index},{line}");
                            break;
                    }
                }
                else
                {
                    if (!currentText.IsEmpty)
                    {
                        currentText.Index = result.Texts.Count + 1;
                        result.Texts.Add(currentText);
                        currentText.CreateOver();
                        TextChecker.Check(currentText);
                        currentText = new Text(result);
                    }
                }
            }

            return result;
        }
        
        private ObservableCollection<Text> _texts;
        private ObservableCollection<TextFileInfomation> _infomations;

        public int FileIndex { get; set; }

        public ObservableCollection<Text> Texts
        {
            get { return _texts; }
            set
            {
                if (Equals(value, _texts)) return;
                _texts = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TextFileInfomation> Infomations
        {
            get { return _infomations; }
            set
            {
                if (Equals(value, _infomations)) return;
                _infomations = value;
                OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            return String.Join("\r\n\r\n",
                new[]
                {
                    String.Join("\r\n", Infomations.Select(s => s.ToString())),
                    String.Join("\r\n\r\n", Texts.Select(s => s.ToString()))
                });
        }

        public void Save()
        {
            File.WriteAllText(Path.Combine(Project.Current.TextPath, Infomations[0][1] + ".c"), ToString(), Encoding.UTF8);
        }
    }
}