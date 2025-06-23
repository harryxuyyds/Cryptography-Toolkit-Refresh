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
public sealed partial class SubstitutionCipherPage : Page
{
    public SubstitutionCipherPage()
    {
        InitializeComponent();
        SubstitutionCipherPlaintextAlphabetTextBox.KeyDown += SubstitutionCipherPlaintextAlphabetTextBox_OnKeyDown;
        SubstitutionCipherPlaintextAlphabetTextBox.TextChanged += SubstitutionCipherTextBox_OnTextChanged;
        SubstitutionCipherCiphertextAlphabetTextBox.KeyDown += SubstitutionCipherCiphertextAlphabetTextBox_OnKeyDown;
        SubstitutionCipherCiphertextAlphabetTextBox.TextChanged += SubstitutionCipherTextBox_OnTextChanged;
        SubstitutionCipherModeToggleSwitch.Toggled += SubstitutionCipherModeToggleSwitch_OnToggled;
        SubstitutionCipherCharactersHandleComboBox.SelectionChanged += SubstitutionCipherCharactersHandleComboBox_OnSelectionChanged;
    }

    private void SubstitutionCipherPlaintextAlphabetPresetsComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int substitutionCipherPlaintextTypeIndex = SubstitutionCipherPlaintextAlphabetPresetsComboBox.SelectedIndex;
        var alphabetOriginal = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        switch (substitutionCipherPlaintextTypeIndex)
        {
            case 0:
                SubstitutionCipherPlaintextAlphabetTextBox.Text = string.Empty;
                SubstitutionCipherPlaintextAlphabetTextBox.Text = alphabetOriginal;
                break;
            case 1:
                SubstitutionCipherPlaintextAlphabetTextBox.Text = string.Empty;
                var alphabetReversed = new string(alphabetOriginal.Reverse().ToArray());
                SubstitutionCipherPlaintextAlphabetTextBox.Text = alphabetReversed;
                break;
            case 2:
                SubstitutionCipherPlaintextAlphabetTextBox.Text = string.Empty;
                var random = new Random();
                var alphabetShuffled = new string(alphabetOriginal.OrderBy(_ => random.Next()).ToArray());
                SubstitutionCipherPlaintextAlphabetTextBox.Text = alphabetShuffled;
                break;
        }
    }

    private void SubstitutionCipherCiphertextAlphabetPresetsComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int substitutionCipherCiphertextTypeIndex = SubstitutionCipherCiphertextAlphabetPresetsComboBox.SelectedIndex;
        var alphabetOriginal = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        switch (substitutionCipherCiphertextTypeIndex)
        {
            case 0:
                SubstitutionCipherCiphertextAlphabetTextBox.Text = string.Empty;
                SubstitutionCipherCiphertextAlphabetTextBox.Text = alphabetOriginal;
                break;
            case 1:
                SubstitutionCipherCiphertextAlphabetTextBox.Text = string.Empty;
                var alphabetReversed = new string(alphabetOriginal.Reverse().ToArray());
                SubstitutionCipherCiphertextAlphabetTextBox.Text = alphabetReversed;
                break;
            case 2:
                SubstitutionCipherCiphertextAlphabetTextBox.Text = string.Empty;
                var random = new Random();
                var alphabetShuffled = new string(alphabetOriginal.OrderBy(_ => random.Next()).ToArray());
                SubstitutionCipherCiphertextAlphabetTextBox.Text = alphabetShuffled;
                break;
        }
    }

    private void SubstitutionCipherPlaintextAlphabetTextBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        SubstitutionCipherPlaintextAlphabetPresetsComboBox.SelectedIndex = 3;
    }

    private void SubstitutionCipherCiphertextAlphabetTextBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        SubstitutionCipherCiphertextAlphabetPresetsComboBox.SelectedIndex = 3;
    }

    private void SubstitutionCipherTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        SubstitutionCipherSetup();
    }

    private void SubstitutionCipherModeToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        SubstitutionCipherSetup();
    }

    private void SubstitutionCipherCharactersHandleComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SubstitutionCipherSetup();
    }

    private void SubstitutionCipherSetup()
    {
        var substitutionCipher = new Components.SubstitutionCipher();
        if (SubstitutionCipherPlaintextAlphabetTextBox != null && SubstitutionCipherCiphertextAlphabetTextBox != null)
        {
            var plaintextAlphabet = SubstitutionCipherPlaintextAlphabetTextBox.Text?.Trim() ?? string.Empty;
            var ciphertextAlphabet = SubstitutionCipherCiphertextAlphabetTextBox.Text?.Trim() ?? string.Empty;

            var selectedCharacterMode = SubstitutionCipherCharactersHandleComboBox.SelectedIndex;
            var isEncryptMode = SubstitutionCipherModeToggleSwitch.IsOn;

            var messageText = SubstitutionCipherMessageTextBox?.Text ?? string.Empty;

            var preCheckResult = substitutionCipher.SubstitutionCipherPreCheck(plaintextAlphabet, ciphertextAlphabet);
            if (preCheckResult != 0)
            {
                SubstitutionCipherStatusInfoBar.Severity = InfoBarSeverity.Warning;
                SubstitutionCipherResultTextBox.Text = preCheckResult switch
                {
                    1 => "Plaintext and ciphertext alphabets have different lengths.",
                    2 => "Plaintext or ciphertext alphabet contains non-letter characters.",
                    3 => "Plaintext and ciphertext alphabets have different numbers of unique characters.",
                    _ => "Unknown error"
                };
                SubstitutionCipherStatusInfoBar.Message = SubstitutionCipherResultTextBox.Text;
                return;
            }

            if (isEncryptMode)
            {
                string ciphertext = substitutionCipher.SubstitutionCipherEncrypt(plaintextAlphabet, ciphertextAlphabet, messageText, selectedCharacterMode);
                SubstitutionCipherResultTextBox.Text = ciphertext;
                SubstitutionCipherStatusInfoBar.Severity = InfoBarSeverity.Success;
                SubstitutionCipherStatusInfoBar.Message = "Encryption successful!";
            }
            else
            {
                string plaintext = substitutionCipher.SubstitutionCipherDecrypt(plaintextAlphabet, ciphertextAlphabet, messageText, selectedCharacterMode);
                SubstitutionCipherResultTextBox.Text = plaintext;
                SubstitutionCipherStatusInfoBar.Severity = InfoBarSeverity.Success;
                SubstitutionCipherStatusInfoBar.Message = "Decryption successful!";
            }
        }
    }
}
