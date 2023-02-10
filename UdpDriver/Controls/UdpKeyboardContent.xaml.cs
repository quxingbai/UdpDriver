using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
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
using static UdpDriver.UdpCommands.KeyboardCommand;

namespace UdpDriver.Controls
{
    /// <summary>
    /// UdpKeyboardContent.xaml 的交互逻辑
    /// </summary>
    public partial class UdpKeyboardContent : UserControl
    {
        public UdpKeyboard UdpKeyboard { get; private set; } = new UdpKeyboard();
        public UdpKeyboardContent()
        {
            UdpKeyboard.IsLocalPrintEvent = false;
            InitializeComponent();
            TEXT_Title.Text = UdpKeyboard.DriverName + " " + " 本机IP " + UdpKeyboard.IP.ToString();
        }

        private void KeyStateChanged(Keys key, bool IsDown)
        {
            UdpKeyboard.ExecuteControlCommand(new UdpCommands.CommandData(new KeyboardCommand()
            {
                IsDown = IsDown,
                Key = key,
            }));
        }

        private void KeyboardControlU_PKeyDown(KeyboardControlU arg1, UdpCommands.KeyboardCommand.Keys arg2)
        {
            KeyStateChanged(arg2, true);
        }

        private void KeyboardControlU_PKeyUp(KeyboardControlU arg1, UdpCommands.KeyboardCommand.Keys arg2)
        {
            KeyStateChanged(arg2, false);
        }

        private void MENUITEM_PRINTEVENT_Click(object sender, RoutedEventArgs e)
        {
            UdpKeyboard.IsPrintEvent = !UdpKeyboard.IsPrintEvent;
            ((MenuItem)sender).Header = "输出事件->"+UdpKeyboard.IsPrintEvent.ToString();
        }

        private void BT_Link_Click(object sender, RoutedEventArgs e)
        {
            string ipt = TEXT_Link.Text;
            Task.Run(() =>
            {
                try
                {

                    IPEndPoint ip = IPEndPoint.Parse(ipt);
                    if (UdpKeyboard.LinkingTargets.Contains(ip))
                    {
                        MessageBox.Show("目标已连接");
                    }
                    var result = UdpKeyboard.LinkTargetDriver(ip);
                    result.Wait();
                    var data = result.Result;
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
        }
    }
}
