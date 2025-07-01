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

namespace Cryptography_Toolkit.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SpeedUpTechniquesRSAPage : Page
    {
        private readonly Common _common;

        public SpeedUpTechniquesRSAPage()
        {
            InitializeComponent();
            _common = new Common();
            RsaWithCrtParaPNumberBox.ValueChanged += RsaWithCrtPrimeNumberBox_OnValueChanged;
            RsaWithCrtParaQNumberBox.ValueChanged += RsaWithCrtPrimeNumberBox_OnValueChanged;
            RsaWithCrtPublicExponentENumberBox.ValueChanged += RsaWithCrtNumberBox_OnValueChanged;
            RsaWithCrtPlaintextNumberBox.ValueChanged += RsaWithCrtNumberBox_OnValueChanged;
            LoadPublicExponentECheck();
        }

        private void RsaWithCrtPrimeNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            LoadPublicExponentECheck();
        }

        private void LoadPublicExponentECheck()
        {
            var millerRabinPrimalityTest = new Components.MillerRabinPrimalityTest();

            if (int.TryParse(RsaWithCrtParaPNumberBox.Text, out var p) &&
                int.TryParse(RsaWithCrtParaQNumberBox.Text, out var q) &&
                millerRabinPrimalityTest.MillerRabinPrimalityTestRun(8, p) &&
                millerRabinPrimalityTest.MillerRabinPrimalityTestRun(8, q))
            {
                int keyPubN = p * q;
                int eulerPhiN = _common.EulerPhiFunctionCalc(keyPubN);

                RsaWithCrtPublicExponentENumberBox.Minimum = 1;
                RsaWithCrtPublicExponentENumberBox.Maximum = eulerPhiN - 1;
                RsaWithCrtPublicExponentENumberBox.Value = eulerPhiN - 1;
                RsaWithCrtPublicExponentENumberBox.IsEnabled = true;
            }
            else
            {
                RsaWithCrtLogTextBox.Text = "Please enter valid prime numbers for p and q.";
            }
        }

        private void RsaWithCrtNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            // RsaDecryptionWithCrtSetup();
        }


        private void RsaWithCrtExecuteButton_OnClick(object sender, RoutedEventArgs e)
        {
            // 检查输入是否为空
            if (double.IsNaN(RsaWithCrtParaPNumberBox.Value) ||
                double.IsNaN(RsaWithCrtParaQNumberBox.Value) ||
                double.IsNaN(RsaWithCrtPublicExponentENumberBox.Value) ||
                double.IsNaN(RsaWithCrtPlaintextNumberBox.Value))
            {
                RsaWithCrtLogTextBox.Text = "Please enter valid numbers for all parameters.";
                return;
            }

            // 所有输入均有效，执行解密流程
            RsaDecryptionWithCrtSetup();
        }

        private void RsaDecryptionWithCrtSetup()
        {
            RsaWithCrtLogTextBox.Text = string.Empty; // 清空日志文本框
            if (RsaWithCrtParaPNumberBox.Value is double pVal &&
                RsaWithCrtParaQNumberBox.Value is double qVal &&
                RsaWithCrtPublicExponentENumberBox.Value is double eVal &&
                RsaWithCrtPlaintextNumberBox.Value is double cVal)
            {
                int p = (int)pVal;
                int q = (int)qVal;
                int e = (int)eVal;
                int cipherText = (int)cVal;

                int modulusN = p * q;
                int phiN = (p - 1) * (q - 1);

                RsaWithCrtLogTextBox.Text += $"Step 1: Compute modulus n = p × q = {p} × {q} = {modulusN}\n";
                RsaWithCrtLogTextBox.Text +=
                    $"Step 2: Compute Euler's totient φ(n) = (p - 1) × (q - 1) = {p - 1} × {q - 1} = {phiN}\n";

                // Extended Euclidean Algorithm to find d
                var (gcd, s, t) = _common.ExtendedEuclideanAlgorithmCalc(phiN, e);
                while (t < 0)
                {
                    t = (t + phiN) % phiN;
                }

                int privateExponentD = t;

                RsaWithCrtLogTextBox.Text +=
                    $"Step 3: Compute private exponent d (modular inverse of e mod φ(n)) = {privateExponentD}\n";

                int cModP = cipherText % p;
                int cModQ = cipherText % q;
                int dModPMinus1 = privateExponentD % (p - 1);
                int dModQMinus1 = privateExponentD % (q - 1);
                int m1 = _common.SquareMultiplyAlgorithmCalc(cModP, dModPMinus1, p);
                int m2 = _common.SquareMultiplyAlgorithmCalc(cModQ, dModQMinus1, q);

                RsaWithCrtLogTextBox.Text += $"Step 4: Compute c mod p = {cipherText} mod {p} = {cModP}\n";
                RsaWithCrtLogTextBox.Text += $"Step 5: Compute c mod q = {cipherText} mod {q} = {cModQ}\n";
                RsaWithCrtLogTextBox.Text +=
                    $"Step 6: Compute d mod (p - 1) = {privateExponentD} mod {p - 1} = {dModPMinus1}\n";
                RsaWithCrtLogTextBox.Text +=
                    $"Step 7: Compute d mod (q - 1) = {privateExponentD} mod {q - 1} = {dModQMinus1}\n";
                RsaWithCrtLogTextBox.Text +=
                    $"Step 8: Compute m1 = c^dP mod p = {cModP}^{dModPMinus1} mod {p} = {m1}\n";
                RsaWithCrtLogTextBox.Text +=
                    $"Step 9: Compute m2 = c^dQ mod q = {cModQ}^{dModQMinus1} mod {q} = {m2}\n";

                // Use Extended Euclidean Algorithm for coefficients
                var (gcd2, sCoeff, tCoeff) = _common.ExtendedEuclideanAlgorithmCalc(q, p);
                while (sCoeff < 0)
                {
                    sCoeff = (sCoeff + p) % p;
                }

                while (tCoeff < 0)
                {
                    tCoeff = (tCoeff + q) % q;
                }

                RsaWithCrtLogTextBox.Text +=
                    $"Step 10: Compute coefficients s and t using Extended Euclidean Algorithm:\n";
                RsaWithCrtLogTextBox.Text += $"         s = {sCoeff}, t = {tCoeff}\n";
                RsaWithCrtLogTextBox.Text +=
                    $"Step 11: Compute m1 × q × s = {m1} × {q} × {sCoeff} = {m1 * q * sCoeff}\n";
                RsaWithCrtLogTextBox.Text +=
                    $"Step 12: Compute m2 × p × t = {m2} × {p} × {tCoeff} = {m2 * p * tCoeff}\n";

                int plainText = (m1 * q * sCoeff + m2 * p * tCoeff) % modulusN;
                if (plainText < 0)
                {
                    plainText += modulusN;
                }

                RsaWithCrtLogTextBox.Text +=
                    $"Step 13: Recover plaintext = (m1 × q × s + m2 × p × t) mod n = {plainText}\n";
            }
        }
    }
}
