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
            // ��ȡ��λֵ(k)
            var shiftValue = (int)Math.Round(ShiftCipherKNumberBox.Value);
            
            // ��ȡ�ַ�����ʽ��ѡ������
            var selectedCharacterMode = ShiftCipherCharactersHandleComboBox.SelectedIndex;
            
            // ��ȡ����/����ģʽ����״̬
            var isEncryptMode = ShiftCipherModeToggleSwitch.IsOn;

            // ��ȡ������Ϣ�ı�
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
            // ��ȡ��λֵ(k)
            var shiftValue = (int)Math.Round(ShiftCipherKNumberBox.Value);

            // ��ȡ�ַ�����ʽ��ѡ������
            var selectedCharacterMode = ShiftCipherCharactersHandleComboBox.SelectedIndex;

            // ��ȡ����/����ģʽ����״̬
            var isEncryptMode = ShiftCipherModeToggleSwitch.IsOn;

            // ��ȡ������Ϣ�ı�
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
            // ��ȡ��λֵ(k)
            var shiftValue = (int)Math.Round(ShiftCipherKNumberBox.Value);

            // ��ȡ�ַ�����ʽ��ѡ������
            var selectedCharacterMode = ShiftCipherCharactersHandleComboBox.SelectedIndex;

            // ��ȡ����/����ģʽ����״̬
            var isEncryptMode = ShiftCipherModeToggleSwitch.IsOn;

            // ��ȡ������Ϣ�ı�
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
            // ��ȡ��λֵ(k)
            var shiftValue = (int)Math.Round(ShiftCipherKNumberBox.Value);

            // ��ȡ�ַ�����ʽ��ѡ������
            var selectedCharacterMode = ShiftCipherCharactersHandleComboBox.SelectedIndex;

            // ��ȡ����/����ģʽ����״̬
            var isEncryptMode = ShiftCipherModeToggleSwitch.IsOn;

            // ��ȡ������Ϣ�ı�
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
