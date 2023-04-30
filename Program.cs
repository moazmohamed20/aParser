using aParser;
using aParser.Parser;
using aParser.Tokenizer;
using System.Diagnostics;

string[] testProgramFiles = { "TestProgram.cs" };

var stopwatch = new Stopwatch();
var tokenizer = new Tokenizer();
var parser = new Parser();

foreach (string programFile in (args.Length > 0 ? args : testProgramFiles))
{
    Console.WriteLine($"> Reading File: {programFile}");
    var programCode = File.ReadAllText(programFile);

    Console.WriteLine("  Tokenizer Started...");
    stopwatch.Start();
    var tokens = tokenizer.Tokenize(programCode).ToList();
    stopwatch.Stop();
    Console.WriteLine($"  Tokenizer Completed Successfully in {stopwatch.ElapsedMilliseconds} ms");

    Console.WriteLine("  Parser Started...");
    stopwatch.Restart();
    var iModel = parser.Parse(tokens, programCode);
    stopwatch.Stop();
    Console.WriteLine($"  Parser Completed Successfully in {stopwatch.ElapsedMilliseconds} ms");

    Console.WriteLine("  Serializer Started...");
    stopwatch.Restart();
    var json = Utilities.JsonSerialize(iModel);
    stopwatch.Stop();
    Console.WriteLine($"  Serializer Completed Successfully in {stopwatch.ElapsedMilliseconds} ms");

    File.WriteAllText($"{Path.GetFileNameWithoutExtension(programFile)}-Parse.json", json);
    Console.WriteLine();
}