# ZKT integration and migration

ARG uses the existing ZKBioTime source and its own new `ARGERP_HR` database. The source's `sync_status` column is neither filtered nor updated. This permits all available history, including punches consumed by EDXS, to be imported independently while the existing EDXS sync continues.

The HR background worker runs every five minutes. History is read in pages of 1,000 transactions. Attendance and a unique `(SourceKey, TransactionId)` receipt are committed together in ARG. Running the worker again retries unprocessed punches and does not duplicate imported transactions. Each pass currently scans available history in bounded pages; very large histories may need an indexed source or a future persistent scan cursor for efficiency. Source credentials and the existing source configuration remain unchanged.

## Current workstation

- Copied the necessary mappings for 3,812 source employee records and 68 device registrations using SELECT-only access to EDXS.
- Branches, departments, designations, employee status references and shifts were mapped to new ARG IDs. Source IDs are recorded in `ARG_LegacyMappings`; imports are repeatable. Client IDs are mapped into the standalone ARG client, with original client identity retained in biometric links.
- Employee import copies identity/machine code, employment dates and attendance references only. It does not copy passwords, tokens, login accounts, salaries or existing attendance from EDXS.
- Some source machine codes/serial registrations are reused. Resolution uses original client identity, device branch and employment dates when necessary. Ambiguous/missing matches remain unprocessed and are retried; they are never marked as consumed in ZKT.
- The real ZKBioTime SQL endpoint at `192.168.18.200:1433` timed out from this workstation during verification. Actual source history has **not** been imported/verified. The configured worker will retry when network access is restored. The local fixture test verifies the import behavior independently.

## Configuration and repeat import

The current workstation already has ignored `.local/zkt-source.json` with `zkConnectionString` and `organizationConnectionString`. To configure a fresh checkout from an existing HR settings file, run:

```powershell
./scripts/configure-zkt.ps1 -SourceSettings 'C:\path\to\existing\HumanResource.Service\appsettings.json'
# Stop ARG backend before compiling/setup; never stop EDXS for this migration.
./scripts/stop.ps1 -BackendOnly
./scripts/setup.ps1
dotnet run --project tools/DatabaseSetup -- --import-zkt-mappings
dotnet build ARGERP.sln
./scripts/start.ps1
```

`--inspect-zkt` checks source connectivity and schema/counts without source writes. Mapping import is restricted to the new ARG target database and runs in a transaction. Rerunning it does not create duplicate identities or overwrite edited ARG profiles; employment end dates are refreshed to support historical disambiguation. Source identity/terminal matching relies on unchanged device serials and employee machine codes.

Changing the spelling of the source server/database in a connection string changes its source key. Preserve the source identity in configuration to keep import receipts continuous.
