# EDXS.Shared.Finance — JV posting engine

Posts system JVs (`vouchertypeid = 1`) for business documents into `FITransactionMaster` /
`FITransactionDetail`, inside the same database transaction as the document. No schema changes.

## Rules

| Event on the document | Engine call | Effect |
|---|---|---|
| Saved | `PostAsync` | New JV, `referenceno = "<SOURCE>:<id>"`, `utsysgen = 1` |
| Edited | `RepostAsync` | **Same JV overwritten** (same Id and code, old lines `utblock = 1`, `utrecordversion + 1`). Posts a new JV if none is active. Blocked when the JV's fiscal year is closed. |
| Voided / deleted | `ReverseAsync` | `"<code>-R"` JV with Dr/Cr swapped; original left intact |

- One active JV per source document (checked under the posting lock).
- Ids and `JV-00001` codes come from `sp_getapplock('EDXS_FI_POSTING')`; any code that writes
  FI rows by `MAX(Id)+1` must take `IFiPostingLock` first.
- Accounts must exist, be active, not blocked and be leaf (`Subsidiary = 1`). A missing account is
  an error, never a silent skip.
- A system JV cannot be edited or deleted from the Journal Voucher screen.

## Wiring a service

```csharp
builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblies(typeof(Program).Assembly, typeof(PostJvCommand).Assembly));
builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<MyServiceDbContext>());
builder.Services.AddJvPosting();
```

Save the document and post its JV in one transaction:

```csharp
await FinanceTransaction.ExecuteAsync(_db, async ct =>
{
    await SaveDocumentAsync(ct);
    await _publisher.Publish(new MyDocumentSavedEvent(id, clientId), ct); // handler calls the engine
}, cancellationToken);
```

## Adding a module (phase 7)

1. **Source code** — add it to `Contracts/JvSources.cs` (code, label, module, screen key and,
   if the document has a number column, `Number = new(table, idColumn, numberColumn)`).
2. **Posting** — in the service, under `Posting/`:
   - `<Doc>JvBuilder` that turns the document into `JvLine`s (Dr/Cr per account);
   - `<Doc>JvEventHandler` for Saved → `RepostAsync` (or `PostAsync`), Voided → `ReverseAsync`;
   - `<Doc>JvBacklogProvider : IJvBacklogProvider` (backlog, build, reconciliation), registered as
     `IJvBacklogProvider`, plus a `JvBacklogV3Controller : JvBacklogControllerBase`.
3. **Frontend back-link** — add the route to `frontend/src/utils/jvSourceLinks.ts`.
4. **Source screen** — `useDeepLinkRecord("id", ...)` to open `?id=&mode=view`, and
   `<JvPostingChip source="<CODE>" sourceId={id} />` in the view header.

The engine does not change. Agree the Dr/Cr accounts for the document before writing the
builder — e.g. GRN needs an inventory (or GRNI) debit account, which the item/GRN tables do not
carry today; the vendor payable is `APVendor.ficoaid`.

## Tests

`tests/EDXS.Shared.Finance.Tests` (SQLite). `SqlServerPostingLockTests` needs
`EDXS_FINANCE_SQL_TEST` pointing at a disposable SQL Server database with the FI tables.
