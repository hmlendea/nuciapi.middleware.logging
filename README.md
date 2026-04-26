[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/fund.html)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/nuciapi.middleware.logging)](https://github.com/hmlendea/nuciapi.middleware.logging/releases/latest)
[![Build Status](https://github.com/hmlendea/nuciapi.middleware.logging/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nuciapi.middleware.logging/actions/workflows/dotnet.yml)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://gnu.org/licenses/gpl-3.0)

# NuciAPI.Middleware.Logging

Logging middleware for ASP.NET Core applications built with NuciAPI.

It records request lifecycle events (start and finish), request metadata, client identity headers, execution duration, and final status code through `NuciLog`.

## Installation

[![Get it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciAPI.Middleware.Logging)

### .NET CLI

```bash
dotnet add package NuciAPI.Middleware.Logging
```

### Package Manager

```powershell
Install-Package NuciAPI.Middleware.Logging
```

## Requirements

- .NET SDK/runtime with support for `net10.0`
- ASP.NET Core (`Microsoft.AspNetCore.App`)
- A configured `NuciLog.Core.ILogger` service in DI

## What This Package Includes

- Request logging middleware: `RequestLoggingMiddleware`
- ASP.NET Core extension method: `UseNuciApiRequestLogging()`

The middleware logs:

- request start (`OperationStatus.Started`)
- request success (`OperationStatus.Success`) for `2xx` status codes
- request failure (`OperationStatus.Failure`) for non-`2xx` responses
- request failure with exception details when downstream middleware throws

Captured log fields:

- `Method`
- `Path`
- `QueryString`
- `IpAddress`
- `Hostname`
- `ClientId`
- `RequestId`
- `Timestamp`
- `HmacToken`
- `StatusCode` (completion log)
- `ElapsedMilliseconds` (completion log)

## Quick Start

1. Register your `NuciLog` logger in dependency injection.
2. Add the middleware to the ASP.NET Core pipeline.

```csharp
using NuciAPI.Middleware.Logging;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Register your NuciLog logger implementation in DI.
// Example:
// builder.Services.AddSingleton<ILogger, MyLogger>();

WebApplication app = builder.Build();

app.UseNuciApiRequestLogging();

app.MapControllers();

app.Run();
```

Recommended placement: register request logging early in the middleware pipeline so request start/failure is consistently captured.

## Behavior Details

- The middleware logs a `Started` event before calling the next middleware.
- If an exception is thrown downstream:
	- an error log is emitted with exception details
	- the exception is rethrown
- After downstream execution, the middleware logs:
	- `Success` for `2xx` responses
	- `Failure` for all other status codes
- Execution time is measured with a `Stopwatch` and added as `ElapsedMilliseconds`.

## Testing

Unit tests cover:

- successful request logging (`Started` + `Success`)
- exception path logging (`Started` + `Failure`, then rethrow)
- non-success status code logging (`Started` + `Failure`)

Run tests with:

```bash
dotnet test NuciAPI.Middleware.Logging.sln
```

## Development

### Build

```bash
dotnet build NuciAPI.Middleware.Logging.sln
```

### Test

```bash
dotnet test
```

### Pack

```bash
dotnet pack -c Release
```

## Contributing

Contributions are welcome.

Please:

- keep the changes cross-platform
- keep the pull requests focused and consistent with the existing style
- update the documentation when the behaviour changes
- add or update the tests for any new behaviour

## Related Projects

- [NuciAPI.Middleware](https://github.com/hmlendea/nuciapi.middleware)
- [NuciAPI.Middleware.ExceptionHandling](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling)
- [NuciAPI.Middleware.Logging](https://github.com/hmlendea/nuciapi.middleware.logging)
- [NuciAPI.Middleware.Security](https://github.com/hmlendea/nuciapi.middleware.security)

## License

Licensed under the GNU General Public License v3.0 or later.
See [LICENSE](./LICENSE) for details.
