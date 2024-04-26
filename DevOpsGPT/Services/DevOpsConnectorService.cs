using DevOpsGPT.Services.DTO;
using DevOpsGPT.Services.DTO.AzureDevOpsIterations;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.XPath;
using WorkItem = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem;

namespace DevOpsGPT.Services;
public class DevOpsConnectorService(DevOpsConfig devOpsConf)
{
    private const string DEVOPS_BASE_URI = "https://dev.azure.com";
    private readonly DevOpsConfig _devOpsConfig = devOpsConf;

    #region PUBLIC METHODS

    public async Task<SprintIterationsDTO> GetSprints()
    {
        using (HttpClient client = new HttpClient())
        {
            string base64Auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _devOpsConfig.PatToken)));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Auth);

            var resp = await client.GetStringAsync(
                $"https://dev.azure.com/{_devOpsConfig.Organization}/{_devOpsConfig.Project}/_apis/work/teamsettings/iterations?api-version=6.0");

            return JsonConvert.DeserializeObject<SprintIterationsDTO>(resp);
        }
    }

    public async Task<IList<WorkItemDTO>> QueryTasks(string state = null)
    {
        List<WorkItemDTO> workItems = new List<WorkItemDTO>();

        // create a wiql object and build our query
        var wiql = new Wiql()
        {
            Query = await BuildQuery(state)
        };

        // create instance of work item tracking http client
        using (var httpClient = GetDevOpsClient())
        {
            // execute the query to get the list of work items in the results
            var result = await httpClient.QueryByWiqlAsync(wiql).ConfigureAwait(false);
            var ids = result.WorkItems.Select(item => item.Id).ToArray();

            // some error handling
            if (ids.Length == 0)
            {
                return Array.Empty<WorkItemDTO>();
            }

            // build a list of the fields we want to see
            var fields = new[] {
                    "System.WorkItemType",
                    "System.Id",
                    "System.Title",
                    "System.State",
                    "System.IterationPath",
                    "System.CreatedDate",
                    "System.Description"
                };

            // get work items for the ids found in query
            var items = await httpClient.GetWorkItemsAsync(ids, fields, result.AsOf).ConfigureAwait(false);

            foreach (var i in items)
            {
                var item = new WorkItemDTO(
                    i.Id.ToString(),
                    i.Fields["System.Title"].ToString(),
                    i.Fields["System.State"].ToString(),
                    $"{DEVOPS_BASE_URI}/{_devOpsConfig.Organization}/{_devOpsConfig.Project}/_workitems/edit/{i.Id}",
                    i.Fields["System.CreatedDate"].ToString(),
                    i.Fields["System.WorkItemType"].ToString(),
                    i.Fields["System.State"].ToString() == "Doing" ? $"https://devops-gpt.azurewebsites.net/api/task/{i.Id}/resolve" : null);

                workItems.Add(
                    item
                    );
            }

            return workItems;
        }
    }

    public async Task<WorkItem> CreateTask(CreateTaskDTO taskInfo)
    {
        var workItemType = "Issue"; // Specify the work item type (e.g., Task, Bug, Feature)

        // Define the fields and their values for the new work item
        var workItemFields = new Dictionary<string, object>
        {
            ["System.Title"] = taskInfo.Title,
            ["System.Description"] = taskInfo.Description,
        };

        if (taskInfo.InSprint)
        {
            workItemFields.Add("System.IterationPath", await GetLatestSprint());
        }

        // create instance of work item tracking http client
        using (var httpClient = GetDevOpsClient())
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();
            foreach (var kv in workItemFields)
            {
                patchDocument.Add(
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/" + kv.Key,
                        Value = kv.Value
                    });
            };

            // Create the work item
            return await httpClient.CreateWorkItemAsync(
                document: patchDocument,
                project: _devOpsConfig.Project,
                type: workItemType
            );
        }
    }

    public async Task ResolveTask(int taskId, string reason)
    {
        var workItemType = "Issue"; // Specify the work item type (e.g., Task, Bug, Feature)

        // create instance of work item tracking http client
        using (var httpClient = GetDevOpsClient())
        {
            CommentCreate comment = new CommentCreate
            {
                Text = reason
            };

            await httpClient.AddCommentAsync(comment, _devOpsConfig.Project, taskId);

            var item = await httpClient.GetWorkItemAsync(taskId);

            JsonPatchDocument patchDocument = new JsonPatchDocument
            {
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.State",
                    Value = "Done"
                }
                // Add more operations here if needed
            };

            // Update the work item
            WorkItem result = await httpClient.UpdateWorkItemAsync(patchDocument, taskId);

        }
    }
    #endregion

    #region PRIVATE METHODS
    private WorkItemTrackingHttpClient GetDevOpsClient()
    {
        var credentials = new VssBasicCredential(string.Empty, _devOpsConfig.PatToken);

        var uri = new Uri($"{DEVOPS_BASE_URI}/{_devOpsConfig.Organization}");

        return new WorkItemTrackingHttpClient(uri, credentials);
    }

    private async Task<string> GetLatestSprint()
    {
        var sprints = await GetSprints();

        return sprints.Value.First(x => x.Attributes.TimeFrame == "current").Path;

    }

    private async Task<string> BuildQuery(string state)
    {
        var currentSprint = await GetLatestSprint();

        StringBuilder builder = new StringBuilder();

        builder.Append($@"
SELECT [Id]
FROM WorkItems
WHERE [System.TeamProject] = '{_devOpsConfig.Project}'
AND[System.IterationPath] = '{currentSprint}'
AND[System.WorkItemType] = 'Issue'
");

        if (!string.IsNullOrEmpty(state.Trim()))
        {
            builder.Append($"AND [System.State] = '{state}'");
        }

        builder.Append("ORDER BY [State] ASC, [Changed Date] DESC");

        return builder.ToString();
    }

    

    #endregion
}

