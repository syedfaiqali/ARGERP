namespace HumanResource.Service.PayrollAccounting.Contracts;

/// <summary>
/// A business rule stopped the JV. Derives from InvalidOperationException so the existing
/// service exception filters return it as a 400 with the message.
/// </summary>
public sealed class JvPostingException : InvalidOperationException
{
	public JvPostingException(string message) : base(message)
	{
	}
}
