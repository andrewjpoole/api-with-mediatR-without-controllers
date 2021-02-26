# api-with-mediatR-without-controllers


## Todo

1) readme
2) Add all required properties to the TRequest
3) In the requestdelegatebuilder
```
deserialise the requestBody or "{}" if empty
   use reflection against the TRequest
     get and loop through all public properties
       if they dont already exist on the deserialised json object, look in route, then query string, then headers and add using JsonElementExtensions
```
4) remove the wrappers (if you need something add a prop to TRequest)
5) swagger DocumentFilter should then be able to also use reflection, but it might need some hints as to paramterLocation? Attributes? namingConvention?
6) posibly look at using mediatr behaviour pipeline for outbound status code + headers etc?
7) hook up statsGatherer to dotnet perf counters? 
