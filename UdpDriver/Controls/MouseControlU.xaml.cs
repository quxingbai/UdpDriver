using System;
using System.Collections.Generic;
using System.IO;
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

namespace UdpDriver.Controls
{
    /// <summary>
    /// MouseControlU.xaml 的交互逻辑
    /// </summary>
    public partial class MouseControlU : UserControl
    {

        public static MouseControlU Instance=null;

        private Point _MousePoint { get; set; } = new Point();
        public Point MousePoint { get => _MousePoint; set { _MousePoint = value; OnMousePointChanged(value); } }

        public bool NeedInitSize { get; set; } = true;
        public event Action<MouseControlU,Point> MousePointChanged;
        public event Action<MouseControlU,Point,MouseButton,MouseButtonState> MouseButtonEvent;
        public MouseControlU()
        {
            InitializeComponent();
            Instance = this;
        }


        private void IMAGE_Display_MouseMove(object sender, MouseEventArgs e)
        {
            MousePoint = e.GetPosition(BD_Display);
            e.Handled = true;
        }

        private void IMAGE_Display_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MousePoint = e.GetPosition(BD_Display);
            OnMouseButtonEvent(MousePoint, e.ChangedButton, e.ButtonState);
            e.Handled = true;
        }

        private void IMAGE_Display_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MousePoint = e.GetPosition(BD_Display);
            OnMouseButtonEvent(MousePoint, e.ChangedButton, e.ButtonState);
            e.Handled = true;
        }
        protected virtual void OnMousePointChanged(Point p)
        {
            MousePointChanged?.Invoke(this, p);
        }
        protected virtual void OnMouseButtonEvent(Point p,MouseButton b,MouseButtonState s)
        {
            MouseButtonEvent?.Invoke(this,p, b, s);
        }
    }
}
