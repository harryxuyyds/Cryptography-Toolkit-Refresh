using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

using Cryptography_Toolkit.Helpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cryptography_Toolkit.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EncryptionDecryptionRSAPage : Page
    {
        private readonly Common _common;

        private int _eulerPhiN, _keyPubN, _keyPubE, _keyPrD;

        public EncryptionDecryptionRSAPage()
        {
            InitializeComponent();
            _common = new Common();
        }

        private void RsaPrimeGenerationExecuteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var millerRabinPrimalityTest = new Components.MillerRabinPrimalityTest();

            if (int.TryParse(RsaPrimeGenerationParaPNumberBox.Text, out var p) &&
                int.TryParse(RsaPrimeGenerationParaPNumberBox.Text, out var q) &&
                millerRabinPrimalityTest.MillerRabinPrimalityTestRun(8, p) &&
                millerRabinPrimalityTest.MillerRabinPrimalityTestRun(8, q))
            {
                _keyPubN = p * q;
                _eulerPhiN = _common.EulerPhiFunctionCalc(_keyPubN);

                RsaKeyGenerationPublicExponentENumberBox.Minimum = 1;
                RsaKeyGenerationPublicExponentENumberBox.Maximum = _eulerPhiN - 1;
                RsaKeyGenerationPublicExponentENumberBox.Value = _eulerPhiN - 1;
                RsaKeyGenerationPublicExponentENumberBox.IsEnabled = true;
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
                (int gcd, int s, int t) = _common.ExtendedEuclideanAlgorithmCalc(_keyPubE, _eulerPhiN);
                if (gcd == 1)
                {
                    _keyPrD = t;
                    if (_keyPrD < 0)
                    {
                        _keyPrD += _eulerPhiN;
                    }
                    RsaKeyGenerationLogTextBox.Text = $"Public Key: k_pub(n, e) = ({_keyPubN}, {_keyPubE})\nPrivate Key: k_pr(d) = ({_keyPrD})";
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
    }
}
