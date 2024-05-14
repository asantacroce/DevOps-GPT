using DevOpsGPT.Application;
using DevOpsGPT.Services;
using DevOpsGPT.Services.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;

namespace DevOpsGPT.Functions
{
    public class GetOpenWorkItemsFunction(
        ILogger<CreateWorkItemsFunction> logger,
        DevOpsConnectorService service)
    {
        private readonly ILogger<CreateWorkItemsFunction> _logger = logger;
        private readonly DevOpsConnectorService _service = service;

        [Function(Const.Functions.GET_WORK_ITEM)]
        [OpenApiOperation(operationId: Const.Functions.GET_WORK_ITEM, tags: new[] { Const.Functions.AREA_WORKITEMS }, Description = "Returns the list of Open Work Items in Sprint")]
        [OpenApiSecurity(Const.Functions.OPENAPI_FUNC_KEY, SecuritySchemeType.ApiKey, Name = Const.Functions.OPENAPI_CODE, In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(HttpStatusCode.Created, Const.Functions.APPLICATION_JSON, typeof(IList<WorkItemDTO>))]
        [OpenApiParameter(name: "state", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The state of the Work Item. Possibile values: Doing, To Do, Done")]
        public async Task<IActionResult> Run(
            [HttpTrigger(
            AuthorizationLevel.Function,
            Const.Functions.METHOD_GET,
            Route = Const.Functions.ROUTE_TASKS)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var state = req.Query["state"];

            var bugs = await _service.QueryTasks(state);
            return new OkObjectResult(bugs);
        }
    }
}