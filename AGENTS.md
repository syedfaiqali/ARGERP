# Standalone ARG ERP scope

- Only Human Resource, Organization and Auth business modules belong in this project.
- APIs needed by HR from another business module must be implemented locally in HR, with HR-owned routes. Do not restore calls or proxy aliases to Academics, Finance, Administration, Transport or other original services.
- HR approvals run inside HumanResource.Service. Keep existing business approval behavior; do not add a separate Workflow business service.
- Solution, AppHost, build/start/setup scripts and frontend may include only these modules and their necessary infrastructure, shared primitives and database tooling.
- Do not add legacy applications, fallback services, old module screens or compatibility gateway routes.
- Loan, Vacancy and Organogram screens remain excluded.
- Preserve the current Al Rahim UI and existing ZKT source integration; never alter the source system's attendance sync flags.
- Do not run a background ARG backend while Visual Studio is debugging/building it. Use isolated build artifacts for verification when output files may be locked.
- Keep the project free of temporary CJS/MJS verification runners, root build logs and generated test reports. Do not leave one-off cleanup or migration helper scripts in the workspace after use. Preserve required local settings and ZKT credentials.
- Send launcher runtime logs to the OS temporary directory (`ARGERP/logs`), outside the project.
