using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace AnkiCollectionHelper.Helpers
{
    public static class XmlSerializationHelper
    {
        public static void Serialize(Stream s, Type type)
        {
            var serializer = new XmlSerializer(type);
            serializer.Serialize(s, serializer);
        }

        public static void Serialize<T>(Stream s, T t)
        {
            Serialize(s, t, typeof(T));
        }

        public static void Serialize<T>(Stream s, T t, Type type)
        {
            var serializer = new XmlSerializer(type);
            serializer.Serialize(s, t);
        }

        private static object Deserialize(Type type, Stream stream)
        {
            var serializer = new XmlSerializer(type);
            return serializer.Deserialize(stream);
        }

        public static T Deserialize<T>(Stream s)
        {
            return (T)Deserialize(typeof(T), s);
        }

        public static void SerializeToFile(string fileName, object value, Type type)
        {
            using (var stream = new MemoryStream())
            {
                Serialize(stream, value, type);
                stream.Seek(0, SeekOrigin.Begin);
                StreamHelper.WriteStreamToFile(stream, fileName);
            }
        }

        public static void SerializeToFile<T>(string fileName, T t)
        {
            SerializeToFile(fileName, t, typeof(T));
        }

        public static T DeserializeFromFile<T>(string fileName)
        {
            return (T)DeserializeFromFile(typeof(T), fileName);
        }

        public static object DeserializeFromFile(Type type, string fileName)
        {
            using (var file = new FileStream(fileName, FileMode.Open))
            {
                return Deserialize(type, file);
            }
        }

        public static object DeserializeFromString(Type type, string xml)
        {
            using (var stream = StreamHelper.StringToStream(xml, Encoding.UTF8))
            {
                return Deserialize(type, stream);
            }
        }

        public static T DeserializeFromString<T>(string xml)
        {
            using (var stream = StreamHelper.StringToStream(xml, Encoding.UTF8))
            {
                return Deserialize<T>(stream);
            }
        }

        public static T DeserializeFromByteArray<T>(byte[] arrayBytes)
        {
            using (Stream stream = new MemoryStream(arrayBytes))
            {
                return Deserialize<T>(stream);
            }
        }

        public static string SerializeToString<T>(object value)
        {
            using (var stream = new MemoryStream())
            {
                Serialize(stream, value, typeof(T));
                stream.Seek(0, SeekOrigin.Begin);
                return StreamHelper.StreamToStringUtf8(stream);
            }
        }

        public static string SerializeToString(object value, Type type)
        {
            using (var stream = new MemoryStream())
            {
                Serialize(stream, value, type);
                stream.Seek(0, SeekOrigin.Begin);
                return StreamHelper.StreamToStringUtf8(stream);
            }
        }

        public static T DeserializeFromXElement<T>(XElement element)
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(element.CreateReader());
        }
    }
}