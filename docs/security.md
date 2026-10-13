# Security

## Security Model

This middleware is a **logging component** — it does not enforce security policies. It captures and logs security-relevant headers for audit purposes.

## Threats and Mitigations

| Threat | Mitigation |
|--------|------------|
| Sensitive data in logs | No body logging; headers logged as-is (consumer must configure redaction) |
| Log injection | Structured logging via NuciLog — no string interpolation |
| DoS via logging | Async sinks (Serilog); no blocking in middleware |
| Correlation header spoofing | Headers logged only — not used for auth decisions |
| IP spoofing | `X-Forwarded-For` read but not trusted for security |

## Sensitive Data Handling

### Logged Headers (Potentially Sensitive)

| Header | Sensitivity | Recommendation |
|--------|-------------|----------------|
| `X-Hmac-Token` | High — authentication token | Configure NuciLog destructuring to mask |
| `X-Client-Id` | Medium — client identifier | Acceptable to log |
| `X-Request-Id` | Low — correlation ID | Acceptable to log |
| `X-Timestamp` | Low — request time | Acceptable to log |

### Redaction Configuration (Consumer Responsibility)

```csharp
// Example: Mask HmacToken in Serilog
options.Destructure.ByTransforming<LogInfo>(li =>
    li.Key == MyLogInfoKey.HmacToken
        ? new LogInfo(li.Key, "***REDACTED***")
        : li);
```

## Authentication/Authorisation

None — this middleware does not authenticate or authorise requests.

## Compliance

- No personal data processed beyond IP address and client-provided headers
- IP address considered personal data under GDPR — document in consumer's privacy policy
- No data persistence — logs managed by consumer's sink configuration

## Related

- Root [SECURITY.md](../SECURITY.md) — repository security policy
- [Logging](./logging.md) — redaction configuration details
- [Privacy](../PRIVACY.md) — data handling policy