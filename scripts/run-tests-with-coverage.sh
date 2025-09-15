#!/bin/bash

# Bash script to run tests with coverage locally
# This mimics what the GitHub Actions workflow does

echo "Running tests with coverage..."

# Clean previous coverage results
rm -rf ./coverage
rm -rf ./TestResults

# Restore dependencies
echo "Restoring dependencies..."
dotnet restore

# Build solution
echo "Building solution..."
dotnet build --configuration Release

# Run tests with coverage
echo "Running tests with coverage..."
dotnet test \
    --configuration Release \
    --collect:"XPlat Code Coverage" \
    --results-directory ./coverage \
    --settings ./OmniSuite.Tests/coverlet.runsettings \
    --logger trx \
    --logger "console;verbosity=detailed"

# Check if coverage files were generated
coverage_files=$(find ./coverage -name "coverage.opencover.xml" 2>/dev/null)
if [ -z "$coverage_files" ]; then
    echo "No coverage files found! Check the test execution."
    exit 1
fi

echo "Found coverage files:"
echo "$coverage_files"

# Install ReportGenerator if not already installed
echo "Installing ReportGenerator tool..."
dotnet tool install -g dotnet-reportgenerator-globaltool --version 5.4.12

# Generate coverage report
echo "Generating coverage report..."
reportgenerator \
    -reports:"./coverage/**/coverage.opencover.xml" \
    -targetdir:"./coverage/report" \
    -reporttypes:"Html;HtmlSummary" \
    -assemblyfilters:"-*.Tests*" \
    -classfilters:"-*.Program;-*.Startup;-*.Migrations.*"

# Check if report was generated
if [ -f "./coverage/report/index.html" ]; then
    echo "Coverage report generated successfully!"
    echo "Open ./coverage/report/index.html in your browser to view the report."
else
    echo "Failed to generate coverage report!"
    exit 1
fi

echo "Done!"
