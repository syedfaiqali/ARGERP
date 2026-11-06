namespace HumanResource.Service.Services;

public static class EmployeeAllowanceCalculation
{
    // Matches the twelve integer digits accepted by the salary amount inputs.
    public const double MaximumAmount = 999_999_999_999.99;

    public static bool IsValidAmount(double? amount) => amount.HasValue
        && double.IsFinite(amount.Value) && amount.Value >= 0 && amount.Value <= MaximumAmount;

    public static bool IsValidRate(double? rate) => rate.HasValue && double.IsFinite(rate.Value)
        && rate.Value >= 0 && rate.Value <= 100
        && Math.Round(rate.Value, 2, MidpointRounding.AwayFromZero) == rate.Value;
}
