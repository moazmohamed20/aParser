using System.Text.RegularExpressions;

namespace aParser.Tokenizer.Models
{
    public class TokenDefinition
    {
        private readonly int _priority;
        private readonly Regex _tokenRegex;
        private readonly TokenType _tokenType;

        public TokenDefinition(TokenType tokenType, string tokenRegex, int priority = 0)
        {
            _tokenRegex = new Regex(tokenRegex, RegexOptions.Compiled);
            _tokenType = tokenType;
            _priority = priority;
        }

        public IEnumerable<Token> FindMatches(string source)
        {
            var matches = _tokenRegex.Matches(source);
            return matches.Select(match => new Token()
            {
                Type = _tokenType,
                Value = match.Value,
                Priority = _priority,
                StartIndex = match.Index,
                EndIndex = match.Index + match.Length,
            });
        }
    }
}
