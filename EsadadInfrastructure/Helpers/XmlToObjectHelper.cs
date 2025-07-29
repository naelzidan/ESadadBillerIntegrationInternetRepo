using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace Esadad.Infrastructure.Helpers
{
    public static class XmlToObjectHelper
    {
        public static T DeserializeXmlToObject<T>(XmlElement xmlElement, T existingObject)
        {
            // Check for null input
            if (xmlElement == null)
                throw new ArgumentNullException(nameof(xmlElement));

            // Create XmlSerializer for the type of the object
            var serializer = new XmlSerializer(typeof(T));

            // Read the XmlElement as string
            using (StringReader stringReader = new StringReader(xmlElement.OuterXml))
            {
                // Deserialize the XmlElement into the existing object
                var deserializedObject = (T)serializer.Deserialize(stringReader);
                return deserializedObject;
            }
        }
    }
}
