using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.RegularExpressions;

namespace aParser
{
    public class Utilities
    {
        public static void GetLnColByPosition(string text, int position, out int lineIndex, out int columnIndex)
        {
            lineIndex = 0;
            columnIndex = 0;

            var lines = text.Split('\r');
            foreach (string line in lines)
            {
                if (position < line.Length)
                {
                    columnIndex = position + 1;
                    break;
                }
                else
                {
                    position -= line.Length + 1;
                }
                lineIndex++;
            }
        }

        public static string JsonSerialize(object value)
        {
            return JsonConvert.SerializeObject(
                        value,
                        new JsonSerializerSettings()
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            TypeNameHandling = TypeNameHandling.Auto,
                            SerializationBinder = new TypeNameSerializationBinder()
                        }
                   );
        }

        public static string ToSnakeCase(string text)
        {
            text = Regex.Replace(text, @"(.)([A-Z][a-z]+)", "$1_$2");
            text = Regex.Replace(text, @"([a-z0-9])([A-Z])", "$1_$2");
            return text.ToLower();
        }
    }

    public class TypeNameSerializationBinder : ISerializationBinder
    {



        public void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
        {
            assemblyName = null;
            typeName = Utilities.ToSnakeCase(serializedType.Name);
        }

        public Type BindToType(string? assemblyName, string typeName)
        {
            return Type.GetType(typeName)!;
        }
    }

}
