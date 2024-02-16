using aParser.Tokenizer.Models;

namespace aParser.Tokenizer
{
    public class Tokenizer
    {
        private readonly List<TokenDefinition> _tokenDefinitions = new List<TokenDefinition>
        {
            // Keywords
            new TokenDefinition(TokenType.Using, @"using"),
            new TokenDefinition(TokenType.Class, @"class"),
            new TokenDefinition(TokenType.If, @"if"),
            new TokenDefinition(TokenType.Else, @"else"),
            new TokenDefinition(TokenType.For, @"for"),
            new TokenDefinition(TokenType.Do, @"do", 1),
            new TokenDefinition(TokenType.While, @"while"),
            new TokenDefinition(TokenType.Switch, @"switch"),
            new TokenDefinition(TokenType.Case, @"case"),
            new TokenDefinition(TokenType.Default, @"default"),
            new TokenDefinition(TokenType.Break, @"break"),
            new TokenDefinition(TokenType.Return, @"return"),
            new TokenDefinition(TokenType.Null, @"null"),
            new TokenDefinition(TokenType.True, @"true"),
            new TokenDefinition(TokenType.False, @"false"),
            new TokenDefinition(TokenType.DataType, @"(void|var)|(bool|char|short|int|long|float|double|decimal|String|string)(\[\]|\?)?"),

            // Values
            new TokenDefinition(TokenType.Number, @"\d*\.\d+|\d+"),
            new TokenDefinition(TokenType.String, @"""[^""]*"""),
            new TokenDefinition(TokenType.Identifier, @"[a-zA-Z_]\w*", 1),
            new TokenDefinition(TokenType.Comment, @"(?<=//).*?(?=(\r|\n|//))"),
            new TokenDefinition(TokenType.MultilineComment, @"(?<=/\*)(?:(?!\*/)(?:.|[\r\n]))*(?=\*/)"),

            // Operators
            new TokenDefinition(TokenType.And, @"&&|&"),
            new TokenDefinition(TokenType.Or, @"\|\||\|"),
            new TokenDefinition(TokenType.Not, @"!", 1),
            new TokenDefinition(TokenType.Equal, @"=", 1),
            new TokenDefinition(TokenType.PlusEqual, @"\+="),
            new TokenDefinition(TokenType.MinusEqual, @"-="),
            new TokenDefinition(TokenType.DoubleEquals, @"=="),
            new TokenDefinition(TokenType.NotEqual, @"!="),
            new TokenDefinition(TokenType.LessThan, @"<", 1),
            new TokenDefinition(TokenType.GreaterThan, @">", 1),
            new TokenDefinition(TokenType.LessThanOrEqual, @"<="),
            new TokenDefinition(TokenType.GreaterThanOrEqual, @">="),
            
            // Symbols
            new TokenDefinition(TokenType.OpenRoundBracket, @"\("),
            new TokenDefinition(TokenType.CloseRoundBracket, @"\)"),
            new TokenDefinition(TokenType.OpenCurlyBracket, @"{"),
            new TokenDefinition(TokenType.CloseCurlyBracket, @"}"),
            new TokenDefinition(TokenType.OpenSquareBracket, @"\["),
            new TokenDefinition(TokenType.CloseSquareBracket, @"\]"),
            new TokenDefinition(TokenType.Plus, @"\+", 1),
            new TokenDefinition(TokenType.Minus, @"-", 1),
            new TokenDefinition(TokenType.DoublePluses, @"\+\+"),
            new TokenDefinition(TokenType.DoubleMinuses, @"--"),
            new TokenDefinition(TokenType.Percent, @"%"),
            new TokenDefinition(TokenType.Asterisk, @"\*", 1),
            new TokenDefinition(TokenType.BackSlash, @"\\"),
            new TokenDefinition(TokenType.ForwardSlash, @"/", 1),
            new TokenDefinition(TokenType.DoubleForwardSlashes, @"//"),
            new TokenDefinition(TokenType.ForwardSlashAsterisk, @"/\*"),
            new TokenDefinition(TokenType.AsteriskForwardSlash, @"\*/"),
            new TokenDefinition(TokenType.Dot, @"\."),
            new TokenDefinition(TokenType.Comma, @","),
            new TokenDefinition(TokenType.Colon, @":"),
            new TokenDefinition(TokenType.Semicolon, @";"),
        };

        public IEnumerable<Token> Tokenize(string source)
        {
            var allTokenMatches = FindAllTokenMatches(source);
            var groupedByIndex = allTokenMatches.GroupBy(m => m.StartIndex).OrderBy(x => x.Key).ToList();

            Token? lastMatch = null;
            foreach (var group in groupedByIndex)
            {
                var highPriorityMatch = group.OrderBy(m => m.Priority).First(); // Take The Highest Priority Match
                if (lastMatch != null && highPriorityMatch.StartIndex < lastMatch.EndIndex) // Take What Started First
                    continue;
                lastMatch = highPriorityMatch;
                yield return highPriorityMatch;
            };
        }

        private IEnumerable<Token> FindAllTokenMatches(string source)
        {
            var allTokenMatches = new List<Token>();

            foreach (var tokenDefinition in _tokenDefinitions)
                allTokenMatches.AddRange(tokenDefinition.FindMatches(source));

            return allTokenMatches;
        }
    }
}
