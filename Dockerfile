# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY WhatsAppAPISolution.sln ./
COPY WhatsAppAPISolutionAPI/WhatsAppAPISolutionAPI.csproj WhatsAppAPISolutionAPI/
COPY WhatsAppAPISolutionBL/WhatsAppAPISolutionBL.csproj WhatsAppAPISolutionBL/
COPY WhatsAppAPISolutionDL/WhatsAppAPISolutionDL.csproj WhatsAppAPISolutionDL/

RUN dotnet restore WhatsAppAPISolution.sln

COPY . .

RUN dotnet publish WhatsAppAPISolutionAPI/WhatsAppAPISolutionAPI.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out .

COPY WhatsAppAPISolutionAPI/Media ./Media

RUN mkdir -p /app/Media

ENV ASPNETCORE_URLS=http://+:5214

EXPOSE 5214

ENTRYPOINT ["dotnet", "WhatsAppAPISolutionAPI.dll"]
