using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using Newtonsoft.Json;

namespace Shared.Tools
{
    public class JsonTools
    {
        #region file

        public static void SerializeFile<T>(string fileName, T obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            File.WriteAllText(fileName, json);
        }

        public static T DeserializeFile<T>(string fileName)
        {
            using(var file=File.Open(fileName, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(file))
            {
                return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
            }
        }

        #endregion

        #region string

        public static string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        #endregion
    }
}
