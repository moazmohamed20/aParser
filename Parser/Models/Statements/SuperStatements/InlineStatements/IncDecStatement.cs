namespace aParser.Parser.Models.Statements.SuperStatements.InlineStatements
{
    public class IncDecStatement : IInlineStatement
    {
        public string Variable { get; set; }

        public string Operator { get; set; }
    }
}
