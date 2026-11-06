namespace Organization.Service.Handlers.Commands;

internal static class OrganizationValidation
{
    internal static void Client(int? clientId)
    {
        if (clientId is not > 0)
            throw new InvalidOperationException("ClientId is required.");
    }

    internal static void Text(string? value, string field, int maxLength, bool required = false)
    {
        if (required && string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"{field} is required.");
        if (value?.Trim().Length > maxLength)
            throw new InvalidOperationException($"{field} must be {maxLength} characters or less.");
    }
}
