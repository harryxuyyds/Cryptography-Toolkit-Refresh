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
public sealed partial class DigitalSignatureAlgorithmPage : Page
{
    private readonly Common _common;

    public DigitalSignatureAlgorithmPage()
    {
        InitializeComponent();
        _common = new Common();
        DsaPrimePNumberBox.ValueChanged += DsaKeyGenerationNumberBox_OnValueChanged;
        DsaPrimeQNumberBox.ValueChanged += DsaKeyGenerationNumberBox_OnValueChanged;
        DsaGeneratorGNumberBox.ValueChanged += DsaKeyGenerationNumberBox_OnValueChanged;
        DsaKeyPrXNumberBox.ValueChanged += DsaKeyGenerationNumberBox_OnValueChanged;
        DsaKeyGenerationSetup();
        SignatureGenerationMessageHashNumberBox.ValueChanged += SignatureGenerationNumberBox_OnValueChanged;
        SignatureGenerationEphemeralKeyNumberBox.ValueChanged += SignatureGenerationNumberBox_OnValueChanged;
        SignatureGenerationSetup();
    }

    private void DsaKeyGenerationNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        DsaKeyGenerationSetup();
    }

    private void DsaKeyGenerationSetup()
    {
        try
        {
            // Get input values
            if (DsaPrimePNumberBox.Value is double pDouble &&
                DsaPrimeQNumberBox.Value is double qDouble &&
                DsaGeneratorGNumberBox.Value is double gDouble &&
                DsaKeyPrXNumberBox.Value is double xDouble &&
                !double.IsNaN(pDouble) && !double.IsNaN(qDouble) &&
                !double.IsNaN(gDouble) && !double.IsNaN(xDouble))
            {
                
            }
            else
            {
                DsaKeyGenerationCheckInfoBar.Message = "Please enter valid numbers.";
                DsaKeyGenerationCheckInfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                DsaKeyGenerationCheckInfoBar.IsOpen = true;
                DsaKeyPubYTextBlock.Text = string.Empty;
                return;
            }

            long p = (long)pDouble;
            long q = (long)qDouble;
            long g = (long)gDouble;
            long x = (long)xDouble;

            // Check parameter validity
            if (p <= 0 || q <= 0 || g <= 0 || x <= 0)
            {
                DsaKeyGenerationCheckInfoBar.Message = "All parameters must be positive integers.";
                DsaKeyGenerationCheckInfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                DsaKeyGenerationCheckInfoBar.IsOpen = true;
                DsaKeyPubYTextBlock.Text = string.Empty;
                return;
            }
            if (x >= q)
            {
                DsaKeyGenerationCheckInfoBar.Message = "Private key x must be less than prime q.";
                DsaKeyGenerationCheckInfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                DsaKeyGenerationCheckInfoBar.IsOpen = true;
                DsaKeyPubYTextBlock.Text = string.Empty;
                return;
            }

            // Calculate public key y = g^x mod p
            int y = _common.SquareMultiplyAlgorithmCalc((int)g, (int)x, (int)p);

            DsaKeyPubYTextBlock.Text = y.ToString();

            DsaKeyGenerationCheckInfoBar.Message = $"Public key calculated successfully: y = {g}^{x} mod {p} = {y}";
            DsaKeyGenerationCheckInfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
            DsaKeyGenerationCheckInfoBar.IsOpen = true;
        }
        catch (Exception ex)
        {
            DsaKeyGenerationCheckInfoBar.Message = $"An error occurred: {ex.Message}";
            DsaKeyGenerationCheckInfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
            DsaKeyGenerationCheckInfoBar.IsOpen = true;
            DsaKeyPubYTextBlock.Text = string.Empty;
        }
    }

    private void SignatureGenerationNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        SignatureGenerationSetup();
    }

    private void SignatureGenerationSetup()
    {
        try
        {
            // 获取输入参数
            if (DsaPrimePNumberBox.Value is double pDouble &&
                DsaPrimeQNumberBox.Value is double qDouble &&
                DsaGeneratorGNumberBox.Value is double gDouble &&
                DsaKeyPrXNumberBox.Value is double xDouble &&
                SignatureGenerationEphemeralKeyNumberBox.Value is double kDouble &&
                SignatureGenerationMessageHashNumberBox.Value is double messageHashDouble &&
                !double.IsNaN(pDouble) && !double.IsNaN(qDouble) && !double.IsNaN(messageHashDouble) &&
                !double.IsNaN(gDouble) && !double.IsNaN(xDouble) && !double.IsNaN(kDouble))
            {
                long p = (long)pDouble;
                long q = (long)qDouble;
                long g = (long)gDouble;
                long messageHash = (long)messageHashDouble;
                long x = (long)xDouble;
                long k = (long)kDouble;

                if (p <= 0 || q <= 0 || g <= 0 || x <= 0 || k <= 0)
                {
                    SignatureGenerationSignatureTextBlock.Text = "All parameters must be positive integers.";
                    return;
                }
                if (x >= q)
                {
                    SignatureGenerationSignatureTextBlock.Text = "Private key x must be less than prime q.";
                    return;
                }
                if (k >= q)
                {
                    SignatureGenerationSignatureTextBlock.Text = "Ephemeral key k must be less than prime q.";
                    return;
                }

                int gkModP = _common.SquareMultiplyAlgorithmCalc((int)g, (int)k, (int)p);
                int r = (int)(gkModP % q);

                if (r == 0)
                {
                    SignatureGenerationSignatureTextBlock.Text = "Signature r is zero, choose another k.";
                    return;
                }

                int m = (int)messageHash;

                var (gcd, kInv, _) = _common.ExtendedEuclideanAlgorithmCalc((int)k, (int)q);
                if (gcd != 1)
                {
                    SignatureGenerationSignatureTextBlock.Text = "Ephemeral key k and q are not coprime, choose another k.";
                    return;
                }
                if (kInv < 0) kInv += (int)q;

                int s = (int)(((long)kInv * (m + x * r)) % q);
                if (s == 0)
                {
                    SignatureGenerationSignatureTextBlock.Text = "Signature s is zero, choose another k.";
                    return;
                }

                SignatureGenerationSignatureTextBlock.Text = $"({r}, {s})";
                SignatureVerificationSetup();
            }
            else
            {
                SignatureGenerationSignatureTextBlock.Text = "Please enter valid numbers.";
            }
        }
        catch (Exception ex)
        {
            SignatureGenerationSignatureTextBlock.Text = $"An error occurred: {ex.Message}";
        }
    }

    private void SignatureVerificationSetup()
    {
        try
        {
            // 获取参数
            if (DsaPrimePNumberBox.Value is double pDouble &&
                DsaPrimeQNumberBox.Value is double qDouble &&
                DsaGeneratorGNumberBox.Value is double gDouble &&
                DsaKeyPubYTextBlock.Text is string yText &&
                SignatureGenerationSignatureTextBlock.Text is string signatureText &&
                SignatureGenerationMessageHashNumberBox.Value is double messageHashDouble &&
                !double.IsNaN(pDouble) && !double.IsNaN(qDouble) && !double.IsNaN(gDouble) && !double.IsNaN(messageHashDouble) &&
                !string.IsNullOrWhiteSpace(yText))
            {
                long p = (long)pDouble;
                long q = (long)qDouble;
                long g = (long)gDouble;
                long m = (long)messageHashDouble;

                // 解析y
                if (!long.TryParse(yText, out long y))
                {
                    SignatureVerificationInfoBar.Message = "Invalid public key y.";
                    SignatureVerificationInfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                    SignatureVerificationInfoBar.IsOpen = true;
                    return;
                }

                // 解析签名 (r, s)
                int l = signatureText.IndexOf('(');
                int rIdx = signatureText.IndexOf(',');
                int sIdx = signatureText.IndexOf(')');
                if (l < 0 || rIdx < 0 || sIdx < 0)
                {
                    SignatureVerificationInfoBar.Message = "Invalid signature format.";
                    SignatureVerificationInfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                    SignatureVerificationInfoBar.IsOpen = true;
                    return;
                }
                string rStr = signatureText.Substring(l + 1, rIdx - l - 1).Trim();
                string sStr = signatureText.Substring(rIdx + 1, sIdx - rIdx - 1).Trim();
                if (!long.TryParse(rStr, out long r) || !long.TryParse(sStr, out long s))
                {
                    Debug.WriteLine(rStr);
                    Debug.WriteLine(sStr);
                    SignatureVerificationInfoBar.Message = "Invalid signature values.";
                    SignatureVerificationInfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                    SignatureVerificationInfoBar.IsOpen = true;
                    return;
                }

                // 步骤1: 计算 w = s^{-1} mod q
                var (gcd, w, _) = _common.ExtendedEuclideanAlgorithmCalc((int)s, (int)q);
                if (gcd != 1)
                {
                    SignatureVerificationInfoBar.Message = "s and q are not coprime, cannot compute modular inverse.";
                    SignatureVerificationInfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                    SignatureVerificationInfoBar.IsOpen = true;
                    return;
                }
                if (w < 0) w += (int)q;
                SignatureVerificationWTextBlock.Text = w.ToString();

                // 步骤2: 计算 u = m * w mod q
                long u = (m * w) % q;
                SignatureVerificationUTextBlock.Text = u.ToString();

                // 步骤3: 计算 uu = r * w mod q
                long uu = (r * w) % q;
                SignatureVerificationUuTextBlock.Text = uu.ToString();

                // 步骤4: 计算 v = (g^u * y^uu mod p) mod q
                int guModP = _common.SquareMultiplyAlgorithmCalc((int)g, (int)u, (int)p);
                int yuuModP = _common.SquareMultiplyAlgorithmCalc((int)y, (int)uu, (int)p);
                long v = ((long)guModP * yuuModP) % p;
                v = v % q;
                SignatureVerificationVTextBlock.Text = v.ToString();

                // 结果信息
                bool valid = (v == r);
                SignatureVerificationInfoBar.Message = valid
                    ? $"Signature is valid. v = {v}, r = {r}."
                    : $"Signature is invalid. v = {v}, r = {r}.";
                SignatureVerificationInfoBar.Severity = valid
                    ? Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success
                    : Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                SignatureVerificationInfoBar.IsOpen = true;
            }
            else
            {
                SignatureVerificationInfoBar.Message = "Please enter valid parameters and generate a signature first.";
                SignatureVerificationInfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
                SignatureVerificationInfoBar.IsOpen = true;
            }
        }
        catch (Exception ex)
        {
            SignatureVerificationInfoBar.Message = $"An error occurred: {ex.Message}";
            SignatureVerificationInfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error;
            SignatureVerificationInfoBar.IsOpen = true;
        }
    }
}
