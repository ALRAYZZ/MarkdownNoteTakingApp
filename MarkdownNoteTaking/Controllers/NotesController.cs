using Microsoft.AspNetCore.Mvc;
using Markdig;
using MarkdownNoteTaking.Models;
using MarkdownNoteTaking.Services;

namespace MarkdownNoteTaking.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class NotesController : Controller
	{
		private static List<NoteModel> notes = new List<NoteModel>();
		private static int nextId = 1;

		static NotesController()
		{
			notes = JsonSaver.LoadNotesFromFile();
			nextId = notes.Count > 0 ? notes.Max(n => n.Id) + 1 : 1;
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] NoteCreateModel noteCreateModel)
		{
			var grammarCheckResponse = await GrammarChecker.CheckGrammarAsync(noteCreateModel.Content);
			if (grammarCheckResponse.HasErrors)
			{
				return BadRequest(grammarCheckResponse.Errors);
			}

			var note = new NoteModel
			{
				Id = nextId++,
				Content = noteCreateModel.Content,
				HtmlContent = Markdown.ToHtml(noteCreateModel.Content)
			};
			notes.Add(note);

			JsonSaver.SaveNotesToFile(notes);

			return CreatedAtAction(nameof(Get), new { id = note.Id }, note);
		}

		[HttpGet]
		public IActionResult GetAll()
		{
			return Ok(notes);
		}

		[HttpGet("{id}")]
		public IActionResult Get(int id)
		{
			var note = notes.FirstOrDefault(n => n.Id == id);
			if (note == null)
			{
				return NotFound();
			}
			return Ok(note);
		}

		[HttpGet("{id}/html")]
		public IActionResult GetHtml(int id)
		{
			var note = notes.FirstOrDefault(n => n.Id == id);
			if (note == null)
			{
				return NotFound();
			}
			return Content(note.HtmlContent, "text/html");
		}

		[HttpPost("checkgrammar")]
		public async Task<IActionResult> CheckGrammar([FromBody] GrammarCheckRequest request)
		{
			var response = await GrammarChecker.CheckGrammarAsync(request.Content);
			return Ok(response);
		}
	}
}
