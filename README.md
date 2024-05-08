# Custom GPT powered by Azure Functions

## Description
This project integrates Custom GPT with Azure DevOps using Azure Functions. It allows the GPT to interact with Azure DevOps Project Boards, handling tasks such as querying, creating, and updating work items.

## Prerequisites
- An Azure Subscription
- Azure DevOps Account
- AZ CLI installed
- Visual Studio Community or another C# IDE
- Azure Function Tools

## Getting Started
These instructions will guide you on setting up and running the project locally for development and testing purposes.

### Setting up the environment
**Clone the repository:**
 ```bash
 git clone https://github.com/yourusername/yourprojectname.git
 ```

### Log in to Azure:
```bash
az login
```

### Create a resource group:
```bash
az group create --name liveGPT --location westeurope
```

### Deploy the function using Azure CLI:
```bash
func azure functionapp publish liveGPT-func
```
