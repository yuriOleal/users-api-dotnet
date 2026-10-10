# Users API

API REST para gerenciamento de usuários com autenticação JWT, construída em ASP.NET Core 8.

## Tecnologias

- ASP.NET Core 8
- Entity Framework Core (SQLite)
- JWT Bearer Authentication
- BCrypt (hash de senhas)
- Swagger / OpenAPI

## Arquitetura

```
Api/
├── Controllers/     # Endpoints HTTP
├── Services/        # Regras de negócio
├── DTOs/            # Objetos de transferência (entrada/saída)
├── Models/          # Entidades do domínio
├── Data/            # DbContext e configuração do banco
├── Middleware/      # Tratamento global de erros
└── Program.cs       # Configuração e DI
```

## Funcionalidades

- Registro e login de usuários
- Autenticação via JWT com expiração de 2 horas
- Consulta, atualização e exclusão dos próprios dados (protegidas por token)
- Respostas públicas sem o hash de senha; testes de regressão para isolamento por usuário
- Hash de senhas com BCrypt
- Validação de dados de entrada
- Tratamento global de exceções
- Health check endpoint
- Documentação interativa com Swagger

## Endpoints

### Autenticação

| Método | Rota              | Descrição         | Auth |
|--------|-------------------|-------------------|------|
| POST   | /api/auth/register| Criar conta       | Não  |
| POST   | /api/auth/login   | Obter token JWT   | Não  |

### Usuários (requer token)

| Método | Rota             | Descrição              |
|--------|------------------|------------------------|
| GET    | /api/users       | Listar apenas a própria conta |
| GET    | /api/users/{id}  | Buscar a própria conta |
| PUT    | /api/users/{id}  | Atualizar a própria conta |
| DELETE | /api/users/{id}  | Remover a própria conta |

Novas contas são criadas exclusivamente por `POST /api/auth/register`. Uma conta autenticada não pode consultar, alterar ou remover outra conta.

### Outros

| Método | Rota         | Descrição       |
|--------|--------------|-----------------|
| GET    | /api/health  | Status da API   |

## Como rodar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Configuração

1. Clone o repositório:
```bash
git clone https://github.com/yuriOleal/users-api-dotnet.git
cd users-api-dotnet
```

2. Configure a chave JWT com pelo menos 32 caracteres. Prefira uma variável de ambiente:
```bash
# PowerShell
$env:Jwt__Key="sua-chave-local-com-pelo-menos-32-caracteres"
```

Como alternativa, crie um arquivo `appsettings.Development.json` na pasta `Api/`:
```json
{
  "Jwt": {
    "Key": "sua-chave-secreta-com-pelo-menos-32-caracteres!",
    "Issuer": "UsersApi",
    "Audience": "UsersApi"
  }
}
```

3. Execute:
```bash
cd Api
dotnet run
```

4. Acesse o Swagger:
```
http://localhost:5150/swagger
```

## Testando a API

Rode a suíte automatizada a partir da raiz do repositório:

```bash
dotnet test Api.Tests/Api.Tests.csproj
```

O GitHub Actions executa restauração, build e testes a cada push e pull request.

### 1. Registrar
```bash
curl -X POST http://localhost:5150/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"name": "Yuri", "email": "yuri@email.com", "password": "Senha123!"}'
```

### 2. Login (retorna o token)
```bash
curl -X POST http://localhost:5150/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "yuri@email.com", "password": "Senha123!"}'
```

### 3. Usar o token nas rotas protegidas
```bash
curl http://localhost:5150/api/users \
  -H "Authorization: Bearer {seu_token}"
```

## Estrutura do Banco

O banco SQLite é criado automaticamente na primeira execução via migrations do EF Core.

**Tabela Users:**
| Campo        | Tipo   | Descrição            |
|--------------|--------|----------------------|
| Id           | GUID   | Identificador único  |
| Name         | string | Nome do usuário      |
| Email        | string | Email da conta       |
| PasswordHash | string | Senha hashada        |

## Decisões Técnicas

- **SQLite** para simplicidade — sem necessidade de instalar banco externo
- **BCrypt** para hash de senhas — resistente a ataques de força bruta
- **JWT** com expiração de 2h — padrão para APIs stateless
- **Interface IUserService** — mantém a regra de negócio isolada e facilita a evolução da implementação
- **Migrations automáticas** no startup — banco sempre atualizado

## Segurança no desenvolvimento

- A chave JWT deve ser fornecida por variável de ambiente ou por `appsettings.Development.json` local.
- Nenhuma credencial real deve ser commitada no repositório.
- O SQLite é usado somente como banco local de demonstração; em um ambiente real, a conexão deve ser configurada para o banco adotado pela aplicação.
- Os endpoints de usuário usam o identificador do token JWT para limitar o acesso à própria conta. `PasswordHash` permanece no modelo de persistência e não integra os DTOs de resposta.

## Próximos passos

- [ ] Adicionar índice único de e-mail e tratar conflito de concorrência no registro
- [ ] Implementar refresh token
- [ ] Adicionar logs estruturados (Serilog)
- [ ] Adicionar testes HTTP de ponta a ponta e ampliar os cenários de validação
- [ ] Deploy com Docker
