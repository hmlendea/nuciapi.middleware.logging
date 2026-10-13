# Configuration

## Configuration Schema

This library has **no configuration schema**. All behaviour is fixed at compile time.

## Configuration Sources

| Source | Used? | Purpose |
|--------|-------|---------|
| `appsettings.json` | No | — |
| Environment variables | No | — |
| Command line | No | — |
| DI container | Yes | `ILogger` injection |

## Precedence

Not applicable — no configuration.

## Runtime Configuration Changes

Not supported — behaviour cannot be changed at runtime.

## Consumer Configuration

Consumers configure logging via **NuciLog**, not this library:

```csharp
// Example NuciLog configuration (consumer responsibility)
builder.Services.AddNuciLog(options =>
{
    options.MinimumLevel = LogLevel.Information;
    options.WriteTo.Console();
    options.WriteTo.File("logs/requests-.log", rollingInterval: RollingInterval.Day);
});
```

## Header Names

Header names are constants from `NuciWeb.HTTP.NuciApiHeaderNames`:
- `ClientId` → `X-Client-Id`
- `RequestId` → `X-Request-Id`
- `Timestamp` → `X-Timestamp`
- `HmacToken` → `X-Hmac-Token`

These cannot be customised.