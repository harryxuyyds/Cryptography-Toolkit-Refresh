using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        List<(string kIndex, string doublePx, string doublePy, string? addPx, string? addPy)> pointMultiplicationLog = new();

        if (DefinitionEcCoefficientANumberBox.Value is double ecCoeffAValue &&
            DefinitionEcCoefficientBNumberBox.Value is double ecCoeffBValue &&
            DefinitionEcFieldModulusNumberBox.Value is double ecModulusPValue &&
            ScalarKNumberBox.Value is double kValue &&
            PointPComboBox.SelectedIndex >= 0)
        {
            int a = (int)ecCoeffAValue;
            int b = (int)ecCoeffBValue;
            int p = (int)ecModulusPValue;
            int k = (int)kValue;

            string selectedPoint = (string)PointPComboBox.SelectedItem;
            var trimmed = selectedPoint.Trim('(', ')');
            var parts = trimmed.Split(',');
            int.TryParse(parts[0].Trim(), out int pointGx);
            int.TryParse(parts[1].Trim(), out int pointGy);

            int pointPx = pointGx;
            int pointPy = pointGy;
            string kBin = Convert.ToString(k, 2);
            Debug.WriteLine(kBin);
            int kBinLength = kBin.Length;
            int index = 1;
            for (index = 1; index <= kBinLength; index++)
            {
                if (index == 1)
                {
                    pointMultiplicationLog.Add((index.ToString(), pointGx.ToString(), pointGy.ToString(), null, null));
                }
                else
                {
                    int[] newPointPLocate = PointDoubleCalc(pointPx, pointPy, a, p);
                    pointPx = newPointPLocate[0];
                    pointPy = newPointPLocate[1];
                    if (kBin[index - 1] == '1')
                    {
                        int[] newPointAddPLocate = PointAddCalc(pointGx, pointGy, pointPx, pointPy, a, p);
                        int pointAddPx = newPointAddPLocate[0];
                        int pointAddPy = newPointAddPLocate[1];
                        pointMultiplicationLog.Add((index.ToString(), pointPx.ToString(), pointPy.ToString(), pointAddPx.ToString(), pointAddPy.ToString()));
                        pointPx = pointAddPx;
                        pointPy = pointAddPy;
                    }
                    else
                    {
                        pointMultiplicationLog.Add((index.ToString(), pointPx.ToString(), pointPy.ToString(), null, null));
                    }
                }
            }
            PointMultiplicationResultTextBlock.Text = "( " + pointPx + ", " + pointPy + " )";
            
            var logLines = new List<string>();
            string header =
                $"{"Step",-6} | {"Double Px",-12} | {"Double Py",-12} | {"Add Px (if any)",-16} | {"Add Py (if any)",-16}";
            string separator = new string('-', header.Length);
            logLines.Add(header);
            logLines.Add(separator);
            foreach (var log in pointMultiplicationLog)
            {
                logLines.Add(
                    $"{log.kIndex,-6} | {log.doublePx,-12} | {log.doublePy,-12} | {log.addPx ?? "",-16} | {log.addPy ?? "",-16}");
            }
            PointMultiplicationLogTextBox.Text = string.Join(Environment.NewLine, logLines);
        }
    }

    private int PointSlopeCalc(int pointGx, int pointGy, int pointPx, int pointPy, int a, int p)
    {
        int pointSlope = -1;
        if (pointGx % p != pointPx % p)
        {
            int temp = pointPx - pointGx;
            while (temp < 0)
                temp = (temp + p) % p;
            (int gcd, int s, int t) = _common.ExtendedEuclideanAlgorithmCalc(p, temp);
            pointSlope = (pointPy - pointGy) * t % p;
            while (pointSlope < 0)
                pointSlope = (pointSlope + p) % p;
        }
        else if ((pointGx % p == pointPx % p) & (pointGy % p == pointPy % p))
        {
            (int gcd, int s, int t) = _common.ExtendedEuclideanAlgorithmCalc(p, 2 * pointGy);
            pointSlope = (3 * pointGx * pointGx + a) * t % p;
            while (pointSlope < 0)
                pointSlope = (pointSlope + p) % p;
        }
        return pointSlope;
    }

    private int[] PointDoubleCalc(int pointGx, int pointGy, int a, int p)
    {
        int pointSlope = PointSlopeCalc(pointGx, pointGy, pointGx, pointGy, a, p);
        int newPointX = (pointSlope * pointSlope - 2 * pointGx) % p;
        int newPointY = (pointSlope * (pointGx - newPointX) - pointGy) % p;
        while (newPointX < 0)
            newPointX = (newPointX + p) % p;
        while (newPointY < 0)
            newPointY = (newPointY + p) % p;
        int[] newPointLocate = [newPointX, newPointY];
        return newPointLocate;
    }

    private int[] PointAddCalc(int pointGx, int pointGy, int pointPx, int pointPy, int a, int p)
    {
        int pointSlope = PointSlopeCalc(pointGx, pointGy, pointPx, pointPy, a, p);
        if (pointSlope != -1)
        {
            int newPointX = (pointSlope * pointSlope - pointGx - pointPx) % p;
            int newPointY = (pointSlope * (pointGx - newPointX) - pointGy) % p;
            while (newPointX < 0)
                newPointX = (newPointX + p) % p;
            while (newPointY < 0)
                newPointY = (newPointY + p) % p;
            int[] newPointLocate = [newPointX, newPointY];
            return newPointLocate;
        }
        else
        {
            int[] newPointLocate = [0, 0];
            return newPointLocate;
        }
    }
}
