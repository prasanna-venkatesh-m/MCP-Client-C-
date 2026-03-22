Groq + Semantic Kernel + MCP Client Integration

Overview
This project demonstrates how to connect a Model Context Protocol (MCP) server with Microsoft Semantic Kernel and use Groq-hosted LLMs (via OpenAI-compatible API) to enable tool calling.

The assistant dynamically discovers MCP tools and allows the LLM to invoke them automatically during conversations.

--------------------------------------------------

Features
- Connects to an MCP server over HTTP (SSE endpoint)
- Dynamically loads MCP tools
- Registers tools as Semantic Kernel functions
- Uses Groq-hosted models via OpenAI-compatible API
- Enables automatic tool invocation
- Interactive chat loop

--------------------------------------------------

Prerequisites
- .NET 10
- MCP-compatible server running locally
- Groq API key

--------------------------------------------------

Setup

1. Clone the repository
git clone <your-repo-url>
cd <your-project-folder>

2. Install dependencies
dotnet add package Microsoft.SemanticKernel
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI

3. Configure MCP Server
Ensure your MCP server is running at:
https://localhost:7139/sse

If using self-signed certificates, this project bypasses SSL validation for development.

4. Set API Key
Replace:
OPENAI-API-KEY

with your actual Groq API key.

5. Choose Model
Recommended models for tool calling:
- llama3-70b-8192
- llama3-8b-8192
- mixtral-8x7b-32768

Example:
modelId: openai/gpt-oss-120b

--------------------------------------------------

How It Works

Step 1: Connect to MCP Server
- Creates HTTP transport
- Establishes connection using McpClient

Step 2: Initialize Semantic Kernel
- Configures OpenAI-compatible endpoint (Groq)
- Registers chat completion model

Step 3: Load MCP Tools
- Fetches tools via ListToolsAsync()
- Converts them into Kernel Functions
- Registers them under a plugin

Step 4: Chat Loop
- Accepts user input
- Sends prompt to LLM
- Automatically invokes tools when needed

--------------------------------------------------

Running the Application
dotnet run

Example prompt:
What is the weather like in New York?

--------------------------------------------------

Expected Output

Connecting to MCP Server...
Connected to MCP Server!

[System] Groq now knows: getWeather

--- Groq + MCP Assistant Ready ---

You: What is the weather in New York?

AI (Groq): The current weather in New York is...

--------------------------------------------------

Error Handling

Common Issues

Tool calls not supported
- Use supported models like:
  llama3-70b
  llama3-8b

SSL Issues
- Development mode bypasses certificate validation
- Do NOT use in production

MCP Connection Failure
- Ensure server is running
- Verify endpoint URL

--------------------------------------------------

Security Notes
- Do not expose API keys in code
- Use environment variables for production
- Remove SSL bypass in production environments

--------------------------------------------------

Future Improvements
- Add streaming responses
- Add memory/context persistence
- Improve error handling
- Add logging and telemetry

--------------------------------------------------

License
MIT License
