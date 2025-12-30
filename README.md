# Users API - ASP.NET Core

API REST simples para gerenciamento de usuários, construída com ASP.NET Core seguindo boas práticas de arquitetura.

## 🚀 Tecnologias
- ASP.NET Core
- Swagger (OpenAPI)
- Git & GitHub

## 📁 Arquitetura
- Controllers → Entrada HTTP
- Services → Regras de negócio
- DTOs → Contratos de entrada/saída
- Models → Entidades do domínio

## 📌 Endpoints

### GET /api/users
Retorna todos os usuários

### GET /api/users/{id}
Retorna um usuário por ID

### POST /api/users
Cria um novo usuário

```json
{
  "name": "Yuri",
  "email": "yurilisco@gmail.com"
}

▶️ Como rodar o projeto
cd Api
dotnet run


Acesse:

http://localhost:5150/swagger
