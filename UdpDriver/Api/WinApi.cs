using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static UdpDriver.UdpCommands.KeyboardCommand;

namespace UdpDriver.Api
{
    internal class WinApi
    {
        [DllImport("User32.Dll")]
        public static extern bool mouse_event(int mouse_state, int x, int y, int data, int info);
        [DllImport("User32.Dll")]
        public static extern bool keybd_event(Keys vk, byte scan, int flags, int info);
        [DllImport("User32.Dll")]
        public static extern bool keybd_event(int vk, byte scan, int flags, int info);
        [DllImport("User32.Dll")]
        public static extern int GetSystemMetrics(int id);
        [DllImport("User32.Dll")]
        public static extern bool GetCursorPos(ref Point point);

        public static void MouseLeftDown(int x, int y)
        {
            mouse_event(0x0002, x, y, 0, 0);
        }
        public static void MouseRightDown(int x, int y)
        {
            mouse_event(0x0008, x, y, 0, 0);
        }
        public static void MouseMiddleDown(int x, int y)
        {
            mouse_event(0x0020, x, y, 0, 0);
        }
        public static void MouseLeftUp(int x, int y)
        {
            mouse_event(0x0004, x, y, 0, 0);
        }
        public static void MouseRightUp(int x, int y)
        {
            mouse_event(0x0010, x, y, 0, 0);
        }
        public static void MouseMiddleUp(int x, int y)
        {
            mouse_event(0x0040, x, y, 0, 0);
        }
        public static void MouseWhell(int Whell, int x, int y)
        {
            mouse_event(0x0800, x, y, Whell, 0);
        }
        public static void MouseMoveAbs(int x, int y)
        {
            var w = GetSystemMetrics(0);
            var h = GetSystemMetrics(1);
            x = x * 65535 / w;
            y = y * 65535 / h;
            mouse_event(0x8000 | 0x0001, x, y, 0, 0);
        }
        public static void MouseMove(int x,int y)
        {
            mouse_event(0x0001, x, y, 0, 0);
        }
        public static void KeyDown(Keys k)
        {
            keybd_event(k, 0, 0, 0);
        }
        public static void KeyUp(Keys k)
        {
            keybd_event(k, 0, 2, 0);
        }
    }
}
