using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace RabbitMQReprocessDeadLetter
{
    public static class ByteArrayExt
    {
        public static string ConvertToString(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public static T Deserialize<T>(this byte[] byteArray)
        {
            if (byteArray == null)
            {
                return default(T);
            }
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(byteArray, 0, byteArray.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = (T)binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}
