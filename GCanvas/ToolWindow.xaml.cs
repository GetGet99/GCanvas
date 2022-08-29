using System;
using System.Windows;
using System.Windows.Interop;
using PInvoke;
using Windows.UI.Xaml.Controls;
using System.Drawing;
using System.Windows.Media.Media3D;
using System.Reflection.Metadata;
using Windows.UI.Input.Inking;
using System.Windows.Ink;

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

            var InkToolbar = myInkToolbar.GetUwpInternalObject() as InkToolbar;
            var InkCanvas = myInkCanvas.GetUwpInternalObject() as InkCanvas;
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
            var MouseIcon = new SymbolIcon((Symbol)0xE962);
            var btn = new InkToolbarCustomToolButton
            {
                IsChecked = false,
                Content = MouseIcon
            };
            InkToolbar.Children.Insert(0, btn);
            {
                var screenshotbtn = new InkToolbarCustomToggleButton
                {
                    Content = new SymbolIcon(Symbol.Camera)
                };
                InkToolbar.Children.Insert(0, screenshotbtn);
                screenshotbtn.Click += delegate
                {
                    Hide();
                    var screen = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(mw).Handle);
                    using var hdcScreen = new User32.SafeDCHandle(
                        IntPtr.Zero, Gdi32Ex.CreateDCW("Display", null, null, IntPtr.Zero)
                    );
                    var bounds = screen.Bounds;

                    // Reference: https://ourcodeworld.com/articles/read/195/capturing-screenshots-of-different-ways-with-c-and-winforms
                    var handle = User32.GetDesktopWindow();
                    using var hdcSrc = User32.GetWindowDC(handle);
                    using var hdcDest = Gdi32.CreateCompatibleDC(hdcSrc);
                    var hBitmap = Gdi32.CreateCompatibleBitmap(hdcSrc, bounds.Width, bounds.Height);
                    var hOld = Gdi32.SelectObject(hdcDest, hBitmap);
                    Gdi32.BitBlt(hdcDest, 0, 0, bounds.Width, bounds.Height, hdcSrc, 0, 0, 0x00CC0020);
                    Show();
                    Gdi32.SelectObject(hdcDest, hOld);
                    Gdi32.DeleteDC(hdcDest);
                    hdcSrc.Dispose();
                    System.Windows.Forms.Clipboard.SetImage(System.Drawing.Image.FromHbitmap(hBitmap));
                };
            }
            {
                var clearbtn = new InkToolbarCustomToggleButton
                {
                    Content = new SymbolIcon(Symbol.Clear)
                };
                Button btnConfirmDelete;
                var flyout = new Flyout
                {
                    Content = new StackPanel
                    {
                        Children =
                        {
                            new TextBlock { Text = "Warning: All the annotations will be deleted" },
                            (btnConfirmDelete = new Button {
                                Content = "Confirm!",
                                Margin = new Windows.UI.Xaml.Thickness {
                                    Top = 16
                                }
                            }
                            )
                        }
                    },
                    Placement = Windows.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Bottom,
                    ShouldConstrainToRootBounds = false
                };
                InkToolbar.Children.Add(clearbtn);
                clearbtn.Click += delegate
                {
                    flyout.ShowAt(clearbtn);
                };
                btnConfirmDelete.Click += delegate
                {
                    InkCanvas.InkPresenter.StrokeContainer.Clear();
                    flyout.Hide();
                };
            }
            {
                //var lasso = new InkToolbarCustomPenButton
                //{
                    
                //};
                //InkToolbar.Children.Add(lasso);
                //InkToolbar.ActiveToolChanged += delegate
                //{
                //    if (InkToolbar.ActiveTool == lasso)
                //    {

                //    }
                //};
                
                //InkCanvas.InkPresenter.InputProcessingConfiguration.RightDragAction = InkInputRightDragAction.LeaveUnprocessed;


                //var inkstroke = new InkStrokeBuilder().CreateStroke(new Windows.Foundation.Point[]
                //{
                //    new Windows.Foundation.Point(0, 100),
                //    new Windows.Foundation.Point(100, 0),
                //    new Windows.Foundation.Point(200, 100),
                //    new Windows.Foundation.Point(100, 200),
                //    new Windows.Foundation.Point(0, 100)
                //});
                //inkstroke.DrawingAttributes.Color = Windows.UI.Color.FromArgb(255, 255, 255, 255);
                //inkstroke.DrawingAttributes.PenTip = PenTipShape.Circle;
                //InkCanvas.InkPresenter.StrokeContainer.AddStroke(inkstroke);

            }
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
                }
                catch
                {

                }
            };
            MouseDown += (o, e) =>
            {
                if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                {
                    DragMove();
                }
                else if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
                {
                    Close();
                }
            };
        }
    }
}
