namespace MarkdownNoteTaking.Models
{
	public class GrammarCheckRequest
	{
		public string Content { get; set; }
	}

	public class GrammarCheckResponse
	{
		public bool HasErrors { get; set; }
		public List<string> Errors { get; set; }
	}
}
