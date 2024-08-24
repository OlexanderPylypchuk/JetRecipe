using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using JetRecipe.TgBot.Models;
using JetRecipe.Api.Models.Dtos;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace JetRecipe.TgBot
{
	class Program
	{
		static HttpClient client = new HttpClient();
        private static ITelegramBotClient _botClient;
		static ReplyKeyboardMarkup replyKeyboardMarkup;
		private static ReceiverOptions _receiverOptions;

		static async Task Main()
		{
			client.BaseAddress = new Uri("https://localhost:8000/");
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
			_botClient = new TelegramBotClient("7509428339:AAGlp9-kDx54ABGOuORDBBRb5uTjDhbbwbM"); 
			_receiverOptions = new ReceiverOptions
			{
				AllowedUpdates = new[] 
				{
				UpdateType.Message,
				UpdateType.CallbackQuery
            },
				ThrowPendingUpdates = true,
			};

			using var cts = new CancellationTokenSource();
			replyKeyboardMarkup = new(new[]
			{
				new KeyboardButton("Random recipe"),
				new KeyboardButton("Choose category")
			})
			{
				ResizeKeyboard = true
			};
			_botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token);
			var me = await _botClient.GetMeAsync(); 
			Console.WriteLine($"{me.FirstName}");
			await Task.Delay(-1);
		}
		private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			try
			{
				ChatId chatId = update.Message == null ? update.CallbackQuery.Message.Chat.Id : update.Message.Chat.Id;
				switch (update.Type)
				{
					case UpdateType.Message:
						{
							if (update.Message.Text == "Random recipe")
							{
								var apiResponce = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri: "/api/recipe/random"));
								if (apiResponce != null)
								{
									string s = await apiResponce.Content.ReadAsStringAsync();
									var responceDto = JsonConvert.DeserializeObject<ResponceDto>(s);
									var recipe = JsonConvert.DeserializeObject<Recipe>(Convert.ToString(responceDto.Result));
									var sb = new StringBuilder();
									sb.AppendLine($"{recipe.DishName}");
									sb.AppendLine($"Difficulty - {recipe.Difficulty}/5");
									sb.AppendLine($"{recipe.Description}");
									sb.AppendLine($"Ingridients: {recipe.Ingridients}");
									sb.AppendLine($"{recipe.StepByStepExplanation}");
									await botClient.SendTextMessageAsync(chatId, sb.ToString());
									break;
								}
								await botClient.SendTextMessageAsync(chatId, "wrong number");
								break;
							}
							else if (update.Message.Text == "Choose category")
							{
								var apiResponce = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri: "/api/category"));
								if (apiResponce != null)
								{
									string s = await apiResponce.Content.ReadAsStringAsync();
									var responceDto = JsonConvert.DeserializeObject<ResponceDto>(s);
									var categories = JsonConvert.DeserializeObject<List<Category>>(Convert.ToString(responceDto.Result));
									var buttons = new InlineKeyboardButton[categories.Count];
									for(int i = 0; i<categories.Count; i++)
									{
										buttons[i] = InlineKeyboardButton.WithCallbackData(categories[i].Name, categories[i].Id.ToString()); 
									}
									var reply = new InlineKeyboardMarkup(buttons);
									Message message = await botClient.SendTextMessageAsync(
										chatId: chatId,
										text: "Choose category",
										replyMarkup: reply,
										cancellationToken: cancellationToken);
									Console.WriteLine(message);
									return;
								}
							}
							else
							{
								if (update.Message.Text != @"/start") 
									await botClient.SendTextMessageAsync(chatId, "No such command");
								Message sentMessage = await botClient.SendTextMessageAsync(
									chatId: chatId,
									text: "Choose option",
									replyMarkup: replyKeyboardMarkup,
									cancellationToken: cancellationToken);
								break;
							}
							break;
						}
					case UpdateType.CallbackQuery:
						{
							int data = int.Parse(update.CallbackQuery.Data);
							var apiResponce = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri: $"/api/recipe/random/{data}"));
							if (apiResponce != null)
							{
								string s = await apiResponce.Content.ReadAsStringAsync();
								var responceDto = JsonConvert.DeserializeObject<ResponceDto>(s);
								var recipe = JsonConvert.DeserializeObject<Recipe>(Convert.ToString(responceDto.Result));
								var sb = new StringBuilder();
								if (recipe != null)
								{
									sb.AppendLine($"{recipe.DishName}");
									sb.AppendLine($"Difficulty - {recipe.Difficulty}/5");
									sb.AppendLine($"{recipe.Description}");
									sb.AppendLine($"Ingridients: {recipe.Ingridients}");
									sb.AppendLine($"{recipe.StepByStepExplanation}");
								}
								else
								{
									sb.AppendLine("This category is currently empty, you can try other ones! :)");
								}
								
								await botClient.SendTextMessageAsync(chatId, sb.ToString());
								break;
							}
							await botClient.SendTextMessageAsync(chatId, "wrong number");
							break;
						}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
		{
			var ErrorMessage = error switch
			{
				ApiRequestException apiRequestException
					=> $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
				_ => error.ToString()
			};

			Console.WriteLine(ErrorMessage);
			return Task.CompletedTask;
		}
	}
}