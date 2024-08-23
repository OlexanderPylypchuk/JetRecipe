using JetRecipe.Api.Models;

namespace JetRecipe.Api.Services.IServices
{
	public interface IJwtTokenGenerator
	{
		string GenerateToken(AppUser user, IEnumerable<string> roles);
	}
}
