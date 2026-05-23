# Security Policy

## Supported Versions

The following versions of EAF (Enterprise Application Foundation) are currently supported with security updates:

| Version | Supported          |
| ------- | ------------------ |
| 9.1.x   | :white_check_mark: |
| 9.0.x   | :white_check_mark: |
| 8.x     | :white_check_mark: |
| < 8.0   | :x:                |

## Reporting a Vulnerability

If you discover a security vulnerability in EAF, please report it responsibly.

### How to Report

1. **Do not** open a public issue for security vulnerabilities
2. Send an email to: <security@afonsoft.com>
3. Include as much detail as possible:
   - Description of the vulnerability
   - Steps to reproduce
   - Potential impact
   - Suggested fix (if available)

### Response Process

- **Initial Response**: You should receive a response within 48 hours
- **Investigation**: We will investigate the vulnerability and determine severity
- **Fix Timeline**: Critical vulnerabilities will be addressed within 7 days, high severity within 14 days
- **Public Disclosure**: We will coordinate with you to determine the appropriate time for public disclosure
- **Credit**: If you wish to be credited, let us know in your initial report

### What to Expect

- If the vulnerability is accepted, we will:
  - Work with you to understand the issue
  - Develop and test a fix
  - Release a security update
  - Publish security advisories
  - Credit you for the discovery (if requested)

- If the vulnerability is declined, we will:
  - Explain why it was declined
  - Provide alternative solutions if applicable
  - Work with you to address your concerns

### Security Best Practices

- Keep your EAF installation updated to the latest supported version
- Review security advisories regularly at: <https://github.com/afonsoft/EAF/security/advisories>
- Follow secure coding practices when extending EAF modules
- Use dependency scanning tools to identify vulnerable packages
- Implement proper authentication and authorization in your applications
