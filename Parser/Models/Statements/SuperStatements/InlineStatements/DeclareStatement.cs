namespace aParser.Parser.Models.Statements.SuperStatements.InlineStatements
{
    public class DeclareStatement : IInlineStatement
    {
        public string DataType { get; set; }

        public string Variable { get; set; }
    }
}
