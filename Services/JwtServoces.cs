using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using auth3.Models;
using Microsoft.IdentityModel.Tokens;

namespace auth3.Services;

public class JwtService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;

    public JwtService(IConfiguration config)
    {
        _config = config;
        var secretKey = _config["JwtServices:SecretKey"];
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

    }
    public string GenerateToken(User user)
    {
        var claims = new []
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Name,(user.Name ?? "Unknown")),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["JwtSettings:ExpInMinutes"])),
            Issuer = _config["JwtSettings:Issure"],
            Audience = _config["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(_key,SecurityAlgorithms.HmacSha256)
        };
        var tokenHadler = new JwtSecurityTokenHandler();
        var token = tokenHadler.CreateToken(tokenDescriptor);
        return tokenHadler.WriteToken(token);

    }
}