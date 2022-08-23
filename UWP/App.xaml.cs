using System.Resources;

namespace UWP;

public sealed partial class App : Microsoft.Toolkit.Win32.UI.XamlHost.XamlApplication
{
    public App()
    {
        try
        {
            //Resources[null] = new Microsoft.UI.Xaml.Controls.XamlControlsResources
            //{
            //    ControlsResourcesVersion = Microsoft.UI.Xaml.Controls.ControlsResourcesVersion.Version2
            //};
            Initialize();
        } catch
        {

        }
        
    }
}
