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
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cryptography_Toolkit.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class GroupOperationsECPage : Page
{
    private readonly Common _common;

    public GroupOperationsECPage()
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
        PointQComboBox.Items.Clear();
        foreach (var (x, y) in points)
        {
            var display = $"({x}, {y})";
            PointPComboBox.Items.Add(display);
            PointQComboBox.Items.Add(display);
        }

        PointPComboBox.SelectionChanged += PointComboBox_OnSelectionChanged;
        PointQComboBox.SelectionChanged += PointComboBox_OnSelectionChanged;
    }

    private void PointComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PointPComboBox.SelectedItem is string pStr && PointQComboBox.SelectedItem is string qStr)
        {
            // Parse (x, y) from string
            var pParts = pStr.Trim('(', ')').Split(',');
            var qParts = qStr.Trim('(', ')').Split(',');
            if (pParts.Length == 2 && qParts.Length == 2 &&
                int.TryParse(pParts[0], out int px) && int.TryParse(pParts[1], out int py) &&
                int.TryParse(qParts[0], out int qx) && int.TryParse(qParts[1], out int qy))
            {
                // Get curve parameters
                int a = (int)DefinitionEcCoefficientANumberBox.Value;
                int p = (int)DefinitionEcFieldModulusNumberBox.Value;

                string result = string.Empty;

                // Point doubling
                if (py == 0)
                {
                    result = "Doubling not defined (y = 0, point at infinity).";
                }
                else
                {
                    // λ = (3x^2 + a) / (2y) mod p
                    int numerator = (3 * px * px + a) % p;
                    int denominator = (2 * py) % p;
                    int invDenominator = ModInverse(denominator, p);
                    if (invDenominator == -1)
                    {
                        result = "Doubling not defined (no modular inverse for denominator).";
                    }
                    else
                    {
                        int lambda = (numerator * invDenominator) % p;
                        int rx = (lambda * lambda - 2 * px) % p;
                        int ry = (lambda * (px - rx) - py) % p;
                        rx = (rx + p) % p;
                        ry = (ry + p) % p;
                        result = $"2P = ({rx}, {ry})";
                    }
                }

                PointDoublingResultTextBlock.Text = result;

                // Point addition
                if (px == qx && (py + qy) % p == 0)
                {
                    result = "Addition not defined (P + Q = point at infinity).";
                }
                else
                {
                    // λ = (y2 - y1) / (x2 - x1) mod p
                    int numerator = (qy - py + p) % p;
                    int denominator = (qx - px + p) % p;
                    int invDenominator = ModInverse(denominator, p);
                    if (invDenominator == -1)
                    {
                        result = "Addition not defined (no modular inverse for denominator).";
                    }
                    else
                    {
                        int lambda = (numerator * invDenominator) % p;
                        int rx = (lambda * lambda - px - qx) % p;
                        int ry = (lambda * (px - rx) - py) % p;
                        rx = (rx + p) % p;
                        ry = (ry + p) % p;
                        result = $"P + Q = ({rx}, {ry})";
                    }
                }

                PointAdditionResultTextBlock.Text = result;
            }
        }
    }

    private int ModInverse(int a, int p)
    {
        int t = 0, newt = 1;
        int r = p, newr = a;
        while (newr != 0)
        {
            int quotient = r / newr;
            (t, newt) = (newt, t - quotient * newt);
            (r, newr) = (newr, r - quotient * newr);
        }
        if (r > 1) return -1; // No inverse
        if (t < 0) t += p;
        return t;
    }
}
