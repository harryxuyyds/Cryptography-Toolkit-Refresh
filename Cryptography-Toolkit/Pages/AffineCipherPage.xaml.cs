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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cryptography_Toolkit.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AffineCipherPage : Page
{
    public AffineCipherPage()
    {
        InitializeComponent();
        AffineCipherAComboBox.SelectionChanged += AffineCipherComboBox_OnSelectionChanged;
        AffineCipherBNumberBox.ValueChanged += AffineCipherNumberBox_OnValueChanged;
        AffineCipherCharactersHandleComboBox.SelectionChanged += AffineCipherComboBox_OnSelectionChanged;
        AffineCipherModeToggleSwitch.Toggled += AffineCipherToggleSwitch_OnToggled;
    }

    private void AffineCipherComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var aStr = AffineCipherAComboBox.SelectedItem?.ToString();
        var a = !string.IsNullOrEmpty(aStr) ? int.Parse(aStr) : 1;
        var b = (int)AffineCipherBNumberBox.Value;
        var charactersHandle = AffineCipherCharactersHandleComboBox.SelectedItem?.ToString();
        var isEncryptMode = AffineCipherModeToggleSwitch.IsOn;
        
    }

    private void AffineCipherNumberBox_OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        throw new NotImplementedException();
    }

    private void AffineCipherToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}
