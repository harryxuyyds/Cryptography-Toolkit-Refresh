using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Cryptography_Toolkit.Components;
using Cryptography_Toolkit.Helpers;
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
public sealed partial class QuadraticResiduePage : Page
{
    private readonly Common _common;

    public QuadraticResiduePage()
    {
        InitializeComponent();
        _common = new Common();
        LegendreCalcIntegerANumberBox.ValueChanged += LegendreCalcNumberBox_OnValueChanged;
        LegendreCalcModulusPNumberBox.ValueChanged += LegendreCalcNumberBox_OnValueChanged;
        LegendreCalcSetup();
    }

    private void LegendreCalcNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        LegendreCalcSetup();
    }

    private void LegendreCalcSetup()
    {
        var millerRabinPrimalityTest = new Components.MillerRabinPrimalityTest();

        if (LegendreCalcIntegerANumberBox.Value is double paraAValue &&
            LegendreCalcModulusPNumberBox.Value is double modulusPValue &&
            millerRabinPrimalityTest.MillerRabinPrimalityTestRun(8, (int)modulusPValue))
        {
            var paraA = (int)paraAValue;
            var modulusP = (int)modulusPValue;

            var legendreResult = _common.SquareMultiplyAlgorithmCalc(paraA, (modulusP - 1) / 2, modulusP);
            if (legendreResult == modulusP - 1)
                legendreResult -= modulusP;
            LegendreCalcResultTextBlock.Text = legendreResult.ToString();
            LegendreCalcCheckInfoBar.Severity = InfoBarSeverity.Success;
            if (legendreResult == 0)
                LegendreCalcCheckInfoBar.Message = $"{paraA} is divisible by {modulusP} ({paraA} ≡ 0 mod {modulusP}).";
            else if (legendreResult == 1)
                LegendreCalcCheckInfoBar.Message = $"{paraA} is a quadratic residue modulo {modulusP} (∃x: x² ≡ {paraA} mod {modulusP}).";
            else
                LegendreCalcCheckInfoBar.Message = $"{paraA} is a quadratic non-residue modulo {modulusP}.";
        }
        else
        {
            LegendreCalcCheckInfoBar.Severity = InfoBarSeverity.Warning;
            LegendreCalcCheckInfoBar.Message = "Please enter correct parameters.";
            LegendreCalcResultTextBlock.Text = "";
        }
    }
}
