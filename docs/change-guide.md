# Change Guide

## Common Modifications

### Adding a Log Field

1. Add new property to `MyLogInfoKey.cs`:
   ```csharp
   internal static LogInfoKey NewField => new MyLogInfoKey(nameof(NewField));
   ```

2. Add field to initial `LogInfo[]` in `RequestLoggingMiddleware.cs`:
   ```csharp
   new LogInfo(MyLogInfoKey.NewField, GetNewFieldValue(context))
   ```

3. Update unit tests to verify field presence

4. Update documentation: `logging.md`, `components/request-logging-middleware.md`

### Changing Success/Failure Classification

Modify the status code check in `RequestLoggingMiddleware.cs`:
```csharp
if (statusCode >= 200 && statusCode < 300)  // Current: 2xx = Success
```

Consider: 3xx redirects, custom success ranges.

### Adding Header Support

1. Add header constant to `NuciWeb.HTTP.NuciApiHeaderNames` (upstream)
2. Add `LogInfoKey` to `MyLogInfoKey.cs`
3. Read header in `RequestLoggingMiddleware.cs`
4. Update tests and docs

### Changing Log Level

Currently fixed: Started/Success = Info, Failure = Error.

To make configurable:
1. Add configuration options class
2. Inject `IOptions<RequestLoggingOptions>`
3. Use configured levels in logger calls
4. Update registration extension

### Adding Correlation ID Generation

If clients don't provide `X-Request-Id`:
1. Generate in middleware (GUID/ULID)
2. Add to response header for client correlation
3. Log generated ID

## Safe Modification Patterns

### 1. Preserve Exception Rethrow

```csharp
// ALWAYS rethrow after logging
catch (Exception ex)
{
    logger.Error(...);
    throw;  // Never omit
}
```

### 2. Maintain Log Field Consistency

Started and terminal events must share base fields. Append only new fields.

### 3. Keep Middleware Stateless

No instance fields. All per-request data in local variables.

### 4. Test Both Paths

Every change must verify:
- Success path (2xx)
- Failure path (non-2xx)
- Exception path

## Breaking Changes

| Change | Impact | Version Bump |
|--------|--------|--------------|
| Remove log field | Consumers' log queries break | Major |
| Change field name | Log aggregation breaks | Major |
| Change operation name | Log filtering breaks | Major |
| Remove middleware | Pipeline breaks | Major |
| Add required header | Clients must update | Minor |
| Change log level | Alerting may change | Minor |

## Testing After Changes

```bash
dotnet test NuciAPI.Middleware.Logging.UnitTests/
```

Verify:
- All existing tests pass
- New tests for added behaviour
- Manual integration test in consumer app