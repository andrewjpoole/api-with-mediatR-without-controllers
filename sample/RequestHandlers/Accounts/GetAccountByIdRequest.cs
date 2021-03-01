using System.Threading;
using System.Threading.Tasks;
using AJP.MediatrEndpoints.PropertyAttributes;
using MediatR;
using mediatr_test.Services;

namespace mediatr_test.RequestHandlers.Accounts
{
    public class GetAccountByIdRequest : IRequest<AccountDetails>
    {
        [RouteParameter]
        public string Id { get; set; }
    }
    
    public class GetAccountByIdRequestHandler : IRequestHandler<GetAccountByIdRequest, AccountDetails>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountByIdRequestHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        
        public Task<AccountDetails> Handle(GetAccountByIdRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_accountRepository.GetById(request.Id));
        }
    }
}