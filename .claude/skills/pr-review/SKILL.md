---
name: pr-review
description: Review pull requests for the smart logistics backend. Use when reviewing a PR, diff, or file change before merge, checking for bugs/security/performance issues, or asking "is this PR safe to merge". Covers Ruby on Rails and .NET/C# code.
argument-hint: "<PR URL, diff, or file path>"
---

# PR Review — Smart Logistics Backend

Review code changes with a structured lens on security, performance, correctness,
maintainability, and logistics-domain correctness. Applies to both Ruby on Rails
and .NET/C# code in this codebase.

## Usage

Review the provided code changes: @$1

If no diff, PR URL, or file is provided, ask what to review before doing anything else.

## Review Dimensions

### 1. Security
- SQL injection, XSS, CSRF, mass-assignment (Rails strong params / EF model binding)
- AuthN/AuthZ flaws — especially on endpoints that mutate shipment/order/inventory state
- Secrets, API keys, carrier credentials hardcoded or logged
- Insecure deserialization
- SSRF on any outbound call to carrier/partner APIs
### 2. Performance
- N+1 queries (ActiveRecord `includes`/`preload` or EF Core `.Include()` missing)
- Unbounded queries or loops over orders/shipments/inventory without pagination
- Missing DB indexes on frequently filtered columns (status, tracking_id, warehouse_id, timestamps)
- Synchronous calls to slow external services (carrier APIs, geocoding) blocking request threads
- Resource leaks (unclosed DB connections, HTTP clients not reused/disposed)
### 3. Correctness
- Edge cases: empty input, null, negative quantities, zero-weight packages, overflow on large shipment batches
- **Idempotency**: any webhook/event handler (order created, shipment status update, carrier callback) MUST be idempotent — check for duplicate-event guards (idempotency key, unique constraint, or upsert)
- **Timezones**: shipment/delivery timestamps stored and compared in UTC; flag any naive `DateTime.Now` / `Time.now` used for business logic instead of UTC-aware equivalents
- **Concurrency**: inventory/stock decrements must use optimistic or pessimistic locking (e.g. `with_lock`, `SELECT ... FOR UPDATE`, EF Core concurrency tokens) — flag any read-then-write without a lock
- **External API failure handling**: carrier/partner API calls must have timeout, retry (with backoff), and a defined failure path (dead-letter queue, manual review state) — not a silent rescue/swallow
- Race conditions and error propagation
- Off-by-one errors, type safety
### 4. Maintainability
- Naming clarity, single responsibility, duplication
- Test coverage — flag missing tests for new branches, especially failure paths and idempotency
- Documentation for non-obvious domain logic (e.g. carrier-specific quirks, SLA rules)
### 5. Cross-stack notes (Rails ↔ .NET)
Since the author is fluent in Rails and learning .NET:
- If reviewing **.NET/C# code**: flag any place where the code is unidiomatic because it's
  mimicking a Rails pattern (e.g. fat controllers instead of using DI/services, manual SQL
  instead of LINQ/EF, missing `async/await` where Rails wouldn't need an equivalent). Note
  the idiomatic .NET alternative AND the Rails concept it maps to, briefly — this doubles as
  a learning note, not just a nitpick.
- If reviewing **Rails code**: review normally, no cross-stack notes needed.
## Output Format

```markdown
## PR Review: [PR title or file]
 
### Summary
[1-2 sentence overview of the change and overall quality]
 
### Critical Issues
| # | File | Line | Issue | Severity |
|---|------|------|-------|----------|
| 1 | [file] | [line] | [description] | 🔴 Critical |
 
### Domain Risks (logistics-specific)
| # | File | Line | Risk | Category |
|---|------|------|------|----------|
| 1 | [file] | [line] | [e.g. missing idempotency guard on webhook] | Idempotency/Concurrency/Timezone/External API |
 
### Suggestions
| # | File | Line | Suggestion | Category |
|---|------|------|------------|----------|
| 1 | [file] | [line] | [description] | Performance/Style/etc |
 
### Cross-Stack Notes (.NET PRs only)
- [Unidiomatic pattern] → [idiomatic .NET fix] (Rails equivalent: [...])
 
### What Looks Good
- [Positive, specific observations]
 
### Verdict
[Approve / Request Changes / Needs Discussion]
```

## Tips for the Author
1. Mention if the PR touches a **hot path** (order intake, real-time tracking) — review focuses harder on performance.
2. Mention if it touches **PII** (customer address, contact info) — review focuses harder on security/compliance.
3. Point out which carrier/partner integration is involved, if any — failure-handling checks are integration-specific.
 