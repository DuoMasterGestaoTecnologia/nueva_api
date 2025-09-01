# OmniSuite Tests

Este projeto contém os testes unitários para a aplicação OmniSuite, seguindo as melhores práticas do mercado e implementando uma cobertura abrangente de todas as camadas da aplicação.

## 🏗️ Estrutura do Projeto

```
OmniSuite.Tests/
├── Common/                          # Classes base e utilitários comuns
│   ├── TestBase.cs                 # Classe base para todos os testes
│   ├── InMemoryDatabaseTestBase.cs # Classe base para testes com banco em memória
│   └── Factories/                  # Factories para criação de dados de teste
│       ├── UserFactory.cs          # Factory para usuários
│       └── CommandFactory.cs       # Factory para comandos
├── Application/                     # Testes da camada de aplicação
│   ├── Pipeline/                   # Testes do pipeline de validação
│   │   └── ValidationBehaviorTests.cs
│   ├── Authentication/             # Testes de autenticação
│   │   ├── AuthenticationHandlerTests.cs
│   │   └── Validations/
│   │       └── LoginValidationTests.cs
│   └── User/                       # Testes de usuário
│       └── UserQueryHandlerTests.cs
├── Domain/                         # Testes da camada de domínio
│   ├── Entities/                   # Testes das entidades
│   │   └── UserTests.cs
│   ├── Enums/                      # Testes dos enums
│   │   └── UserStatusEnumTests.cs
│   └── Utils/                      # Testes dos utilitários
│       └── UserClaimsHelperTests.cs
├── API/                            # Testes da camada de API
│   └── Controllers/                # Testes dos controladores
│       └── UserControllerTests.cs
└── Infrastructure/                 # Testes da camada de infraestrutura
```

## 🛠️ Frameworks e Ferramentas

- **xUnit**: Framework de testes padrão da Microsoft
- **Moq**: Framework para criação de mocks e stubs
- **FluentAssertions**: Biblioteca para assertions mais legíveis e expressivas
- **AutoFixture**: Geração automática de dados de teste
- **Entity Framework In-Memory**: Banco de dados em memória para testes
- **Coverlet**: Coleta de cobertura de código
- **ReportGenerator**: Geração de relatórios HTML de cobertura

## 📊 Cobertura de Testes Implementada

### ✅ **Camada de Aplicação (Application)**
- **ValidationBehavior**: Pipeline de validação MediatR
- **AuthenticationHandler**: Lógica de autenticação (Login, RefreshToken, Logout)
- **UserQueryHandler**: Operações de usuário (CRUD, MFA, consultas)
- **LoginValidation**: Validações de login com regras de negócio

### ✅ **Camada de Domínio (Domain)**
- **User Entity**: Entidade principal com todas as propriedades
- **UserStatusEnum**: Enum de status com valores e operações
- **UserClaimsHelper**: Utilitário para extração de claims de usuário

### ✅ **Camada de API (Controllers)**
- **UserController**: Todos os endpoints e métodos públicos
- **Validação de atributos**: Route, Authorize, herança
- **Tipos de retorno**: Verificação de tipos de resposta

### ✅ **Infraestrutura de Testes**
- **TestBase**: Classe base com AutoFixture e mocks
- **InMemoryDatabaseTestBase**: Banco em memória para testes de integração
- **Factories**: Geração consistente de dados de teste
- **Global Usings**: Redução de boilerplate

## 🎯 Melhores Práticas Implementadas

### **Convenção AAA (Arrange, Act, Assert)**
```csharp
[Fact]
public async Task Handle_LoginCommand_WithValidCredentials_ShouldReturnAuthenticationResponse()
{
    // Arrange
    var user = UserFactory.CreateValidUser();
    await SaveEntityAsync(user);
    var command = CommandFactory.CreateValidLoginCommand(user.Email, "password");
    
    // Act
    var result = await _handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Data.Should().NotBeNull();
}
```

### **Nomenclatura Descritiva**
- Formato: `MethodName_Scenario_ExpectedBehavior`
- Exemplos:
  - `Handle_LoginCommand_WithValidCredentials_ShouldReturnAuthenticationResponse`
  - `ValidateAsync_WithInvalidEmail_ShouldFailValidation`
  - `GetUserId_WhenUserIsAuthenticated_ShouldReturnUserId`

### **Isolamento e Independência**
- Cada teste é completamente independente
- Banco de dados único por teste (`Guid.NewGuid()`)
- Setup e cleanup automáticos
- Mocks apropriados para dependências externas

## 🚀 Executando os Testes

### **Via Visual Studio**
1. Abra o Test Explorer (Test > Test Explorer)
2. Execute todos os testes ou testes específicos
3. Visualize resultados e cobertura em tempo real

### **Via Linha de Comando**
```bash
# Executar todos os testes
dotnet test

# Executar com detalhes
dotnet test --verbosity normal

# Executar com cobertura de código
dotnet test --collect:"XPlat Code Coverage"

# Executar testes específicos
dotnet test --filter "FullyQualifiedName~UserControllerTests"

# Executar testes de uma categoria específica
dotnet test --filter "Category=Unit"
```

### **Via Docker**
```bash
# Construir e executar testes
docker build -f OmniSuite.Tests/Dockerfile -t omnisuite-tests .

# Executar testes em container
docker run --rm omnisuite-tests

# Executar com volume para desenvolvimento
docker run --rm -v ${PWD}:/app -w /app mcr.microsoft.com/dotnet/sdk:8.0 dotnet test
```

## 📈 Cobertura de Código

### **Configuração Coverlet**
- Formato de saída: OpenCover
- Exclusões: Program, Startup, Migrations, Test files
- Inclusões: Application, Domain, Infrastructure, Persistence
- Relatórios HTML com ReportGenerator

### **Gerar Relatório de Cobertura**
```bash
# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage

# Gerar relatório HTML
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:./coverage/**/coverage.opencover.xml -targetdir:./coverage/report -reporttypes:Html
```

### **Visualizar Cobertura**
- Abra `./coverage/report/index.html` no navegador
- Análise detalhada por classe e método
- Identificação de código não testado
- Métricas de qualidade e confiabilidade

## 🔧 Configurações Avançadas

### **xUnit Runner Configuration**
```json
{
  "parallelizeTestCollections": true,
  "maxParallelThreads": 4,
  "diagnosticMessages": true,
  "stopOnFail": false,
  "methodDisplay": "method"
}
```

### **EditorConfig**
- Codificação UTF-8
- Fim de linha CRLF (Windows)
- Indentação: 4 espaços
- Convenções de nomenclatura para campos privados

### **Global Usings**
- Redução de `using` statements
- Namespaces comuns disponíveis globalmente
- Melhoria na legibilidade dos testes

## 🚀 CI/CD Integration

### **GitHub Actions**
- Execução automática em push/PR
- Setup .NET 8.0
- Execução de testes com cobertura
- Upload para Codecov
- Geração de relatórios HTML
- Artefatos para análise

### **Workflow de Testes**
```yaml
name: Run Tests
on: [push, pull_request]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - setup-dotnet@v4
      - restore dependencies
      - build
      - test with coverage
      - upload to Codecov
      - generate HTML report
```

## 📋 Padrões de Teste

### **1. Testes de Unidade**
- Testam uma única funcionalidade isoladamente
- Usam mocks para dependências externas
- São rápidos e determinísticos
- Cobertura de cenários positivos e negativos

### **2. Testes de Integração**
- Testam a interação entre componentes
- Usam banco de dados em memória
- Testam fluxos completos
- Validação de persistência e recuperação

### **3. Testes de Validação**
- Verificam regras de negócio
- Testam cenários de erro
- Validam mensagens de erro
- Cobertura de edge cases

### **4. Testes de Controllers**
- Verificam tipos de retorno
- Validam atributos e herança
- Testam estrutura da API
- Garantem conformidade com contratos

## 🏭 Factories e Dados de Teste

### **UserFactory**
```csharp
// Usuário válido padrão
var user = UserFactory.CreateValidUser();

// Usuário com configurações específicas
var user = UserFactory.CreateValidUser(
    name: "John Doe",
    email: "john@example.com",
    status: UserStatusEnum.active
);

// Usuário com MFA habilitado
var mfaUser = UserFactory.CreateUserWithMFA();

// Múltiplos usuários
var users = UserFactory.CreateMultipleUsers(5);
```

### **CommandFactory**
```csharp
// Comando de login válido
var loginCommand = CommandFactory.CreateValidLoginCommand();

// Comando com dados específicos
var loginCommand = CommandFactory.CreateValidLoginCommand(
    email: "user@example.com",
    password: "SecurePass123!"
);

// Comando de atualização de foto
var photoCommand = CommandFactory.CreateValidUpdatePhotoCommand();
```

## 🔍 Troubleshooting

### **Problemas Comuns**

1. **Testes falhando aleatoriamente**
   - Verifique se os testes são independentes
   - Use `Guid.NewGuid()` para nomes únicos de banco
   - Implemente `IDisposable` corretamente

2. **Mocks não funcionando**
   - Verifique se a interface está sendo mockada corretamente
   - Use `Verify()` para confirmar chamadas
   - Configure mocks de forma específica

3. **Problemas de banco de dados**
   - Use `SetupDatabase()` no construtor
   - Use `CleanupDatabase()` no `Dispose()`
   - Verifique se o contexto está sendo criado corretamente

### **Logs de Debug**
```bash
# Logs detalhados
dotnet test --logger "console;verbosity=detailed"

# Logs específicos do xUnit
dotnet test --verbosity normal

# Execução com diagnóstico
dotnet test --diagnostic
```

## 📚 Exemplos de Testes

### **Teste de Handler com Banco em Memória**
```csharp
public class AuthenticationHandlerTests : InMemoryDatabaseTestBase
{
    [Fact]
    public async Task Handle_LoginCommand_WithValidCredentials_ShouldReturnAuthenticationResponse()
    {
        // Arrange
        var user = UserFactory.CreateValidUser();
        await SaveEntityAsync(user);
        
        var command = CommandFactory.CreateValidLoginCommand(user.Email, "password");
        _mockTokenService.Setup(x => x.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                       .Returns("valid_token");
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }
}
```

### **Teste de Validação**
```csharp
[Theory]
[InlineData("")]
[InlineData(null)]
[InlineData("invalid-email")]
public async Task ValidateAsync_WithInvalidEmailFormats_ShouldFailValidation(string email)
{
    // Arrange
    var command = new LoginCommand(email, "TestPassword123!");
    
    // Act
    var result = await _validator.ValidateAsync(command);
    
    // Assert
    result.Should().NotBeNull();
    result.IsValid.Should().BeFalse();
    result.Errors.Should().HaveCount(1);
}
```

## 🎯 Boas Práticas

### **1. Isolamento**
- Cada teste deve ser independente
- Use `SetupDatabase()` e `CleanupDatabase()` para limpeza
- Evite dependências entre testes
- Use `Guid.NewGuid()` para nomes únicos

### **2. Nomes Descritivos**
- Use nomes que descrevem o cenário e resultado esperado
- Evite nomes genéricos como "Test1" ou "ShouldWork"
- Siga o padrão `MethodName_Scenario_ExpectedBehavior`

### **3. Assertions Múltiplas**
- Use `FluentAssertions` para assertions mais legíveis
- Agrupe assertions relacionadas
- Verifique tanto o resultado quanto o estado
- Use assertions específicas para tipos de dados

### **4. Mocks**
- Use mocks apenas para dependências externas
- Configure mocks de forma específica
- Verifique se os mocks foram chamados corretamente
- Use `Verify()` para confirmar comportamento

### **5. Dados de Teste**
- Use factories para criar dados consistentes
- Evite dados hardcoded nos testes
- Use dados realistas mas não reais
- Configure dados específicos quando necessário

## 📈 Métricas e Qualidade

### **Cobertura Alvo**
- **Mínima**: 80% de cobertura de código
- **Recomendada**: 90%+ para código crítico
- **Foco**: Application, Domain, Infrastructure, Persistence

### **Qualidade dos Testes**
- **Naming**: 100% seguindo convenções
- **Structure**: Padrão AAA em todos os testes
- **Isolation**: Testes independentes
- **Maintainability**: Fácil manutenção e extensão

## 🤝 Contribuindo

### **Guidelines**
1. Mantenha a cobertura de testes acima de 80%
2. Adicione testes para novas funcionalidades
3. Siga as convenções estabelecidas
4. Execute todos os testes antes de fazer commit
5. Use as factories existentes quando possível

### **Processo**
1. Crie testes para nova funcionalidade
2. Execute testes existentes para garantir não quebrar
3. Execute novos testes para validar implementação
4. Verifique cobertura de código
5. Faça commit com testes passando

## 🔗 Recursos Adicionais

- [Documentação do xUnit](https://xunit.net/)
- [Documentação do Moq](https://github.com/moq/moq4)
- [Documentação do FluentAssertions](https://fluentassertions.com/)
- [Documentação do AutoFixture](https://github.com/AutoFixture/AutoFixture)
- [Documentação do Coverlet](https://github.com/coverlet-coverage/coverlet)
- [Documentação do ReportGenerator](https://github.com/danielpalme/ReportGenerator)

## 📊 Status do Projeto

- ✅ **Projeto de Testes**: Criado e configurado
- ✅ **Frameworks**: xUnit, Moq, FluentAssertions, AutoFixture
- ✅ **Infraestrutura**: TestBase, InMemoryDatabaseTestBase, Factories
- ✅ **Testes Implementados**: Application, Domain, API
- ✅ **CI/CD**: GitHub Actions configurado
- ✅ **Docker**: Containerização para testes
- ✅ **Cobertura**: Coverlet + ReportGenerator
- ✅ **Documentação**: README completo e detalhado

---

**OmniSuite Tests** - Qualidade e confiabilidade garantidas através de testes abrangentes e bem estruturados.
