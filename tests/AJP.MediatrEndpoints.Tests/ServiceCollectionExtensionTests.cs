using System.Linq;
using FluentAssertions;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AJP.MediatrEndpoints.Tests
{
    public class ServiceCollectionExtensionTests
    {
        [Fact]
        public void AddMediatrEndpoints_should_add_mediator_depenecies()
        {
            var services = new ServiceCollection();

            var result = services.AddMediatrEndpoints(typeof(ServiceCollectionExtensionTests));

            result.FirstOrDefault(s => s.ServiceType == typeof(IMediator)).Should().NotBeNull();
            result.FirstOrDefault(s => s.ServiceType == typeof(IPublisher)).Should().NotBeNull();
            result.FirstOrDefault(s => s.ServiceType == typeof(ISender)).Should().NotBeNull();
            result.FirstOrDefault(s => s.ServiceType == typeof(IRequestExceptionHandler<,,>)).Should().NotBeNull();
        }
    }
}