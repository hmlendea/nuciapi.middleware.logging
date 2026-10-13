# Dependencies

## External Dependencies (NuGet)

| Package | Version | Purpose | License |
|---------|---------|---------|---------|
| NuciAPI.Middleware | 2.0.3 | Base middleware class (`NuciApiMiddleware`) | GPL-3.0-or-later |
| NuciLog | 1.2.1 | Logging implementation (Serilog wrapper) | GPL-3.0-or-later |
| NuciLog.Core | 3.1.0 | Logging abstraction (`ILogger`, `Operation`, `LogInfoKey`) | GPL-3.0-or-later |
| NuciWeb.HTTP | 1.7.3 | IP resolution (`NetworkUtils`), header constants (`NuciApiHeaderNames`) | GPL-3.0-or-later |

## Framework Dependencies

| Dependency | Version | Purpose |
|------------|---------|---------|
| Microsoft.AspNetCore.App | 10.0 (via FrameworkReference) | ASP.NET Core types (`HttpContext`, `IApplicationBuilder`, etc.) |
| System.Diagnostics | Built-in | `Stopwatch` for timing |
| System.Linq | Built-in | `Enumerable.Append` for log info collection |

## Internal Dependencies

| Component | Depends On |
|-----------|------------|
| `RequestLoggingMiddleware` | `NuciApiMiddleware` (base), `ILogger`, `NetworkUtils`, `NuciApiHeaderNames` |
| `MyOperation` | `NuciLog.Core.Operation` |
| `MyLogInfoKey` | `NuciLog.Core.LogInfoKey` |
| `NuciApiMiddlewareExtensions` | `RequestLoggingMiddleware` |

## Dependency Graph

```
NuciAPI.Middleware.Logging
├── NuciAPI.Middleware (2.0.3)
│   └── Microsoft.AspNetCore.App
├── NuciLog (1.2.1)
│   ├── NuciLog.Core (3.1.0)
│   └── Serilog (transitive)
├── NuciLog.Core (3.1.0)
└── NuciWeb.HTTP (1.7.3)
    └── Microsoft.AspNetCore.App (transitive)
```

## Version Constraints

- All Nuci* packages from same ecosystem — version alignment expected
- Target framework `net10.0` requires compatible dependency versions
- No explicit version ranges — fixed versions in `.csproj`

## Transitive Dependencies (Key)

Via NuciLog → Serilog:
- Serilog
- Serilog.Sinks.Console
- Serilog.Sinks.File (if used)

Via NuciWeb.HTTP:
- System.Net.Http
- System.Text.Json

## License Compatibility

All direct dependencies use GPL-3.0-or-later — compatible with this package's GPL-3.0-or-later license.