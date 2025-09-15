# Scripts para gerenciar Docker e Migrations

# Função para iniciar os containers
function Start-DockerContainers {
    Write-Host "Iniciando containers do Docker..." -ForegroundColor Green
    docker-compose up -d
    Write-Host "Containers iniciados com sucesso!" -ForegroundColor Green
    Write-Host "PostgreSQL disponível em: localhost:5432" -ForegroundColor Yellow
    Write-Host "MySQL disponível em: localhost:3306" -ForegroundColor Yellow
    Write-Host "pgAdmin disponível em: http://localhost:8080" -ForegroundColor Yellow
}

# Função para parar os containers
function Stop-DockerContainers {
    Write-Host "Parando containers do Docker..." -ForegroundColor Red
    docker-compose down
    Write-Host "Containers parados!" -ForegroundColor Red
}

# Função para executar migrations no MySQL
function Run-MySQLMigrations {
    Write-Host "Executando migrations no MySQL..." -ForegroundColor Green
    $env:ASPNETCORE_ENVIRONMENT = "Docker"
    dotnet ef database update --project OmniSuite.Persistence --startup-project OmniSuite.API --connection "Server=localhost;Database=nueva;User Id=root;Password=Senha@123;"
    Write-Host "Migrations executadas com sucesso!" -ForegroundColor Green
}

# Função para executar migrations no PostgreSQL
function Run-PostgreSQLMigrations {
    Write-Host "Executando migrations no PostgreSQL..." -ForegroundColor Green
    $env:ASPNETCORE_ENVIRONMENT = "Docker"
    dotnet ef database update --project OmniSuite.Persistence --startup-project OmniSuite.API --connection "Host=localhost;Database=nueva;Username=postgres;Password=Senha@123;"
    Write-Host "Migrations executadas com sucesso!" -ForegroundColor Green
}

# Função para verificar status dos containers
function Get-ContainerStatus {
    Write-Host "Status dos containers:" -ForegroundColor Cyan
    docker-compose ps
}

# Função para ver logs dos containers
function Show-ContainerLogs {
    param(
        [string]$Service = "all"
    )
    
    if ($Service -eq "all") {
        docker-compose logs -f
    } else {
        docker-compose logs -f $Service
    }
}

# Função para resetar o banco de dados
function Reset-Database {
    Write-Host "Resetando banco de dados..." -ForegroundColor Red
    docker-compose down -v
    docker-compose up -d
    Start-Sleep -Seconds 10
    Write-Host "Banco de dados resetado!" -ForegroundColor Green
}

# Menu principal
function Show-Menu {
    Write-Host "`n=== Gerenciador Docker - Nueva API ===" -ForegroundColor Cyan
    Write-Host "1. Iniciar containers" -ForegroundColor White
    Write-Host "2. Parar containers" -ForegroundColor White
    Write-Host "3. Status dos containers" -ForegroundColor White
    Write-Host "4. Executar migrations (MySQL)" -ForegroundColor White
    Write-Host "5. Executar migrations (PostgreSQL)" -ForegroundColor White
    Write-Host "6. Ver logs" -ForegroundColor White
    Write-Host "7. Resetar banco de dados" -ForegroundColor White
    Write-Host "8. Sair" -ForegroundColor White
    Write-Host "`nEscolha uma opção: " -NoNewline -ForegroundColor Yellow
}

# Loop principal
do {
    Show-Menu
    $choice = Read-Host
    
    switch ($choice) {
        "1" { Start-DockerContainers }
        "2" { Stop-DockerContainers }
        "3" { Get-ContainerStatus }
        "4" { Run-MySQLMigrations }
        "5" { Run-PostgreSQLMigrations }
        "6" { 
            Write-Host "Escolha o serviço (postgres, mysql, pgadmin ou all): " -NoNewline -ForegroundColor Yellow
            $service = Read-Host
            Show-ContainerLogs -Service $service
        }
        "7" { Reset-Database }
        "8" { Write-Host "Saindo..." -ForegroundColor Green; break }
        default { Write-Host "Opção inválida!" -ForegroundColor Red }
    }
    
    if ($choice -ne "8") {
        Write-Host "`nPressione qualquer tecla para continuar..." -ForegroundColor Gray
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    }
} while ($choice -ne "8")
