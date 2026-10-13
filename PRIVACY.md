# Privacy Policy

## Data Collection

This middleware library **does not collect, store, or transmit any personal data** directly. It operates entirely within the consumer's application process.

## Data Processed

The middleware processes the following data **in-memory only** during request handling:

| Data | Source | Purpose | Retention |
|------|--------|---------|-----------|
| HTTP Method | `HttpContext.Request.Method` | Request identification | In-memory, request scope |
| Request Path | `HttpContext.Request.Path` | Request identification | In-memory, request scope |
| Query String | `HttpContext.Request.QueryString` | Request context | In-memory, request scope |
| Client IP Address | `HttpContext.Connection.RemoteIpAddress` / `X-Forwarded-For` | Client identification | In-memory, request scope |
| Hostname | Reverse DNS of IP | Operational context | In-memory, request scope |
| `X-Client-Id` header | Client request | Client correlation | In-memory, request scope |
| `X-Request-Id` header | Client request | Request correlation | In-memory, request scope |
| `X-Timestamp` header | Client request | Request timing | In-memory, request scope |
| `X-Hmac-Token` header | Client request | Request authentication | In-memory, request scope |
| Response Status Code | `HttpContext.Response.StatusCode` | Outcome classification | In-memory, request scope |
| Elapsed Time | `Stopwatch` | Performance measurement | In-memory, request scope |

## Data Flow

```
HTTP Request → Middleware (in-memory processing) → NuciLog → Consumer's Sinks
```

**This library does not:**
- Write to files, databases, or external services
- Send data to third parties
- Persist data beyond the request lifetime
- Track users across requests (unless client provides correlation headers)

## Log Persistence

Log persistence is **entirely controlled by the consumer** via NuciLog configuration:

```csharp
builder.Services.AddNuciLog(options =>
{
    options.WriteTo.Console();                    // Ephemeral
    options.WriteTo.File("logs/requests-.log");   // Consumer-controlled retention
    options.WriteTo.Seq("http://seq:5341");       // Consumer-controlled retention
});
```

Consumers are responsible for:
- Log retention policies
- Access controls on log storage
- Compliance with applicable regulations (GDPR, CCPA, etc.)

## Personal Data Considerations

### IP Addresses

- IP addresses are considered personal data under GDPR
- Processed only for request correlation and operational logging
- Not used for profiling or tracking
- Consumers should document IP logging in their privacy policies

### Client-Provided Headers

- `X-Client-Id`, `X-Request-Id`, `X-Timestamp`, `X-Hmac-Token` are client-controlled
- Logged as-is — consumers should validate/sanitize if needed
- `X-Hmac-Token` may be sensitive — consumers should configure redaction

## Data Subject Rights

This library provides no direct mechanism for data subject requests. Consumers must implement:
- Access to logs containing personal data
- Rectification/deletion of log data
- Export of log data

## Third-Party Processors

None — this library has no third-party data processors.

## Security

- Structured logging prevents log injection
- No sensitive data in exception messages (only in logged headers)
- Consumer responsible for log transport encryption (TLS to Seq, etc.)

## Changes to This Policy

Updates to this policy will be reflected in the repository's `PRIVACY.md` and noted in release notes.

## Contact

Privacy questions: **privacy@hmlendea.ro**