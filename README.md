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
- Autenticação via JWT com expiração configurável
- CRUD completo de usuários (protegido por token)
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
| GET    | /api/users       | Listar todos           |
| GET    | /api/users/{id}  | Buscar por ID          |
| POST   | /api/users       | Criar usuário          |
| PUT    | /api/users/{id}  | Atualizar usuário      |
| DELETE | /api/users/{id}  | Remover usuário        |

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
git clone https://github.com/seu-usuario/users-api.git
cd users-api
```

2. Configure a chave JWT (crie um arquivo `appsettings.Development.json` na pasta `Api/`):
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
| Email        | string | Email (único)        |
| PasswordHash | string | Senha hashada        |

## Decisões Técnicas

- **SQLite** para simplicidade — sem necessidade de instalar banco externo
- **BCrypt** para hash de senhas — resistente a ataques de força bruta
- **JWT** com expiração de 2h — padrão para APIs stateless
- **Interface IUserService** — facilita testes e troca de implementação
- **Migrations automáticas** no startup — banco sempre atualizado

## Próximos passos

- [ ] Adicionar paginação na listagem
- [ ] Implementar refresh token
- [ ] Adicionar logs estruturados (Serilog)
- [ ] Testes unitários e de integração
- [ ] Deploy com Docker
