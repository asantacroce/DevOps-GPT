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
    public class CreateWorkItemsFunction(
        ILogger<CreateWorkItemsFunction> logger,
        DevOpsConnectorService service)
    { 
        private readonly ILogger<CreateWorkItemsFunction> _logger = logger;
        private readonly DevOpsConnectorService _service = service;

        [Function(Const.Functions.CREATE_WORK_ITEM)]
        [OpenApiOperation(operationId: Const.Functions.CREATE_WORK_ITEM, tags: new[] { Const.Functions.AREA_WORKITEMS }, Description = "Creates a new Work Item in the Project")]
        [OpenApiSecurity(Const.Functions.OPENAPI_FUNC_KEY, SecuritySchemeType.ApiKey, Name = Const.Functions.OPENAPI_CODE, In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(HttpStatusCode.Created, Const.Functions.APPLICATION_JSON, typeof(IList<WorkItemDTO>))]
        [OpenApiRequestBody(contentType: Const.Functions.APPLICATION_JSON, bodyType: typeof(CreateTaskDTO), Required = true, Description = "The Work Item to be created")]
        public async Task<IActionResult> Run(
            [HttpTrigger(Microsoft.Azure.Functions.Worker.AuthorizationLevel.Function,
            Const.Functions.METHOD_POST,
            Route = Const.Functions.ROUTE_TASKS)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await req.ReadAsStringAsync();

            CreateTaskDTO createTaskDTO = JsonConvert.DeserializeObject<CreateTaskDTO>(requestBody);

            var workItem = await _service.CreateTask(createTaskDTO);

            _logger.LogInformation($"Work Item created: {JsonConvert.SerializeObject(workItem)}");

            return new OkObjectResult(workItem);
        }
    }
}