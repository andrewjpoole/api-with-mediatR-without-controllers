using System.Threading;
using System.Threading.Tasks;
using AJP.MediatrEndpoints.PropertyAttributes;
using AJP.MediatrEndpoints.Sample.Services;
using MediatR;

namespace AJP.MediatrEndpoints.Sample.RequestHandlers.Accounts
{
    public class GetAccountByIdRequest : IRequest<AccountDetails>
    {
        [SwaggerRouteParameter]
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