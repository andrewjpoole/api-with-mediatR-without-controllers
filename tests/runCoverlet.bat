cd C:\dev\api-with-mediatR-without-controllers\tests\AJP.MediatrEndpoints.Tests

coverlet ".\bin\Debug\net5.0\AJP.MediatrEndpoints.Tests.dll" --target "dotnet" --targetargs "test AJP.MediatrEndpoints.Tests.csproj --no-build" --format cobertura

reportgenerator -reports:.\AJP.MediatrEndpoints.Tests\coverage.cobertura.xml -targetdir:./coverageReport

start C:\dev\api-with-mediatR-without-controllers\tests\coverageReport\index.html