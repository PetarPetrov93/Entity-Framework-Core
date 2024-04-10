using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CarDealer.Utilities
{
    public class XMLHelper
    {
        public T Deserialize<T>(string xml, string rootName)
        {
            XmlRootAttribute root = new XmlRootAttribute(rootName);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), root);

            using StringReader reader = new StringReader(xml);

            T deserializedDTOs = (T)xmlSerializer.Deserialize(reader)!;

            return deserializedDTOs!;
        }

        public string Serialize<T>(T obj,  string rootName)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using StringWriter writer = new StringWriter(sb);

            xmlSerializer.Serialize(writer, obj, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}
