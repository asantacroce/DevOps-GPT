param location string
param functionAppName string
param storageAccountName string
param resourceGroupName string

// Storage account
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

// Function App
resource functionApp 'Microsoft.Web/sites@2021-02-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  properties: {
    siteConfig: {
      appSettings: [
        // Transform object to array of key-value pairs
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value};EndpointSuffix=core.windows.net'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'OpenApi__DocTitle'
          value: 'DevOps GPT Documentation'
        }
        {
          name: 'OpenApi__DocDescription'
          value: 'APIs to support Custom GPT Actions'
        }
		{
          name: 'DevOpsPatToken'
          value: 'SECURE'
        }
		{
          name: 'DevOpsOrganization'
          value: 'andresantacroce-dev'
        }
		{
          name: 'DevOpsProject'
          value: 'Yooth'
        }
      ]
    }
  }
}
