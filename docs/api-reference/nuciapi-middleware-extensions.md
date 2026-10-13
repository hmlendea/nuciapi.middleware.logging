# NuciApiMiddlewareExtensions

Base path: N/A (middleware registration, not HTTP endpoints)

## UseNuciApiRequestLogging (Extension Method)

### Signature

```csharp
public static IApplicationBuilder UseNuciApiRequestLogging(
    this IApplicationBuilder app)
```

### Description

Registers `RequestLoggingMiddleware` in the ASP.NET Core pipeline. Must be called during application startup in `Program.cs` or `Startup.cs`.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `app` | `IApplicationBuilder` | The application builder |

### Returns

`IApplicationBuilder` — for method chaining

### Authentication/Authorisation

None — middleware registration only.

### Request

Not applicable — this is a pipeline registration method, not an HTTP endpoint.

### Response

Not applicable.

### Behaviour Summary

Adds `RequestLoggingMiddleware` to the pipeline. The middleware will:
1. Log a `Started` event for each incoming request with request metadata
2. Execute the next middleware in the pipeline
3. Log a `Success` event (2xx) or `Failure` event (non-2xx/exception) with response metadata

### Flow References

- [Request Logging Flow](../flows/request-logging-flow.md)

### Error Scenarios

| Condition | Result |
|-----------|--------|
| `app` is null | `ArgumentNullException` (from `UseMiddleware`) |
| Middleware not registered | No request logging occurs |

### Example Usage

```csharp
var app = builder.Build();
app.UseNuciApiRequestLogging();
app.MapControllers();
app.Run();
```

### Related

- [RequestLoggingMiddleware Component](../components/request-logging-middleware.md)
- [Logging Abstractions](../components/logging-abstractions.md)