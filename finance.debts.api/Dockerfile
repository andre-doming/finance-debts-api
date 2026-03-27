FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build 
WORKDIR /src 

COPY . . 

RUN dotnet restore finance-debts-api/finance.debts.api/finance.debts.api.csproj 

RUN dotnet publish finance-debts-api/finance.debts.api/finance.debts.api.csproj -c Release -o /app/publish 

FROM mcr.microsoft.com/dotnet/aspnet:9.0 
WORKDIR /app 

COPY --from=build /app/publish . 

EXPOSE 8080 

ENTRYPOINT ["dotnet", "finance.debts.api.dll"]