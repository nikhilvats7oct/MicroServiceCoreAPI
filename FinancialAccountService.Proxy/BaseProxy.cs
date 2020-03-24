using FinancialAccountService.Models.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FinancialAccountService.Proxy
{
    public abstract class BaseProxy : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerSettings _jsonSetting;


        protected BaseProxy(ILogger<BaseProxy> logger,
                            HttpClient httpClient)
        {
             Logger = logger;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _jsonSetting = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }
        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        protected ILogger<BaseProxy> Logger { get; }

        protected async Task<T> GetResultAsync<T>(string innerUrl)
        {
            if (string.IsNullOrEmpty(innerUrl))
            {
                throw new ArgumentNullException(nameof(innerUrl), "No URI provided");
            }

            try
            {
                var sw = Stopwatch.StartNew();
                using (var response = await _httpClient.GetAsync(innerUrl))
                {
                    sw.Stop();

                    Logger.LogInformation($"Data from URI {GetFullUrl(innerUrl)} took {sw.Elapsed.TotalMilliseconds}ms.");

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new ProxyException(
                            response.StatusCode, await response.Content.ReadAsStringAsync());
                    }

                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        return default(T);
                    }

                    using (var content = response.Content)
                    {
                        var contentType = content.Headers.ContentType.MediaType;

                        switch (contentType.ToLower())
                        {
                            case "application/json":
                                var contentString = await content.ReadAsStringAsync();
                                return JsonConvert.DeserializeObject<T>(contentString);
                            default:
                                throw new InvalidOperationException($"Content type {contentType} is not supported!");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"An error has occurred while fetching data from {GetFullUrl(innerUrl)}.");
                throw;
            }
        }

        protected async Task<HttpResponseMessage> PostAsync<TPostObject>(string innerUrl, TPostObject postObject)
        {
            if (string.IsNullOrEmpty(innerUrl))
            {
                throw new ArgumentNullException(nameof(innerUrl), "No URI provided");
            }

            try
            {
                var jsonContent = postObject == null ? "{}" : JsonConvert.SerializeObject(postObject, _jsonSetting);
#if DEBUG
                Logger.LogDebug($"POST Body: {jsonContent}");
#endif
                var body = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var sw = Stopwatch.StartNew();
                using (var response = await _httpClient.PostAsync(innerUrl, body))
                {
                    sw.Stop();

                    Logger.LogInformation($"Data from URI {GetFullUrl(innerUrl)} took {sw.Elapsed.TotalMilliseconds}ms.");

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new ProxyException(response.StatusCode, await response.Content.ReadAsStringAsync());
                    }

                    using (var content = response.Content)
                    {
                        var contentType = content.Headers.ContentType.MediaType;

                        switch (contentType.ToLower())
                        {
                            case "application/json":
                                return JsonConvert.DeserializeObject<HttpResponseMessage>(await content.ReadAsStringAsync());
                            default:
                                throw new InvalidOperationException($"Content type {contentType} is not supported!");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"An error has occurred while Posting data from {GetFullUrl(innerUrl)}.");
                throw;
            }
        }

        protected async Task PostwithNoReturnTypeAsync<TPostObject>(string innerUrl, TPostObject postObject, string replayId)
        {
            if (string.IsNullOrEmpty(innerUrl))
            {
                throw new ArgumentNullException(nameof(innerUrl), "No URI provided");
            }

            try
            {
                var jsonContent = postObject == null ? "{}" : JsonConvert.SerializeObject(postObject, _jsonSetting);
#if DEBUG
                Logger.LogDebug($"POST Body: {jsonContent}");
#endif
                var body = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                _httpClient.DefaultRequestHeaders.Add("idempotency-key", replayId);

                var sw = Stopwatch.StartNew();
                using (var response = await _httpClient.PostAsync(innerUrl, body))
                {
                    sw.Stop();

                    Logger.LogInformation($"Data from URI {GetFullUrl(innerUrl)} took {sw.Elapsed.TotalMilliseconds}ms.");

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new ProxyException(response.StatusCode, await response.Content.ReadAsStringAsync());
                    }
                }
            }

            catch (Exception e)
            {
                Logger.LogError(e, $"An error has occurred while POSTing data from {GetFullUrl(innerUrl)}.");
                throw;
            }
        }

        protected async Task PostwithNoReturnTypeAsync<TPostObject>(string innerUrl, TPostObject postObject)
        {
            if (string.IsNullOrEmpty(innerUrl))
            {
                throw new ArgumentNullException(nameof(innerUrl), "No URI provided");
            }

            try
            {
                var jsonContent = postObject == null ? "{}" : JsonConvert.SerializeObject(postObject, _jsonSetting);
#if DEBUG
                Logger.LogDebug($"POST Body: {jsonContent}");
#endif
                var body = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var sw = Stopwatch.StartNew();
                using (var response = await _httpClient.PostAsync(innerUrl, body))
                {
                    sw.Stop();

                    Logger.LogInformation($"Data from URI {GetFullUrl(innerUrl)} took {sw.Elapsed.TotalMilliseconds}ms.");

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new ProxyException(response.StatusCode, await response.Content.ReadAsStringAsync());
                    }
                }
            }

            catch (Exception e)
            {
                Logger.LogError(e, $"An error has occurred while Posting data from {GetFullUrl(innerUrl)}.");
                throw;
            }
        }

        protected async Task PutwithNoReturnTypeAsync<TPostObject>(string innerUrl, TPostObject postObject)
        {
            if (string.IsNullOrEmpty(innerUrl))
            {
                throw new ArgumentNullException(nameof(innerUrl), "No URI provided");
            }

            try
            {
                var jsonContent = postObject == null ? "{}" : JsonConvert.SerializeObject(postObject, _jsonSetting);
#if DEBUG
                Logger.LogDebug($"POST Body: {jsonContent}");
#endif
                var body = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var sw = Stopwatch.StartNew();
                using (var response = await _httpClient.PutAsync(innerUrl, body))
                {
                    sw.Stop();

                    Logger.LogInformation($"Data from URI {GetFullUrl(innerUrl)} took {sw.Elapsed.TotalMilliseconds}ms.");

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new ProxyException(response.StatusCode, await response.Content.ReadAsStringAsync());
                    }
                }
            }

            catch (Exception e)
            {
                Logger.LogError(e, $"An error has occurred while Posting data from {GetFullUrl(innerUrl)}.");
                throw;
            }
        }

        protected async Task<HttpResponseMessage> PutResultAsync<TPostObject>(string innerUrl, TPostObject putObject)
        {
            if (string.IsNullOrEmpty(innerUrl))
            {
                throw new ArgumentNullException(nameof(innerUrl), "No URI provided");
            }

            try
            {
                var jsonContent = putObject == null ? "{}" : JsonConvert.SerializeObject(putObject, _jsonSetting);
#if DEBUG
                Logger.LogDebug($"POST Body: {jsonContent}");
#endif
                var body = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var sw = Stopwatch.StartNew();
                using (var response = await _httpClient.PutAsync(innerUrl, body))
                {
                    sw.Stop();

                    Logger.LogInformation($"Data from URI {GetFullUrl(innerUrl)} took {sw.Elapsed.TotalMilliseconds}ms.");

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new ProxyException(response.StatusCode, await response.Content.ReadAsStringAsync());
                    }

                    using (var content = response.Content)
                    {
                        var contentType = content.Headers.ContentType.MediaType;

                        switch (contentType.ToLower())
                        {
                            case "application/json":
                                return JsonConvert.DeserializeObject<HttpResponseMessage>(await content.ReadAsStringAsync());
                            default:
                                throw new InvalidOperationException($"Content type {contentType} is not supported!");
                        }
                    }
                }
            }

            catch (Exception e)
            {
                Logger.LogError(e, $"An error has occurred while putting data from {GetFullUrl(innerUrl)}.");
                throw;
            }
        }

        protected async Task PutWithNoReturnTypeAsync<TPostObject>(string innerUrl, TPostObject postObject, string replayId)
        {
            if (string.IsNullOrEmpty(innerUrl))
            {
                throw new ArgumentNullException(nameof(innerUrl), "No URI provided");
            }

            try
            {
                var jsonContent = postObject == null ? "{}" : JsonConvert.SerializeObject(postObject, _jsonSetting);
#if DEBUG
                Logger.LogDebug($"POST Body: {jsonContent}");
#endif
                var body = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                _httpClient.DefaultRequestHeaders.Add("idempotency-key", replayId);
                var sw = Stopwatch.StartNew();
                using (var response = await _httpClient.PutAsync(innerUrl, body))
                {
                    sw.Stop();

                    Logger.LogInformation($"Data from URI {GetFullUrl(innerUrl)} took {sw.Elapsed.TotalMilliseconds}ms.");

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new ProxyException(response.StatusCode, await response.Content.ReadAsStringAsync());
                    }
                }
            }

            catch (Exception e)
            {
                Logger.LogError(e, $"An error has occurred while putting data from {GetFullUrl(innerUrl)}.");
                throw;
            }

        }

        protected async Task DeleteWithNoReturnTypeAsync<TPostObject>(string innerUrl, string replayId)
        {
            if (string.IsNullOrEmpty(innerUrl))
            {
                throw new ArgumentNullException(nameof(innerUrl), "No URI provided");
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Add("idempotency-key", replayId);
                var sw = Stopwatch.StartNew();
                using (var response = await _httpClient.DeleteAsync(innerUrl))
                {
                    sw.Stop();

                    Logger.LogInformation($"Data deleted from URI {GetFullUrl(innerUrl)} took {sw.Elapsed.TotalMilliseconds}ms.");

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new ProxyException(response.StatusCode, await response.Content.ReadAsStringAsync());
                    }
                }
            }

            catch (Exception e)
            {
                Logger.LogError(e, $"An error has occurred while deleting data from {GetFullUrl(innerUrl)}.");
                throw;
            }
        }

        public string GetFullUrl(string innerUrl)
        {
            return _httpClient.BaseAddress + innerUrl;
        }


    }
}
