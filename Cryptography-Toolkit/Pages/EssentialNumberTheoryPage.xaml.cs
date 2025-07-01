using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

using Cryptography_Toolkit.Helpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cryptography_Toolkit.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class EssentialNumberTheoryPage : Page
{
    private readonly Common _common;

    public EssentialNumberTheoryPage()
    {
        InitializeComponent();
        _common = new Common();
        EuclideanAlgorithmParaR0NumberBox.ValueChanged += EuclideanAlgorithmParaNumberBox_OnValueChanged;
        EuclideanAlgorithmParaR1NumberBox.ValueChanged += EuclideanAlgorithmParaNumberBox_OnValueChanged;
        ExtendedEuclideanAlgorithmParaR0NumberBox.ValueChanged += ExtendedEuclideanAlgorithmParaNumberBox_OnValueChanged;
        ExtendedEuclideanAlgorithmParaR1NumberBox.ValueChanged += ExtendedEuclideanAlgorithmParaNumberBox_OnValueChanged;
        EulerPhiFunctionParaMNumberBox.ValueChanged += EulerPhiFunctionParaMNumberBox_OnValueChanged;
        SquareMultiplyAlgorithmBaseElementNumberBox.ValueChanged += SquareMultiplyAlgorithmNumberBox_OnValueChanged;
        SquareMultiplyAlgorithmExponentNumberBox.ValueChanged += SquareMultiplyAlgorithmNumberBox_OnValueChanged;
        SquareMultiplyAlgorithmModulusNumberBox.ValueChanged += SquareMultiplyAlgorithmNumberBox_OnValueChanged;
    }

    private void EuclideanAlgorithmParaR0NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        throw new NotImplementedException();
    }

    private void EuclideanAlgorithmLogTextBoxReset()
    {
        EuclideanAlgorithmLogTextBox.Text =
            "The running log generated during the euclidean algorithm will be displayed here.";
    }

    private void ExtendedEuclideanAlgorithmLogTextBoxReset()
    {
        ExtendedEuclideanAlgorithmLogTextBox.Text =
            "The running log generated during the extended euclidean algorithm will be displayed here.";
    }

    private void EuclideanAlgorithmExecuteButton_OnClick(object sender, RoutedEventArgs e)
    {
        // 获取输入框的值
        if (int.TryParse(EuclideanAlgorithmParaR0NumberBox.Text, out int r0) &&
            int.TryParse(EuclideanAlgorithmParaR1NumberBox.Text, out int r1))
        {
            int gcd = _common.EuclideanAlgorithmCalc(r0, r1);
            EuclideanAlgorithmLogTextBox.Text = $"GCD({r0}, {r1}) = {gcd}";
        }
        else
        {
            EuclideanAlgorithmLogTextBox.Text = "Please enter correct parameters.";
        }
    }

    private void EuclideanAlgorithmParaNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        EuclideanAlgorithmLogTextBoxReset();
    }

    private void ExtendedEuclideanAlgorithmExecuteButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(ExtendedEuclideanAlgorithmParaR0NumberBox.Text, out int r0) &&
            int.TryParse(ExtendedEuclideanAlgorithmParaR1NumberBox.Text, out int r1))
        {
            (int gcd, int s, int t) = _common.ExtendedEuclideanAlgorithmCalc(r0, r1);
            string inverseStatement = "";
            if (gcd == 1)
            {
                inverseStatement = $"Since gcd({r0}, {r1}) = 1, t = {t} is the inverse of {r1} modulo {r0}\n";
            }
            ExtendedEuclideanAlgorithmLogTextBox.Text =
                $"GCD({r0}, {r1}) = {gcd}\n" +
                $"Coefficients of Bézout's identity: s = {s}, t = {t}\n" +
                $"Verification: {gcd} = {r0} * {s} + {r1} * {t}\n" +
                inverseStatement;
        }
        else
        {
            ExtendedEuclideanAlgorithmLogTextBox.Text = "Please enter correct parameters.";
        }
    }

    private void ExtendedEuclideanAlgorithmParaNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        ExtendedEuclideanAlgorithmLogTextBoxReset();
    }

    private void EulerPhiFunctionExecuteButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(EulerPhiFunctionParaMNumberBox.Text, out int m))
        {
            int eulerPhiResult = _common.EulerPhiFunctionCalc(m);
            EulerPhiFunctionLogTextBox.Text = $"φ({m}) = {eulerPhiResult}";
        }
        else
        {
            EulerPhiFunctionLogTextBox.Text = "Please enter correct parameters.";
        }
    }

    private void EulerPhiFunctionParaMNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        EulerPhiFunctionLogTextBox.Text = "The result of the Euler's phi function will be displayed here.";
    }

    private void SquareMultiplyAlgorithmExecuteButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(SquareMultiplyAlgorithmBaseElementNumberBox.Text, out var baseElement) &&
            int.TryParse(SquareMultiplyAlgorithmExponentNumberBox.Text, out var exponent) &&
             int.TryParse(SquareMultiplyAlgorithmModulusNumberBox.Text, out var modulus))
        {
            var squareMultiplyResult = _common.SquareMultiplyAlgorithmCalc(baseElement, exponent, modulus);
            SquareMultiplyAlgorithmLogTextBox.Text = $"Result: {baseElement} ^ {exponent} mod {modulus} = {squareMultiplyResult}\n";
        }
        else
        {
            SquareMultiplyAlgorithmLogTextBox.Text = "Please enter correct parameters.";
        }
    }

    private void SquareMultiplyAlgorithmNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        SquareMultiplyAlgorithmLogTextBox.Text =
            "The running log generated during the square-and-multiply algorithm will be displayed here.";
    }

    private void ChineseRemainderTheoremExecuteButton_OnClick(object sender, RoutedEventArgs e)
    {
        // Parse moduli and remainders from TextBoxes
        var moduliText = ChineseRemainderTheoremModuliTextBox.Text;
        var remaindersText = ChineseRemainderTheoremRemaindersTextBox.Text;

        try
        {
            var moduli = moduliText.Split([',', ' ', ';'], StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToArray();
            var remainders = remaindersText.Split([',', ' ', ';'], StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToArray();

            if (moduli.Length != remainders.Length || moduli.Length == 0)
            {
                ChineseRemainderTheoremLogTextBox.Text = "Please enter the same number of moduli and remainders.";
                return;
            }

            // Check pairwise coprime
            for (int i = 0; i < moduli.Length; i++)
            {
                for (int j = i + 1; j < moduli.Length; j++)
                {
                    if (_common.EuclideanAlgorithmCalc(moduli[i], moduli[j]) != 1)
                    {
                        ChineseRemainderTheoremLogTextBox.Text = "Moduli must be pairwise coprime.";
                        return;
                    }
                }
            }

            // Chinese Remainder Theorem calculation with detailed steps
            long M = 1;
            foreach (var m in moduli) M *= m;

            var log = $"Step 1: Product of all moduli, M = {string.Join(" * ", moduli)} = {M}\n\n";

            // Build table header
            log += "Step 2: Calculation Table\n";
            log += "| Index | Modulus m[i] | Remainder a[i] |   Mi = M/m[i]   | Inverse of Mi mod m[i] |   Term = a[i] * Mi * inverse  |\n";
            log += "|-------|--------------|----------------|-----------------|------------------------|-------------------------------|\n";

            long result = 0;
            for (int i = 0; i < moduli.Length; i++)
            {
                long Mi = M / moduli[i];
                var (gcd, _, ti) = _common.ExtendedEuclideanAlgorithmCalc(moduli[i], (int)Mi);
                long inverse = ti;
                if (inverse < 0) inverse += moduli[i];
                long term = remainders[i] * Mi * inverse;

                log += $"| {i,-5} | {moduli[i],-12} | {remainders[i],-14} | {Mi,-15} | {inverse,-22} | {term,-29} |\n";
                result += term;
            }
            log += "\nStep 3: Sum all terms and take modulo M:\n";
            log += $"  Raw result = {result}\n";
            result = ((result % M) + M) % M;
            log += $"  Final result: x ≡ {result} mod {M}\n\n";
            log += $"That is, x = {result} (mod {M})";

            ChineseRemainderTheoremLogTextBox.Text = log;
        }
        catch
        {
            ChineseRemainderTheoremLogTextBox.Text = "Please enter valid integers separated by commas, spaces, or semicolons.";
        }
    }
}
