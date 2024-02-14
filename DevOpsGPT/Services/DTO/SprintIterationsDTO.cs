namespace DevOpsGPT.Services.DTO
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    namespace AzureDevOpsIterations
    {
        public class Iteration
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("path")]
            public string Path { get; set; }

            [JsonProperty("attributes")]
            public IterationAttributes Attributes { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }
        }

        public class IterationAttributes
        {
            [JsonProperty("startDate")]
            public DateTime StartDate { get; set; }

            [JsonProperty("finishDate")]
            public DateTime FinishDate { get; set; }

            [JsonProperty("timeFrame")]
            public string TimeFrame { get; set; }
        }

        public class SprintIterationsDTO
        {
            [JsonProperty("count")]
            public int Count { get; set; }

            [JsonProperty("value")]
            public List<Iteration> Value { get; set; }
        }
    }

}
