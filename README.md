# api-with-mediatR-without-controllers

## Background

There are some increasingly widely help opinions that controllers are somewhat old fashioned.
A principal dev where I work demonstrated some middleware which bound MediatR request handlers to routes.
As of aspnetcore 3 we now have Endpoint Routing, which easily allows a RequestDelegate to be bound to a route without even needing custom middleware. 
Could Endpoint Routing + some reflection + MediatR provide a more modern alternative to controllers?
and in a real-world use case, would anything be missing?

## How to use it

## Cross cutting concerns (such as request logging stats collection)

## Swagger Support

## Todo

1) finish readme
2) add route param name checker for swagger
3) outbound status code + headers, currently get 200 even if theres an error in the repository etc, possibly look at using mediatr behaviour pipeline
4) hook up statsGatherer to dotnet perf counters? 
