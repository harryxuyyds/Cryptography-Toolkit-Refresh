using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Cryptography_Toolkit.Helpers;

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
        switch (aexPlaintextTypeIndex)
        {
            case 0:
                AesPlaintextTextBox.Text = string.Empty;
                AesPlaintextTextBox.Text = "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
                break;
            case 1:
                AesPlaintextTextBox.Text = string.Empty;
                AesPlaintextTextBox.Text = "FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF FF";
                break;
            case 2:
                AesPlaintextTextBox.Text = string.Empty;
                AesPlaintextTextBox.Text = "F0 F0 F0 F0 F0 F0 F0 F0 F0 F0 F0 F0 F0 F0 F0 F0";
                break;
            case 3:
                AesPlaintextTextBox.Text = string.Empty;
                AesPlaintextTextBox.Text += _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " ";
                AesPlaintextTextBox.Text += _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " ";
                AesPlaintextTextBox.Text += _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " ";
                AesPlaintextTextBox.Text += _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2) + " " + _common.GenerateRandomNumber(2);
                break;
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
        string plaintext = AesPlaintextTextBox.Text.Trim();
        string key = AesKeyTextBox.Text.Trim();
        int plaintextLength = plaintext.Length;
        int keyLength = key.Length;
        int plaintextNoSpaceLength = string.IsNullOrWhiteSpace(plaintext) ? 0 : plaintext.Replace(" ", "").Length;
        int keyNoSpaceLength = string.IsNullOrWhiteSpace(key) ? 0 : key.Replace(" ", "").Length;

        if (plaintextLength != 47 || keyLength != 47 || plaintextNoSpaceLength != 32 || keyNoSpaceLength != 32)
        {
            AesEncryptionStatusInfoBar.Severity = InfoBarSeverity.Error;
            AesEncryptionStatusInfoBar.Message = "Plaintext and key must be 16 bytes ( 32 hexadecimal characters, with spaces between bytes ).";
            AesEncryptionStatusInfoBar.IsOpen = true;
        }
        else if (!System.Text.RegularExpressions.Regex.IsMatch(plaintext, @"^([0-9A-Fa-f]{2}\s){15}[0-9A-Fa-f]{2}$") ||
                 !System.Text.RegularExpressions.Regex.IsMatch(key, @"^([0-9A-Fa-f]{2}\s){15}[0-9A-Fa-f]{2}$"))
        {
            AesEncryptionStatusInfoBar.Severity = InfoBarSeverity.Error;
            AesEncryptionStatusInfoBar.Message = "Plaintext and key must be in hexadecimal format ( 0-9, A-F ).";
            AesEncryptionStatusInfoBar.IsOpen = true;
        }
        else
        {
            AesEncryptionStatusInfoBar.Severity = InfoBarSeverity.Success;
            AesEncryptionStatusInfoBar.Message = "Plaintext and key are in the correct format, ready for 128-Bit Key AES.";
            AesEncryptionStatusInfoBar.IsOpen = true;
        }
    }

    private void AesPlaintextTextBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        AesPlaintextPresetsComboBox.SelectedIndex = 4;
        ResetAesTextBoxes();
    }

    private void AesKeyTextBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        AesKeyPresetsComboBox.SelectedIndex = 4;
        ResetAesTextBoxes();
    }

    private async void AesExecuteEncryptionButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (AesEncryptionStatusInfoBar.Severity != InfoBarSeverity.Success)
        {
            ContentDialog aesExecuteEncryptionParaErrorDialog = new ContentDialog
            {
                Title = "Parameter settings incomplete",
                Content = "Check the parameter inputs and status bar error messages.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            ContentDialogResult checkResult = await aesExecuteEncryptionParaErrorDialog.ShowAsync();
        }
        else
        {
            string plaintext = AesPlaintextTextBox.Text;
            string key = AesKeyTextBox.Text;
            ContentDialog aesExecuteEncryptionParaCheckDialog = new ContentDialog
            {
                Title = "Correct parameter settings?",
                Content = "AES Plaintext:\t" + plaintext + "\r\nAES Key:\t\t" + key,
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Confirmed",
                XamlRoot = this.XamlRoot
            };

            ContentDialogResult checkResult = await aesExecuteEncryptionParaCheckDialog.ShowAsync();
            if (checkResult == ContentDialogResult.Primary)
            {
                AesEncryptCalc(plaintext, key);
            }
        }
    }

    private void ResetAesTextBoxes()
    {
        RoundInitLogTextBox.Text = "The running log generated during the 128-bit key AES will be displayed here.";
        Round1LogTextBox.Text = "The running log generated during the 128-bit key AES will be displayed here.";
        Round2LogTextBox.Text = "The running log generated during the 128-bit key AES will be displayed here.";
        Round3LogTextBox.Text = "The running log generated during the 128-bit key AES will be displayed here.";
        Round4LogTextBox.Text = "The running log generated during the 128-bit key AES will be displayed here.";
        Round5LogTextBox.Text = "The running log generated during the 128-bit key AES will be displayed here.";
        Round6LogTextBox.Text = "The running log generated during the 128-bit key AES will be displayed here.";
        Round7LogTextBox.Text = "The running log generated during the 128-bit key AES will be displayed here.";
        Round8LogTextBox.Text = "The running log generated during the 128-bit key AES will be displayed here.";
        Round9LogTextBox.Text = "The running log generated during the 128-bit key AES will be displayed here.";
        Round10LogTextBox.Text = "The running log generated during the 128-bit key AES will be displayed here.";
        RoundFinalLogTextBox.Text = "The running log generated during the 128-bit key AES will be displayed here.";
    }

    private void CleanAesTextBoxes()
    {
        RoundInitLogTextBox.Text = string.Empty;
        Round1LogTextBox.Text = string.Empty;
        Round2LogTextBox.Text = string.Empty;
        Round3LogTextBox.Text = string.Empty;
        Round4LogTextBox.Text = string.Empty;
        Round5LogTextBox.Text = string.Empty;
        Round6LogTextBox.Text = string.Empty;
        Round7LogTextBox.Text = string.Empty;
        Round8LogTextBox.Text = string.Empty;
        Round9LogTextBox.Text = string.Empty;
        Round10LogTextBox.Text = string.Empty;
        RoundFinalLogTextBox.Text = string.Empty;
    }

    private void AesEncryptCalc(string plaintext, string key)
    {
        var aes = new Components.AES();
        CleanAesTextBoxes();
        Debug.WriteLine($"AES Encrypting with plaintext: {plaintext} and key: {key}");

        var aesRoundKeyTransform = aes.KeyTransformCalc(key);

        var roundInitKeyAdditionLayer = aes.KeyAdditionLayerCalc(plaintext, key);
        RoundInitLogTextBox.Text +=
            "==================== Key Whitening Round ====================\n" +
            $"{"Initial State:",-20} {plaintext}\n" +
            $"{"Key Transform:",-20} {key}\n" +
            $"{"Key Addition:",-20} {roundInitKeyAdditionLayer}\n";

        var round1InitialStateLine = roundInitKeyAdditionLayer;
        var round1ByteSubstitutionLayerLine = aes.ByteSubstitutionLayerCalc(round1InitialStateLine);
        var round1ShiftRowsLayerLine = aes.ShiftRowsLayerCalc(round1ByteSubstitutionLayerLine);
        var round1MixColumnsLayerLine = aes.MixColumnsLayerCalc(round1ShiftRowsLayerLine);
        var round1KeyTransformLine = aesRoundKeyTransform[1];
        var round1KeyAdditionLayer = aes.KeyAdditionLayerCalc(round1MixColumnsLayerLine, round1KeyTransformLine);
        Round1LogTextBox.Text +=
            "==================== Round 1 ====================\n" +
            $"{"Initial State:",-20} {round1InitialStateLine}\n" +
            $"{"Byte Substitution:",-20} {round1ByteSubstitutionLayerLine}\n" +
            $"{"ShiftRows:",-20} {round1ShiftRowsLayerLine}\n" +
            $"{"MixColumn:",-20} {round1MixColumnsLayerLine}\n" +
            $"{"Key Transform:",-20} {round1KeyTransformLine}\n" +
            $"{"Key Addition:",-20} {round1KeyAdditionLayer}\n";

        var round2InitialStateLine = round1KeyAdditionLayer;
        var round2ByteSubstitutionLayerLine = aes.ByteSubstitutionLayerCalc(round2InitialStateLine);
        var round2ShiftRowsLayerLine = aes.ShiftRowsLayerCalc(round2ByteSubstitutionLayerLine);
        var round2MixColumnsLayerLine = aes.MixColumnsLayerCalc(round2ShiftRowsLayerLine);
        var round2KeyTransformLine = aesRoundKeyTransform[2];
        var round2KeyAdditionLayer = aes.KeyAdditionLayerCalc(round2MixColumnsLayerLine, round2KeyTransformLine);
        Round2LogTextBox.Text +=
            "==================== Round 2 ====================\n" +
            $"{"Initial State:",-20} {round2InitialStateLine}\n" +
            $"{"Byte Substitution:",-20} {round2ByteSubstitutionLayerLine}\n" +
            $"{"ShiftRows:",-20} {round2ShiftRowsLayerLine}\n" +
            $"{"MixColumn:",-20} {round2MixColumnsLayerLine}\n" +
            $"{"Key Transform:",-20} {round2KeyTransformLine}\n" +
            $"{"Key Addition:",-20} {round2KeyAdditionLayer}\n";

        var round3InitialStateLine = round2KeyAdditionLayer;
        var round3ByteSubstitutionLayerLine = aes.ByteSubstitutionLayerCalc(round3InitialStateLine);
        var round3ShiftRowsLayerLine = aes.ShiftRowsLayerCalc(round3ByteSubstitutionLayerLine);
        var round3MixColumnsLayerLine = aes.MixColumnsLayerCalc(round3ShiftRowsLayerLine);
        var round3KeyTransformLine = aesRoundKeyTransform[3];
        var round3KeyAdditionLayer = aes.KeyAdditionLayerCalc(round3MixColumnsLayerLine, round3KeyTransformLine);
        Round3LogTextBox.Text +=
            "==================== Round 3 ====================\n" +
            $"{"Initial State:",-20} {round3InitialStateLine}\n" +
            $"{"Byte Substitution:",-20} {round3ByteSubstitutionLayerLine}\n" +
            $"{"ShiftRows:",-20} {round3ShiftRowsLayerLine}\n" +
            $"{"MixColumn:",-20} {round3MixColumnsLayerLine}\n" +
            $"{"Key Transform:",-20} {round3KeyTransformLine}\n" +
            $"{"Key Addition:",-20} {round3KeyAdditionLayer}\n";

        var round4InitialStateLine = round3KeyAdditionLayer;
        var round4ByteSubstitutionLayerLine = aes.ByteSubstitutionLayerCalc(round4InitialStateLine);
        var round4ShiftRowsLayerLine = aes.ShiftRowsLayerCalc(round4ByteSubstitutionLayerLine);
        var round4MixColumnsLayerLine = aes.MixColumnsLayerCalc(round4ShiftRowsLayerLine);
        var round4KeyTransformLine = aesRoundKeyTransform[4];
        var round4KeyAdditionLayer = aes.KeyAdditionLayerCalc(round4MixColumnsLayerLine, round4KeyTransformLine);
        Round4LogTextBox.Text +=
            "==================== Round 4 ====================\n" +
            $"{"Initial State:",-20} {round4InitialStateLine}\n" +
            $"{"Byte Substitution:",-20} {round4ByteSubstitutionLayerLine}\n" +
            $"{"ShiftRows:",-20} {round4ShiftRowsLayerLine}\n" +
            $"{"MixColumn:",-20} {round4MixColumnsLayerLine}\n" +
            $"{"Key Transform:",-20} {round4KeyTransformLine}\n" +
            $"{"Key Addition:",-20} {round4KeyAdditionLayer}\n";

        var round5InitialStateLine = round4KeyAdditionLayer;
        var round5ByteSubstitutionLayerLine = aes.ByteSubstitutionLayerCalc(round5InitialStateLine);
        var round5ShiftRowsLayerLine = aes.ShiftRowsLayerCalc(round5ByteSubstitutionLayerLine);
        var round5MixColumnsLayerLine = aes.MixColumnsLayerCalc(round5ShiftRowsLayerLine);
        var round5KeyTransformLine = aesRoundKeyTransform[5];
        var round5KeyAdditionLayer = aes.KeyAdditionLayerCalc(round5MixColumnsLayerLine, round5KeyTransformLine);
        Round5LogTextBox.Text +=
            "==================== Round 5 ====================\n" +
            $"{"Initial State:",-20} {round5InitialStateLine}\n" +
            $"{"Byte Substitution:",-20} {round5ByteSubstitutionLayerLine}\n" +
            $"{"ShiftRows:",-20} {round5ShiftRowsLayerLine}\n" +
            $"{"MixColumn:",-20} {round5MixColumnsLayerLine}\n" +
            $"{"Key Transform:",-20} {round5KeyTransformLine}\n" +
            $"{"Key Addition:",-20} {round5KeyAdditionLayer}\n";

        var round6InitialStateLine = round5KeyAdditionLayer;
        var round6ByteSubstitutionLayerLine = aes.ByteSubstitutionLayerCalc(round6InitialStateLine);
        var round6ShiftRowsLayerLine = aes.ShiftRowsLayerCalc(round6ByteSubstitutionLayerLine);
        var round6MixColumnsLayerLine = aes.MixColumnsLayerCalc(round6ShiftRowsLayerLine);
        var round6KeyTransformLine = aesRoundKeyTransform[6];
        var round6KeyAdditionLayer = aes.KeyAdditionLayerCalc(round6MixColumnsLayerLine, round6KeyTransformLine);
        Round6LogTextBox.Text +=
            "==================== Round 6 ====================\n" +
            $"{"Initial State:",-20} {round6InitialStateLine}\n" +
            $"{"Byte Substitution:",-20} {round6ByteSubstitutionLayerLine}\n" +
            $"{"ShiftRows:",-20} {round6ShiftRowsLayerLine}\n" +
            $"{"MixColumn:",-20} {round6MixColumnsLayerLine}\n" +
            $"{"Key Transform:",-20} {round6KeyTransformLine}\n" +
            $"{"Key Addition:",-20} {round6KeyAdditionLayer}\n";

        var round7InitialStateLine = round6KeyAdditionLayer;
        var round7ByteSubstitutionLayerLine = aes.ByteSubstitutionLayerCalc(round7InitialStateLine);
        var round7ShiftRowsLayerLine = aes.ShiftRowsLayerCalc(round7ByteSubstitutionLayerLine);
        var round7MixColumnsLayerLine = aes.MixColumnsLayerCalc(round7ShiftRowsLayerLine);
        var round7KeyTransformLine = aesRoundKeyTransform[7];
        var round7KeyAdditionLayer = aes.KeyAdditionLayerCalc(round7MixColumnsLayerLine, round7KeyTransformLine);
        Round7LogTextBox.Text +=
            "==================== Round 7 ====================\n" +
            $"{"Initial State:",-20} {round7InitialStateLine}\n" +
            $"{"Byte Substitution:",-20} {round7ByteSubstitutionLayerLine}\n" +
            $"{"ShiftRows:",-20} {round7ShiftRowsLayerLine}\n" +
            $"{"MixColumn:",-20} {round7MixColumnsLayerLine}\n" +
            $"{"Key Transform:",-20} {round7KeyTransformLine}\n" +
            $"{"Key Addition:",-20} {round7KeyAdditionLayer}\n";

        var round8InitialStateLine = round7KeyAdditionLayer;
        var round8ByteSubstitutionLayerLine = aes.ByteSubstitutionLayerCalc(round8InitialStateLine);
        var round8ShiftRowsLayerLine = aes.ShiftRowsLayerCalc(round8ByteSubstitutionLayerLine);
        var round8MixColumnsLayerLine = aes.MixColumnsLayerCalc(round8ShiftRowsLayerLine);
        var round8KeyTransformLine = aesRoundKeyTransform[8];
        var round8KeyAdditionLayer = aes.KeyAdditionLayerCalc(round8MixColumnsLayerLine, round8KeyTransformLine);
        Round8LogTextBox.Text +=
            "==================== Round 8 ====================\n" +
            $"{"Initial State:",-20} {round8InitialStateLine}\n" +
            $"{"Byte Substitution:",-20} {round8ByteSubstitutionLayerLine}\n" +
            $"{"ShiftRows:",-20} {round8ShiftRowsLayerLine}\n" +
            $"{"MixColumn:",-20} {round8MixColumnsLayerLine}\n" +
            $"{"Key Transform:",-20} {round8KeyTransformLine}\n" +
            $"{"Key Addition:",-20} {round8KeyAdditionLayer}\n";

        var round9InitialStateLine = round8KeyAdditionLayer;
        var round9ByteSubstitutionLayerLine = aes.ByteSubstitutionLayerCalc(round9InitialStateLine);
        var round9ShiftRowsLayerLine = aes.ShiftRowsLayerCalc(round9ByteSubstitutionLayerLine);
        var round9MixColumnsLayerLine = aes.MixColumnsLayerCalc(round9ShiftRowsLayerLine);
        var round9KeyTransformLine = aesRoundKeyTransform[9];
        var round9KeyAdditionLayer = aes.KeyAdditionLayerCalc(round9MixColumnsLayerLine, round9KeyTransformLine);
        Round9LogTextBox.Text +=
            "==================== Round 9 ====================\n" +
            $"{"Initial State:",-20} {round9InitialStateLine}\n" +
            $"{"Byte Substitution:",-20} {round9ByteSubstitutionLayerLine}\n" +
            $"{"ShiftRows:",-20} {round9ShiftRowsLayerLine}\n" +
            $"{"MixColumn:",-20} {round9MixColumnsLayerLine}\n" +
            $"{"Key Transform:",-20} {round9KeyTransformLine}\n" +
            $"{"Key Addition:",-20} {round9KeyAdditionLayer}\n";

        var round10InitialStateLine = round9KeyAdditionLayer;
        var round10ByteSubstitutionLayerLine = aes.ByteSubstitutionLayerCalc(round10InitialStateLine);
        var round10ShiftRowsLayerLine = aes.ShiftRowsLayerCalc(round10ByteSubstitutionLayerLine);
        var round10KeyTransformLine = aesRoundKeyTransform[10];
        var round10KeyAdditionLayer = aes.KeyAdditionLayerCalc(round10ShiftRowsLayerLine, round10KeyTransformLine);
        Round10LogTextBox.Text +=
            "==================== Round 10 ====================\n" +
            $"{"Initial State:",-20} {round10InitialStateLine}\n" +
            $"{"Byte Substitution:",-20} {round10ByteSubstitutionLayerLine}\n" +
            $"{"ShiftRows:",-20} {round10ShiftRowsLayerLine}\n" +
            $"{"Key Transform:",-20} {round10KeyTransformLine}\n" +
            $"{"Key Addition:",-20} {round10KeyAdditionLayer}\n";

        var roundFinalCiphertextLine = round10KeyAdditionLayer;
        RoundFinalLogTextBox.Text +=
            "==================== Final Round ====================\n" +
            $"{"Ciphertext:",-20} {roundFinalCiphertextLine}\n";
        
        AesEncryptionStatusInfoBar.Severity = InfoBarSeverity.Success;
        AesEncryptionStatusInfoBar.Message = "AES encryption completed successfully. Check the logs for details.";
        AesEncryptionStatusInfoBar.IsOpen = true;
    }
}
