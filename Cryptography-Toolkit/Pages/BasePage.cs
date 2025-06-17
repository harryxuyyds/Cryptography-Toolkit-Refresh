using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Storage;
using Microsoft.UI.Xaml;

namespace Cryptography_Toolkit.Pages;

public class BasePage : Page
{
    public BasePage()
    {
        this.Loaded += BasePage_Loaded;
    }

    private void BasePage_Loaded(object sender, RoutedEventArgs e)
    {
        ApplyEditorFontFamily();
    }

    protected void ApplyEditorFontFamily()
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        if (localSettings.Values.TryGetValue("EditorFontFamily", out object? fontName) && fontName is string fontFamilyName)
        {
            var fontFamily = new FontFamily(fontFamilyName);
            UpdateTextBoxFontFamilyRecursive(this, fontFamily);
        }
    }

    private void UpdateTextBoxFontFamilyRecursive(DependencyObject parent, FontFamily fontFamily)
    {
        int count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is TextBox textBox)
            {
                textBox.FontFamily = fontFamily;
            }
            UpdateTextBoxFontFamilyRecursive(child, fontFamily);
        }
    }
}