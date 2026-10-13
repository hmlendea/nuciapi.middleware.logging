# Frequently Asked Questions

## General

### What does this middleware log?

Every HTTP request: method, path, query string, client IP, hostname, correlation headers (ClientId, RequestId, Timestamp, HmacToken), response status code, and elapsed time.

### Does it log request/response bodies?

No. Only metadata. Body logging would require buffering and has privacy/performance implications.

### Can I customise the log fields?

Not currently. Fields are fixed in `MyLogInfoKey`. To add fields, modify the library (see [Change Guide](./change-guide.md)).

### Is it compatible with non-NuciAPI applications?

Yes. It only requires ASP.NET Core and NuciLog. The correlation headers follow NuciAPI conventions but are optional.

## Configuration

### How do I change log levels?

Configure NuciLog in your application:
```csharp
builder.Services.AddNuciLog(options =>
{
    options.MinimumLevel = LogLevel.Information;
    options.MinimumLevel.Override("NuciAPI.Middleware.Logging", LogLevel.Warning);
});
```

### Can I disable logging for specific paths?

Not built-in. Add a custom middleware before this one to short-circuit, or filter in your log sink.

### Does it support structured logging to Seq/Elasticsearch?

Yes — via NuciLog/Serilog sinks. Configure in your application.

## Correlation

### What if clients don't send X-Request-Id?

Logged as null. No ID generated. Add trace middleware upstream if needed.

### Can I use W3C TraceContext?

Not directly. This middleware doesn't read `traceparent`. Add trace middleware upstream.

## Performance

### Does hostname lookup add latency?

Yes, `NetworkUtils.GetHostnames()` does reverse DNS. Typically <50ms but can be higher. Consider caching upstream.

### What's the allocation per request?

~13 `LogInfo` objects + array + Stopwatch. Minimal for most applications.

## Errors

### Why are 3xx redirects logged as Failure?

By design: only 2xx = Success. Change in `RequestLoggingMiddleware.cs` if needed.

### Why isn't my exception logged?

Check middleware order — logging middleware must be before `UseExceptionHandler`.

## Testing

### How do I test my application with this middleware?

Unit test your controllers/handlers. The middleware is tested in this library. For integration tests, use `WebApplicationFactory` with real NuciLog.