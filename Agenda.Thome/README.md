# Agenda.Thome

Sistema de agendamento online para profissionais da saúde (nutricionistas, dentistas, etc.).

## Tecnologias

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core (InMemory)
- JWT Authentication
- Clean Architecture

## Arquitetura

O projeto segue os princípios de **Clean Architecture**, separado em 4 camadas:

| Camada | Responsabilidade |
|--------|-----------------|
| `Agenda.Thome.Domain` | Entidades e interfaces de repositório |
| `Agenda.Thome.Application` | DTOs, interfaces de serviço e regras de negócio |
| `Agenda.Thome.Infrastructure` | EF Core, repositórios, autenticação (JWT/BCrypt) |
| `Agenda.Thome.API` | Controllers e configuração da aplicação |

## Funcionalidades

- **Autenticação** — Registro e login com JWT
- **CRUD de Consultas** — Criar, listar, editar e excluir agendamentos
- **Link de Agendamento** — Gerar link público para pacientes agendarem
- **Horários Disponíveis** — Horário comercial (08h-18h), dias úteis, slots de 1h
- **Validações** — Conflito de horários, horário comercial, dias úteis

## Como executar

```bash
dotnet restore
dotnet run --project src/Agenda.Thome.API
```

A API estará disponível em `https://localhost:5001` (ou a porta configurada).

Acesse o Swagger em: `https://localhost:5001/swagger`

## Seed (Dados de teste)

Ao iniciar, o sistema cria automaticamente:

- **Usuário:** `joao@email.com` / senha: `123456`
- **2 consultas** agendadas para o próximo dia útil (09h e 14h)

## Endpoints

### Autenticação

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/auth/register` | Registrar novo usuário |
| POST | `/api/auth/login` | Login (retorna JWT + booking token) |

### Consultas (requer autenticação)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/appointments` | Listar consultas |
| GET | `/api/appointments/{id}` | Buscar consulta por ID |
| POST | `/api/appointments` | Criar consulta |
| PUT | `/api/appointments/{id}` | Atualizar consulta |
| DELETE | `/api/appointments/{id}` | Excluir consulta |

### Agendamento público (sem autenticação)

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/booking/{token}/slots?date=2024-03-01` | Ver horários disponíveis |
| POST | `/api/booking/{token}/book` | Agendar consulta |

## Exemplo de uso

1. Faça login com `POST /api/auth/login`
2. Copie o `bookingToken` da resposta
3. Envie o link `/api/booking/{bookingToken}/slots?date=YYYY-MM-DD` para o paciente
4. O paciente vê os horários disponíveis e agenda com `POST /api/booking/{bookingToken}/book`
