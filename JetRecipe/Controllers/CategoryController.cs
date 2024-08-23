using JetRecipe.Api.Data;
using JetRecipe.Api.Models;
using JetRecipe.Api.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
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
		[HttpGet("{id:int}")]
		public async Task<ResponceDto> Get(int id)
		{
			try
			{
				var item = await _db.Categories.FirstOrDefaultAsync(c=>c.Id==id);
				_responceDto.Success = true;
				_responceDto.Result = item;
				return _responceDto;
			}
			catch (Exception ex)
			{
				_responceDto.Success = false;
				_responceDto.Message = ex.Message;
				return _responceDto;
			}

		}
		[HttpPost]
		[Authorize(Roles = "admin")]
		public async Task<ResponceDto> Create([FromBody]Category category)
		{
			try
			{
				await _db.Categories.AddAsync(category);
				await _db.SaveChangesAsync();
				_responceDto.Success = true;
				_responceDto.Result = category;
			}
			catch (Exception ex)
			{
				_responceDto.Success = false;
				_responceDto.Message = ex.Message;
			}
			return _responceDto;
		}
		[HttpPut]
		[Authorize(Roles = "admin")]
		public async Task<ResponceDto> Update([FromBody] Category category)
		{
			try
			{
				_db.Categories.Update(category);
				await _db.SaveChangesAsync();
				_responceDto.Success = true;
				_responceDto.Result = category;
			}
			catch (Exception ex)
			{
				_responceDto.Success = false;
				_responceDto.Message = ex.Message;
			}
			return _responceDto;
		}
		[HttpDelete]
		[Authorize(Roles = "admin")]
		[Route("/api/recipe/{id:int}")]
		public async Task<ResponceDto> Delete(int id)
		{
			try
			{
				var category = await _db.Categories.FindAsync(id);
				_db.Categories.Remove(category);
				await _db.SaveChangesAsync();
				_responceDto.Success = true;
				_responceDto.Result = category;
			}
			catch (Exception ex)
			{
				_responceDto.Success = false;
				_responceDto.Message = ex.Message;
			}
			return _responceDto;
		}
	}
}
