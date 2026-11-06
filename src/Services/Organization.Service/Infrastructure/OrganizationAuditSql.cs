using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace Organization.Service.Infrastructure.Data;

public partial class OrganizationDbContext
{
    // School Setup writes through SQL inside its import transaction, bypassing SaveChanges.
    internal void AuditImport(DbCommand command)
    {
        var insertCount = 0;
        command.CommandText = Regex.Replace(command.CommandText,
            @"INSERT\s+INTO\s+(\w+)\s*\(([^)]+)\)\s*VALUES\s*\((.*?)\);",
            match =>
            {
                insertCount++;
                var table = OrganizationAuditSchema.ImportTables.SingleOrDefault(t =>
                    string.Equals(t.Table, match.Groups[1].Value, StringComparison.OrdinalIgnoreCase))
                    ?? throw new InvalidOperationException("Import audit schema is not configured.");
                var columns = match.Groups[2].Value.Split(',').Select(c => c.Trim()).ToList();
                var values = SplitSqlValues(match.Groups[3].Value);
                if (columns.Count != values.Count)
                    throw new InvalidOperationException("Import columns and values do not match.");
                for (var index = columns.Count - 1; index >= 0; index--)
                {
                    if (!table.Columns.Any(c => c.Name.Equals(columns[index], StringComparison.OrdinalIgnoreCase))) continue;
                    columns.RemoveAt(index);
                    values.RemoveAt(index);
                }
                var audit = AuditValues(table.Columns, added: true);
                audit["utdoctype"] = table.DocType;
                audit["utsubdoctype"] = table.SubType;
                audit["utrecordversion"] = 1;
                audit["utblock"] = false;
                audit["utsysgen"] = false;
                audit["utispartial"] = false;
                foreach (var column in table.Columns)
                {
                    columns.Add($"[{column.Name}]");
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@organizationAudit_" + column.Name;
                    parameter.DbType = column.SqlType switch
                    {
                        "bit" => DbType.Boolean,
                        "int" => DbType.Int32,
                        "bigint" => DbType.Int64,
                        "datetime" => DbType.DateTime,
                        "datetime2" => DbType.DateTime2,
                        "nvarchar" => DbType.String,
                        _ => DbType.AnsiString
                    };
                    if (parameter.DbType is DbType.String or DbType.AnsiString)
                        parameter.Size = column.SqlType == "nvarchar" ? column.Length / 2 : column.Length;
                    parameter.Value = audit.GetValueOrDefault(column.Name) ?? DBNull.Value;
                    command.Parameters.Add(parameter);
                    values.Add(parameter.ParameterName);
                }
                return $"INSERT INTO [{table.Table}] ({string.Join(',', columns)}) VALUES ({string.Join(',', values)});";
            }, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        if (insertCount != 1)
            throw new InvalidOperationException("An import command must contain exactly one audited insert.");
    }

    private static List<string> SplitSqlValues(string sql)
    {
        var values = new List<string>();
        var depth = 0;
        var quoted = false;
        var start = 0;
        for (var index = 0; index < sql.Length; index++)
        {
            var character = sql[index];
            if (character == '\'')
            {
                if (quoted && index + 1 < sql.Length && sql[index + 1] == '\'') index++;
                else quoted = !quoted;
            }
            else if (!quoted)
            {
                if (character == '(') depth++;
                else if (character == ')') depth--;
                else if (character == ',' && depth == 0)
                {
                    values.Add(sql[start..index].Trim());
                    start = index + 1;
                }
            }
        }
        values.Add(sql[start..].Trim());
        return values;
    }
}
