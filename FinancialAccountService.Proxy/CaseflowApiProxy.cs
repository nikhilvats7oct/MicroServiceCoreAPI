using FinancialAccountService.Models.DataTransferObjects;
using FinancialAccountService.Models.Exceptions;
using FinancialAccountService.Proxy.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace FinancialAccountService.Proxy
{
    public class CaseflowApiProxy : BaseProxy, ICaseflowApiProxy
    {
        public CaseflowApiProxy(ILogger<BaseProxy> logger, HttpClient httpClient)
            : base(logger, httpClient)
        {
        }

        public async Task<CaseflowPingDto> HitHeartBeat()
        {
            var innerUrl = "v1/ping";
            return await GetResultAsync<CaseflowPingDto>(innerUrl);
        }

        public List<string> GetLinkedAccountsAsync(string reference)
        {
            throw new NotImplementedException();
        }

        public async Task<RecievedAccountDto> CheckDataProtection(RegisterValidationDto dto)
        {
            string uri = $"v1/accounts/{Uri.EscapeDataString(dto.LowellReference)}";

            try
            {
                var accountDto = await GetResultAsync<RecievedAccountDto>(uri);
                return accountDto;

            }
            catch (ProxyException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    //When caseflow is unable to find lowell reference, null is returned.
                    return null;
                }
                throw;
            }

        }

        public async Task<RecievedCustomerSummariesDto> CheckHasWebAccountAsync(WebRegisteredDto dto)
        {
            string uri = $"v1/accounts/{Uri.EscapeDataString(dto.LowellReference)}/customer-summary";

            var summaries = await GetResultAsync<RecievedCustomerSummariesDto>(uri);

            return summaries;
        }

        public async Task SendForgotPasswordAsync(SendForgottenPasswordDto forgotPasswordDto, string userId, string replayId)
        {
            string uri = $"v1/accounts/{Uri.EscapeDataString(userId)}/communications";

            await PostwithNoReturnTypeAsync<SendForgottenPasswordDto>(uri, forgotPasswordDto, replayId);
        }

        public async Task SendRegistrationEmailAsync(SendEmailDto model, string lowellReference, string replayId)
        {
            string uri = $"v1/accounts/{Uri.EscapeDataString(lowellReference)}/communications";

            await PostwithNoReturnTypeAsync<SendEmailDto>(uri, model, replayId);
        }
        public async Task SendConfirmEmailEmailAsync(SendEmailDto model, string lowellReference, string replayId)
        {
            string uri = $"v1/accounts/{lowellReference}/communications";

            await PostwithNoReturnTypeAsync<SendEmailDto>(uri, model, replayId);
        }

        public async Task CompleteDeregistrationAsync(CompleteDeregistrationDto completeDeregistrationDto, string replayId)
        {
            string uri = $"v1/users/{Uri.EscapeDataString(completeDeregistrationDto.UserId)}";

            await DeleteWithNoReturnTypeAsync<CompleteRegistrationDto>(uri, replayId);
        }

        public async Task CompleteRegistrationAsync(CompleteRegistrationDto dto, string replayId)
        {
            string uri = $"v1/users/{Uri.EscapeDataString(dto.UserId)}";

            await PutWithNoReturnTypeAsync<CompleteRegistrationDto>(uri, dto, replayId);
        }
        public async Task<CreatedUserDto> GetUserAsync(string userId)
        {
            return await GetResultAsync<CreatedUserDto>($"v1/users/{Uri.EscapeDataString(userId)}");
        }

        public async Task LogWebActivity(WebActivityDto webActivity)
        {
            await PostwithNoReturnTypeAsync("v1/web-logs", new
            {
                lowellReference = webActivity.LowellReference ?? String.Empty,
                company = webActivity.Company,
                dateTime = webActivity.DateTime,
                webActionID = webActivity.WebActionID,
                guid = webActivity.Guid ?? String.Empty,
                webUser = String.IsNullOrEmpty(webActivity.Guid) ? "WebAnon" : "WebUser"
            });
        }

        public async Task<CaseFlowMyAccountsDto> GetMyAccountsAsync(string userId)
        {
            // Double check that this contains a GUID - should already have been validated on model
            Guid.Parse(userId);

            string innerUrl = $"v1/users/{Uri.EscapeDataString(userId)}/customer-summary";
            return await GetResultAsync<CaseFlowMyAccountsDto>(innerUrl);
        }

        public async Task<CaseFlowMyAccountsDto> GetMyAccountsSummaryAsync(string accountId)
        {
            string innerUrl = $"v1/accounts/{Uri.EscapeDataString(accountId)}/customer-summary";
            return await GetResultAsync<CaseFlowMyAccountsDto>(innerUrl);
        }

        public async Task<CaseFlowMyAccountsDetailDto> GetMyAccountsDetailAsync(string lowellReference)
        {
            string innerUrl = $"v1/accounts/{Uri.EscapeDataString(lowellReference)}";
            return await GetResultAsync<CaseFlowMyAccountsDetailDto>(innerUrl);
        }

        public async Task<RecievedTransactionsDto> GetTransactionsAsync(GetTransactionsDto model)
        {
            Logger.LogInformation("CaseflowApiProxy GetTransactionsAsync action account service");

            var uri = $"v1/accounts/{Uri.EscapeDataString(model.AccountReference)}/transactions?limit=0";
            var result = await GetResultAsync<RecievedTransactionsDto>(uri);

            return result;
        }

        public async Task<RecievedTransactionsDto> GetTransactionsAsync(string lowellReference, uint limit)
        {
            var uri = $"v1/accounts/{Uri.EscapeDataString(lowellReference)}/transactions?limit={limit}";
            var result = await GetResultAsync<RecievedTransactionsDto>(uri);

            return result;
        }

        public async Task SendCallbackRequest(CallbackDto dto)
        {
            try
            {
                var innerUrl = "v1/contact/callback-request";

                await PutwithNoReturnTypeAsync(innerUrl, dto);
            }
            catch (Exception ex)
            {
                Logger.LogError("An error occured.", ex);
            }
        }

        public async Task<ContactPreferencesRetrieved> GetContactPreferences(string lowellReference)
        {
            try
            {
                var uri = $"v1/accounts/{lowellReference}/contact-details";
                var result = await GetResultAsync<ContactPreferencesRetrieved>(uri);
                return result;
            }
            catch (ProxyException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    //if caseflow is unable to find contact preferences details
                    return null;
                }
                throw;
                
            }
        }

        public async Task SaveContactPreferences(SaveContactPreferences dto)
        {
            //var uri = $"v1/accounts/{dto.LowellReference}/contact-details";
            var uri = $"v1/users/{dto.CaseflowUserId}/contact-details";
            await PutwithNoReturnTypeAsync(uri, dto);
        }

        public async Task UpdateUserEmailAsync(UpdateEmailDto dto, string replayId)
        {
            CreatedUserDto user = await GetResultAsync<CreatedUserDto>($"v1/users/{Uri.EscapeDataString(dto.UserId)}");
            user.EmailAddress = dto.EmailAddress;

            await PutWithNoReturnTypeAsync($"v1/users/{dto.UserId}", user, replayId);
        }
    }
}