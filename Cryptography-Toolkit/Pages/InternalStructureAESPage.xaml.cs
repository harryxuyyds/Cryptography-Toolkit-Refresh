using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using Cryptography_Toolkit.Components;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cryptography_Toolkit.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class InternalStructureAESPage : Page
{
    private readonly Common _common;

    public InternalStructureAESPage()
    {
        InitializeComponent();
        _common = new Common();
    }

    private void AesPlaintextPresetsComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int aexPlaintextTypeIndex = AesPlaintextPresetsComboBox.SelectedIndex;
        if (aexPlaintextTypeIndex == 0)
        {
            AesPlaintextTextBox.Text = string.Empty;
            AesPlaintextTextBox.Text = "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
        }
        else if (aexPlaintextTypeIndex == 1)
        {
            AesPlaintextTextBox.Text = string.Empty;
            AesPlaintextTextBox.Text = "FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF";
        }
        else if (aexPlaintextTypeIndex == 2)
        {
            AesPlaintextTextBox.Text = string.Empty;
            AesPlaintextTextBox.Text = "F0 F0 F0 F0 F0 F0 F0 F0 F0 F0 F0 F0 F0 F0 F0 F0";
        }
        else if (aexPlaintextTypeIndex == 3)
        {
            AesPlaintextTextBox.Text = string.Empty;
            AesPlaintextTextBox.Text += _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " ";
            AesPlaintextTextBox.Text += _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " ";
            AesPlaintextTextBox.Text += _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " ";
            AesPlaintextTextBox.Text += _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2);
        }
    }

    private void AesKeyPresetsComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int aexKeyTypeIndex = AesKeyPresetsComboBox.SelectedIndex;
        if (aexKeyTypeIndex == 0)
        {
            AesKeyTextBox.Text = string.Empty;
            AesKeyTextBox.Text = "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
        }
        else if (aexKeyTypeIndex == 1)
        {
            AesKeyTextBox.Text = string.Empty;
            AesKeyTextBox.Text = "FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF";
        }
        else if (aexKeyTypeIndex == 2)
        {
            AesKeyTextBox.Text = string.Empty;
            AesKeyTextBox.Text = "F0 F0 F0 F0 F0 F0 F0 F0 F0 F0 F0 F0 F0 F0 F0 F0";
        }
        else if (aexKeyTypeIndex == 3)
        {
            AesKeyTextBox.Text = string.Empty;
            AesKeyTextBox.Text += _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " ";
            AesKeyTextBox.Text += _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " ";
            AesKeyTextBox.Text += _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " ";
            AesKeyTextBox.Text += _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2);
        }
    }

    private void AesTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        string plaintext = AesPlaintextTextBox.Text;
        string key = AesKeyTextBox.Text;
        int plaintextLength = string.IsNullOrWhiteSpace(plaintext) ? 0 : plaintext.Replace(" ", "").Length;
        int keyLength = string.IsNullOrWhiteSpace(key) ? 0 : key.Replace(" ", "").Length;

        // 输出到 Output 窗口
        System.Diagnostics.Debug.WriteLine($"AesPlaintextTextBox 值: {plaintext}");
        System.Diagnostics.Debug.WriteLine($"AesPlaintextTextBox 长度: {plaintextLength}");
        System.Diagnostics.Debug.WriteLine($"AesKeyTextBox 值: {key}");
        System.Diagnostics.Debug.WriteLine($"AesKeyTextBox 长度: {keyLength}");
    }
}
