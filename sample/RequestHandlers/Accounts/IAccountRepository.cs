using System;
using System.Collections.Generic;

namespace mediatr_test.RequestHandlers.Accounts
{
    public interface IAccountRepository
    {
        AccountDetails GetById(string id);
        IEnumerable<AccountDetails> GetAll(string sortcode = "", Func<decimal, bool> balanceCriteria = null);
        AccountDetails Create(AccountDetails newAccount);
        string Delete(string id);
        AccountDetails Block(string id);
        AccountDetails UnBlock(string id);
    }
}