using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.WinUI.Controls;
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

public class PublicKeyAlgorithmSecurityLevelItem
{
    public string AlgorithmFamily { get; set; } = string.Empty;

    public string Cryptosystems { get; set; } = string.Empty;

    public string SecurityLevel80 { get; set; } = string.Empty;

    public string SecurityLevel128 { get; set; } = string.Empty;

    public string SecurityLevel192 { get; set; } = string.Empty;

    public string SecurityLevel256 { get; set; } = string.Empty;

    public List<PublicKeyAlgorithmSecurityLevelItem> SubItems { get; set; } = new();
}

public sealed partial class PublicKeyAlgorithmFamiliesPage : Page
{
    public PublicKeyAlgorithmFamiliesPage()
    {
        InitializeComponent();
    }

    public ObservableCollection<PublicKeyAlgorithmSecurityLevelItem> PublicKeyAlgorithmSecurityLevelItems { get; set; } =
    [
        new()
        {
            AlgorithmFamily = "Integer factorization",
            Cryptosystems = "RSA",
            SecurityLevel80 = "(1024 bits)",
            SecurityLevel128 = "3072 bits",
            SecurityLevel192 = "7680 bits",
            SecurityLevel256 = "15360 bits",
        },
        new()
        {
            AlgorithmFamily = "Discrete logarithm",
            Cryptosystems = "DH, DSA, Elgamal",
            SecurityLevel80 = "(1024 bits)",
            SecurityLevel128 = "3072 bits",
            SecurityLevel192 = "7680 bits",
            SecurityLevel256 = "15360 bits",
        },
        new()
        {
            AlgorithmFamily = "Elliptic curves",
            Cryptosystems = "ECDH, ECDSA",
            SecurityLevel80 = "(160 bits)",
            SecurityLevel128 = "256 bits",
            SecurityLevel192 = "384 bits",
            SecurityLevel256 = "512 bits",
        },
        new()
        {
            AlgorithmFamily = "Symmetric-key",
            Cryptosystems = "e.g., AES",
            SecurityLevel80 = "(80 bits)",
            SecurityLevel128 = "128 bits",
            SecurityLevel192 = "192 bits",
            SecurityLevel256 = "256 bits",
        },
    ];
}
