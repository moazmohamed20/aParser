namespace aParser.Parser.Models
{
    public class Condition
    {
        public string Left { get; set; }

        public string Operator { get; set; }

        public string Right { get; set; }

        public bool? Value { get; set; }
    }
}
