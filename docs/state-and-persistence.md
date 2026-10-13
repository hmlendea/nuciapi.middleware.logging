# State and Persistence

## State Management

### No Persistent State

This middleware maintains **no persistent state**. All state is ephemeral and request-scoped.

### Per-Request Ephemeral State

| State | Type | Lifetime |
|-------|------|----------|
| `Stopwatch` | `System.Diagnostics.Stopwatch` | Per request (local variable) |
| `logInfos` | `IEnumerable<LogInfo>` | Per request (local variable) |
| `statusCode` | `int` | Per request (captured after `Next`) |
| `ipAddress` | `string` | Per request (computed at start) |

### No Shared State

- No class-level fields
- No static collections
- No DI service state
- No file or database state

## Persistence

### No Data Persistence

This middleware does not write to:
- Files
- Databases
- Message queues
- External APIs

### Log Persistence (Consumer Responsibility)

Logs are persisted by the consumer's NuciLog configuration:

```csharp
builder.Services.AddNuciLog(options =>
{
    options.WriteTo.File("logs/requests-.log", rollingInterval: RollingInterval.Day);
    options.WriteTo.Seq("http://localhost:5341");
});
```

## State Transitions

### Request Lifecycle

```
Idle → InvokeAsync(context) → Started log
    ↓
    await Next(context)
    ↓
    Terminal log (Success/Failure)
    ↓
Idle
```

### No State Between Requests

Each request starts from `Idle` state. No carry-over of:
- Timing data
- Log fields
- Correlation IDs (unless provided by client)

## Concurrency

### Thread Safety

- Stateless → inherently thread-safe
- No shared mutable state
- Safe for concurrent requests

### Async Safety

- All operations awaitable
- No blocking calls
- `Stopwatch` is thread-local

## State Invariants

1. **No cross-request state** — each request independent
2. **No mutation of `HttpContext`** — only reads
3. **No external resource state** — no files, DB, network
4. **Deterministic timing** — `Stopwatch` measures actual elapsed time

## Consumer State Dependencies

### Logger State

Consumer configures `ILogger` (NuciLog) which may have:
- Sink state (file rotation, network connections)
- Buffering state
- Internal caches

### IP Resolution State

`NetworkUtils.GetClientIpAddress()` may have:
- DNS cache (in NuciWeb.HTTP)
- Connection pool state

## State Management Best Practices

### For Consumers

1. **Configure log retention** via NuciLog sinks
2. **Monitor sink health** (disk space, network connectivity)
3. **Consider hostname lookup cost** if high volume

### For Future Modifications

1. **Avoid adding instance fields** unless necessary
2. **Prefer local variables** over class state
3. **Document any state changes** in `change-guide.md`

## State Change Impact

### Adding State

- Must be thread-safe
- Must be documented
- Must be tested for race conditions
- Must have clear lifecycle (when created/destroyed)

### Removing State

- Backward compatible if state was internal
- May break consumers relying on side effects
- Must update documentation