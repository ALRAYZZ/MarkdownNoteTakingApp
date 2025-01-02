using MarkdownNoteTaking.Models;
using System.Text.Json;

namespace MarkdownNoteTaking.Services
{
	public static class JsonSaver
	{
		private static readonly string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Notes", "notes.json");
		public static void SaveNotesToFile(List<NoteModel> notes)
		{
			var json = JsonSerializer.Serialize(notes, new JsonSerializerOptions { WriteIndented = true });
			Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			File.WriteAllText(filePath, json);
		}

		public static List<NoteModel> LoadNotesFromFile()
		{
			if (File.Exists(filePath))
			{
				var json = File.ReadAllText(filePath);
				return JsonSerializer.Deserialize<List<NoteModel>>(json) ?? new List<NoteModel>();
			}
			return new List<NoteModel>();
		}
	}
}
