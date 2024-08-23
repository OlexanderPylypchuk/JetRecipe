using JetRecipe.Api.Models.Dtos;

namespace JetRecipe.Api.Services.IServices
{
	public interface IAuthService
	{
		Task<string> Register(RegistrationRequestDto registrationRequestDto);
		Task<LoginResponceDto> Login(LoginRequestDto loginRequestDto);
		Task<bool> AssignRole(string email, string role);
	}
}
