using DevOpsGPT.Services;
using Microsoft.Extensions.Configuration;

namespace DevOpsGPT.Test
{
    public class TestFactory
    {
        public static DevOpsConnectorService GetDevOpsConnector(IConfiguration config)
        {
            var org = config["DevOpsOrganization"];
            var token = config["DevOpsPatToken"];
            var project = config["DevOpsProject"];

            return new DevOpsConnectorService(
                token,
                project,
                org);
        }
    }
}
