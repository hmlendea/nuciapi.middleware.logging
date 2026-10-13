# Concurrency and Scheduling

## Threading Model

### Request Processing

- Each request handled on its own thread (ASP.NET Core thread pool)
- Middleware is stateless — no shared mutable state
- No explicit locking required

### Async/Await

```csharp
public override async Task InvokeAsync(HttpContext context)
{
    // Synchronous setup
    Stopwatch stopwatch = Stopwatch.StartNew();
    // ...

    // Async downstream
    await Next(context);

    // Synchronous completion
    stopwatch.Stop();
    // ...
}
```

- `Next(context)` is awaited — yields thread during downstream execution
- No `Task.Run` or explicit threading
- No `ConfigureAwait(false)` needed (ASP.NET Core handles context)

## Scheduling

### No Background Work

- No `IHostedService`
- No `Timer`
- No `BackgroundService`
- No scheduled tasks

### No Custom Schedulers

- Uses default ASP.NET Core thread pool
- No `TaskScheduler` manipulation

## Concurrency Control

### No Locks

- No `lock`, `Monitor`, `SemaphoreSlim`
- No `ConcurrentDictionary` or thread-safe collections
- Stateless design eliminates need

### No Shared Resources

- No file handles
- No database connections
- No network connections
- Logger is thread-safe (NuciLog/Serilog responsibility)

## Parallelism

### Request-Level Parallelism

- ASP.NET Core processes requests in parallel
- Middleware scales horizontally with thread pool
- No bottleneck in middleware itself

### No Internal Parallelism

- Single request = single execution path
- No `Parallel.ForEach`, `Task.WhenAll`
- No batching

## Resource Contention

### Logger Contention

- `ILogger` calls may contend on sink (Serilog)
- Serilog uses async sinks with bounded queues
- Backpressure handled by Serilog

### DNS Resolution Contention

- `NetworkUtils.GetHostnames()` may perform DNS
- Multiple concurrent requests → multiple DNS lookups
- No internal caching (upstream concern)

## Performance Characteristics

| Metric | Expectation |
|--------|-------------|
| Thread allocation | 1 per request (ASP.NET Core) |
| Lock contention | None |
| Async yield points | 1 (`await Next(context)`) |
| Memory allocation | ~13 LogInfo objects + array per request |
| CPU overhead | Minimal (Stopwatch, field extraction) |

## Scaling Considerations

### Horizontal Scaling

- Stateless → scales with application instances
- No sticky sessions required
- No distributed state

### Vertical Scaling

- Limited by thread pool and logger throughput
- Logger async sinks prevent blocking
- DNS lookup may become bottleneck at high volume

## Concurrency Invariants

1. **No shared mutable state** — verified by code inspection
2. **No blocking calls** — only `await Next(context)`
3. **Thread-safe logger** — NuciLog/Serilog guarantee
4. **Request isolation** — each request independent

## Future Concurrency Changes

If adding state:
- Use `ConcurrentDictionary` for caches
- Consider `AsyncLocal` for request-scoped data
- Document locking strategy
- Add concurrency tests