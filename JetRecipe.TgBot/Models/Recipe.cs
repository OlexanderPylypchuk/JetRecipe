namespace JetRecipe.TgBot.Models
{
	public class Recipe
	{
		public int Id { get; set; }
		public string DishName { get; set; }
		public string Description { get; set; }
		public string Ingridients { get; set; }
		public string ImgUrl { get; set; }
		public string StepByStepExplanation { get; set; }
		public int Difficulty { get; set; }
		public int CategoryId { get; set; }
		public Category Category { get; set; }
	}
}
