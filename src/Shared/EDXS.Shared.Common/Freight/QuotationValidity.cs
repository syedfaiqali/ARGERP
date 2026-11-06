using System.Globalization;
using System.Text.RegularExpressions;

namespace EDXS.Shared.Common.Freight;

public static class QuotationValidity
{
    public static DateTime? EndDate(string? value)
    {
        var text = (value ?? "").Trim();
        if (DateTime.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)) return date;
        var legacy = Regex.Match(text, @"(?:^|[-–])\s*(\d{1,2}\s+[A-Za-z]+\s+\d{4})\s*$");
        return legacy.Success && DateTime.TryParseExact(legacy.Groups[1].Value, ["d MMM yyyy", "dd MMM yyyy", "d MMMM yyyy", "dd MMMM yyyy"], CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out date) ? date : null;
    }

    public static string Format(DateTime? start, string? validity)
    {
        var end = EndDate(validity);
        if (!end.HasValue) return validity ?? "";
        var endText = end.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
        return start.HasValue ? start.Value.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) + " - " + endText : endText;
    }
}
