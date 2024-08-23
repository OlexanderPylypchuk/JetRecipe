using JetRecipe.Api.Data;
using JetRecipe.Api.Models;
using JetRecipe.Api.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.EntityFrameworkCore;

namespace JetRecipe.Api.Controllers
{
	[Route("api/recipe")]
	[ApiController]
	public class RecipeController : ControllerBase
	{
		private readonly AppDbContext _appDbContext;
		public ResponceDto ResponceDto;
        public RecipeController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
			ResponceDto = new ResponceDto();
        }
		[HttpGet]
		public async Task<ResponceDto> GetAllRecipes()
		{
			try
			{
				var list = await _appDbContext.Recipes.Include(r => r.Category).ToListAsync();
				ResponceDto.Result = list;
				ResponceDto.Success = true;
			}
			catch (Exception ex)
			{
				ResponceDto.Success = false;
				ResponceDto.Message = ex.Message;
			}
			return ResponceDto;
		}
		[HttpGet]
		[Route("/api/recipe/random")]
		public async Task<ResponceDto> GetRandomRecipe()
		{
			try
			{
				var list = await _appDbContext.Recipes.Include(r=>r.Category).ToListAsync();
				var item = list[new Random().Next(list.Count())];
				ResponceDto.Result = item;
				ResponceDto.Success = true;
			}
			catch (Exception ex)
			{
				ResponceDto.Success = false;
				ResponceDto.Message = ex.Message;
			}
			return ResponceDto;
		}
		[HttpGet]
		[Route("/api/recipe/random/{Categoryid:int}")]
		public async Task<ResponceDto> GetRandomRecipe(int Categoryid)
		{
			try
			{
				var list = await _appDbContext.Recipes.Where(r => r.CategoryId == Categoryid).Include(r => r.Category).ToListAsync();
				if(list.Count == 0)
				{
					throw new Exception("No recipies in such category");
				}
				var item = list[new Random().Next(list.Count())];
				ResponceDto.Result = item;
				ResponceDto.Success = true;
			}
			catch (Exception ex)
			{
				ResponceDto.Success = false;
				ResponceDto.Message = ex.Message;
			}
			return ResponceDto;
		}
		[Authorize(Roles ="admin")]
		[HttpPost]
		public async Task<ResponceDto> CreateRecipe([FromBody]Recipe recipe)
		{
			try
			{
				recipe.Category = _appDbContext.Categories.Where(c => c.Id == recipe.CategoryId).FirstOrDefault();
				if(recipe.Category == null)
				{
					throw new Exception("Couldnt find category with such id");
				}
				await _appDbContext.Recipes.AddAsync(recipe);
				await _appDbContext.SaveChangesAsync();
				ResponceDto.Success = true;
				ResponceDto.Result=recipe;
			}
			catch (Exception ex)
			{
				ResponceDto.Success = false;
				ResponceDto.Message = ex.Message;
			}
			return ResponceDto;
		}
		[HttpPut]
		[Authorize(Roles = "admin")]
		public async Task<ResponceDto> UpdateRecipe([FromBody]Recipe recipe)
		{
			try
			{
				_appDbContext.Recipes.Update(recipe);
				await _appDbContext.SaveChangesAsync();
				ResponceDto.Success = true;
				ResponceDto.Result = recipe;
			}
			catch (Exception ex)
			{
				ResponceDto.Success = false;
				ResponceDto.Message = ex.Message;
			}
			return ResponceDto;
		}
		[HttpDelete]
		[Authorize(Roles = "admin")]
		[Route("/api/recipe/{id:int}")]
		public async Task<ResponceDto> Delete(int id)
		{
			try
			{
				var recipe = await _appDbContext.Recipes.FindAsync(id);
				_appDbContext.Recipes.Remove(recipe);
				await _appDbContext.SaveChangesAsync();
				ResponceDto.Success = true;
				ResponceDto.Result = recipe;
			}
			catch (Exception ex)
			{
				ResponceDto.Success = false;
				ResponceDto.Message = ex.Message;
			}
			return ResponceDto;
		}
	}
}
