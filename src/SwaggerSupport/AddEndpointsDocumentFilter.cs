using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Routing;
using AJP.MediatrEndpoints;

namespace AJP.MediatrEndpoints.SwaggerSupport
{
    public class AddEndpointsDocumentFilter : IDocumentFilter
    {
        private readonly EndpointDataSource _endpointDataSource;

        public AddEndpointsDocumentFilter(EndpointDataSource endpointDataSource)
        {
            _endpointDataSource = endpointDataSource;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var endpoint in _endpointDataSource.Endpoints)
            {
                var swaggerDecorater = endpoint.Metadata.GetMetadata<SwaggerDecoraterAttribute>();

                var requestSchema = context.SchemaGenerator.GenerateSchema(swaggerDecorater.RequestType, context.SchemaRepository);
                var responseSchema = context.SchemaGenerator.GenerateSchema(swaggerDecorater.ResponseType, context.SchemaRepository);
                
                // ToDo look up whether there is already a path existing or else add one


                var pathItem = new OpenApiPathItem();
                var operation = new OpenApiOperation();
                var operationParemter = new OpenApiParameter();
                operationParemter.In = ParameterLocation.Path;
                operationParemter.Name = "exampleParam";
                operationParemter.Schema = requestSchema;
                operation.Parameters.Add(operationParemter);
                pathItem.Operations.Add(swaggerDecorater.OperationType, operation);
                swaggerDoc.Paths.Add(endpoint.DisplayName, pathItem);
            }
        }
    }
}
