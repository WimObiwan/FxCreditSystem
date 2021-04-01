using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FxCreditSystem.API
{
    internal interface IHealthCheckWriter
    {
        Task WriteResponse(HttpContext httpContext, HealthReport report);
    }

    internal class ZabbixFriendlyHealthCheckWriter : IHealthCheckWriter
    {
        private class Response
        {
            [JsonPropertyName("status")]
            public int Status { get; set; }

            [JsonPropertyName("statusText")]
            public string StatusText { get; set; }
             
            [JsonPropertyName("results")]
            public IList<ResponseResultEntry> Results { get; set; }
        }

        private class ResponseResultEntry
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("status")]
            public int Status { get; set; }

            [JsonPropertyName("statusText")]
            public string StatusText { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("data")]
            public Dictionary<string, string> Data { get; set; }
        }

        public async Task WriteResponse(HttpContext httpContext, HealthReport report)
        {
            httpContext.Response.ContentType = "application/json";

            var response = new Response()
            {
                Status = (int)report.Status,
                StatusText = report.Status.ToString(),
                Results = report.Entries.Select(e => 
                    new ResponseResultEntry()
                    {
                        Name = e.Key,
                        Status = (int)e.Value.Status,
                        StatusText = e.Value.Status.ToString(),
                        Description = e.Value.Description,
                        Data = e.Value.Data.ToDictionary(d => d.Key, d => d.Value.ToString())
                    }
                ).ToList()
            };

            string json = JsonSerializer.Serialize<Response>(response);
            await httpContext.Response.WriteAsync(json);
        }
    }    
}