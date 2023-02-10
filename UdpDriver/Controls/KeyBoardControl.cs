using System;
using System.Collections.Generic;
using System.Linq;
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
using static UdpDriver.UdpCommands.KeyboardCommand;

namespace UdpDriver.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:UdpDriver.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:UdpDriver.Controls;assembly=UdpDriver.Controls"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:KeyBoardControl/>
    ///
    /// </summary>
    public class KeyBoardControl : Control
    {
        public class KeyStruct
        {
            public Keys? Key { get; set; }
            public double Width { get; set; } = 50;
            public double Height { get; set; } = 50;
            public KeyStruct(Keys? k)
            {
                this.Key = k;
            }
            public KeyStruct(Keys? k, double? w, double? h)
            {
                this.Key = k; this.Width = w ?? Width; Height = h ?? Height;
            }
        }
        private static List<List<KeyStruct>> KeySources = new List<List<KeyStruct>>();
        private static void AddSourceKey(params KeyStruct[] k)
        {
            if (KeySources.Count > 0)
            {
                KeySources.Last().AddRange(k);
            }
            else
            {
                KeySources.Add(new List<KeyStruct>());
                AddSourceKey(k);
            }
        }
        private static void AddSourceKey(params Keys?[] k)
        {
            var ks = k.Select(k1 => new KeyStruct(k1)).ToArray();
            AddSourceKey(ks);
            Array.Clear(ks);
        }
        private static void AddSourceKeyLine()
        {
            KeySources.Add(new List<KeyStruct>());
        }
        static KeyBoardControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KeyBoardControl), new FrameworkPropertyMetadata(typeof(KeyBoardControl)));
            AddSourceKey(Keys.Escape, null);
            AddSourceKey(Keys.F1, Keys.F2, Keys.F3, Keys.F4);
            AddSourceKey(new KeyStruct(null, 25, null));
            AddSourceKey(Keys.F5, Keys.F6, Keys.F7, Keys.F8);
            AddSourceKey(new KeyStruct(null, 25, null));
            AddSourceKey(Keys.F9, Keys.F10, Keys.F11, Keys.F12);
            AddSourceKeyLine();
            AddSourceKey(null, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0);
            AddSourceKeyLine();
            AddSourceKey(new KeyStruct(null, 70, null));
            AddSourceKey(Keys.Q, Keys.W, Keys.E, Keys.R, Keys.T, Keys.Y, Keys.U, Keys.I, Keys.O, Keys.P);
            AddSourceKey(new KeyStruct(Keys.Tab, 75, null));
            AddSourceKey(Keys.A, Keys.S, Keys.D, Keys.F, Keys.G, Keys.H, Keys.J, Keys.K, Keys.L);
            AddSourceKey(new KeyStruct(Keys.LShiftKey, 85, null));
            AddSourceKey(Keys.Z, Keys.X, Keys.C, Keys.V, Keys.B, Keys.N, Keys.M);


        }
    }
}
