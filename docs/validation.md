# Local validation

Verified against the separate `ARGERP_HR` database on the current workstation:

- Solution build and frontend TypeScript/Vite production build passed.
- 49 screen components rendered (HR, supporting Organization screens, Auth).
- 97 API checks passed, including supporting references, employee documents and reports. Removed Loan/Vacancy/Organogram routes return 404; anonymous/invalid sign-in requests return 401. Some root routes are validated for reachability/required-parameter handling rather than full business execution.
- Create/read/update/delete checks passed for 12 HR/Organization reference setups.
- Auth integration passed: user creation/update, sign-in, blocking, password change and cross-service session revocation. Its disposable test user is deactivated by the test; the verification run's account was removed afterwards using its exact fixture identity.
- Employee document upload/download preserved file bytes; leave request creation → pending → approval passed.
- 28 original HR unit tests passed.
- ZKT fixture integration passed: all-history import including already-consumed source punches, repeat idempotency, retry after missing mapping is added, source client isolation and unchanged source flags.
- `http://localhost:3000` returned HTTP 200.

Interactive browser/visual testing was unavailable in this session. The real ZKT SQL server timed out, so actual ZKT history and ambiguous real-world mappings remain to be verified when connectivity is available. The existing EDXS database was accessed only for SELECT-based attendance mapping migration; source code/configuration and source sync flags were not changed.
