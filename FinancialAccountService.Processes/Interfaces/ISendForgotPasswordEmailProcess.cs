﻿using FinancialAccountService.Models.DataTransferObjects;
using System.Threading.Tasks;

namespace FinancialAccountService.Processes.Interfaces
{
    public interface ISendForgotPasswordEmailProcess
    {
        Task SendForgotPasswordAsync(ForgotPasswordDto model);
    }
}
