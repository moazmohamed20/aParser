using aParser.Parser.Models.Statements.SuperStatements.InlineStatements;

namespace aParser.Parser.Models.Statements.SuperStatements
{
    public class FunctionStatement : ISuperStatement
    {
        public string ReturnType { get; set; }

        public string Name { get; set; }

        public IEnumerable<DeclareStatement> Parameters { get; set; }

        public IEnumerable<IStatement> Statements { get; set; }
    }
}
