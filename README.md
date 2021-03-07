
# Testing

## Code-coverage

``` pwsh
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
reportgenerator "-reports:./FxCreditSystem.Repository.Test/coverage.cobertura.xml" "-targetdir:/tmp/report/"
firefox /tmp/report/index.htm
```