#!/bin/bash
echo "UPDATING DATABASE"
dotnet restore "./SmartLogisticsBackend.csproj"
dotnet-ef database update
dotnet watch run --no-launch-profile