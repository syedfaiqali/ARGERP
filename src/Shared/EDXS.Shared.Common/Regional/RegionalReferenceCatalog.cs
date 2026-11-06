using System.Data;
using System.Data.Common;

namespace EDXS.Shared.Common.Regional;

public sealed record RegionalCurrencyReference(int CountryId,string Code,string Name,string Symbol);
public static class RegionalReferenceCatalog
{
    public static async Task<Dictionary<int,RegionalCurrencyReference>> Countries(DbConnection connection,CancellationToken token,DbTransaction? transaction = null)
    {
        var opened=connection.State!=ConnectionState.Open;
        try
        {
            if(opened) await connection.OpenAsync(token);
            await using var probe=connection.CreateCommand();
            probe.Transaction=transaction;
            probe.CommandText="SELECT CASE WHEN OBJECT_ID('dbo.SMRegionalCountry') IS NULL THEN 0 ELSE 1 END";
            if(Convert.ToInt32(await probe.ExecuteScalarAsync(token))==0) return [];
            await using var command=connection.CreateCommand();
            command.Transaction=transaction;
            command.CommandText="SELECT countryid,currencycode,currencyname,currencysymbol FROM dbo.SMRegionalCountry WHERE utblock=0";
            await using var reader=await command.ExecuteReaderAsync(token);
            var result=new Dictionary<int,RegionalCurrencyReference>();
            while(await reader.ReadAsync(token)) result[reader.GetInt32(0)]=new(reader.GetInt32(0),reader.GetString(1),reader.GetString(2),reader.GetString(3));
            return result;
        }
        finally { if(opened && connection.State==ConnectionState.Open) await connection.CloseAsync(); }
    }
}
