using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UdpDriver.Api;
using UdpDriver.UdpCommands;
using static UdpDriver.UdpCommands.GetCommand;
using static UdpDriver.UdpCommands.SocketHelp;

namespace UdpDriver.Drivers
{
    public abstract class UdpDriverBase : IUdpDriver
    {
        public ObservableCollection<IPEndPoint> LinkingTargets = new ObservableCollection<IPEndPoint>();
        public abstract int Port { get; }
        public abstract string DriverName { get; }

        private CommandSocket _Socket = null;
        private LongIpProvider GetResultServicesNextId = new LongIpProvider();
        private Dictionary<long, Func<GetCommand, bool>> GetResultServices = new Dictionary<long, Func<GetCommand, bool>>();
        private const int MaxWaitGetMessageSecond = 15;
        private bool IsReceivingGet = false;

        public event Action<UdpDriverBase, UdpPack> ReceviedPack;
        public event Action<UdpDriverBase, CommandData> ExecutedControlCommand;
        public event Action<UdpDriverBase, GetCommand> ReceviedGetResult;
        public event Action<UdpDriverBase,IPEndPoint, CommandData> Sended;

        public CommandSocket Socket => (_Socket == null ? (_Socket = InitSocket()) : _Socket);
        public bool IsReceiveCommand { get; set; } = true;

        public IPEndPoint IP => (IPEndPoint)Socket.LocalEndPoint;
        public List<IPEndPoint> WhiteMemberList = new List<IPEndPoint>();//只有位于白名单内的IP才可以控制设备
        public Task<GetCommand.DriverInfoResult> GetTargetDriverInfoTask = null;
        public Task<GetCommand.LinkResult> LinkTargetDriverTask = null;
        public bool CanLink { get => IsReceiveCommand = true; }
        public String? Password { get; set; }
        public virtual bool IsPrintEvent { get; set; } = true;
        public virtual bool IsLocalPrintEvent { get; set; } = false;

        public void Dispose()
        {
        }
        private CommandSocket InitSocket()
        {
            var s = new CommandSocket(Port);
            s.CommandReceived += S_CommandReceived;
            return s;
        }

        private void S_CommandReceived(CommandSocket arg1, UdpPack arg2)
        {
            OnReceviedPack(arg2);
        }

        protected virtual void OnReceviedPack(UdpPack pack)
        {
            if (!IsReceiveCommand && pack.Data.ReadCommand() is IControlCommand && !WhiteMemberList.Any(ip => ip == pack.From)) return;
            var cmd = pack.Data.ReadCommand();
            ReceviedPack?.Invoke(this, pack);
            pack.Data.PackParent = pack;
            if (cmd is IControlCommand)
            {
                OnExecutedControlCommand(pack.Data);
            }
            else if (cmd is IDataCommand)
            {
                ExecuteDataCommand(pack.Data);
            }
        }
        /// <summary>
        /// Get命令的数据返回处理
        /// </summary>
        protected virtual void OnReceviedGetResult(GetCommand get)
        {
            while (IsReceivingGet) { Thread.Sleep(250); }
            IsReceiveCommand = true;
            List<long> remove = new List<long>();
            foreach (var item in GetResultServices.ToArray())
            {
                var b = item.Value.Invoke(get);
                if (!b)
                {
                    remove.Add(item.Key);
                }
            }
            foreach (var item in remove)
            {
                UnRegisterGetResultService(item);
            }
            remove.Clear();
            this.ReceviedGetResult?.Invoke(this, get);
            IsReceivingGet = false;
        }
        protected virtual void OnExecutedControlCommand(CommandData command)
        {
            if (!(command.ReadCommand() is IControlCommand))
            {
                throw new Exception("无法执行非控制命令");
            }
            ExecuteControlCommand(command);
            ExecutedControlCommand?.Invoke(this, command);
        }
        //注册一个返回值获取的服务 返回false就删除服务
        protected long RegisterGetResultService(Func<GetCommand, bool> get)
        {
            var key = GetResultServicesNextId.NextID;
            GetResultServices.Add(key, get);
            return key;
        }
        //删除返回值获取的服务
        protected bool UnRegisterGetResultService(long key)
        {
            if (!GetResultServices.ContainsKey(key)) return false;
            return GetResultServices.Remove(key);
        }

        public virtual bool ExecuteControlCommand(CommandData Command)
        {
            foreach (var i in LinkingTargets)
            {
                var d = Socket.SendCommand(i, Command);
                OnSendCommand(i, Command);
            }
            return true;
        }
        /// <summary>
        /// 处理接收的数据命令
        /// </summary>
        public virtual bool ExecuteDataCommand(CommandData Command)
        {
            var cmd = Command.ReadCommand();
            if (!(cmd is IDataCommand)) throw new Exception("无法执行非数据命令");
            if (cmd is GetCommand)
            {
                var ip = Command.PackParent.From;
                var get = cmd as GetCommand;
                //如果是别人发来的
                if (get.MessageType == GetCommand.MsgType.Send)
                {
                    get.MessageType = GetCommand.MsgType.Result;
                    switch (get.Get)
                    {
                        case GetCommand.GetDataType.DriverInfo:
                            {
                                get.Result = JsonConvert.SerializeObject(new GetCommand.DriverInfoResult()
                                {
                                    CanLink = CanLink,
                                    HasPassword = Password != null,
                                    Name = this.DriverName
                                });
                            }
                            break;
                        case GetCommand.GetDataType.TryLink:
                            {
                                bool link = true;
                                string msg = "";
                                if (link)
                                {
                                    if (!CanLink)
                                    {
                                        msg = "对方设置为不可连接"; link = false;
                                    }
                                    else if (get.Params[0] != DriverName)
                                    {
                                        msg = "设备不匹配"; link = false;
                                    }
                                }
                                if (link)
                                {
                                    WhiteMemberList.Add((IPEndPoint)ip);
                                }
                                get.Result = JsonConvert.SerializeObject(new GetCommand.LinkResult()
                                {
                                    Result = link,
                                    Message = msg
                                });
                            }
                            break;
                        case GetCommand.GetDataType.ScreenImage:
                            {
                                throw new Exception("udp的内容空间太小了所以不适用");
                                var m = GetScreenImageMemory(out int w, out int h);
                                get.Result = JsonConvert.SerializeObject(new ScreenImageResult()
                                {
                                    Width = w,
                                    Height = h,
                                    ImageMemory = new byte[0]
                                });
                            }
                            ; break;
                        case GetDataType.PCInfo:
                            {
                                System.Drawing.Point point = new System.Drawing.Point();
                                WinApi.GetCursorPos(ref point);
                                get.Result = JsonConvert.SerializeObject(new PCInfoResult()
                                {
                                    MousePos = point,
                                    ScreenHeight = WinApi.GetSystemMetrics(1),
                                    ScreenWidth = WinApi.GetSystemMetrics(0)
                                });
                            }
                            ; break;
                    }
                    return SendCommand((IPEndPoint)ip, new CommandData(get));
                }
                //如果是别人返回的
                else if (get.MessageType == GetCommand.MsgType.Result)
                {
                    OnReceviedGetResult(get);
                }
            }
            return true;
        }

        public virtual bool SendCommand(IPEndPoint TargetIP, CommandData Command)
        {
            var b =Socket.SendCommand(TargetIP, Command);
            OnSendCommand(TargetIP, Command);
            return b;
        }
        public virtual void OnSendCommand(IPEndPoint ip,CommandData d)
        {
            Sended?.Invoke(this,ip, d);
        }
        /// <summary>
        /// 获取对应IP设备的详细信息
        /// </summary>
        /// <param name="IP">设备IP</param>
        /// <returns></returns>
        public virtual Task<GetCommand.DriverInfoResult> GetTargetDriverInfo(IPEndPoint IP)
        {
            if (GetTargetDriverInfoTask != null) return GetTargetDriverInfoTask;
            GetCommand GetTargetDriverInfoCmd = new GetCommand() { Get = GetCommand.GetDataType.DriverInfo, MessageType = GetCommand.MsgType.Send };
            bool b = false;
            GetCommand.DriverInfoResult data = null;
            long service = 0;
            GetTargetDriverInfoTask = Task.Run<GetCommand.DriverInfoResult>(() =>
             {
                 int maxWaitSecond = MaxWaitGetMessageSecond;
                 bool send = false;
                 while (--maxWaitSecond != 0)
                 {
                     if (!send)
                     {
                         service = RegisterGetResultService((cmd) =>
                          {
                              var result = cmd.ReadResult();
                              if (cmd.ID == GetTargetDriverInfoCmd.ID)
                              {
                                  b = true;
                                  var r = cmd.ReadResult() as GetCommand.DriverInfoResult;
                                  data = r;
                                  return false;
                              }
                              return true;
                          });
                         SendCommand(IP, new CommandData(GetTargetDriverInfoCmd));
                         send = true;
                     }
                     if (b)
                     {
                         break;
                     }
                     Thread.Sleep(1000);
                 }
                 UnRegisterGetResultService(service);
                 return data;
             });
            return Task.Run<GetCommand.DriverInfoResult>(() =>
            {
                GetTargetDriverInfoTask.Wait();
                var a = GetTargetDriverInfoTask;
                GetTargetDriverInfoTask = null;
                return a;
            });
        }
        /// <summary>
        /// 尝试连接到某个设备
        /// </summary>
        /// <param name="IP">设备IP</param>
        /// <returns></returns>
        public virtual Task<GetCommand.LinkResult> LinkTargetDriver(IPEndPoint IP)
        {
            if (LinkTargetDriverTask != null) return LinkTargetDriverTask;
            LinkTargetDriverTask = Task.Run<GetCommand.LinkResult>(() =>
            {
                GetCommand get = new GetCommand() { Get = GetCommand.GetDataType.TryLink, MessageType = GetCommand.MsgType.Send, Params = new string[] { DriverName } };
                GetCommand.LinkResult res = null;
                int wait = MaxWaitGetMessageSecond;
                var service = RegisterGetResultService(g =>
                  {
                      if (g.ID == get.ID)
                      {
                          res = g.ReadResult() as GetCommand.LinkResult;
                          if (res.Result)
                          {
                              IPEndPoint ip = IP;
                              LinkingTargets.Add(ip);
                          }
                          return false;
                      }
                      return true;
                  });
                SendCommand(IP, new CommandData(get));
                while (--wait != 0 && res == null)
                {
                    Thread.Sleep(1000);
                }
                UnRegisterGetResultService(service);
                return res;
            });
            return Task.Run<GetCommand.LinkResult>(() =>
            {
                var a = LinkTargetDriverTask;
                a.Wait();
                LinkTargetDriverTask = null;
                return a;
            });
        }
        public virtual Task<ScreenImageResult> GetTargetScreenImage(IPEndPoint IP)
        {
            return Task.Run<ScreenImageResult>(() =>
            {
                ScreenImageResult res = null;
                var get = new GetCommand() { Get = GetDataType.ScreenImage, MessageType = MsgType.Send };
                var s = RegisterGetResultService((g) =>
                {
                    if (g.ID == get.ID)
                    {
                        res = get.ReadResult() as ScreenImageResult;
                        return false;
                    }
                    return true;
                });
                SendCommand(IP, new CommandData(get));
                int wait = MaxWaitGetMessageSecond;
                while (--wait != 0 && res == null)
                {
                    Thread.Sleep(1000);
                }
                UnRegisterGetResultService(s);
                return res;
            });
        }
        public virtual Task<PCInfoResult> GetTargetPCInfo(IPEndPoint IP)
        {
            PCInfoResult res = null;
            return Task.Run<PCInfoResult>(() =>
            {
                var get = new GetCommand() { Get = GetDataType.PCInfo, MessageType = MsgType.Send };
                WaitServiceCallBack((g) =>
                {
                    if (g.ID == get.ID)
                    {
                        res = g.ReadResult() as PCInfoResult;
                        return false;
                    }
                    return true;
                }, ()=>SendCommand(IP, new CommandData(get)), () => res == null);
                return res;
            });
        }
        public void WaitServiceCallBack(Func<GetCommand, bool> func, Action run, Func<bool> CanWhile)
        {
            var s = RegisterGetResultService(func);
            int wait = MaxWaitGetMessageSecond;
            run?.Invoke();
            while (--wait != 0 && CanWhile.Invoke())
            {
                Thread.Sleep(1000);
            }
            UnRegisterGetResultService(s);

        }
    }
}
