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
public sealed partial class RSASignatureSchemePage : Page
{
    private readonly Common _common;

    private int _eulerPhiN, _keyPubN, _keyPubE, _keyPrD;

    public RSASignatureSchemePage()
    {
        InitializeComponent();
        _common = new Common();
    }

    private void RsaPrimeGenerationExecuteButton_OnClick(object sender, RoutedEventArgs e)
    {
        var millerRabinPrimalityTest = new Components.MillerRabinPrimalityTest();

        if (int.TryParse(RsaPrimeGenerationParaPNumberBox.Text, out var p) &&
            int.TryParse(RsaPrimeGenerationParaQNumberBox.Text, out var q) &&
            millerRabinPrimalityTest.MillerRabinPrimalityTestRun(8, p) &&
            millerRabinPrimalityTest.MillerRabinPrimalityTestRun(8, q))
        {
            _keyPubN = p * q;
            _eulerPhiN = _common.EulerPhiFunctionCalc(_keyPubN);

            RsaKeyGenerationPublicExponentENumberBox.Minimum = 1;
            RsaKeyGenerationPublicExponentENumberBox.Maximum = _eulerPhiN - 1;
            RsaKeyGenerationPublicExponentENumberBox.Value = _eulerPhiN - 1;
            RsaKeyGenerationPublicExponentENumberBox.IsEnabled = true;
            // RsaPrimeGenerationSettingsExpander.IsExpanded = false;
            RsaKeyGenerationSettingsExpander.IsExpanded = true;
        }
        else
        {
            RsaKeyGenerationLogTextBox.Text = "Please enter correct parameters.";
        }
    }

    private void RsaKeyGenerationExecuteButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(RsaKeyGenerationPublicExponentENumberBox.Text, out _keyPubE))
        {
            (int gcd, int s, int t) = _common.ExtendedEuclideanAlgorithmCalc(_eulerPhiN, _keyPubE);
            if (gcd == 1)
            {
                _keyPrD = t;
                if (_keyPrD < 0)
                {
                    _keyPrD += _eulerPhiN;
                }
                RsaKeyGenerationLogTextBox.Text = $"Public Key: k_pub(n, e) = ({_keyPubN}, {_keyPubE})\nPrivate Key: k_pr(d) = ({_keyPrD})";
                RsaSignatureVerificationPublicKeyNNumberBox.Value = _keyPubN;
                RsaSignatureVerificationPublicKeyENumberBox.Value = _keyPubE;
                RsaSignatureGenerationPublicKeyNNumberBox.Value = _keyPubN;
                RsaSignatureGenerationPrivateKeyDNumberBox.Value = _keyPrD;
                RsaSignatureGenerationSettingsExpander.IsExpanded = true;
            }
            else
            {
                RsaKeyGenerationLogTextBox.Text = $"The public exponent e and Euler's totient ¦Õ(n) ({_eulerPhiN}) are not coprime. Please choose a different e.";
            }
        }
        else
        {
            RsaKeyGenerationLogTextBox.Text = "Please enter correct parameters.";
        }
    }

    private void RsaSignatureGenerationExecuteButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(RsaSignatureGenerationMessageNumberBox.Text, out var m) &&
            int.TryParse(RsaSignatureGenerationPrivateKeyDNumberBox.Text, out var _keyPrD) &&
            int.TryParse(RsaSignatureGenerationPublicKeyNNumberBox.Text, out var _keyPubN))
        {
            if (m < 0 || m >= _keyPubN)
            {
                RsaSignatureGenerationLogTextBox.Text = "Message m must be in the range 0 <= m < n.";
                return;
            }
            var signature = _common.SquareMultiplyAlgorithmCalc(m, _keyPrD, _keyPubN);
            RsaSignatureGenerationLogTextBox.Text = $"Signature generated successfully:\nSignature Value: {signature}\n\n";
            RsaSignatureVerificationSignatureNumberBox.Value = signature;
            RsaSignatureVerificationMessageNumberBox.Value = m;
            RsaSignatureVerificationSettingsExpander.IsExpanded = true;
        }
        else
        {
            RsaSignatureGenerationLogTextBox.Text = "Please enter correct parameters.";
        }
    }

    private void RsaSignatureVerificationExecuteButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(RsaSignatureVerificationSignatureNumberBox.Text, out var s) &&
            int.TryParse(RsaSignatureVerificationPublicKeyENumberBox.Text, out var _keyPubE) &&
            int.TryParse(RsaSignatureVerificationPublicKeyNNumberBox.Text, out var _keyPubN) &&
            int.TryParse(RsaSignatureVerificationMessageNumberBox.Text, out var m))
        {
            var mPrime = _common.SquareMultiplyAlgorithmCalc(s, _keyPubE, _keyPubN);
            if (mPrime == m)
            {
                RsaSignatureVerificationLogTextBox.Text = "Signature is valid.";
            }
            else
            {
                RsaSignatureVerificationLogTextBox.Text = $"Signature is invalid. Computed message: {mPrime}";
            }
        }
        else
        {
            RsaSignatureVerificationLogTextBox.Text = "Please enter correct parameters.";
        }
    }
}
