using HumanResource.Service.PayrollAccounting.Contracts;

namespace HumanResource.Service.PayrollAccounting.Engine;

/// <summary>
/// Line rules shared by the FluentValidation validator and the engine, so a direct engine call
/// cannot bypass them.
/// </summary>
public static class JvLineRules
{
	/// <summary>Returns the first broken rule, or null when the lines can be posted.</summary>
	public static string? Check(IReadOnlyList<JvLine>? lines)
	{
		if (lines == null || lines.Count < 2) return "A JV needs at least two lines.";

		foreach (var line in lines)
		{
			if (line.AccountId <= 0) return "Every JV line needs an account.";
			if (line.Debit < 0 || line.Credit < 0) return "JV line amounts cannot be negative.";

			var debit = Round(line.Debit);
			var credit = Round(line.Credit);
			if ((debit > 0) == (credit > 0)) return "Each JV line must have either a debit or a credit amount.";
		}

		var totalDebit = lines.Sum(x => Round(x.Debit));
		var totalCredit = lines.Sum(x => Round(x.Credit));
		if (totalDebit != totalCredit)
			return $"JV is not balanced: debit {totalDebit:0.00}, credit {totalCredit:0.00}.";

		return null;
	}

	public static void EnsureValid(IReadOnlyList<JvLine>? lines)
	{
		var error = Check(lines);
		if (error != null) throw new JvPostingException(error);
	}

	public static decimal Round(decimal amount) => Math.Round(amount, 2, MidpointRounding.AwayFromZero);
}
