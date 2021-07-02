FROM mcr.microsoft.com/dotnet/core/aspnet:2.1.22-bionic
WORKDIR /App
COPY Publish/ .
ENTRYPOINT ["dotnet", "EVA.EIMS.Security.API.dll"]
EXPOSE 80