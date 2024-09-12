using Markdig;
using Microsoft.AspNetCore.Components;

namespace OllamaChat.Components.Pages;

public partial class Home
{
    private string pageContent = "<b>File Read Error!</b>";
    private string contentFileName = "HomePageContent.txt";

    [Inject]
    public IWebHostEnvironment? _environment { get; set; }

    protected override Task OnInitializedAsync()
    {
        string fullPath = contentFileName;
        if (!File.Exists(contentFileName))
        {
            if (_environment != null)
            {
                fullPath = Path.Combine(_environment.WebRootPath, contentFileName);
            }
        }
        if (File.Exists(fullPath))
        {
            var fileContent = File.ReadAllText(fullPath);
            pageContent = Markdown.ToHtml(fileContent);
        }
        return base.OnInitializedAsync();
    }
}