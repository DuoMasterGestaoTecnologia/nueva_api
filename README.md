# 🚀 OmniSuite API

Uma API robusta e escalável construída com **Clean Architecture** e **.NET 8**, implementando padrões modernos de desenvolvimento de software.

## 🏗️ Arquitetura

Este projeto utiliza a **Clean Architecture (Arquitetura Limpa)** com separação clara de responsabilidades em camadas bem definidas.

### 📁 Estrutura do Projeto

```
nueva_api/
├── OmniSuite.API/           # 🌐 Camada de Apresentação
├── OmniSuite.Application/    # 🔧 Camada de Aplicação
├── OmniSuite.Domain/         # 🎯 Camada de Domínio
├── OmniSuite.Infrastructure/ # 🏗️ Camada de Infraestrutura
└── OmniSuite.Persistence/    # 💾 Camada de Persistência
```

### 🎯 Camadas e Responsabilidades

#### **🌐 OmniSuite.API (Camada de Apresentação)**
- **Controllers** - Endpoints da API REST
- **Middlewares** - Validação JWT, tratamento de exceções
- **Program.cs** - Configuração da aplicação
- **appsettings.json** - Configurações da aplicação

#### **🔧 OmniSuite.Application (Camada de Aplicação)**
- **Commands** - Comandos CQRS (Create, Update, Delete)
- **Queries** - Consultas CQRS (Read)
- **Handlers** - Manipuladores de comandos e consultas
- **Validations** - Validações de entrada
- **Responses** - DTOs de resposta
- **Pipeline** - Comportamentos de pipeline (validação)

#### **🎯 OmniSuite.Domain (Camada de Domínio)**
- **Entities** - Entidades de negócio
- **Enums** - Enumerações do domínio
- **Interfaces** - Contratos de serviços
- **Utils** - Utilitários do domínio

#### **🏗️ OmniSuite.Infrastructure (Camada de Infraestrutura)**
- **Services** - Implementações de serviços externos
  - FlowpagService (integração com gateway de pagamento)
  - MfaService (autenticação de dois fatores)
  - TokenService (geração de JWT)
  - SmtpEmailService (envio de emails)
- **KeyGenerator** - Geração de chaves
- **Security** - Segurança de arquivos

#### **💾 OmniSuite.Persistence (Camada de Persistência)**
- **ApplicationDbContext** - Contexto do Entity Framework
- **Migrations** - Migrações do banco de dados

## 🎭 Padrões Arquiteturais

### **📋 CQRS (Command Query Responsibility Segregation)**
```
Commands/          # Modificam o estado
├── CreateUserCommand.cs
├── UpdateUserCommand.cs
└── DeleteUserCommand.cs

Queries/           # Consultam dados
├── GetUserQuery.cs
├── UserByEmailQuery.cs
└── UsersPendingQuery.cs
```

### **🎯 Mediator Pattern**
- **Handlers** implementam o padrão mediator
- Separação clara entre comandos e consultas
- Processamento através de pipelines

### **🏗️ Repository Pattern**
- Entity Framework como ORM
- DbContext como repositório genérico
- Entidades mapeadas para tabelas

### **🔐 JWT Authentication**
- Middleware de validação JWT
- TokenService para geração de tokens
- Refresh tokens implementados

## 🛠️ Tecnologias e Frameworks

- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM
- **MySQL** - Banco de dados
- **JWT** - Autenticação
- **CQRS** - Padrão de separação de responsabilidades
- **Dependency Injection** - Injeção de dependências nativa

## 📊 Banco de Dados

### **Configuração**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=nueva;User Id=root;Password=Senha@123;"
  }
}
```

### **Tabelas Principais**
- `Users` - Usuários do sistema
- `UserTokens` - Tokens de autenticação
- `Deposits` - Depósitos
- `UserBalances` - Saldos dos usuários
- `Withdraw` - Saques
- `Affiliates` - Afiliados
- `AffiliatesCommission` - Comissões de afiliados
- `ActiveTransactions` - Transações ativas

## 🚀 Como Executar

### **Pré-requisitos**
- .NET 8 SDK
- MySQL Server
- Entity Framework CLI

### **1. Clone o repositório**
```bash
git clone <url-do-repositorio>
cd nueva_api
```

### **2. Restaure as dependências**
```bash
dotnet restore
```

### **3. Configure o banco de dados**
- Certifique-se de que o MySQL está rodando
- Verifique as configurações em `OmniSuite.API/appsettings.json`
- Execute as migrations:
```bash
cd OmniSuite.API
dotnet ef database update
```

### **4. Execute a aplicação**
```bash
dotnet run
```

A API estará disponível em:
- **HTTP:** http://localhost:5114
- **HTTPS:** https://localhost:7248
- **Swagger:** http://localhost:5114/swagger

## 🔧 Comandos Úteis

### **Entity Framework**
```bash
# Listar migrations
dotnet ef migrations list

# Aplicar migrations
dotnet ef database update

# Criar nova migration
dotnet ef migrations add NomeDaMigration

# Remover última migration
dotnet ef migrations remove
```

### **Build e Testes**
```bash
# Build do projeto
dotnet build

# Executar testes
dotnet test

# Limpar build
dotnet clean
```

## 🏆 Vantagens da Arquitetura

✅ **Separação de Responsabilidades** - Cada camada tem função específica  
✅ **Testabilidade** - Fácil de testar cada camada isoladamente  
✅ **Manutenibilidade** - Código organizado e fácil de manter  
✅ **Escalabilidade** - Fácil de expandir e modificar  
✅ **Independência de Frameworks** - Domínio não depende de tecnologias externas  
✅ **Flexibilidade** - Fácil trocar implementações (ex: banco de dados)  

## 📋 Funcionalidades Principais

- **🔐 Autenticação e Autorização** - JWT com refresh tokens
- **👥 Gestão de Usuários** - CRUD completo de usuários
- **💰 Sistema de Depósitos** - Processamento de pagamentos
- **💸 Sistema de Saques** - Transferências PIX
- **🤝 Sistema de Afiliados** - Programa de comissões
- **🔒 MFA** - Autenticação de dois fatores
- **📧 Notificações** - Envio de emails

## 🧪 Testes

O projeto está preparado para implementação de testes unitários e de integração, seguindo as melhores práticas de TDD (Test-Driven Development).

## 📚 Documentação da API

A documentação completa da API está disponível através do Swagger UI quando a aplicação estiver rodando.

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença [MIT](LICENSE).

## 👨‍💻 Desenvolvedores

- **Rafael** - Desenvolvedor Principal

## 📞 Suporte

Para suporte ou dúvidas, entre em contato através dos canais disponibilizados pela equipe.

---

**⭐ Se este projeto foi útil para você, considere dar uma estrela!**