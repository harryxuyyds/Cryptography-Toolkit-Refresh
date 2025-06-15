using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cryptography_Toolkit.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class NavigationRootPage : Page
{
    private List<(string Tag, string Title)> _allPages;

    public NavigationRootPage()
    {
        InitializeComponent();
        this.Loaded += NavigationRootPage_Loaded;

        // ���� NavigationView �� DisplayModeChanged �¼�
        NavigationViewControl.DisplayModeChanged += NavigationViewControl_OnDisplayModeChanged;
        NavigationViewControl.SelectionChanged += NavigationViewControl_OnSelectionChanged;

        // ��ʼ������ҳ������
        _allPages = new List<(string Tag, string Title)>();
        foreach (NavigationViewItem item in NavigationViewControl.MenuItems.OfType<NavigationViewItem>())
        {
            if (item.Tag != null)
            {
                _allPages.Add((item.Tag.ToString(), item.Content.ToString()));
            }
            if (item.MenuItems.Count > 0)
            {
                foreach (NavigationViewItem subItem in item.MenuItems.OfType<NavigationViewItem>())
                {
                    if (subItem.Tag != null)
                    {
                        _allPages.Add((subItem.Tag.ToString(), subItem.Content.ToString()));
                    }
                }
            }
        }

        // ����AutoSuggestBox�¼�����
        ComponentsSearchBox.TextChanged += ComponentsSearchBox_TextChanged;
        ComponentsSearchBox.SuggestionChosen += ComponentsSearchBox_SuggestionChosen;
        ComponentsSearchBox.QuerySubmitted += ComponentsSearchBox_QuerySubmitted;
    }

    private void NavigationRootPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            var appWindow = App.MainWindow?.AppWindow;
            // RootFrame.Navigate(typeof(Cryptography_Toolkit.Pages.HomePage));
            NavigateToPage("HomePage"); // ����Ŀ��ҳ���Tag

            if (appWindow != null)
            {
                var titleBar = appWindow.TitleBar;
                titleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

                // ���ô��ڱ���
                if (App.MainWindow != null) // ��ӿ����ü��
                {
                    App.MainWindow.Title = AppTitleText;
                }

                // ���� XAML �е� AppTitle TextBlock
                if (AppTitle != null)
                {
                    AppTitle.Text = AppTitleText;
                }
            }
        }

        // ���Ĵ��ڼ���״̬�ı��¼�
        if (App.MainWindow != null)
        {
            App.MainWindow.Activated += MainWindow_Activated;
        }
    }

    private void NavigationViewControl_OnDisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        if (sender.PaneDisplayMode == NavigationViewPaneDisplayMode.Top)
        {
            VisualStateManager.GoToState(this, "Top", true);
        }
        else
        {
            if (args.DisplayMode == NavigationViewDisplayMode.Minimal)
            {
                VisualStateManager.GoToState(this, "Compact", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Default", true);
            }
        }
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState == WindowActivationState.Deactivated)
        {
            VisualStateManager.GoToState(this, "Deactivated", true);
        }
        else
        {
            VisualStateManager.GoToState(this, "Activated", true);
        }
    }

    public string AppTitleText
    {
        get
        {
#if DEBUG
            return "Cryptography Toolkit Dev";
#else
            return "Cryptography Toolkit";
#endif
        }
    }

    private void NavigationViewControl_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            RootFrame.Navigate(typeof(Cryptography_Toolkit.Pages.ToolkitSettingsPage));
            return;
        }

        if (args.SelectedItem is not NavigationViewItem selectedItem)
        {
            return;
        }

        string? selectedItemTag = selectedItem.Tag?.ToString();
        if (string.IsNullOrEmpty(selectedItemTag))
        {
            return;
        }

        string pageName = $"Cryptography_Toolkit.Pages.{selectedItemTag}";
        
        Type? pageType = Type.GetType(pageName);
        
        if (pageType == null)
        {
            Debug.WriteLine($"�޷��ҵ�ҳ������: {pageName}");
            return;
        }

        RootFrame.Navigate(pageType, null, new DrillInNavigationTransitionInfo());
    }

    public void NavigateToPage(string tag)
    {
        // NavigationViewControl.SelectionChanged -= NavigationViewControl_OnSelectionChanged;
        var matchingItem = FindMenuItemRecursively(tag, NavigationViewControl.MenuItems);
        if (matchingItem != null)
        {
            NavigationViewControl.SelectedItem = matchingItem;
        }
        // NavigationViewControl.SelectionChanged += NavigationViewControl_OnSelectionChanged;
    }

    private NavigationViewItem? FindMenuItemRecursively(string tag, IList<object> menuItems)
    {
        foreach (var item in menuItems)
        {
            if (item is NavigationViewItem navItem)
            {
                // ��鵱ǰ��
                if (navItem.Tag?.ToString() == tag)
                {
                    return navItem;
                }

                // �ݹ�������
                if (navItem.MenuItems.Count > 0)
                {
                    var childResult = FindMenuItemRecursively(tag, navItem.MenuItems);
                    if (childResult != null)
                    {
                        // ������������ҵ�ƥ���չ������
                        navItem.IsExpanded = true;
                        return childResult;
                    }
                }
            }
        }
        return null;
    }

    private void NavigationViewControl_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        if (RootFrame.CanGoBack)
        {
            RootFrame.GoBack();

            // ��ȡ��ǰҳ������
            if (RootFrame.Content is Page currentPage)
            {
                // ����ҳ�����Ͳ��Ҷ�Ӧ�� NavigationViewItem
                var matchingItem = FindNavigationViewItemByPageType(currentPage.GetType());
                if (matchingItem != null)
                {
                    // ����ѡ����
                    NavigationViewControl.SelectedItem = matchingItem;
                }
            }
        }
    }

    // �޸�������ʹ�� null-forgiving ����� (!) ��ȷ��֪�������˴����� null ��Ԥ����Ϊ��
    private NavigationViewItem? FindNavigationViewItemByPageType(Type pageType)
    {
        foreach (var item in NavigationViewControl.MenuItems)
        {
            if (item is NavigationViewItem navigationViewItem)
            {
                string? tag = navigationViewItem.Tag?.ToString();
                if (!string.IsNullOrEmpty(tag) && tag == pageType.Name)
                {
                    return navigationViewItem;
                }

                // �������
                if (navigationViewItem.MenuItems.Count > 0)
                {
                    foreach (var subItem in navigationViewItem.MenuItems)
                    {
                        if (subItem is NavigationViewItem subNavItem)
                        {
                                string? subTag = subNavItem.Tag?.ToString();
                            if (!string.IsNullOrEmpty(subTag) && subTag == pageType.Name)
                            {
                                return subNavItem;
                            }
                        }
                    }
                }
            }
        }
        // �޸��������׳��쳣�Ա��ⷵ�� null��
        throw new InvalidOperationException($"δ�ҵ���ҳ������ {pageType.Name} ��Ӧ�� NavigationViewItem��");
    }

    private void ComponentsSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            var suggestions = _allPages
                .Where(p => p.Title.Contains(sender.Text, StringComparison.OrdinalIgnoreCase))
                .Select(p => p.Title)
                .ToList();
            
            sender.ItemsSource = suggestions;
        }
    }

    private void ComponentsSearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is string selectedTitle)
        {
            sender.Text = selectedTitle;
        }
    }

    private void ComponentsSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        string queryText = args.QueryText;
        var matchingPage = _allPages.FirstOrDefault(p => 
            p.Title.Equals(queryText, StringComparison.OrdinalIgnoreCase));
            
        if (matchingPage != default)
        {
            // ������ѡ�е�ҳ��
            RootFrame.Navigate(Type.GetType($"Cryptography_Toolkit.Pages.{matchingPage.Tag}"));
            NavigationViewControl.SelectedItem = NavigationViewControl.MenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(n => n.Tag?.ToString() == matchingPage.Tag);
        }
    }

    private void CtrlF_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        ComponentsSearchBox.Focus(FocusState.Programmatic);
    }
}