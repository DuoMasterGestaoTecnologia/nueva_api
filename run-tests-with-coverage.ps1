# PowerShell script to run tests with coverage locally
# This mimics what the GitHub Actions workflow does

Write-Host "Running tests with coverage..." -ForegroundColor Green

# Clean previous coverage results
if (Test-Path "./coverage") {
    Remove-Item -Recurse -Force "./coverage"
}
if (Test-Path "./TestResults") {
    Remove-Item -Recurse -Force "./TestResults"
}

# Restore dependencies
Write-Host "Restoring dependencies..." -ForegroundColor Yellow
dotnet restore

# Build solution
Write-Host "Building solution..." -ForegroundColor Yellow
dotnet build --configuration Release

# Run tests with coverage
Write-Host "Running tests with coverage..." -ForegroundColor Yellow
dotnet test `
    --configuration Release `
    --collect:"XPlat Code Coverage" `
    --results-directory ./coverage `
    --settings ./OmniSuite.Tests/coverlet.runsettings `
    --logger trx `
    --logger "console;verbosity=detailed"

# Check if coverage files were generated
$coverageFiles = Get-ChildItem -Path "./coverage" -Recurse -Filter "coverage.opencover.xml"
if ($coverageFiles.Count -eq 0) {
    Write-Host "No coverage files found! Check the test execution." -ForegroundColor Red
    exit 1
}

Write-Host "Found $($coverageFiles.Count) coverage file(s):" -ForegroundColor Green
foreach ($file in $coverageFiles) {
    Write-Host "  - $($file.FullName)" -ForegroundColor Cyan
}

# Install ReportGenerator if not already installed
Write-Host "Installing ReportGenerator tool..." -ForegroundColor Yellow
dotnet tool install -g dotnet-reportgenerator-globaltool --version 5.4.12

# Generate coverage report
Write-Host "Generating coverage report..." -ForegroundColor Yellow
reportgenerator `
    -reports:"./coverage/**/coverage.opencover.xml" `
    -targetdir:"./coverage/report" `
    -reporttypes:"Html;HtmlSummary" `
    -assemblyfilters:"-*.Tests*" `
    -classfilters:"-*.Program;-*.Startup;-*.Migrations.*"

# Check if report was generated
if (Test-Path "./coverage/report/index.html") {
    Write-Host "Coverage report generated successfully!" -ForegroundColor Green
    Write-Host "Open ./coverage/report/index.html in your browser to view the report." -ForegroundColor Cyan
} else {
    Write-Host "Failed to generate coverage report!" -ForegroundColor Red
    exit 1
}

Write-Host "Done!" -ForegroundColor Green
