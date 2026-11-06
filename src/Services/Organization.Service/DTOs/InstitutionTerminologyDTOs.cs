namespace Organization.Service.DTOs
{
    /// <summary>
    /// A single keyword swap for an institution type, e.g. "Teacher" -> { en: "Caretaker", ur: "نگران", ... }.
    /// The <see cref="To"/> dictionary is keyed by language code (en, ur, ar, fa, ps); "en" is required.
    /// </summary>
    public sealed class TerminologyMappingDto
    {
        public string From { get; set; } = string.Empty;
        public Dictionary<string, string> To { get; set; } = new();
    }
}
