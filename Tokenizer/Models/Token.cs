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

    public enum TokenType
    {
        // ----------- Keywords ----------- //
        Using,               // using
        Class,               // class
        If,                  // if
        Else,                // else
        For,                 // for
        Do,                  // do
        While,               // while
        Switch,              // switch
        Case,                // case
        Break,               // break
        Default,             // default
        Return,              // return
        Null,                // null
        True,                // true
        False,               // false
        DataType,            // void | var | bool | char | short | int | long | float | double | decimal | String | string

        // ------------ Values ------------ //
        Number,              // .25 | 3.14
        String,              // "I am 'Moaz'"
        Comment,             // Any Character After (//) & Before (\r | \n | //)
        Identifier,          // fact | _private | iD1
        MultilineComment,    // Any Character After (/*) & Before (*/)

        // ---------- Operators ---------- //
        And,                 // && | &
        Or,                  // || | |
        Not,                 // !
        Equal,               // =
        PlusEqual,           // +=
        MinusEqual,          // -=
        DoubleEquals,        // ==
        NotEqual,            // !=
        LessThan,            // <
        GreaterThan,         // >
        LessThanOrEqual,     // <=
        GreaterThanOrEqual,  // >=

        // ----------- Symbols ----------- //
        OpenRoundBracket,    // (
        CloseRoundBracket,   // )
        OpenCurlyBracket,    // {
        CloseCurlyBracket,   // }
        OpenSquareBracket,   // [
        CloseSquareBracket,  // ]
        Plus,                // +
        Minus,               // -
        DoublePluses,        // ++
        DoubleMinuses,       // --
        Percent,             // %
        Asterisk,            // *
        BackSlash,           // \
        ForwardSlash,        // /
        DoubleForwardSlashes,// //
        ForwardSlashAsterisk,// /*
        AsteriskForwardSlash,// */
        Dot,                 // .
        Comma,               // ,
        Colon,               // :
        Semicolon            // ;
    }
}
