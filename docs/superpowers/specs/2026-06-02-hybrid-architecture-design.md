# Hybrid Architecture: Local ERP + Cloud Management

## Overview

Sell EDSF to retail stores as a hybrid system: each store runs a local server (offline-capable, low latency), while a central cloud instance provides multi-store dashboards, license management, health monitoring, and backups.

## Architecture

### Per-Store (Local)

```
┌──────────────────────────────────────┐
│ Store Hardware (Intel NUC / Mini PC) │
│                                      │
│  ┌────────────────────────────────┐  │
│  │ EDSF.Api (Kestrel)            │  │
│  │ - SQLite database (local)     │  │
│  │ - JWT auth (local users)      │  │
│  │ - Sync API endpoints          │  │
│  └────────────┬───────────────────┘  │
│               │                       │
│  ┌────────────▼───────────────────┐  │
│  │ MAUI App (LAN clients)        │  │
│  │ - Connects to http://store-pc  │  │
│  │ - No internet required        │  │
│  └────────────────────────────────┘  │
│               │                       │
│  ┌────────────▼───────────────────┐  │
│  │ Sync Agent (Windows Service)   │  │
│  │ - Outbound HTTPS → cloud       │  │
│  │ - Sends: invoices, stock,      │  │
│  │   health metrics, SQLite backup │  │
│  │ - Receives: products, prices,  │  │
│  │   config updates               │  │
│  │ - Queue & retry on failure     │  │
│  └────────────────────────────────┘  │
└──────────────────────────────────────┘
```

**Local hardware requirements:**
- Intel NUC / HP EliteDesk Mini / equivalent
- Windows 10/11 Pro or Linux (Ubuntu Server)
- 4GB RAM, 120GB SSD (sufficient for SQLite + app)
- Static IP or hostname on local LAN

### Central Cloud

```
┌──────────────────────────────────────────┐
│ VPS (Hetzner / Contabo / Azure)          │
│                                          │
│  ┌────────────────────────────────────┐  │
│  │ EDSF.Cloud.Api (ASP.NET Core)      │  │
│  │ - Multi-tenant (tenant = store)    │  │
│  │ - PostgreSQL database              │  │
│  │ - Ingress: sync endpoints          │  │
│  └────────────┬───────────────────────┘  │
│               │                           │
│  ┌────────────▼───────────────────────┐  │
│  │ Multi-Store Dashboard (Blazor WASM)│  │
│  │ - Aggregated KPIs across stores    │  │
│  │ - Store health status (RAG)        │  │
│  │ - License overview & expiry        │  │
│  │ - Alert log                        │  │
│  └────────────────────────────────────┘  │
│               │                           │
│  ┌────────────▼───────────────────────┐  │
│  │ License Manager                   │  │
│  │ - Generate/store license keys     │  │
│  │ - Activate/deactivate stores      │  │
│  │ - Track module entitlements       │  │
│  │ - Alert on expiry                 │  │
│  └────────────────────────────────────┘  │
│               │                           │
│  ┌────────────▼───────────────────────┐  │
│  │ Health Monitor                     │  │
│  │ - Heartbeat per store (5-min)      │  │
│  │ - Disk / CPU / RAM / uptime        │  │
│  │ - Last sync timestamp              │  │
│  │ - Alerting (email/Telegram)        │  │
│  └────────────────────────────────────┘  │
│               │                           │
│  ┌────────────▼───────────────────────┐  │
│  │ Backup Vault                      │  │
│  │ - Encrypted SQLite snapshots      │  │
│  │ - Retention: 30 daily, 12 monthly │  │
│  │ - Point-in-time restore           │  │
│  └────────────────────────────────────┘  │
└──────────────────────────────────────────┘
```

## Data Flow

### Sync: Store → Cloud (outbound HTTPS)

| Data | Frequency | Direction | Notes |
|---|---|---|---|
| Invoices + Lines | 5 min | Store → Cloud | Creates/updates since last sync |
| Stock movements | 5 min | Store → Cloud | Delta sync |
| Finance records | 5 min | Store → Cloud | Payments, receipts |
| Leads/Opportunities | 5 min | Store → Cloud | CRM sync |
| Health heartbeat | 30 sec | Store → Cloud | CPU, disk, RAM, uptime, EDSF version |
| SQLite backup | 6 hours | Store → Cloud | Encrypted .zip upload |
| License validation | 24 hours | Store → Cloud | Verify key + entitlements |

### Sync: Cloud → Store (polled by sync agent)

| Data | Frequency | Direction | Notes |
|---|---|---|---|
| Product catalog | On change | Cloud → Store | Central price/product management |
| Tax tables | On change | Cloud → Store | IVA rates, regimes |
| Config overrides | On change | Cloud → Store | Store-level settings |
| License updates | On change | Cloud → Store | Entitlement changes, grace period |

### Conflict Resolution

- **Transations (invoices, payments):** Store is source of truth. Cloud rejects conflicting updates.
- **Master data (products, prices, tax tables):** Cloud is source of truth. Store overwrites on sync.
- **Stock:** Store wins (local physical count). Cloud aggregates for multi-store view.

## License System

### License Key Format

```
EDSF-{STORE_ID}-{YYYYMMDD-EXPIRY}-{CHECKSUM}
```

Generated server-side with HMAC signature to prevent forgery.

### Activation Flow

1. Store installs EDSF → enters license key on first boot
2. Sync agent calls `POST /api/license/activate` with key + hardware fingerprint
3. Cloud validates: key format → not expired → not already used → HMAC valid
4. Cloud returns: `{ storeId, modules, maxUsers, expiresAt }`
5. Store caches license locally, validates every 24h
6. If unreachable for 30 days, local app enters degraded mode (view-only)

### Entitlements (per key)

| Module | Description |
|---|---|
| `core` | Invoices, customers, basic CRM |
| `stock` | Warehouse, products, suppliers |
| `finance` | Treasury, payment notes, cash flow |
| `accounting` | Contabilidade, SAF-T |
| `hr` | Employees, payroll |
| `reports` | BI dashboards, statistical charts |

## Health Monitoring

### Heartbeat Payload

```json
{
  "storeId": "LOJA-A7F3",
  "timestamp": "2026-06-02T10:00:00Z",
  "version": "1.2.0",
  "uptimeHours": 312,
  "diskFreeGb": 45.2,
  "diskTotalGb": 118.0,
  "ramFreeMb": 2048,
  "ramTotalMb": 4096,
  "cpuPercent": 23,
  "dbSizeMb": 14.5,
  "lastBackupUtc": "2026-06-02T04:00:00Z",
  "syncQueueCount": 0
}
```

### Alert Rules

| Condition | Severity | Action |
|---|---|---|
| No heartbeat > 5 min | Warning | Dashboard yellow |
| No heartbeat > 60 min | Critical | Email + Telegram |
| Disk free < 10% | Critical | Email |
| License expires < 30 days | Warning | Dashboard + email |
| License expires < 7 days | Critical | Email + Telegram |
| Sync queue > 1000 | Warning | Dashboard yellow |
| No backup > 24h | Warning | Dashboard yellow |

## Security

- All sync traffic: HTTPS (TLS 1.3)
- License key: HMAC-signed, validated server-side
- SQLite backup: AES-256 encrypted before upload
- Cloud API: JWT + API key per store
- Store local API: JWT (local users), no inbound ports from internet
- Sync agent: outbound HTTPS only (no open inbound ports)

## Deployment

### Per-Store Installation
1. Windows installer (MSI via Inno Setup): bundles API + Sync Agent + SQLite
2. First boot wizard: license key, store name, local admin user
3. Auto-generates self-signed cert for local HTTPS

### Cloud Stack
- VPS: 4 vCPU, 8GB RAM, 100GB SSD (supports ~100 stores)
- Docker Compose: EDSF.Cloud.Api + PostgreSQL + Nginx (reverse proxy)
- Option: Azure App Service for auto-scaling

## Dashboard Central (you)

### Multi-Store View
- Map with store pins (RAG status)
- Aggregated revenue today/this month
- Stores with alerts (grouped by severity)
- License expiry countdown

### Per-Store Drill-Down
- Health metrics (CPU, disk, RAM graphs)
- Sync status (last sync, queue depth)
- Database size & last backup
- Active users
- Recent invoices

## Implementation Roadmap

| Phase | What | Timeline |
|---|---|---|
| 1 | Sync Agent + cloud sync API | 2 weeks |
| 2 | License system (generation + validation) | 1 week |
| 3 | Health monitor + alerting | 1 week |
| 4 | Multi-store dashboard (Blazor WASM) | 2 weeks |
| 5 | Backup system (encrypted upload + restore) | 1 week |
| 6 | Installer + deployment automation | 1 week |
| 7 | Hardening: [Authorize], password hashing, audit | 1 week |

## Questions / Next Steps

- SQLite local sync field: add `LastModifiedAt` + `IsSynced` columns to all entities
- Sync agent: stand-alone .NET console app or Windows Service
- Cloud API: reuse existing EDSF.Core models with tenantId column
