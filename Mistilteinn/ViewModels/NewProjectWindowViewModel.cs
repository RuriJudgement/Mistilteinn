using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using Mistilteinn.Unit;
using Newtonsoft.Json;

namespace Mistilteinn.ViewModels
{
    public class NewProjectWindowViewModel : ViewModelBase
    {
        public NewProjectWindowViewModel(EventWaitHandle waitHandle)
        {
            WaitHandle = waitHandle;
            TextFileEx = "*.c";
#if DEBUG
            TextFileFolder = @"E:\Localization\共通";
            VoiceFileFolder = @"E:\Localization\数据包\语音";
            MusicFileFolder = @"E:\Localization\数据包\BGM";
            CharaFileFolder = @"E:\Localization\数据包\立绘";
            FaceFileFolder = @"E:\Localization\数据包\face";
            BackgroundFileFolder = @"E:\Localization\数据包\BG";
            NameTableFile = @"E:\Development\GitHub\SoraHana\名词表.txt";
#endif

            if (File.Exists("Project.cache"))
            {
                JsonConvert.PopulateObject(File.ReadAllText("Project.cache"), this);
            }

        }

        [JsonIgnore]
        public EventWaitHandle WaitHandle { get; private set; }

        [JsonIgnore]
        public Project CreatedProject { get; private set; }

        public Project Create()
        {
            File.WriteAllText("Project.cache",JsonConvert.SerializeObject(this));
            
            Project result = Project.CreateProject();

            result.TextPath = TextFileFolder;
            result.FileExtension = TextFileEx;
            result.NameTablePath = NameTableFile;
            result.FileIndex = 0;
            result.TextIndex = 0;
            result.IsVoiceEnable = IsVoiceEnable;
            result.IsMusicEnable = IsMusicEnable;
            result.IsPreviewEnable = IsPreviewEnable;

            if (IsVoiceEnable)
            {
                result.VoicePath = VoiceFileFolder;
            }

            if (IsMusicEnable)
            {
                result.MusicPath = MusicFileFolder;
            }

            if (IsPreviewEnable)
            {
                result.CharacterPath = CharaFileFolder;
                result.FacePath = FaceFileFolder;
                result.BackgroundPath = BackgroundFileFolder;
            }

            if (File.Exists(result.NameTablePath))
            {
                NameTableUnit.LoadNameTable(result.NameTablePath);
            }

            return CreatedProject = result;
        }

        private String _textFileFolder;

        public String TextFileFolder
        {
            get { return _textFileFolder; }
            set
            {
                if (_textFileFolder == value) return;
                _textFileFolder = value;
                OnPropertyChanged();
            }
        }

        private String _nameTableFile;

        public String NameTableFile
        {
            get { return _nameTableFile; }
            set
            {
                if (_nameTableFile == value) return;
                _nameTableFile = value;
                OnPropertyChanged();
            }
        }

        private String _voiceFileFolder;

        public String VoiceFileFolder
        {
            get { return _voiceFileFolder; }
            set
            {
                if (_voiceFileFolder == value) return;
                _voiceFileFolder = value;
                OnPropertyChanged();
            }
        }

        private String _musicFileFolder;

        public String MusicFileFolder
        {
            get { return _musicFileFolder; }
            set
            {
                if (_musicFileFolder == value) return;
                _musicFileFolder = value;
                OnPropertyChanged();
            }
        }

        private String _charaFileFolder;

        public String CharaFileFolder
        {
            get { return _charaFileFolder; }
            set
            {
                if (_charaFileFolder == value) return;
                _charaFileFolder = value;
                OnPropertyChanged();
            }
        }

        private String _faceFileFolder;

        public String FaceFileFolder
        {
            get { return _faceFileFolder; }
            set
            {
                if (_faceFileFolder == value) return;
                _faceFileFolder = value;
                OnPropertyChanged();
            }
        }

        private String _backgroundFileFolder;

        public String BackgroundFileFolder
        {
            get { return _backgroundFileFolder; }
            set
            {
                if (_backgroundFileFolder == value) return;
                _backgroundFileFolder = value;
                OnPropertyChanged();
            }
        }

        private String _textFileEx;

        public String TextFileEx
        {
            get { return _textFileEx; }
            set
            {
                if (_textFileEx == value) return;
                _textFileEx = value;
                OnPropertyChanged();
            }
        }

        private bool _isVoiceEnable;

        public bool IsVoiceEnable
        {
            get { return _isVoiceEnable; }
            set
            {
                if (_isVoiceEnable == value) return;
                _isVoiceEnable = value;
                OnPropertyChanged();
            }
        }

        private bool _isMusicEnable;

        public bool IsMusicEnable
        {
            get { return _isMusicEnable; }
            set
            {
                if (_isMusicEnable == value) return;
                _isMusicEnable = value;
                OnPropertyChanged();
            }
        }

        private bool _isPreviewEnable;

        public bool IsPreviewEnable
        {
            get { return _isPreviewEnable; }
            set
            {
                if (_isPreviewEnable == value) return;
                _isPreviewEnable = value;
                OnPropertyChanged();
            }
        }
    }
}
