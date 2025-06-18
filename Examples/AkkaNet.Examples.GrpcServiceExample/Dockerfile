# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
RUN dotnet publish "./AkkaNet.Examples.GrpcServiceExample.csproj" -c Release --self-contained -o /app
EXPOSE 5000

ENTRYPOINT ["dotnet", "AkkaNet.Examples.GrpcServiceExample.dll"]