# Quick Start

## Installation

```bash
dotnet add package NuciAPI.Middleware.Logging
```

## Registration

In `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure NuciLog (required)
builder.Services.AddNuciLog(options =>
{
    options.MinimumLevel = LogLevel.Information;
    options.WriteTo.Console();
});

var app = builder.Build();

// Register request logging middleware
app.UseNuciApiRequestLogging();

app.MapControllers();
app.Run();
```

## Required NuciLog Configuration

This middleware requires NuciLog to be configured. Minimum:

```csharp
builder.Services.AddNuciLog(options =>
{
    options.MinimumLevel = LogLevel.Information;
    options.WriteTo.Console();
});
```

## Expected Log Output

### Successful Request

```json
{
  "Operation": "HttpRequest",
  "Status": "Started",
  "Method": "POST",
  "Path": "/orders",
  "QueryString": "?page=1",
  "IpAddress": "203.0.113.50",
  "Hostname": "example.com",
  "ClientId": "client-123",
  "RequestId": "ABC123...",
  "Timestamp": "2026-10-13T10:00:00Z",
  "HmacToken": "token"
}
```

```json
{
  "Operation": "HttpRequest",
  "Status": "Success",
  "Method": "POST",
  "Path": "/orders",
  "QueryString": "?page=1",
  "IpAddress": "203.0.113.50",
  "Hostname": "example.com",
  "ClientId": "client-123",
  "RequestId": "ABC123...",
  "Timestamp": "2026-10-13T10:00:00Z",
  "HmacToken": "token",
  "StatusCode": 200,
  "ElapsedMilliseconds": 42
}
```

### Failed Request (500)

```json
{
  "Operation": "HttpRequest",
  "Status": "Failure",
  "Method": "POST",
  "Path": "/orders",
  "QueryString": "?page=1",
  "IpAddress": "203.0.113.50",
  "Hostname": "example.com",
  "ClientId": "client-123",
  "RequestId": "ABC123...",
  "Timestamp": "2026-10-13T10:00:00Z",
  "HmacToken": "token",
  "StatusCode": 500,
  "ElapsedMilliseconds": 15
}
```

### Exception

```json
{
  "Operation": "HttpRequest",
  "Status": "Failure",
  "Method": "POST",
  "Path": "/orders",
  "QueryString": "?page=1",
  "IpAddress": "203.0.113.50",
  "Hostname": "example.com",
  "ClientId": "client-123",
  "RequestId": "ABC123...",
  "Timestamp": "2026-10-13T10:00:00Z",
  "HmacToken": "token",
  "Exception": {
    "Type": "InvalidOperationException",
    "Message": "Database connection failed",
    "StackTrace": "..."
  }
}
```

## Client Headers

Send these headers for correlation:

| Header | Example | Purpose |
|--------|---------|---------|
| `X-Client-Id` | `client-123` | Client identification |
| `X-Request-Id` | `ABC123...` | Request correlation |
| `X-Timestamp` | `2026-10-13T10:00:00Z` | Request timestamp |
| `X-Hmac-Token` | `token` | Request authentication |

## Verification

1. Start application
2. Send request: `curl -H "X-Client-Id: test" http://localhost:5000/api/orders`
3. Check console for two log events (Started + Success/Failure)