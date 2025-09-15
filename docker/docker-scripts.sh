#!/bin/bash

# Scripts para gerenciar Docker e Migrations

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
WHITE='\033[1;37m'
NC='\033[0m' # No Color

# Função para iniciar os containers
start_containers() {
    echo -e "${GREEN}Iniciando containers do Docker...${NC}"
    docker-compose up -d
    echo -e "${GREEN}Containers iniciados com sucesso!${NC}"
    echo -e "${YELLOW}PostgreSQL disponível em: localhost:5432${NC}"
    echo -e "${YELLOW}MySQL disponível em: localhost:3306${NC}"
    echo -e "${YELLOW}pgAdmin disponível em: http://localhost:8080${NC}"
}

# Função para parar os containers
stop_containers() {
    echo -e "${RED}Parando containers do Docker...${NC}"
    docker-compose down
    echo -e "${RED}Containers parados!${NC}"
}

# Função para executar migrations no MySQL
run_mysql_migrations() {
    echo -e "${GREEN}Executando migrations no MySQL...${NC}"
    export ASPNETCORE_ENVIRONMENT=Docker
    dotnet ef database update --project OmniSuite.Persistence --startup-project OmniSuite.API --connection "Server=localhost;Database=nueva;User Id=root;Password=Senha@123;"
    echo -e "${GREEN}Migrations executadas com sucesso!${NC}"
}

# Função para executar migrations no PostgreSQL
run_postgresql_migrations() {
    echo -e "${GREEN}Executando migrations no PostgreSQL...${NC}"
    export ASPNETCORE_ENVIRONMENT=Docker
    dotnet ef database update --project OmniSuite.Persistence --startup-project OmniSuite.API --connection "Host=localhost;Database=nueva;Username=postgres;Password=Senha@123;"
    echo -e "${GREEN}Migrations executadas com sucesso!${NC}"
}

# Função para verificar status dos containers
check_status() {
    echo -e "${CYAN}Status dos containers:${NC}"
    docker-compose ps
}

# Função para ver logs dos containers
show_logs() {
    local service=${1:-all}
    
    if [ "$service" = "all" ]; then
        docker-compose logs -f
    else
        docker-compose logs -f "$service"
    fi
}

# Função para resetar o banco de dados
reset_database() {
    echo -e "${RED}Resetando banco de dados...${NC}"
    docker-compose down -v
    docker-compose up -d
    sleep 10
    echo -e "${GREEN}Banco de dados resetado!${NC}"
}

# Menu principal
show_menu() {
    echo -e "\n${CYAN}=== Gerenciador Docker - Nueva API ===${NC}"
    echo -e "${WHITE}1. Iniciar containers${NC}"
    echo -e "${WHITE}2. Parar containers${NC}"
    echo -e "${WHITE}3. Status dos containers${NC}"
    echo -e "${WHITE}4. Executar migrations (MySQL)${NC}"
    echo -e "${WHITE}5. Executar migrations (PostgreSQL)${NC}"
    echo -e "${WHITE}6. Ver logs${NC}"
    echo -e "${WHITE}7. Resetar banco de dados${NC}"
    echo -e "${WHITE}8. Sair${NC}"
    echo -e "\nEscolha uma opção: "
}

# Loop principal
while true; do
    show_menu
    read -r choice
    
    case $choice in
        1) start_containers ;;
        2) stop_containers ;;
        3) check_status ;;
        4) run_mysql_migrations ;;
        5) run_postgresql_migrations ;;
        6) 
            echo -e "${YELLOW}Escolha o serviço (postgres, mysql, pgadmin ou all): ${NC}"
            read -r service
            show_logs "$service"
            ;;
        7) reset_database ;;
        8) echo -e "${GREEN}Saindo...${NC}"; break ;;
        *) echo -e "${RED}Opção inválida!${NC}" ;;
    esac
    
    if [ "$choice" != "8" ]; then
        echo -e "\nPressione Enter para continuar..."
        read -r
    fi
done
