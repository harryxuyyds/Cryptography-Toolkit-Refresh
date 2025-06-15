using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cryptography_Toolkit.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ToolkitSettingsPage : Page
{
    private const string ThemeSettingKey = "AppTheme";

    public ToolkitSettingsPage()
    {
        InitializeComponent();
        LoadThemeSetting();
        SettingsAppThemeComboBox.SelectionChanged += SettingsAppThemeComboBox_SelectionChanged;
    }

    private void LoadThemeSetting()
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        if (localSettings.Values.TryGetValue(ThemeSettingKey, out object? theme) && theme is int themeIndex)
        {
            SettingsAppThemeComboBox.SelectedIndex = themeIndex;
            ApplyTheme(SettingsAppThemeComboBox.SelectedIndex);
        }
    }

    private void SettingsAppThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedIndex = SettingsAppThemeComboBox.SelectedIndex;
        ApplicationData.Current.LocalSettings.Values[ThemeSettingKey] = selectedIndex;
        ApplyTheme(selectedIndex);
    }

    private void ApplyTheme(int selectedIndex)
    {
        var window = App.MainWindow;
        if (window?.Content is FrameworkElement rootElement)
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
}
