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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cryptography_Toolkit.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class RandomNumberGeneratorsPage : Page
{
    public RandomNumberGeneratorsPage()
    {
        InitializeComponent();
    }

    private async void RngSampleParameters_OnClick(object sender, RoutedEventArgs e)
    {
        ContentDialog rngSampleParametersDialog = new ContentDialog
        {
            Title = "Load Sample Parameters?",
            Content = "Sample: rand() function used in ANSI C\rParameters seed = 12345\rParameters a = 1103515245\rParameters b = 12345\rParameters m = 2^31",
            CloseButtonText = "Cancel",
            PrimaryButtonText = "Confirmed",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.XamlRoot
        };

        ContentDialogResult checkResult = await rngSampleParametersDialog.ShowAsync();
        if (checkResult == ContentDialogResult.Primary)
        {
            // AES_Encrypt_Calc(AES_Plaintext_Line, AES_Key_Line);
        }
    }
}
