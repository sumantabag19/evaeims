FROM mcr.microsoft.com/dotnet/core/aspnet:2.1.22-bionic
WORKDIR /App
COPY Publish/ .
ENTRYPOINT ["dotnet", "AuthorizationServer.Api.dll"]
EXPOSE 80