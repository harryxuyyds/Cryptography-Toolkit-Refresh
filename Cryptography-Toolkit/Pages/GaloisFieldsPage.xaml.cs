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
public sealed partial class GaloisFieldsPage : Page
{
    public GaloisFieldsPage()
    {
        InitializeComponent();
    }

    private void GfAdditionElementTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        string input1 = GfAdditionElement1TextBox.Text.Trim();
        string input2 = GfAdditionElement2TextBox.Text.Trim();

        bool IsValidHex(string s) =>
            s.Length == 2 && int.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out _);

        if (IsValidHex(input1) && IsValidHex(input2))
        {
            int value1 = Convert.ToInt32(input1, 16);
            int value2 = Convert.ToInt32(input2, 16);

            GfAdditionResultHyperlinkButton.Content = (value1 ^ value2).ToString("X2");
            GfAdditionStatusInfoBar.Severity = InfoBarSeverity.Success;
            GfAdditionStatusInfoBar.Message = "Computation successful.";
        }
        else
        {
            GfAdditionStatusInfoBar.Severity = InfoBarSeverity.Warning;
            GfAdditionStatusInfoBar.Message = "Please enter valid 2-digit hexadecimal values.";
        }
    }

    private void GfMultiplicationElementTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var galoisFields = new Components.GaloisFields();

        string input1 = GfMultiplicationElement1TextBox.Text.Trim();
        string input2 = GfMultiplicationElement2TextBox.Text.Trim();

        bool IsValidHex(string s) =>
            s.Length == 2 && int.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out _);

        if (IsValidHex(input1) && IsValidHex(input2))
        {
            int value1 = Convert.ToInt32(input1, 16);
            int value2 = Convert.ToInt32(input2, 16);

            string xTimePara1 = Convert.ToString(value1, 2).PadLeft(8, '0');
            string xTimePara2 = Convert.ToString(value2, 2).PadLeft(8, '0');
            string result = galoisFields.GFMultiplicationCalc(xTimePara1, xTimePara2);
            GfMultiplicationResultHyperlinkButton.Content = result;

            GfMultiplicationStatusInfoBar.Severity = InfoBarSeverity.Success;
            GfMultiplicationStatusInfoBar.Message = "Computation successful.";
        }
        else
        {
            GfMultiplicationStatusInfoBar.Severity = InfoBarSeverity.Warning;
            GfMultiplicationStatusInfoBar.Message = "Please enter valid 2-digit hexadecimal values.";
        }
    }

    private void GfInversionElementTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var galoisFields = new Components.GaloisFields();

        var input1 = GfInversionElement1TextBox.Text.Trim();

        bool IsValidHex(string s) =>
            s.Length == 2 && int.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out _);

        if (IsValidHex(input1))
        {
            string valueInv = galoisFields.GFInversionCalc(input1);

            GfInversionResultHyperlinkButton.Content = valueInv;

            GfInversionStatusInfoBar.Severity = InfoBarSeverity.Success;
            GfInversionStatusInfoBar.Message = "Computation successful.";
        }
        else
        {
            GfInversionStatusInfoBar.Severity = InfoBarSeverity.Warning;
            GfInversionStatusInfoBar.Message = "Please enter valid 2-digit hexadecimal values.";
        }
    }
}
