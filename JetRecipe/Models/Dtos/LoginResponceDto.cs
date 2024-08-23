namespace JetRecipe.Api.Models.Dtos
{
	public class LoginResponceDto
	{
		public UserDto User { get; set; }
		public string Token { get; set; }
	}
}
