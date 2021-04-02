cd C:\dev\api-with-mediatR-without-controllers\tests\

coverlet ".\AJP.MediatrEndpoints.Tests\bin\Debug\net5.0\AJP.MediatrEndpoints.Tests.dll" --target "dotnet" --targetargs "test AJP.MediatrEndpoints.Tests\AJP.MediatrEndpoints.Tests.csproj --no-build" --format cobertura

del "./coverageReport/*.*?"

reportgenerator -reports:.\coverage.cobertura.xml -targetdir:./coverageReport

start C:\dev\api-with-mediatR-without-controllers\tests\coverageReport\index.html