using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UdpDriver.UdpCommands;

namespace UdpDriver.Drivers
{
    internal interface IUdpDriver : IDisposable
    {
        public bool IsReceiveCommand { get; set; }
        public int Port { get; }
        public String DriverName { get; }
        public IPEndPoint IP { get; }
        public bool IsPrintEvent { get; set; }
        public CommandSocket Socket { get; }
        public event Action<UdpDriverBase, UdpPack> ReceviedPack;
        public event Action<UdpDriverBase, CommandData> ExecutedControlCommand;
        public bool ExecuteControlCommand(CommandData Command);
        public bool SendCommand(IPEndPoint TargetIP, CommandData Command);
    }
}
