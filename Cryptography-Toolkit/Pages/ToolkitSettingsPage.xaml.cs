using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.UI.Text;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

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
        LoadAiIntegrationToggleSetting();
    }

    private void LoadAiIntegrationToggleSetting()
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        // var aiIntegrationSettings = localSettings.Values["AiIntegrationEnabled"];
        if (localSettings.Values.TryGetValue("AiIntegrationEnabled", out object? aiIntegrationSettings) && (bool)aiIntegrationSettings)
        {
            SettingsAiIntegrationToggleSwitch.IsOn = true;
        }
        else
        {
            SettingsAiIntegrationToggleSwitch.IsOn = false;
        }
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
            var appWindow = AppWindow.GetFromWindowId(window.AppWindow.Id);
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

    private void ApplySystemThemeToCaptionButtons(Window window)
    {
        var frame = window.Content as FrameworkElement;
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
        var localSettings = ApplicationData.Current.LocalSettings;
        string? savedFontFamily = !string.IsNullOrEmpty(_selectedFontFamily)
            ? _selectedFontFamily
            : (localSettings.Values.TryGetValue(FontSettingKey, out object? fontName) && fontName is string fontFamilyName
                ? fontFamilyName
                : "Consolas");

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

            var newFontFamily = new FontFamily(_selectedFontFamily);
            Application.Current.Resources[FontSettingKey] = newFontFamily;

            SettingsFontInfoBar.IsOpen = true;
        }
    }

    private async void SettingsDeepSeekApiSettingsButton_OnClick(object sender, RoutedEventArgs e)
    {
        var apiKey = ApplicationData.Current.LocalSettings.Values.TryGetValue("DeepSeekApiKey", out object? value) && value is string s ? s : string.Empty;

        var apiKeyTextBox = new TextBox
        {
            Name = "SettingsDeepSeekApiTextBox",
            Header = "DeepSeek API Key",
            Text = apiKey
        };

        var dialog = new ContentDialog
        {
            Title = "DeepSeek API Settings",
            XamlRoot = this.XamlRoot,
            CloseButtonText = "Cancel",
            PrimaryButtonText = "Confirmed",
            Content = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Spacing = 12,
                Children =
                {
                    new Image
                    {
                        Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/AIFeaturesSample.png")),
                        Height = 300,
                        HorizontalAlignment = HorizontalAlignment.Center
                    },
                    new TextBlock
                    {
                        Text = "Enable Advanced AI Integration to unlock multiple AI features including AI-powered search, expanding the breadth and depth of this toolkit application. Learn more about DeepSeek usage and privacy terms.",
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 4, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Center
                    },
                    new TextBlock
                    {
                        Text = "Configure DeepSeek API Key",
                        FontWeight = FontWeights.Bold,
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 4, 0, 0),
                    },
                    new TextBlock
                    {
                        Text = " - Log in to your DeepSeek Open Platform,\n - Create a new API key, and paste it in the field below,\n - Note: Your DeepSeek account must have active paid credits.",
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 0, 0, 4),
                    },
                    apiKeyTextBox
                }
            }
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            var apiKeyTemp = apiKeyTextBox.Text;
            var apiCheck = await AiPlatformHelper.ApiConnectivityTest(apiKeyTemp);

            if (apiKeyTemp == "" && apiCheck != 0)
            {
                AppNotification notification = new AppNotificationBuilder()
                    .AddText("DeepSeek API Removed")
                    .AddText("Your DeepSeek API key has been successfully removed. All AI features requiring API access will be disabled until a new valid key is provided.")
                    .BuildNotification();

                AppNotificationManager.Default.Show(notification);

                ApplicationData.Current.LocalSettings.Values["DeepSeekApiKey"] = "";
            }
            else if (apiCheck == 0)
            {
                AppNotification notification = new AppNotificationBuilder()
                    .AddText("DeepSeek API Connected")
                    .AddText("DeepSeek API configured successfully. Advanced AI features enabled.")
                    .BuildNotification();

                AppNotificationManager.Default.Show(notification);

                // 保存API Key到应用设置
                ApplicationData.Current.LocalSettings.Values["DeepSeekApiKey"] = apiKeyTemp;
            }
            else
            {
                AppNotification notification = new AppNotificationBuilder()
                    .AddText("DeepSeek API Connection Failed")
                    .AddText("Failed to connect to DeepSeek platform. Please verify your API key.")
                    .BuildNotification();

                AppNotificationManager.Default.Show(notification);
            }
        }
    }

    private void SettingsAiIntegrationToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        bool isOn = SettingsAiIntegrationToggleSwitch.IsOn;
        ApplicationData.Current.LocalSettings.Values["AiIntegrationEnabled"] = isOn;
    }
}
