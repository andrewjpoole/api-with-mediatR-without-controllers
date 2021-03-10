﻿using System.Threading;
using System.Threading.Tasks;
using AJP.MediatrEndpoints.Sample.Services;
using MediatR;

namespace AJP.MediatrEndpoints.Sample.RequestHandlers.Accounts
{
    public class CreateAccountRequest : IRequest<AccountDetails>
    {
        public string SortCode { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
    }
    
    public class CreateAccountRequestHandler : IRequestHandler<CreateAccountRequest, AccountDetails>
    {
        private readonly IAccountRepository _accountRepository;

        public CreateAccountRequestHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        
        public Task<AccountDetails> Handle(CreateAccountRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_accountRepository.Create(new AccountDetails
            {
                SortCode = request.SortCode,
                AccountNumber = request.AccountNumber,
                Balance = request.Balance,
                Status = AccountStatus.Unblocked
            }));
        }
    }
}