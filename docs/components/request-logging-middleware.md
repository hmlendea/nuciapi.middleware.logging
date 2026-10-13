# RequestLoggingMiddleware Component

## Purpose

Intercepts every HTTP request in the ASP.NET Core pipeline, captures structured metadata, measures elapsed time, and emits correlated log events via NuciLog.

## Location

`NuciAPI.Middleware.Logging/RequestLoggingMiddleware.cs`

## Entry Point

`InvokeAsync(HttpContext context)` — called by ASP.NET Core pipeline for each request.

## Execution Flow

```
InvokeAsync(context)
    │
    ├─► Stopwatch.StartNew()
    │
    ├─► GetClientIpAddress(context) → string ipAddress
    │
    ├─► Build initial LogInfo[]:
    │       Method, Path, QueryString, IpAddress, Hostname,
    │       ClientId, RequestId, Timestamp, HmacToken
    │
    ├─► logger.Info(HttpRequest, Started, logInfos)
    │
    ├─► try
    │       await Next(context)
    │   catch (Exception ex)
    │       logger.Error(HttpRequest, Failure, ex, logInfos)
    │       throw
    │
    ├─► stopwatch.Stop()
    │
    ├─► Append StatusCode, ElapsedMilliseconds to logInfos
    │
    ├─► if (200 <= statusCode < 300)
    │       logger.Info(HttpRequest, Success, logInfos)
    │   else
    │       logger.Error(HttpRequest, Failure, logInfos)
```

## Data Transformations

| Input | Transformation | Output |
|-------|---------------|--------|
| `HttpContext.Request` | Extract method, path, query string | LogInfo fields |
| `HttpContext.Request.Headers` | Read NuciAPI headers | ClientId, RequestId, Timestamp, HmacToken |
| `X-Forwarded-For` / `RemoteIpAddress` | `NetworkUtils.GetClientIpAddress` | IpAddress |
| `IpAddress` | `NetworkUtils.GetHostnames` | Hostname |
| `Stopwatch.ElapsedMilliseconds` | After `Next()` completes | ElapsedMilliseconds |
| `HttpContext.Response.StatusCode` | After `Next()` completes | StatusCode |

## Side Effects

- Logs via `ILogger` (NuciLog) — 2 or 3 log events per request
- No mutation of `HttpContext` beyond reading
- No external I/O beyond logging

## Error Paths

| Scenario | Log Events | Exception Handling |
|----------|------------|-------------------|
| Downstream throws | Started → Failure (with exception) | Rethrown |
| Non-2xx status | Started → Failure | Not thrown |
| 2xx status | Started → Success | Not thrown |

## Dependencies

| Dependency | Usage |
|------------|-------|
| `NuciApiMiddleware` (base) | Provides `Next` delegate, constructor |
| `ILogger` (NuciLog.Core) | Structured logging |
| `NetworkUtils` (NuciWeb.HTTP) | IP address resolution, hostname lookup |
| `NuciApiHeaderNames` (NuciWeb.HTTP) | Header name constants |

## Configuration

None — behaviour fixed at compile time. Consumers configure logging via NuciLog.

## Tests

`NuciAPI.Middleware.Logging.UnitTests/RequestLoggingMiddlewareTests.cs`:
- `Given_SuccessfulRequest_When_InvokeAsync_Then_LogsStartedAndSuccess`
- `Given_DownstreamException_When_InvokeAsync_Then_LogsFailureAndRethrows`
- `Given_NonSuccessfulStatusCode_When_InvokeAsync_Then_LogsFailure`