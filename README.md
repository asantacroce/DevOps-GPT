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

### 1. Clone the repository
```bash
 git clone https://github.com/asantacroce/DevOps-GPT.git
```

### 2. Navigate to the project directory
```bash
cd DevOpsGPT
```
### 3. Edit the Azure DevOps 

Open the parameters in local.settings.json, and edit the following:

| Parameter | Description
|----------|----------
DevOps__Organization|This will be the **ORG-NAME** that you have in the DevOps link https://dev.azure.com/<ORG-NAME>
DevOps__Project|This will be the name of your Azure Devops Project
DevOps__PatToken|This is Personal Access Token that you can generate and copy from Azure Devops. Check my blog post for specific indications on where to find this 

```bash
{
    "IsEncrypted": false,
    "Values": {
      "AzureWebJobsStorage": "UseDevelopmentStorage=true",
      "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",

      "OpenApi__DocTitle": "DevOps GPT Documentation",
      "OpenApi__DocDescription": "APIs to support Custom GPT Actions",

      "DevOps__Organization": "<Organization Name>",
      "DevOps__Project": "<Project>",
      "DevOps__PatToken": "<Personal Access Token>"
    }
}
```
### 4. Run the Function App in Emulator

```bash
func start
```
By running the above command you should see the following which lists the local API endpoints exposed

![image](https://github.com/asantacroce/DevOps-GPT/assets/45071168/ad812067-36b4-4617-bc09-617971751742)

## Deploy to Azure

### Log in to Azure:
```bash
az login
```

### Create a resource group:
```bash
az group create --name <resGroupName> --location <locationName>
```

Parameters:

- name: name assigned to the resource group you are creating
- location: specifies the Azure region where the resource group will be created. For a list of possibile locations use the command *az account list-locations --output table*

### Provision the Infrastructure

```bash
az deployment group create --resource-group <resGroupName> --template-file main.bicep --parameters devOpsOrganization=<orgName> devOpsProject=<projectName>
```

Parameters:

- resource-group: name of the resource group that will host the provisioned resources
- template-file: path to the BICEP file that describes the infrastructure to be provisioned
- parameters: set of parameters injected at command line in the form parameter-name=parameter-value separated by space

### Deploy the function using Azure CLI:
```bash
func azure functionapp publish <functionAppName>
```
