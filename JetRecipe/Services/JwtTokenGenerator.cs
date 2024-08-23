using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JetRecipe.Api.Models;
using JetRecipe.Api.Services.IServices;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JetRecipe.Api.Services
{
	public class JwtTokenGenerator : IJwtTokenGenerator
	{
		private readonly JwtOptions _options;
        public JwtTokenGenerator(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }
        public string GenerateToken(AppUser user, IEnumerable<string> roles)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_options.Secret);
			var claims = new List<Claim>(){
				new Claim(JwtRegisteredClaimNames.Email,user.Email),
				new Claim(JwtRegisteredClaimNames.Sub,user.Id),
				new Claim(JwtRegisteredClaimNames.Name,user.UserName)
			};
			claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
			var description = new SecurityTokenDescriptor()
			{
				Issuer = _options.Issuer,
				Audience = _options.Audience,
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddDays(1),
				SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(description);
			return tokenHandler.WriteToken(token);
		}
	}
}
