﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UdpDriver.UdpCommands
{
    public interface ICommand
    {
        public enum CommandType
        {
            MouseMove,MouseButton,Keyboard, Get
        }
        public CommandType Type { get; }
    }
    public interface IControlCommand
    {

    }
    public interface IDataCommand
    {

    }
    public abstract class Command : ICommand
    {
        public  abstract ICommand.CommandType Type { get;  }
    }
    public class MouseMoveCommand: Command, IControlCommand
    {
        public enum Direction
        {
            Up,Left,Down,Right
        }
        public bool IsAbs { get; set; }
        public Point Move { get; set; }

        public override ICommand.CommandType Type =>  ICommand.CommandType.MouseMove;
    }
    public class MouseButtonCommand: Command, IControlCommand
    {
        public MouseButtonState State { get; set; }
        public MouseButton Button { get; set; }
        public override ICommand.CommandType Type =>  ICommand.CommandType.MouseButton;
    }
    public class KeyboardCommand: Command, IControlCommand
    {
        public enum Keys
        {
            KeyCode = 0xFFFF,
            Modifiers = -65536,
            None = 0x0,
            LButton = 0x1,
            RButton = 0x2,
            Cancel = 0x3,
            MButton = 0x4,
            XButton1 = 0x5,
            XButton2 = 0x6,
            Back = 0x8,
            Tab = 0x9,
            LineFeed = 0xA,
            Clear = 0xC,
            Return = 0xD,
            Enter = 0xD,
            ShiftKey = 0x10,
            ControlKey = 0x11,
            Menu = 0x12,
            Pause = 0x13,
            Capital = 0x14,
            CapsLock = 0x14,
            KanaMode = 0x15,
            HanguelMode = 0x15,
            HangulMode = 0x15,
            JunjaMode = 0x17,
            FinalMode = 0x18,
            HanjaMode = 0x19,
            KanjiMode = 0x19,
            Escape = 0x1B,
            IMEConvert = 0x1C,
            IMENonconvert = 0x1D,
            IMEAccept = 0x1E,
            IMEAceept = 0x1E,
            IMEModeChange = 0x1F,
            Space = 0x20,
            Prior = 0x21,
            PageUp = 0x21,
            Next = 0x22,
            PageDown = 0x22,
            End = 0x23,
            Home = 0x24,
            Left = 0x25,
            Up = 0x26,
            Right = 0x27,
            Down = 0x28,
            Select = 0x29,
            Print = 0x2A,
            Execute = 0x2B,
            Snapshot = 0x2C,
            PrintScreen = 0x2C,
            Insert = 0x2D,
            Delete = 0x2E,
            Help = 0x2F,
            D0 = 0x30,
            D1 = 0x31,
            D2 = 0x32,
            D3 = 0x33,
            D4 = 0x34,
            D5 = 0x35,
            D6 = 0x36,
            D7 = 0x37,
            D8 = 0x38,
            D9 = 0x39,
            A = 0x41,
            B = 0x42,
            C = 0x43,
            D = 0x44,
            E = 0x45,
            F = 0x46,
            G = 0x47,
            H = 0x48,
            I = 0x49,
            J = 0x4A,
            K = 0x4B,
            L = 0x4C,
            M = 0x4D,
            N = 0x4E,
            O = 0x4F,
            P = 0x50,
            Q = 0x51,
            R = 0x52,
            S = 0x53,
            T = 0x54,
            U = 0x55,
            V = 0x56,
            W = 0x57,
            X = 0x58,
            Y = 0x59,
            Z = 0x5A,
            LWin = 0x5B,
            RWin = 0x5C,
            Apps = 0x5D,
            Sleep = 0x5F,
            NumPad0 = 0x60,
            NumPad1 = 0x61,
            NumPad2 = 0x62,
            NumPad3 = 0x63,
            NumPad4 = 0x64,
            NumPad5 = 0x65,
            NumPad6 = 0x66,
            NumPad7 = 0x67,
            NumPad8 = 0x68,
            NumPad9 = 0x69,
            Multiply = 0x6A,
            Add = 0x6B,
            Separator = 0x6C,
            Subtract = 0x6D,
            Decimal = 0x6E,
            Divide = 0x6F,
            F1 = 0x70,
            F2 = 0x71,
            F3 = 0x72,
            F4 = 0x73,
            F5 = 0x74,
            F6 = 0x75,
            F7 = 0x76,
            F8 = 0x77,
            F9 = 0x78,
            F10 = 0x79,
            F11 = 0x7A,
            F12 = 0x7B,
            F13 = 0x7C,
            F14 = 0x7D,
            F15 = 0x7E,
            F16 = 0x7F,
            F17 = 0x80,
            F18 = 0x81,
            F19 = 0x82,
            F20 = 0x83,
            F21 = 0x84,
            F22 = 0x85,
            F23 = 0x86,
            F24 = 0x87,
            NumLock = 0x90,
            Scroll = 0x91,
            LShiftKey = 0xA0,
            RShiftKey = 0xA1,
            LControlKey = 0xA2,
            RControlKey = 0xA3,
            LMenu = 0xA4,
            RMenu = 0xA5,
            BrowserBack = 0xA6,
            BrowserForward = 0xA7,
            BrowserRefresh = 0xA8,
            BrowserStop = 0xA9,
            BrowserSearch = 0xAA,
            BrowserFavorites = 0xAB,
            BrowserHome = 0xAC,
            VolumeMute = 0xAD,
            VolumeDown = 0xAE,
            VolumeUp = 0xAF,
            MediaNextTrack = 0xB0,
            MediaPreviousTrack = 0xB1,
            MediaStop = 0xB2,
            MediaPlayPause = 0xB3,
            LaunchMail = 0xB4,
            SelectMedia = 0xB5,
            LaunchApplication1 = 0xB6,
            LaunchApplication2 = 0xB7,
            OemSemicolon = 0xBA,
            Oem1 = 0xBA,
            Oemplus = 0xBB,
            Oemcomma = 0xBC,
            OemMinus = 0xBD,
            OemPeriod = 0xBE,
            OemQuestion = 0xBF,
            Oem2 = 0xBF,
            Oemtilde = 0xC0,
            Oem3 = 0xC0,
            OemOpenBrackets = 0xDB,
            Oem4 = 0xDB,
            OemPipe = 0xDC,
            Oem5 = 0xDC,
            OemCloseBrackets = 0xDD,
            Oem6 = 0xDD,
            OemQuotes = 0xDE,
            Oem7 = 0xDE,
            Oem8 = 0xDF,
            OemBackslash = 0xE2,
            Oem102 = 0xE2,
            ProcessKey = 0xE5,
            Packet = 0xE7,
            Attn = 0xF6,
            Crsel = 0xF7,
            Exsel = 0xF8,
            EraseEof = 0xF9,
            Play = 0xFA,
            Zoom = 0xFB,
            NoName = 0xFC,
            Pa1 = 0xFD,
            OemClear = 0xFE,
            Shift = 0x10000,
            Control = 0x20000,
            Alt = 0x40000
        }
        public bool IsDown { get; set; }
        public Keys Key { get; set; }
        public override ICommand.CommandType Type =>  ICommand.CommandType.Keyboard;
    }
    public class GetCommand : Command, IDataCommand
    {
        public class GetResult
        {

        }
        public class DriverInfoResult:GetResult
        {
            public String Name { get; set; }
            public bool HasPassword { get; set; }
            public bool CanLink { get; set; }
        }
        public class LinkResult : GetResult
        {
            public bool Result { get; set; }
            public String Message { get; set; }
        }
        public class ScreenImageResult:GetResult
        {
            public Byte[] ImageMemory { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }
        public class PCInfoResult : GetResult
        {
            public int ScreenWidth { get; set; }
            public int ScreenHeight { get; set; }
            public Point MousePos { get; set; }
        }
        private static int _NextID { get; set; } = 0;
        private static int NextID { get => _NextID += 1; }
        public enum GetDataType
        {
            DriverInfo,//获取对方的设备信息
            TryLink,//尝试连接
            ScreenImage,//获取对方屏幕图像
            PCInfo,//获取对方电脑信息
        }
        public enum MsgType
        {
            Send,Result
        }
        public GetDataType Get { get; set; }
        public MsgType MessageType { get; set; }
        public String ID { get; set; } = NextID.ToString();
        public String Result { get; set; }//返回信息
        public String[] Params { get; set; }//附加信息
        public GetResult ReadResult()
        {
            return 
                this.Get == GetDataType.DriverInfo ? JsonConvert.DeserializeObject<DriverInfoResult>(Result):
                this.Get == GetDataType.TryLink ? JsonConvert.DeserializeObject<LinkResult>(Result):
                this.Get == GetDataType.ScreenImage ? JsonConvert.DeserializeObject<ScreenImageResult>(Result):
                this.Get == GetDataType.PCInfo ? JsonConvert.DeserializeObject<PCInfoResult>(Result):
                null;
        }
        public override ICommand.CommandType Type => ICommand.CommandType.Get;

    }
    public class LinkControlCommand
    {
        public String DriverName { get; set; }
    }
}
