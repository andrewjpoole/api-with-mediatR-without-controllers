using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using AJP.MediatrEndpoints.EndpointRegistration;
using AJP.MediatrEndpoints.Swagger.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AJP.MediatrEndpoints.Swagger
{
    public static class OpenApiOperationRenderer
    {
        public static OpenApiOperation Render(
            Endpoint endpoint,
            EndpointMetadataDecoratorAttribute swaggerDecorator, 
            OpenApiDocument swaggerDoc,
            DocumentFilterContext context)
        {
            
            var responseSchema = context.SchemaGenerator.GenerateSchema(swaggerDecorator.ResponseType, context.SchemaRepository);
            var responseStatusCode = swaggerDecorator.SuccessfulStatusCode;
            var responseStatusCodeName = ReasonPhrases.GetReasonPhrase(responseStatusCode);
            
            var endpointSwaggerDescription =
                (SwaggerDescriptionAttribute)swaggerDecorator.RequestType.GetCustomAttributes(typeof(SwaggerDescriptionAttribute)).FirstOrDefault();
            
            var operation = new OpenApiOperation
            {
                Tags = new List<OpenApiTag>
                {
                    new()
                    {
                        Name = swaggerDecorator.EndpointGroupName // this groups operations together into a path.
                    }
                },
                Description = endpointSwaggerDescription?.Description ?? swaggerDecorator.Description,
                Parameters = new List<OpenApiParameter>(),
                Responses = new OpenApiResponses()
            };

            var operationResponse = new OpenApiResponse
            {
                Description = responseStatusCodeName, Content = new Dictionary<string, OpenApiMediaType>()
            };
            var operationResponseMediaType = new OpenApiMediaType();
            if (responseStatusCode != StatusCodes.Status204NoContent)
                operationResponseMediaType.Schema = responseSchema;
            
            operationResponse.Content.Add("default", operationResponseMediaType);
            operation.Responses.Add(responseStatusCode.ToString(), operationResponse);

            var (parameters, bodyExampleObject) = OpenApiParameterRenderer.Render(swaggerDecorator);
            foreach (var parameter in parameters)
                operation.Parameters.Add(parameter);
            
            // Now add the bodyExampleObject, if there are any props left (not already rendered as route, query or header)
            if (bodyExampleObject.EnumerateObject().Any())
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        {
                            "default", new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Default = new OpenApiString(bodyExampleObject.GetRawText()),
                                }
                            }
                        }
                    }
                };
            
            return operation;
        }
    }
}