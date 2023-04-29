namespace aParser.Parser.Models.Statements.StructStatements
{
    public class DoWhileStatement : IStructStatement
    {
        public Condition Condition { get; set; }

        public IStatement Body { get; set; }
    }
}
