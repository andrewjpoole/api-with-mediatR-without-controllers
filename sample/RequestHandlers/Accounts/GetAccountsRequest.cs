using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AJP.MediatrEndpoints.PropertyAttributes;
using MediatR;
using mediatr_test.Services;

namespace mediatr_test.RequestHandlers.Accounts
{
    public class GetAccountsRequest : IRequest<IEnumerable<AccountDetails>>
    {
        [OptionalProperty]
        [SwaggerExample("2099")]
        public string SortCodeMatchOptional { get; set; }

        [OptionalProperty]
        [SwaggerExample("<-1000")]
        public string BalanceFilterOptional { get; set; }
    }

    public class GetAccountsRequestHandler : IRequestHandler<GetAccountsRequest, IEnumerable<AccountDetails>>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountsRequestHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        
        public Task<IEnumerable<AccountDetails>> Handle(GetAccountsRequest request, CancellationToken cancellationToken)
        {
            return request.BalanceFilterOptional switch
            {
                string s when s.StartsWith("<") => Task.FromResult(
                    _accountRepository.GetAll(request.SortCodeMatchOptional,
                        balance => balance < decimal.Parse(s.Remove(0, 1)))),
                string s when s.StartsWith(">") => Task.FromResult(
                    _accountRepository.GetAll(request.SortCodeMatchOptional,
                        balance => balance > decimal.Parse(s.Remove(0, 1)))),
                _ => Task.FromResult(_accountRepository.GetAll(request.SortCodeMatchOptional))
            };
        }
    }
}