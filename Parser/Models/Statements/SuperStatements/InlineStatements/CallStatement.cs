namespace aParser.Parser.Models.Statements.SuperStatements.InlineStatements
{
    public class CallStatement : IInlineStatement
    {
        public IEnumerable<string> Path { get; set; }

        public IEnumerable<string> Parameters { get; set; }
    }
}
