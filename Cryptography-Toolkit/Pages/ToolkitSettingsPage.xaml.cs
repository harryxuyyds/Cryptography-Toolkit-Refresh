using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.ApplicationModel.WindowsAppRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

using Cryptography_Toolkit.Helpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cryptography_Toolkit.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ToolkitSettingsPage : Page
{
    private const string ThemeSettingKey = "AppTheme";
    private const string FontSettingKey = "EditorFontFamily";

    private string _selectedFontFamily = string.Empty;

    public string WinAppSdkRuntimeDetails => VersionHelper.WinAppSdkRuntimeDetails;

    public ToolkitSettingsPage()
    {
        InitializeComponent();
        LoadThemeSetting();
        LoadSystemFonts();
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
            var appWindow = AppWindow.GetFromWindowId(window.AppWindow.Id); // 使用 AppWindow 获取 TitleBar
            var titleBar = appWindow.TitleBar;

            switch (selectedIndex)
            {
                case 0: // 浅色
                    rootElement.RequestedTheme = ElementTheme.Light;
                    window.ExtendsContentIntoTitleBar = true;
                    SetCaptionButtonColors(window, Colors.Black);
                    break;
                case 1: // 深色
                    rootElement.RequestedTheme = ElementTheme.Dark;
                    window.ExtendsContentIntoTitleBar = true;
                    SetCaptionButtonColors(window, Colors.White);
                    break;
                case 2: // 系统默认
                    rootElement.RequestedTheme = ElementTheme.Default;
                    window.ExtendsContentIntoTitleBar = true;
                    ApplySystemThemeToCaptionButtons(window);
                    break;
            }
        }
    }
    private static void SetCaptionButtonColors(Window window, Windows.UI.Color color)
    {
        var res = Application.Current.Resources;
        res["WindowCaptionForeground"] = color;
        window.AppWindow.TitleBar.ButtonForegroundColor = color;
    }

    // Fix for CS1061: Ensure the App class contains a method named GetRootFrame or replace the call with an appropriate alternative.

    private void ApplySystemThemeToCaptionButtons(Window window)
    {
        // Replace the problematic line with a safer alternative or ensure the App class has the required method.
        var frame = window.Content as FrameworkElement; // Use the window's content directly instead of relying on a non-existent method.
        Windows.UI.Color color;

        if (frame?.ActualTheme == ElementTheme.Dark)
        {
            color = Colors.White;
        }
        else
        {
            color = Colors.Black;
        }

        SetCaptionButtonColors(window, color);
    }

    private void LoadSystemFonts()
    {
        // 优先使用 _selectedFontFamily，如果为空则从本地设置读取
        var localSettings = ApplicationData.Current.LocalSettings;
        string? savedFontFamily = !string.IsNullOrEmpty(_selectedFontFamily)
            ? _selectedFontFamily
            : (localSettings.Values.TryGetValue(FontSettingKey, out object? fontName) && fontName is string fontFamilyName
                ? fontFamilyName
                : null);

        var fontFamilies = CanvasTextFormat.GetSystemFontFamilies().OrderBy(f => f).ToList();

        SettingsTextFontComboBox.Items.Clear();
        int selectedIndex = 0;
        for (int i = 0; i < fontFamilies.Count; i++)
        {
            var fontFamily = fontFamilies[i];
            var comboBoxItem = new ComboBoxItem
            {
                Content = fontFamily,
                FontFamily = new Microsoft.UI.Xaml.Media.FontFamily(fontFamily)
            };
            SettingsTextFontComboBox.Items.Add(comboBoxItem);

            // 如果找到已保存的字体名，记录其索引
            if (!string.IsNullOrEmpty(savedFontFamily) && fontFamily == savedFontFamily)
            {
                selectedIndex = i;
            }
        }

        // 设置选中项
        if (SettingsTextFontComboBox.Items.Count > 0)
            SettingsTextFontComboBox.SelectedIndex = selectedIndex;
    }

    private void SettingsTextFontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SettingsTextFontComboBox.SelectedItem is ComboBoxItem item && item.Content != null)
        {
            _selectedFontFamily = item.Content.ToString()!;
            ApplicationData.Current.LocalSettings.Values[FontSettingKey] = _selectedFontFamily;

            // 动态更新全局字体资源
            var newFontFamily = new FontFamily(_selectedFontFamily);
            Application.Current.Resources[FontSettingKey] = newFontFamily;

            SettingsFontInfoBar.IsOpen = true;
        }
    }

}
