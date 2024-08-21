using JetRecipe.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JetRecipe.Api.Data
{
	public class AppDbContext: IdentityDbContext<AppUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{

		}
		public DbSet<Category> Categories { get; set; }
		public DbSet<Recipe> Recipes { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<Category>().HasData(new Category[]
			{
				new Category { Id = 1, Name = "Pasta" },
				new Category { Id = 2, Name = "Salad"},
				new Category { Id = 3, Name = "Soup"}
			});
			modelBuilder.Entity<Recipe>().HasData(new Recipe[]
			{
				new Recipe { Id = 1, DishName = "Carbonara", CategoryId = 1, Description = "We will work on this", Difficulty=2, ImgUrl="https://placehold.co/600x400", Ingridients="Pasta, Bacon, Egg, Cream", StepByStepExplanation="We will work on this" },
				new Recipe { Id = 2, DishName = "Bolognese", CategoryId = 1, Description = "We will work on this", Difficulty=2, ImgUrl="https://placehold.co/600x400", Ingridients="We will work on this", StepByStepExplanation="We will work on this" }
			});
		}

	}
}
