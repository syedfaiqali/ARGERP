using System.Data.Common;
using System.Text.RegularExpressions;

namespace HumanResource.Service.Infrastructure.Data;

public sealed partial class HumanResourceDbContext
{
    // Only the explicitly listed attachment/document tables are eligible. Values remain
    // parameters on the caller's command and participate in its existing transaction.
    internal void PrepareAuditCommand(DbCommand command)
    {
        var sql = command.CommandText;
        var update = Regex.Match(sql, @"\bUPDATE\s+(?:dbo\.)?(\w+)\s+SET\s+", RegexOptions.IgnoreCase);
        var insert = Regex.Match(sql, @"\bINSERT\s+(?:INTO\s+)?(?:dbo\.)?(\w+)\s*\(([^)]+)\)\s*VALUES\s*\(", RegexOptions.IgnoreCase);
        var match = update.Success ? update : insert;
        if (!match.Success || !HumanResourceAuditSchema.SqlTables.TryGetValue(match.Groups[1].Value, out var columns)) return;

        string Parameter(object? value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@hrAudit" + command.Parameters.Count;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
            return parameter.ParameterName;
        }

        var values = AuditValues(columns, added: !update.Success);
        if (update.Success)
        {
            var assignments = values.Select(v => $"[{v.Key}] = {Parameter(v.Value)}").ToList();
            if (columns.Any(c => c.Name == "utrecordversion")
                && !Regex.IsMatch(sql, @"\butrecordversion\s*=", RegexOptions.IgnoreCase))
                assignments.Add("utrecordversion = ISNULL(utrecordversion, 0) + 1");
            command.CommandText = sql.Insert(update.Index + update.Length, string.Join(", ", assignments) + ", ");
        }
        else
        {
            var present = insert.Groups[2].Value.Split(',').Select(x => x.Trim().Trim('[', ']')).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var additions = values.Where(v => !present.Contains(v.Key)).ToArray();
            var end = ValuesEnd(sql, insert.Index + insert.Length);
            command.CommandText = sql.Insert(end, ", " + string.Join(", ", additions.Select(v => Parameter(v.Value))))
                .Insert(insert.Groups[2].Index + insert.Groups[2].Length, ", " + string.Join(", ", additions.Select(v => $"[{v.Key}]")));
        }
    }

    private static int ValuesEnd(string sql, int start)
    {
        var depth = 0;
        var quoted = false;
        for (var i = start; i < sql.Length; i++)
        {
            if (sql[i] == '\'')
            {
                if (quoted && i + 1 < sql.Length && sql[i + 1] == '\'') i++;
                else quoted = !quoted;
            }
            else if (!quoted)
            {
                if (sql[i] == '(') depth++;
                else if (sql[i] == ')') { if (depth == 0) return i; depth--; }
            }
        }
        throw new InvalidOperationException("HR attachment insert values were not recognized.");
    }
}
