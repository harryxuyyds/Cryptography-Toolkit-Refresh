using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cryptography_Toolkit.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class NavigationRootPage : Page
{
    public NavigationRootPage()
    {
        InitializeComponent();
        this.Loaded += NavigationRootPage_Loaded;
        
        // 订阅 NavigationView 的 DisplayModeChanged 事件
        NavigationViewControl.DisplayModeChanged += NavigationViewControl_OnDisplayModeChanged;

    }

    private void NavigationRootPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            var appWindow = App.MainWindow?.AppWindow;
            if (appWindow != null)
            {
                var titleBar = appWindow.TitleBar;
                titleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

                // 设置窗口标题
                if (App.MainWindow != null) // 添加空引用检查
                {
                    App.MainWindow.Title = AppTitleText;
                }

                // 更新 XAML 中的 AppTitle TextBlock
                if (AppTitle != null)
                {
                    AppTitle.Text = AppTitleText;
                }
            }
        }

        // 订阅窗口激活状态改变事件
        if (App.MainWindow != null)
        {
            App.MainWindow.Activated += MainWindow_Activated;
        }
    }

    private void NavigationViewControl_OnDisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        if (sender.PaneDisplayMode == NavigationViewPaneDisplayMode.Top)
        {
            VisualStateManager.GoToState(this, "Top", true);
        }
        else
        {
            if (args.DisplayMode == NavigationViewDisplayMode.Minimal)
            {
                VisualStateManager.GoToState(this, "Compact", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Default", true);
            }
        }
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState == WindowActivationState.Deactivated)
        {
            VisualStateManager.GoToState(this, "Deactivated", true);
        }
        else
        {
            VisualStateManager.GoToState(this, "Activated", true);
        }
    }

    public string AppTitleText
    {
        get
        {
#if DEBUG
            return "Cryptography Toolkit Dev";
#else
            return "Cryptography Toolkit";
#endif
        }
    }

}
