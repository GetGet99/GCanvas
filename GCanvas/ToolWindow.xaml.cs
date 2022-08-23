using System.Windows;
using System.Windows.Interop;
using PInvoke;
using WinUIControls = Windows.UI.Xaml.Controls;
namespace GCanvas
{
    /// <summary>
    /// Interaction logic for ToolWindow.xaml
    /// </summary>
    public partial class ToolWindow : Window
    {
        public ToolWindow(MainWindow mw, Microsoft.Toolkit.Wpf.UI.Controls.InkCanvas myInkCanvas)
        {
            InitializeComponent();

            var InkToolbar = myInkToolbar.GetUwpInternalObject() as WinUIControls.InkToolbar;
            var InkCanvas = myInkCanvas.GetUwpInternalObject() as WinUIControls.InkCanvas;
            InkToolbar.RequestedTheme = Windows.UI.Xaml.ElementTheme.Dark;
            InkToolbar.TargetInkCanvas = InkCanvas;
            InkToolbar.Background = new Windows.UI.Xaml.Media.SolidColorBrush(new Windows.UI.Color
            {
                R = 32,
                G = 32,
                B = 32,
                A = 255
            });
            var uisetttings = new Windows.UI.ViewManagement.UISettings();
            var MouseIcon = new WinUIControls.SymbolIcon((WinUIControls.Symbol)0xE962);
            var btn = new WinUIControls.InkToolbarCustomToolButton
            {
                IsChecked = false,
                Content = MouseIcon
            };
            InkToolbar.Children.Insert(0,btn);
            void SetClickThrough(Window mw)
            {
                var handle = new WindowInteropHelper(mw).Handle;
                var style = (User32.WindowStylesEx)User32.GetWindowLong(handle, User32.WindowLongIndexFlags.GWL_EXSTYLE);
                style |= User32.WindowStylesEx.WS_EX_TOOLWINDOW;
                style &= ~User32.WindowStylesEx.WS_EX_APPWINDOW;
                style |= (User32.WindowStylesEx)0x80000 | User32.WindowStylesEx.WS_EX_TRANSPARENT;
                User32.SetWindowLong(handle, User32.WindowLongIndexFlags.GWL_EXSTYLE, (User32.SetWindowLongFlags)style);
            }
            void UnSetClickThrough(Window mw)
            {
                var handle = new WindowInteropHelper(mw).Handle;
                var style = (User32.WindowStylesEx)User32.GetWindowLong(handle, User32.WindowLongIndexFlags.GWL_EXSTYLE);
                style |= User32.WindowStylesEx.WS_EX_TOOLWINDOW;
                style &= ~User32.WindowStylesEx.WS_EX_APPWINDOW;
                style &= ~User32.WindowStylesEx.WS_EX_TRANSPARENT;
                User32.SetWindowLong(handle, User32.WindowLongIndexFlags.GWL_EXSTYLE, (User32.SetWindowLongFlags)style);
            }
            btn.Checked += delegate
            {
                SetClickThrough(mw);
                MouseIcon.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(
                    uisetttings.GetColorValue(Windows.UI.ViewManagement.UIColorType.AccentLight2)
                );
            };
            btn.Unchecked += delegate
            {
                UnSetClickThrough(mw);
                MouseIcon.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(
                    Windows.UI.Colors.White
                );
            };
            Loaded += delegate
            {
                UnSetClickThrough(mw);
                UnSetClickThrough(this);
            };
            Closed += delegate
            {
                try
                {
                    mw?.Close();
                } catch
                {

                }
            };
            MouseDown += (o, e) =>
            {
                if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                {
                    DragMove();
                } else if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
                {
                    Close();
                }
            };
        }
    }
}
