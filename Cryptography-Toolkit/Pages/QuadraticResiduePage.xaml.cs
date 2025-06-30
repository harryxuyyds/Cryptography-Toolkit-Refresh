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
        QuadraticCongruenceCalcIntegerANumberBox.ValueChanged += QuadraticCongruenceCalcNumberBox_OnValueChanged;
        QuadraticCongruenceCalcModulusPNumberBox.ValueChanged += QuadraticCongruenceCalcNumberBox_OnValueChanged;
        LegendreCalcSetup();
        QuadraticCongruenceCalc();
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

    private void QuadraticCongruenceCalcNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        QuadraticCongruenceCalc();
    }

    private void QuadraticCongruenceCalc()
    {
        var millerRabinPrimalityTest = new Components.MillerRabinPrimalityTest();

        if (QuadraticCongruenceCalcIntegerANumberBox.Value is double paraAValue &&
            QuadraticCongruenceCalcModulusPNumberBox.Value is double modulusPValue &&
            millerRabinPrimalityTest.MillerRabinPrimalityTestRun(8, (int)modulusPValue) &&
            (int)modulusPValue % 4 == 3)
        {
            var paraA = (int)paraAValue;
            var modulusP = (int)modulusPValue;

            // Calculate Legendre Symbol
            var legendreResult = _common.SquareMultiplyAlgorithmCalc(paraA, (modulusP - 1) / 2, modulusP);
            int legendreSymbol = legendreResult == modulusP - 1 ? -1 : legendreResult;

            // Calculate Explicit Formula (when p ≡ 3 mod 4)
            // x ≡ a^((p+1)/4) mod p
            int expFormula = _common.SquareMultiplyAlgorithmCalc(paraA, (modulusP + 1) / 4, modulusP);

            // Calculate Solutions
            List<int> solutions = new();
            if (legendreSymbol == 1)
            {
                // Two solutions: x and p-x
                solutions.Add(expFormula);
                solutions.Add((modulusP - expFormula) % modulusP);
                QuadraticCongruenceCalcCheckInfoBar.Severity = InfoBarSeverity.Success;
                QuadraticCongruenceCalcCheckInfoBar.Message =
                    $"Solutions: x ≡ {expFormula} mod {modulusP}, x ≡ {(modulusP - expFormula) % modulusP} mod {modulusP}";
            }
            else if (legendreSymbol == 0)
            {
                // Unique solution x ≡ 0 mod p
                solutions.Add(0);
                QuadraticCongruenceCalcCheckInfoBar.Severity = InfoBarSeverity.Success;
                QuadraticCongruenceCalcCheckInfoBar.Message = $"Unique solution: x ≡ 0 mod {modulusP}";
            }
            else
            {
                // No solution
                QuadraticCongruenceCalcCheckInfoBar.Severity = InfoBarSeverity.Warning;
                QuadraticCongruenceCalcCheckInfoBar.Message = $"{paraA} is not a quadratic residue modulo {modulusP}, no solution.";
            }

            // 显示结果
            QuadraticCongruenceCalcSolutionTextBlock.Text = string.Join(", ", solutions);
            QuadraticCongruenceCalcLegendreValueTextBlock.Text = legendreSymbol.ToString();
            QuadraticCongruenceCalcExplicitFormulaTextBlock.Text = expFormula.ToString();
            
        }
        else
        {
            QuadraticCongruenceCalcCheckInfoBar.Severity = InfoBarSeverity.Warning;
            QuadraticCongruenceCalcCheckInfoBar.Message = "Please enter correct parameters.";
            LegendreCalcResultTextBlock.Text = "";
            QuadraticCongruenceCalcSolutionTextBlock.Text = String.Empty;
            QuadraticCongruenceCalcLegendreValueTextBlock.Text = String.Empty;
            QuadraticCongruenceCalcExplicitFormulaTextBlock.Text = String.Empty;
        }
    }
}
