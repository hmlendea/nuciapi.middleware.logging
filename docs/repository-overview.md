# Repository Overview

## Purpose

NuciAPI.Middleware.Logging is a NuGet package providing ASP.NET Core middleware for structured HTTP request/response logging. It integrates with the NuciAPI ecosystem and uses NuciLog for structured, correlated logging.

## Scope

Single middleware component with supporting types:
- Request logging middleware
- Operation type (MyOperation)
- Log field keys (MyLogInfoKey)
- Pipeline registration extension

## Entry Points

### Consumer Registration

```csharp
app.UseNuciApiRequestLogging();
```

Called during `WebApplication` configuration in `Program.cs` or `Startup.cs`.

### Middleware Execution

`RequestLoggingMiddleware.InvokeAsync(HttpContext)` — invoked automatically by ASP.NET Core pipeline for each request.

## Principal Capabilities

1. **Request capture** — Method, path, query string, client IP, hostname
2. **Correlation headers** — ClientId, RequestId, Timestamp, HmacToken
3. **Response capture** — Status code, elapsed milliseconds
4. **Structured logging** — All fields via NuciLog LogInfo keys
5. **Error handling** — Logs exceptions, preserves pipeline propagation
6. **Success/failure classification** — 2xx = Success, else = Failure

## Module Organisation

```
NuciAPI.Middleware.Logging/
├── RequestLoggingMiddleware.cs    # Core middleware
├── MyOperation.cs                 # Operation type for HTTP requests
├── MyLogInfoKey.cs                # Structured log field keys
├── NuciApiMiddlewareExtensions.cs # Pipeline registration
└── AssemblyInfo.cs                # InternalsVisibleTo for tests
```

## Target Framework

- .NET 10.0 (net10.0)
- Requires ASP.NET Core via FrameworkReference

## Versioning

- Current: 1.0.2
- Semantic versioning
- Published to NuGet.org