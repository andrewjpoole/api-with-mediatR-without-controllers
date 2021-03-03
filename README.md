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

1) readme
2) fix route value issue from swagger
3) possibly look at using mediatr behaviour pipeline for outbound status code + headers etc?
4) hook up statsGatherer to dotnet perf counters? 
