# EDSF — Gestão Empresarial

Aplicação desktop de gestão empresarial (faturação, stock, tesouraria, contabilidade) desenvolvida com **.NET MAUI Blazor Hybrid** + **ASP.NET Core Web API** + **SQLite**.

---

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Windows 10 19041+ (para executar o App)
- Visual Studio 2022 (opcional, recomendado para MAUI)

---

## Como Executar

### 1. Configurar a chave JWT (primeira vez apenas)

```cmd
dotnet user-secrets set "Jwt:Key" "uma-chave-com-pelo-menos-32-caracteres" --project src\EDSF.Api
```

### 2. Iniciar tudo (recomendado)

```cmd
run.cmd
```

Isto arranca a **API** (`http://localhost:5285`) e a **App** (janela nativa Windows).

### 3. Manualmente (terminais separados)

```cmd
:: Terminal 1 — API
dotnet run --project src\EDSF.Api

:: Terminal 2 — App
dotnet run --project src\EDSF.App -f net10.0-windows10.0.19041.0
```

---

## Credenciais de Teste

| Utilizador | Password  | Role  |
|------------|-----------|-------|
| admin      | admin123  | admin |
| joao       | 1234      | user  |
| maria      | 1234      | user  |
| carlos     | 1234      | user  |

> **Nota para desenvolvimento:** O login na App está atualmente em bypass — qualquer nome de utilizador entra com role `admin`. A API também não exige autenticação (dev-mode).

---

## Estrutura do Projeto

```
EDSF/
├── src/
│   ├── EDSF.Core/          # Domínio: models, enums, interfaces, serviços (SAFT, CSV, NIF, IVA)
│   ├── EDSF.Data/          # Persistência: EF Core + SQLite, migrações, repositórios, seed
│   ├── EDSF.Api/           # API REST: controllers, JWT auth, Scalar/OpenAPI
│   └── EDSF.App/           # App desktop: MAUI + Blazor Hybrid, páginas, serviços
├── tests/
│   └── EDSF.Tests/         # Testes unitários (xUnit)
├── run.cmd                 # Script para arrancar API + App em simultâneo
└── edsf.db                 # Base de dados SQLite (criada automaticamente)
```

### Dependências entre projetos

```
EDSF.App ──> EDSF.Core
EDSF.Api ──> EDSF.Core ──> EDSF.Data
EDSF.Tests ──> EDSF.Core
```

---

## API REST

A API está disponível em `http://localhost:5285/api/`.

**Documentação interativa** (apenas em desenvolvimento):  
[http://localhost:5285/scalar/v1](http://localhost:5285/scalar/v1)

### Endpoints principais

| Método | Rota                | Descrição              |
|--------|---------------------|------------------------|
| POST   | `/api/auth/login`   | Autenticação           |
| GET    | `/api/dashboard/*`  | Dashboard (KPI, trends)|
| CRUD   | `/api/servicos/*`   | Faturação, orçamentos  |
| CRUD   | `/api/armazem/*`    | Produtos, stock, fornecedores |
| CRUD   | `/api/tesouraria/*` | Finanças, caixa, notas |
| CRUD   | `/api/contabilidade/*` | Fluxo caixa, inventários |
| CRUD   | `/api/gestao/*`     | Utilizadores, permissões |
| GET    | `/api/saft`         | Exportação A-SAF-T     |

---

## Base de Dados

- **SQLite** (ficheiro `edsf.db` na pasta `src/EDSF.Api/`)
- As migrações são aplicadas automaticamente ao iniciar a API
- **Seed data** é carregada automaticamente se a base estiver vazia (clientes, produtos, faturas, etc.)
- Para re-criar a base de raiz: apague o `edsf.db` e reinicie a API

### Comandos de migração (se alterar os modelos)

```cmd
dotnet ef migrations add NomeDaMigracao ^
    --project src\EDSF.Data ^
    --startup-project src\EDSF.Api
```

---

## Tecnologias

| Camada        | Tecnologia                                    |
|---------------|-----------------------------------------------|
| Frontend      | .NET MAUI Blazor Hybrid, Bootstrap 5, Chart.js|
| Backend       | ASP.NET Core 10, EF Core 10                   |
| Base de Dados | SQLite                                        |
| Autenticação  | JWT Bearer (SHA256 + pepper)                  |
| API Docs      | OpenAPI + Scalar                              |
| Testes        | xUnit + coverlet                              |
| Formatação    | CSharpier (`dotnet csharpier .`)              |

---

## Configuração

| Chave               | Local          | Obrigatório | Descrição                     |
|---------------------|----------------|-------------|-------------------------------|
| `Jwt:Key`           | user-secrets   | Sim         | Chave de assinatura JWT       |
| `EDSF_API_URL`      | env var        | Não         | URL da API (def: `localhost`) |

---

## Troubleshooting

- **"API não conecta"** — Confirme que a API está a correr antes da App.
- **"Ficheiro bloqueado"** — Feche a API (Ctrl+C) antes de rebuildar.
- **"Maui não compila"** — Instale a carga de trabalho: `dotnet workload install maui`.
