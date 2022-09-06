using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GCanvas;

class Gdi32Ex
{
    [DllImport("gdi32.dll", EntryPoint = "CreateDCW", CharSet = CharSet.Unicode)]
    public static extern IntPtr CreateDCW(string strDriver, string? strDevice, string? strOutput, IntPtr pData);
}
