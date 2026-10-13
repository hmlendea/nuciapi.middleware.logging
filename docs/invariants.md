# Invariants

## System-Wide Invariants

### Logging Invariants

1. **Every request produces exactly one Started event**
   - Emitted before `Next(context)` executes
   - Contains request metadata (method, path, headers, IP)

2. **Every request produces exactly one terminal event**
   - Success (Info level) for 2xx status codes
   - Failure (Error level) for non-2xx status codes or exceptions
   - Contains response metadata (status code, elapsed time)

3. **Exception propagation preserved**
   - Exceptions caught, logged with full context, then rethrown
   - No exception swallowing or transformation

4. **Log field consistency**
   - All fields use `MyLogInfoKey` constants
   - Field names match property names via `nameof()`
   - Same fields present in Started and terminal events (plus StatusCode/ElapsedMilliseconds)

### Execution Invariants

5. **Timing accuracy**
   - Stopwatch starts before any async work
   - Stops after `Next(context)` completes (success or exception)
   - ElapsedMilliseconds reflects total pipeline time

6. **Stateless execution**
   - No instance state mutated across requests
   - All per-request data in local variables
   - Safe for concurrent requests

7. **Header handling**
   - Missing headers logged as null (not omitted)
   - Header names from `NuciApiHeaderNames` constants
   - No header modification

### Integration Invariants

8. **Logger contract**
   - `ILogger.Info` called for Started and Success
   - `ILogger.Error` called for Failure (exception or status code)
   - Operation always `MyOperation.HttpRequest`
   - Status always `OperationStatus.Started`, `Success`, or `Failure`

9. **Base middleware contract**
   - Constructor accepts `RequestDelegate next, ILogger logger`
   - Calls `Next(context)` exactly once (or throws before)
   - Inherits from `NuciApiMiddleware`

## Component Contracts

### RequestLoggingMiddleware

| Invariant | Verification |
|-----------|--------------|
| `InvokeAsync` called once per request | ASP.NET Core pipeline guarantee |
| `Next(context)` awaited exactly once | Code inspection |
| Logger methods called with correct args | Unit tests |
| Exception rethrown | Unit test `Given_DownstreamException_When_InvokeAsync_Then_LogsFailureAndRethrows` |

### MyOperation

| Invariant | Verification |
|-----------|--------------|
| Single `HttpRequest` instance per access | `new MyOperation(nameof(HttpRequest))` each call |
| Name equals "HttpRequest" | `nameof(HttpRequest)` |

### MyLogInfoKey

| Invariant | Verification |
|-----------|--------------|
| Key name matches property name | `nameof(PropertyName)` |
| Immutable instances | New instance per property access |

## Consumer Invariants

Consumers can rely on:

1. **Registration** — `UseNuciApiRequestLogging()` adds middleware to pipeline
2. **Logging** — All requests logged with consistent structure
3. **Non-interference** — Middleware never modifies request/response
4. **Error transparency** — Exceptions propagate unchanged
5. **Performance** — Minimal overhead (Stopwatch, log allocation)

## Violation Consequences

| Invariant Violated | Consequence |
|--------------------|-------------|
| Started event missing | Incomplete request trace |
| Terminal event missing | No success/failure visibility |
| Exception swallowed | Silent failures, broken error handling |
| Logger contract broken | Log aggregation/query failures |
| State mutation | Race conditions, data corruption |