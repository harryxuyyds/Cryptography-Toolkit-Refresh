using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Cryptography_Toolkit.Components;
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
public sealed partial class AllComponentsPage : Page
{
    private readonly NavigationRootPage _navigationRoot;

    public AllComponentsPage()
    {
        InitializeComponent();
        _navigationRoot = new NavigationRootPage();
    }

    private void PageNavigationHyperlinkButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is HyperlinkButton button)
        {
            // ��ȡĿ��ҳ������
            string pageName = button.Tag?.ToString() ?? string.Empty;

            // ִ�е���
            // Frame.Navigate(Type.GetType($"Cryptography_Toolkit.Pages.{pageName}"));

            // ���� NavigationView ѡ����
            // ������ҳ����
            if (App.MainWindow?.Content is NavigationRootPage rootPage) // ��ӿ�ֵ���
            {
                Debug.WriteLine("222"+pageName);
                rootPage.NavigateToPage(pageName); // ����Ŀ��ҳ���Tag
            }
        }
    }
}
