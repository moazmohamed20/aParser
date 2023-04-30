namespace aParser.Parser.Models.Statements.StructStatementsCases
{
    public class CaseStatement : ICase
    {
        public string Constant { get; set; }

        public IStatement Statement { get; set; }
    }
}
