using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Cryptography_Toolkit.Pages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cryptography_Toolkit
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private const string ThemeSettingKey = "AppTheme";
        private Window? _window;
        public static Window? MainWindow { get; set; }

        public static int MinimumWidth { get; } = 800;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            MainWindow = _window;

            // 创建 NavigationRootPage 实例并设置为窗口内容
            NavigationRootPage rootPage = new NavigationRootPage();
            _window.Content = rootPage;
            
            // 确保在窗口激活后再加载主题
            _window.Activated += (sender, e) =>
            {
                LoadThemeSetting();
            };
            
            _window.Activate();
        }

        private void LoadAndApplyTheme()
        {
            if (MainWindow?.Content is FrameworkElement rootElement)
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                if (localSettings.Values.TryGetValue(ThemeSettingKey, out object? theme) && theme is int themeIndex)
                {
                    ElementTheme requestedTheme = themeIndex switch
                    {
                        0 => ElementTheme.Light,
                        1 => ElementTheme.Dark,
                        _ => ElementTheme.Default
                    };
                    rootElement.RequestedTheme = requestedTheme;
                }
            }
        }

        private void LoadThemeSetting()
        {
            var localThemeSettingsKey = ApplicationData.Current.LocalSettings.Values[ThemeSettingKey];
            ApplyTheme((int)localThemeSettingsKey);
        }

        private void ApplyTheme(int selectedIndex)
        {
            try
            {
                if (App.MainWindow?.Content is FrameworkElement rootElement
                    && App.MainWindow.AppWindow != null)
                {
                    switch (selectedIndex)
                    {
                        case 0: // 浅色
                            rootElement.RequestedTheme = ElementTheme.Light;
                            break;
                        case 1: // 深色
                            rootElement.RequestedTheme = ElementTheme.Dark;
                            break;
                        case 2: // 系统默认
                            rootElement.RequestedTheme = ElementTheme.Default;
                            break;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                // 忽略已关闭窗口的异常
                Debug.WriteLine("Window already closed, theme change ignored.");
            }
        }
    }
}
