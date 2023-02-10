using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// KeyboardControlU.xaml 的交互逻辑
    /// </summary>
    public partial class KeyboardControlU : UserControl
    {

        public static KeyboardControlU Instance = null;
        public class KeyStruct : INotifyPropertyChanged
        {
            private bool _IsDown { get; set; } = false;
            public Keys? Key { get; set; }
            public double Width { get; set; } = 50;
            public double Height { get; set; } = 50;
            public bool IsDown { get => _IsDown; set { _IsDown = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsDown")); } }
            public object Display
            {
                get
                {
                    switch (this.Key)
                    {
                        case Keys.Escape: return "ESC";
                        case Keys.Oemtilde: return "`";
                        case Keys.OemMinus: return "-";
                        case Keys.Oemplus: return "+";
                        case Keys.PrintScreen: return "PS";
                        case Keys.PageUp: return "PU +";
                        case Keys.OemPipe: return "\\ |";
                        case Keys.PageDown: return "PD -";
                        case Keys.LShiftKey: return "Shift";
                        case Keys.Oemcomma: return ", <";
                        case Keys.OemPeriod: return ". >";
                        case Keys.Oem2: return "/ ?";
                        case Keys.RShiftKey: return "Shift";
                        case Keys.LControlKey: return "CTRL";
                        case Keys.RControlKey: return "CTRL";
                        case Keys.NumLock: return "NL";
                        case Keys.Divide: return "/";
                        case Keys.Multiply: return "*";
                        case Keys.Subtract: return "-";
                        case Keys.NumPad9: return "9";
                        case Keys.NumPad8: return "8";
                        case Keys.NumPad7: return "7";
                        case Keys.NumPad6: return "6";
                        case Keys.NumPad5: return "5";
                        case Keys.NumPad4: return "4";
                        case Keys.NumPad3: return "3";
                        case Keys.NumPad2: return "2";
                        case Keys.NumPad1: return "1";
                        case Keys.NumPad0: return "0";
                        case Keys.Decimal: return ".";
                        case Keys.D9: return "9";
                        case Keys.D8: return "8";
                        case Keys.D7: return "7";
                        case Keys.D6: return "6";
                        case Keys.D5: return "5";
                        case Keys.D4: return "4";
                        case Keys.D3: return "3";
                        case Keys.D2: return "2";
                        case Keys.D1: return "1";
                        case Keys.D0: return "0";
                        case Keys.OemSemicolon: return "; :";
                        case Keys.OemQuotes: return "' \"";
                        case Keys.OemOpenBrackets: return "[ {";
                        case Keys.Oem6: return "] }";
                        case Keys.LMenu: return "ALT";
                        case Keys.RMenu: return "ALT";
                        case Keys.Return: return "Enter";
                        default: return this.Key;
                    }
                }
            }
            public KeyStruct(Keys? k)
            {
                this.Key = k;
            }
            public KeyStruct(Keys? k, double? w, double? h)
            {
                this.Key = k; this.Width = w ?? Width; Height = h ?? Height;
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }
        private List<List<KeyStruct>> KeySources = new List<List<KeyStruct>>();
        private void AddSourceKey(params KeyStruct[] k)
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
        private void AddSourceKey(params Keys?[] k)
        {
            var ks = k.Select(k1 => new KeyStruct(k1)).ToArray();
            AddSourceKey(ks);
            Array.Clear(ks);
        }
        private void AddSourceKeyLine()
        {
            KeySources.Add(new List<KeyStruct>());
        }
        private void InitKeys()
        {
            KeySources.Clear();

            AddSourceKey(Keys.Escape, null);
            AddSourceKey(Keys.F1, Keys.F2, Keys.F3, Keys.F4);
            AddSourceKey(new KeyStruct(null, 25, null));
            AddSourceKey(Keys.F5, Keys.F6, Keys.F7, Keys.F8);
            AddSourceKey(new KeyStruct(null, 25, null));
            AddSourceKey(Keys.F9, Keys.F10, Keys.F11, Keys.F12);
            AddSourceKey(new KeyStruct(null, 15, null));
            AddSourceKey(Keys.PrintScreen, Keys.Scroll, Keys.Pause);

            AddSourceKeyLine();
            AddSourceKey(Keys.Oemtilde, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0, Keys.OemMinus, Keys.Oemplus);
            AddSourceKey(new KeyStruct(Keys.Back, 120, null));
            AddSourceKey(new KeyStruct(null, 15, null));
            AddSourceKey(Keys.Insert, Keys.Home, Keys.PageUp);
            AddSourceKey(new KeyStruct(null, 15, null));
            AddSourceKey(Keys.NumLock, Keys.Divide, Keys.Multiply, Keys.Subtract);

            AddSourceKeyLine();
            AddSourceKey(new KeyStruct(Keys.Tab, 75, null));
            AddSourceKey(Keys.Q, Keys.W, Keys.E, Keys.R, Keys.T, Keys.Y, Keys.U, Keys.I, Keys.O, Keys.P, Keys.OemOpenBrackets, Keys.Oem6);
            AddSourceKey(new KeyStruct(Keys.Oem5, 95, null));
            AddSourceKey(new KeyStruct(null, 15, null));
            AddSourceKey(Keys.Delete, Keys.End, Keys.PageDown);
            AddSourceKey(new KeyStruct(null, 15, null));
            AddSourceKey(Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, Keys.Add);

            AddSourceKeyLine();
            AddSourceKey(new KeyStruct(Keys.CapsLock, 95, null));
            AddSourceKey(Keys.A, Keys.S, Keys.D, Keys.F, Keys.G, Keys.H, Keys.J, Keys.K, Keys.L, Keys.Oem1, Keys.OemQuotes);
            AddSourceKey(new KeyStruct(Keys.Return, 135, null));
            AddSourceKey(new KeyStruct(null, 15, null));
            AddSourceKey(new KeyStruct(null, 15, null));
            AddSourceKey((Keys?)null, null, null);
            AddSourceKey(Keys.NumPad4, Keys.NumPad5, Keys.NumPad6, null);

            AddSourceKeyLine();
            AddSourceKey(new KeyStruct(Keys.LShiftKey, 110, null));
            AddSourceKey(Keys.Z, Keys.X, Keys.C, Keys.V, Keys.B, Keys.N, Keys.M, Keys.Oemcomma, Keys.OemPeriod, Keys.OemQuestion);
            AddSourceKey(new KeyStruct(Keys.RShiftKey, 180, null));
            AddSourceKey(new KeyStruct(null, 15, null));
            AddSourceKey(null, Keys.Up);
            AddSourceKey(new KeyStruct(null, 15, null));
            AddSourceKey((Keys?)null);
            AddSourceKey(Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.Return);

            AddSourceKeyLine();
            AddSourceKey(Keys.LControlKey, Keys.LWin, Keys.LMenu);
            AddSourceKey(new KeyStruct(Keys.Space, 450, null));
            AddSourceKey(new KeyStruct(null, 20, null));
            AddSourceKey(null, null, Keys.RMenu, Keys.RControlKey);
            AddSourceKey(new KeyStruct(null, 15, null));
            AddSourceKey(Keys.Left, Keys.Down, Keys.Right);
            AddSourceKey(new KeyStruct(null, 15, null));
            AddSourceKey(new KeyStruct(Keys.NumPad0, 110, null));
            AddSourceKey(Keys.Decimal, null);

        }



        public bool IsSingleClickMode
        {
            get { return (bool)GetValue(IsSingleClickModeProperty); }
            set { SetValue(IsSingleClickModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSingleClickMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSingleClickModeProperty =
            DependencyProperty.Register("IsSingleClickMode", typeof(bool), typeof(KeyboardControlU), new PropertyMetadata(false));



        public event Action<KeyboardControlU, Keys> PKeyDown;
        public event Action<KeyboardControlU, Keys> PKeyUp;
        public KeyboardControlU()
        {
            InitKeys();
            InitializeComponent();
            LIST.ItemsSource = KeySources;
            Instance = this;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox l = sender as ListBox;
            KeyStruct k = l.SelectedItem as KeyStruct;
            if (l.SelectedIndex == -1 || k.Key == null) return;
            if (IsSingleClickMode)
            {
                (l.SelectedItem as KeyStruct).IsDown = !(l.SelectedItem as KeyStruct).IsDown;
                if (k.IsDown)
                {
                    OnKeyDown(k.Key.Value);
                }
                else
                {
                    OnKeyUp(k.Key.Value);
                }
            }
            else
            {
                (l.SelectedItem as KeyStruct).IsDown = !(l.SelectedItem as KeyStruct).IsDown;
                (l.SelectedItem as KeyStruct).IsDown = !(l.SelectedItem as KeyStruct).IsDown;
                OnKeyDown(k.Key.Value);
                OnKeyUp(k.Key.Value);

            }
            l.SelectedIndex = -1;
        }
        public void KeyDown(Keys k)
        {
            foreach (var i in KeySources)
            {
                foreach (var i1 in i)
                {
                    if(i1.Key == k)
                    {
                        i1.IsDown = true;
                    }
                }
            }
        }
        public void KeyUp(Keys k)
        {
            foreach (var i in KeySources)
            {
                foreach (var i1 in i)
                {
                    if(i1.Key == k)
                    {
                        i1.IsDown = false;
                    }
                }
            }
        }
        protected virtual void OnKeyDown(Keys k)
        {
            PKeyDown?.Invoke(this, k);
        }
        protected virtual void OnKeyUp(Keys k)
        {
            PKeyUp?.Invoke(this, k);
        }
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            KeySources.Clear();
            base.OnMouseRightButtonDown(e);
        }

    }
}
