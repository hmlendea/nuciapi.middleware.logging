# Repository Structure

## Source Tree

```
nuciapi.middleware.logging/
├── LICENSE
├── NuciAPI.Middleware.Logging.slnx
├── README.md
├── NuciAPI.Middleware.Logging/
│   ├── AssemblyInfo.cs
│   ├── MyLogInfoKey.cs
│   ├── MyOperation.cs
│   ├── NuciApiMiddlewareExtensions.cs
│   ├── NuciAPI.Middleware.Logging.csproj
│   └── RequestLoggingMiddleware.cs
├── NuciAPI.Middleware.Logging.UnitTests/
│   ├── NuciAPI.Middleware.Logging.UnitTests.csproj
│   └── RequestLoggingMiddlewareTests.cs
└── docs/
    └── (documentation)
```

## Module Organisation

### NuciAPI.Middleware.Logging (main project)

| File | Responsibility |
|------|---------------|
| `RequestLoggingMiddleware.cs` | Core middleware — captures request/response data, logs structured events |
| `MyOperation.cs` | Defines `HttpRequest` operation type for NuciLog |
| `MyLogInfoKey.cs` | Defines structured log field keys (Method, Path, StatusCode, etc.) |
| `NuciApiMiddlewareExtensions.cs` | `UseNuciApiRequestLogging()` extension for `IApplicationBuilder` |
| `AssemblyInfo.cs` | `InternalsVisibleTo` for unit test access |
| `NuciAPI.Middleware.Logging.csproj` | Project definition, dependencies, NuGet metadata |

### NuciAPI.Middleware.Logging.UnitTests (test project)

| File | Responsibility |
|------|---------------|
| `RequestLoggingMiddlewareTests.cs` | Tests for middleware logging behaviour |
| `NuciAPI.Middleware.Logging.UnitTests.csproj` | Test project definition |

## Build Artifacts

- `bin/` — Compiled output (Debug/net10.0)
- `obj/` — MSBuild intermediate files

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| NuciAPI.Middleware | 2.0.3 | Base middleware class |
| NuciLog | 1.2.1 | Logging implementation |
| NuciLog.Core | 3.1.0 | Logging abstraction (ILogger) |
| NuciWeb.HTTP | 1.7.3 | IP resolution, header constants |

## Target Framework

- `net10.0` — .NET 10.0 with ASP.NET Core