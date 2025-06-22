using System;
using System.Collections.Generic;
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
}
