using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace Shared.Helpers
{
    public static class XmlTools
    {
        public static void Serialize<T>(Stream writer, T obj)
        {
            var ser = new XmlSerializer(typeof(T));
            ser.Serialize(writer, obj);
        }

        public static void Serialize<T>(string filePath, T obj)
        {
            using (var writer = File.OpenWrite(filePath))
            {
                var ser = new XmlSerializer(typeof(T));
                ser.Serialize(writer, obj);
            }
        }

        public static string Serialize<T>(T obj)
        {
            using (var writer = new StringWriter())
            {
                var ser = new XmlSerializer(typeof(T));
                ser.Serialize(writer, obj);

                return writer.ToString();
            }
        }

        public static T Deserialize<T>(string text)
        {
            using (var reader = new StringReader(text))
            {
                var ser = new XmlSerializer(typeof(T));
                return (T) ser.Deserialize(reader);
            }
        }

        public static T Deserialize<T>(Stream reader)
        {
            var ser = new XmlSerializer(typeof(T));
            return (T) ser.Deserialize(reader);
        }

        public static T Deserialize<T>(Uri uri)
        {
            try
            {
                using (var client = new WebClient())
                using (var reader = client.OpenRead(uri))
                {
                    var ser = new XmlSerializer(typeof(T));
                    return (T) ser.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public static T DeserializeFile<T>(string filePath)
        {
            using (var reader = File.OpenRead(filePath))
            {
                var ser = new XmlSerializer(typeof(T));
                return (T)ser.Deserialize(reader);
            }
        }
    }
}