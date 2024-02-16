namespace aParser.Tokenizer.Models
{
    public class Token
    {
        public TokenType Type { get; set; }

        public string Value { get; set; } = string.Empty;

        public int Priority { get; set; }

        public int StartIndex { get; set; }

        public int EndIndex { get; set; }

        public override string ToString() => $"[{Type}, '{Value}']";
    }
}
