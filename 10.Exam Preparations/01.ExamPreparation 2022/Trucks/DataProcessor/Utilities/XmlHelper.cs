using System.Text;
using System.Xml.Serialization;

namespace Trucks.Utilities
{
    public class XmlHelper
    {
        public T Deserialize<T>(string xml, string rootName)
        {
            XmlRootAttribute root = new XmlRootAttribute(rootName);
            XmlSerializer serializer = new XmlSerializer(typeof(T), root);

            using StringReader reader = new StringReader(xml);

            T deserializedDTOs = (T)serializer.Deserialize(reader)!;

            return deserializedDTOs;
        }

        public string Serialize<T>(T obj, string rootName)
        {
            StringBuilder sb = new StringBuilder();
            XmlRootAttribute root = new XmlRootAttribute(rootName);
            XmlSerializer serializer = new XmlSerializer(typeof(T), root);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using StringWriter writer = new StringWriter(sb);

            serializer.Serialize(writer, obj, namespaces);
            return sb.ToString().TrimEnd();
        }
    }
}
