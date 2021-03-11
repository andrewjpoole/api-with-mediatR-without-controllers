using System.Threading;
using System.Threading.Tasks;
using AJP.MediatrEndpoints.PropertyAttributes;
using AJP.MediatrEndpoints.Sample.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AJP.MediatrEndpoints.Sample.RequestHandlers.Accounts
{
    public class DeleteAccountByIdRequest : IRequest<AccountDeletedResponse>
    {
        [SwaggerRouteParameter]
        public string Id { get; set; }
    }
    
    public class AccountDeletedResponse
    {
        public string Id { get; init; }

        public int StatusCode { get; init; }
    }
    
    public class DeleteAccountByIdRequestHandler : IRequestHandler<DeleteAccountByIdRequest, AccountDeletedResponse>
    {
        private readonly ILogger<DeleteAccountByIdRequestHandler> _logger;
        private readonly IAccountRepository _accountRepository;
        private readonly IEndpointContextAccessor _endpointContextAccessor;

        public DeleteAccountByIdRequestHandler(ILogger<DeleteAccountByIdRequestHandler> logger, IAccountRepository accountRepository, IEndpointContextAccessor endpointContextAccessor)
        {
            _logger = logger;
            _accountRepository = accountRepository;
            _endpointContextAccessor = endpointContextAccessor;
        }
        
        public Task<AccountDeletedResponse> Handle(DeleteAccountByIdRequest request, CancellationToken cancellationToken)
        {
            _accountRepository.Delete(request.Id);
            
            var correlationId = _endpointContextAccessor.CurrentContext.Request.Headers["CorrelationId"];
            _logger.LogInformation($"Deleted account {request.Id} from request with CorrelationId: {correlationId}");
            
            return Task.FromResult(new AccountDeletedResponse
            {
                Id = request.Id,
                StatusCode = 204
            });
        }
    }
}