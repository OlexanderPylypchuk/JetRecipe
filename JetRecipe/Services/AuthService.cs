using JetRecipe.Api.Data;
using JetRecipe.Api.Models;
using JetRecipe.Api.Models.Dtos;
using JetRecipe.Api.Services.IServices;
using Microsoft.AspNetCore.Identity;

namespace JetRecipe.Api.Services
{
	public class AuthService : IAuthService
	{
		private readonly AppDbContext _appDbContext;
		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public AuthService(AppDbContext appDbContext, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGeneratior)
        {
            _appDbContext = appDbContext;
			_userManager = userManager;
			_roleManager = roleManager;
			_jwtTokenGenerator = jwtTokenGeneratior;
        }
        public async Task<bool> AssignRole(string email, string role)
		{
			try
			{
				var user = _appDbContext.Users.Where(u => u.Email == email).FirstOrDefault();
				if (user == null)
				{
					return false;
				}
				if (await _roleManager.RoleExistsAsync(role.ToUpper()) != true)
				{
					await _roleManager.CreateAsync(new IdentityRole(role));
				}
				await _userManager.AddToRoleAsync(user, role);
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public async Task<LoginResponceDto> Login(LoginRequestDto loginRequestDto)
		{
			var responce = new LoginResponceDto();
			var user = _appDbContext.Users.FirstOrDefault(u=>u.NormalizedEmail==loginRequestDto.UserName.ToUpper());
			if (user == null)
			{
				return responce;
			}
			var result = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
			if (result == false)
			{
				return responce;
			}
			var userDto = new UserDto()
			{
				Email = user.Email,
				Id = user.Id,
				Name = user.Name,
				PhoneNumber = user.PhoneNumber
			};
			responce.User = userDto;
			responce.Token = _jwtTokenGenerator.GenerateToken(user, await _userManager.GetRolesAsync(user));
			return responce;
		}

		public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
		{
			AppUser applicationUser = new()
			{
				UserName = registrationRequestDto.Email,
				Email = registrationRequestDto.Email,
				NormalizedEmail = registrationRequestDto.Email.ToUpper(),
				PhoneNumber = registrationRequestDto.PhoneNumber,
				Name = registrationRequestDto.Name,
			};
			try
			{
				var result = await _userManager.CreateAsync(applicationUser, registrationRequestDto.Password);
				if (result.Succeeded)
				{
					var user = _appDbContext.Users.Where(u => u.Email == registrationRequestDto.Email).FirstOrDefault();
					if(!string.IsNullOrEmpty(registrationRequestDto.Role))
					{
						await AssignRole(user.Email, registrationRequestDto.Role);
					}
					UserDto userDTO = new()
					{
						Email = user.Email,
						Id = user.Id,
						Name = user.Name,
						PhoneNumber = user.PhoneNumber

					};
					return "";
				}
				return result.Errors.FirstOrDefault().Description;
			}
			catch (Exception ex)
			{

			}
			return "Error occured";
		}
	}
}
