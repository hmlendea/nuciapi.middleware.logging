# Documentation Maintenance

## Ownership

- **Primary**: Horațiu Mlendea (repository owner)
- **Contributors**: Update docs with behaviour changes

## Update Triggers

| Trigger | Required Updates |
|---------|------------------|
| New log field | `logging.md`, `components/request-logging-middleware.md`, `components/logging-abstractions.md` |
| Behaviour change | `architecture.md`, `flows/request-logging-flow.md`, `change-guide.md` |
| New dependency | `dependencies.md`, `architecture.md` |
| Breaking change | `README.md`, `change-guide.md`, version bump |
| New test | `testing.md` |
| Configuration change | `configuration.md`, `quick-start.md` |

## Review Cadence

- **Per PR**: Update affected docs in same PR
- **Per Release**: Full review of `quick-start.md`, `README.md`, `CHANGELOG.md`
- **Quarterly**: Review `ambiguities-and-open-questions.md`, `faq.md`

## Documentation Standards

- Location: `docs/` (this directory)
- Format: Markdown
- Naming: kebab-case
- Cross-references: Relative links from `docs/` root
- Code symbols: Backticks with full namespace

## Validation

```bash
# Check for broken links (requires markdown-link-check)
npx markdown-link-check docs/**/*.md

# Verify all documented files exist
ls docs/**/*.md
```

## Adding New Documentation

1. Create file in appropriate `docs/` subdirectory
2. Follow naming conventions
3. Add entry to `docs/INDEX.md` catalogue
4. Cross-reference from related files
5. Update `docs/INDEX.md` maintenance metadata (date, owner)