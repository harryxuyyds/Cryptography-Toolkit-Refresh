using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OtpNet;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cryptography_Toolkit.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class HayiTwoFactorAuthPage : Page
{
    public HayiTwoFactorAuthPage()
    {
        InitializeComponent();
        AuthAppAccountNameTextBox.TextChanged += AuthAppAddNewAuthenticatorTextBox_OnTextChanged;
        AuthAppSecretKeyTextBox.TextChanged += AuthAppAddNewAuthenticatorTextBox_OnTextChanged;
        LoadAuthenticatorFile();
    }

    private void TestTwoFactor()
    {
        byte[] secretKey = Base32Encoding.ToBytes("7J64V3P3E77J3LKNUGSZ5QANTLRLTKVL");
        var totp = new Totp(secretKey);
        var code = totp.ComputeTotp(DateTime.UtcNow);
        var remainingSeconds = totp.RemainingSeconds(DateTime.UtcNow);
    }

    private void AuthAppAddNewAuthenticatorButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (AuthAppAddNewAuthenticatorStatusInfoBar.Severity == InfoBarSeverity.Warning)
        {
            AuthAppAddNewAuthenticatorStatusInfoBar.Severity = InfoBarSeverity.Error;
            return;
        }

        var accountName = AuthAppAccountNameTextBox.Text;
        var secretKey = AuthAppSecretKeyTextBox.Text;

        AddNewAuthenticatorItemRun(accountName, secretKey);
    }

    private void AuthAppAddNewAuthenticatorTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var accountName = AuthAppAccountNameTextBox.Text;
        var secretKey = AuthAppSecretKeyTextBox.Text;

        if (string.IsNullOrWhiteSpace(accountName))
        {
            AuthAppAddNewAuthenticatorStatusInfoBar.Severity = InfoBarSeverity.Warning;
            AuthAppAddNewAuthenticatorStatusInfoBar.Message = "Account name cannot be empty.";
            AuthAppAddNewAuthenticatorStatusInfoBar.IsOpen = true;
            return;
        }
        else if (string.IsNullOrWhiteSpace(secretKey))
        {
            AuthAppAddNewAuthenticatorStatusInfoBar.Severity = InfoBarSeverity.Warning;
            AuthAppAddNewAuthenticatorStatusInfoBar.Message = "Secret key cannot be empty.";
            AuthAppAddNewAuthenticatorStatusInfoBar.IsOpen = true;
            return;
        }
        else if (!secretKey.All(c => "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".Contains(char.ToUpperInvariant(c))))
        {
            AuthAppAddNewAuthenticatorStatusInfoBar.Severity = InfoBarSeverity.Warning;
            AuthAppAddNewAuthenticatorStatusInfoBar.Message = "Secret key must be valid Base32 characters (A-Z, 2-7).";
            AuthAppAddNewAuthenticatorStatusInfoBar.IsOpen = true;
            return;
        }
        else
        {
            AuthAppAddNewAuthenticatorStatusInfoBar.Severity = InfoBarSeverity.Informational;
            AuthAppAddNewAuthenticatorStatusInfoBar.Message = "Ready for adding new authenticator.";
            AuthAppAddNewAuthenticatorStatusInfoBar.IsOpen = true;
        }
    }

    private async void LoadAuthenticatorFile()
    {
        var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        var fileName = "hayi_auth_appdata.txt";
        var file = await localFolder.TryGetItemAsync(fileName) as Windows.Storage.StorageFile;
        if (file == null)
            return;

        var lines = await Windows.Storage.FileIO.ReadLinesAsync(file);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = line.Split('|');
            if (parts.Length == 2)
            {
                var accountName = parts[0].Trim();
                var secretKey = parts[1].Trim();
                if (!string.IsNullOrEmpty(accountName) && !string.IsNullOrEmpty(secretKey))
                {
                    AddLocalAuthenticatorItemRun(accountName, secretKey);
                }
            }
        }
    }

    private async void AddNewAuthenticatorItemRun(string accountName, string secretKey)
    {
        // 1. ±£´æµ½AppData
        var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        var fileName = "hayi_auth_appdata.txt";
        var line = $"{accountName}|{secretKey}{Environment.NewLine}";
        Windows.Storage.StorageFile file;
        if (await localFolder.TryGetItemAsync(fileName) == null)
        {
            file = await localFolder.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
        }
        else
        {
            file = await localFolder.GetFileAsync(fileName);
        }
        await Windows.Storage.FileIO.AppendTextAsync(file, line);

        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 4, 0, 4),
            VerticalAlignment = VerticalAlignment.Center
        };
        var nameText = new TextBlock
        {
            Text = accountName,
            Width = 160,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 14,
            Margin = new Thickness(0, 0, 8, 0)
        };
        var codeText = new TextBlock
        {
            Width = 90,
            FontSize = 22,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 8, 0)
        };
        var progressBar = new ProgressBar
        {
            Width = 100,
            Height = 6,
            Minimum = 0,
            Maximum = 30,
            Margin = new Thickness(0, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center
        };

        stackPanel.Children.Add(nameText);
        stackPanel.Children.Add(codeText);
        stackPanel.Children.Add(progressBar);
        AuthenticatorListPanel.Children.Add(stackPanel);

        byte[] secretBytes = OtpNet.Base32Encoding.ToBytes(secretKey);
        var totp = new OtpNet.Totp(secretBytes);

        async void UpdateTotp()
        {
            while (true)
            {
                var now = DateTime.UtcNow;
                var code = totp.ComputeTotp(now);
                var remain = totp.RemainingSeconds(now);

                await DispatcherQueue.EnqueueAsync(() =>
                {
                    codeText.Text = code;
                    progressBar.Value = 30 - remain;
                });
                await Task.Delay(1000);
            }
        }

        _ = Task.Run(UpdateTotp);

        AuthAppAddNewAuthenticatorStatusInfoBar.Severity = InfoBarSeverity.Success;
        AuthAppAddNewAuthenticatorStatusInfoBar.Message = "Authenticator added successfully.";
        AuthAppAddNewAuthenticatorStatusInfoBar.IsOpen = true;
    }

    private async void AddLocalAuthenticatorItemRun(string accountName, string secretKey)
    {
        var stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 8, 0, 8) };
        var nameText = new TextBlock { Text = accountName, Width = 180, VerticalAlignment = VerticalAlignment.Center };
        var codeText = new TextBlock { Width = 100, FontSize = 20, VerticalAlignment = VerticalAlignment.Center };
        var progressBar = new ProgressBar { Width = 120, Height = 8, Minimum = 0, Maximum = 30, Margin = new Thickness(12, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };

        stackPanel.Children.Add(nameText);
        stackPanel.Children.Add(codeText);
        stackPanel.Children.Add(progressBar);
        AuthenticatorListPanel.Children.Add(stackPanel);

        byte[] secretBytes = OtpNet.Base32Encoding.ToBytes(secretKey);
        var totp = new OtpNet.Totp(secretBytes);

        async void UpdateTotp()
        {
            while (true)
            {
                var now = DateTime.UtcNow;
                var code = totp.ComputeTotp(now);
                var remain = totp.RemainingSeconds(now);

                await DispatcherQueue.EnqueueAsync(() =>
                {
                    codeText.Text = code;
                    progressBar.Value = 30 - remain;
                });
                await Task.Delay(1000);
            }
        }

        _ = Task.Run(UpdateTotp);

        AuthAppAddNewAuthenticatorStatusInfoBar.Severity = InfoBarSeverity.Success;
        AuthAppAddNewAuthenticatorStatusInfoBar.Message = "Authenticator added successfully.";
        AuthAppAddNewAuthenticatorStatusInfoBar.IsOpen = true;
    }
}
