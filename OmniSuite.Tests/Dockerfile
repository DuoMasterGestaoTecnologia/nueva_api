FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["OmniSuite.Tests/OmniSuite.Tests.csproj", "OmniSuite.Tests/"]
COPY ["OmniSuite.Application/OmniSuite.Application.csproj", "OmniSuite.Application/"]
COPY ["OmniSuite.Domain/OmniSuite.Domain.csproj", "OmniSuite.Domain/"]
COPY ["OmniSuite.Infrastructure/OmniSuite.Infrastructure.csproj", "OmniSuite.Infrastructure/"]
COPY ["OmniSuite.Persistence/OmniSuite.Persistence.csproj", "OmniSuite.Persistence/"]
COPY ["OmniSuite.API/OmniSuite.API.csproj", "OmniSuite.API/"]

RUN dotnet restore "OmniSuite.Tests/OmniSuite.Tests.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/OmniSuite.Tests"
RUN dotnet build "OmniSuite.Tests.csproj" -c Release -o /app/build

# Test stage
FROM build AS test
WORKDIR /src/OmniSuite.Tests

# Install reportgenerator
RUN dotnet tool install -g dotnet-reportgenerator-globaltool

# Run tests with coverage
RUN dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage" --results-directory ./coverage

# Generate coverage report
RUN reportgenerator -reports:./coverage/**/coverage.opencover.xml -targetdir:./coverage/report -reporttypes:Html

# Final stage for running tests
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app

# Copy test project and dependencies
COPY --from=build /src/OmniSuite.Tests ./
COPY --from=build /src/OmniSuite.Application ./
COPY --from=build /src/OmniSuite.Domain ./
COPY --from=build /src/OmniSuite.Infrastructure ./
COPY --from=build /src/OmniSuite.Persistence ./
COPY --from=build /src/OmniSuite.API ./

# Install reportgenerator
RUN dotnet tool install -g dotnet-reportgenerator-globaltool

# Set environment variables
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Default command to run tests
ENTRYPOINT ["dotnet", "test", "--verbosity", "normal"]
