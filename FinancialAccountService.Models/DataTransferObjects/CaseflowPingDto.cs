using Newtonsoft.Json;

namespace FinancialAccountService.Models.DataTransferObjects
{
    public class CaseflowPingDto
    {
        [JsonProperty("response")]
        public Response Response { get; set; }
    }

    public class Response
    {
        [JsonProperty("_retVal")]
        public string ReturnValue { get; set; }
    }
}