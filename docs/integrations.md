# Integrations

## External System Integrations

### NuciLog / Serilog

- **Purpose**: Structured logging implementation
- **Coupling**: Runtime only via `ILogger` interface
- **Configuration**: Consumer responsibility
- **Sinks**: Console, File, Seq, Elasticsearch, Datadog, etc.

### NuciWeb.HTTP

- **Purpose**: Client IP resolution, header name constants
- **Coupling**: Static utility classes (`NetworkUtils`, `NuciApiHeaderNames`)
- **Version**: 1.7.3

### NuciAPI.Middleware

- **Purpose**: Base middleware class (`NuciApiMiddleware`)
- **Coupling**: Inheritance
- **Version**: 2.0.3

## Internal Integrations

### MyOperation ↔ NuciLog.Core.Operation

- `MyOperation.HttpRequest` passed to all logger calls
- Enables log filtering by operation type

### MyLogInfoKey ↔ NuciLog.Core.LogInfoKey

- All log fields use `MyLogInfoKey` constants
- Ensures consistent field names across events

### RequestLoggingMiddleware ↔ ILogger

- Constructor injection
- Calls `Info()` for Started/Success
- Calls `Error()` for Failure (exception or status code)

## Integration Points for Consumers

### Pipeline Registration

```csharp
app.UseNuciApiRequestLogging();
```

### Logger Configuration

```csharp
builder.Services.AddNuciLog(options => { ... });
```

### Correlation Headers (Client → Middleware)

| Header | Constant | Logged As |
|--------|----------|-----------|
| `X-Client-Id` | `NuciApiHeaderNames.ClientId` | `ClientId` |
| `X-Request-Id` | `NuciApiHeaderNames.RequestId` | `RequestId` |
| `X-Timestamp` | `NuciApiHeaderNames.Timestamp` | `Timestamp` |
| `X-Hmac-Token` | `NuciApiHeaderNames.HmacToken` | `HmacToken` |

## No Direct Integrations

This middleware does not integrate with:
- Databases
- Message queues
- External APIs
- Authentication providers
- Metrics systems (Prometheus, etc.)
- Tracing systems (OpenTelemetry, etc.) — correlation via headers only