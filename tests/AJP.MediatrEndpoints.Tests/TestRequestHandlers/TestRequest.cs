using AJP.MediatrEndpoints.PropertyAttributes;
using AJP.MediatrEndpoints.SwaggerSupport.Attributes;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace AJP.MediatrEndpoints.Tests.TestRequestHandlers
{
    public class TestRequest: IRequest<TestResponse>
    {
        [SwaggerRouteParameter()]
        public string Prop1 { get; init; }
        
        [OptionalProperty]
        [SwaggerQueryParameter]
        public string Prop2 { get; init; }
        
        [OptionalProperty]
        [SwaggerHeaderParameter]
        public string Prop3 { get; init; }
        
        [OptionalProperty]
        [SwaggerExampleValue("43hg5f")]
        [SwaggerDescription("3k4jh5gk4jh5g")]
        public string Prop4 { get; init; }
    }
}