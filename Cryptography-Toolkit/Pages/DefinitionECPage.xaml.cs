using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI;
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
public sealed partial class DefinitionECPage : Page
{
    public DefinitionECPage()
    {
        InitializeComponent();
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
                DefinitionEcPreviewTextBox.Text = "";
                DefinitionEcPreviewCanvas.Children.Clear();
            }
        }
        else
        {
            DefinitionEcCheckInfoBar.Severity = InfoBarSeverity.Warning;
            DefinitionEcCheckInfoBar.Message = "Please enter correct parameters.";
            DefinitionEcPreviewTextBox.Text = "";
            DefinitionEcPreviewCanvas.Children.Clear();
        }
    }

    private void LoadEllipticCurvePreview(int coeffA, int coeffB, int modulusP)
    {
        DefinitionEcPreviewTextBox.Text = "";
        DefinitionEcPreviewCanvas.Children.Clear();
        var orderCount = 1;

        double width = DefinitionEcPreviewCanvas.ActualWidth;
        double height = DefinitionEcPreviewCanvas.ActualHeight;
        if (width == 0 || height == 0) return;

        int samplePoints = Math.Min(modulusP, 200);

        double xScale = width / modulusP;
        double yScale = height / modulusP;

        var pointBrush = (SolidColorBrush)Application.Current.Resources["AccentTextFillColorPrimaryBrush"];

        for (int x = 0; x < modulusP; x++)
        {
            // 计算右侧 y^2 = x^3 + ax + b mod p
            int rhs = (int)((((long)x * x % modulusP) * x % modulusP + coeffA * x % modulusP + coeffB) % modulusP);
            // 对每个 y，检查 y^2 ≡ rhs (mod p)
            for (int y = 0; y < modulusP; y++)
            {
                if ((y * y) % modulusP == rhs)
                {
                    // 映射到画布坐标（y轴反向）
                    orderCount ++;
                    double px = x * xScale;
                    double py = height - y * yScale;
                    var ellipse = new Ellipse
                    {
                        Width = Math.Max(2, xScale),
                        Height = Math.Max(2, yScale),
                        Fill = pointBrush
                    };
                    Canvas.SetLeft(ellipse, px - ellipse.Width / 2);
                    Canvas.SetTop(ellipse, py - ellipse.Height / 2);
                    DefinitionEcPreviewCanvas.Children.Add(ellipse);

                    if (DefinitionEcPreviewTextBox.Text == "")
                    {
                        DefinitionEcPreviewTextBox.Text = "Elliptic Curve Points (mod p)\r\n";
                        DefinitionEcPreviewTextBox.Text += "-------------------------------\r\n";
                        DefinitionEcPreviewTextBox.Text += $"{"x",8} | {"y",8}\r\n";
                        DefinitionEcPreviewTextBox.Text += "-------------------------------\r\n";
                    }
                    DefinitionEcPreviewTextBox.Text += $"{x,8} | {y,8}\r\n";
                }
            }
        }

        DefinitionEcOrderTextBlock.Text = orderCount.ToString();
    }

    private void UIElement_OnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        DefinitionEcSetup();
    }

    private void DefinitionEcPreviewCanvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        DefinitionEcSetup();
    }
}
