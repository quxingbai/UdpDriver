using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpDriver.UdpCommands
{
    public class CommandSocket : Socket
    {
        private static IPAddress LocalIp = SocketHelp.GetLocalIP();
        private static long MaxListenSize = 1024 * 1024 * 20;
        private Task ReceiveTask = null;
        private bool Life = true;
        public event Action<CommandSocket, UdpPack> CommandReceived;
        public CommandSocket(int Port) : base(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            Bind(new IPEndPoint(LocalIp, Port));
            ReceiveTask = Task.Run(ReceiveThread);
        }

        private void OnCommandReceive(UdpPack Command)
        {
            this.CommandReceived?.Invoke(this, Command);
        }

        private void ReceiveThread()
        {
            byte[] Buffer = new byte[MaxListenSize];
            while (Life)
            {
                EndPoint From = new IPEndPoint(0, 0);
                int size = ReceiveFrom(Buffer, ref From);
                string data = Encoding.UTF8.GetString(Buffer, 0, size);
                var cmd = JsonConvert.DeserializeObject<CommandData>(data);
                cmd.ReceviTime = DateTime.Now;
                OnCommandReceive(new UdpPack()
                {
                    Data=cmd,
                    From=From,
                    Size=size
                });
            }
        }
        public bool SendCommand(IPEndPoint TargetIP, CommandData Command)
        {
            var str = JsonConvert.SerializeObject(Command);
            var bs = Encoding.UTF8.GetBytes(str);
            bool b = 0 != SendTo(bs, TargetIP);
            Array.Clear(bs);
            return b;
        }
        protected override void Dispose(bool disposing)
        {
            Life = false;
            ReceiveTask.Wait();
            base.Dispose(disposing);
        }
    }
}
