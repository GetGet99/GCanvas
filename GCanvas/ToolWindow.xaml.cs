using System;
using System.Windows;
using System.Windows.Interop;
using PInvoke;
using Windows.UI.Xaml.Controls;
using Windows.UI.Input.Inking;
using Colors = Windows.UI.Colors;
using SolidColorBrush = Windows.UI.Xaml.Media.SolidColorBrush;
using Point = Windows.Foundation.Point;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using InkDrawingAttributes = Windows.UI.Input.Inking.InkDrawingAttributes;
using InkToolbarInitialControls = Windows.UI.Xaml.Controls.InkToolbarInitialControls;
using System.Windows.Documents;
using Windows.UI.Xaml.Shapes;
using System.Drawing;
using System.Numerics;

namespace GCanvas;

public partial class ToolWindow : Window
{
    readonly InkToolbarBallpointPenButton InkToolbarBallpointPenButton = new InkToolbarBallpointPenButton
    {
        SelectedBrushIndex = 7
    };
    readonly InkToolbarPencilButton InkToolbarPencilButton = new InkToolbarPencilButton
    {
        SelectedBrushIndex = 7
    };
    readonly InkToolbarHighlighterButton InkToolbarHighlighterButton = new();
    readonly InkToolbarEraserButton InkToolbarEraserButton = new();
    readonly InkToolbarStencilButton InkToolbarStencilButton = new();
    readonly Windows.UI.ViewManagement.UISettings uisetttings = new();

    public ToolWindow(MainWindow mw, Microsoft.Toolkit.Wpf.UI.Controls.InkCanvas myInkCanvas)
    {
        InitializeComponent();
        // ToolWindow Init
        {
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
        var InkToolbar = (InkToolbar)myInkToolbar.GetUwpInternalObject();
        var InkCanvas = (InkCanvas)myInkCanvas.GetUwpInternalObject();
        // InkToolbar Init
        {
            InkToolbar.InitialControls = InkToolbarInitialControls.None;
            InkToolbar.Children.Add(InkToolbarBallpointPenButton);
            InkToolbar.Children.Add(InkToolbarPencilButton);
            InkToolbar.Children.Add(InkToolbarHighlighterButton);
            InkToolbar.Children.Add(InkToolbarEraserButton);
            InkToolbar.RequestedTheme = Windows.UI.Xaml.ElementTheme.Dark;
            InkToolbar.TargetInkCanvas = InkCanvas;
            InkToolbar.Background = new SolidColorBrush(new Windows.UI.Color
            {
                R = 32,
                G = 32,
                B = 32,
                A = 255
            });
        }
        // Mouse
        {
            var MouseIcon = new SymbolIcon((Symbol)0xE962);
            var MouseBtn = new InkToolbarCustomToolButton
            {
                IsChecked = false,
                Content = MouseIcon
            };
            InkToolbar.Children.Insert(0, MouseBtn);
            Loaded += delegate
            {
                UnSetClickThrough(mw);
                UnSetClickThrough(this);
            };
            MouseBtn.Checked += delegate
            {
                SetClickThrough(mw);
                MouseIcon.Foreground = new SolidColorBrush(
                    uisetttings.GetColorValue(Windows.UI.ViewManagement.UIColorType.AccentLight2)
                );
            };
            MouseBtn.Unchecked += delegate
            {
                UnSetClickThrough(mw);
                MouseIcon.Foreground = new SolidColorBrush(
                    Colors.White
                );
            };
        }
        // Line Tool
        {
            TextBlock LineIcon;
            var LineButton = new InkToolbarCustomToolButton
            {
                Content = LineIcon = new TextBlock { Text = "L" }
            };
            LineButton.Checked += delegate
            {
                LineIcon.Foreground = new SolidColorBrush(
                    uisetttings.GetColorValue(Windows.UI.ViewManagement.UIColorType.AccentLight2)
                );
                InkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(new InkDrawingAttributes
                {
                    Color = Colors.Transparent
                });
            };
            LineButton.Unchecked += delegate
            {
                LineIcon.Foreground = new SolidColorBrush(Colors.White);
            };
            InkToolbar.Children.Add(LineButton);
            var pt1 = new Point();
            var builder = new InkStrokeBuilder();
            InkCanvas.InkPresenter.UnprocessedInput.PointerPressed += (_, e) =>
            {
                if ((LineButton.IsChecked ?? false))
                {
                    var _pt1 = e.CurrentPoint;
                    pt1 = new Point(_pt1.RawPosition.X, _pt1.RawPosition.Y);
                    var strokeWidth = InkToolbarBallpointPenButton.SelectedStrokeWidth;

                    builder.SetDefaultDrawingAttributes(new InkDrawingAttributes
                    {
                        Color = (InkToolbarBallpointPenButton.SelectedBrush as SolidColorBrush)?.Color ?? Colors.Red,
                        PenTip = PenTipShape.Circle,
                        Size = new Windows.Foundation.Size(strokeWidth, strokeWidth),
                        FitToCurve = false
                    });
                }
            };
            InkStroke? tmp = null;
            InkCanvas.InkPresenter.UnprocessedInput.PointerMoved += (_, e) =>
            {
                if ((LineButton.IsChecked ?? false))
                {

                    if (tmp is not null)
                    {
                        tmp.Selected = true;
                        InkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
                    }
                    tmp = DrawLine(e.CurrentPoint);
                }
            };
            InkCanvas.InkPresenter.UnprocessedInput.PointerReleased += (_, e) =>
            {
                if ((LineButton.IsChecked ?? false))
                {
                    if (tmp is not null)
                    {
                        tmp.Selected = true;
                        InkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
                    }
                    tmp = null;
                    DrawLine(e.CurrentPoint);
                }
            };
            InkStroke DrawLine(Windows.UI.Input.PointerPoint _pt2)
            {
                var pt2 = new Point(_pt2.RawPosition.X, _pt2.RawPosition.Y);
                //var topleft = new Point(Math.Min(pt1.X, pt2.X), Math.Min(pt1.Y, pt2.Y));
                //var botrigh = new Point(Math.Max(pt1.X, pt2.X), Math.Max(pt1.Y, pt2.Y));
                var stroke = builder.CreateStroke(new Point[]
                {
                    pt1,
                    pt2
                });

                InkCanvas.InkPresenter.StrokeContainer.AddStroke(stroke);
                return stroke;
            }
        }
        // Arrow Tool
        {
            SymbolIcon ArriwIcon;
            var ArrowButton = new InkToolbarCustomToolButton
            {
                Content = ArriwIcon = new SymbolIcon((Symbol)0xebe7)
            };
            ArrowButton.Checked += delegate
            {
                ArriwIcon.Foreground = new SolidColorBrush(
                    uisetttings.GetColorValue(Windows.UI.ViewManagement.UIColorType.AccentLight2)
                );
                InkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(new InkDrawingAttributes
                {
                    Color = Colors.Transparent
                });
            };
            ArrowButton.Unchecked += delegate
            {
                ArriwIcon.Foreground = new SolidColorBrush(Colors.White);
            };
            InkToolbar.Children.Add(ArrowButton);
            var pt1 = new Point();
            var builder = new InkStrokeBuilder();
            InkCanvas.InkPresenter.UnprocessedInput.PointerPressed += (_, e) =>
            {
                if ((ArrowButton.IsChecked ?? false))
                {
                    var _pt1 = e.CurrentPoint;
                    pt1 = new Point(_pt1.RawPosition.X, _pt1.RawPosition.Y);
                    var strokeWidth = InkToolbarBallpointPenButton.SelectedStrokeWidth;

                    builder.SetDefaultDrawingAttributes(new InkDrawingAttributes
                    {
                        Color = (InkToolbarBallpointPenButton.SelectedBrush as SolidColorBrush)?.Color ?? Colors.Red,
                        PenTip = PenTipShape.Circle,
                        Size = new Windows.Foundation.Size(strokeWidth, strokeWidth),
                        FitToCurve = false
                    });
                }
            };
            InkStroke? tmp = null;
            InkCanvas.InkPresenter.UnprocessedInput.PointerMoved += (_, e) =>
            {
                if ((ArrowButton.IsChecked ?? false))
                {

                    if (tmp is not null)
                    {
                        tmp.Selected = true;
                        InkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
                    }
                    tmp = DrawArrow(e.CurrentPoint);
                }
            };
            InkCanvas.InkPresenter.UnprocessedInput.PointerReleased += (_, e) =>
            {
                if ((ArrowButton.IsChecked ?? false))
                {
                    if (tmp is not null)
                    {
                        tmp.Selected = true;
                        InkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
                    }
                    tmp = null;
                    DrawArrow(e.CurrentPoint);
                }
            };
            InkStroke DrawArrow(Windows.UI.Input.PointerPoint _pt2)
            {
                var pt2 = new Point(_pt2.RawPosition.X, _pt2.RawPosition.Y);
                // m = delta Y / delta X
                var p = pt2;
                var dx = pt1.X - pt2.X;
                var dy = pt1.Y - pt2.Y;
                // Edit and modified from: https://stackoverflow.com/questions/3010803/draw-arrow-on-line-algorithm
                const double cos = 0.866;
                const double sin = 0.500;
                var endVec = Vector2.Normalize(new Vector2((float)(dx * cos + dy * -sin), (float)(dx * sin + dy * cos))) * 30;
                Point end1 = new Point(
                    (p.X + endVec.X),
                    (p.Y + endVec.Y));
                endVec = Vector2.Normalize(new Vector2((float)(dx * cos + dy * sin), (float)(dx * -sin + dy * cos))) * 30;
                Point end2 = new Point(
                    (p.X + endVec.X),
                    (p.Y + endVec.Y));
                var stroke = builder.CreateStroke(new Point[]
                {
                    pt1,
                    pt2,
                    end2,
                    pt2,
                    end1
                });

                InkCanvas.InkPresenter.StrokeContainer.AddStroke(stroke);
                return stroke;
            }
        }
        // Rectangle and Crop
        InkToolbarCustomToolButton CropTool;
        {
            SymbolIcon RectangleIcon;
            var Rectangle = new InkToolbarCustomToolButton
            {
                Content = RectangleIcon = new SymbolIcon((Symbol)0xf16b)
            };
            Rectangle.Checked += delegate
            {
                RectangleIcon.Foreground = new SolidColorBrush(
                    uisetttings.GetColorValue(Windows.UI.ViewManagement.UIColorType.AccentLight2)
                );
                InkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(new InkDrawingAttributes
                {
                    Color = Colors.Transparent
                });
            };
            Rectangle.Unchecked += delegate
            {
                RectangleIcon.Foreground = new SolidColorBrush(Colors.White);
            };
            SymbolIcon CropIcon;
            CropTool = new InkToolbarCustomToolButton
            {
                Content = CropIcon = new SymbolIcon(Symbol.Crop)
            };
            CropTool.Checked += delegate
            {
                CropIcon.Foreground = new SolidColorBrush(
                    uisetttings.GetColorValue(Windows.UI.ViewManagement.UIColorType.AccentLight2)
                );
                InkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(new InkDrawingAttributes
                {
                    Color = Colors.Transparent
                });
            };
            CropTool.Unchecked += delegate
            {
                CropIcon.Foreground = new SolidColorBrush(Colors.White);
            };
            InkToolbar.Children.Add(Rectangle);
            var pt1 = new Point();
            var builder = new InkStrokeBuilder();
            InkCanvas.InkPresenter.UnprocessedInput.PointerPressed += (_, e) =>
            {
                if ((Rectangle.IsChecked ?? false) || (CropTool.IsChecked ?? false))
                {
                    var _pt1 = e.CurrentPoint;
                    pt1 = new Point(_pt1.RawPosition.X, _pt1.RawPosition.Y);
                    var strokeWidth = InkToolbarBallpointPenButton.SelectedStrokeWidth;
                    if (CropTool.IsChecked ?? false)
                        builder.SetDefaultDrawingAttributes(new InkDrawingAttributes
                        {
                            Color = Colors.Red,
                            PenTip = PenTipShape.Circle,
                            Size = new Windows.Foundation.Size(1, 1),
                            FitToCurve = false
                        });
                    else
                        builder.SetDefaultDrawingAttributes(new InkDrawingAttributes
                        {
                            Color = (InkToolbarBallpointPenButton.SelectedBrush as SolidColorBrush)?.Color ?? Colors.Red,
                            PenTip = PenTipShape.Circle,
                            Size = new Windows.Foundation.Size(strokeWidth, strokeWidth),
                            FitToCurve = false
                        });
                }
            };
            InkStroke? tmp = null;
            InkCanvas.InkPresenter.UnprocessedInput.PointerMoved += (_, e) =>
            {
                if ((Rectangle.IsChecked ?? false) || (CropTool.IsChecked ?? false))
                {

                    if (tmp is not null)
                    {
                        tmp.Selected = true;
                        InkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
                    }
                    tmp = DrawRectangle(e.CurrentPoint);
                }
            };
            InkCanvas.InkPresenter.UnprocessedInput.PointerReleased += (_, e) =>
            {
                if ((Rectangle.IsChecked ?? false) || (CropTool.IsChecked ?? false))
                {
                    if (tmp is not null)
                    {
                        tmp.Selected = true;
                        InkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
                    }
                    tmp = null;
                    if (CropTool.IsChecked ?? false)
                    {
                        var _pt2 = e.CurrentPoint;
                        var pt2 = new Point(_pt2.RawPosition.X, _pt2.RawPosition.Y);
                        var topleft = new Point(Math.Min(pt1.X, pt2.X), Math.Min(pt1.Y, pt2.Y));
                        var botrigh = new Point(Math.Max(pt1.X, pt2.X), Math.Max(pt1.Y, pt2.Y));
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
                        var width = (int)(botrigh.X - topleft.X);
                        var height = (int)(botrigh.Y - topleft.Y);
                        var hBitmap = Gdi32.CreateCompatibleBitmap(hdcSrc, width, height);
                        var hOld = Gdi32.SelectObject(hdcDest, hBitmap);
                        Gdi32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, bounds.Top + (int)topleft.X, bounds.Left + (int)topleft.Y, 0x00CC0020);
                        Show();
                        Gdi32.SelectObject(hdcDest, hOld);
                        Gdi32.DeleteDC(hdcDest);
                        hdcSrc.Dispose();
                        System.Windows.Forms.Clipboard.SetImage(System.Drawing.Image.FromHbitmap(hBitmap));
                        CropTool.IsChecked = false;
                    }
                    else
                    {
                        DrawRectangle(e.CurrentPoint);
                    }
                }
            };
            InkStroke DrawRectangle(Windows.UI.Input.PointerPoint _pt2)
            {
                var pt2 = new Point(_pt2.RawPosition.X, _pt2.RawPosition.Y);
                var topleft = new Point(Math.Min(pt1.X, pt2.X), Math.Min(pt1.Y, pt2.Y));
                var botrigh = new Point(Math.Max(pt1.X, pt2.X), Math.Max(pt1.Y, pt2.Y));
                var stroke = builder.CreateStroke(new Point[]
                {
                        topleft,
                        new Point(topleft.X, botrigh.Y),
                        botrigh,
                        new Point(botrigh.X, topleft.Y),
                        topleft
                });

                InkCanvas.InkPresenter.StrokeContainer.AddStroke(stroke);
                return stroke;
            }
        }
        // Ellipse Tool
        {
            SymbolIcon EllipseIcon;
            var EllipseButton = new InkToolbarCustomToolButton
            {
                Content = EllipseIcon = new SymbolIcon((Symbol)0xEA3A)
            };
            EllipseButton.Checked += delegate
            {
                EllipseIcon.Foreground = new SolidColorBrush(
                    uisetttings.GetColorValue(Windows.UI.ViewManagement.UIColorType.AccentLight2)
                );
                InkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(new InkDrawingAttributes
                {
                    Color = Colors.Transparent
                });
            };
            EllipseButton.Unchecked += delegate
            {
                EllipseIcon.Foreground = new SolidColorBrush(Colors.White);
            };
            InkToolbar.Children.Add(EllipseButton);
            var pt1 = new Point();
            var builder = new InkStrokeBuilder();
            InkCanvas.InkPresenter.UnprocessedInput.PointerPressed += (_, e) =>
            {
                if ((EllipseButton.IsChecked ?? false))
                {
                    var _pt1 = e.CurrentPoint;
                    pt1 = new Point(_pt1.RawPosition.X, _pt1.RawPosition.Y);
                    var strokeWidth = InkToolbarBallpointPenButton.SelectedStrokeWidth;

                    builder.SetDefaultDrawingAttributes(new InkDrawingAttributes
                    {
                        Color = (InkToolbarBallpointPenButton.SelectedBrush as SolidColorBrush)?.Color ?? Colors.Red,
                        PenTip = PenTipShape.Circle,
                        Size = new Windows.Foundation.Size(strokeWidth, strokeWidth),
                        FitToCurve = false
                    });
                }
            };
            InkStroke? tmp = null;
            InkCanvas.InkPresenter.UnprocessedInput.PointerMoved += (_, e) =>
            {
                if ((EllipseButton.IsChecked ?? false))
                {

                    if (tmp is not null)
                    {
                        tmp.Selected = true;
                        InkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
                    }
                    tmp = DrawEllipse(e.CurrentPoint);
                }
            };
            InkCanvas.InkPresenter.UnprocessedInput.PointerReleased += (_, e) =>
            {
                if ((EllipseButton.IsChecked ?? false))
                {
                    if (tmp is not null)
                    {
                        tmp.Selected = true;
                        InkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
                    }
                    tmp = null;
                    DrawEllipse(e.CurrentPoint);
                }
            };
            InkStroke DrawEllipse(Windows.UI.Input.PointerPoint _pt2)
            {
                var pt2 = new Point(_pt2.RawPosition.X, _pt2.RawPosition.Y);
                var topleft = new Point(Math.Min(pt1.X, pt2.X), Math.Min(pt1.Y, pt2.Y));
                var botrigh = new Point(Math.Max(pt1.X, pt2.X), Math.Max(pt1.Y, pt2.Y));
                var radiusX = (botrigh.X - topleft.X) / 2;
                var radiusY = (botrigh.Y - topleft.Y) / 2;

                (double X, double Y) Ellipse(double t)
                {
                    var x = radiusX * Math.Cos(t) + radiusX + topleft.X;
                    var y = radiusY * Math.Sin(t) + radiusY + topleft.Y;
                    return (x, y);
                }
                const double raincir = 2 * Math.PI;
                var stroke = builder.CreateStroke(
                    from t in Enumerable.Range(0, 360)
                    let trad = t * 1 / raincir
                    let a = Ellipse(trad)
                    select new Point(a.X, a.Y)
                );

                InkCanvas.InkPresenter.StrokeContainer.AddStroke(stroke);
                return stroke;
            }
        }
        // Screenshot
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
            InkToolbar.Children.Add(CropTool);
        }
        InkToolbar.Children.Add(InkToolbarStencilButton);
        // Clear
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
        // Lasso (Not Finished)
        {
            var lasso = new InkToolbarCustomToolButton
            {
                
            };
            var builder = new InkStrokeBuilder();
            builder.SetDefaultDrawingAttributes(new InkDrawingAttributes
            {
                Color = Colors.Red,
                PenTip = PenTipShape.Circle,
                Size = new Windows.Foundation.Size(1, 1),
                FitToCurve = true
            });
            List<Point> List = new();
            InkStroke? tmp = null;
            //InkToolbar.Children.Add(lasso);
            InkCanvas.InkPresenter.UnprocessedInput.PointerPressed += (_, e) =>
            {
                if (lasso.IsChecked ?? false)
                {
                    List.Clear();
                    var pt = e.CurrentPoint;
                    List.Add(new Point(pt.Position.X, pt.Position.Y));
                    var stroke = tmp = builder.CreateStroke(List);

                    InkCanvas.InkPresenter.StrokeContainer.AddStroke(stroke);
                }
            };
            InkCanvas.InkPresenter.UnprocessedInput.PointerMoved += (_, e) =>
            {
                if (lasso.IsChecked ?? false)
                {
                    if (tmp is not null)
                    {
                        tmp.Selected = true;
                        InkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
                    }
                    var pt = e.CurrentPoint;
                    List.Add(new Point(pt.Position.X, pt.Position.Y));
                    var stroke = tmp = builder.CreateStroke(List);

                    InkCanvas.InkPresenter.StrokeContainer.AddStroke(stroke);
                }
            };
            InkCanvas.InkPresenter.UnprocessedInput.PointerReleased += (_, e) =>
            {
                if (lasso.IsChecked ?? false)
                {
                    if (tmp is not null)
                    {
                        tmp.Selected = true;
                        InkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
                    }
                    InkCanvas.InkPresenter.StrokeContainer.SelectWithPolyLine(List);
                    List.Clear();
                }
            };


        }
    }
    static void SetClickThrough(Window mw)
    {
        var handle = new WindowInteropHelper(mw).Handle;
        var style = (User32.WindowStylesEx)User32.GetWindowLong(handle, User32.WindowLongIndexFlags.GWL_EXSTYLE);
        style |= User32.WindowStylesEx.WS_EX_TOOLWINDOW;
        style &= ~User32.WindowStylesEx.WS_EX_APPWINDOW;
        style |= (User32.WindowStylesEx)0x80000 | User32.WindowStylesEx.WS_EX_TRANSPARENT;
        User32.SetWindowLong(handle, User32.WindowLongIndexFlags.GWL_EXSTYLE, (User32.SetWindowLongFlags)style);
    }
    static void UnSetClickThrough(Window mw)
    {
        var handle = new WindowInteropHelper(mw).Handle;
        var style = (User32.WindowStylesEx)User32.GetWindowLong(handle, User32.WindowLongIndexFlags.GWL_EXSTYLE);
        style |= User32.WindowStylesEx.WS_EX_TOOLWINDOW;
        style &= ~User32.WindowStylesEx.WS_EX_APPWINDOW;
        style &= ~User32.WindowStylesEx.WS_EX_TRANSPARENT;
        User32.SetWindowLong(handle, User32.WindowLongIndexFlags.GWL_EXSTYLE, (User32.SetWindowLongFlags)style);
    }
}

