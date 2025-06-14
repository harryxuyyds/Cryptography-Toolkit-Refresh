using System;
using System.Diagnostics;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Cryptography_Toolkit.Components;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cryptography_Toolkit.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    
    public sealed partial class ShiftCipherPage : Page
    {
        private readonly ShiftCipher _shiftCipher;

        public ShiftCipherPage()
        {
            this.InitializeComponent();
            _shiftCipher = new ShiftCipher();
            ShiftCipherKNumberBox.ValueChanged += ShiftCipherNumberBox_OnValueChanged;
            ShiftCipherCharactersHandleComboBox.SelectionChanged += ShiftCipherComboBox_OnSelectionChanged;
            ShiftCipherModeToggleSwitch.Toggled += ShiftCipherToggleSwitch_OnToggled;
            ShiftCipherMessageTextBox.TextChanged += ShiftCipherTextBox_OnTextChanged;
        }

        private void ShiftCipherTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // 获取移位值(k)
            var shiftValue = (int)Math.Round(ShiftCipherKNumberBox.Value);
            
            // 获取字符处理方式的选择索引
            var selectedCharacterMode = ShiftCipherCharactersHandleComboBox.SelectedIndex;
            
            // 获取加密/解密模式开关状态
            var isEncryptMode = ShiftCipherModeToggleSwitch.IsOn;

            // 获取输入消息文本
            string messageText = string.Empty;
            if (ShiftCipherMessageTextBox != null)
            {
                messageText = ShiftCipherMessageTextBox.Text;
            }

            if (isEncryptMode)
            {
                string ciphertext = _shiftCipher.ShiftCipher_Encrypt(messageText, shiftValue, selectedCharacterMode);
                ShiftCipherResultTextBox.Text = ciphertext;
            }
            else
            {
                string plaintext = _shiftCipher.ShiftCipher_Decrypt(messageText, shiftValue, selectedCharacterMode);
                ShiftCipherResultTextBox.Text = plaintext;
            }
        }

        private void ShiftCipherComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 获取移位值(k)
            var shiftValue = (int)Math.Round(ShiftCipherKNumberBox.Value);

            // 获取字符处理方式的选择索引
            var selectedCharacterMode = ShiftCipherCharactersHandleComboBox.SelectedIndex;

            // 获取加密/解密模式开关状态
            var isEncryptMode = ShiftCipherModeToggleSwitch.IsOn;

            // 获取输入消息文本
            string messageText = string.Empty;
            if (ShiftCipherMessageTextBox != null)
            {
                messageText = ShiftCipherMessageTextBox.Text;
            }

            if (isEncryptMode)
            {
                string ciphertext = _shiftCipher.ShiftCipher_Encrypt(messageText, shiftValue, selectedCharacterMode);
                ShiftCipherResultTextBox.Text = ciphertext;
            }
            else
            {
                string plaintext = _shiftCipher.ShiftCipher_Decrypt(messageText, shiftValue, selectedCharacterMode);
                ShiftCipherResultTextBox.Text = plaintext;
            }
        }

        private void ShiftCipherToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            // 获取移位值(k)
            var shiftValue = (int)Math.Round(ShiftCipherKNumberBox.Value);

            // 获取字符处理方式的选择索引
            var selectedCharacterMode = ShiftCipherCharactersHandleComboBox.SelectedIndex;

            // 获取加密/解密模式开关状态
            var isEncryptMode = ShiftCipherModeToggleSwitch.IsOn;

            // 获取输入消息文本
            string messageText = string.Empty;
            if (ShiftCipherMessageTextBox != null)
            {
                messageText = ShiftCipherMessageTextBox.Text;
            }

            if (isEncryptMode)
            {
                string ciphertext = _shiftCipher.ShiftCipher_Encrypt(messageText, shiftValue, selectedCharacterMode);
                ShiftCipherResultTextBox.Text = ciphertext;
            }
            else
            {
                string plaintext = _shiftCipher.ShiftCipher_Decrypt(messageText, shiftValue, selectedCharacterMode);
                ShiftCipherResultTextBox.Text = plaintext;
            }
        }

        private void ShiftCipherNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            // 获取移位值(k)
            var shiftValue = (int)Math.Round(ShiftCipherKNumberBox.Value);

            // 获取字符处理方式的选择索引
            var selectedCharacterMode = ShiftCipherCharactersHandleComboBox.SelectedIndex;

            // 获取加密/解密模式开关状态
            var isEncryptMode = ShiftCipherModeToggleSwitch.IsOn;

            // 获取输入消息文本
            string messageText = string.Empty;
            if (ShiftCipherMessageTextBox != null)
            {
                messageText = ShiftCipherMessageTextBox.Text;
            }

            if (isEncryptMode)
            {
                string ciphertext = _shiftCipher.ShiftCipher_Encrypt(messageText, shiftValue, selectedCharacterMode);
                ShiftCipherResultTextBox.Text = ciphertext;
            }
            else
            {
                string plaintext = _shiftCipher.ShiftCipher_Decrypt(messageText, shiftValue, selectedCharacterMode);
                ShiftCipherResultTextBox.Text = plaintext;
            }
        }
    }
}
