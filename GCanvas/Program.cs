using System;
namespace GCanvas;

public class Program
{
    [STAThread]
    public static void Main()
    {
        using (new UWP.App())
        {
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
