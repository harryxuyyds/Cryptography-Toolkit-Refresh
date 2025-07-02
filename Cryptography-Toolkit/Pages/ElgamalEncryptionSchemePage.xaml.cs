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
public sealed partial class ElgamalEncryptionSchemePage : Page
{
    private readonly Common _common;

    public ElgamalEncryptionSchemePage()
    {
        InitializeComponent();
        _common = new Common();
        ElgamalKeyGenerationPrimePNumberBox.ValueChanged += ElgamalKeyGenerationNumberBox_OnValueChanged;
        ElgamalKeyGenerationGeneratorGNumberBox.ValueChanged += ElgamalKeyGenerationNumberBox_OnValueChanged;
        ElgamalKeyGenerationKeyPrXNumberBox.ValueChanged += ElgamalKeyGenerationNumberBox_OnValueChanged;
        ElgamalKeyGenerationSetup();
        ElgamalEncryptionPlaintextMNumberBox.ValueChanged += ElgamalEncryptionNumberBox_OnValueChanged;
        ElgamalEncryptionNonceKNumberBox.ValueChanged += ElgamalEncryptionNumberBox_OnValueChanged;
        ElgamalEncryptionSetup();
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

            ElgamalEncryptionSetup();
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

    private void ElgamalEncryptionNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        ElgamalEncryptionSetup();
    }

    private void ElgamalEncryptionSetup()
    {
        try
        {
            // Get parameters
            if (ElgamalKeyGenerationPrimePNumberBox.Value is double pDouble &&
                ElgamalKeyGenerationGeneratorGNumberBox.Value is double gDouble &&
                ElgamalKeyGenerationKeyPrXNumberBox.Value is double xDouble &&
                KeyGenerationKeyPubYTextBlock.Text is string yText &&
                ElgamalEncryptionNonceKNumberBox.Value is double kDouble &&
                ElgamalEncryptionPlaintextMNumberBox.Value is double mDouble)
            {
                int p = (int)pDouble;
                int g = (int)gDouble;
                int x = (int)xDouble;
                int k = (int)kDouble;
                int m = (int)mDouble;

                if (!int.TryParse(yText, out int y))
                {
                    ElgamalEncryptionCiphertextTextBlock.Text = "Invalid public key y.";
                    ElgamalDecryptionSecretSTextBlock.Text = "Invalid ciphertext.";
                    ElgamalDecryptionPlaintextTextBlock.Text = "";
                    return;
                }

                if (k <= 0 || k >= p - 1)
                {
                    ElgamalEncryptionCiphertextTextBlock.Text = "1 < k < p-1";
                    ElgamalDecryptionSecretSTextBlock.Text = "Invalid ciphertext.";
                    ElgamalDecryptionPlaintextTextBlock.Text = "";
                    return;
                }
                if (m < 0 || m >= p)
                {
                    ElgamalEncryptionCiphertextTextBlock.Text = "0 ≤ m < p";
                    ElgamalDecryptionSecretSTextBlock.Text = "Invalid ciphertext.";
                    ElgamalDecryptionPlaintextTextBlock.Text = "";
                    return;
                }

                // c1 = g^k mod p
                int c1 = _common.SquareMultiplyAlgorithmCalc(g, k, p);
                // c2 = m * y^k mod p
                int yk = _common.SquareMultiplyAlgorithmCalc(y, k, p);
                int c2 = (m * yk) % p;

                ElgamalEncryptionCiphertextTextBlock.Text = $"({c1}, {c2})";

                ElgamalDecryptionSetup();
            }
            else
            {
                ElgamalEncryptionCiphertextTextBlock.Text = "Invalid input.";
                ElgamalDecryptionSecretSTextBlock.Text = "Invalid ciphertext.";
                ElgamalDecryptionPlaintextTextBlock.Text = "";
            }
        }
        catch (Exception ex)
        {
            ElgamalEncryptionCiphertextTextBlock.Text = $"Error: {ex.Message}";
            ElgamalDecryptionSecretSTextBlock.Text = "Invalid ciphertext.";
            ElgamalDecryptionPlaintextTextBlock.Text = "";
        }
    }

    private void ElgamalDecryptionSetup()
    {
        try
        {
            // Get parameters
            if (ElgamalKeyGenerationPrimePNumberBox.Value is double pDouble &&
                ElgamalKeyGenerationKeyPrXNumberBox.Value is double xDouble &&
                ElgamalEncryptionCiphertextTextBlock.Text is string ciphertext &&
                !string.IsNullOrWhiteSpace(ciphertext))
            {
                int p = (int)pDouble;
                int x = (int)xDouble;

                // Parse ciphertext (c1, c2)
                var parts = ciphertext.Trim('(', ')').Split(',');
                if (parts.Length != 2 ||
                    !int.TryParse(parts[0].Trim(), out int c1) ||
                    !int.TryParse(parts[1].Trim(), out int c2))
                {
                    ElgamalDecryptionSecretSTextBlock.Text = "Invalid ciphertext.";
                    ElgamalDecryptionPlaintextTextBlock.Text = "";
                    return;
                }

                // s = c1^x mod p
                int s = _common.SquareMultiplyAlgorithmCalc(c1, x, p);
                ElgamalDecryptionSecretSTextBlock.Text = $"{s}";

                // Compute s^{-1} mod p using Extended Euclidean Algorithm
                var (gcd, sInv, _) = _common.ExtendedEuclideanAlgorithmCalc(s, p);
                if (gcd != 1)
                {
                    ElgamalDecryptionPlaintextTextBlock.Text = "No modular inverse for s.";
                    return;
                }
                // Ensure sInv is positive
                if (sInv < 0) sInv += p;

                // m = c2 * s^{-1} mod p
                int m = (int)(((long)c2 * sInv) % p);
                ElgamalDecryptionPlaintextTextBlock.Text = $"{m}";
            }
            else
            {
                ElgamalDecryptionSecretSTextBlock.Text = "Invalid input.";
                ElgamalDecryptionPlaintextTextBlock.Text = "";
            }
        }
        catch (Exception ex)
        {
            ElgamalDecryptionSecretSTextBlock.Text = $"Error: {ex.Message}";
            ElgamalDecryptionPlaintextTextBlock.Text = "";
        }
    }
}
