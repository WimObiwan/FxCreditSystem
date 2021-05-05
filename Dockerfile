
##########
# BUILD
##########

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /src
COPY ["FxCreditSystem.sln", "./"]WORKDIR /app
COPY ["FxCreditSystem.API/FxCreditSystem.API.csproj", "FxCreditSystem.API/"]
COPY ["FxCreditSystem.API.Integration.Tests/FxCreditSystem.API.Integration.Tests.csproj", "FxCreditSystem.API.Integration.Tests/"]
COPY ["FxCreditSystem.API.Tests/FxCreditSystem.API.Tests.csproj", "FxCreditSystem.API.Tests/"]
COPY ["FxCreditSystem.Common/FxCreditSystem.Common.csproj", "FxCreditSystem.Common/"]
COPY ["FxCreditSystem.Core/FxCreditSystem.Core.csproj", "FxCreditSystem.Core/"]
COPY ["FxCreditSystem.Core.Tests/FxCreditSystem.Core.Tests.csproj", "FxCreditSystem.Core.Tests/"]
COPY ["FxCreditSystem.Repository/FxCreditSystem.Repository.csproj", "FxCreditSystem.Repository/"]
COPY ["FxCreditSystem.Repository.Tests/FxCreditSystem.Repository.Tests.csproj", "FxCreditSystem.Repository.Tests/"]
RUN ["dotnet", "restore"]

COPY [".", "./"]
RUN ["dotnet", "build", "-c", "Release", "--no-restore"]

RUN ["dotnet", "test", "-c", "Release", "--no-restore", "--no-build"]

RUN ["dotnet", "publish", "-c", "Release", "--no-restore", "--no-build", "-o", "./out/"]


##########
# RUN
##########

FROM mcr.microsoft.com/dotnet/aspnet:5.0 
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT Development
COPY --from=build-env ["src/out", "./"]
EXPOSE 80
ENTRYPOINT ["dotnet", "/app/FxCreditSystem.API.dll"]
