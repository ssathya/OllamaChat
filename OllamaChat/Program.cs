using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.SemanticKernel;
using OllamaChat.Components;

#pragma warning disable SKEXP0010

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddBlazorise(o =>
{
    o.Immediate = true;
})
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons()
    ;
//Connect to Ollama server using semantic kernel
string? endpoint = builder.Configuration.GetValue<string>("Ollama:Endpoint");
string? modelId = builder.Configuration.GetValue<string>("Ollama:ModelId");
if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(modelId))
{
    throw new ArgumentNullException(nameof(endpoint));
}
builder.Services.AddKernel();
builder.Services.AddOpenAIChatCompletion(modelId, apiKey: null, endpoint: new Uri(endpoint));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();