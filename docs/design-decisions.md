# Design Decisions

## 1. Single Middleware Class

**Decision**: One middleware class (`RequestLoggingMiddleware`) handling all logging.

**Rationale**:
- Simplicity — single responsibility, easy to understand
- No configuration surface needed
- All request/response logging in one place

**Alternatives Considered**:
- Separate request/response middlewares — adds complexity, splits correlated data
- Configurable field selection — YAGNI, fixed fields sufficient for NuciAPI ecosystem

## 2. Structured Logging via NuciLog

**Decision**: Use NuciLog.Core abstractions (`ILogger`, `Operation`, `LogInfoKey`) rather than `ILogger<T>` or Serilog directly.

**Rationale**:
- Consistent with NuciAPI ecosystem
- Structured fields via `LogInfoKey` ensure type-safe, consistent naming
- Operation types enable log querying/filtering
- Abstraction allows sink swapping without code changes

## 3. Fixed Log Fields

**Decision**: Hard-coded set of 11 log fields (MyLogInfoKey).

**Rationale**:
- Covers standard HTTP request metadata
- Correlation headers match NuciAPI conventions
- No configuration needed — zero-friction adoption

**Trade-off**: Cannot add custom fields without library modification.

## 4. Exception Rethrow After Logging

**Decision**: Catch, log, rethrow.

**Rationale**:
- Preserves ASP.NET Core pipeline error handling
- Allows `UseExceptionHandler` / `UseDeveloperExceptionPage` to work
- Logs full exception context before propagation

## 5. Success/Failure by Status Code

**Decision**: 2xx = Success, else = Failure (logged as Error level).

**Rationale**:
- Aligns with HTTP semantics
- Simple, no configuration
- Error level for non-2xx ensures visibility in log aggregation

**Trade-off**: 3xx redirects logged as Failure — may be noisy for API redirects.

## 6. No Correlation ID Generation

**Decision**: Only log `RequestId` if provided via header; don't generate one.

**Rationale**:
- NuciAPI clients expected to provide `X-Request-Id`
- Avoids opinionated ID format (GUID vs ULID vs custom)
- Downstream trace middleware can generate if needed

## 7. Hostname Resolution

**Decision**: Resolve hostname via `NetworkUtils.GetHostnames(ipAddress)`.

**Rationale**:
- Provides operational context (reverse DNS)
- Delegates to shared NuciWeb utility

**Trade-off**: DNS lookup adds latency; could be cached or made optional.

## 8. Internal Types

**Decision**: `MyOperation`, `MyLogInfoKey`, `RequestLoggingMiddleware` are `internal`.

**Rationale**:
- Implementation details not part of public API
- Consumers only interact via `UseNuciApiRequestLogging()`
- `InternalsVisibleTo` allows unit testing

## 9. Base Middleware Class

**Decision**: Inherit from `NuciApiMiddleware` (from NuciAPI.Middleware package).

**Rationale**:
- Shared middleware infrastructure in ecosystem
- Provides `Next` delegate and constructor pattern
- Consistent with other NuciAPI middlewares

## 10. No Async Disposal

**Decision**: Middleware implements no `IAsyncDisposable` or cleanup.

**Rationale**:
- Stateless — no resources to dispose
- Logger lifetime managed by DI container
- Stopwatch is local to `InvokeAsync`