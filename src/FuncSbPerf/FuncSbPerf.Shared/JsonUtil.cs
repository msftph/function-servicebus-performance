using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FuncSbPerf.Shared
{
    public class JsonUtil
    {
        public static void Serialize(object value, Stream s)
        {
            using (StreamWriter writer = new StreamWriter(s))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer ser = new JsonSerializer();
                ser.Serialize(jsonWriter, value);
                jsonWriter.Flush();
            }
        }

        public static byte[] SerializeToByteArray(object value)
        {
            using (var stream = new MemoryStream())
            {
                Serialize(value, stream);
                return stream.ToArray();
            }
        }

        public static string SerializeToString(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static T Deserialize<T>(Stream s)
        {
            using (StreamReader reader = new StreamReader(s))
            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                JsonSerializer ser = new JsonSerializer();
                return ser.Deserialize<T>(jsonReader);
            }
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream(bytes))
            {
                return Deserialize<T>(memoryStream);
            }
        }
    }
}
