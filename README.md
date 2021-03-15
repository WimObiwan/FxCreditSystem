
# Testing

## Code-coverage

[![codecov.io](https://codecov.io/github/wimobiwan/FxCreditSystem/coverage.svg?branch=master)]
(https://codecov.io/github/wimobiwan/FxCreditSystem?branch=master)


``` pwsh
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=coberdotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=coberturatura
reportgenerator "-reports:./FxCreditSystem.Repository.Test/coverage.cobertura.xml" "-targetdir:/tmp/report/"
firefox /tmp/report/index.htm
```