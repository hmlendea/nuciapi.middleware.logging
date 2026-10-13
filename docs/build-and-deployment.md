# Build and Deployment

## Build Pipeline

### Local Build

```bash
dotnet build NuciAPI.Middleware.Logging.slnx
```

### Release Build

```bash
dotnet build NuciAPI.Middleware.Logging.slnx -c Release
```

### Pack

```bash
dotnet pack NuciAPI.Middleware.Logging/NuciAPI.Middleware.Logging.csproj -c Release -o ./artifacts
```

## CI/CD

### GitHub Actions

No workflow files present in repository. Typical workflow would include:
1. Build
2. Test
3. Pack
4. Publish to NuGet.org (on tag)

### Versioning

- Version in `.csproj`: `<Version>1.0.2</Version>`
- Semantic versioning
- Updated manually before release

## Deployment

### NuGet Package

- Package ID: `NuciAPI.Middleware.Logging`
- Published to: NuGet.org
- License: GPL-3.0-or-later
- Repository: https://github.com/hmlendea/nuciapi.middleware.logging

### Installation (Consumer)

```bash
dotnet add package NuciAPI.Middleware.Logging
```

## Target Framework

- `net10.0` — .NET 10.0
- Requires `Microsoft.AspNetCore.App` FrameworkReference

## Dependencies

| Package | Version | Type |
|---------|---------|------|
| NuciAPI.Middleware | 2.0.3 | NuGet |
| NuciLog | 1.2.1 | NuGet |
| NuciLog.Core | 3.1.0 | NuGet |
| NuciWeb.HTTP | 1.7.3 | NuGet |

## Build Output

```
bin/Release/net10.0/
├── NuciAPI.Middleware.Logging.dll
├── NuciAPI.Middleware.Logging.pdb
└── NuciAPI.Middleware.Logging.xml (if GenerateDocumentationFile=true)
```

## Signing

Not configured — no strong name or certificate signing.

## Reproducible Builds

Not explicitly configured. Deterministic builds depend on SDK defaults.