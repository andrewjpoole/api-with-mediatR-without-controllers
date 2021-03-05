# api-with-mediatR-without-controllers

## Background

There are some increasingly widely help opinions that controllers are somewhat old fashioned.
A principal dev where I work demonstrated some middleware which bound MediatR request handlers to routes in an aspnetcore app.
As of aspnetcore 3 we now have Endpoint Routing, which easily allows a RequestDelegate to be bound to a route without even needing custom middleware. 
I wondered if Endpoint Routing + some reflection + MediatR could provide a more modern alternative to controllers and in a real-world use case, would anything be missing?

## What is it

A small library which allows Endpoints to be easily registered wired up to a MediatR IRequestHandler.
The RequestHandlers should not have an knowledge of being behind aspnetcore, but instead focus on their business purpose.
When a request is received, public properties from the TRequest type are looked up and satisfied from first the request body, then the route/path variables, 
then variables from the query string and finally from the request headers collection, if a property is still not found AND not decorated with the Optional attribute, 
then the request will be returned as a bad request.
The TResponse returned from the RequestHandler is serialied into the response body.
If the TResponse has a public int property named ResponseCode, then it will be used as the response HttpStatusCode.

## How to use it

Check out the sample project, basically:

* Add MediatR package 
* Define some requests, responses and some request handlers as per documentation [mediatR wiki](https://github.com/jbogard/MediatR/wiki)
* Use the `services.AddMediatR(typeof(Startup));` extension method to register the types above
* Add MediatrEndpoints package
* Define Endpoints using the extension methods
```c#
endpoints.MapGroupOfEndpointsForAPath("/api/v1/accounts", "Accounts", "everything to do with accounts")
    .WithGet<GetAccountsRequest, IEnumerable<AccountDetails>>("/")
    .WithGet<GetAccountByIdRequest, AccountDetails>("/{Id}") // route parameter name must match property on TRequest, including case!! otherwise swagger breaks
    .WithPost<CreateAccountRequest, AccountDetails>("/")
```

## Exception handling and response codes

## Cross cutting concerns (such as request logging stats collection)

The library contains an interface named IMediatrEndpointsProcessors:
```c#
public interface IMediatrEndpointsProcessors
    {
        Action<HttpContext, ILogger> PreProcess {get; set;}
        Action<HttpContext, TimeSpan, ILogger> PostProcess {get; set;}
        Action<Exception, HttpContext, ILogger> ErrorProcess {get; set;}
    }
```
If an implementation if found in the DI container, its actions will be called at the appropriate times, 
giving opportunity for request pre and post processing to take place with access to the HttpContext, 
this makes things like standard logging of requests/responses or timing to take place. 
E.g. in the sample I check for the presence of a CorrelationId header, create one if it doesn't exist and propergate it to the response headers. 

## Swagger Support

* Optionally add Swashbuckle package and configure as per the [microsoft docs](https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-5.0&tabs=visual-studio)
* Then add the AddEndpointsDocumentFilter when configuring SwaggerGen, see the Startup.cs in the sample.
`c.DocumentFilter<AddEndpointsDocumentFilter>();`
* This will loop through the endpoints registered, pull out some metadata and add paths, operations and parameters accordingly.

## Testing

## Todo

* access to CorrelationId and HttpContext via DI if needed?
* add route param name checker for swagger
* outbound status code + headers, currently get 200 even if theres an error in the repository etc, possibly look at using mediatr behaviour pipeline
* hook up statsGatherer to dotnet perf counters? 
