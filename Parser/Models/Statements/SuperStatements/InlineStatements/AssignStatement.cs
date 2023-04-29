namespace aParser.Parser.Models.Statements.SuperStatements.InlineStatements
{
    public class AssignStatement : IInlineStatement
    {
        public string Variable { get; set; }

        public string Operator { get; set; }

        public string? Expression { get; set; }
    }
}
