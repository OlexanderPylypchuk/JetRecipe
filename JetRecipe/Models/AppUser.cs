using Microsoft.AspNetCore.Identity;

namespace JetRecipe.Api.Models
{
	public class AppUser:IdentityUser
	{
		public string Name { get; set; }
	}
}
