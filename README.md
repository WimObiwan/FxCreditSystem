
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

# Docker

https://docs.docker.com/samples/dotnetcore/
https://docs.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=windows#create-the-dockerfile

```
sudo docker build -t fxcreditsystem .
sudo docker run -it -p 7991:80 --name fxcreditsystem-api fxcreditsystem
sudo docker rm -f fxcreditsystem-api

sudo docker exec -it fxcreditsystem-api-1 bash
==> apt install procps
==> ps -a

sudo docker-compose up -d
sudo docker-compose down

sudo docker swarm init --advertise-addr 192.168.0.130
sudo docker stack deploy --compose-file ./docker-compose.yml test
sudo docker stack rm test

pw=$(strings /dev/urandom | grep -o '[[:alnum:]]' | head -n 30 | tr -d '\n'; echo)
echo $pw | sudo docker secret create db_sqlserver_sa -

sudo docker service update test_api

sudo docker service ls
sudo docker service inspect --pretty test_api
sudo docker service scale test_api=5

# Redeploy, after changes
sudo docker build -t fxcreditsystem .

# this works (but gives errors, since port is in use)
sudo docker-compose up --force-recreate --build
sudo docker service update --force test_api

# this works
sudo docker-compose build
sudo docker service update --force test_api

# configuration
sudo docker config create fxcreditsystem_api_appsettings.json-v2 ./FxCreditSystem.API/appsettings.Production.json
sudo docker service update \
    --config-rm fxcreditsystem_api_appsettings.json-v1 \
    --config-add source=fxcreditsystem_api_appsettings.json-v2,target=/app/appsettings.Production.json,mode=0440 \
    test_api
sudo docker config rm fxcreditsystem_api_appsettings.json-v1

# http://127.0.0.1:7991/hc
# http://127.0.0.1:7991/openapi
```


appsettings: server=db  #telnet db 1433