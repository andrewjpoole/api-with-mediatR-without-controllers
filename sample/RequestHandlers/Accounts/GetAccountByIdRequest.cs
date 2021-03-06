﻿using System.Threading;
using System.Threading.Tasks;
using AJP.MediatrEndpoints.Exceptions;
using AJP.MediatrEndpoints.Sample.Services;
using AJP.MediatrEndpoints.Swagger.Attributes;
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
            var account = _accountRepository.GetById(request.Id);

            if (account == null)
                throw new NotFoundHttpException($"account with id:{request.Id} not found", "resource not found");
            
            return Task.FromResult(account);
        }
    }
}