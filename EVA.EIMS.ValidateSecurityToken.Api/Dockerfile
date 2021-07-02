FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY Publish/ .
ENTRYPOINT ["dotnet","EVA.EIMS.ValidateSecurityToken.Api.dll"]
EXPOSE 80

