using JetRecipe.Api.Models.Dtos;
using JetRecipe.Api.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JetRecipe.Api.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		protected ResponceDto _responceDTO;
		public AuthController(IAuthService authService)
		{
			_authService = authService;
			_responceDTO = new ResponceDto();

		}
		[HttpPost("register")]
		public async Task<IActionResult> Register(RegistrationRequestDto registrationRequestDto)
		{
			var errormessage = await _authService.Register(registrationRequestDto);
			if(!string.IsNullOrEmpty(errormessage))
			{
				_responceDTO.Success = false;
				_responceDTO.Message = errormessage;
				return BadRequest(errormessage);
			}
			_responceDTO.Success = true;
			return Ok(_responceDTO);
		}
		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
		{
			var loginresponce = await _authService.Login(loginRequestDto);
			if (loginresponce.User == null)
			{
				_responceDTO.Success = false;
				_responceDTO.Message = "Username or password is incorrect";
				return BadRequest(_responceDTO);
			}
			_responceDTO.Success = true;
			_responceDTO.Result = loginresponce;
			return Ok(_responceDTO);
		}
		[HttpPost("AssignRole")]
		public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto registrationRequestDto)
		{
			var assignRoleSuccessful = await _authService.AssignRole(registrationRequestDto.Email, registrationRequestDto.Role.ToUpper());
			if (!assignRoleSuccessful)
			{
				_responceDTO.Success = false;
				_responceDTO.Message = "Error occured";
				return BadRequest(_responceDTO);
			}
			_responceDTO.Success = true;
			return Ok(_responceDTO);
		}
	}
}
