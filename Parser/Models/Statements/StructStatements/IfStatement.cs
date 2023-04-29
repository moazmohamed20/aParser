namespace aParser.Parser.Models.Statements.StructStatements
{
    public class IfStatement : IStructStatement
    {
        public Condition Condition { get; set; }

        public IStatement Body { get; set; }

        public IStatement? ElseBody { get; set; }
    }
}
