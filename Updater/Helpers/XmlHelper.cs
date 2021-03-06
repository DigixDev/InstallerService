using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Updater.Helpers
{
    public static class XmlHelper
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
                    return (T)ser.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }


        public static T Deserialize<T>(string filePath)
        {
            using (var reader = File.OpenRead(filePath))
            {
                var ser = new XmlSerializer(typeof(T));
                return (T) ser.Deserialize(reader);
            }
        }
    }
}
