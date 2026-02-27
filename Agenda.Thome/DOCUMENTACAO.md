# Agenda.Thome — Guia Completo para o Desenvolvedor

Este documento explica **tudo** sobre o projeto Agenda.Thome: cada pasta, cada arquivo, cada conceito utilizado. Foi escrito para uma pessoa que está começando no desenvolvimento de software e quer entender profundamente como o projeto funciona.

---

## Índice

1. [O que é o projeto?](#1-o-que-é-o-projeto)
2. [Tecnologias utilizadas](#2-tecnologias-utilizadas)
3. [Arquitetura do projeto (Clean Architecture)](#3-arquitetura-do-projeto-clean-architecture)
4. [Estrutura de pastas](#4-estrutura-de-pastas)
5. [Camada Domain (Domínio)](#5-camada-domain-domínio)
6. [Camada Application (Aplicação)](#6-camada-application-aplicação)
7. [Camada Infrastructure (Infraestrutura)](#7-camada-infrastructure-infraestrutura)
8. [Camada API](#8-camada-api)
9. [Frontend React](#9-frontend-react)
10. [Conceitos importantes (SOLID, Clean Code)](#10-conceitos-importantes-solid-clean-code)
11. [Fluxo completo: como funciona de ponta a ponta](#11-fluxo-completo-como-funciona-de-ponta-a-ponta)
12. [Como rodar o projeto](#12-como-rodar-o-projeto)
13. [Glossário](#13-glossário)

---

## 1. O que é o projeto?

O **Agenda.Thome** é um sistema de agendamento online pensado para profissionais da saúde (nutricionistas, dentistas, etc.).

### O que o sistema faz:

- O profissional se cadastra e faz login
- Ele pode criar, editar, listar e excluir consultas (CRUD)
- O sistema gera um **link público de agendamento**
- O profissional envia esse link para seus pacientes
- O paciente acessa o link, vê os horários disponíveis e agenda uma consulta
- A consulta agendada pelo paciente aparece automaticamente na agenda do profissional

### Regras de negócio:

- Horário comercial: **08h às 18h**
- Somente **dias úteis** (segunda a sexta)
- Agendamentos em **horas cheias** (08:00, 09:00, 10:00...)
- Não pode agendar no **mesmo horário** que outro paciente
- Não pode agendar no **passado**

---

## 2. Tecnologias utilizadas

### Backend (API)

| Tecnologia | Para que serve |
|---|---|
| **.NET 8** | Framework para construir a API (feito pela Microsoft) |
| **ASP.NET Core** | Parte do .NET específica para criar APIs web |
| **Entity Framework Core** | ORM — traduz código C# em operações no banco de dados |
| **InMemory Database** | Banco de dados que vive na memória (para simplificar, sem instalar nada) |
| **JWT (JSON Web Token)** | Sistema de autenticação — gera um "token" que prova que o usuário está logado |
| **BCrypt** | Algoritmo para criptografar senhas (nunca guardamos senha em texto puro!) |
| **Swagger** | Gera uma interface visual para testar a API no navegador |

### Frontend

| Tecnologia | Para que serve |
|---|---|
| **React** | Biblioteca JavaScript para construir interfaces (criada pelo Facebook) |
| **Vite** | Ferramenta que cria e roda o projeto React (mais rápido que Create React App) |
| **React Router** | Gerencia as páginas/rotas do React (login, cadastro, etc.) |
| **Axios** | Biblioteca para fazer chamadas HTTP (comunicar com a API) |

---

## 3. Arquitetura do projeto (Clean Architecture)

### O que é Clean Architecture?

É uma forma de **organizar o código** em camadas, onde cada camada tem uma responsabilidade clara. A ideia principal é:

> **O código de negócio (regras) não deve depender de tecnologia (banco de dados, framework, etc.)**

Imagine assim: se amanhã você trocar o banco de dados de InMemory para PostgreSQL, **apenas a camada de infraestrutura muda**. As regras de negócio continuam iguais.

### As 4 camadas do projeto:

```
┌─────────────────────────────────────────┐
│              API (Controllers)          │  ← Recebe as requisições HTTP
├─────────────────────────────────────────┤
│           Infrastructure                │  ← Banco de dados, JWT, BCrypt
├─────────────────────────────────────────┤
│             Application                 │  ← Regras de negócio, serviços
├─────────────────────────────────────────┤
│               Domain                    │  ← Entidades e contratos
└─────────────────────────────────────────┘
```

### Regra de dependência:

- **Domain** não depende de nada (é o centro)
- **Application** depende apenas do Domain
- **Infrastructure** depende do Domain e Application
- **API** depende de Application e Infrastructure

Isso significa que o Domain é puro — ele não sabe que existe banco de dados, não sabe que existe HTTP, não sabe que existe JWT. Ele só define "o que existe" (entidades) e "o que precisa ser feito" (interfaces).

---

## 4. Estrutura de pastas

```
Agenda.Thome/
│
├── Agenda.Thome.sln                          ← Arquivo da solução .NET
├── README.md
├── .gitignore
│
├── src/
│   ├── Agenda.Thome.Domain/                  ← CAMADA 1: Domínio
│   │   ├── Entities/
│   │   │   ├── User.cs                       ← Entidade Usuário
│   │   │   └── Appointment.cs                ← Entidade Consulta
│   │   └── Interfaces/
│   │       ├── IUserRepository.cs            ← Contrato do repositório de usuários
│   │       └── IAppointmentRepository.cs     ← Contrato do repositório de consultas
│   │
│   ├── Agenda.Thome.Application/             ← CAMADA 2: Aplicação
│   │   ├── DTOs/
│   │   │   ├── LoginRequest.cs               ← Dados que chegam no login
│   │   │   ├── LoginResponse.cs              ← Dados que saem no login
│   │   │   ├── RegisterRequest.cs            ← Dados que chegam no cadastro
│   │   │   ├── AppointmentRequest.cs         ← Dados para criar/editar consulta
│   │   │   ├── AppointmentResponse.cs        ← Dados que saem da consulta
│   │   │   ├── AvailableSlotResponse.cs      ← Horário disponível
│   │   │   └── BookingRequest.cs             ← Dados para agendamento público
│   │   ├── Interfaces/
│   │   │   ├── IAuthService.cs               ← Contrato do serviço de auth
│   │   │   ├── IAppointmentService.cs        ← Contrato do serviço de consultas
│   │   │   ├── IBookingService.cs            ← Contrato do serviço de agendamento
│   │   │   ├── ITokenService.cs              ← Contrato para gerar token JWT
│   │   │   └── IPasswordHasher.cs            ← Contrato para criptografia de senha
│   │   └── Services/
│   │       ├── AuthService.cs                ← Implementação do login/cadastro
│   │       ├── AppointmentService.cs         ← Implementação do CRUD de consultas
│   │       └── BookingService.cs             ← Implementação do agendamento público
│   │
│   ├── Agenda.Thome.Infrastructure/          ← CAMADA 3: Infraestrutura
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs               ← Configuração do Entity Framework
│   │   │   └── DatabaseSeeder.cs             ← Dados iniciais de teste
│   │   ├── Repositories/
│   │   │   ├── UserRepository.cs             ← Acesso ao banco para usuários
│   │   │   └── AppointmentRepository.cs      ← Acesso ao banco para consultas
│   │   ├── Auth/
│   │   │   ├── TokenService.cs               ← Geração de tokens JWT
│   │   │   └── PasswordHasher.cs             ← Criptografia de senhas com BCrypt
│   │   └── DependencyInjection.cs            ← Registro de todos os serviços
│   │
│   └── Agenda.Thome.API/                     ← CAMADA 4: API
│       ├── Controllers/
│       │   ├── AuthController.cs             ← Endpoints de login/cadastro
│       │   ├── AppointmentsController.cs     ← Endpoints CRUD de consultas
│       │   └── BookingController.cs          ← Endpoints de agendamento público
│       ├── Program.cs                        ← Configuração e inicialização da API
│       └── appsettings.json                  ← Configurações (chave JWT, etc.)
│
└── frontend/                                 ← FRONTEND REACT
    ├── src/
    │   ├── main.jsx                          ← Ponto de entrada do React
    │   ├── App.jsx                           ← Roteamento e estrutura principal
    │   ├── index.css                         ← Reset CSS global
    │   ├── styles.css                        ← Estilos do projeto
    │   ├── contexts/
    │   │   └── AuthContext.jsx               ← Contexto de autenticação
    │   ├── services/
    │   │   └── api.js                        ← Configuração do Axios
    │   └── pages/
    │       ├── Login.jsx                     ← Página de login
    │       ├── Register.jsx                  ← Página de cadastro
    │       ├── Appointments.jsx              ← Página de consultas (CRUD)
    │       └── Booking.jsx                   ← Página pública de agendamento
    ├── package.json
    └── vite.config.js
```

---

## 5. Camada Domain (Domínio)

A camada Domain é o **coração** do projeto. Ela define:
- **O que existe** no sistema (Entidades)
- **O que o sistema precisa fazer** com os dados (Interfaces de Repositório)

### 5.1 Entidades

#### User.cs (Usuário)

```csharp
public class User
{
    public Guid Id { get; private set; }           // Identificador único
    public string Name { get; private set; }        // Nome do profissional
    public string Email { get; private set; }       // E-mail (login)
    public string PasswordHash { get; private set; } // Senha criptografada
    public Guid BookingToken { get; private set; }  // Token para o link de agendamento
    public DateTime CreatedAt { get; private set; } // Data de criação

    public ICollection<Appointment> Appointments { get; private set; } // Consultas do profissional
}
```

**Conceitos importantes aqui:**

- **`Guid`**: É um identificador único universal (algo como `3fa85f64-5717-4562-b3fc-2c963f66afa6`). Diferente de `int` (1, 2, 3...), o Guid é único mesmo em sistemas distribuídos.

- **`private set`**: Significa que **só a própria classe pode alterar** essas propriedades. Isso é **encapsulamento** — um princípio fundamental da programação orientada a objetos. Ninguém de fora pode fazer `user.Name = "outro nome"`. Para mudar, precisa chamar o método `user.UpdateName("outro nome")`.

- **`BookingToken`**: É um Guid gerado automaticamente quando o usuário se cadastra. Esse token é usado para criar o link público de agendamento. Exemplo: `/booking/3fa85f64-5717-4562-b3fc-2c963f66afa6`

- **`PasswordHash`**: Nunca guardamos a senha real. Usamos BCrypt para transformá-la em um hash irreversível. Exemplo: a senha `"123456"` vira algo como `"$2a$11$Kj8J..."`.

- **Construtor protegido `protected User() { }`**: Existe para o Entity Framework conseguir criar objetos User quando lê do banco de dados. O EF precisa de um construtor sem parâmetros.

#### Appointment.cs (Consulta)

```csharp
public class Appointment
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }           // A qual profissional pertence
    public string PatientName { get; private set; }     // Nome do paciente
    public string PatientEmail { get; private set; }    // E-mail do paciente
    public string PatientPhone { get; private set; }    // Telefone do paciente
    public DateTime ScheduledAt { get; private set; }   // Data e hora da consulta
    public DateTime CreatedAt { get; private set; }     // Data de criação

    public User User { get; private set; }             // Navegação para o usuário
}
```

**Conceitos:**

- **`UserId`**: É a **chave estrangeira** (Foreign Key). Ela conecta a consulta ao profissional. Cada consulta pertence a um usuário.

- **`User` (propriedade de navegação)**: O Entity Framework usa isso para fazer JOINs automaticamente. Se você tiver uma consulta, pode acessar `appointment.User.Name` para pegar o nome do profissional.

### 5.2 Interfaces de Repositório

```csharp
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByBookingTokenAsync(Guid bookingToken);
    Task AddAsync(User user);
}
```

**O que é uma Interface?**

Uma interface é um **contrato**. Ela diz: "quem me implementar, precisa ter esses métodos". Mas ela **não diz como** esses métodos funcionam.

Por exemplo, `IUserRepository` diz que precisa existir um método `GetByEmailAsync`. Mas não diz se vai buscar no SQL Server, no MongoDB, ou na memória. Quem decide isso é a **implementação** (que fica na camada Infrastructure).

**Por que usar interfaces?**

1. **Desacoplamento**: O código de negócio não sabe qual banco de dados está sendo usado
2. **Testabilidade**: Para testes, você pode criar uma implementação fake
3. **Flexibilidade**: Pode trocar a implementação sem mudar nada no código de negócio

**O que é `Task<User?>`?**

- `Task`: Significa que o método é **assíncrono**. Ele não trava o sistema enquanto espera o banco de dados responder. Isso é importante para performance.
- `User?`: O `?` significa que pode retornar `null` (quando o usuário não existe).

---

## 6. Camada Application (Aplicação)

A camada Application contém as **regras de negócio** e **DTOs**.

### 6.1 DTOs (Data Transfer Objects)

DTOs são objetos simples usados para **transportar dados** entre camadas. Eles existem para:

1. **Não expor as entidades do domínio diretamente** — se você retornar a entidade `User` na API, o `PasswordHash` iria junto (grave problema de segurança!)
2. **Controlar o que entra e o que sai** da API
3. **Validação** — podemos adicionar regras de validação nos DTOs

```csharp
public record LoginRequest(
    [Required] string Email,
    [Required] string Password
);
```

**O que é `record`?**

É uma forma moderna do C# de criar classes simples e imutáveis (não podem ser alteradas depois de criadas). Perfeito para DTOs. É mais limpo que criar uma classe com propriedades.

**O que é `[Required]`?**

É um **Data Annotation** — uma anotação que diz ao ASP.NET Core que este campo é obrigatório. Se alguém enviar um login sem e-mail, a API automaticamente retorna um erro 400 (Bad Request).

### 6.2 Interfaces de Serviço

```csharp
public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> RegisterAsync(RegisterRequest request);
}
```

Mesma ideia das interfaces de repositório. Definem **o que o serviço faz**, mas não **como**.

### 6.3 Serviços (Implementações)

#### AuthService.cs

```csharp
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUserRepository userRepository, ITokenService tokenService, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }
}
```

**Conceitos importantes:**

- **`private readonly`**: `private` significa que só esta classe pode acessar. `readonly` significa que só pode ser atribuído uma vez (no construtor). Isso é uma boa prática — garante que as dependências não mudem depois da criação do objeto.

- **Injeção de Dependência (DI)**: O `AuthService` **não cria** os repositórios e serviços que ele precisa. Ele **recebe** eles pelo construtor. Isso é o padrão de **Injeção de Dependência**. Quem decide qual implementação usar é o container de DI (configurado no `Program.cs`).

- **Por que usar DI?** Imagine que amanhã você quer trocar o BCrypt por outro algoritmo de hash. Sem DI, você teria que mudar em todos os lugares que usam. Com DI, muda em **um só lugar** (o registro no container).

#### AppointmentService.cs — Regras de Negócio

```csharp
private static void ValidateBusinessHours(DateTime scheduledAt)
{
    if (scheduledAt.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        throw new InvalidOperationException("Não é possível agendar nos finais de semana.");

    if (hour < 8 || hour >= 18)
        throw new InvalidOperationException("O horário deve estar dentro do horário comercial (08:00 às 18:00).");

    if (scheduledAt.Minute != 0 || scheduledAt.Second != 0)
        throw new InvalidOperationException("Os agendamentos devem ser em horas cheias.");
}
```

Aqui ficam as **regras de negócio**:
- Validar horário comercial
- Verificar se o horário está disponível
- Impedir agendamento em finais de semana

**Por que lançar `throw new InvalidOperationException`?** 

Porque quando uma regra de negócio é violada, o código lança uma **exceção**. O Controller (na camada API) captura essa exceção e retorna o erro apropriado para o usuário.

---

## 7. Camada Infrastructure (Infraestrutura)

É aqui que mora a **tecnologia real**: banco de dados, JWT, criptografia.

### 7.1 AppDbContext.cs (Entity Framework)

```csharp
public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
}
```

**O que é o DbContext?**

É a classe principal do Entity Framework. Ele é a **ponte entre o C# e o banco de dados**. Cada `DbSet<T>` representa uma **tabela** no banco.

- `DbSet<User>` = tabela de usuários
- `DbSet<Appointment>` = tabela de consultas

**O que é o `OnModelCreating`?**

É onde configuramos as **regras do banco de dados** (tamanhos de campos, chaves, índices, etc.):

```csharp
entity.HasKey(u => u.Id);                        // Define a chave primária
entity.Property(u => u.Name).IsRequired()        // Campo obrigatório
      .HasMaxLength(150);                        // Máximo 150 caracteres
entity.HasIndex(u => u.Email).IsUnique();        // E-mail deve ser único
entity.HasMany(u => u.Appointments)              // Um usuário tem muitas consultas
      .WithOne(a => a.User)                      // Cada consulta tem um usuário
      .HasForeignKey(a => a.UserId);             // Ligados pelo UserId
```

### 7.2 Repositórios

Os repositórios **implementam** as interfaces do Domain:

```csharp
public class UserRepository : IUserRepository  // ← implementa a interface
{
    private readonly AppDbContext _context;

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}
```

**O que é `async/await`?**

Quando o código precisa esperar algo demorado (como uma consulta ao banco de dados), usamos `async/await` para **não travar** a aplicação. Enquanto espera a resposta do banco, a thread fica livre para processar outras requisições.

**O que é LINQ?**

`.Where(a => a.UserId == userId)` é **LINQ** — uma linguagem de consulta integrada ao C#. É como SQL, mas escrito em C#. O Entity Framework traduz automaticamente para SQL real.

### 7.3 TokenService.cs (JWT)

```csharp
public string GenerateToken(Guid userId, string email)
{
    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Email, email)
    };

    var token = new JwtSecurityToken(
        issuer: "Agenda.Thome",          // Quem criou o token
        audience: "Agenda.Thome.Users",  // Para quem é o token
        claims: claims,                   // Informações dentro do token
        expires: DateTime.UtcNow.AddHours(8),  // Expira em 8 horas
        signingCredentials: credentials   // Assinatura criptográfica
    );
}
```

**Como funciona o JWT?**

1. O usuário faz login com e-mail e senha
2. Se estiver correto, o servidor gera um **token** (uma string longa)
3. O frontend guarda esse token no `localStorage`
4. Em toda requisição futura, o frontend envia o token no header `Authorization: Bearer <token>`
5. O servidor valida o token e sabe quem é o usuário

**O que são Claims?**

Claims são **informações** que ficam dentro do token. No nosso caso, guardamos o `userId` e o `email`. Assim, quando o servidor recebe o token, ele extrai o userId e sabe qual usuário está fazendo a requisição.

### 7.4 PasswordHasher.cs (BCrypt)

```csharp
public string Hash(string password)
{
    return BCrypt.Net.BCrypt.HashPassword(password);
}

public bool Verify(string password, string passwordHash)
{
    return BCrypt.Net.BCrypt.Verify(password, passwordHash);
}
```

**Por que nunca guardar senhas em texto puro?**

Se alguém tiver acesso ao banco de dados, veria todas as senhas. Com BCrypt, mesmo que o banco vaze, as senhas são **irreversíveis** — ninguém consegue descobrir a senha original a partir do hash.

### 7.5 DependencyInjection.cs

```csharp
public static IServiceCollection AddInfrastructure(this IServiceCollection services)
{
    services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("AgendaThomeDb"));

    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IAppointmentService, AppointmentService>();
    // ...
}
```

**O que é `AddScoped`?**

Diz ao .NET: "quando alguém pedir um `IUserRepository`, crie um `UserRepository`". O **Scoped** significa que um novo objeto é criado para cada requisição HTTP.

Existem 3 tipos de ciclo de vida:
- **Transient**: novo objeto a cada vez que é pedido
- **Scoped**: novo objeto a cada requisição HTTP (mais comum)
- **Singleton**: mesmo objeto para toda a aplicação

---

## 8. Camada API

### 8.1 Program.cs

É o **ponto de entrada** da aplicação. Aqui configuramos tudo:

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Registra os serviços (DI)
builder.Services.AddInfrastructure();
builder.Services.AddControllers();

// 2. Configura autenticação JWT
builder.Services.AddAuthentication(...)
    .AddJwtBearer(options => { ... });

// 3. Configura Swagger (documentação da API)
builder.Services.AddSwaggerGen(options => { ... });

// 4. Configura CORS (permite o frontend acessar a API)
builder.Services.AddCors(options => { ... });

var app = builder.Build();

// 5. Seed: cria dados iniciais de teste
using (var scope = app.Services.CreateScope())
{
    DatabaseSeeder.Seed(context, passwordHasher);
}

// 6. Pipeline de middlewares
app.UseAuthentication();  // Verifica o token JWT
app.UseAuthorization();   // Verifica permissões
app.MapControllers();     // Mapeia as rotas dos controllers
```

**O que é CORS?**

Cross-Origin Resource Sharing. O navegador bloqueia requisições de um domínio para outro por segurança. Como o frontend roda em `localhost:5173` e a API em `localhost:5068`, precisamos habilitar CORS para que o frontend possa acessar a API.

### 8.2 Controllers

Controllers são classes que **recebem as requisições HTTP** e retornam respostas.

```csharp
[ApiController]                    // Indica que é um controller de API
[Route("api/[controller]")]        // A rota será /api/auth
public class AuthController : ControllerBase
{
    [HttpPost("login")]            // POST /api/auth/login
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);           // HTTP 200
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });  // HTTP 401
        }
    }
}
```

**O que são os atributos `[HttpGet]`, `[HttpPost]`, etc.?**

Eles definem qual **verbo HTTP** ativa cada método:
- `[HttpGet]` — Buscar dados
- `[HttpPost]` — Criar algo novo
- `[HttpPut]` — Atualizar algo existente
- `[HttpDelete]` — Excluir algo

**O que é `[Authorize]`?**

```csharp
[Authorize]  // ← Este controller exige token JWT
public class AppointmentsController : ControllerBase
```

Isso faz com que o ASP.NET Core **rejeite** requisições sem um token válido. O `BookingController` **não tem** `[Authorize]` porque é público (o paciente não tem conta).

**Códigos HTTP:**
- `200 OK` — Sucesso
- `201 Created` — Criado com sucesso
- `204 No Content` — Excluído com sucesso
- `400 Bad Request` — Dados inválidos
- `401 Unauthorized` — Não autenticado
- `403 Forbidden` — Sem permissão
- `404 Not Found` — Não encontrado
- `409 Conflict` — Conflito (ex: e-mail já existe)

---

## 9. Frontend React

### 9.1 Estrutura

O frontend foi feito de forma **simples e direta**, usando apenas:
- **React** (componentes funcionais com hooks)
- **React Router** (roteamento)
- **Axios** (chamadas HTTP)
- **CSS puro** (sem bibliotecas visuais)

### 9.2 api.js — Configuração do Axios

```javascript
const api = axios.create({
  baseURL: 'http://localhost:5068/api',
});

// Interceptor: adiciona o token JWT em toda requisição
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});
```

**O que é um interceptor?**

É um "middleware" do Axios. Antes de cada requisição, ele automaticamente pega o token do `localStorage` e coloca no header. Assim, não precisamos repetir esse código em todo lugar.

### 9.3 AuthContext.jsx — Contexto de Autenticação

```javascript
const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    const saved = localStorage.getItem('user');
    return saved ? JSON.parse(saved) : null;
  });

  const login = (userData, token) => {
    localStorage.setItem('token', token);
    localStorage.setItem('user', JSON.stringify(userData));
    setUser(userData);
  };
}
```

**O que é Context API?**

É a forma do React de compartilhar dados entre componentes sem precisar passar props de pai para filho. O `AuthContext` guarda as informações do usuário logado e todas as páginas podem acessar.

**O que é `localStorage`?**

É um armazenamento do navegador. Os dados ficam salvos mesmo se o usuário fechar a aba. Usamos para guardar o token JWT e os dados do usuário.

### 9.4 App.jsx — Roteamento

```javascript
<Routes>
  <Route path="/login" element={<PublicRoute><Login /></PublicRoute>} />
  <Route path="/appointments" element={<PrivateRoute><Appointments /></PrivateRoute>} />
  <Route path="/booking/:token" element={<Booking />} />
</Routes>
```

**O que é `:token`?**

É um **parâmetro de rota**. Quando o paciente acessa `/booking/abc-123`, o React Router extrai `abc-123` e disponibiliza via `useParams()`.

**PrivateRoute e PublicRoute:**

- `PrivateRoute`: Se não estiver logado, redireciona para o login
- `PublicRoute`: Se já estiver logado, redireciona para as consultas

### 9.5 Páginas

- **Login.jsx**: Formulário de e-mail e senha. Chama `POST /api/auth/login`.
- **Register.jsx**: Formulário de cadastro. Chama `POST /api/auth/register`.
- **Appointments.jsx**: Principal página do profissional. Lista consultas, permite criar/editar/excluir. Mostra o link de agendamento.
- **Booking.jsx**: Página pública. O paciente escolhe uma data, vê os horários disponíveis e agenda.

### 9.6 Hooks do React usados

| Hook | Para que serve |
|------|----------------|
| `useState` | Guarda e atualiza dados no componente (ex: valor do input) |
| `useEffect` | Executa código quando o componente é montado (ex: carregar consultas) |
| `useContext` | Acessa dados compartilhados (ex: dados do usuário logado) |
| `useNavigate` | Navega para outra página programaticamente |
| `useParams` | Extrai parâmetros da URL (ex: o token no link de agendamento) |

---

## 10. Conceitos importantes (SOLID, Clean Code)

### SOLID

São 5 princípios para escrever código de qualidade:

#### S — Single Responsibility Principle (Princípio da Responsabilidade Única)

> Cada classe deve ter **uma única razão para mudar**.

No projeto:
- `AuthService` cuida **apenas** de autenticação
- `AppointmentService` cuida **apenas** de consultas
- `UserRepository` cuida **apenas** do acesso a dados de usuários

#### O — Open/Closed Principle (Princípio Aberto/Fechado)

> Classes devem estar **abertas para extensão** e **fechadas para modificação**.

No projeto: se quisermos adicionar um novo tipo de repositório, criamos uma nova implementação da interface. Não precisamos mudar o código existente.

#### L — Liskov Substitution Principle (Princípio da Substituição de Liskov)

> Qualquer implementação de uma interface deve ser substituível sem quebrar o sistema.

No projeto: podemos trocar `UserRepository` (InMemory) por uma versão que use SQL Server, e o `AuthService` continua funcionando sem mudanças.

#### I — Interface Segregation Principle (Princípio da Segregação de Interfaces)

> É melhor ter **várias interfaces pequenas** do que uma grande.

No projeto: temos `IUserRepository`, `IAppointmentRepository`, `ITokenService`, `IPasswordHasher` — cada uma com sua responsabilidade específica. Não temos uma interface "IRepository" gigante com tudo junto.

#### D — Dependency Inversion Principle (Princípio da Inversão de Dependência)

> Dependa de **abstrações (interfaces)**, não de implementações concretas.

No projeto: o `AuthService` depende de `IUserRepository` (interface), não de `UserRepository` (classe concreta). Isso é garantido pela injeção de dependência.

### Clean Code

São práticas para escrever código **legível e manutenível**:

1. **Nomes significativos**: `ValidateBusinessHours` diz exatamente o que faz. Evitamos nomes como `Check` ou `Validate1`.

2. **Funções pequenas**: Cada método faz uma coisa só. `ValidateBusinessHours` só valida horário. `ValidateSlotAvailability` só valida disponibilidade.

3. **Sem código duplicado (DRY)**: As validações de horário são usadas tanto no `AppointmentService` quanto no `BookingService`, com a mesma lógica.

4. **Tratamento de erros**: Usamos exceções tipadas (`KeyNotFoundException`, `UnauthorizedAccessException`, `InvalidOperationException`) em vez de retornar strings de erro.

---

## 11. Fluxo completo: como funciona de ponta a ponta

### Fluxo 1: Login

```
Frontend (Login.jsx)
  → POST /api/auth/login { email, password }
    → AuthController.Login()
      → AuthService.LoginAsync()
        → UserRepository.GetByEmailAsync()    ← busca no banco
        → PasswordHasher.Verify()             ← verifica a senha
        → TokenService.GenerateToken()        ← gera o JWT
      ← retorna { token, name, email, bookingToken }
    ← HTTP 200 OK
  → localStorage.setItem('token', token)
  → navega para /appointments
```

### Fluxo 2: Criar consulta

```
Frontend (Appointments.jsx)
  → POST /api/appointments (com token no header)
    → [Authorize] verifica o token JWT
    → AppointmentsController.Create()
      → extrai userId do token
      → AppointmentService.CreateAsync()
        → ValidateBusinessHours()            ← verifica horário comercial
        → ValidateSlotAvailability()         ← verifica se horário está livre
        → AppointmentRepository.AddAsync()   ← salva no banco
      ← retorna AppointmentResponse
    ← HTTP 201 Created
  → recarrega a lista de consultas
```

### Fluxo 3: Paciente agenda pelo link

```
Frontend (Booking.jsx)
  → GET /api/booking/{token}/slots?date=2026-03-02
    → BookingController.GetAvailableSlots()
      → BookingService.GetAvailableSlotsAsync()
        → UserRepository.GetByBookingTokenAsync()  ← encontra o profissional
        → AppointmentRepository.GetByUserIdAndDateAsync() ← consultas do dia
        → gera lista de 08h-18h marcando ocupados
      ← retorna lista de slots
    ← HTTP 200 OK

  → Paciente seleciona horário e preenche dados
  → POST /api/booking/{token}/book
    → BookingController.Book()
      → BookingService.BookAsync()
        → valida horário
        → verifica disponibilidade
        → cria Appointment vinculado ao profissional
      ← retorna AppointmentResponse
    ← HTTP 201 Created
```

---

## 12. Como rodar o projeto

### Pré-requisitos

- **.NET 8 SDK** — baixe em https://dotnet.microsoft.com/download
- **Node.js** (v20+) — baixe em https://nodejs.org/

### Backend (API)

```bash
# Na pasta raiz do projeto (Agenda.Thome)
dotnet restore
dotnet run --project src/Agenda.Thome.API
```

A API estará rodando em `http://localhost:5068`.
Swagger (documentação visual): `http://localhost:5068/swagger`

### Frontend (React)

```bash
# Na pasta frontend
cd frontend
npm install
npm run dev
```

O frontend estará em `http://localhost:5173`.

### Dados de teste (Seed)

O sistema já vem com dados para testar:

- **Usuário**: `joao@email.com` / senha `123456`
- **2 consultas**: agendadas para o próximo dia útil (09h e 14h)

---

## 13. Glossário

| Termo | Significado |
|-------|-------------|
| **API** | Application Programming Interface — é o "servidor" que o frontend consulta |
| **REST** | Padrão de organização de APIs usando verbos HTTP (GET, POST, PUT, DELETE) |
| **CRUD** | Create, Read, Update, Delete — as 4 operações básicas |
| **DTO** | Data Transfer Object — objeto para transportar dados entre camadas |
| **ORM** | Object-Relational Mapping — traduz código em operações no banco de dados |
| **JWT** | JSON Web Token — token para autenticação |
| **Hash** | Transformação irreversível (ex: senha → hash) |
| **Middleware** | Código que roda entre a requisição e a resposta |
| **Endpoint** | Uma rota específica da API (ex: POST /api/auth/login) |
| **DI** | Dependency Injection — padrão para gerenciar dependências |
| **CORS** | Cross-Origin Resource Sharing — permite acesso entre domínios diferentes |
| **LINQ** | Language Integrated Query — consultas integradas ao C# |
| **Seed** | Dados iniciais inseridos automaticamente para teste |
| **InMemory** | Banco de dados que vive na RAM (sem instalar banco real) |
| **Interceptor** | Código que roda automaticamente antes/depois de uma requisição |
| **Context API** | Forma do React de compartilhar dados entre componentes |
| **Hook** | Funções especiais do React (useState, useEffect, etc.) |
| **Clean Architecture** | Padrão de organização em camadas com separação de responsabilidades |
| **SOLID** | 5 princípios de design de software orientado a objetos |
| **Encapsulamento** | Esconder detalhes internos de uma classe (private set) |
