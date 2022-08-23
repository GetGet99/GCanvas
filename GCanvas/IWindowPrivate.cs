﻿// Reference: https://github.com/ADeltaX/IWindowPrivate/blob/main/IWindowPrivate.cs
using System;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.UI.Xaml;

//PLACEHOLDER - Replace with the *correct* GUID. I haven't found any hint for this, yet.
[ComImport, Guid("15645012-8F3F-5090-B584-DF078FCC509A"), InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
public interface IAtlasRequestCallback
{
    bool AtlasRequest(uint width, uint height, Windows.Graphics.DirectX.DirectXPixelFormat pixelFormat);
}

[ComImport, Guid("06636C29-5A17-458D-8EA2-2422D997A922"), InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
public interface IWindowPrivate
{
    bool TransparentBackground { get; set; }
    void Show();
    void Hide();
    void MoveWindow(int x, int y, int width, int height);
    void SetAtlasSizeHint(uint width, uint height);
    void ReleaseGraphicsDeviceOnSuspend(bool enable);
    void SetAtlasRequestCallback(IAtlasRequestCallback callback);
    Rect GetWindowContentBoundsForElement(DependencyObject element);
}

// Reference: https://stackoverflow.com/questions/34935077/getting-hwnd-off-of-corewindow-object-in-uwp
[ComImport, Guid("45D64A29-A63E-4CB6-B498-5781D298CB4F")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
interface ICoreWindowInterop
{
    IntPtr WindowHandle { get; }
    bool MessageHandled { set; }
}