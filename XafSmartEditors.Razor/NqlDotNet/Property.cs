using System.Reflection;

namespace NqlDotNet
{
    public class Property
    {
        public string PropertyName { get; set; }
        public string Type { get; set; }
        public string Key { get; set; } // "primary", "foreign", or null
        public string Description { get; set; }
        public string References { get; set; } // For foreign keys
        public bool IsCollection { get; set; }
    }
    public static class EmbeddedResourceHelper
    {
        public static string ReadEmbeddedResource(string resourceName,string nameSpace,Type type)
        {
            var assembly = Assembly.GetAssembly(type);

            // Adjust the namespace and folder if needed
            string fullResourceName = $"{nameSpace}.{resourceName}";

            using (Stream stream = assembly.GetManifestResourceStream(fullResourceName))
            {
                if (stream == null)
                {
                    // Resource not found, throw an exception or handle accordingly
                    throw new FileNotFoundException($"Embedded resource '{fullResourceName}' not found.");
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
