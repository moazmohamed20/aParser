namespace aParser.Parser.Models.Statements.StructStatements
{
    public class WhileStatement : IStructStatement
    {
        public IStatement Body { get; set; }

        public Condition Condition { get; set; }
    }
}
