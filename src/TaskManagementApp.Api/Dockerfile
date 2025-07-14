# Define a imagem base
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TaskManagementApp.sln .

COPY src/TaskManagementApp.Api/ TaskManagementApp.Api/
COPY src/TaskManagementApp.Application/ TaskManagementApp.Application/
COPY src/TaskManagementApp.Models/ TaskManagementApp.Models/
COPY src/TaskManagementApp.ExternalService/ TaskManagementApp.ExternalService/
COPY src/TaskManagementApp.Data/ TaskManagementApp.Data/
COPY src/TaskManagementApp.Domain/ TaskManagementApp.Domain/
COPY test/TaskManagementApp.Tests/ test/TaskManagementApp.Tests/

WORKDIR /src/TaskManagementApp.Api
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Estágio final (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Configura certificado HTTPS de dev
ENV ASPNETCORE_URLS=https://+:443;http://+:80
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=devpassword

# Copia o certificado para o container
COPY https/aspnetapp.pfx https/aspnetapp.pfx

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "TaskManagementApp.Api.dll"]