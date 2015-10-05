using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mistilteinn.Commands
{
    public static class MenuCommands
    {
        public static readonly RoutedUICommand FileCommand = new RoutedUICommand("文件(_F)", "FileCommand", typeof (MenuCommands));

        public static readonly RoutedUICommand NewProjectCommand = new RoutedUICommand("新建(_N)", "NewProjectCommand", typeof(MenuCommands),
            new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.N, ModifierKeys.Control) }));

        public static readonly RoutedUICommand OpenProjectCommand = new RoutedUICommand("打开(_O)", "OpenProjectCommand", typeof(MenuCommands),
            new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.O, ModifierKeys.Control) }));
        
        public static readonly RoutedUICommand SaveProjectCommand = new RoutedUICommand("保存(_S)", "SaveProjectCommand", typeof(MenuCommands),
            new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.S, ModifierKeys.Control) }));

        public static readonly RoutedUICommand SaveAsProjectCommand = new RoutedUICommand("另存为...(_A)", "SaveProjectCommand", typeof(MenuCommands),
            new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.A, ModifierKeys.Control) }));
        
        public static readonly RoutedUICommand ExitCommand = new RoutedUICommand("退出(_X)", "ExitCommand", typeof(MenuCommands));

        public static readonly RoutedUICommand EditCommand = new RoutedUICommand("编辑(_E)", "EditCommand", typeof (MenuCommands));

        public static readonly RoutedUICommand EditInfoCommand = new RoutedUICommand("编辑脚本头(_S)", "EditInfoCommand", typeof(MenuCommands),
            new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.I, ModifierKeys.Control) }));

        public static readonly RoutedUICommand EditRawCommand = new RoutedUICommand("编辑原脚本(_E)", "EditRawCommand", typeof(MenuCommands),
            new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.E, ModifierKeys.Control) }));

        public static readonly RoutedUICommand FindCommand = new RoutedUICommand("查找(_F)", "EditCommand", typeof (MenuCommands));

        public static readonly RoutedUICommand FastFindCommand = new RoutedUICommand("快速查找(_F)", "FastFindCommand", typeof(MenuCommands),
            new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F, ModifierKeys.Control) }));

        public static readonly RoutedUICommand FindInOriginalCommand = new RoutedUICommand("查找原文(_O)", "FindInOriginalCommand", typeof(MenuCommands), 
            new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Shift) }));

        public static readonly RoutedUICommand FindInTranslatedCommand = new RoutedUICommand("查找译文(_T)", "FindInTranslatedCommand", typeof(MenuCommands),
            new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.T, ModifierKeys.Control | ModifierKeys.Shift) }));

        public static readonly RoutedUICommand NameTableCommand = new RoutedUICommand("名词表(_T)", "NameTableCommand",
            typeof(MenuCommands), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.T, ModifierKeys.Control) }));

        public static readonly RoutedUICommand ProjectSettingCommand = new RoutedUICommand("项目设定(_S)", "ProjectSettingCommand",
            typeof(MenuCommands), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.P, ModifierKeys.Control) }));

        public static readonly RoutedUICommand SoundCommand = new RoutedUICommand("声音(_S)", "SoundCommand", typeof(MenuCommands));

        public static readonly RoutedUICommand MuteCommand = new RoutedUICommand("静音(_M)", "MuteCommand",
            typeof(MenuCommands), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.M, ModifierKeys.Control) }));

        public static readonly RoutedUICommand CloseMusicCommand = new RoutedUICommand("关闭BGM(_B)", "MuteCommand",
            typeof(MenuCommands), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.M, ModifierKeys.Control | ModifierKeys.Shift) }));

        public static readonly RoutedUICommand CloseVoiceCommand = new RoutedUICommand("关闭语音(_V)", "CloseVoiceCommand",
            typeof(MenuCommands), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.V, ModifierKeys.Control | ModifierKeys.Shift) }));

        public static readonly RoutedUICommand ReplayVoiceCommand = new RoutedUICommand("重播语音(_R)", "ReplayVoiceCommand",
            typeof(MenuCommands), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F5) }));

        public static readonly RoutedUICommand ViewCheckCommand = new RoutedUICommand("查看检查结果(_V)", "ViewCheckCommand",
            typeof(MenuCommands), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.C, ModifierKeys.Control | ModifierKeys.Shift) }));

        public static readonly RoutedUICommand PreviousInfomationTextCommand = new RoutedUICommand("上一个提示(_I)", "PreviousInfomationTextCommand",
            typeof(MenuCommands), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F3, ModifierKeys.Control | ModifierKeys.Shift) }));

        public static readonly RoutedUICommand PreviousWarnningTextCommand = new RoutedUICommand("上一个警告(_W)", "PreviousWarnningTextCommand",
            typeof(MenuCommands), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F3, ModifierKeys.Control) }));

        public static readonly RoutedUICommand PreviousErrorTextCommand = new RoutedUICommand("上一个错误(_E)", "PreviousErrorTextCommand",
            typeof(MenuCommands), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F3) }));

        public static readonly RoutedUICommand NextInfomationTextCommand = new RoutedUICommand("下一个提示(_I)", "NextInfomationTextCommand",
            typeof(MenuCommands), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F4, ModifierKeys.Control | ModifierKeys.Shift) }));

        public static readonly RoutedUICommand NextWarnningTextCommand = new RoutedUICommand("下一个警告(_W)", "NextWarnningTextCommand",
            typeof(MenuCommands), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F4, ModifierKeys.Control) }));

        public static readonly RoutedUICommand NextErrorTextCommand = new RoutedUICommand("下一个错误(_E)", "NextErrorTextCommand",
            typeof(MenuCommands), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F4) }));

        public static readonly RoutedUICommand NextCommentTextCommand = new RoutedUICommand("下一个注释(_C)", "NextCommentTextCommand",
            typeof(MenuCommands), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F4, ModifierKeys.Alt) }));

        public static readonly RoutedUICommand PreviousCommentTextCommand = new RoutedUICommand("上一个注释(_C)", "PreviousCommentTextCommand",
            typeof(MenuCommands), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F3, ModifierKeys.Alt) }));

        public static readonly RoutedUICommand ClassicViewCommand = new RoutedUICommand("经典视图(_C)", "ClassicViewCommand",
            typeof(MenuCommands), new InputGestureCollection(new InputGesture[] { new KeyGesture(Key.F2) }));
    }
}
