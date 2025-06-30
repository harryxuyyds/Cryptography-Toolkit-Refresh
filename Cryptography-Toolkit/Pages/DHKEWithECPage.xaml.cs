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
public sealed partial class DHKEWithECPage : Page
{
    private readonly Common _common;

    private int ecCoeffA, ecCoeffB, ecModulusP;

    public DHKEWithECPage()
    {
        InitializeComponent();
        _common = new Common();
        DefinitionEcCoefficientANumberBox.ValueChanged += DefinitionEcNumberBox_OnValueChanged;
        DefinitionEcCoefficientBNumberBox.ValueChanged += DefinitionEcNumberBox_OnValueChanged;
        DefinitionEcFieldModulusNumberBox.ValueChanged += DefinitionEcNumberBox_OnValueChanged;
        DefinitionEcSetup();
        DiffieHellmanPrivateKeyAliceNumberBox.ValueChanged += DiffieHellmanPrivateKeyNumberBox_OnValueChanged;
        DiffieHellmanPrivateKeyBobNumberBox.ValueChanged += DiffieHellmanPrivateKeyNumberBox_OnValueChanged;
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
            ecCoeffA = (int)ecCoeffAValue;
            ecCoeffB = (int)ecCoeffBValue;
            ecModulusP = (int)ecModulusPValue;

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

        PointPComboBox.SelectionChanged += PointPComboBox_OnSelectionChanged;
    }

    private void PointPComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ECDiffieHellmanSetup();
    }

    private void DiffieHellmanPrivateKeyNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        ECDiffieHellmanSetup();
    }

    private void ECDiffieHellmanSetup()
    {
        var ellipticCurveDiffieHellmanKeyExchange = new Components.EllipticCurveDiffieHellmanKeyExchange();
        if (DiffieHellmanPrivateKeyAliceNumberBox.Value is double privateKeyAValue &&
            DiffieHellmanPrivateKeyBobNumberBox.Value is double privateKeyBValue &&
            PointPComboBox.SelectedIndex >= 0)
        {
            var selectedPoint = PointPComboBox.SelectedItem.ToString();
            var trimmed = selectedPoint.Trim('(', ')');
            var parts = trimmed.Split(',');
            int.TryParse(parts[0].Trim(), out int pointGx);
            int.TryParse(parts[1].Trim(), out int pointGy);
            var privateKeyA = (int)privateKeyAValue;
            var privateKeyB = (int)privateKeyBValue;

            var preCheckResult = ellipticCurveDiffieHellmanKeyExchange.KeyExchangePreCheck(pointGx, pointGy, ecCoeffA, ecCoeffB, ecModulusP, privateKeyA, privateKeyB);
            if (preCheckResult == 1)
            {
                DiffieHellmanLogTextBox.Text = "All parameters must be greater than 1.";
                DiffieHellmanStatusInfoBar.Severity = InfoBarSeverity.Warning;
                DiffieHellmanStatusInfoBar.Message = "All parameters must be greater than 1.";
            }
            else if (preCheckResult == 2)
            {
                DiffieHellmanLogTextBox.Text = "Modulus p must be a prime number.";
                DiffieHellmanStatusInfoBar.Severity = InfoBarSeverity.Warning;
                DiffieHellmanStatusInfoBar.Message = "Modulus p must be a prime number.";
            }
            else if (preCheckResult == 3)
            {
                DiffieHellmanLogTextBox.Text = "Point Generator is not on the elliptic curve.";
                DiffieHellmanStatusInfoBar.Severity = InfoBarSeverity.Warning;
                DiffieHellmanStatusInfoBar.Message = "Point Generator is not on the elliptic curve.";
            }
            else if (preCheckResult == 4)
            {
                DiffieHellmanLogTextBox.Text = "Private keys must be less than modulus.";
                DiffieHellmanStatusInfoBar.Severity = InfoBarSeverity.Warning;
                DiffieHellmanStatusInfoBar.Message = "Private keys must be less than modulus.";
            }
            else
            {
                var result = ellipticCurveDiffieHellmanKeyExchange.KeyExchangeRun(pointGx, pointGy, ecCoeffA, ecCoeffB, ecModulusP, privateKeyA, privateKeyB);
                DiffieHellmanLogTextBox.Text = $"Public Key A: {result.PublicKeyA}\n" +
                                               $"Public Key B: {result.PublicKeyB}\n" +
                                               $"Joint Private Key: {result.PrivateKeyJoint}";
                DiffieHellmanStatusInfoBar.Severity = InfoBarSeverity.Success;
                DiffieHellmanStatusInfoBar.Message = $"Elliptic Curve Diffie-Hellman key exchange successful. Joint Private Key: {result.PrivateKeyJoint}.";
            }

        }
        else
        {
            DiffieHellmanLogTextBox.Text = "Please enter correct parameters.";
            DiffieHellmanStatusInfoBar.Severity = InfoBarSeverity.Warning;
            DiffieHellmanStatusInfoBar.Message = "Please enter correct parameters.";
        }
    }
}
