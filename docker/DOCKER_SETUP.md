# Configuração Docker - Nueva API

Este guia explica como configurar e executar o banco de dados usando Docker para o projeto Nueva API.

## 📋 Pré-requisitos

- Docker Desktop instalado e rodando
- .NET 8.0 SDK instalado
- Entity Framework Core CLI instalado

## 🚀 Início Rápido

### 1. Iniciar os Containers

```bash
# Windows PowerShell
.\docker\docker-scripts.ps1

# Linux/Mac
./docker/docker-scripts.sh
```

Ou manualmente:

```bash
docker-compose up -d
```

### 2. Executar Migrations

#### Para MySQL (padrão atual):
```bash
dotnet ef database update --project OmniSuite.Persistence --startup-project OmniSuite.API --connection "Server=localhost;Database=nueva;User Id=root;Password=Senha@123;"
```

#### Para PostgreSQL:
```bash
dotnet ef database update --project OmniSuite.Persistence --startup-project OmniSuite.API --connection "Host=localhost;Database=nueva;Username=postgres;Password=Senha@123;"
```

## 🐳 Serviços Disponíveis

| Serviço | Porta | Descrição |
|---------|-------|-----------|
| PostgreSQL | 5432 | Banco de dados principal |
| MySQL | 3306 | Banco de dados alternativo |
| pgAdmin | 8080 | Interface web para PostgreSQL |

### Credenciais de Acesso

#### PostgreSQL
- **Host:** localhost
- **Porta:** 5432
- **Database:** nueva
- **Usuário:** postgres
- **Senha:** Senha@123

#### MySQL
- **Host:** localhost
- **Porta:** 3306
- **Database:** nueva
- **Usuário:** root
- **Senha:** Senha@123

#### pgAdmin
- **URL:** http://localhost:8080
- **Email:** admin@nueva.com
- **Senha:** admin123

## 🔧 Comandos Úteis

### Gerenciar Containers
```bash
# Iniciar containers
docker-compose up -d

# Parar containers
docker-compose down

# Ver status
docker-compose ps

# Ver logs
docker-compose logs -f [serviço]

# Resetar banco (remove dados)
docker-compose down -v
docker-compose up -d
```

### Migrations
```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration --project OmniSuite.Persistence --startup-project OmniSuite.API

# Aplicar migrations
dotnet ef database update --project OmniSuite.Persistence --startup-project OmniSuite.API

# Remover última migration
dotnet ef migrations remove --project OmniSuite.Persistence --startup-project OmniSuite.API
```

## ⚙️ Configuração da Aplicação

### Connection Strings

O projeto está configurado com múltiplas connection strings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=nueva;User Id=root;Password=Senha@123;",
    "DockerConnection": "Server=localhost;Database=nueva;User Id=root;Password=Senha@123;",
    "PostgresConnection": "Host=localhost;Database=nueva;Username=postgres;Password=Senha@123;"
  }
}
```

### Ambiente Docker

Para usar as configurações específicas do Docker, defina a variável de ambiente:

```bash
# Windows
$env:ASPNETCORE_ENVIRONMENT = "Docker"

# Linux/Mac
export ASPNETCORE_ENVIRONMENT=Docker
```

## 🔄 Migração de MySQL para PostgreSQL

Se desejar migrar de MySQL para PostgreSQL:

1. **Atualizar o projeto Persistence:**
   ```bash
   # Remover referência do MySQL
   dotnet remove package Pomelo.EntityFrameworkCore.MySql
   
   # Adicionar referência do PostgreSQL
   dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
   ```

2. **Atualizar ApplicationDbContext:**
   ```csharp
   // No Program.cs ou DependencyInjection.cs
   services.AddDbContext<ApplicationDbContext>(options =>
       options.UseNpgsql(connectionString));
   ```

3. **Recriar migrations:**
   ```bash
   # Remover pasta Migrations
   rm -rf OmniSuite.Persistence/Migrations
   
   # Criar nova migration inicial
   dotnet ef migrations add InitialCreate --project OmniSuite.Persistence --startup-project OmniSuite.API
   ```

## 🐛 Troubleshooting

### Container não inicia
```bash
# Verificar logs
docker-compose logs

# Verificar se as portas estão em uso
netstat -an | findstr :5432
netstat -an | findstr :3306
```

### Erro de conexão
- Verifique se os containers estão rodando: `docker-compose ps`
- Verifique se as portas estão abertas
- Verifique as credenciais no arquivo de configuração

### Erro de migration
- Verifique se o banco de dados está acessível
- Verifique se a connection string está correta
- Verifique se o Entity Framework CLI está instalado

## 📁 Estrutura de Arquivos

```
nueva_api/
├── docker-compose.yml          # Configuração dos containers
├── docker/docker-scripts.ps1   # Scripts PowerShell
├── docker/docker-scripts.sh    # Scripts Bash
├── docker.env                  # Variáveis de ambiente
├── DOCKER_SETUP.md            # Este arquivo
└── OmniSuite.API/
    ├── appsettings.Docker.json # Configuração para Docker
    └── appsettings.json        # Configuração padrão
```

## 🆘 Suporte

Se encontrar problemas:

1. Verifique os logs dos containers
2. Verifique se todas as dependências estão instaladas
3. Verifique se as portas não estão em uso
4. Consulte a documentação do Docker e Entity Framework
