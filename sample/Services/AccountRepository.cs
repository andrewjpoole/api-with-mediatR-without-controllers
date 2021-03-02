using System;
using System.Collections.Generic;
using System.Linq;
using mediatr_test.RequestHandlers.Accounts;

namespace mediatr_test.Services
{
    public class AccountRepository : IAccountRepository
    {
        private readonly Dictionary<string, AccountDetails> _accounts;

        public AccountRepository()
        {
            _accounts = new List<AccountDetails>
            {
                new AccountDetails
                {
                    Id = "7ed8a882-214d-4a41-bbc4-1d16970481a5",
                    SortCode = "040405",
                    AccountNumber = "15846476",
                    Balance = 58228181.34m,
                    Status = AccountStatus.Unblocked
                },
                new AccountDetails
                {
                    Id = "72fb07f2-d44e-4535-8277-73ec7a4af705",
                    SortCode = "040405",
                    AccountNumber = "23581953",
                    Balance = 3613.99m,
                    Status = AccountStatus.Unblocked
                },
                new AccountDetails
                {
                    Id = "232584fe-5dee-4c9d-8198-eacaf8a2a224",
                    SortCode = "040405",
                    AccountNumber = "92701053",
                    Balance = 92.11m,
                    Status = AccountStatus.Unblocked
                },
                new AccountDetails
                {
                    Id = "6e23be24-a915-47c5-ba1e-80c88d59c905",
                    SortCode = "229940",
                    AccountNumber = "85548970",
                    Balance = 3021.00m,
                    Status = AccountStatus.Unblocked
                },
                new AccountDetails
                {
                    Id = "4dba7bde-8093-4920-bffb-b825647af154",
                    SortCode = "209940",
                    AccountNumber = "82539351",
                    Balance = 88841746.86m,
                    Status = AccountStatus.Unblocked
                },
                new AccountDetails
                {
                    Id = "750c9f57-2d00-48a5-97e3-f26a85e070c3",
                    SortCode = "209941",
                    AccountNumber = "46385481",
                    Balance = 7.85m,
                    Status = AccountStatus.Unblocked
                },
                new AccountDetails
                {
                    Id = "ea35af30-54eb-441c-9b7b-2948bdf5604f",
                    SortCode = "209940",
                    AccountNumber = "92114140",
                    Balance = -616375.0m,
                    Status = AccountStatus.Blocked
                },
                new AccountDetails
                {
                    Id = "d627a2fa-b9b1-4cdf-b04b-048188d343e8",
                    SortCode = "140505",
                    AccountNumber = "72940752",
                    Balance = -991.87m,
                    Status = AccountStatus.Unblocked
                },
                new AccountDetails
                {
                    Id = "f32d1be8-93be-45f5-9889-80cbc65ef1ab",
                    SortCode = "160405",
                    AccountNumber = "82956315",
                    Balance = -3.80m,
                    Status = AccountStatus.Unblocked
                },
                new AccountDetails
                {
                    Id = "2dee0818-e5dc-435f-abd7-e0e69d74f5b2",
                    SortCode = "040405",
                    AccountNumber = "98167665",
                    Balance = 75_559_726.00m,
                    Status = AccountStatus.Unblocked
                }
            }.ToDictionary(x => x.Id);
        }
        
        public AccountDetails GetById(string id) => _accounts[id];

        public IEnumerable<AccountDetails> GetAll(string sortcode = "", Func<decimal, bool> balanceCriteria = null, AccountStatus statusFilter = AccountStatus.Any)
        {
            var filteredAccounts = _accounts.Select(x => x.Value);
            
            if (!string.IsNullOrEmpty(sortcode))
                filteredAccounts = filteredAccounts.Where(x => x.SortCode.StartsWith(sortcode)).ToList();

            if (balanceCriteria != null)
                filteredAccounts = filteredAccounts.Where(x => balanceCriteria(x.Balance) == true).ToList();

            if (statusFilter != AccountStatus.Any)
                filteredAccounts = filteredAccounts.Where(x => x.Status == statusFilter);

            return filteredAccounts;
        }

        public AccountDetails Create(AccountDetails newAccount)
        {
            // Add some validation here
            
            var newId = Guid.NewGuid().ToString();
            newAccount.Id = newId;
            _accounts.Add(newId, newAccount);
            return _accounts[newId];
        }

        public string Delete(string id)
        {
            if (_accounts.ContainsKey(id) == false)
                return string.Empty;

            _accounts.Remove(id);
            return id;
        }

        public AccountDetails Block(string id)
        {
            if (!_accounts.ContainsKey(id) == false)
                return null;

            _accounts[id].Status = AccountStatus.Blocked;
            return _accounts[id];
        }

        public AccountDetails UnBlock(string id)
        {
            if (!_accounts.ContainsKey(id) == false)
                return null;

            _accounts[id].Status = AccountStatus.Unblocked;
            return _accounts[id];
        }
    }
}