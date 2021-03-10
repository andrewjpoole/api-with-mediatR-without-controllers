﻿using System.Threading;
using System.Threading.Tasks;
using AJP.MediatrEndpoints.PropertyAttributes;
using AJP.MediatrEndpoints.Sample.Services;
using MediatR;

namespace AJP.MediatrEndpoints.Sample.RequestHandlers.Accounts
{
    public class UpdateAccountStatusRequest : IRequest<AccountDetails>
    {
        [SwaggerRouteParameter]
        public string Id { get; set; }
        public bool Blocked { get; set; }
    }
    
    public class UpdateAccountStatusRequestHandler : IRequestHandler<UpdateAccountStatusRequest, AccountDetails>
    {
        private readonly IAccountRepository _accountRepository;

        public UpdateAccountStatusRequestHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        
        public Task<AccountDetails> Handle(UpdateAccountStatusRequest request, CancellationToken cancellationToken)
        {
            return request.Blocked switch
            {
                true => Task.FromResult(_accountRepository.Block(request.Id)),
                false => Task.FromResult(_accountRepository.UnBlock(request.Id))
            };
        }
    }
}