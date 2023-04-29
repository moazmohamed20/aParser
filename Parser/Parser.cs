using aParser.Parser.Models;
using aParser.Parser.Models.Statements;
using aParser.Parser.Models.Statements.StructStatements;
using aParser.Parser.Models.Statements.StructStatementsCases;
using aParser.Parser.Models.Statements.SuperStatements;
using aParser.Parser.Models.Statements.SuperStatements.InlineStatements;
using aParser.Tokenizer.Models;

namespace aParser.Parser
{
    /*
     * ************************************************************
     *                    Context Free Grammar                    *
     * ************************************************************
     * PROGRAM --> IMPORTS CLASSES
     *
     * IMPORTS          --> IMPORT_STATEMENT IMPORTS | ε
     * IMPORT_STATEMENT --> using IDS;
     *
     * CLASSES          --> CLASS_STATEMENT CLASSES | ε
     * CLASS_STATEMENT  --> class id { SUPER_STATEMENTS }
     *
     *
     *
     * SUPER_STATEMENTS --> SUPER_STATEMENT SUPER_STATEMENTS | ε
     * SUPER_STATEMENT  --> COMMENT_STATEMENT | FUNCTION_STATEMENT | INLINE_STATEMENT
     *
     * COMMENT_STATEMENT  --> // comment | /* multiline_comment *\/
     * FUNCTION_STATEMENT --> DATA_TYPE id (DECLARES) { STATEMENTS }
     * INLINE_STATEMENT     --> DECSIGN_STATEMENT | DECLARE_STATEMENT | INC_DEC_STATEMENT | ASSIGN_STATEMENT | CALL_STATEMENT
     *   DECSIGN_STATEMENT  --> DATA_TYPE id = EXPRESSION
     *   DECLARE_STATEMENT  --> DATA_TYPE id
     *   INC_DEC_STATEMENT  --> id INC_DEC_OPERATOR
     *   ASSIGN_STATEMENT   --> id ASSIGN_OPERATOR EXPRESSION
     *   CALL_STATEMENT     --> IDS(EXPRESSIONS)
     *
     *
     *
     * STATEMENTS --> STATEMENT STATEMENTS | ε
     * STATEMENT  --> SUPER_STATEMENT | STRUCT_STATEMENT
     *
     * STRUCT_STATEMENT --> IF_STATEMENT | WHILE_STATEMENT | DO_WHILE_STATEMENT | FOR_STATEMENT | BLOCK_STATEMENT | RETURN_STATEMENT | SWITCH_STATEMENT
     *   IF_STATEMENT          --> if (CONDITION) STATEMENT ELSE_STATEMENT
     *   ELSE_STATEMENT        --> else STATEMENT  | ε
     *   WHILE_STATEMENT       --> while (CONDITION) STATEMENT
     *   Do_WHILE_STATEMENT    --> do STATEMENT while (CONDITION);
     *   FOR_STATEMENT         --> for (INLINE_STATEMENT; CONDITION; INLINE_STATEMENT) STATEMENT
     *   BLOCK_STATEMENT       --> { STATEMENTS }
     *   RETURN_STATEMENT      --> return RETURN_STATEMENT_REST;
     *   RETURN_STATEMENT_REST --> EXPRESSION | ε
     *   SWITCH_STATEMENT      --> switch { CASES }
     *   CASES                 --> CASE CASES | ε
     *   CASE                  --> CASE_STATEMENT | DEFAULT_STATEMENT
     *   CASE_STATEMENT        --> case VALUE: STATEMENTS break;
     *   DEFAULT_STATEMENT     --> default: STATEMENTS break;
     *
     *
     *
     * CONDITION  --> EXPRESSION REL_OPERATOR EXPRESSION | true | false
     * EXPRESSION --> VALUE | id | ( EXPRESSION )
     * VALUE      --> string | number | true | false | null
     *
     * ************************************************************
     *
     * IDS              --> id MORE_IDS
     * MORE_IDS         --> .IDS | ;
     *
     * DECLARES         --> DECLARE_STATEMENT MORE_DECLARES | ε
     * MORE_DECLARES    --> , DECLARES | ε
     *
     * EXPRESSIONS      --> EXPRESSION MORE_EXPRESSIONS | ε
     * MORE_EXPRESSIONS --> , EXPRESSIONS | ε
     *
     * ************************************************************
     * INC_DEC_OPERATOR --> ++ | --
     * ASSIGN_OPERATOR  --> = | += | -=
     * REL_OPERATOR     --> == | != | >|  >= | < | <=
     */
    public class Parser
    {
        private string? _source;
        private Token _lookahead;
        private Stack<Token> _tokensStack;

        public ProgramModel Parse(IEnumerable<Token> tokens, string? source = null)
        {
            //  Fill the Stack with Tokens and load the lookahead
            _tokensStack = new Stack<Token>();
            foreach (var token in tokens.Reverse())
                _tokensStack.Push(token);
            _lookahead = _tokensStack.Peek();

            // Optional: Used to find the syntax error location (Line No, Column No)
            _source = source;

            var programModel = Program();

            return programModel;
        }

        private Token Match(TokenType tokenType)
        {
            if (_lookahead.Type == tokenType)
            {
                var tempLookahead = _tokensStack.Pop();
                if (_tokensStack.Any()) _lookahead = _tokensStack.Peek();
                return tempLookahead;
            }

            if (string.IsNullOrEmpty(_source))
                throw new Exception(string.Format("Expected: '{0}' but found: '{1}'", Utilities.ToSnakeCase(tokenType.ToString()), _lookahead.Value));
            Utilities.GetLnColByPosition(_source, _lookahead.StartIndex, out int lineIndex, out int columnIndex);
            throw new Exception(string.Format("Expected: '{0}' at Line: {1}, Col: {2}", Utilities.ToSnakeCase(tokenType.ToString()), lineIndex, columnIndex));
        }

        private bool LookaheadFor(params TokenType[] types)
        {
            var tempStack = new Stack<Token>();

            bool found = true;
            foreach (var type in types)
                if (_tokensStack.Peek().Type == type) tempStack.Push(_tokensStack.Pop());
                else found = false;

            while (tempStack.Any())
                _tokensStack.Push(tempStack.Pop());

            return found;
        }





        // PROGRAM --> IMPORTS CLASSES
        private ProgramModel Program()
        {
            var imports = Imports().ToList();
            var classes = Classes().ToList();
            return new ProgramModel() { Imports = imports, Classes = classes };
        }

        // IMPORTS          --> IMPORT_STATEMENT IMPORTS | ε
        // IMPORT_STATEMENT --> using IDS;
        private IEnumerable<Import> Imports()
        {
            while (_lookahead.Type == TokenType.Using)
            {
                Match(TokenType.Using);
                var packages = IDs().ToList();
                Match(TokenType.Semicolon);
                yield return new Import() { Packages = packages };
            }
        }

        // CLASSES          --> CLASS_STATEMENT CLASSES | ε
        // CLASS_STATEMENT  --> class id { SUPER_STATEMENTS }
        private IEnumerable<Class> Classes()
        {
            while (_lookahead.Type == TokenType.Class)
            {
                Match(TokenType.Class);
                var name = Match(TokenType.Identifier).Value;
                Match(TokenType.OpenCurlyBracket);
                var statements = SuperStatements().ToList();
                Match(TokenType.CloseCurlyBracket);
                yield return new Class() { Name = name, Statements = statements };
            }
        }





        // SUPER_STATEMENTS --> SUPER_STATEMENT SUPER_STATEMENTS | ε
        private IEnumerable<ISuperStatement> SuperStatements()
        {
            while (IsSuperStatement(_lookahead.Type))
                yield return SuperStatement();
        }

        // SUPER_STATEMENT  --> COMMENT_STATEMENT | FUNCTION_STATEMENT | INLINE_STATEMENT
        private bool IsSuperStatement(TokenType type)
        {
            return IsCommentStatement() || IsFunctionStatement() || IsInlineStatement();
        }
        private ISuperStatement SuperStatement()
        {
            if (_lookahead.Type == TokenType.DoubleForwardSlashes || _lookahead.Type == TokenType.ForwardSlashAsterisk)
                return CommentStatement();
            else if (IsFunctionStatement())
                return FunctionStatement();
            else if (IsInlineStatement())
            {
                var statement = InlineStatement();
                Match(TokenType.Semicolon);
                return statement;
            }

            if (string.IsNullOrEmpty(_source))
                throw new Exception(string.Format("Invalid statement '{0}'", _lookahead.Value));
            Utilities.GetLnColByPosition(_source, _lookahead.StartIndex, out int lineIndex, out int columnIndex);
            throw new Exception(string.Format("Invalid statement '{0}' at Line: {1}, Col: {2}", _lookahead.Value, lineIndex, columnIndex));
        }

        // COMMENT_STATEMENT  --> // comment | /* multiline_comment */
        private CommentStatement CommentStatement()
        {
            if (_lookahead.Type == TokenType.DoubleForwardSlashes)
            {
                Match(TokenType.DoubleForwardSlashes);
                var comment = Match(TokenType.Comment).Value;
                return new CommentStatement() { Comment = comment };
            }
            else
            {
                Match(TokenType.ForwardSlashAsterisk);
                var comment = Match(TokenType.MultilineComment).Value;
                Match(TokenType.AsteriskForwardSlash);
                return new CommentStatement() { Comment = comment };
            }
        }
        private bool IsCommentStatement()
        {
            return _lookahead.Type == TokenType.DoubleForwardSlashes || _lookahead.Type == TokenType.ForwardSlashAsterisk;
        }

        // FUNCTION_STATEMENT --> DATA_TYPE id (DECLARES) { STATEMENTS }
        private FunctionStatement FunctionStatement()
        {
            var type = Match(TokenType.DataType).Value;
            var name = Match(TokenType.Identifier).Value;

            Match(TokenType.OpenRoundBracket);
            var parameters = Declares().ToList();
            Match(TokenType.CloseRoundBracket);

            Match(TokenType.OpenCurlyBracket);
            var statements = Statements().ToList();
            Match(TokenType.CloseCurlyBracket);

            return new FunctionStatement() { ReturnType = type, Name = name, Parameters = parameters, Statements = statements };
        }
        private bool IsFunctionStatement()
        {
            return LookaheadFor(TokenType.DataType, TokenType.Identifier, TokenType.OpenRoundBracket);
        }
        




        // INLINE_STATEMENT --> DECSIGN_STATEMENT | DECLARE_STATEMENT | INC_DEC_STATEMENT | ASSIGN_STATEMENT | CALL_STATEMENT
        private IInlineStatement InlineStatement()
        {
            if (_lookahead.Type == TokenType.DataType)
                if (IsDecsignStatement()) return DecsignStatement();
                else return DeclareStatement();
            else if (IsIncDecStatement())
                return IncDecStatement();
            else if (IsAssignStatement())
                return AssignStatement();
            else if (IsCallStatement())
                return CallStatement();

            if (string.IsNullOrEmpty(_source))
                throw new Exception(string.Format("Invalid statement", _lookahead.Value));
            Utilities.GetLnColByPosition(_source, _lookahead.StartIndex, out int lineIndex, out int columnIndex);
            throw new Exception(string.Format("Invalid statement '{0}' at Line: {1}, Col: {2}", _lookahead.Value, lineIndex, columnIndex));
        }
        private bool IsInlineStatement()
        {
            return IsDecsignStatement() || IsDeclareStatement() || IsIncDecStatement() || IsAssignStatement() || IsCallStatement();
        }

        // DECSIGN_STATEMENT  --> DATA_TYPE id = EXPRESSION
        private DecsignStatement DecsignStatement()
        {
            var type = Match(TokenType.DataType).Value;
            var name = Match(TokenType.Identifier).Value;
            Match(TokenType.Equal);
            var expression = Expression();

            return new DecsignStatement() { DataType = type, Variable = name, Expression = expression };
        }
        private bool IsDecsignStatement()
        {
            return LookaheadFor(TokenType.DataType, TokenType.Identifier, TokenType.Equal);
        }

        // DECLARE_STATEMENT  --> DATA_TYPE id
        private DeclareStatement DeclareStatement()
        {
            var type = Match(TokenType.DataType).Value;
            var name = Match(TokenType.Identifier).Value;

            return new DeclareStatement() { DataType = type, Variable = name };
        }
        private bool IsDeclareStatement()
        {
            return LookaheadFor(TokenType.DataType, TokenType.Identifier);
        }

        // INC_DEC_STATEMENT  --> id INC_DEC_OPERATOR
        private IncDecStatement IncDecStatement()
        {
            var name = Match(TokenType.Identifier).Value;
            var @operator = MatchIncDecOperator();

            return new IncDecStatement() { Variable = name, Operator = @operator };
        }
        private bool IsIncDecStatement()
        {
            return LookaheadFor(TokenType.Identifier, TokenType.DoublePluses) ||
                   LookaheadFor(TokenType.Identifier, TokenType.DoubleMinuses);
        }

        // ASSIGN_STATEMENT   --> id ASSIGN_OPERATOR EXPRESSION
        private AssignStatement AssignStatement()
        {
            var name = Match(TokenType.Identifier).Value;
            var @operator = MatchAssignOperator();
            var expression = Expression();

            return new AssignStatement() { Variable = name, Operator = @operator, Expression = expression };
        }
        private bool IsAssignStatement()
        {
            return LookaheadFor(TokenType.Identifier, TokenType.Equal) ||
                   LookaheadFor(TokenType.Identifier, TokenType.PlusEqual) ||
                   LookaheadFor(TokenType.Identifier, TokenType.MinusEqual);
        }

        // CALL_STATEMENT     --> IDS(EXPRESSIONS)
        private CallStatement CallStatement()
        {
            var path = IDs().ToList();
            Match(TokenType.OpenRoundBracket);
            var parameters = Expressions().ToList();
            Match(TokenType.CloseRoundBracket);

            return new CallStatement() { Path = path, Parameters = parameters };
        }
        private bool IsCallStatement()
        {
            return LookaheadFor(TokenType.Identifier, TokenType.Dot) ||
                   LookaheadFor(TokenType.Identifier, TokenType.OpenRoundBracket);
        }





        // STRUCT_STATEMENT --> IF_STATEMENT | WHILE_STATEMENT | DO_WHILE_STATEMENT | FOR_STATEMENT | BLOCK_STATEMENT | RETURN_STATEMENT | SWITCH_STATEMENT
        private IStructStatement StructStatement()
        {
            if (_lookahead.Type == TokenType.If)
                return IfStatement();
            else if (_lookahead.Type == TokenType.While)
                return WhileStatement();
            else if (_lookahead.Type == TokenType.Do)
                return DoWhileStatement();
            else if (_lookahead.Type == TokenType.For)
                return ForStatement();
            else if (_lookahead.Type == TokenType.OpenCurlyBracket)
                return BlockStatement();
            else if (_lookahead.Type == TokenType.Return)
                return ReturnStatement();
            else if (_lookahead.Type == TokenType.Switch)
                return SwitchStatement();

            if (string.IsNullOrEmpty(_source))
                throw new Exception(string.Format("Invalid statement '{0}'", _lookahead.Value));
            Utilities.GetLnColByPosition(_source, _lookahead.StartIndex, out int lineIndex, out int columnIndex);
            throw new Exception(string.Format("Invalid statement '{0}' at Line: {1}, Col: {2}", _lookahead.Value, lineIndex, columnIndex));
        }
        private bool IsStructStatement(TokenType type)
        {
            return type == TokenType.If || type == TokenType.While || type == TokenType.Do || type == TokenType.For || type == TokenType.OpenCurlyBracket || type == TokenType.Return || type == TokenType.Switch;
        }

        // IF_STATEMENT       --> if (CONDITION) STATEMENT ELSE_STATEMENT
        private IfStatement IfStatement()
        {
            Match(TokenType.If);
            Match(TokenType.OpenRoundBracket);
            var condition = Condition();
            Match(TokenType.CloseRoundBracket);
            var body = Statement();

            // ELSE_STATEMENT --> else STATEMENT  | ε
            IStatement? elseBody = null;
            if (_lookahead.Type == TokenType.Else)
            {
                Match(TokenType.Else);
                elseBody = Statement();
            }

            return new IfStatement() { Condition = condition, Body = body, ElseBody = elseBody };
        }

        // WHILE_STATEMENT    --> while (CONDITION) STATEMENT
        private WhileStatement WhileStatement()
        {
            Match(TokenType.While);
            Match(TokenType.OpenRoundBracket);
            var condition = Condition();
            Match(TokenType.CloseRoundBracket);
            var body = Statement();

            return new WhileStatement() { Condition = condition, Body = body };
        }

        // DO_WHILE_STATEMENT --> do STATEMENT while (CONDITION);
        private DoWhileStatement DoWhileStatement()
        {
            Match(TokenType.Do);
            var body = Statement();
            Match(TokenType.While);
            Match(TokenType.OpenRoundBracket);
            var condition = Condition();
            Match(TokenType.CloseRoundBracket);
            Match(TokenType.Semicolon);

            return new DoWhileStatement() { Condition = condition, Body = body };
        }

        // FOR_STATEMENT      --> for (INLINE_STATEMENT; CONDITION; INLINE_STATEMENT) STATEMENT
        private ForStatement ForStatement()
        {
            Match(TokenType.For);
            Match(TokenType.OpenRoundBracket);
            var prefix = InlineStatement();
            Match(TokenType.Semicolon);
            var condition = Condition();
            Match(TokenType.Semicolon);
            var repeat = InlineStatement();
            Match(TokenType.CloseRoundBracket);
            var body = Statement();

            return new ForStatement() { Prefix = prefix, Condition = condition, Repeat = repeat, Body = body };
        }

        // BLOCK_STATEMENT    --> { STATEMENTS }
        private BlockStatement BlockStatement()
        {
            Match(TokenType.OpenCurlyBracket);
            var statements = Statements().ToList();
            Match(TokenType.CloseCurlyBracket);

            return new BlockStatement() { Statements = statements };
        }

        // RETURN_STATEMENT   --> return RETURN_STATEMENT_REST;
        private ReturnStatement ReturnStatement()
        {
            Match(TokenType.Return);

            // RETURN_STATEMENT_REST --> EXPRESSION | ε
            string? expression = null;
            if (IsExpression(_lookahead.Type))
            {
                expression = Expression();
            }
            Match(TokenType.Semicolon);

            return new ReturnStatement() { ReturnValue = expression };
        }

        // SWITCH_STATEMENT   --> switch { CASES }
        private SwitchStatement SwitchStatement()
        {
            Match(TokenType.Switch);
            Match(TokenType.OpenCurlyBracket);
            var cases = CaseStatements().ToList();
            Match(TokenType.CloseCurlyBracket);

            return new SwitchStatement() { Cases = cases };
        }





        // CASES              --> CASE CASES | ε
        private IEnumerable<ICase> CaseStatements()
        {
            while (IsCaseStatement(_lookahead.Type))
                yield return Case();
        }
        private bool IsCaseStatement(TokenType type)
        {
            return type == TokenType.Case || type == TokenType.Default;
        }

        // CASE               --> CASE_STATEMENT | DEFAULT_STATEMENT
        private ICase Case()
        {
            if (_lookahead.Type == TokenType.Case)
                return CaseStatement();
            else if (_lookahead.Type == TokenType.Default)
                return DefaultStatement();

            if (string.IsNullOrEmpty(_source))
                throw new Exception(string.Format("Invalid statement '{0}'", _lookahead.Value));
            Utilities.GetLnColByPosition(_source, _lookahead.StartIndex, out int lineIndex, out int columnIndex);
            throw new Exception(string.Format("Invalid statement '{0}' at Line: {1}, Col: {2}", _lookahead.Value, lineIndex, columnIndex));
        }

        // CASE_STATEMENT     --> case VALUE: STATEMENTS break;
        private CaseStatement CaseStatement()
        {
            Match(TokenType.Case);
            var value = Value();
            Match(TokenType.Colon);
            var statement = Statement();
            Match(TokenType.Break);
            Match(TokenType.Semicolon);
            return new CaseStatement() { Value = value, Statement = statement };
        }

        // DEFAULT_STATEMENT  --> default: STATEMENTS break;
        private DefaultStatement DefaultStatement()
        {
            Match(TokenType.Default);
            Match(TokenType.Colon);
            var statement = Statement();
            Match(TokenType.Break);
            Match(TokenType.Semicolon);
            return new DefaultStatement() { Statement = statement };
        }





        // STATEMENTS --> STATEMENT STATEMENTS | ε
        private IEnumerable<IStatement> Statements()
        {
            while (IsStatement(_lookahead.Type))
                yield return Statement();
        }

        // STATEMENT  --> SUPER_STATEMENT | STRUCT_STATEMENT
        private IStatement Statement()
        {
            if (IsSuperStatement(_lookahead.Type))
                return SuperStatement();
            else if (IsStructStatement(_lookahead.Type))
                return StructStatement();

            if (string.IsNullOrEmpty(_source))
                throw new Exception(string.Format("Invalid statement '{0}'", _lookahead.Value));
            Utilities.GetLnColByPosition(_source, _lookahead.StartIndex, out int lineIndex, out int columnIndex);
            throw new Exception(string.Format("Invalid statement '{0}' at Line: {1}, Col: {2}", _lookahead.Value, lineIndex, columnIndex));
        }
        private bool IsStatement(TokenType type)
        {
            return IsSuperStatement(type) || IsStructStatement(type);
        }





        // CONDITION  --> EXPRESSION REL_OPERATOR EXPRESSION | true | false
        private Condition Condition()
        {
            if (_lookahead.Type == TokenType.True)
            {
                Match(TokenType.True);
                return new Condition() { Value = true };
            }
            else if (_lookahead.Type == TokenType.False)
            {
                Match(TokenType.False);
                return new Condition() { Value = false };
            }
            else
            {
                var left = Expression();
                var @operator = MatchRelOperator();
                var right = Expression();
                return new Condition() { Left = left, Operator = @operator, Right = right };
            }
        }

        // EXPRESSION --> VALUE | id | ( EXPRESSION )
        private string Expression()
        {
            if (IsValue(_lookahead.Type))
            {
                return Value();
            }
            else if (_lookahead.Type == TokenType.Identifier)
            {
                return Match(TokenType.Identifier).Value;
            }
            else if (_lookahead.Type == TokenType.OpenRoundBracket)
            {
                Match(TokenType.OpenRoundBracket);
                var expression = Expression();
                Match(TokenType.CloseRoundBracket);
                return expression;
            }

            if (string.IsNullOrEmpty(_source))
                throw new Exception(string.Format("Invalid expression '{0}'", _lookahead.Value));
            Utilities.GetLnColByPosition(_source, _lookahead.StartIndex, out int lineIndex, out int columnIndex);
            throw new Exception(string.Format("Invalid expression '{0}' at Line: {1}, Col: {2}", _lookahead.Value, lineIndex, columnIndex));
        }
        private bool IsExpression(TokenType type)
        {
            return IsValue(type) || type == TokenType.Identifier || type == TokenType.OpenRoundBracket;
        }

        // VALUE      --> string | number | true | false | null
        private string Value()
        {
            if (IsValue(_lookahead.Type))
                return Match(_lookahead.Type).Value;

            if (string.IsNullOrEmpty(_source))
                throw new Exception(string.Format("Invalid value '{0}'", _lookahead.Value));
            Utilities.GetLnColByPosition(_source, _lookahead.StartIndex, out int lineIndex, out int columnIndex);
            throw new Exception(string.Format("Invalid value '{0}' at Line: {1}, Col: {2}", _lookahead.Value, lineIndex, columnIndex));
        }
        private bool IsValue(TokenType type)
        {
            return type == TokenType.String || type == TokenType.Number || type == TokenType.True || type == TokenType.False || type == TokenType.Null;
        }





        // IDS         --> id MORE_IDS
        private IEnumerable<string> IDs()
        {
            yield return Match(TokenType.Identifier).Value;

            // MORE_IDS --> .IDS | ;
            while (_lookahead.Type == TokenType.Dot)
            {
                Match(TokenType.Dot);
                yield return Match(TokenType.Identifier).Value;
            }
        }

        // DECLARES    --> DECLARE_STATEMENT MORE_DECLARES | ε
        private IEnumerable<DeclareStatement> Declares()
        {
            if (_lookahead.Type == TokenType.DataType)
            {
                var type = Match(TokenType.DataType).Value;
                var name = Match(TokenType.Identifier).Value;
                yield return new DeclareStatement() { DataType = type, Variable = name };

                // MORE_DECLARES --> , DECLARES | ε
                while (_lookahead.Type == TokenType.Comma)
                {
                    Match(TokenType.Comma);
                    type = Match(TokenType.DataType).Value;
                    name = Match(TokenType.Identifier).Value;
                    yield return new DeclareStatement() { DataType = type, Variable = name };
                }
            }
        }

        // EXPRESSIONS --> EXPRESSION MORE_EXPRESSIONS | ε
        private IEnumerable<string> Expressions()
        {
            if (IsExpression(_lookahead.Type))
            {
                yield return Expression();

                // MORE_EXPRESSIONS --> , EXPRESSIONS | ε
                while (_lookahead.Type == TokenType.Comma)
                {
                    Match(TokenType.Comma);
                    yield return Expression();
                }
            }
        }





        // INC_DEC_OPERATORS --> = | += | -=
        private string MatchIncDecOperator()
        {
            if (IsIncDecOperator(_lookahead.Type))
                return Match(_lookahead.Type).Value;

            if (string.IsNullOrEmpty(_source))
                throw new Exception(string.Format("Expected an increment/decrement operator [ ++ | -- ] but found: '{0}'", _lookahead.Value));
            Utilities.GetLnColByPosition(_source, _lookahead.StartIndex, out int lineIndex, out int columnIndex);
            throw new Exception(string.Format("Expected an increment/decrement operator [ ++ | -- ] at Line: {0}, Col: {1}", lineIndex, columnIndex));
        }
        private bool IsIncDecOperator(TokenType type)
        {
            return type == TokenType.DoublePluses ||
                   type == TokenType.DoubleMinuses;
        }

        // ASSIGN_OPERATOR   --> = | += | -=
        private string MatchAssignOperator()
        {
            if (IsAssignOperator(_lookahead.Type))
                return Match(_lookahead.Type).Value;

            if (string.IsNullOrEmpty(_source))
                throw new Exception(string.Format("Expected an assign operator [ = | += | -=  ] but found: '{0}'", _lookahead.Value));
            Utilities.GetLnColByPosition(_source, _lookahead.StartIndex, out int lineIndex, out int columnIndex);
            throw new Exception(string.Format("Expected an assign operator [ = | += | -= ] at Line: {0}, Col: {1}", lineIndex, columnIndex));
        }
        private bool IsAssignOperator(TokenType type)
        {
            return type == TokenType.Equal ||
                   type == TokenType.PlusEqual ||
                   type == TokenType.MinusEqual;
        }

        // REL_OPERATOR      --> == | != | >|  >= | < | <=
        private string MatchRelOperator()
        {
            if (IsRelOperator(_lookahead.Type))
                return Match(_lookahead.Type).Value;

            if (string.IsNullOrEmpty(_source))
                throw new Exception(string.Format("Expected a relation operator [ == | != | >|  >= | < | <= ] but found: '{0}'", _lookahead.Value));
            Utilities.GetLnColByPosition(_source, _lookahead.StartIndex, out int lineIndex, out int columnIndex);
            throw new Exception(string.Format("Expected: a relation operator [ == | != | >|  >= | < | <= ] at Line: {0}, Col: {1}", lineIndex, columnIndex));
        }
        private bool IsRelOperator(TokenType type)
        {
            return type == TokenType.DoubleEquals ||
                   type == TokenType.NotEqual ||
                   type == TokenType.GreaterThan ||
                   type == TokenType.GreaterThanOrEqual ||
                   type == TokenType.LessThan ||
                   type == TokenType.LessThanOrEqual;
        }

    }
}
