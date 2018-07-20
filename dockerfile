FROM microsoft/dotnet:2.1.300-sdk AS build
WORKDIR /ConsoleApp1

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:aspnetcore-runtime AS runtime
WORKDIR /ConsoleApp1
COPY --from=build /ConsoleApp1/out .
ENTRYPOINT ["dotnet", "./ConsoleApp1.dll"]