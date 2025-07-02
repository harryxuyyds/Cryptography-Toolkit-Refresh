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
public sealed partial class ElgamalDigitalSignatureSchemePage : Page
{
    private readonly Common _common;

    public ElgamalDigitalSignatureSchemePage()
    {
        InitializeComponent();
        _common = new Common();
        ElgamalKeyGenerationPrimePNumberBox.ValueChanged += ElgamalKeyGenerationNumberBox_OnValueChanged;
        ElgamalKeyGenerationGeneratorGNumberBox.ValueChanged += ElgamalKeyGenerationNumberBox_OnValueChanged;
        ElgamalKeyGenerationKeyPrXNumberBox.ValueChanged += ElgamalKeyGenerationNumberBox_OnValueChanged;
        ElgamalKeyGenerationSetup();
        ElgamalSignatureGenerationMessageNumberBox.ValueChanged += ElgamalSignatureGenerationNumberBox_OnValueChanged;
        ElgamalSignatureGenerationNonceKNumberBox.ValueChanged += ElgamalSignatureGenerationNumberBox_OnValueChanged;
    }

    private void ElgamalKeyGenerationNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        ElgamalKeyGenerationSetup();
    }

    private void ElgamalKeyGenerationSetup()
    {
        var millerRabinPrimalityTest = new Components.MillerRabinPrimalityTest();
        try
        {
            // 获取参数
            if (ElgamalKeyGenerationPrimePNumberBox.Value is double pDouble &&
                ElgamalKeyGenerationGeneratorGNumberBox.Value is double gDouble &&
                ElgamalKeyGenerationKeyPrXNumberBox.Value is double xDouble &&
                millerRabinPrimalityTest.MillerRabinPrimalityTestRun(8, (int)pDouble))
            {

            }
            else
            {
                KeyGenerationKeyPubYTextBlock.Text = "Invalid input: Please enter valid numbers.";
                return;
            }

            var p = (int)pDouble;
            var g = (int)gDouble;
            var x = (int)xDouble;

            if (p <= 2)
            {
                KeyGenerationKeyPubYTextBlock.Text = "p > 2";
                return;
            }
            if (g <= 1 || g >= p)
            {
                KeyGenerationKeyPubYTextBlock.Text = "1 < g < p";
                return;
            }
            if (x <= 0 || x >= p - 1)
            {
                KeyGenerationKeyPubYTextBlock.Text = "1 < x < p-1";
                return;
            }
            if (!IsPrimitiveRoot(g, p))
            {
                KeyGenerationKeyPubYTextBlock.Text = "g is not a primitive root";
                return;
            }

            // 计算 y = g^x mod p
            long y = _common.SquareMultiplyAlgorithmCalc(g, x, p);

            KeyGenerationKeyPubYTextBlock.Text = $"{y}";

            ElgamalSignatureGeneration();
        }
        catch (Exception ex)
        {
            KeyGenerationKeyPubYTextBlock.Text = $"Error: {ex.Message}";
        }
    }

    private bool IsPrimitiveRoot(int g, int p)
    {
        if (g <= 1 || g >= p) return false;
        int phi = p - 1;
        var factors = new HashSet<int>();
        int n = phi;
        for (int i = 2; i * i <= n; i++)
        {
            if (n % i == 0)
            {
                factors.Add(i);
                while (n % i == 0)
                    n /= i;
            }
        }
        if (n > 1) factors.Add(n);

        foreach (var factor in factors)
        {
            if (_common.SquareMultiplyAlgorithmCalc(g, phi / factor, p) == 1)
                return false;
        }
        return true;
    }

    private void ElgamalSignatureGenerationNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        ElgamalSignatureGeneration();
    }

    private void ElgamalSignatureGeneration()
    {
        try
        {
            // 获取参数
            if (ElgamalKeyGenerationPrimePNumberBox.Value is double pDouble &&
                ElgamalKeyGenerationGeneratorGNumberBox.Value is double gDouble &&
                ElgamalKeyGenerationKeyPrXNumberBox.Value is double xDouble &&
                KeyGenerationKeyPubYTextBlock.Text is string yText &&
                ElgamalSignatureGenerationMessageNumberBox.Value is double mDouble &&
                ElgamalSignatureGenerationNonceKNumberBox.Value is double kDouble)
            {
                int p = (int)pDouble;
                int g = (int)gDouble;
                int x = (int)xDouble;
                int m = (int)mDouble;
                int k = (int)kDouble;

                if (p <= 2 || g <= 1 || g >= p || x <= 0 || x >= p - 1 || k <= 0 || k >= p - 1)
                {
                    ElgamalSignatureGenerationSignatureTextBlock.Text = "Invalid input: Check p, g, x, k ranges.";
                    ElgamalSignatureVerificationValueTTextBlock.Text = String.Empty;
                    ElgamalSignatureVerification();
                    return;
                }

                // 检查k与p-1互素
                var (gcd, s, t) = _common.ExtendedEuclideanAlgorithmCalc(k, p - 1);
                if (gcd != 1)
                {
                    ElgamalSignatureGenerationSignatureTextBlock.Text = "k and p-1 are not coprime.";
                    ElgamalSignatureVerificationValueTTextBlock.Text = String.Empty;
                    ElgamalSignatureVerification();
                    return;
                }

                // 计算r = g^k mod p
                int r = _common.SquareMultiplyAlgorithmCalc(g, k, p);

                // 计算k的逆元
                int kInv = s;
                if (kInv < 0) kInv += (p - 1);

                // 计算s = k^{-1} * (m - x*r) mod (p-1)
                int sVal = (int)(((long)kInv * (m - (long)x * r)) % (p - 1));
                if (sVal < 0) sVal += (p - 1);

                ElgamalSignatureGenerationSignatureTextBlock.Text = $"({r}, {sVal})";
                ElgamalSignatureVerificationValueTTextBlock.Text = String.Empty;
                ElgamalSignatureVerification();
            }
            else
            {
                ElgamalSignatureGenerationSignatureTextBlock.Text = "Invalid input: Please enter valid numbers.";
                ElgamalSignatureVerificationValueTTextBlock.Text = String.Empty;
                ElgamalSignatureVerification();
            }
        }
        catch (Exception ex)
        {
            ElgamalSignatureGenerationSignatureTextBlock.Text = $"Error: {ex.Message}";
            ElgamalSignatureVerificationValueTTextBlock.Text = String.Empty;
            ElgamalSignatureVerification();
        }
    }

    private void ElgamalSignatureVerification()
    {
        try
        {
            // 获取参数
            if (ElgamalKeyGenerationPrimePNumberBox.Value is double pDouble &&
                ElgamalKeyGenerationGeneratorGNumberBox.Value is double gDouble &&
                KeyGenerationKeyPubYTextBlock.Text is string yText &&
                ElgamalSignatureGenerationMessageNumberBox.Value is double mDouble &&
                ElgamalSignatureGenerationSignatureTextBlock.Text is string sigText)
            {
                int p = (int)pDouble;
                int g = (int)gDouble;
                int m = (int)mDouble;

                // 解析y
                if (!long.TryParse(yText, out long y))
                {
                    SignatureVerificationInfoBar.Severity = InfoBarSeverity.Warning;
                    SignatureVerificationInfoBar.Message = "Invalid public key y.";
                    SignatureVerificationInfoBar.IsOpen = true;
                    return;
                }

                // 解析签名(r, s)
                var sig = sigText.Trim('(', ')').Split(',');
                if (sig.Length != 2 ||
                    !int.TryParse(sig[0].Trim(), out int r) ||
                    !int.TryParse(sig[1].Trim(), out int s))
                {
                    SignatureVerificationInfoBar.Severity = InfoBarSeverity.Warning;
                    SignatureVerificationInfoBar.Message = "Invalid signature format.";
                    SignatureVerificationInfoBar.IsOpen = true;
                    return;
                }

                // 检查r, s范围
                if (r <= 0 || r >= p || s < 0 || s >= p - 1)
                {
                    SignatureVerificationInfoBar.Severity = InfoBarSeverity.Warning;
                    SignatureVerificationInfoBar.Message = "Signature values out of range.";
                    SignatureVerificationInfoBar.IsOpen = true;
                    return;
                }

                long v1 = (_common.SquareMultiplyAlgorithmCalc((int)y, r, p) *
                           _common.SquareMultiplyAlgorithmCalc(r, s, p)) % p;
                long v2 = _common.SquareMultiplyAlgorithmCalc(g, m, p);

                ElgamalSignatureVerificationValueTTextBlock.Text = $"v1 = {y}^{r} * {r}^{s} mod {p} = {v1}\nv2 = {g}^{m} mod {p} = {v2}";

                if (v1 == v2)
                {
                    SignatureVerificationInfoBar.Severity = InfoBarSeverity.Success;
                    SignatureVerificationInfoBar.Message = "Signature is valid.";
                    SignatureVerificationInfoBar.IsOpen = true;
                }
                else
                {
                    SignatureVerificationInfoBar.Severity = InfoBarSeverity.Error;
                    SignatureVerificationInfoBar.Message = "Signature is invalid.";
                    SignatureVerificationInfoBar.IsOpen = true;
                }
            }
            else
            {
                SignatureVerificationInfoBar.Severity = InfoBarSeverity.Warning;
                SignatureVerificationInfoBar.Message = "Invalid input: Please enter valid numbers.";
                SignatureVerificationInfoBar.IsOpen = true;
            }
        }
        catch (Exception ex)
        {
            SignatureVerificationInfoBar.Severity = InfoBarSeverity.Error;
            SignatureVerificationInfoBar.Message = $"Error: {ex.Message}";
            SignatureVerificationInfoBar.IsOpen = true;
        }
    }
}
