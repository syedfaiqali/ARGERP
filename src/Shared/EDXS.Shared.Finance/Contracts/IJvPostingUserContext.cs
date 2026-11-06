using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace EDXS.Shared.Finance.Contracts;

/// <summary>
/// Who is posting. Fills utadduser/utaddip when a command does not carry them.
/// </summary>
public interface IJvPostingUserContext
{
	string? UserName { get; }
	string? IpAddress { get; }
}

/// <summary>Reads the user from the current request's JWT claims (same order as the service audit trails).</summary>
public sealed class HttpJvPostingUserContext : IJvPostingUserContext
{
	private readonly IHttpContextAccessor _http;

	public HttpJvPostingUserContext(IHttpContextAccessor http)
	{
		_http = http;
	}

	public string? UserName
	{
		get
		{
			var user = _http.HttpContext?.User;
			if (user?.Identity?.IsAuthenticated != true) return null;
			return user.FindFirst("userName")?.Value
				?? user.FindFirst(ClaimTypes.Name)?.Value
				?? user.FindFirst("unique_name")?.Value
				?? user.FindFirst("userId")?.Value
				?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
				?? user.FindFirst("sub")?.Value;
		}
	}

	public string? IpAddress
	{
		get
		{
			var address = _http.HttpContext?.Connection.RemoteIpAddress;
			if (address?.IsIPv4MappedToIPv6 == true) address = address.MapToIPv4();
			return address?.ToString();
		}
	}
}

public sealed class NoJvPostingUserContext : IJvPostingUserContext
{
	public static readonly NoJvPostingUserContext Instance = new();

	public string? UserName => null;
	public string? IpAddress => null;
}
