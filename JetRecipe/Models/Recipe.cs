using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace JetRecipe.Api.Models
{
	public class Recipe
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string DishName { get; set; }
		[Required]
		public string Description { get; set; }
		[Required]
		public string Ingridients { get; set; }
		[Required]
		public string ImgUrl { get; set; }
		[Required]
		[StringLength(2000)]
		public string StepByStepExplanation { get; set; }
		[Required]
		[Range(1,5)]
		public int Difficulty { get; set; }
		[Required]
		[ForeignKey(nameof(Category))]
		public int CategoryId { get; set; }
		[ValidateNever]
		public Category Category { get; set; }
	}
}
