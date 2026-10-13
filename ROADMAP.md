# Roadmap

## Version 1.1.0 (Planned)

### Features
- [ ] Optional hostname resolution (configurable)
- [ ] Correlation ID generation if not provided by client
- [ ] Structured exception detail configuration (include/exclude stack trace)

### Improvements
- [ ] Cache hostname lookups in NuciWeb.HTTP
- [ ] Add integration tests with real NuciLog sinks
- [ ] Benchmark and document performance characteristics

## Version 1.2.0 (Future)

### Features
- [ ] Request/response body logging (opt-in, with size limits)
- [ ] Log sampling support (fixed rate, adaptive)
- [ ] W3C TraceContext support (`traceparent` header)

### Improvements
- [ ] Configuration options class for middleware behaviour
- [ ] Per-endpoint log level override via attributes

## Version 2.0.0 (Breaking Changes)

### Potential Breaking Changes
- [ ] Change log field names (add namespace prefix)
- [ ] Make `MyOperation` and `MyLogInfoKey` public
- [ ] Change success/failure classification (3xx handling)
- [ ] Upgrade to .NET 11 when released

## Ongoing

- [ ] Keep NuciLog/NuciWeb dependencies updated
- [ ] Monitor .NET 10 release and align target framework
- [ ] Address ambiguities in [docs/ambiguities-and-open-questions.md](./docs/ambiguities-and-open-questions.md)

## Completed

- ✅ v1.0.0 — Initial release with core middleware
- ✅ v1.0.1 — Bug fixes
- ✅ v1.0.2 — Current version
- ✅ Documentation structure under `docs/`
- ✅ Root ARCHITECTURE.md, SECURITY.md