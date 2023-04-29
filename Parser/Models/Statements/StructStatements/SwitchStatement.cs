using aParser.Parser.Models.Statements.StructStatementsCases;

namespace aParser.Parser.Models.Statements.StructStatements
{
    public class SwitchStatement : IStructStatement
    {
        public IEnumerable<ICase> Cases { get; set; }
    }
}
