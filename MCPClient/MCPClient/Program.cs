using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using System.Net.Http;

// --- 1. SETUP MCP CONNECTION ---
var serverUrl = new Uri("https://localhost:7139/sse");

var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
};
var myHttpClient = new HttpClient(handler);

var options = new HttpClientTransportOptions { Endpoint = serverUrl };
var transport = new HttpClientTransport(options, myHttpClient);

Console.WriteLine("Connecting to MCP Server...");
var mcpClient = await McpClient.CreateAsync(transport);
Console.WriteLine("Connected to MCP Server!");


// --- 2. SETUP SEMANTIC KERNEL (GROQ) ---
var builder = Kernel.CreateBuilder();

// Groq uses the OpenAI SDK format. 
// We use "llama3-70b-8192" or "mixtral-8x7b-32768" as they support tool calling well.
builder.AddOpenAIChatCompletion(
    modelId: "openai/gpt-oss-120b",
    apiKey: "OPENAI-API-KEY",
    endpoint: new Uri("https://api.groq.com/openai/v1") // <--- Point to Groq
);

var kernel = builder.Build();


// --- 3. BRIDGE MCP TOOLS TO THE LLM ---
var mcpTools = await mcpClient.ListToolsAsync();

// Convert all MCP tools into Kernel Functions first
var functions = mcpTools.Select(tool => tool.AsKernelFunction()).ToList();

// Add them all at once under ONE plugin name
if (functions.Any())
{
    kernel.Plugins.AddFromFunctions("WeatherServer", functions);

    foreach (var tool in mcpTools)
    {
        Console.WriteLine($"[System] Groq now knows: {tool.Name}");
    }
}


// --- 4. START THE CHAT LOOP ---
var settings = new OpenAIPromptExecutionSettings
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

Console.WriteLine("\n--- Groq + MCP Assistant Ready ---");
Console.WriteLine("Ask: 'What is the weather like in New York?'");

while (true)
{
    Console.Write("\nYou: ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) break;

    try
    {
        var response = await kernel.InvokePromptAsync(input, new(settings));
        Console.WriteLine($"\nAI (Groq): {response}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n[Error]: {ex.Message}");
        // Pro-tip: If Groq complains about "Tool calls not supported", 
        // make sure you are using llama3-70b or llama3-8b.
    }
}