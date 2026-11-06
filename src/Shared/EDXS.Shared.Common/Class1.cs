using System.Diagnostics;

namespace EDXS.Shared.Common;

public static class CorrelationIds
{
	public const string HeaderName = "X-Correlation-ID";

	public static string Create() => Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();
}
