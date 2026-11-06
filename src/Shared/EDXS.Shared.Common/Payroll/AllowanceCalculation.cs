namespace EDXS.Shared.Common.Payroll;

public static class AllowanceCalculation
{
    public static double Resolve(double? value, bool isPercentage, double baseSalary) =>
        isPercentage ? Calculate(baseSalary, (decimal)(value ?? 0d)) : value ?? 0d;

    public static double Calculate(double baseSalary, decimal rate) =>
        (double)decimal.Round((decimal)baseSalary * rate / 100m, 2, MidpointRounding.AwayFromZero);
}
