using JetRecipe.Api.Data;
using JetRecipe.Api.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JetRecipe.Api.Controllers
{
	[Route("api/category")]
	[ApiController]
	public class CategoryController : ControllerBase
	{
		private readonly AppDbContext _db;
		protected ResponceDto _responceDto;
        public CategoryController(AppDbContext db)
        {
			_db = db;
			_responceDto = new ResponceDto();
        }
        [HttpGet]
		public async Task<ResponceDto> Get()
		{
			try
			{
				var list = await _db.Categories.ToListAsync();
				_responceDto.Success = true;
				_responceDto.Result = list;
				return _responceDto;
			}
			catch (Exception ex)
			{
				_responceDto.Success = false;
				_responceDto.Message = ex.Message;
				return _responceDto;
			}
			
		}
	}
}
