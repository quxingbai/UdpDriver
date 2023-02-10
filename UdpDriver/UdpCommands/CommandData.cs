using Newtonsoft.Json;
using System;
using System.Net;
using static UdpDriver.UdpCommands.ICommand;

namespace UdpDriver.UdpCommands
{
    public class CommandData
    {
        public UdpPack PackParent { get; set; }
        public String DriverName { get; set; }
        public DateTime SendTime { get; set; }
        public DateTime ReceviTime { get; set; }
        private Command _Command { get; set; }
        public String Command { get; set; }
        public Object Params { get; set; }
        public Command ReadCommand()
        {
            if (_Command != null) return _Command;
            var cmd = JsonConvert.DeserializeObject<dynamic>(Command);
            CommandType type = ((CommandType)(int)cmd.Type);
            switch (type)
            {
                case CommandType.MouseMove:
                    return (_Command = JsonConvert.DeserializeObject<MouseMoveCommand>(Command));
                case CommandType.MouseButton:
                    return (_Command = JsonConvert.DeserializeObject<MouseButtonCommand>(Command));
                case CommandType.Keyboard:
                    return (_Command = JsonConvert.DeserializeObject<KeyboardCommand>(Command));
                case CommandType.Get:
                    return (_Command = JsonConvert.DeserializeObject<GetCommand>(Command));
                default:
                    throw new Exception("未定义此命令");
            }
        }
        public CommandData SetCommand(Command cmd)
        {
            Command = JsonConvert.SerializeObject(cmd);
            return this;
        }
        public CommandData() { }
        public CommandData(Command cmd)
        {
            SetCommand(cmd);
        }
    }
    public class UdpPack
    {
        public CommandData Data { get; set; }
        public EndPoint From { get; set; }
        public long Size { get; set; }
    }
}
