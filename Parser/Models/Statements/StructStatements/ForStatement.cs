using aParser.Parser.Models.Statements.SuperStatements.InlineStatements;

namespace aParser.Parser.Models.Statements.StructStatements
{
    public class ForStatement : IStructStatement
    {
        public IInlineStatement Prefix { get; set; }

        public Condition Condition { get; set; }

        public IInlineStatement Repeat { get; set; }

        public IStatement Body { get; set; }
    }
}
