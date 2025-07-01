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
        AiPoweredSearchPreCheck();
    }

    private void AiPoweredSearchPreCheck()
    {
        var aiPlatformHelper = new Helpers.AiPlatformHelper();
        if (!aiPlatformHelper.AiEnableCheck())
        {
            AiSearchButton.IsEnabled = false;
            AiSearchInputTextBox.IsEnabled = false;
            AiSearchAnswerTextBlock.Text = "AI feature is not enabled. Please enable it and configure the API key in Settings.";
        }
    }
    private async void AiSearchButton_OnClick(object sender, RoutedEventArgs e)
    {
        AiSearchButton.IsEnabled = false;
        AiSearchButton.Content = "Waiting";
        var systemMessage = ReadAiSearchSystemPromptFromFile();
        var searchMessage = AiSearchInputTextBox.Text;
        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        var apiKey = localSettings.Values["DeepSeekApiKey"] as string;

        if (string.IsNullOrEmpty(apiKey))
        {
            AiSearchAnswerTextBlock.Text = "API key is missing. Please configure it in Settings.";
            AiSearchButton.Content = "Send";
            AiSearchButton.IsEnabled = true;
            return;
        }

        var client = new DeepSeekClient(apiKey);
        var request = new ChatRequest
        {
            Messages = [
                Message.NewSystemMessage(systemMessage),
                Message.NewUserMessage(searchMessage)
            ],
            Model = DeepSeekModels.ChatModel
        };

        var chatResponse = await client.ChatAsync(request, CancellationToken.None);
        if (chatResponse is null)
        {
            Debug.WriteLine(client.ErrorMsg);
        }
        AiSearchAnswerTextBlock.Text = chatResponse?.Choices.First().Message?.Content ?? "No response from AI.";
        AiSearchButton.Content = "Send";
        AiSearchButton.IsEnabled = true;
    }

    private string ReadAiSearchSystemPromptFromFile()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Assets", "CryptoAiSearchSystemPrompts.md");
        Debug.WriteLine(filePath);
        if (!File.Exists(filePath))
        {
            return "System prompt file not found.";
        }
        return File.ReadAllText(filePath);
    }
}
