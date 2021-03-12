using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;

namespace AJP.MediatrEndpoints
{
    public class RequestGenericExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
        where TException : Exception {

        public async Task Handle(TRequest request,
            TException exception,
            RequestExceptionHandlerState<TResponse> state,
            CancellationToken cancellationToken)
        {
        }
    }
}