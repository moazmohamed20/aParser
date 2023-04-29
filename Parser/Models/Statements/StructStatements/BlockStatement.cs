namespace aParser.Parser.Models.Statements.StructStatements
{
    public class BlockStatement : IStructStatement
    {
        public IEnumerable<IStatement> Statements { get; set; }
    }
}
