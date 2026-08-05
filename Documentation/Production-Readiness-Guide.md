# Mamia Seeds Oil Limited Website
## Production Readiness Guide

## 1. Project Overview
This application is an ASP.NET Core MVC corporate website for Mamia Seeds Oil Limited, structured to support:
- Corporate content publishing
- Bilingual website localization (English and Hausa)
- Controlled AI assistant for company-scoped Q&A
- Contact, distributor, and partnership enquiry intake
- SEO metadata and discoverability endpoints

The approved visual design is preserved. Production-readiness work focused on security, reliability, performance, maintainability, and operational clarity.

## 2. Architecture Overview
### Runtime Pattern
- ASP.NET Core MVC with controllers, view models, options binding, services, and middleware.
- Configuration-driven content and behavior through strongly typed options.
- Service-oriented business logic for contact, partnership, AI, SEO, and content assembly.

### Key Layers
- Controllers: API and page orchestration.
- Services: business rules and reusable logic.
- Middleware: security headers, maintenance mode, exception handling, and telemetry.
- Configuration: options classes bound to appsettings files.
- Views/Partials: modular page rendering with localized text and placeholders.

### SOLID and Maintainability Notes
- Business logic is primarily in services rather than controllers.
- New concerns were isolated into dedicated classes (telemetry middleware, config audit service, file security validator, privacy masker).
- Most dependencies are injected through interfaces, enabling future testing and replacement.

## 3. Folder Structure
High-level structure:
- Api/ (if used for API-specific grouping)
- Configuration/ (strongly typed options)
- Controllers/ (MVC and API endpoints)
- Data/Knowledge/ (AI knowledge sources)
- DTOs/ (API request/response models)
- Extensions/ (service and app pipeline wiring)
- Helpers/ (cross-cutting utility helpers)
- Interfaces/ (abstractions)
- Middleware/ (HTTP pipeline concerns)
- Models/ (domain models)
- Resources/ (resx localization)
- Services/ (business logic)
- ViewComponents/ (reusable UI components)
- ViewModels/ (presentation models)
- Views/ (Razor views and partials)
- wwwroot/ (static assets)
- Documentation/ (operational docs)

## 4. Configuration Guide
### Configuration Files
- appsettings.json: baseline defaults
- appsettings.Development.json: development overrides
- appsettings.Production.json: production overrides

### Core Config Sections
- WebsiteContent: company/site content and placeholders
- CompanyProfile: company profile model
- ProductCatalog: product entries
- Localization: cultures and fallback
- SecurityHeaders: CSP and hardened security headers
- SiteOperations: maintenance mode switch
- Analytics: analytics IDs
- AiAssistant: AI behavior, provider config, rate limits
- EmailDelivery: provider and delivery options
- FeatureFlags: runtime feature toggles
- Observability: telemetry behavior and slow request thresholds
- FileHandling: safe upload constraints (for current/future file workflows)
- FutureDataConnections: database placeholders for future persistence
- ImageDelivery: CDN/image URL behavior

### Placeholder Policy
Unknown business data must remain blank or [To Be Updated].
No fake company data should be introduced.

## 5. Deployment Guide
### Supported Targets
- IIS
- Azure App Service
- Linux hosts
- Future Docker hosting

### Deployment Prerequisites
- Set ASPNETCORE_ENVIRONMENT=Production
- Provide production appsettings values (or environment variables)
- Configure HTTPS termination and forwarded headers at reverse proxy
- Ensure logs are routed to persistent sinks in hosting environment

### Reverse Proxy
Forwarded headers are enabled for X-Forwarded-For and X-Forwarded-Proto.

### Security Expectations
- HTTPS mandatory in production
- HSTS enabled outside Development
- Cookies configured for secure transport and restricted browser behavior

### Monitoring Endpoints
- /health
- /health/live
- /health/ready
- /status (feature-flag controlled)

## 6. AI Architecture
### Scope and Guardrails
- AI assistant is company-scoped and must not invent unknown information.
- English-only assistant behavior is preserved.
- Unknown or unavailable facts return controlled fallback responses.

### AI Composition
- Controller: request handling and rate-limit policy application
- Assistant service: orchestration, timing logs, redaction-aware logging
- Provider abstraction: swappable providers; default rule-based provider
- Knowledge layer: structured knowledge files with validation/search support

### AI Configuration
- Display name, welcome text, fallbacks, suggestions, provider toggles, streaming, and rate limits are all configuration-driven.

## 7. Language Experience
### Supported Language
- English only

### Content Behavior
- UI text, forms, navigation, and AI assistant messages are rendered in English only.
- No runtime language switching or resource-based localization is enabled.

## 8. Security Controls
### Implemented Controls
- CSP, X-Frame-Options, X-Content-Type-Options, Referrer-Policy, Permissions-Policy
- Additional cross-origin hardening headers
- CSRF enforcement on POST API endpoints with antiforgery header token
- Input validation via DataAnnotations + NoHtml validators
- Output encoding through Razor default encoding
- Global exception middleware with safe error responses
- PII-aware logging via masking helpers

### Logging Safety
Sensitive values (for example, full emails) are masked in logs where applicable.
No stack traces are exposed in production responses.

## 9. Performance and Frontend Optimization Notes
### Current Optimizations
- Response compression enabled for HTTPS
- Response caching enabled
- Static asset cache headers tuned (immutable for non-HTML assets)
- HTML no-cache strategy to prevent stale content shell issues
- Lazy loading and async decoding set for images on initialization
- Slow request detection and warning logs in telemetry middleware

### Frontend Stability
- Existing visual design preserved
- Focus visibility improved for keyboard accessibility
- Partnership form multi-step semantics and reset flow improved

## 10. SEO System
- Canonical URL assignment per request
- Open Graph and Twitter metadata in layout
- JSON-LD structured data generation via SEO service
- sitemap.xml and robots.txt endpoints
- Response caching on SEO endpoints for efficiency

## 11. Future Expansion Guide
Recommended next expansions:
- Replace in-memory enquiry store with persistent storage (SQL or managed DB)
- Add real email provider implementation per selected provider
- Add object storage provider for future document uploads
- Add external health checks for DB/email/storage dependencies
- Add automated tests for controllers/services/middleware
- Add CI/CD pipeline once hosting and release process are finalized

## 12. Placeholder Management Guide
### Where placeholders exist
- Website content sections in appsettings
- AI knowledge files in Data/Knowledge and ai-knowledge-base configuration
- Future connection and provider fields

### Safe Update Process
1. Update placeholders only from verified business inputs.
2. Keep unknown values blank or [To Be Updated].
3. Validate config shape and startup logs after updates.
4. Re-run diagnostics and endpoint checks before deployment.

## 13. Maintenance Recommendations
- Keep configuration strongly typed and validated on startup.
- Avoid adding controller business logic; place logic in services.
- Keep middleware concerns small and focused.
- Prefer feature flags for controlled rollout of optional functionality.
- Periodically review CSP and third-party script domains.
- Track JS/CSS asset growth and rebundle/minify for releases.
- Keep localization resources synchronized when adding UI text.
- Run routine security, accessibility, and performance audits before each release.
