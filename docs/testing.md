# Testing

## Test Strategy

- **Framework**: NUnit 3.x with Moq
- **Project**: `NuciAPI.Middleware.Logging.UnitTests`
- **Coverage**: Core middleware behaviour (3 tests)
- **Approach**: Unit tests with mocked `ILogger` and `HttpContext`

## Test Organisation

```
NuciAPI.Middleware.Logging.UnitTests/
└── RequestLoggingMiddlewareTests.cs
    ├── Given_SuccessfulRequest_When_InvokeAsync_Then_LogsStartedAndSuccess
    ├── Given_DownstreamException_When_InvokeAsync_Then_LogsFailureAndRethrows
    └── Given_NonSuccessfulStatusCode_When_InvokeAsync_Then_LogsFailure
```

## Test Cases

### Successful Request

```csharp
[Test]
public async Task Given_SuccessfulRequest_When_InvokeAsync_Then_LogsStartedAndSuccess()
{
    Mock<ILogger> logger = new();
    RequestLoggingMiddleware middleware = new(_ => Task.CompletedTask, logger.Object);
    DefaultHttpContext context = CreateContext();

    await middleware.InvokeAsync(context);

    Assert.Multiple(() =>
    {
        Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Info)), Is.EqualTo(2));
        Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Error)), Is.EqualTo(0));
    });
}
```

**Verifies**: 2 Info calls (Started + Success), 0 Error calls

### Downstream Exception

```csharp
[Test]
public void Given_DownstreamException_When_InvokeAsync_Then_LogsFailureAndRethrows()
{
    Mock<ILogger> logger = new();
    RequestLoggingMiddleware middleware = new(_ => throw new InvalidOperationException("boom"), logger.Object);
    DefaultHttpContext context = CreateContext();

    Assert.ThrowsAsync<InvalidOperationException>(async () => await middleware.InvokeAsync(context));

    Assert.Multiple(() =>
    {
        Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Info)), Is.EqualTo(1));
        Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Error)), Is.EqualTo(1));
    });
}
```

**Verifies**: 1 Info (Started), 1 Error (Failure with exception), exception rethrown

### Non-Success Status Code

```csharp
[Test]
public async Task Given_NonSuccessfulStatusCode_When_InvokeAsync_Then_LogsFailure()
{
    Mock<ILogger> logger = new();
    RequestLoggingMiddleware middleware = new(context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return Task.CompletedTask;
    }, logger.Object);
    DefaultHttpContext context = CreateContext();

    await middleware.InvokeAsync(context);

    Assert.Multiple(() =>
    {
        Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Info)), Is.EqualTo(1));
        Assert.That(logger.Invocations.Count(x => x.Method.Name == nameof(ILogger.Error)), Is.EqualTo(1));
    });
}
```

**Verifies**: 1 Info (Started), 1 Error (Failure), no exception

## Test Context Factory

```csharp
private static DefaultHttpContext CreateContext()
{
    DefaultHttpContext context = new();
    context.Request.Method = HttpMethods.Post;
    context.Request.Path = "/orders";
    context.Request.QueryString = new QueryString("?page=1");
    context.Request.Headers[NuciApiHeaderNames.ClientId] = "client-123";
    context.Request.Headers[NuciApiHeaderNames.RequestId] = Guid.NewGuid().ToString().ToUpperInvariant();
    context.Request.Headers[NuciApiHeaderNames.Timestamp] = DateTimeOffset.UtcNow.ToString("O");
    context.Request.Headers[NuciApiHeaderNames.HmacToken] = "token";
    context.Request.Headers["X-Forwarded-For"] = "203.0.113.50";
    return context;
}
```

## Running Tests

```bash
dotnet test NuciAPI.Middleware.Logging.UnitTests/NuciAPI.Middleware.Logging.UnitTests.csproj
```

## Coverage Gaps

| Area | Covered? | Notes |
|------|----------|-------|
| Successful request | Yes | 2xx path |
| Exception path | Yes | Exception logged and rethrown |
| Non-2xx status | Yes | 500 tested |
| Header variations | Partial | Only happy path headers tested |
| Missing headers | No | Null/empty header handling not tested |
| IP resolution | No | `NetworkUtils` not mocked |
| Hostname lookup | No | `NetworkUtils.GetHostnames` not tested |
| Concurrent requests | No | Thread safety not verified |

## Integration Tests

None — this library has no integration tests. Consumers should test pipeline integration in their applications.