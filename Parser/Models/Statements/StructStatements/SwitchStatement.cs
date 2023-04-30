using aParser.Parser.Models.Statements.StructStatementsCases;

namespace aParser.Parser.Models.Statements.StructStatements
{
    public class SwitchStatement : IStructStatement
    {
        public string Expression { get; set; }

        public IEnumerable<ICase> Cases { get; set; }
    }
}
