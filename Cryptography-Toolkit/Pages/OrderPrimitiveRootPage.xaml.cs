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
public sealed partial class OrderPrimitiveRootPage : Page
{
    public OrderPrimitiveRootPage()
    {
        InitializeComponent();
        OrderCalcBaseANumberBox.ValueChanged += OrderCalcNumberBox_OnValueChanged;
        OrderCalcModulusNNumberBox.ValueChanged += OrderCalcNumberBox_OnValueChanged;
        PrimitiveRootCalcModulusNNumberBox.ValueChanged += PrimitiveRootCalcNumberBox_OnValueChanged;
        OrderCalcSetup();
        PrimitiveRootCalcSetup();
    }

    private void OrderCalcNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        OrderCalcSetup();
    }

    private void OrderCalcSetup()
    {
        var millerRabinPrimalityTest = new Components.MillerRabinPrimalityTest();

        if (OrderCalcBaseANumberBox.Value is double paraAValue &&
            OrderCalcModulusNNumberBox.Value is double modulusPValue &&
            millerRabinPrimalityTest.MillerRabinPrimalityTestRun(8, (int)modulusPValue))
        {
            var paraA = (int)paraAValue;
            var modulusP = (int)modulusPValue;
            var common = new Helpers.Common();

            // Calculate Euler's totient function ¦Õ(n)
            int phi = common.EulerPhiFunctionCalc(modulusP);
            OrderCalcEulerPhiTextBlock.Text = $"Euler's totient ¦Õ({modulusP}) = {phi}";

            // Find the order of a modulo n
            int order = -1;
            string log = $"Finding the order of {paraA} modulo {modulusP}:\n";
            for (int k = 1; k <= phi; k++)
            {
                int result = common.SquareMultiplyAlgorithmCalc(paraA, k, modulusP);
                log += $"{paraA}^{k} mod {modulusP} = {result}\n";
                if (result == 1)
                {
                    order = k;
                    break;
                }
            }

            if (order != -1)
            {
                OrderCalcOrderTextBlock.Text = $"Order of {paraA} modulo {modulusP} is {order}";
                OrderCalcLogTextBox.Text = log + $"Order found: {order}";
                OrderCalcCheckInfoBar.Severity = InfoBarSeverity.Success;
                OrderCalcCheckInfoBar.Message = "Order calculation succeeded.";
            }
            else
            {
                OrderCalcOrderTextBlock.Text = "Order not found.";
                OrderCalcLogTextBox.Text = log + "Order not found.";
                OrderCalcCheckInfoBar.Severity = InfoBarSeverity.Warning;
                OrderCalcCheckInfoBar.Message = "Order not found for the given parameters.";
            }
        }
        else
        {
            OrderCalcCheckInfoBar.Severity = InfoBarSeverity.Warning;
            OrderCalcCheckInfoBar.Message = "Please enter correct parameters.";
            OrderCalcOrderTextBlock.Text = string.Empty;
            OrderCalcEulerPhiTextBlock.Text = string.Empty;
            OrderCalcLogTextBox.Text = string.Empty;
        }
    }

    private void PrimitiveRootCalcNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        PrimitiveRootCalcSetup();
    }

    private void PrimitiveRootCalcSetup()
    {
        var millerRabinPrimalityTest = new Components.MillerRabinPrimalityTest();

        if (PrimitiveRootCalcModulusNNumberBox.Value is double modulusNValue)
        {
            var modulusN = (int)modulusNValue;
            var common = new Helpers.Common();
            string log = $"Finding primitive roots modulo {modulusN}:\n";

            // Check if modulusN is prime
            if (!millerRabinPrimalityTest.MillerRabinPrimalityTestRun(8, modulusN))
            {
                PrimitiveRootCalcCheckInfoBar.Severity = InfoBarSeverity.Warning;
                PrimitiveRootCalcCheckInfoBar.Message = "Modulus n must be a prime number.";
                PrimitiveRootCalcCountTextBlock.Text = string.Empty;
                PrimitiveRootCalcEulerTotientTextBlock.Text = string.Empty;
                PrimitiveRootCalcLogTextBox.Text = string.Empty;
                return;
            }

            int phi = common.EulerPhiFunctionCalc(modulusN);
            PrimitiveRootCalcEulerTotientTextBlock.Text = $"Euler's totient ¦Õ({modulusN}) = {phi}";

            List<int> primitiveRoots = new();
            List<int> divisors = new();
            for (int i = 1; i * i <= phi; i++)
            {
                if (phi % i == 0)
                {
                    divisors.Add(i);
                    if (i != phi / i)
                        divisors.Add(phi / i);
                }
            }
            divisors = divisors.Where(d => d != phi).ToList();

            for (int g = 2; g < modulusN; g++)
            {
                bool isPrimitiveRoot = true;
                foreach (var d in divisors)
                {
                    if (common.SquareMultiplyAlgorithmCalc(g, d, modulusN) == 1)
                    {
                        isPrimitiveRoot = false;
                        break;
                    }
                }
                if (isPrimitiveRoot)
                {
                    primitiveRoots.Add(g);
                    log += $"{g} is a primitive root.\n";
                }
            }

            PrimitiveRootCalcCountTextBlock.Text = $"Number of primitive roots: {primitiveRoots.Count}";
            log += $"Total primitive roots: {primitiveRoots.Count}\n";
            PrimitiveRootCalcLogTextBox.Text = log;
            PrimitiveRootCalcCheckInfoBar.Severity = InfoBarSeverity.Success;
            PrimitiveRootCalcCheckInfoBar.Message = "Primitive root calculation succeeded.";
        }
        else
        {
            PrimitiveRootCalcCheckInfoBar.Severity = InfoBarSeverity.Warning;
            PrimitiveRootCalcCheckInfoBar.Message = "Please enter correct parameters.";
            PrimitiveRootCalcCountTextBlock.Text = string.Empty;
            PrimitiveRootCalcEulerTotientTextBlock.Text = string.Empty;
            PrimitiveRootCalcLogTextBox.Text = string.Empty;
        }
    }
}
