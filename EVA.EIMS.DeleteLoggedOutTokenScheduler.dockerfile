FROM mcr.microsoft.com/dotnet/core/aspnet:2.1.22-bionic
WORKDIR /App
COPY Publish/ .
ENTRYPOINT ["dotnet", "EVA.EIMS.DeleteLoggedOutTokenScheduler.dll"]
EXPOSE 80