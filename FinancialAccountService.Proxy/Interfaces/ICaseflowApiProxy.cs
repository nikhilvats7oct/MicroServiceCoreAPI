using FinancialAccountService.Models.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinancialAccountService.Proxy.Interfaces
{
    public interface ICaseflowApiProxy
    {
        Task<CaseflowPingDto> HitHeartBeat();
        List<string> GetLinkedAccountsAsync(string reference);
        Task<RecievedAccountDto> CheckDataProtection(RegisterValidationDto dto);
        Task<RecievedCustomerSummariesDto> CheckHasWebAccountAsync(WebRegisteredDto dto);
        Task SendForgotPasswordAsync(SendForgottenPasswordDto sendForgottenPasswordDtoDto, string userId, string replayId);
        Task SendRegistrationEmailAsync(SendEmailDto model, string lowellReference, string replayId);
        Task CompleteRegistrationAsync(CompleteRegistrationDto model, string replayId);
        Task<CaseFlowMyAccountsDto> GetMyAccountsAsync(string userId);
        Task<CaseFlowMyAccountsDto> GetMyAccountsSummaryAsync(string accountId);
        Task<CaseFlowMyAccountsDetailDto> GetMyAccountsDetailAsync(string lowellReference);
        Task<RecievedTransactionsDto> GetTransactionsAsync(GetTransactionsDto model);
        Task<RecievedTransactionsDto> GetTransactionsAsync(string lowellReference, uint limit);
        Task<CreatedUserDto> GetUserAsync(string userId);
        Task LogWebActivity(WebActivityDto webActivity);
        Task SendConfirmEmailEmailAsync(SendEmailDto model, string lowellReference, string replayId);
        Task CompleteDeregistrationAsync(CompleteDeregistrationDto completeDeregistrationDto, string replayId);
        Task SendCallbackRequest(CallbackDto dto);

        Task<ContactPreferencesRetrieved> GetContactPreferences(string lowellReference);
        Task SaveContactPreferences(SaveContactPreferences dto);

        Task UpdateUserEmailAsync(UpdateEmailDto dto, string replayId);
    }
}