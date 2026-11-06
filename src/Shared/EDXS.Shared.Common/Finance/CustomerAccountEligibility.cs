using System.Data;
using System.Data.Common;

namespace EDXS.Shared.Common.Finance;

public sealed record CustomerAccountClassification(int Id, int? ParentId, string Title,
    bool? Subsidiary, bool Active, bool Blocked, string PartyType);

/// <summary>Shared customer selection for Finance profiles and Freight quotations.</summary>
public static class CustomerAccountEligibility
{
    public static HashSet<int> Select(IEnumerable<CustomerAccountClassification> source, bool includeHeadings = false)
    {
        var accounts = source.ToDictionary(x => x.Id);
        var parents = accounts.Values.Where(x => x.ParentId.HasValue && !x.Blocked).Select(x => x.ParentId!.Value).ToHashSet();
        var result = new HashSet<int>();
        foreach (var account in accounts.Values)
        {
            if (!account.Active || account.Blocked || account.Subsidiary == false || parents.Contains(account.Id)) continue;
            if (!string.IsNullOrWhiteSpace(PartyName(account.PartyType)) && !CustomerType(account.PartyType)) continue;
            var customer = CustomerType(account.PartyType);
            var seen = new HashSet<int>();
            var current = account;
            var valid = true;
            while (true)
            {
                if (!seen.Add(current.Id) || !current.Active || current.Blocked) { valid = false; break; }
                if (current.Id != account.Id && (CustomerType(current.PartyType) || CustomerControl(current.Title))) customer = true;
                if (!current.ParentId.HasValue || !accounts.TryGetValue(current.ParentId.Value, out var parent)) break;
                current = parent;
            }
            if (valid && customer) result.Add(account.Id);
        }
        if (includeHeadings)
            foreach (var accountId in result.ToArray())
            {
                var current = accounts[accountId];
                while (current.ParentId.HasValue && accounts.TryGetValue(current.ParentId.Value, out var parent) && result.Add(parent.Id)) current = parent;
            }
        return result;
    }

    private static string Normalize(string value) => new(value.Where(char.IsLetterOrDigit).Select(char.ToUpperInvariant).ToArray());
    private static string PartyName(string value) => Normalize(value).Replace("LOCAL", "").Replace("FOREIGN", "").Replace("INTERNATIONAL", "");
    private static bool CustomerType(string value) => PartyName(value) is "CUSTOMER" or "CUSTOMERS" or "DEBTOR" or "DEBTORS" or "TRADEDEBTOR" or "TRADEDEBTORS" or "RECEIVABLE" or "RECEIVABLES" or "ACCOUNTSRECEIVABLE"
        or "VENDOR" or "VENDORS" or "SUPPLIER" or "SUPPLIERS" or "CREDITOR" or "CREDITORS" or "TRADECREDITOR" or "TRADECREDITORS" or "PAYABLE" or "PAYABLES" or "ACCOUNTSPAYABLE";
    private static bool CustomerControl(string title)
    {
        var value = PartyName(title);
        return value.StartsWith("TRADEDEBTOR", StringComparison.Ordinal) || value.StartsWith("TRADERECEIVABLES", StringComparison.Ordinal)
            || value.StartsWith("ACCOUNTSRECEIVABLE", StringComparison.Ordinal)
            || value.StartsWith("TRADECREDITOR", StringComparison.Ordinal) || value.StartsWith("TRADEPAYABLES", StringComparison.Ordinal)
            || value.StartsWith("ACCOUNTSPAYABLE", StringComparison.Ordinal)
            || value is "CUSTOMERS" or "CUSTOMERACCOUNTS" or "CUSTOMERCONTROL" or "DEBTORS" or "SUNDRYDEBTORS"
                or "CREDITORS" or "SUNDRYCREDITORS" or "VENDORS" or "SUPPLIERS" or "SUPPLIERACCOUNTS" or "VENDORACCOUNTS";
    }

    public static async Task<HashSet<int>> ReadAsync(DbConnection connection, int clientId, CancellationToken ct, DbTransaction? transaction = null, bool includeHeadings = false)
    {
        if (clientId <= 0) throw new ArgumentOutOfRangeException(nameof(clientId));
        var opened = connection.State != ConnectionState.Open;
        try
        {
            if (opened) await connection.OpenAsync(ct);
            await using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = @"
                SELECT coa.Id, coa.ParentId, COALESCE(coa.Title, ''), coa.Subsidiary,
                       CAST(CASE WHEN coa.Active = 0 THEN 0 ELSE 1 END AS bit), CAST(COALESCE(coa.utblock, 0) AS bit),
                       COALESCE(NULLIF(party.title, ''), legacy.stxt, '')
                FROM dbo.FICOA_New coa
                LEFT JOIN dbo.FIPartytype party ON party.Id = coa.partytypeid
                LEFT JOIN dbo.lov legacy ON legacy.Id = coa.lovpartytype
                WHERE coa.clientid = @clientId";
            var parameter = command.CreateParameter(); parameter.ParameterName = "@clientId"; parameter.DbType = DbType.Int32; parameter.Value = clientId;
            command.Parameters.Add(parameter);
            await using var reader = await command.ExecuteReaderAsync(ct);
            var rows = new List<CustomerAccountClassification>();
            while (await reader.ReadAsync(ct)) rows.Add(new(reader.GetInt32(0), reader.IsDBNull(1) ? null : reader.GetInt32(1), reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetBoolean(3), reader.GetBoolean(4), reader.GetBoolean(5), reader.GetString(6)));
            return Select(rows, includeHeadings);
        }
        finally { if (opened && connection.State == ConnectionState.Open) await connection.CloseAsync(); }
    }
}
