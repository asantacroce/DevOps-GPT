namespace DevOpsGPT.Application
{
    public class Const
    {
        public const string SETTING_DEVOPS_TOKEN = "DevOpsPatToken";
        public const string SETTING_DEVOPS_ORG = "DevOpsOrganization";
        public const string SETTING_DEVOPS_PROJECT = "DevOpsProject";

        public class Functions
        { 
            public const string APPLICATION_JSON = "application/json";

            public const string AREA_WORKITEMS = "WorkItems";
            public const string ROUTE_TASKS = "task";

            public const string METHOD_GET = "get";
            public const string METHOD_POST = "post";

            public const string CREATE_WORK_ITEM = "CreateWorkItem";
            public const string GET_WORK_ITEM = "GetWorkItem";
            public const string RESOLVE_WORK_ITEM = "ResolveWorkItem";

            public const string OPENAPI_FUNC_KEY = "function_key";
            public const string OPENAPI_CODE = "code";
        }
    }
}