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
    private Dictionary<string, string> systemMessages = [];
    private bool disableSubmit = false;

    protected override void OnInitialized()
    {
        chatHistory = new ChatHistory("A brief explanation in not more than 100 words");
        systemMessages["1"] = "You are going to answer the question in not more than 100 words";
        systemMessages["2"] = "You are expected to write a blog based on the items mentioned";
        systemMessages["3"] = "Change the tone to sound friendly";
        systemMessages["4"] = "Convert the text given below into a informational blog; you don't " +
            "have to be too formal or polite; however it will be appreciated";
        systemMessages["5"] = "Convert the text given below into a informational email; you don't " +
            "have to be too formal or polite; however it will be appreciated";
        systemMessages["6"] = "you are going to answer in one  paragraph; " +
            "please limit the output to one paragraph";
        systemMessages["7"] = "Convert the text given below to a professional and formal  blog.";
        systemMessages["8"] = "Convert the text given below to a professional and formal email.";
        systemMessages["9"] = "please answer in less than one paragraph; " +
            "please sound professional.";
        systemMessages["10"] = "rephrase the message to sound more formal and polite";
        systemMessages["11"] = "Get a detailed report on all things you know about the topic. " +
            "Present it in a friendly and pleasant format";
        systemMessages["12"] = "You need to summarize the message that is not more than 150 words";
    }

    protected async void OnSubmitClick()
    {
        if (Kernel is not null && chat is not null && UserInput != string.Empty)
        {
            disableSubmit = true;
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
            disableSubmit = false;
            StateHasChanged();
        }
    }

    protected void SystemSelectionChanged(ChangeEventArgs e)
    {
        var value = e.Value as string;
        if (!string.IsNullOrEmpty(value))
        {
            if (systemMessages[value].Equals(savedSystemMsg) && !string.IsNullOrEmpty(SystemMsg))
            {
                return;
            }
            SystemMsg = systemMessages[value];
            chatHistory = new ChatHistory(value);
            chatHistory.Clear();
            savedSystemMsg = SystemMsg;
        }
    }
}