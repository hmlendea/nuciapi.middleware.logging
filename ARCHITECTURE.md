# Architecture

## Overview

NuciAPI.Middleware.Logging is a single-purpose ASP.NET Core middleware library for structured HTTP request/response logging. It integrates with the NuciAPI ecosystem and uses NuciLog for structured, correlated logging.

## Component Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                    Consumer Application                      │
│  ┌─────────────────────────────────────────────────────┐    │
│  │              ASP.NET Core Pipeline                   │    │
│  │  ┌─────────────────────────────────────────────┐    │    │
│  │  │         RequestLoggingMiddleware             │    │    │
│  │  │  ┌─────────────┐  ┌─────────────────────┐  │    │    │
│  │  │  │  Stopwatch  │  │   LogInfo[] (11)    │  │    │    │
│  │  │  └─────────────┘  └─────────────────────┘  │    │    │
│  │  └─────────────────────────────────────────────┘    │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      NuciLog.Core.ILogger                    │
│                              │                               │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                        NuciLog (Serilog)                     │
│                              │                               │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    Sinks (Console, File, Seq, ...)           │
└─────────────────────────────────────────────────────────────┘
```

## Key Components

| Component | Responsibility |
|-----------|---------------|
| `RequestLoggingMiddleware` | Core middleware — captures request/response, logs via NuciLog |
| `MyOperation` | Operation type (`HttpRequest`) for log categorisation |
| `MyLogInfoKey` | Structured log field keys (11 fields) |
| `NuciApiMiddlewareExtensions` | `UseNuciApiRequestLogging()` registration |

## Data Flow

1. Request enters middleware
2. Stopwatch starts
3. Request metadata captured (method, path, headers, IP, hostname)
4. `Started` log event emitted
5. `Next(context)` executes downstream pipeline
6. On exception: `Failure` log with exception, rethrow
7. On completion: Stopwatch stops, status code captured
8. `Success` (2xx) or `Failure` (non-2xx) log event emitted

## Dependencies

- **NuciAPI.Middleware** (2.0.3) — base middleware class
- **NuciLog** (1.2.1) — logging implementation
- **NuciLog.Core** (3.1.0) — logging abstraction
- **NuciWeb.HTTP** (1.7.3) — IP resolution, header constants
- **Microsoft.AspNetCore.App** — framework reference

## Design Principles

- Zero configuration — works out of the box
- Structured logging — all fields via typed keys
- Exception transparency — never swallows exceptions
- Stateless — safe for concurrent requests
- Minimal overhead — Stopwatch + log allocation only