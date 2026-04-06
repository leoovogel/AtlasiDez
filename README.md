# AtlasiDez

API REST em .NET para consulta de municípios brasileiros por UF, com SPA interativa em React para visualização no mapa do Brasil.

## Tecnologias

| Camada | Stack |
|--------|-------|
| Backend | .NET 10, ASP.NET Core, Redis |
| Frontend | React 19, TypeScript, Vite, Tailwind CSS |
| Infra | Docker, GitHub Actions, Azure Container Apps |

## Arquitetura

O backend segue **Clean Architecture** com quatro camadas:

```
Domain            Entidades e interfaces (City, ICityProvider)
  └─ Application  Casos de uso, DTOs e interfaces de serviço (GetCitiesByStateUseCase, ICacheService)
       └─ Infrastructure  Implementações externas (Providers, Redis Cache)
            └─ Api        Controllers, middleware e configuração
```

**Padrões principais:**

- **Provider Pattern** — A fonte de dados (Brasil API ou IBGE) é definida via variável de ambiente `CityProvider`, permitindo alternar sem alterar código.
- **Cache-Aside** — O use case consulta o Redis antes de chamar o provider externo. Caso não encontre, busca da API e armazena no cache.
- **Exception Handling Middleware** — Erros são capturados globalmente e retornados sem expor detalhes internos.

## Deploy

A aplicacao esta disponivel em ambiente cloud com deploy automatico via CD (GitHub Actions) a cada push na branch `main`.

| Servico | Hospedagem | URL |
|---------|-----------|-----|
| Frontend | Azure Static Web Apps | https://kind-mushroom-092df1110.6.azurestaticapps.net |
| Backend | Azure Container Apps | https://atlasidez-api.proudriver-27e08694.brazilsouth.azurecontainerapps.io |
| Redis | Redis Cloud | — |

---

## Como rodar localmente

### Com Docker Compose (recomendado)

```bash
docker compose up
```

| Serviço | URL |
|---------|-----|
| API | http://localhost:5087 |
| Frontend | http://localhost:3000 |
| Redis | localhost:6379 |

### Sem Docker

**Pré-requisitos:** .NET 10 SDK, Node.js 24+, Redis rodando em `localhost:6379`

**Backend:**
```bash
cd backend
dotnet restore AtlasiDez.slnx
dotnet run --project AtlasiDez.Api
```
A API estará disponível em `http://localhost:5087`.

**Frontend:**
```bash
cd frontend
npm install
npm run dev
```
O frontend estará disponível em `http://localhost:3000`.

## Configuracao

| Variavel | Valores | Default | Descricao |
|----------|---------|---------|-----------|
| `CityProvider` | `Ibge`, `BrasilApi` | `Ibge` | Provider de dados de municipios |
| `Cache__RedisConnectionString` | connection string | `localhost:6379` | Conexao com Redis |
| `Cache__ExpirationInMinutes` | inteiro | `5` | Tempo de expiracao do cache em minutos |
| `Cors__AllowedOrigins__0` | URL | `http://localhost:3000` | Origem permitida para CORS |

Todas podem ser definidas via variavel de ambiente ou `appsettings.json`.

## API

**Endpoint:** `GET /api/cities/{uf}`

```bash
# Municipios do RS (primeira pagina)
curl http://localhost:5087/api/cities/RS

# Pagina 2, 20 itens, filtrando por nome
curl "http://localhost:5087/api/cities/RS?page=2&pageSize=20&name=Santa"
```

**Resposta:**
```json
{
  "items": [
    { "name": "Porto Alegre", "ibge_code": "4314902" }
  ],
  "page": 1,
  "page_size": 10,
  "total_count": 497
}
```

## Testes

```bash
cd backend
dotnet test
```

| Tipo | Quantidade | Abordagem |
|------|-----------|-----------|
| Unitarios | 23 | NSubstitute para mocks de dependencias |
| Integracao | 27 | WebApplicationFactory com FakeCityProvider e cache in-memory |

**Cobertura minima:** 85% (enforced no CI).

Os testes de integracao validam o pipeline HTTP completo (routing, controller, use case, serialization) sem dependencias externas.

## Estrutura do Projeto

## CI/CD

| Workflow | Trigger | O que faz |
|----------|---------|-----------|
| CI | Push e PRs para `main` | Build, testes, lint frontend, verifica cobertura >= 85% |
| CD | Push para `main` | Build, testes, deploy automatico do backend (Azure Container Apps) e frontend (Azure Static Web Apps) |

```
AtlasiDez/
├── backend/
│   ├── AtlasiDez.Api/           # Controllers, middleware, Program.cs
│   ├── AtlasiDez.Application/   # Use cases, DTOs, interfaces
│   ├── AtlasiDez.Domain/        # Entidades e interfaces de dominio
│   ├── AtlasiDez.Infrastructure/ # Providers (BrasilApi, IBGE), Redis cache
│   ├── AtlasiDez.Tests/         # Testes unitarios e de integracao
│   └── Dockerfile
├── frontend/
│   ├── src/
│   │   ├── components/          # BrazilMap, Sidebar, CityList, Pagination
│   │   ├── services/            # Cliente HTTP da API
│   │   └── types/               # Tipagens TypeScript
│   └── Dockerfile
├── docs/
│   └── API.md                   # Documentacao detalhada da API
├── .github/workflows/           # CI e CD
└── docker-compose.yml           # Orquestracao dos servicos
```
