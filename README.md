# DevOps-GPT

## Description
This project exposes Azure DevOps over OpenAPI using C# Azure Functions. It allows a Custom-GPT to interact with Azure DevOps Project Boards, handling tasks such as querying, creating, and updating work items. For extensive details on the whole project, please read my full blog post at this [link](https://blog.andresantacroce.com/custom-gpt-powered-by-azure-functions).

## Prerequisites
* For Local Run and Development
  * Azure DevOps Account - [here](https://azure.microsoft.com/en-gb/products/devops/)
  * Visual Studio Community or another C# IDE - [here](https://visualstudio.microsoft.com/vs/community)
  * Azure Function Tools - [here](https://github.com/Azure/azure-functions-core-tools)
* Tu support Azure Deploy
  * An Azure Subscription - [here](https://azure.microsoft.com/en-us/free/)
  * AZ CLI - [here](https://learn.microsoft.com/en-us/cli/azure/)

## Local Run
These instructions will guide you on setting up and running the project locally for development and testing purposes.

### Clone the repository
```bash
 git clone https://github.com/yourusername/yourprojectname.git
 ```
## Deploy to Azure

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
