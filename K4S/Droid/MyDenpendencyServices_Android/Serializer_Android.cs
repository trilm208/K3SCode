

using LoginXF.Droid;
using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using Xamarin.Forms;
using MyDependencyServices;

[assembly: Dependency(typeof(Serializer_Android))]
namespace LoginXF.Droid
{
    public  class Serializer_Android: ISerializer
    {
        public  T Decompress<T>(byte[] compressedData) where T : class
        {
            using (MemoryStream memory = new MemoryStream(compressedData))
            {
                using (GZipStream zip = new GZipStream(memory, CompressionMode.Decompress, false))
                {
                    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    return formatter.Deserialize(zip) as T;
                }
            }
        }


        public  byte[] Compress<T>(T data) where T : class
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream zip = new GZipStream(memory, CompressionMode.Compress, false))
                {
                    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    formatter.Serialize(zip, data);
                }

                return memory.ToArray();
            }
        }

        public  string SerializeJSon<T>(T t) where T : class
        {
            MemoryStream stream = new MemoryStream();

            DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(T));
            DataContractJsonSerializerSettings s = new DataContractJsonSerializerSettings();
            ds.WriteObject(stream, t);
            string jsonString = Encoding.UTF8.GetString(stream.ToArray());
            stream.Close();
            return jsonString;
        }


        public  T DeserializeJSon<T>(string jsonString) where T : class
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(stream);
            return obj;
        }
        public  byte[] ToBinary<T>(T data) where T : class
        {
            using (MemoryStream memory = new MemoryStream())
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(memory, data);
                return memory.ToArray();
            }
        }


        public  T FromBinary<T>(byte[] binary) where T : class
        {
            using (MemoryStream memory = new MemoryStream(binary))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return formatter.Deserialize(memory) as T;
            }
        }
        public  string Serialize(object obj)
        {
            MemoryStream memorystream = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(memorystream, obj);
            byte[] mStream = memorystream.ToArray();
            string slist = Convert.ToBase64String(mStream);
            return slist;
        }
        public  object Unserialize(string str)
        {
            byte[] mData = Convert.FromBase64String(str);
            MemoryStream memorystream = new MemoryStream(mData);
            BinaryFormatter bf = new BinaryFormatter();
            Object obj = bf.Deserialize(memorystream);
            return obj;
        }
    }
}
