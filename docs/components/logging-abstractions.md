# Logging Abstractions

## MyOperation

### Purpose

Defines the operation type for HTTP request logging in NuciLog. Provides a single operation: `HttpRequest`.

### Location

`NuciAPI.Middleware.Logging/MyOperation.cs`

### Structure

```csharp
internal sealed class MyOperation : Operation
{
    MyOperation(string name) : base(name) { }

    internal static Operation HttpRequest => new MyOperation(nameof(HttpRequest));
}
```

### Usage

Passed as first argument to `ILogger.Info()` and `ILogger.Error()` to categorise log events.

### Contract

- Inherits from `NuciLog.Core.Operation`
- Single static instance per operation name
- Name: "HttpRequest"

## MyLogInfoKey

### Purpose

Defines structured log field keys for HTTP request/response metadata. Ensures consistent field names across all log events.

### Location

`NuciAPI.Middleware.Logging/MyLogInfoKey.cs`

### Fields

| Key | Type | Description | Source |
|-----|------|-------------|--------|
| `Method` | string | HTTP method (GET, POST, etc.) | `context.Request.Method` |
| `Path` | string | Request path | `context.Request.Path` |
| `QueryString` | string | Raw query string | `context.Request.QueryString.ToString()` |
| `IpAddress` | string | Client IP address | `NetworkUtils.GetClientIpAddress()` |
| `Hostname` | string | Hostname(s) for IP | `NetworkUtils.GetHostnames()` |
| `ClientId` | string? | `X-Client-Id` header | `NuciApiHeaderNames.ClientId` |
| `RequestId` | string? | `X-Request-Id` header | `NuciApiHeaderNames.RequestId` |
| `Timestamp` | string? | `X-Timestamp` header | `NuciApiHeaderNames.Timestamp` |
| `HmacToken` | string? | `X-Hmac-Token` header | `NuciApiHeaderNames.HmacToken` |
| `StatusCode` | int | HTTP response status | `context.Response.StatusCode` |
| `ElapsedMilliseconds` | long | Request duration | `Stopwatch.ElapsedMilliseconds` |

### Structure

```csharp
internal sealed class MyLogInfoKey : LogInfoKey
{
    MyLogInfoKey(string name) : base(name) { }

    internal static LogInfoKey ClientId => new MyLogInfoKey(nameof(ClientId));
    internal static LogInfoKey ElapsedMilliseconds => new MyLogInfoKey(nameof(ElapsedMilliseconds));
    internal static LogInfoKey Hostname => new MyLogInfoKey(nameof(Hostname));
    internal static LogInfoKey HmacToken => new MyLogInfoKey(nameof(HmacToken));
    internal static LogInfoKey IpAddress => new MyLogInfoKey(nameof(IpAddress));
    internal static LogInfoKey Method => new MyLogInfoKey(nameof(Method));
    internal static LogInfoKey Path => new MyLogInfoKey(nameof(Path));
    internal static LogInfoKey QueryString => new MyLogInfoKey(nameof(QueryString));
    internal static LogInfoKey RequestId => new MyLogInfoKey(nameof(RequestId));
    internal static LogInfoKey StatusCode => new MyLogInfoKey(nameof(StatusCode));
    internal static LogInfoKey Timestamp => new MyLogInfoKey(nameof(Timestamp));
}
```

### Usage

Each key wraps a string name and is used to construct `LogInfo` instances:

```csharp
new LogInfo(MyLogInfoKey.Method, context.Request.Method)
```

### Contract

- Inherits from `NuciLog.Core.LogInfoKey`
- Immutable — new instance per static property access
- Name matches property name via `nameof()`

## Integration with NuciLog

Both types integrate with NuciLog.Core:
- `Operation` — base class for operation categorisation
- `LogInfoKey` — base class for structured field keys
- `LogInfo` — key-value pair for log entries
- `ILogger.Info(Error)` — logging methods accepting operation, status, LogInfo[]