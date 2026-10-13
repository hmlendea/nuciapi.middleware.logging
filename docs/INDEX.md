# NuciAPI.Middleware.Logging Documentation

## Repository Summary

NuciAPI.Middleware.Logging is an ASP.NET Core middleware library that provides structured HTTP request/response logging for NuciAPI applications. It captures request metadata (method, path, query string, client IP, headers), measures elapsed time, and logs structured events using NuciLog with correlation IDs for distributed tracing.

## Root Document Map

- [ARCHITECTURE.md](../ARCHITECTURE.md) — High-level architecture
- [SECURITY.md](../SECURITY.md) — Security model and mitigations
- [ARCHITECTURE.md](../ARCHITECTURE.md) — High-level architecture
- [SECURITY.md](../SECURITY.md) — Security model and mitigations
- [ROADMAP.md](../ROADMAP.md) — Future plans
- [PRIVACY.md](../PRIVACY.md) — Data handling policy
- [LICENSE](../LICENSE) — GPL-3.0-or-later license

## Documentation Catalogue

### Architecture & Design
- [architecture.md](./architecture.md) — Component relationships and design rationale
- [design-decisions.md](./design-decisions.md) — Key architectural choices
- [invariants.md](./invariants.md) — System-wide invariants and contracts
- [security.md](./security.md) — Security model, threats, mitigations

### Components
- [components/request-logging-middleware.md](./components/request-logging-middleware.md) — Core middleware implementation
- [components/logging-abstractions.md](./components/logging-abstractions.md) — MyOperation and MyLogInfoKey

### Flows
- [flows/request-logging-flow.md](./flows/request-logging-flow.md) — End-to-end request logging execution

### API Reference
- [api-reference/INDEX.md](./api-reference/INDEX.md) — API reference index
- [api-reference/nuciapi-middleware-extensions.md](./api-reference/nuciapi-middleware-extensions.md) — Extension methods

### Operations
- [configuration.md](./configuration.md) — Configuration schema and precedence
- [logging.md](./logging.md) — Logging framework, levels, structured fields, correlation
- [error-handling.md](./error-handling.md) — Error taxonomy and handling patterns
- [testing.md](./testing.md) — Test strategy and organisation
- [build-and-deployment.md](./build-and-deployment.md) — Build pipeline and deployment
- [concurrency-and-scheduling.md](./concurrency-and-scheduling.md) — Threading, async, scheduling
- [state-and-persistence.md](./state-and-persistence.md) — State management, caches, persistence
- [integrations.md](./integrations.md) — External system integrations

### Reference
- [dependencies.md](./dependencies.md) — External and internal dependencies
- [repository-structure.md](./repository-structure.md) — Source tree layout
- [repository-overview.md](./repository-overview.md) — Purpose, scope, entry points
- [quick-start.md](./quick-start.md) — Getting started guide
- [change-guide.md](./change-guide.md) — Safe modification patterns
- [troubleshooting.md](./troubleshooting.md) — Common issues and solutions
- [faq.md](./faq.md) — Frequently asked questions
- [ambiguities-and-open-questions.md](./ambiguities-and-open-questions.md) — Unresolved items

## Navigation Aids

- **Start here** (newcomers): [quick-start.md](./quick-start.md) → [repository-overview.md](./repository-overview.md) → [architecture.md](./architecture.md)
- **Deep dive** (component owners): [components/request-logging-middleware.md](./components/request-logging-middleware.md) → [flows/request-logging-flow.md](./flows/request-logging-flow.md)
- **Flows** (debuggers): [flows/request-logging-flow.md](./flows/request-logging-flow.md) → [logging.md](./logging.md)

## Maintenance Metadata

- Last reviewed: 2026-10-13
- Owner: Horațiu Mlendea
- Coverage: Complete for v1.0.2