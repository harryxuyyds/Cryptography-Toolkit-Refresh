using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cryptography_Toolkit
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private OverlappedPresenter? _presenter;

        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;

            // 设置任务栏图标和窗口最小宽度
            if (AppWindow != null)
            {
                IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
                WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
                AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

                // 设置任务栏图标
                appWindow.SetIcon("ms-appx:///Assets/Square44x44Logo.png");

                // 获取 OverlappedPresenter
                _presenter = appWindow.Presenter as OverlappedPresenter;
                if (_presenter != null)
                {
                    // 设置最小宽度（以有效像素为单位）
                    _presenter.IsResizable = true;
                    _presenter.IsMaximizable = true;
                    _presenter.IsMinimizable = true;
                    _presenter.PreferredMinimumWidth = 680;
                }
            }
        }
    }
}
