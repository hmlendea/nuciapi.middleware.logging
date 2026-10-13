# Troubleshooting

## No Logs Appear

**Cause**: NuciLog not configured or minimum level too high.

**Fix**:
```csharp
builder.Services.AddNuciLog(options =>
{
    options.MinimumLevel = LogLevel.Information;  // Not Warning/Error
    options.WriteTo.Console();
});
```

## Middleware Not Executing

**Cause**: `UseNuciApiRequestLogging()` not called or called after `MapControllers()`.

**Fix**: Register early in pipeline:
```csharp
var app = builder.Build();
app.UseNuciApiRequestLogging();  // Before MapControllers
app.MapControllers();
```

## Missing Correlation Headers

**Cause**: Client not sending `X-Request-Id`, `X-Client-Id`, etc.

**Fix**: Ensure clients send headers. Middleware logs null if absent — not an error.

## Hostname Resolution Slow

**Cause**: `NetworkUtils.GetHostnames()` performs reverse DNS lookup.

**Mitigation**:
- Cache hostnames in NuciWeb.HTTP (upstream)
- Run DNS on separate thread (not currently)
- Accept latency for operational value

## Logs Too Verbose

**Cause**: Every request logs 2-3 events at Information/Error level.

**Fix**: Configure NuciLog level per namespace:
```csharp
options.MinimumLevel.Override("NuciAPI.Middleware.Logging", LogLevel.Warning);
```

## Exception Not Logged

**Cause**: Global exception handler catches before middleware logs.

**Fix**: Ensure middleware registered before `UseExceptionHandler`:
```csharp
app.UseNuciApiRequestLogging();
app.UseExceptionHandler("/error");  // After logging middleware
```

## Status Code Always 200

**Cause**: Downstream middleware sets status after logging middleware runs.

**Fix**: Middleware order matters. Logging middleware should be early but after status-setting middleware if needed.

## Unit Tests Fail After NuciLog Update

**Cause**: NuciLog.Core API changed (ILogger signature, OperationStatus enum).

**Fix**: Update test mocks to match new signatures. Check NuciLog changelog.

## Package Not Restoring

**Cause**: NuGet source missing or version conflict.

**Fix**:
```bash
dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org
dotnet restore
```

## Build Fails on net10.0

**Cause**: .NET 10 SDK not installed.

**Fix**: Install .NET 10 SDK or change TargetFramework in .csproj.