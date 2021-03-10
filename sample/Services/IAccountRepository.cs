using System;
using System.Collections.Generic;
using AJP.MediatrEndpoints.Sample.RequestHandlers.Accounts;

namespace AJP.MediatrEndpoints.Sample.Services
{
    public interface IAccountRepository
    {
        AccountDetails GetById(string id);
        IEnumerable<AccountDetails> GetAll(string sortcode = "", Func<decimal, bool> balanceCriteria = null, AccountStatus statusFilter = AccountStatus.Any);
        AccountDetails Create(AccountDetails newAccount);
        string Delete(string id);
        AccountDetails Block(string id);
        AccountDetails UnBlock(string id);
    }
}