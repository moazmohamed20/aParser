using aParser.Parser.Models.Statements.SuperStatements;

namespace aParser.Parser.Models
{
    public class Class
    {
        public string Name { get; set; }

        public IEnumerable<ISuperStatement> Statements { get; set; }
    }
}
