using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace mediatr_test.RequestHandlers.Accounts
{
    public class DeleteAccountByIdRequest : IRequest<AccountDeletedResponse>
    {
        public string Id { get; set; }
    }
    
    public class DeleteAccountByIdRequestHandler : IRequestHandler<DeleteAccountByIdRequest, AccountDeletedResponse>
    {
        private readonly IAccountRepository _accountRepository;

        public DeleteAccountByIdRequestHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        
        public Task<AccountDeletedResponse> Handle(DeleteAccountByIdRequest request, CancellationToken cancellationToken)
        {
            _accountRepository.Delete(request.Id);
            return Task.FromResult(new AccountDeletedResponse
            {
                Id = request.Id
            });
        }
    }
}