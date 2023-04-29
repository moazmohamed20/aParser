namespace aParser.Parser.Models.Statements.SuperStatements.InlineStatements
{
    public class DecsignStatement : IInlineStatement
    {
        public string DataType { get; set; }

        public string Variable { get; set; }

        public string Expression { get; set; }
    }
}
