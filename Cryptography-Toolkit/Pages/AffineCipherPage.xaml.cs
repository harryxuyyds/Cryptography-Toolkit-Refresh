using System;
using System.Collections.Generic;
using System.Diagnostics;
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

using Cryptography_Toolkit.Components;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cryptography_Toolkit.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AffineCipherPage : Page
{
    private readonly AffineCipher _affineCipher;

    public AffineCipherPage()
    {
        InitializeComponent();
        _affineCipher = new AffineCipher();
        AffineCipherAComboBox.SelectionChanged += AffineCipherComboBox_OnSelectionChanged;
        AffineCipherBNumberBox.ValueChanged += AffineCipherNumberBox_OnValueChanged;
        AffineCipherCharactersHandleComboBox.SelectionChanged += AffineCipherComboBox_OnSelectionChanged;
        AffineCipherModeToggleSwitch.Toggled += AffineCipherToggleSwitch_OnToggled;
        AffineCipherMessageTextBox.TextChanged += AffineCipherMessageTextBox_OnTextChanged;
    }

    private void AffineCipherComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        AffineCipherSetup();
    }

    private void AffineCipherNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        AffineCipherSetup();
    }

    private void AffineCipherToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        AffineCipherSetup();
    }

    private void AffineCipherMessageTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        AffineCipherSetup();
    }

    private void AffineCipherSetup()
    {
        Debug.WriteLine("111111");

        // Ensure AffineCipherAComboBox.SelectedItem is not null and cast safely
        if (AffineCipherAComboBox.SelectedItem is ComboBoxItem selectedItemA &&
            int.TryParse(selectedItemA.Content?.ToString(), out var a))
        {
            var b = (int)Math.Round(AffineCipherBNumberBox.Value);
            var selectedCharacterMode = AffineCipherCharactersHandleComboBox.SelectedIndex;
            var isEncryptMode = AffineCipherModeToggleSwitch.IsOn;

            var messageText = string.Empty;
            if (AffineCipherMessageTextBox != null)
            {
                messageText = AffineCipherMessageTextBox.Text;
            }

            if (isEncryptMode)
            {
                var ciphertext = _affineCipher.AffineCipher_Encrypt(messageText, b, a, selectedCharacterMode);
                AffineCipherResultTextBox.Text = ciphertext;
            }
            else
            {
                var plaintext = _affineCipher.AffineCipher_Decrypt(messageText, b, a, selectedCharacterMode);
                AffineCipherResultTextBox.Text = plaintext;
            }
        }
        else
        {
            AffineCipherResultTextBox.Text = "AffineCipherAComboBox.SelectedItem is null or invalid.";
        }
    }
}
