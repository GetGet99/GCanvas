using Microsoft.Toolkit.Wpf.UI.XamlHost;
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
using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using PInvoke;

namespace GCanvas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            myInkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Touch | CoreInputDeviceTypes.Pen;
            (Windows.UI.Xaml.Window.Current as object as IWindowPrivate)!.TransparentBackground = true;
            Loaded += delegate
            {
                dynamic corewin = Windows.UI.Core.CoreWindow.GetForCurrentThread();
                var interop = (ICoreWindowInterop)corewin;
                User32.SetWindowPos(interop.WindowHandle, new IntPtr(-1), 0, 0, 0, 0, User32.SetWindowPosFlags.SWP_NOMOVE | User32.SetWindowPosFlags.SWP_NOSIZE);
                new ToolWindow(this, myInkCanvas)
                {
                    Owner = this
                }.Show();
            };
        }
    }
}
