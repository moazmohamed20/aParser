namespace aParser.Parser.Models
{

    public class ProgramModel
    {
        public IEnumerable<Import> Imports { get; set; }

        public IEnumerable<Class> Classes { get; set; }
    }
}