using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtHelper
{
    private readonly IConfiguration _config;

    public JwtHelper(IConfiguration config)
    {
        _config = config;
    }

    // Original method (still useful for simple cases)
    public string GenerateToken(string username, string role, string matid)
    {
        var claims = new[]
        {
        new Claim(ClaimTypes.Name, username),
        new Claim(ClaimTypes.Role, role),
        new Claim(ClaimTypes.NameIdentifier, matid) // Store MATID for later use
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],             // "ProgressReportAPI"
            audience: _config["Jwt:Audience"],         // Must be "ProgressReportAPIUsers"
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    // ✅ New overloaded method that accepts custom claims (e.g., Student_id)
    public string GenerateToken(Claim[] claims)
    {
        return GenerateTokenWithClaims(claims);
    }

    // 🔧 Shared internal token generator
    private string GenerateTokenWithClaims(Claim[] claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"], // You can adjust if needed
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
