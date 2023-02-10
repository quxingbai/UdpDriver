using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UdpDriver.Api;
using UdpDriver.UdpCommands;
using static UdpDriver.UdpCommands.SocketHelp;

namespace UdpDriver.Drivers
{
    public class UdpMouse : UdpDriverBase
    {
        public override int Port => SocketHelp.GetPort();
        public override string DriverName => "Mouse";

        public void Dispose()
        {
        }

        public override bool ExecuteControlCommand(CommandData Command)
        {
            bool IsLocal = Command.PackParent == null;
            if (IsPrintEvent&&IsLocal?IsLocalPrintEvent:true)
            {
                var cmd = Command.ReadCommand();
                if (cmd is MouseMoveCommand)
                {
                    var c = cmd as MouseMoveCommand;
                    if (c.IsAbs)
                    {
                        WinApi.MouseMoveAbs(c.Move.X, c.Move.Y);
                    }
                    else
                    {
                        WinApi.MouseMove(c.Move.X, c.Move.Y);
                    }
                }
                else if (cmd is MouseButtonCommand)
                {
                    var c = cmd as MouseButtonCommand;
                    var p = new Point();
                    WinApi.GetCursorPos(ref p);
                    switch (c.Button)
                    {
                        case System.Windows.Input.MouseButton.Left:
                            if (c.State == System.Windows.Input.MouseButtonState.Pressed)
                            {
                                WinApi.MouseLeftDown(p.X, p.Y);
                            }
                            else
                            {
                                WinApi.MouseLeftUp(p.X, p.Y);
                            }
                            break;
                        case System.Windows.Input.MouseButton.Middle:
                            if (c.State == System.Windows.Input.MouseButtonState.Pressed)
                            {
                                WinApi.MouseMiddleDown(p.X, p.Y);
                            }
                            else
                            {
                                WinApi.MouseMiddleUp(p.X, p.Y);
                            }
                            break;
                        case System.Windows.Input.MouseButton.Right:
                            if (c.State == System.Windows.Input.MouseButtonState.Pressed)
                            {
                                WinApi.MouseRightDown(p.X, p.Y);
                            }
                            else
                            {
                                WinApi.MouseRightUp(p.X, p.Y);
                            }
                            break;
                        default: throw new Exception("未定义此命令");
                    }
                }

            }
            return base.ExecuteControlCommand(Command);
        }
    }
}
