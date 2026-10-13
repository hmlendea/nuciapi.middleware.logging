# Error Handling

## Error Taxonomy

| Category | Source | Handling |
|----------|--------|----------|
| Downstream exception | `Next(context)` throws | Logged as Failure with exception, rethrown |
| Non-2xx response | `context.Response.StatusCode` >= 300 or < 200 | Logged as Failure, not thrown |
| Null context | Pipeline bug | Not handled (would throw NullReferenceException) |
| Logger failure | `ILogger` throws | Not caught — propagates up |

## Handling Patterns

### Exception Path

```csharp
try
{
    await Next(context);
}
catch (Exception ex)
{
    logger.Error(MyOperation.HttpRequest, OperationStatus.Failure, ex, logInfos);
    throw;  // Preserve pipeline error handling
}
```

- Logs exception with full context
- Rethrows to allow global error handling middleware to respond
- Does not swallow or transform exceptions

### Non-Success Status Path

```csharp
if (statusCode >= 200 && statusCode < 300)
{
    logger.Info(MyOperation.HttpRequest, OperationStatus.Success, logInfos);
}
else
{
    logger.Error(MyOperation.HttpRequest, OperationStatus.Failure, logInfos);
}
```

- Classifies 2xx as Success, everything else as Failure
- Does not throw — pipeline continues normally
- Logs response metadata (status code, elapsed time)

## Recovery

No recovery logic in this middleware. Errors are:
1. Logged with full context
2. Propagated (exceptions) or recorded (status codes)
3. Handled by upstream middleware or ASP.NET Core defaults

## Error Response Format

This middleware does not produce HTTP responses. It only logs.

Consumer applications should add:
- `UseExceptionHandler` for error pages/API responses
- `UseStatusCodePages` for non-2xx responses

## Invariants

1. **No exception swallowed** — all exceptions rethrown after logging
2. **Exactly one terminal log event** — Success or Failure per request
3. **Started event always logged** — before any downstream execution
4. **Logger failures propagate** — if `ILogger` throws, pipeline fails