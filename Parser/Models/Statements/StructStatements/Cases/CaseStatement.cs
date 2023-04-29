namespace aParser.Parser.Models.Statements.StructStatementsCases
{
    public class CaseStatement : ICase
    {
        public string Value { get; set; }

        public IStatement Statement { get; set; }
    }
}
