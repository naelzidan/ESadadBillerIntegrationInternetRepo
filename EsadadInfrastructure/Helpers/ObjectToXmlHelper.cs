using System.Xml;
using System.Xml.Serialization;

namespace Esadad.Infrastructure.Helpers
{
    public static class ObjectToXmlHelper
    {
        public static XmlElement ObjectToXmlElement<T>(T obj)
        {
            string xmlString = SerializeToXmlString(obj);
            return ConvertToXmlElement(xmlString);
        }
        
        private static string SerializeToXmlString<T>(T obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, obj);
                return textWriter.ToString();
            }
        }
        
        private static XmlElement ConvertToXmlElement(string xmlString)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            return xmlDoc.DocumentElement;
        }
    }
}