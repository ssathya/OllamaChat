using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;

namespace OllamaChat.Components.Pages;

public partial class Chat
{
    [Inject]
    public Kernel? Kernel { get; set; }

    [Inject]
    public IChatCompletionService? chat { get; set; }

    protected string UserInput = string.Empty;
    protected string SystemMsg = string.Empty;
    private string savedSystemMsg = string.Empty;
    protected string processingMsg = string.Empty;
    protected StringBuilder responseMsg = new();
    protected string responseToDisplay = string.Empty;
    private ChatHistory chatHistory = [];

    protected override void OnInitialized()
    {
        chatHistory = new ChatHistory("A brief explanation in not more than 100 words");
    }

    protected async void OnSubmitClick()
    {
        if (Kernel is not null && chat is not null)
        {
            chatHistory.AddUserMessage(UserInput);
            responseMsg.AppendLine($">*{UserInput}*\n\n");
            processingMsg = UserInput.Length > 40 ? $"Processing {UserInput[..40]} ..." :
                $"Processing {UserInput} ...";
            await foreach (var response in chat.GetStreamingChatMessageContentsAsync(chatHistory))
            {
                var msg = response.ToString();
                responseMsg.Append(msg);
                responseToDisplay = Markdown.ToHtml(responseMsg.ToString());
                StateHasChanged();
                await Task.Delay(30);
            }
            responseMsg.Append("\n\n");
            responseToDisplay = Markdown.ToHtml(responseMsg.ToString());
            processingMsg = string.Empty;
            UserInput = string.Empty;
            SystemMsg = savedSystemMsg;
            StateHasChanged();
        }
    }

    protected void SystemSelectionChanged(ChangeEventArgs e)
    {
        var value = e.Value as string;
        if (!string.IsNullOrEmpty(value))
        {
            if (SystemMsg.Equals(savedSystemMsg) && !string.IsNullOrEmpty(SystemMsg))
            {
                return;
            }
            SystemMsg = value;
            chatHistory = new ChatHistory(value);
            chatHistory.Clear();
            if (UserInput != string.Empty)
            {
                savedSystemMsg = value;
            }
        }
    }
}