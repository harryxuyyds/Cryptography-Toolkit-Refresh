using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
public sealed partial class DLPWithECPage : Page
{
    private readonly Common _common;

    public DLPWithECPage()
    {
        InitializeComponent();
        _common = new Common();
        DefinitionEcCoefficientANumberBox.ValueChanged += DefinitionEcNumberBox_OnValueChanged;
        DefinitionEcCoefficientBNumberBox.ValueChanged += DefinitionEcNumberBox_OnValueChanged;
        DefinitionEcFieldModulusNumberBox.ValueChanged += DefinitionEcNumberBox_OnValueChanged;
        DefinitionEcSetup();
    }

    private void DefinitionEcNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        DefinitionEcSetup();
    }

    private void DefinitionEcSetup()
    {
        var millerRabinPrimalityTest = new Components.MillerRabinPrimalityTest();

        if (DefinitionEcCoefficientANumberBox.Value is double ecCoeffAValue &&
            DefinitionEcCoefficientBNumberBox.Value is double ecCoeffBValue &&
            DefinitionEcFieldModulusNumberBox.Value is double ecModulusPValue &&
            millerRabinPrimalityTest.MillerRabinPrimalityTestRun(8, (int)ecModulusPValue))
        {
            var ecCoeffA = (int)ecCoeffAValue;
            var ecCoeffB = (int)ecCoeffBValue;
            var ecModulusP = (int)ecModulusPValue;

            var check = (4 * (int)Math.Pow(ecCoeffA, 3) + 27 * (int)Math.Pow(ecCoeffB, 2)) % ecModulusP;
            if (check != 0)
            {
                DefinitionEcCheckInfoBar.Severity = InfoBarSeverity.Success;
                DefinitionEcCheckInfoBar.Message = $"Valid Curve (4a³ + 27b² ≡ {check} ≢ 0 mod p)​";
                LoadEllipticCurvePreview(ecCoeffA, ecCoeffB, ecModulusP);
            }
            else
            {
                DefinitionEcCheckInfoBar.Severity = InfoBarSeverity.Error;
                DefinitionEcCheckInfoBar.Message = "Invalid Curve (4a³ + 27b² ≡ 0 mod p)​";
            }
        }
        else
        {
            DefinitionEcCheckInfoBar.Severity = InfoBarSeverity.Warning;
            DefinitionEcCheckInfoBar.Message = "Please enter correct parameters.";
        }
    }

    private void LoadEllipticCurvePreview(int coeffA, int coeffB, int modulusP)
    {
        var orderCount = 1;
        var points = new List<(int x, int y)>();

        for (int x = 0; x < modulusP; x++)
        {
            int rhs = (int)((((long)x * x % modulusP) * x % modulusP + coeffA * x % modulusP + coeffB) % modulusP);
            for (int y = 0; y < modulusP; y++)
            {
                if ((y * y) % modulusP == rhs)
                {
                    // 保存点坐标到列表
                    points.Add((x, y));
                    orderCount++;
                }
            }
        }

        DefinitionEcOrderTextBlock.Text = orderCount.ToString();

        PointPComboBox.Items.Clear();
        foreach (var (x, y) in points)
        {
            var display = $"({x}, {y})";
            PointPComboBox.Items.Add(display);
        }

        PointPComboBox.SelectionChanged += PointComboBox_OnSelectionChanged;
    }

    private void PointComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        PointMultiplicationSetup();
    }

    private void ScalarKNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        PointMultiplicationSetup();
    }

    private void PointMultiplicationSetup()
    {

    }
}
