using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml;

namespace Assets.Scripts.Infrastructure
{
    public static class Serialization
    {
        public static void Serialize<T>(Stream stream, T obj) where T : class
        {
            var serializer = new DataContractSerializer(typeof(T));
            var settings = new XmlWriterSettings { Indent = true };

            using (var xmlWriter = XmlWriter.Create(stream, settings))
            {
                serializer.WriteObject(xmlWriter, obj);
            }
        }
        
        public static T Deserialize<T>(Stream stream) where T : class
        {
            var serializer = new DataContractSerializer(typeof(T));
            return serializer.ReadObject(stream) as T;
        }
    }
}
