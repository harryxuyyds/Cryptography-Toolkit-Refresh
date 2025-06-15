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
public sealed partial class StreamCiphersPage : Page
{
    public StreamCiphersPage()
    {
        InitializeComponent();
    }

    private void PageNavigationHyperlinkButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is HyperlinkButton button)
        {
            // 获取目标页面名称
            string pageName = button.Tag?.ToString() ?? string.Empty;

            // 执行导航
            // Frame.Navigate(Type.GetType($"Cryptography_Toolkit.Pages.{pageName}"));

            // 更新 NavigationView 选中项
            if (App.MainWindow?.Content is NavigationRootPage rootPage) // 添加空值检查
            {
                rootPage.NavigateToPage(pageName); // 传入目标页面的Tag
            }
        }
    }
}
