using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
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
public sealed partial class DiffieHellmanKeyExchangePage : Page
{
    public DiffieHellmanKeyExchangePage()
    {
        InitializeComponent();
        DiffieHellmanSetup();
        DiffieHellmanModulusNumberBox.ValueChanged += DiffieHellmanNumberBox_OnValueChanged;
        DiffieHellmanGeneratorNumberBox.ValueChanged += DiffieHellmanNumberBox_OnValueChanged;
        DiffieHellmanPrivateKeyAliceNumberBox.ValueChanged += DiffieHellmanNumberBox_OnValueChanged;
        DiffieHellmanPrivateKeyBobNumberBox.ValueChanged += DiffieHellmanNumberBox_OnValueChanged;
    }

    private void DiffieHellmanNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        DiffieHellmanSetup();
    }

    private void DiffieHellmanSetup()
    {
        var diffieHellmanKeyExchange = new Components.DiffieHellmanKeyExchange();
        if (DiffieHellmanModulusNumberBox.Value is double modulusValue &&
            DiffieHellmanGeneratorNumberBox.Value is double generatorValue &&
            DiffieHellmanPrivateKeyAliceNumberBox.Value is double privateKeyAValue &&
            DiffieHellmanPrivateKeyBobNumberBox.Value is double privateKeyBValue)
        {
            var modulus = (int)modulusValue;
            var generator = (int)generatorValue;
            var privateKeyA = (int)privateKeyAValue;
            var privateKeyB = (int)privateKeyBValue;
            
            var preCheckResult = diffieHellmanKeyExchange.DiffieHellmanKeyExchangePreCheck(modulus, generator, privateKeyA, privateKeyB);
            if (preCheckResult == 2)
            {
                DiffieHellmanLogTextBox.Text = "Modulus must be a prime number.";
                DiffieHellmanStatusInfoBar.Severity = InfoBarSeverity.Warning;
                DiffieHellmanStatusInfoBar.Message = "Modulus must be a prime number.";
            }
            else if (preCheckResult == 3)
            {
                DiffieHellmanLogTextBox.Text = "Generator is not a primitive root of modulus.";
                DiffieHellmanStatusInfoBar.Severity = InfoBarSeverity.Warning;
                DiffieHellmanStatusInfoBar.Message = "Generator is not a primitive root of modulus.";
            }
            else if (preCheckResult == 4)
            {
                DiffieHellmanLogTextBox.Text = "Private keys must be less than modulus.";
                DiffieHellmanStatusInfoBar.Severity = InfoBarSeverity.Warning;
                DiffieHellmanStatusInfoBar.Message = "Private keys must be less than modulus.";
            }
            else
            {
                var result = diffieHellmanKeyExchange.DiffieHellmanKeyExchangeRun(modulus, generator, privateKeyA, privateKeyB);
                DiffieHellmanLogTextBox.Text = $"Public Key A: {result.PublicKeyA}\n" +
                                               $"Public Key B: {result.PublicKeyB}\n" +
                                               $"Joint Private Key: {result.PrivateKeyJoint}";
                DiffieHellmanStatusInfoBar.Severity = InfoBarSeverity.Success;
                DiffieHellmanStatusInfoBar.Message = $"Diffie-Hellman key exchange successful. Joint Private Key: {result.PrivateKeyJoint}.";
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
