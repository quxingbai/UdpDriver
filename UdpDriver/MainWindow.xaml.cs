using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UdpDriver.Drivers;
using UdpDriver.UdpCommands;

namespace UdpDriver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            KB.UdpKeyboard.ReceviedPack += UdpKeyboard_ReceviedPack;
            MS.UdpMouse.ReceviedPack += UdpKeyboard_ReceviedPack;
            KB.UdpKeyboard.Sended+=UdpMouse_Sended;
            MS.UdpMouse.Sended += UdpMouse_Sended; 
        }

        private void UdpMouse_Sended(UdpDriverBase arg1, IPEndPoint arg2, CommandData arg3)
        {
            Dispatcher.Invoke(() => {
                if (LIST.Items.Count > 20) LIST.Items.Clear();
                LIST.Items.Insert(0, "Send\t"+arg2.ToString()+"\t"+arg3.ReadCommand().Type+"\t"+arg3.Command);
            });
        }

        private void UdpKeyboard_ReceviedPack(UdpDriverBase arg1, UdpPack arg2)
        {
            Dispatcher.Invoke(() => {
                if (LIST.Items.Count > 20) LIST.Items.Clear();
                LIST.Items.Insert(0,"Receive\t"+ arg1.DriverName+"\t"+arg2.From + "\t" + arg2.Size + "\t" + arg2.Data.ReadCommand().Type+"\t|\t"+arg2.Data.Command);
            });
        }

    }
}
