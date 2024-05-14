using DevOpsGPT.Application;
using DevOpsGPT.Services;
using DevOpsGPT.Services.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Net;

namespace DevOpsGPT.Functions
{
    public class ResolveWorkItemFunction(
        ILogger<ResolveWorkItemFunction> logger,
        DevOpsConnectorService service)
    { 
        private readonly ILogger<ResolveWorkItemFunction> _logger = logger;
        private readonly DevOpsConnectorService _service = service;

        [Function(Const.Functions.RESOLVE_WORK_ITEM)]
        [OpenApiOperation(operationId: Const.Functions.RESOLVE_WORK_ITEM, tags: new[] { Const.Functions.AREA_WORKITEMS }, Description = "Resolves an In Progress work item by providing a Resolution. The Resolution is mandatory, without we should block the operation.")]
        [OpenApiSecurity(Const.Functions.OPENAPI_FUNC_KEY, SecuritySchemeType.ApiKey, Name = Const.Functions.OPENAPI_CODE, In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(HttpStatusCode.Created, Const.Functions.APPLICATION_JSON, typeof(IList<WorkItemDTO>))]
        [OpenApiParameter("id", In = ParameterLocation.Path, Required = true )]
        [OpenApiRequestBody(contentType: Const.Functions.APPLICATION_JSON, bodyType: typeof(ResolveTaskDTO), Required = true, Description = "Providers the resolution comment.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(Microsoft.Azure.Functions.Worker.AuthorizationLevel.Function,
            Const.Functions.METHOD_POST,
            Route = "task/{id}/resolve")] HttpRequest req, int id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await req.ReadAsStringAsync();

            ResolveTaskDTO resolveTaskDTO = JsonConvert.DeserializeObject<ResolveTaskDTO>(requestBody);

            await _service.ResolveTask(id, resolveTaskDTO.Reason);
            return new OkResult();
        }
    }
}