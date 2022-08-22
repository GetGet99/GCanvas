using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GCanvas
{
    /// <summary>
    /// Interaction logic for ToolWindow.xaml
    /// </summary>
    public partial class ToolWindow : Window
    {
        public ToolWindow(Microsoft.Toolkit.Wpf.UI.Controls.InkCanvas myInkCanvas)
        {
            InitializeComponent();
            var InkToolbar = myInkToolbar.GetUwpInternalObject() as Windows.UI.Xaml.Controls.InkToolbar;
            var InkCanvas = myInkCanvas.GetUwpInternalObject() as Windows.UI.Xaml.Controls.InkCanvas;
            InkToolbar.TargetInkCanvas = InkCanvas;
        }
    }
}
