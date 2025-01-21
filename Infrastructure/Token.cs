using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public static class Token
{
    public static string Generator(IConfiguration configuration, string userName, int businessEntityId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(configuration["JwtSettings:Key"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, "Customer"),
                new Claim("BusinessEntityId", businessEntityId.ToString()) // BusinessEntityId claim'i eklendi
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = configuration["JwtSettings:Issuer"],
            Audience = configuration["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }


    public static int GetBusinessEntityId(ClaimsPrincipal user)
    {
        var claimsIdentity = user.Identity as ClaimsIdentity;
        var userIdClaim = claimsIdentity?.FindFirst("BusinessEntityId");

        if (userIdClaim == null)
        {
            throw new UnauthorizedAccessException("Authentication failed. BusinessEntityId not found in token.");
        }

        return int.Parse(userIdClaim.Value);
    }
}