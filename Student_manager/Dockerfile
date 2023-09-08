#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Student_manager/Student_manager.csproj", "Student_manager/"]
RUN dotnet restore "Student_manager/Student_manager.csproj"
COPY . .
WORKDIR "/src/Student_manager"
RUN dotnet build "Student_manager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Student_manager.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Student_manager.dll"]