using aParser.Parser.Models;
using aParser.Parser.Models.Statements;
using aParser.Parser.Models.Statements.StructStatements;
using aParser.Parser.Models.Statements.StructStatementsCases;
using aParser.Parser.Models.Statements.SuperStatements;
using aParser.Parser.Models.Statements.SuperStatements.InlineStatements;
using aParser.Tokenizer.Models;
using Newtonsoft.Json.Linq;

namespace aParser.Translator
{
    public class Translator
    {
        private ProgramModel _programModel;
        public string Translate(ProgramModel programModel)
        {
            _programModel = programModel;

            return TranslateProgram();
        }





        private string TranslateProgram()
        {
            string program = string.Empty;
            program += TranslateImports();
            program += TranslateClasses();
            return program;
        }
        private string TranslateImports()
        {
            string imports = string.Empty;
            foreach (var import in _programModel.Imports)
                imports += "Imports " + string.Join('.', import.Packages) + Environment.NewLine;
            return imports;
        }
        private string TranslateClasses()
        {
            string classes = string.Empty;
            foreach (var @class in _programModel.Classes)
            {
                classes += "Class " + @class.Name + Environment.NewLine;
                classes += TranslateSuperStatements(@class.Statements);
                classes += "End Class" + Environment.NewLine;
            }
            return classes;
        }





        private string TranslateSuperStatements(IEnumerable<ISuperStatement> superStatements)
        {
            string statements = string.Empty;
            foreach (var superStatement in superStatements)
                statements += TranslateSuperStatement(superStatement) + Environment.NewLine;
            return statements;
        }
        private string TranslateSuperStatement(ISuperStatement superStatement)
        {
            return superStatement switch
            {
                CommentStatement s => TranslateComment(s),
                FunctionStatement s => TranslateFunction(s),
                IInlineStatement s => TranslateInline(s),
                _ => string.Empty,
            };
        }

        private string TranslateComment(CommentStatement statement)
        {
            statement.Comment = statement.Comment.ReplaceLineEndings();
            return "'" + statement.Comment.Replace(Environment.NewLine, Environment.NewLine + "'");
        }
        private string TranslateFunction(FunctionStatement statement)
        {
            var parameters = TranslateDeclares(statement.Parameters);
            var type = TranslateType(statement.ReturnType);
            var isVoid = (type == "Sub");
            var name = statement.Name;

            string translatedFunction = (isVoid ? "Sub " : "Function ") + name + "(" + parameters + ")" + (isVoid ? "" : " As " + type) + Environment.NewLine;
            translatedFunction += TranslateStatements(statement.Statements);
            translatedFunction += (isVoid ? "End Sub" : "End Function") + Environment.NewLine;

            return translatedFunction;
        }





        private string TranslateInline(IInlineStatement superStatement)
        {
            return superStatement switch
            {
                DecsignStatement s => TranslateDecsign(s),
                DeclareStatement s => TranslateDeclare(s),
                IncDecStatement s => TranslateIncDec(s),
                AssignStatement s => TranslateAssign(s),
                CallStatement s => TranslateCall(s),
                _ => string.Empty,
            };
        }

        private string TranslateDecsign(DecsignStatement statement)
        {
            return "Dim " + statement.Variable + " As " + TranslateType(statement.DataType) + " = " + TranslateExpression(statement.Expression);
        }
        private string TranslateDeclare(DeclareStatement statement)
        {
            return "Dim " + statement.Variable + " As " + TranslateType(statement.DataType);
        }
        private string TranslateIncDec(IncDecStatement statement)
        {
            return statement.Variable + " " + TranslateIncDecOperator(statement.Operator);
        }
        private string TranslateAssign(AssignStatement statement)
        {
            return statement.Variable + " " + TranslateAssignOperator(statement.Operator) + " " + TranslateExpression(statement.Expression);
        }
        private string TranslateCall(CallStatement statement)
        {
            var path = TranslateIDs(statement.Path);
            var expressions = TranslateExpressions(statement.Parameters);
            return path + "(" + expressions + ")";
        }














        private string TranslateStructStatement(IStructStatement structStatement)
        {
            return structStatement switch
            {
                IfStatement s => TranslateIf(s),
                WhileStatement s => TranslateWhile(s),
                DoWhileStatement s => TranslateDoWhile(s),
                ForStatement s => TranslateFor(s),
                BlockStatement s => TranslateBlock(s),
                ReturnStatement s => TranslateReturn(s),
                SwitchStatement s => TranslateSwitch(s),
                _ => string.Empty,
            };
        }

        private string TranslateIf(IfStatement statement)
        {
            string translatedIf = string.Empty;

            var condition = TranslateCondition(statement.Condition);
            var body = TranslateStatement(statement.Body);

            translatedIf += "If " + condition + " Then" + Environment.NewLine;
            translatedIf += body + Environment.NewLine;
            if (statement.ElseBody != null)
            {
                translatedIf += "Else" + Environment.NewLine;
                translatedIf += TranslateStatement(statement.ElseBody) + Environment.NewLine;
            }
            translatedIf += "End If" + Environment.NewLine;

            return translatedIf;
        }
        private string TranslateWhile(WhileStatement statement)
        {
            string translatedWhile = string.Empty;

            var condition = TranslateCondition(statement.Condition);
            var body = TranslateStatement(statement.Body);

            translatedWhile += "While " + condition + Environment.NewLine;
            translatedWhile += body + Environment.NewLine;
            translatedWhile += "End While" + Environment.NewLine;

            return translatedWhile;
        }
        private string TranslateDoWhile(DoWhileStatement statement)
        {
            string translatedDoWhile = string.Empty;

            var condition = TranslateCondition(statement.Condition);
            var body = TranslateStatement(statement.Body);

            translatedDoWhile += "Do While " + condition + Environment.NewLine;
            translatedDoWhile += body + Environment.NewLine;
            translatedDoWhile += "Loop" + Environment.NewLine;

            return translatedDoWhile;
        }
        private string TranslateFor(ForStatement statement)
        {
            string translatedFor = string.Empty;

            var prefix = TranslateInline(statement.Prefix);
            var body = TranslateStatement(statement.Body);
            var repeat = TranslateInline(statement.Repeat);
            var condition = TranslateCondition(statement.Condition);

            translatedFor += prefix + Environment.NewLine;
            translatedFor += "While " + condition + Environment.NewLine;
            translatedFor += body + Environment.NewLine;
            translatedFor += repeat + Environment.NewLine;
            translatedFor += "End While" + Environment.NewLine;

            return translatedFor;
        }
        private string TranslateBlock(BlockStatement statement)
        {
            return TranslateStatements(statement.Statements);
        }
        private string TranslateReturn(ReturnStatement statement)
        {
            return "Return" + (statement.ReturnValue == null ? "" : " " + statement.ReturnValue);
        }
        private string TranslateSwitch(SwitchStatement statement)
        {
            string translatedSwitch = string.Empty;

            var cases = TranslateCases(statement.Cases);

            translatedSwitch += "Select Case " + TranslateExpression(statement.Expression) + Environment.NewLine;
            translatedSwitch += cases;
            translatedSwitch += "End Select" + Environment.NewLine;

            return translatedSwitch;
        }





        private string TranslateCases(IEnumerable<ICase> cases)
        {
            string translatedCases = string.Empty;
            foreach (var @case in cases)
                translatedCases += TranslateCase(@case) + Environment.NewLine;
            return translatedCases;
        }
        private string TranslateCase(ICase @case)
        {
            return @case switch
            {
                CaseStatement s => TranslateCaseAlternative(s),
                DefaultStatement s => TranslateDefaultAlternative(s),
                _ => string.Empty,
            };
        }

        private string TranslateCaseAlternative(CaseStatement statement)
        {
            return "Case " + statement.Constant + Environment.NewLine +
                TranslateStatement(statement.Statement) + Environment.NewLine;
        }
        private string TranslateDefaultAlternative(DefaultStatement statement)
        {
            return "Case Else" + Environment.NewLine +
                TranslateStatement(statement.Statement) + Environment.NewLine;
        }





        private string TranslateType(string type)
        {
            var trimmedType = type.Trim('?', ']', '[');
            var newType = trimmedType switch
            {
                "bool" => "Boolean",
                "char" => "Char",
                "short" => "Short",
                "int" => "Integer",
                "long" => "Long",
                "float" => "Single",
                "double" => "Double",
                "decimal" => "Decimal",
                "string" or "String" => "String",
                "void" => "Sub",
                _ => "Object",
            };
            return type.Replace(trimmedType, newType).Replace("String?", "String").Replace("[]", "()");
        }
        private string TranslateCondition(Condition condition)
        {
            if (condition.Output == null)
                return condition.Left + " " + TranslateRelOperator(condition.Operator) + " " + condition.Right;
            else
                return (condition.Output.Value ? "True" : "False");

        }
        private string TranslateExpression(string expression)
        {
            return expression switch
            {
                "null" => "Nothing",
                "true" => "True",
                "false" => "False",
                _ => expression
            };
        }





        private string TranslateStatements(IEnumerable<IStatement> statements)
        {
            string translatedStatements = string.Empty;
            foreach (var statement in statements)
                translatedStatements += TranslateStatement(statement) + Environment.NewLine;
            return translatedStatements;
        }
        private string TranslateStatement(IStatement statement)
        {
            return statement switch
            {
                ISuperStatement s => TranslateSuperStatement(s),
                IStructStatement s => TranslateStructStatement(s),
                _ => string.Empty,
            };
        }





        private string TranslateIDs(IEnumerable<string> ids)
        {
            return string.Join(".", ids);
        }
        private string TranslateDeclares(IEnumerable<DeclareStatement> declares)
        {
            var connectedDeclares = new List<string>();
            foreach (var declare in declares)
                connectedDeclares.Add(declare.Variable + " As " + TranslateType(declare.DataType));
            return string.Join(", ", connectedDeclares);
        }
        private string TranslateExpressions(IEnumerable<string> expressions)
        {
            return string.Join(", ", expressions);
        }





        private string TranslateIncDecOperator(string @operator)
        {
            if (@operator == "++")
                return "+= 1";
            else if (@operator == "--")
                return "-= 1";
            return string.Empty;
        }
        private string TranslateAssignOperator(string @operator)
        {
            return @operator;
        }
        private string TranslateRelOperator(string @operator)
        {
            return @operator == "!=" ? "<>" : @operator;
        }
    }
}
