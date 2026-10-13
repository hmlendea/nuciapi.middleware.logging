# Ambiguities and Open Questions

## Unresolved Items

### 1. Hostname Resolution Performance

**Question**: Should `NetworkUtils.GetHostnames()` be cached or made optional?

**Context**: Reverse DNS lookup on every request adds latency. Current implementation calls it unconditionally.

**Options**:
- Add in-memory cache with TTL in NuciWeb.HTTP
- Make hostname resolution configurable (enabled/disabled)
- Move to background task (fire-and-forget)

**Decision Needed**: Performance vs. operational value trade-off.

### 2. 3xx Redirect Classification

**Question**: Should 3xx status codes be logged as Success or Failure?

**Current**: Failure (only 2xx = Success)

**Consideration**: API redirects (301, 302, 307, 308) may be expected behaviour.

**Options**:
- Keep as Failure (current)
- Treat 3xx as Success
- Make configurable

### 3. Correlation ID Generation

**Question**: Should middleware generate `RequestId` if client doesn't provide one?

**Current**: No generation — logs null

**Options**:
- Generate GUID/ULID and add to response header
- Require upstream trace middleware
- Document as consumer responsibility

### 4. Request/Response Body Logging

**Question**: Should body logging be added?

**Current**: No body logging

**Considerations**:
- Privacy (PII, secrets)
- Performance (buffering, allocation)
- Payload size limits
- Opt-in vs opt-out

### 5. Log Sampling

**Question**: Should high-volume endpoints support log sampling?

**Current**: All requests logged

**Options**:
- Fixed rate sampling (e.g., 10%)
- Adaptive sampling based on volume
- Per-endpoint configuration
- Not needed — let sink handle it

### 6. Structured Exception Details

**Question**: Current exception logging includes full stack trace. Should it be configurable?

**Considerations**:
- Security (stack traces in logs)
- Log volume
- Debugging value

### 7. Middleware Ordering Guidance

**Question**: Should documentation specify exact middleware ordering relative to other NuciAPI middlewares?

**Current**: "Register early" — vague

**Needed**: Explicit ordering diagram with ExceptionHandling, Security, etc.

### 8. Net10.0 Target Framework

**Question**: .NET 10 is not yet released (as of 2026-10-13). Is this a preview target?

**Action**: Verify target framework alignment with release schedule.

### 9. NuciLog Version Alignment

**Question**: NuciLog 1.2.1, NuciLog.Core 3.1.0 — major version mismatch. Is this intentional?

**Action**: Verify compatibility and document versioning strategy.

### 10. GPL-3.0 License Implications

**Question**: GPL-3.0-or-later requires derivative works to be GPL. Does this affect NuciAPI ecosystem adoption?

**Consideration**: All Nuci* packages use GPL-3.0 — consistent but restrictive for proprietary consumers.

## Known Gaps

| Area | Gap | Priority |
|------|-----|----------|
| Integration tests | None exist | Medium |
| Load testing | No performance benchmarks | Low |
| Header customisation | Not supported | Low |
| Body logging | Not supported | Medium |
| Sampling | Not implemented | Low |
| Correlation ID gen | Not implemented | Medium |
| W3C TraceContext | Not supported | Low |