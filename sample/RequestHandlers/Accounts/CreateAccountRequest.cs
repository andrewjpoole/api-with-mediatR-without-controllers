using System.Threading;
using System.Threading.Tasks;
using AJP.MediatrEndpoints.PropertyAttributes;
using AJP.MediatrEndpoints.Sample.Services;
using MediatR;

namespace AJP.MediatrEndpoints.Sample.RequestHandlers.Accounts
{
    public class CreateAccountRequest : IRequest<CreateAccountResponse>
    {
        public string SortCode { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
    }
    
    [StatusCode(201, "Created")]
    public class CreateAccountResponse : AccountDetails
    {
        public CreateAccountResponse(AccountDetails account)
        {
            Id = account.Id;
            SortCode = account.SortCode;
            AccountNumber = account.AccountNumber;
            Balance = account.Balance;
            Status = account.Status;
        }
    }
    
    public class CreateAccountRequestHandler : IRequestHandler<CreateAccountRequest, CreateAccountResponse>
    {
        private readonly IAccountRepository _accountRepository;

        public CreateAccountRequestHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        
        public Task<CreateAccountResponse> Handle(CreateAccountRequest request, CancellationToken cancellationToken)
        {
            var newAccount = _accountRepository.Create(new AccountDetails()
            {
                SortCode = request.SortCode,
                AccountNumber = request.AccountNumber,
                Balance = request.Balance,
                Status = AccountStatus.Unblocked
            });
            return Task.FromResult(new CreateAccountResponse(newAccount));
        }
    }
}