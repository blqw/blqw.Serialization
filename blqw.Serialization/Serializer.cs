using blqw.Serialization;
using blqw.SerializationComponent;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public static partial class Serializer
    {
        static readonly byte[] HEAD = new byte[] { 77, 0, 78, 90 };

        /// <summary>
        /// 压缩字节的头标识
        /// </summary>
        public static byte[] Head
        {
            get
            {
                var head = new byte[HEAD.Length];
                HEAD.CopyTo(head, 0);
                return head;
            }
        }

        /// <summary>
        /// 验证数据的头标识
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool ValidationHead(byte[] data)
        {
            if (data == null || data.Length < HEAD.Length + 1)
            {
                return false;
            }
            var offset = data[0] == 0 ? 1 : 0;
            for (int i = 0; i < HEAD.Length; i++)
            {
                if (data[i + offset] != Head[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary> 压缩数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] GZipCompress(byte[] data)
        {
            using (var ms = new MemoryStream())
            using (var gzip = new GZipStream(ms, CompressionMode.Compress))
            {
                gzip.Write(data, 0, data.Length);//将数据压缩并写到基础流中
                gzip.Close();
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 将数据写入数据块
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static void Write(Stream stream, object obj, IFormatter previous = null)

        {
            using (ReferencedCache.Context())
            {
                var refindex = ReferencedCache.AddOrGet(obj);
                FormatterProvider provider;
                if (refindex < 0)
                {
                    provider = FormatterCache.GetProvider(obj);
                }
                else
                {
                    obj = refindex;
                    provider = FormatterCache.GetProvider(HeadFlag.Referenced);
                }

                if (previous?.SurrogateSelector != null)
                {
                    var type = obj.GetType();
                    ISurrogateSelector selector;
                    var surrogate = previous.SurrogateSelector.GetSurrogate(type, previous.Context, out selector);
                    if (surrogate != null)
                    {
                        var data = new SerializationInfo(type, Component.Converter);
                        surrogate.GetObjectData(obj, data, previous.Context);
                        obj = data;
                        provider = FormatterCache.GetProvider(HeadFlag.SerializationInfo);
                    }
                }

                FormatterCache.GetByteFormatter(previous).Serialize(stream, provider.Flag);
                provider.GetFormatter(previous).Serialize(stream, obj);
            }
        }
        /// <summary>
        /// 从流中读取数据,并反序列化为对象
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static object Read(Stream stream, IFormatter previous = null)
        {
            using (ReferencedCache.Context())
            {
                TraceDeserialize.WriteName("flag");
                TraceDeserialize.SetWriting(false);
                var fragmentType = (HeadFlag)FormatterCache.GetByteFormatter(previous).Deserialize(stream); //读取片段类型标识
                TraceDeserialize.SetWriting(true);
                TraceDeserialize.WriteValue(fragmentType.ToString());
                var provider = FormatterCache.GetProvider(fragmentType);
                return provider.GetFormatter(previous).Deserialize(stream);
            }
        }

        /// <summary> 将对象序列化成字节流
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] GetBytes(object obj, FormatterArgs args = null)
        {
            if (obj == null)
            {
                return null;
            }
            using (var stream = new MemoryStream(4096))
            {
                Write(stream, obj, args);
                var data = stream.ToArray();
                stream.SetLength(0);
                using (var gzip = new GZipStream(stream, CompressionMode.Compress))
                {
                    stream.WriteByte(0);
                    stream.Write(HEAD, 0, HEAD.Length); //写入头
                    gzip.Write(data, 0, data.Length);//将数据压缩并写到基础流中
                    gzip.Close();
                    return stream.ToArray();
                }
            }
        }

        /// <summary> 将字节流反序列化为对象
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static object GetObject(byte[] bytes, FormatterArgs args = null)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }
            if (ValidationHead(bytes) == false)
            {
                throw new SerializationException("反序列化头必须为" + string.Join("-", HEAD.Select(it => it.ToString("X2").ToUpperInvariant())));
            }
            using (var stream = new MemoryStream(4096))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Position = HEAD.Length;
                if (bytes[0] == 0)
                {
                    stream.Position += 1;
                }
                using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
                {
                    bytes = ReadAll(gzip).ToArray();
                    stream.SetLength(0);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Position = 0;
                    return Read(stream, args);
                }
            }
        }

        /// <summary> 读取流中的所有字节
        /// </summary>
        /// <param name="stream"></param>
        private static IEnumerable<byte> ReadAll(Stream stream)
        {
            int length = 1024;
            byte[] buffer = new byte[length];
            int index = 0;
            while ((index = stream.Read(buffer, 0, length)) > 0)
            {
                for (int i = 0; i < index; i++)
                {
                    yield return buffer[i];
                }
            }
        }

        /// <summary> 将对象序列化成字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetString(object obj, FormatterArgs args = null)
        {
            unsafe
            {
                var buffer = GetBytes(obj, args);
                var length = buffer.Length;
                fixed (byte* p = buffer)
                {
                    if ((length & 1) == 0) //2的倍数
                    {
                        return new string((char*)p, 0, length >> 1); //除以2;
                    }
                    //忽略头位的0
                    return new string((char*)(p + 1), 0, length >> 1);
                }
            }
        }

        /// <summary> 将字符串反序列化为对象
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static object GetObject(string str, FormatterArgs args = null)
        {
            if (str == null || str.Length == 0)
            {
                return null;
            }
            var chars = str.ToCharArray();
            var buffer = new byte[str.Length << 1];
            Buffer.BlockCopy(chars, 0, buffer, 0, buffer.Length);
            return GetObject(buffer, args);
        }
    }
}
