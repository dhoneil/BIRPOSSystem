# BIRPOSSystem Agent Guide

This file is the living handoff for future agents working on BIRPOSSystem. Every future agent must update this file before finishing their turn, even for small changes, so the next agent can understand the app, what exists, what is planned, what changed most recently, and what should happen next.

## App Summary

BIRPOSSystem is a local-first point-of-sale and BIR compliance application for a small retail/cafe store. The solution is split into three projects:

- `Server`: ASP.NET Core host, Identity account pages, EF Core/SQLite persistence, API controllers, database seeding, static assets, and the root Razor app shell.
- `Client`: Blazor WebAssembly UI for authenticated operational screens, layout/navigation, page components, and typed HTTP services.
- `Shared`: DTOs, roles, and shared sale calculation logic used by both client and server.

The app uses AdminLTE, Bootstrap, Font Awesome, ASP.NET Core Identity, Entity Framework Core, and SQLite. All non-account app pages are authenticated. API endpoints under `/api` return `401` or `403` for unauthorized calls instead of redirecting to account pages.

## Current Feature Map

Implemented or partially implemented workflows:

- Authentication and accounts: Identity registration/login/manage pages with seeded roles and demo users.
- Dashboard: local store metrics for today's sales, transaction count, cash position, pending sync count, active shift, subscription status, recent sales, and low-stock alerts.
- POS register: product/category catalog, category/search filtering, favorites/recent tabs, cart quantity controls, manual discount, amount tendered, checkout, invoice number consumption, payment capture, inventory reduction, audit logging, and sync outbox queueing.
- Sales history: latest sales list with client-side search, status filter, date filters, count, and filtered total.
- Product catalog: product and category loading, product filtering, product create modal, SKU auto-generation, duplicate SKU handling through API conflict response, and stock/status display.
- Categories: category cards with product counts from the current product list.
- Brands: derived brand cards and product counts based on product SKU/category heuristics.
- Cash movements: live cash shift workspace with open shift, opening float, cash in, cash out, cash drop, payout, close shift, expected cash, variance, audit logging, and sync outbox queueing.
- BIR reports: same-day Z-reading preview with branch, terminal, invoice range, gross sales, discounts, VAT totals, net sales, void/refund placeholders, and print action.
- Store setup: seeded tenant, branch, terminal, receipt series, roles/permissions, and subscription setup overview.
- Sync center: local-first sync status and manual upload endpoint that marks pending outbox items as uploaded.

Scaffolded pages that still need real persistence/API flows:

- Suppliers
- Inventory
- Stock adjustments
- Barcode labels
- Purchase orders
- Expenses
- Customers
- User management
- Void/refund/reversal workflows
- Receipt printing and ESC/POS bridge
- Automatic cloud sync and conflict handling

## Data And Domain Notes

- `ApplicationDbContext` owns tenants, branches, POS terminals, product categories, products, inventory ledger entries, receipt series, cash shifts, cash movements, sales transactions/lines/payments, Z-readings, audit logs, and sync outbox items.
- `DatabaseSeeder` migrates the database, seeds roles/users, and creates the Northstar Market Cafe demo tenant/branch/terminal/products when no tenant exists.
- `SaleCalculator` applies line validation, manual discount allocation, VAT calculation, VAT-exempt totals, and money rounding.
- Sale creation is transactional: it requires an open cash shift, validates cart/payments, calculates totals server-side, consumes the next receipt number, updates inventory for tracked products, writes sales/audit/outbox records, and commits.
- Cash shift expected cash is opening cash plus cash sale impact and cash-in movements, minus cash out, drops, and payouts. Cash sale impact subtracts change due from cash tendered.
- The local SQLite database is `Server/Data/app.db`. Treat it as developer data unless the user explicitly asks for database resets or migrations.

## Near-Term Plan

1. Complete register operations: payment method selection, held orders, returns, voids, and receipts.
2. Make inventory first-class: suppliers, purchase orders, receiving, stock adjustments, inventory valuation, reorder suggestions, and barcode label generation.
3. Harden compliance: persisted Z-readings, X-readings, audit trail views, immutable posted sales, BIR export formats, receipt layout settings, and terminal series controls.
4. Expand administration: user/role management UI, branch/terminal CRUD, tenant subscription/license cache, permission policies, and store settings.
5. Build sync properly: automatic upload, retries, last-error display, cloud API integration, conflict resolution, and offline grace enforcement.
6. Improve quality: unit tests for sale calculations and API behavior, component tests for core flows, seed-data safety, migration review, and end-to-end smoke checks for POS checkout.

## What's Next

Next recommended build: receipt flow.

Scope:

- Add a receipt DTO/API endpoint for retrieving completed sale receipt details by sale ID or invoice number.
- Show a receipt modal/page immediately after POS checkout.
- Include store/BIR details, branch/terminal metadata, invoice number, cashier, item lines, discounts, VAT breakdown, payment methods, amount tendered, and change due.
- Add print support for the receipt view.
- Add sales history action to view/reprint a receipt.

Why this is next: cash shift management now supports the register day cycle, and receipts are the next user-visible POS requirement plus a foundation for BIR compliance and reprint workflows.

## Agent Update Protocol

Every future agent must update this file before finishing their work. Keep entries concise and factual.

Required update steps:

- Always add or update an `Agent Work Log` entry for the current turn, even if the change is documentation-only or verification-only.
- Update `Current Feature Map` when a feature changes status from scaffolded to implemented, or when behavior materially changes.
- Update `Data And Domain Notes` when models, migrations, seeding, calculations, or persistence rules change.
- Update `Near-Term Plan` when roadmap priorities are completed, removed, or superseded.
- Always update `What's Next` before finishing a task. It must name the single best next build, summarize scope, explain why it follows from the current state, and be phrased as a clear recommendation to the next agent.
- Do not remove prior log entries unless the user explicitly asks for cleanup.

Suggested log format:

```text
### YYYY-MM-DD - Short title
- Changed: ...
- Verified: ...
- Notes: ...
```

## Verification Commands

Use these from the repository root:

```powershell
dotnet build BIRPOSSystem.slnx
dotnet run --project Server\BIRPOSSystem.csproj
```

If the work touches sales math, add or run focused tests around `Shared/Sales/SaleCalculator.cs`. If the work changes migrations or seed data, verify against a disposable database before touching real local data.

## Agent Work Log

### 2026-08-08 - Initial agent guide and theme toggle

- Changed: Added this living app guide, documented implemented/scaffolded features, and defined the future-agent update protocol.
- Changed: Added a global light/dark theme toggle in the authenticated app header with persisted browser preference.
- Changed: Hardened dark-mode sidebar brand styles so the brand block and brand text stay dark/readable when AdminLTE `dark-mode` is active.
- Verified: `dotnet build BIRPOSSystem.slnx` passed with 0 warnings and 0 errors; local app started at `http://localhost:5297` and served `theme.js` with HTTP 200.
- Verified: Sidebar brand fix was reviewed by CSS inspection only; the app was not started for this follow-up.
- Notes: Existing uncommitted change in `Server/Data/app.db` was present before this work and was not modified intentionally.

### 2026-08-08 - Cash shift management

- Changed: Added `CashMovement` domain model, EF mapping, and `AddCashMovements` migration.
- Changed: Added cash shift DTOs/API endpoints for workspace state, opening shifts, adding drawer movements, and closing shifts.
- Changed: Replaced the static cash movements page with a live UI for drawer status, movement history, expected cash, close counts, and variance.
- Changed: Dashboard cash position now uses open-shift drawer math; sales creation now requires an open cash shift.
- Verified: `dotnet build BIRPOSSystem.slnx` passed with 0 warnings and 0 errors. The app was not started for this change.
- Notes: EF generated the migration successfully, with an EF tools/runtime patch-version warning (`10.0.6` tools vs `10.0.9` runtime).

### 2026-08-08 - What's Next protocol

- Changed: Added a dedicated `What's Next` section and made updating it mandatory for future agents.
- Changed: Strengthened the top-level and protocol wording so future agents always update `AGENTS.md` and always suggest the next best build before finishing.
- Verified: Documentation-only change; app was not started.
- Notes: Current next recommended build is receipt flow.
