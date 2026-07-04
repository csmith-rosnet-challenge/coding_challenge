#!/usr/bin/env pwsh
dotnet restore "../TodoApi.csproj"
dotnet build "../TodoApi.csproj" -c Release
