using Microsoft.IdentityModel.Tokens;

namespace EDXS.Shared.Auth;

public sealed class JwtValidationOptions
{
	public string Audience { get; set; } = string.Empty;
	public string Issuer { get; set; } = string.Empty;
	public string SigningKey { get; set; } = string.Empty;
	public bool RequireHttpsMetadata { get; set; } = true;
}

public static class JwtValidationParametersFactory
{
	public static TokenValidationParameters Create(JwtValidationOptions options) => new()
	{
		ValidateIssuer = !string.IsNullOrWhiteSpace(options.Issuer),
		ValidIssuer = options.Issuer,
		ValidateAudience = !string.IsNullOrWhiteSpace(options.Audience),
		ValidAudience = options.Audience,
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(options.SigningKey)),
		ValidateLifetime = true,
		ClockSkew = TimeSpan.FromMinutes(2)
	};
}
