# Architecture

## Purpose

NuciAPI.Middleware.Logging provides a single middleware component that intercepts HTTP requests in an ASP.NET Core pipeline, captures structured request/response metadata, and emits correlated log events via NuciLog.

## Scope

**Owns:**
- RequestLoggingMiddleware — the sole middleware implementation
- MyOperation — operation type for HTTP request logging
- MyLogInfoKey — structured log field keys
- NuciApiMiddlewareExtensions — registration extension method

**Does not own:**
- HTTP pipeline construction (consumer responsibility)
- Logger implementation (delegates to NuciLog.Core.ILogger)
- IP address resolution (delegates to NuciWeb.HTTP.NetworkUtils)
- Header name constants (consumes NuciWeb.HTTP.NuciApiHeaderNames)

## Component Relationships

```
Consumer Application
       │
       ▼
UseNuciApiRequestLogging() ──────► RequestLoggingMiddleware
       │                                    │
       │                                    ▼
       │                          NuciLog.Core.ILogger
       │                                    │
       │                                    ▼
       │                          NuciLog (Serilog sink)
       │
       ▼
NuciWeb.HTTP.NetworkUtils (IP → hostname)
NuciWeb.HTTP.NuciApiHeaderNames (header constants)
```

## Dependency Direction

```
NuciAPI.Middleware.Logging
    ├── NuciAPI.Middleware (base middleware class)
    ├── NuciLog.Core (logging abstraction)
    ├── NuciLog (logging implementation)
    └── NuciWeb.HTTP (IP resolution, header constants)
```

All dependencies flow inward — the middleware has no dependants within this repository.

## Runtime Topology

- Single middleware instance per pipeline registration
- Stateless — no instance fields beyond injected dependencies
- Async throughout — `InvokeAsync` returns `Task`
- One logger instance shared across requests (DI singleton/scoped)

## Major State

No persistent state. Per-request ephemeral data:
- Stopwatch for elapsed time measurement
- LogInfo collection built per request
- Response status code captured after `Next(context)`

## Integrations

| Integration | Purpose | Coupling |
|-------------|---------|----------|
| NuciLog.Core | Structured logging abstraction | Interface-only (ILogger) |
| NuciLog | Concrete logging implementation | Runtime only |
| NuciWeb.HTTP | Client IP resolution, header names | Static utility classes |
| NuciAPI.Middleware | Base middleware class | Inheritance |

## System-Wide Invariants

1. **Every request logs exactly one Started event** — before `Next(context)`
2. **Every request logs exactly one terminal event** — Success or Failure after `Next(context)`
3. **Exceptions are logged and rethrown** — pipeline error handling preserved
4. **Elapsed time measured** — Stopwatch starts before `Next`, stops after
5. **Structured fields consistent** — MyLogInfoKey enum used for all log properties