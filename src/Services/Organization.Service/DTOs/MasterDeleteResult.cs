namespace Organization.Service.DTOs;

public sealed record MasterDeleteResult(bool Found, IReadOnlyList<string> UsedIn)
{
    public bool Deleted => Found && UsedIn.Count == 0;
    public string Message(string kind) => $"{kind} cannot be deleted because it is used in: {string.Join(", ", UsedIn)}.";
}
