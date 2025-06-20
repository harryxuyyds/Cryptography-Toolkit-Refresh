using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

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
        PrngSetup();
        PrngSeedTextBox.TextChanged += PrngTextBox_OnTextChanged;
        PrngParaATextBox.TextChanged += PrngTextBox_OnTextChanged;
        PrngParaCTextBox.TextChanged += PrngTextBox_OnTextChanged;
        PrngParaMTextBox.TextChanged += PrngTextBox_OnTextChanged;
        PrngGenerateQuantityNumberBox.ValueChanged += PrngNumberBox_OnValueChanged;
    }

    private async void PrngSampleParametersHyperlinkButton_OnClick(object sender, RoutedEventArgs e)
    {
        ContentDialog rngSampleParametersDialog = new ContentDialog
        {
            Title = "Load Sample Parameters?",
            Content = "Sample: rand() function used in ANSI C\rSeed = 12345\rMultiplier = 1103515245\rIncrement = 12345\rModulus = 2^31",
            CloseButtonText = "Cancel",
            PrimaryButtonText = "Confirmed",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.XamlRoot
        };

        ContentDialogResult checkResult = await rngSampleParametersDialog.ShowAsync();
        if (checkResult == ContentDialogResult.Primary)
        {
            PrngSeedTextBox.Text = "12345";
            PrngParaATextBox.Text = "1103515245";
            PrngParaCTextBox.Text = "12345";
            PrngParaMTextBox.Text = "2147483648"; // 2^31
        }
    }

    private void PrngNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        PrngSetup();
    }

    private void PrngTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        PrngSetup();
    }

    private void PrngSetup()
    {
        if (double.TryParse(PrngSeedTextBox.Text, out double seedValue) &&
            double.TryParse(PrngParaATextBox.Text, out double multiplierValue) &&
            double.TryParse(PrngParaCTextBox.Text, out double incrementValue) &&
            double.TryParse(PrngParaMTextBox.Text, out double modulusValue) &&
            PrngGenerateQuantityNumberBox.Value is double quantityValue)
        {
            var seed = (long)seedValue;
            var multiplier = (long)multiplierValue;
            var increment = (long)incrementValue;
            var modulus = (long)modulusValue;
            var quantity = (long)quantityValue;

            if (seed < 0 || multiplier <= 0 || increment < 0 || modulus <= 0 || quantity <= 0)
            {
                PrngLogTextBox.Text = "Invalid parameters. Please ensure all parameters are positive and non-zero where applicable.";
                return;
            }

            // 可根据需要进一步处理这些值
            var randomNumberGenerator = new Components.RandomNumberGenerator();

            var randomNumberOutput =
                randomNumberGenerator.PseudorandomNumberGenerator(seed, multiplier, increment, modulus, quantity);

            var lines = randomNumberOutput.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            var maxIndexWidth = lines.Length.ToString().Length;
            PrngLogTextBox.Text = string.Join(
                Environment.NewLine,
                lines.Select((line, idx) => $"{(idx + 1).ToString().PadLeft(maxIndexWidth)}: {line}")
            );
        }
        else
        {
            PrngLogTextBox.Text = "Invalid parameters. Please ensure all parameters are positive and non-zero where applicable.";
        }
    }
}
