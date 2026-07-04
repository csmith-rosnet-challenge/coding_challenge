#!/usr/bin/env pwsh
dotnet restore "../Backend.csproj"
dotnet build "../Backend.csproj" -c Release
