using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UdpDriver.Api;
using UdpDriver.Controls;
using UdpDriver.UdpCommands;

namespace UdpDriver.Drivers
{
    public class UdpKeyboard : UdpDriverBase
    {
        //public override int Port => 8001;
        public override int Port => SocketHelp.GetPort();

        public override string DriverName => "Keyboard";
        public void Dispose()
        {
        }

        public override bool ExecuteControlCommand(CommandData Command)
        {
            bool IsLocal=Command.PackParent == null;
            var cmd = Command.ReadCommand() as KeyboardCommand;
            if (IsPrintEvent&&IsLocal?IsLocalPrintEvent:true)
            {
                if (cmd.IsDown)
                {
                    KeyboardControlU.Instance.KeyDown(cmd.Key);
                    WinApi.KeyDown(cmd.Key);
                }
                else
                {
                    KeyboardControlU.Instance.KeyUp(cmd.Key);
                    WinApi.KeyUp(cmd.Key);
                }
            }
            return base.ExecuteControlCommand(Command);
        }
    }
}
