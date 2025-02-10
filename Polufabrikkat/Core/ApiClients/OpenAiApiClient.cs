using Microsoft.Extensions.Options;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models;
using Polufabrikkat.Core.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Polufabrikkat.Core.ApiClients
{
	public class OpenAiApiClient : IOpenAiApiClient
	{
        private readonly OpenAiApiOptions _apiOptions;
        private readonly HttpClient _httpClient;



		public OpenAiApiClient(HttpClient httpClient, IOptions<OpenAiApiOptions> apiOptions)
		{
			_apiOptions = apiOptions.Value;
            _httpClient = httpClient;
		}

		public async Task<string> GenerateTextAsync(string prompt)
		{
			var requestBody = new
			{
				model = "gpt-4o",
				messages = new[]
				{
				new { role = "system", content = "Задание:\nТвоя задача — создавать оригинальные текстовые публикации для TikTok, которые побуждают пользователей отмечать в комментариях своих близких, особенно вторую половинку. Тексты должны быть искренними, трогательными и напоминать людям о важности их отношений. Они должны быть схожи по стилю и настроению с приведенными примерами, но не повторять их. Не используй чрезмерно художественный или поэтичный язык. Старайся не писать в конце \"отмечай\", пусть люди интуитивно будут хотеть это сделать.\nПримеры:\n1. Знаешь, а я ведь полюбил тебя не из-за твоей фигуры, не из-за каких-то грязных мыслей. Влюбился в твои глаза, в твой голос, в твой характер... Я просто полюбил тебя такую, какая ты есть.\n2. Когда мои дети спросят меня, кто был моей первой настоящей любовью, я не хочу садиться и рассказывать им. Я хочу, чтобы они видели, как ты садишься рядом со мной.\n3. Да плевать на всех, ты главное кушай, не болей, одевайся теплее и пиши мне, когда тебе плохо. Я переживаю, и береги себя, нам еще вместе семью строить.\n4. Спасибо тебе за всё, вот реально. Ты самый лучший человек, от которого зависит многое. Ты заставляешь меня улыбаться даже в самые ужасные дни. Я очень дорожу тобой.\n5. Единственное, о чем я правда мечтаю, — это пройти с тобой через все трудности вместе и в конечном итоге сесть в обнимку, глядя друг на друга, сказать: «Мы справились».\n6. бро, хватит притворятся, я знаю что ты миллионер, купи мне бмв, а то тяжело уже ходить пешком\n7. Если ты показываешь мне свою слабую сторону или место, где болит, это не значит, что я тут же отвернусь от тебя или ударю посильнее. Я буду знать, где тебя больше любить, и буду знать, от чего тебя оберегать" },
				new { role = "user", content = prompt }
				},
				max_tokens = 2048,
				temperature = 1,
				n = 1
			};

			var url = "https://api.openai.com/v1/chat/completions";

			using var request = new HttpRequestMessage(HttpMethod.Post, url);
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiOptions.ApiKey);
			request.Content = JsonContent.Create(requestBody, new MediaTypeHeaderValue("application/json"));
			using var res = await _httpClient.SendAsync(request);

			if (!res.IsSuccessStatusCode)
			{
				throw new Exception($"OpenAI API Error: {res.StatusCode}");
			}

            var content = await res.Content.ReadFromJsonAsync<OpenAiApiResponse>(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });

            return content.Choices.FirstOrDefault()?.Message.Content;
		}

	}
}