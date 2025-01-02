
using Markdig;
using MarkdownNoteTaking.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MarkdownNoteTaking.Services
{
	public static class GrammarChecker
	{
		private static readonly HttpClient client = new HttpClient();
		public static async Task<GrammarCheckResponse> CheckGrammarAsync(string content)
		{
			var response = new GrammarCheckResponse
			{
				HasErrors = false,
				Errors = new List<string>()
			};

			var plainTextContent = Markdown.ToPlainText(content);

			var requestContent = new StringContent(JsonSerializer.Serialize(new { text = plainTextContent, language = "en-US" }), Encoding.UTF8, "application/json");
			var httpResponse = await client.PostAsync("https://api.languagetool.org/v2/check", requestContent);

			if (httpResponse.IsSuccessStatusCode)
			{
				var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
				var result = JsonSerializer.Deserialize<LanguageToolResponse>(jsonResponse);

				if (result != null && result.Matches.Any())
				{
					response.HasErrors = true;
					response.Errors = result.Matches.Select(m => m.Message).ToList();
				}
			}
			return response;
		}

		private class LanguageToolResponse
		{
			public List<Match> Matches { get; set; }
		}
		private class Match
		{
			public string Message { get; set; }
		}
	}
}
