using aParser;
using aParser.Parser;
using aParser.Tokenizer;
using aParser.Translator;
using System.Diagnostics;

string[] testProgramFiles = { "TestProgram.cs" };

var parser = new Parser();
var tokenizer = new Tokenizer();
var translator = new Translator();
var stopwatch = new Stopwatch();

foreach (string programFile in (args.Length > 0 ? args : testProgramFiles))
{
    Console.WriteLine($"> Reading File: {programFile}");
    var programCode = File.ReadAllText(programFile);

    #region "Tokenizer"
    Console.WriteLine("  Tokenizer Started...");
    stopwatch.Start();
    var tokens = tokenizer.Tokenize(programCode).ToList();
    stopwatch.Stop();
    Console.WriteLine($"  Tokenizer Completed Successfully in {stopwatch.ElapsedMilliseconds} ms");
    #endregion

    #region "Parser"
    Console.WriteLine("  Parser Started...");
    stopwatch.Restart();
    var iModel = parser.Parse(tokens, programCode);
    stopwatch.Stop();
    Console.WriteLine($"  Parser Completed Successfully in {stopwatch.ElapsedMilliseconds} ms");
    #endregion

    #region "Translator"
    Console.WriteLine("  Translator Started...");
    stopwatch.Restart();
    var program = translator.Translate(iModel);
    stopwatch.Stop();
    Console.WriteLine($"  Translator Completed Successfully in {stopwatch.ElapsedMilliseconds} ms");
    #endregion

    #region "Serializer"
    Console.WriteLine("  Serializer Started...");
    stopwatch.Restart();
    var tokensJson = Utilities.JsonSerialize(tokens);
    var iModelJson = Utilities.JsonSerialize(iModel);
    stopwatch.Stop();
    Console.WriteLine($"  Serializer Completed Successfully in {stopwatch.ElapsedMilliseconds} ms");
    #endregion

    File.WriteAllText($"{Path.GetFileNameWithoutExtension(programFile)}-tokens.json", tokensJson);
    File.WriteAllText($"{Path.GetFileNameWithoutExtension(programFile)}-iModel.json", iModelJson);
    File.WriteAllText($"{Path.GetFileNameWithoutExtension(programFile)}.vb", program);
    Console.WriteLine();
}