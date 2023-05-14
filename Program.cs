using aParser;
using aParser.Parser;
using aParser.Tokenizer;
using aParser.Translator;
using System.Diagnostics;

string[] surroundingFiles = Directory.GetFiles(Environment.CurrentDirectory)
                            .Where(s => Path.GetExtension(s) == ".cs")
                            .ToArray();

var parser = new Parser();
var tokenizer = new Tokenizer();
var translator = new Translator();
var stopwatch = new Stopwatch();

foreach (string file in (args.Length > 0 ? args : surroundingFiles))
{
    Console.WriteLine($"> Reading File: {Path.GetFileName(file)}");
    var programCode = File.ReadAllText(file);

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

    File.WriteAllText($"{Path.GetDirectoryName(file)}/{Path.GetFileNameWithoutExtension(file)}-tokens.json", tokensJson);
    File.WriteAllText($"{Path.GetDirectoryName(file)}/{Path.GetFileNameWithoutExtension(file)}-iModel.json", iModelJson);
    File.WriteAllText($"{Path.GetDirectoryName(file)}/{Path.GetFileNameWithoutExtension(file)}.vb", program);
    Console.WriteLine();
}