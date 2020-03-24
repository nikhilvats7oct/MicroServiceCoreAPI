using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinancialAccountService.Models.DataTransferObjects;

namespace FinancialAccountService.Services.Interfaces
{
    public interface IWebActivityService
    {
        Task LogWebActivity(WebActivityDto webActivity);
    }
}
