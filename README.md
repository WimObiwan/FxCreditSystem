
# Testing

## Code-coverage

[![codecov.io](https://codecov.io/github/wimobiwan/FxCreditSystem/coverage.svg?branch=master)](https://codecov.io/github/wimobiwan/FxCreditSystem?branch=master)


``` pwsh
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./CoverageResults/
reportgenerator "-reports:./*/CoverageResults/coverage.cobertura.xml" "-targetdir:/tmp/report/"
firefox /tmp/report/index.htm
```

Update packages

```
dotnet list package --outdated
```

Migrations

```
dotnet ef migrations add Initial --project ./FxCreditSystem.Repository
dotnet ef migrations remove

dotnet ef database update --project ./FxCreditSystem.API
```