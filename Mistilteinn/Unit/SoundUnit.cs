using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Mistilteinn.Helper;
using Mistilteinn.Texts;

namespace Mistilteinn.Unit
{
    public static class SoundUnit
    {
        public static ZPlay VoicePlayer { get; set; }
        public static ZPlay BgmPlayer { get; set; }

        static readonly TCallbackFunc BgmCallback = MsgBgm;
        public static void Initialize()
        {
            VoicePlayer = new ZPlay();
            BgmPlayer = new ZPlay();

            VoiceVolume = Project.Current.VoiceVolume;
            MusicVolume = Project.Current.MusicVolume;

            BgmPlayer.SetCallbackFunc(BgmCallback, TCallbackMessage.MsgStopAsync, 0);
        }
        static String Bgm { get; set; }
        static String Voice { get; set; }

        private static bool _notStop;
        private static DateTime _createTime;

        public static async void ReadScript(Text text)
        {
            var bgm = text?.Music?.Path;
            var voice = text?.Voice?.Path;

            await Task.Run(() =>
            {
                if (Project.Current.IsMusicEnable && !String.IsNullOrEmpty(bgm))
                {
                    TStreamStatus status = new TStreamStatus();
                    BgmPlayer.GetStatus(ref status);

                    if (Bgm != bgm || !status.fPlay)
                    {
                        Bgm = bgm;
                        var bgmFile = FileHelper.FindFile(Project.Current.MusicPath, bgm);

                        if (!String.IsNullOrEmpty(bgmFile))
                        {
                            if (!Project.Current.IsMute && !Project.Current.IsMusicMute)
                            {
                                _notStop = true;
                                BgmPlayer.StopPlayback();
                                BgmPlayer.OpenFile(bgmFile, TStreamFormat.sfUnknown);
                                BgmPlayer.StartPlayback();
                            }
                        }
                    }
                }

                if (Project.Current.IsVoiceEnable  && !String.IsNullOrEmpty(voice))
                {
                    var voiceFile = FileHelper.FindFile(Project.Current.VoicePath, voice);
                    Voice = voiceFile;

                    if (!String.IsNullOrEmpty(voiceFile))
                    {
                        Voice = voiceFile;
                        if (!Project.Current.IsMute && !Project.Current.IsVoiceMute)
                        {
                            VoicePlayer.StopPlayback();
                            if (DateTime.Now - _createTime > TimeSpan.FromMilliseconds(500))
                            {
                                VoicePlayer.OpenFile(voiceFile, TStreamFormat.sfUnknown);
                                VoicePlayer.StartPlayback();
                            }
                            _createTime = DateTime.Now;
                        }
                    }
                }
            });
        }
        static int MsgBgm(uint objptr, int userData, TCallbackMessage msg, uint param1, uint param2)
        {
            if (_notStop)
            {
                _notStop = false;
                return 0;
            }
            switch (msg)
            {
                case TCallbackMessage.MsgStopAsync:
                    var bgmFile = FileHelper.FindFile(Project.Current.MusicPath, Bgm);

                    if (!String.IsNullOrEmpty(bgmFile))
                    {
                        //BgmPlayer.OpenFile(bgmFile, TStreamFormat.sfUnknown);
                        BgmPlayer.StartPlayback();
                    }
                    break;

            }
            return 0;
        }

        public static void ReplayVoice()
        {
            if (Project.Current.IsMute || Project.Current.IsVoiceMute)
            {
                return;
            }
            
            if (!String.IsNullOrEmpty(Voice) && File.Exists(Voice))
            {
                TStreamStatus status = new TStreamStatus();
                VoicePlayer.GetStatus(ref status);
                if (!status.fPlay)
                {
                    VoicePlayer.OpenFile(Voice, TStreamFormat.sfUnknown);
                    VoicePlayer.StartPlayback();
                }
            }
        }

        public static void SetMute()
        {
            if (!Project.Current.IsMute)
            {
                if (Project.Current.IsMusicEnable && !Project.Current.IsMusicMute && !String.IsNullOrEmpty(Bgm))
                {
                    var bgmFile = FileHelper.FindFile(Project.Current.MusicPath, Bgm);

                    TStreamStatus status = new TStreamStatus();
                    BgmPlayer.GetStatus(ref status);
                    if (!String.IsNullOrEmpty(bgmFile) && !status.fPlay)
                    {
                        _notStop = true;
                        BgmPlayer.StopPlayback();
                        BgmPlayer.OpenFile(bgmFile, TStreamFormat.sfUnknown);
                        BgmPlayer.StartPlayback();
                    }
                }
                else
                {
                    BgmPlayer.StopPlayback();
                }

                if (Project.Current.IsVoiceEnable && !Project.Current.IsVoiceMute)
                {
                    ReplayVoice();
                }
                else
                {
                    VoicePlayer.StopPlayback();
                }
            }
            else
            {
                BgmPlayer.StopPlayback();
                VoicePlayer.StopPlayback();
            }
        }

        public static int MusicVolume
        {
            get
            {
                int lVolume = 0,rVolume = 0;
                BgmPlayer.GetPlayerVolume(ref lVolume, ref rVolume);
                return lVolume;
            }

            set { BgmPlayer.SetPlayerVolume(value, value); }
        }

        public static int VoiceVolume
        {
            get
            {
                int lVolume = 0, rVolume = 0;
                VoicePlayer.GetPlayerVolume(ref lVolume, ref rVolume);
                return lVolume;
            }

            set { VoicePlayer.SetPlayerVolume(value, value); }
        }
    }
}