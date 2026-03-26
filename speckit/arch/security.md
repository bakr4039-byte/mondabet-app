# Security Specification – Mondabet

## Auth Flow Summary
```
1. POST /auth/login → Keycloak validates credentials → returns sessionToken + mfaRequired=true
2. POST /auth/mfa/verify → OTP via Unifonic SMS → returns JWT (RS256) + refreshToken
3. JWT contains: sub, tid (tenantId), roles[], exp, jti (for revocation)
4. All subsequent requests: Authorization: Bearer {jwt}
5. YARP validates JWT signature (Keycloak public key via JWKS endpoint)
```

## Biometric Auth
```
1. Device registers public key on first biometric setup → stored in identity-svc
2. /auth/biometric/challenge → server returns random nonce (15s TTL in Redis)
3. Device signs nonce with private key → /auth/biometric/verify
4. Server verifies signature with stored public key → issues JWT
```

## Nafath Integration
```
1. /auth/nafath/initiate {iqamaNumber} → call Nafath API → returns transactionId
2. User approves in Nafath app
3. /auth/nafath/verify {transactionId} → poll Nafath status → issue JWT on success
```

## OWASP Top 10 Controls
| Risk | Control |
|------|---------|
| A01 Broken Access Control | RBAC via Keycloak roles; TenantId isolation on every query |
| A02 Cryptographic Failures | AES-256 PII at rest; TLS 1.3; bcrypt for passwords (Keycloak) |
| A03 Injection | EF Core parameterized; no raw SQL in commands |
| A04 Insecure Design | Onion arch; Result<T>; no business logic in controllers |
| A05 Security Misconfiguration | Helm secrets; no secrets in code; env-specific configs |
| A06 Vulnerable Components | Dependabot; weekly image scans (Trivy) |
| A07 Auth Failures | JWT RS256; refresh token rotation; jti revocation list in Redis |
| A08 Data Integrity | Signed JWTs; MinIO object checksums; migration scripts versioned |
| A09 Logging Failures | OpenTelemetry structured logs; audit log table per tenant |
| A10 SSRF | Allowlist for external HTTP calls; no user-supplied URLs fetched |

## YARP Security Policies
```yaml
RateLimiting:
  /auth/**:     10 req/min per IP
  /api/**:      200 req/min per JWT sub
  /reports/**:  5 req/min per JWT sub

CORS: explicit allowlist per tenant subdomain
RequestSizeLimit: 10MB (file uploads), 100KB (JSON)
HeaderValidation: strip X-Forwarded-For spoofing
```

## Sensitive Data Handling
- Iqama stored encrypted (AES-256), decrypted only in application layer
- Mobile numbers: masked in logs (`+9665****1234`)
- Files in MinIO: private bucket, pre-signed URLs (15 min TTL)
- No PII in JWT beyond sub + tid + roles
