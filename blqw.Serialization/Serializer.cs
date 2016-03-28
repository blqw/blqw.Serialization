using blqw.Serialization;
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
        #region ThreadStatic
        /// <summary>
        /// 当前上下文中使用的 <seealso cref="SerializationBinder"/>
        /// </summary>
        [ThreadStatic]
        public static SerializationBinder CurrentBinder;
        /// <summary>
        /// 当前上下文中使用的 <seealso cref="StreamingContext"/>
        /// </summary>
        [ThreadStatic]
        public static StreamingContext CurrentContext;
        /// <summary>
        /// 当前上下文中使用的 <seealso cref="ISurrogateSelector"/>
        /// </summary>
        [ThreadStatic]
        public static ISurrogateSelector CurrentSurrogateSelector;
        #endregion

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
        public static void Write(Stream stream, object obj)
        {
            using (ReferencedCache.Context())
            {
                var refindex = ReferencedCache.AddOrGet(obj);
                if (refindex < 0)
                {
                    var formatter = FormatterCache.GetProvider(obj);
                    FormatterCache.ByteFormatter.Serialize(stream, formatter.Flag); //写入片段类型标识
                    formatter.Formatter.Serialize(stream, obj);
                }
                else
                {
                    var formatter = FormatterCache.GetProvider(HeadFlag.Referenced);
                    FormatterCache.ByteFormatter.Serialize(stream, formatter.Flag); //写入片段类型标识
                    formatter.Formatter.Serialize(stream, refindex);
                }
            }
        }

        /// <summary> 将对象序列化成字节流
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] GetBytes(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            using (var stream = new MemoryStream(4096))
            {
                Write(stream, obj);
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
        public static object GetObject(Byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }
            if (ValidationHead(bytes) == false)
            {
                throw new System.Runtime.Serialization.SerializationException("反序列化头必须为" + string.Join("-", HEAD.Select(it => it.ToString("X2").ToUpperInvariant())));
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
                    return Read(stream);
                }
            }
        }

        /// <summary>
        /// 从流中读取数据,并反序列化为对象
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static object Read(Stream stream)
        {
            using (ReferencedCache.Context())
            {
                TraceDeserialize.WriteName("flag");
                TraceDeserialize.SetWriting(false);
                var fragmentType = (HeadFlag)FormatterCache.ByteFormatter.Deserialize(stream); //读取片段类型标识
                TraceDeserialize.SetWriting(true);
                TraceDeserialize.WriteValue(fragmentType.ToString());
                var formatter = FormatterCache.GetProvider(fragmentType);
                return formatter.Formatter.Deserialize(stream);
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
        public static string GetString(object obj)
        {
            unsafe
            {
                var buffer = GetBytes(obj);
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
        public static object GetObject(string str)
        {
            if (str == null || str.Length == 0)
            {
                return null;
            }
            var chars = str.ToCharArray();
            var buffer = new byte[str.Length << 1];
            Buffer.BlockCopy(chars, 0, buffer, 0, buffer.Length);
            return GetObject(buffer);
        }
    }
}
