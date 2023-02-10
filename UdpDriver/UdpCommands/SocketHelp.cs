using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UdpDriver.Api;

namespace UdpDriver.UdpCommands
{
    internal class SocketHelp
    {
        public class RandomNumber
        {
            private int Max { get; set; } = 9999;
            private int Min { get; set; } = 8800;
            private int? PVal { get; set; }
            public int Value { get => (PVal ?? (PVal = Random.Shared.Next(Min, Max))).Value; }
            public RandomNumber(int min, int max)
            {
                this.Max = max;
                this.Min = min;
            }
        }
        public class TempData<T>
        {
            private Func<T> Get = null;
            private bool IsNull = true;
            private T PVal = default(T);
            public T Value { get => IsNull ? (PVal = Get.Invoke()) : PVal; }
            public TempData(Func<T> Get)
            {
                this.Get = Get;
            }

        }
        public class LongIpProvider
        {
            private long _id { get; set; }
            public long NextID { get => ++_id; }
        }
        public static IPAddress GetLocalIP(AddressFamily IpFamily = AddressFamily.InterNetwork)
        {
            var ip = Dns.GetHostEntry(Dns.GetHostName());
            return ip.AddressList.Where(w => w.AddressFamily == IpFamily).Last();
        }
        public static bool IsUsingPort(int Port)
        {
            var ps=IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners();
            bool b=ps.Where(w=>w.Port== Port).Any();
            Array.Clear(ps);
            return b;
        }
        public static int GetPort(int Start=8848,int End=65535)
        {
            for(int f = Start; f <= End; f++)
            {
                if (!IsUsingPort(f))
                {
                    return f;
                }
            }
            throw new Exception("无可用端口");
        }
        public static byte[] GetScreenImageMemory(out int w,out int h)
        {
            w = WinApi.GetSystemMetrics(0);
            h = WinApi.GetSystemMetrics(1);
            Bitmap bmp = new Bitmap(w, h);
            Graphics g =Graphics.FromImage(bmp);
            g.CopyFromScreen(0,0,0,0,new Size(w,h));
            g.Dispose();
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms,System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }
    }
}
