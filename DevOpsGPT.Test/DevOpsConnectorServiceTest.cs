using DevOpsGPT.Services;
using Microsoft.Extensions.Configuration;

namespace DevOpsGPT.Test
{
    public class DevOpsConnectorServiceTest
    {
        private readonly DevOpsConnectorService _service;

        private readonly IConfiguration _configuration;

        public DevOpsConnectorServiceTest()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            // Build the configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

            _configuration = builder.Build();

            _service = TestFactory.GetDevOpsConnector(_configuration);
        }

        [Fact]
        public async Task GetAllSprints()
        {
            var sprints = await _service.GetSprints();

            Assert.True(sprints.Count > 0);
        }

        [Fact]
        public async Task GetAllTasksInSprint()
        {
            var tasks = await _service.QueryTasks();
        }

        [Fact]
        public async Task CreateTaskInSprint()
        {
            var task = await _service.CreateTask(new Services.DTO.CreateTaskDTO(true, "My Task", "My Desc"));
        }

        [Fact]
        public async Task CreateTaskOnBacklog()
        {
            var task = await _service.CreateTask(new Services.DTO.CreateTaskDTO(false, "My Task", "My Desc"));
        }

        [Fact]
        public async Task ResolveTask()
        {
            var task = await _service.CreateTask(new Services.DTO.CreateTaskDTO(true, "My Task", "My Desc"));

            await _service.ResolveTask(task.Id.Value, "My Comment on the closure of the task");
        }
    }
}