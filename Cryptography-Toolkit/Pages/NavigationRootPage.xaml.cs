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
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;

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

        // ���� NavigationView �� DisplayModeChanged �¼�
        NavigationViewControl.DisplayModeChanged += NavigationViewControl_OnDisplayModeChanged;

    }

    private void NavigationRootPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            var appWindow = App.MainWindow?.AppWindow;
            RootFrame.Navigate(typeof(Cryptography_Toolkit.Pages.HomePage));
            if (appWindow != null)
            {
                var titleBar = appWindow.TitleBar;
                titleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

                // ���ô��ڱ���
                if (App.MainWindow != null) // ��ӿ����ü��
                {
                    App.MainWindow.Title = AppTitleText;
                }

                // ���� XAML �е� AppTitle TextBlock
                if (AppTitle != null)
                {
                    AppTitle.Text = AppTitleText;
                }
            }
        }

        // ���Ĵ��ڼ���״̬�ı��¼�
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

    private void NavigationViewControl_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            // sender.Header = "Toolkit Settings";
            RootFrame.Navigate(typeof(Cryptography_Toolkit.Pages.ToolkitSettingsPage));
        }
        else
        {
            var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
            string selectedItemTag = (string)selectedItem.Tag;
            string selectedItemContent = (string)selectedItem.Content;
            // sender.Header = selectedItemContent;
            string pageName = "Cryptography_Toolkit.Pages." + selectedItemTag;

            // Ensure the pageType is not null before navigating
            Type? pageType = Type.GetType(pageName);
            if (pageType != null)
            {
                RootFrame.Navigate(pageType);
            }
            else
            {
                // Handle the case where the page type could not be resolved
                throw new InvalidOperationException($"Page type '{pageName}' could not be resolved.");
            }
        }
    }
}