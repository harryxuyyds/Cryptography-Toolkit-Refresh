using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using DeepSeek.Core;
using Microsoft.Extensions.AI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OpenAI;
using Windows.Foundation;
using Windows.Foundation.Collections;
using DeepSeek.Core.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Cryptography_Toolkit.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AIPoweredSearchPage : Page
{
    public AIPoweredSearchPage()
    {
        InitializeComponent();
        // TestDeepSeek();
    }

    private async void TestDeepSeek()
    {
        var apiKey = "sk-";
        // 通过apiKey创建实例
        var client = new DeepSeekClient(apiKey);

        var modelResponse = await client.ListModelsAsync(CancellationToken.None);
        if (modelResponse is null)
        {
            Debug.WriteLine(client.ErrorMsg);
            return;
        }
        foreach (var model in modelResponse.Data)
        {
            Debug.WriteLine(model);
        }
    }
}
