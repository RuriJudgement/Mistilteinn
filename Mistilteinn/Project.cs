using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Mistilteinn.Helper;
using Mistilteinn.Unit;
using Mistilteinn.ViewModels;
using Newtonsoft.Json;

namespace Mistilteinn
{
    public class Project : ViewModelBase
    {
        private string _textPath;
        private string _fileExtension;
        private string _backgroundPath;
        private string _characterPath;
        private string _facePath;
        private string _voicePath;
        private string _musicPath;
        private string _nameTablePath;
        private int _fileIndex;
        private int _textIndex;
        private string _version;
        private bool _isVoiceEnable;
        private bool _isMusicEnable;
        private bool _isPreviewEnable;
        private bool _isMute = false;
        private bool _isMusicMute;
        private bool _isVoiceMute;
        private int _musicVolume = 80;
        private int _voiceVolume = 100;

        public static Project CreateProject()
        {
            ProjectPath = null;
            return Current = new Project()
            {
                Version = "2.7"
            };
        }
        public static String ProjectPath { get; private set; }
        public static Project Current { get; private set; }
        public static Project LoadProject(String file)
        {
            if (!File.Exists(file))
            {
                MessageDialog.Show("项目文件不存在!", "找不到文件", Char.ConvertFromUtf32(0xe886));
                return null;
            }

            Project result = null;
            try
            {
                result = JsonConvert.DeserializeObject<Project>(File.ReadAllText(file));
            }
            catch (JsonException jsonException)
            {

                MessageDialog.Show($"该文件不是正确的 Mis v2 文件，请确定文件是第二版 Mis 项目文件。\r\nMesssage: {jsonException.Message}", "非法的格式", Char.ConvertFromUtf32(0xe886));
                return null;
            }

            if (File.Exists(result.NameTablePath))
            {
                NameTableUnit.LoadNameTable(result.NameTablePath);
            }

            ProjectPath = file;

            if (result.Version == "2.0")
            {
                result.Version = "2.3";
                result.IsMusicMute = false;
                result.IsVoiceMute = false;
            }
            if (result.Version == "2.3")
            {
                result.Version = "2.7";
                result.MusicVolume = 80;
                result.VoiceVolume = 100;
            }
            
            return Current = result;
        }
        public static Project OpenProject(Window owner)
        {
            var projectFile = DialogHelper.ShowOpenFileDialog(owner, "打开 Mis 项目", "Mis 项目文件(*.mis)|*.mis|所有文件(*.*)|*.*");

            if (projectFile != null)
            {
                return LoadProject(projectFile);
            }
            else
            {
                return null;
            }
        }
        public static void SaveProject(bool isSaveAs = false)
        {
            if (Current == null)
            {
                MessageDialog.Show("请先打开一个项目!", "项目为空", Char.ConvertFromUtf32(0xe886));
                return;
            }

            if (String.IsNullOrEmpty(ProjectPath) || isSaveAs)
            {
                SaveFileDialog fd = new SaveFileDialog()
                {
                    DefaultExt = "*.mis",
                    AddExtension = true,
                    Filter = "Mis 项目文件(*.mis)|*.mis|所有文件(*.*)|*.*",
                    Title = "保存 Mis 项目"
                };

                if ((bool)fd.ShowDialog(Application.Current.MainWindow))
                {
                    Current.Save(fd.FileName);
                }
            }
            else
            {
                Current.Save(ProjectPath);
            }
        }

        public static void SaveProject(String path)
        {
            Current.Save(path);
        }

        public String TextPath
        {
            get { return _textPath; }
            set
            {
                if (value == _textPath) return;
                _textPath = value;
                OnPropertyChanged();
            }
        }
        public String FileExtension
        {
            get { return _fileExtension; }
            set
            {
                if (value == _fileExtension) return;
                _fileExtension = value;
                OnPropertyChanged();
            }
        }
        public String BackgroundPath
        {
            get { return _backgroundPath; }
            set
            {
                if (value == _backgroundPath) return;
                _backgroundPath = value;
                OnPropertyChanged();
            }
        }
        public String CharacterPath
        {
            get { return _characterPath; }
            set
            {
                if (value == _characterPath) return;
                _characterPath = value;
                OnPropertyChanged();
            }
        }
        public String FacePath
        {
            get { return _facePath; }
            set
            {
                if (value == _facePath) return;
                _facePath = value;
                OnPropertyChanged();
            }
        }
        public String VoicePath
        {
            get { return _voicePath; }
            set
            {
                if (value == _voicePath) return;
                _voicePath = value;
                OnPropertyChanged();
            }
        }
        public String MusicPath
        {
            get { return _musicPath; }
            set
            {
                if (value == _musicPath) return;
                _musicPath = value;
                OnPropertyChanged();
            }
        }
        public String NameTablePath
        {
            get { return _nameTablePath; }
            set
            {
                if (value == _nameTablePath) return;
                _nameTablePath = value;
                OnPropertyChanged();
            }
        }
        public int FileIndex
        {
            get { return _fileIndex; }
            set
            {
                if (value == _fileIndex) return;
                _fileIndex = value;
                OnPropertyChanged();
            }
        }
        public int TextIndex
        {
            get { return _textIndex; }
            set
            {
                if (value == _textIndex) return;
                _textIndex = value;
                OnPropertyChanged();
            }
        }
        public String Version
        {
            get { return _version; }
            set
            {
                if (value == _version) return;
                _version = value;
                OnPropertyChanged();
            }
        }
        public bool IsVoiceEnable
        {
            get { return _isVoiceEnable; }
            set
            {
                if (value == _isVoiceEnable) return;
                _isVoiceEnable = value;
                OnPropertyChanged();
            }
        }
        public bool IsMusicEnable
        {
            get { return _isMusicEnable; }
            set
            {
                if (value == _isMusicEnable) return;
                _isMusicEnable = value;
                OnPropertyChanged();
            }
        }
        public bool IsPreviewEnable
        {
            get { return _isPreviewEnable; }
            set
            {
                if (value == _isPreviewEnable) return;
                _isPreviewEnable = value;
                OnPropertyChanged();
            }
        }
        public bool IsMute
        {
            get { return _isMute; }
            set
            {
                if (value == _isMute) return;
                _isMute = value;
                OnPropertyChanged();
            }
        }
        public bool IsMusicMute
        {
            get { return _isMusicMute; }
            set
            {
                if (value == _isMusicMute) return;
                _isMusicMute = value;
                OnPropertyChanged();
            }
        }
        public bool IsVoiceMute
        {
            get { return _isVoiceMute; }
            set
            {
                if (value == _isVoiceMute) return;
                _isVoiceMute = value;
                OnPropertyChanged();
            }
        }
        public int MusicVolume
        {
            get { return _musicVolume; }
            set
            {
                if (value == _musicVolume) return;
                _musicVolume = value;
                OnPropertyChanged();
                SoundUnit.MusicVolume = value;
            }
        }
        public int VoiceVolume
        {
            get { return _voiceVolume; }
            set
            {
                if (value == _voiceVolume) return;
                _voiceVolume = value;
                OnPropertyChanged();
                SoundUnit.VoiceVolume = value;
            }
        }

        public void Save(String path)
        {
            File.WriteAllText(path, ToString(), Encoding.UTF8);

            if (MainWindowViewModel.Current != null)
            {
                foreach (var textFile in MainWindowViewModel.Current.TextFiles)
                {
                    textFile.Save();
                }
            }
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
