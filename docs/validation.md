# Local validation

Verified against the separate `ARGERP_HR` database on the current workstation:

- Solution build and frontend TypeScript/Vite production build passed.
- 51 render checks passed: 49 screen components plus the branded login and home portal.
- 97 API checks passed, including supporting references, employee documents and reports. Removed Loan/Vacancy/Organogram routes return 404; anonymous/invalid sign-in requests return 401. Some root routes are validated for reachability/required-parameter handling rather than full business execution.
- Create/read/update/delete checks passed for 12 HR/Organization reference setups.
- Auth integration passed: user creation/update, sign-in, blocking, password change and cross-service session revocation, including Workflow. Its disposable test user is deactivated by the test; the verification run's account was removed afterwards using its exact fixture identity.
- Employee document upload/download preserved file bytes; leave request creation → pending → approval passed.
- 28 original HR unit tests passed.
- ZKT fixture integration passed: all-history import including already-consumed source punches, repeat idempotency, retry after missing mapping is added, source client isolation and unchanged source flags.
- `http://localhost:3000` returned HTTP 200.

Interactive browser/visual testing was unavailable in this session. The real ZKT SQL server timed out, so actual ZKT history and ambiguous real-world mappings remain to be verified when connectivity is available. The existing EDXS database was accessed only for SELECT-based attendance mapping migration; source code/configuration and source sync flags were not changed.

After restoring the original portal presentation and wiring AppHost infrastructure, the frontend production build, all 97 API checks, Auth integration and HR document/leave flows passed again. Redis contained workflow nonce entries, RabbitMQ diagnostics ping succeeded, and both the frontend (3000) and Aspire dashboard (18880) returned HTTP 200. AppHost uses explicit no-launch-profile settings to prevent duplicate/random service endpoints. Browser automation remained unavailable, so interactive visual comparison could not be completed.

After the reported Visual Studio file-lock errors, background ARG backend processes were stopped. An isolated solution build (`dotnet build ARGERP.sln --artifacts-path .local/verification --nologo -v quiet`) passed with 0 errors and 429 warnings. The AppHost launch profile now includes the missing dashboard/OTLP environment variables. Interactive Visual Studio F5 execution was not directly verified; backend services were left stopped so the user's rebuild can replace the normal output files.


Scope consolidation: approval hosting and payroll posting moved into HumanResource.Service; the standalone Workflow service and Shared.Finance project were removed from the solution and AppHost. Frontend module aliases were replaced by HR-owned endpoints, and the hard-coded remote monthly-attendance call was replaced by a local employee/date-scoped HR recalculation. Scope guard, isolated backend build (0 errors), frontend production build, 97 API checks, Auth revocation, 51 screen renders and 28 HR unit tests passed. PDF/XLSX report generation, HR-only requests, employee scoping and no-data handling passed fixture checks. No interactive visual comparison or live Zoom credentials were available.

The login panel now contains only HR copy (employee records, attendance, leave approvals and payroll); unsupported usage/success statistics were replaced with feature labels. Education icons were replaced with HR icons, and the oversized horizontal divider was corrected to 1px. Frontend TypeScript check passed after this text/layout adjustment.
