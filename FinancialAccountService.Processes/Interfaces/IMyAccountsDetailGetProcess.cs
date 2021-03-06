﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinancialAccountService.Models.DataTransferObjects;

namespace FinancialAccountService.Processes.Interfaces
{
    public interface IMyAccountsDetailGetProcess
    {
        Task<CaseFlowMyAccountsDetailDto> GetMyAccountsDetail(string lowellReference);
    }
}
