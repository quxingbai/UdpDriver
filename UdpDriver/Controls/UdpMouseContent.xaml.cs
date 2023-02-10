using System;
using System.Collections.Generic;
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

namespace UdpDriver.Controls
{
    /// <summary>
    /// UdpMouseContent.xaml 的交互逻辑
    /// </summary>
    public partial class UdpMouseContent : UserControl
    {
        public UdpMouse UdpMouse { get; private set; } = new UdpMouse();
        public bool NeedInitSize = true;
        public UdpMouseContent()
        {
            InitializeComponent();
            UdpMouse.IsLocalPrintEvent = false;
            TEXT_Title.Text = UdpMouse.DriverName + " " + " 本机IP " + UdpMouse.IP.ToString();
            UdpMouse.LinkingTargets.CollectionChanged += LinkingTargets_CollectionChanged;
            MOUSE.MousePointChanged += MOUSE_MousePointChanged;
            MOUSE.MouseButtonEvent += MOUSE_MouseButtonEvent;
        }

        private void MOUSE_MouseButtonEvent(MouseControlU arg1, Point arg2, MouseButton arg3, MouseButtonState arg4)
        {
            UdpMouse.ExecuteControlCommand(new CommandData(new MouseButtonCommand()
            {
                Button = arg3,
                State = arg4,
            }));
        }

        private void MOUSE_MousePointChanged(MouseControlU arg1, Point arg2)
        {
            UdpMouse.ExecuteControlCommand(new UdpCommands.CommandData(new MouseMoveCommand()
            {
                IsAbs = true,
                Move = new System.Drawing.Point((int)arg2.X, (int)arg2.Y),
            })); ;
        }

        private void LinkingTargets_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (NeedInitSize && UdpMouse.LinkingTargets.Any())
            {
                NeedInitSize = false;
                var display = UdpMouse.LinkingTargets.First();
                UdpMouse.GetTargetPCInfo(display).ContinueWith(w =>
                {
                    var wid = w.Result.ScreenWidth;
                    var hei = w.Result.ScreenHeight;
                    Dispatcher.Invoke(() =>
                    {
                        MOUSE.Width = wid;
                        MOUSE.Height = hei;
                        MOUSE.HorizontalAlignment = HorizontalAlignment.Left;
                    });
                });
            }
        }
        private void BT_Link_Click(object sender, RoutedEventArgs e)
        {
            string ipt = TEXT_Link.Text;
            Task.Run(() =>
            {
                try
                {
                    IPEndPoint ip = IPEndPoint.Parse(ipt);
                    if (UdpMouse.LinkingTargets.Contains(ip))
                    {
                        MessageBox.Show("目标已连接");
                    }
                    var result = UdpMouse.LinkTargetDriver(ip);
                    result.Wait();
                    var data = result.Result;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            });
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            UdpMouse.IsPrintEvent = !UdpMouse.IsPrintEvent;
            (sender as MenuItem).Header = UdpMouse.IsPrintEvent ? "输出" : "不输出";
        }
    }
}
