using System.ComponentModel.DataAnnotations;

namespace JetRecipe.Api.Models
{
	public class Category
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
	}
}
