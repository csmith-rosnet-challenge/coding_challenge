FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj and restore first for better caching
COPY ["Backend.csproj", "./"]
RUN dotnet restore "./Backend.csproj"

# copy rest and publish
COPY . .
RUN dotnet publish "Backend.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5000
COPY --from=build /app/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "Backend.dll"]
