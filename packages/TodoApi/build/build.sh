#!/usr/bin/env bash
dotnet restore ../TodoApi.csproj
dotnet build ../TodoApi.csproj -c Release
