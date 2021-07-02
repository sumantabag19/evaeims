FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY Publish/ .
ENTRYPOINT ["dotnet","AuthorizationServer.Api.dll"]
EXPOSE 80

