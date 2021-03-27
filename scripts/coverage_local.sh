
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./CoverageResults/
reportgenerator "-reports:./*/CoverageResults/coverage.cobertura.xml" "-targetdir:/tmp/report/"
