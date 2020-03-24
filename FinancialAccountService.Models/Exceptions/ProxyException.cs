using System;
using System.Diagnostics;
using System.Net;

namespace FinancialAccountService.Models.Exceptions
{
    public class ProxyException : Exception
    {

        public HttpStatusCode StatusCode { get; set; }
        public string Content { get; set; }
        public Guid? ExceptionOccurrenceId { get; set; }

        public ProxyException(HttpStatusCode statusCode, string content)
            : base($"Remote API call failed with status code: {(int)statusCode} - {Enum.GetName(typeof(HttpStatusCode), statusCode)}, content: {content}")
        {
            this.StatusCode = statusCode;
            this.Content = content;

            Debug.Assert(Guid.TryParse(null, out var test) == false);   // ensure no exception if content is null

            // If contains a GUID, indicates that this is an id that can be used to correlate logs
            if (Guid.TryParse(content, out var contentGuid))
                ExceptionOccurrenceId = contentGuid;
        }
    }
}
