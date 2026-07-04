#!/usr/bin/env bash
dotnet restore ../Backend.csproj
dotnet build ../Backend.csproj -c Release
