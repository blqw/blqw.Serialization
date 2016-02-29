using blqw.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public static class Serializer
    {

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
                    var formatter = FormatterCache.GetFormatter(obj);
                    FormatterCache.ByteFormatter.Serialize(stream, formatter.FragmentType); //写入片段类型标识
                    formatter.Serialize(stream, obj);
                }
                else
                {
                    var formatter = FormatterCache.GetFormatter(FormatterFragmentType.Referenced);
                    FormatterCache.ByteFormatter.Serialize(stream, formatter.FragmentType); //写入片段类型标识
                    formatter.Serialize(stream, refindex);
                }
            }
        }

        /// <summary> 将对象序列化成字节流
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] GetBytes(object obj)
        {
            using (var stream = new MemoryStream(4096))
            {
                Write(stream, obj);
                var data = stream.ToArray();
                stream.SetLength(0);
                using (var gzip = new GZipStream(stream, CompressionMode.Compress))
                {
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
            using (var stream = new MemoryStream(4096))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Position = 0;
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
                var fragmentType = (FormatterFragmentType)FormatterCache.ByteFormatter.Deserialize(stream); //读取片段类型标识
                var formatter = FormatterCache.GetFormatter(fragmentType);
                return formatter.Deserialize(stream);
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
                    //gzip头固定为 [31,139] ,如果压缩后byte为基数,则忽略31
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
            byte[] buffer;
            if ((chars[0] & 0xFF) == 139) //如果头为139
            {
                buffer = new byte[(str.Length << 1) + 1];
                Buffer.BlockCopy(chars, 0, buffer, 1, buffer.Length - 1);
                buffer[0] = (byte)31; //补全gzi压缩头 [31,139]
            }
            else
            {
                buffer = new byte[str.Length << 1];
                Buffer.BlockCopy(chars, 0, buffer, 0, buffer.Length);
            }
            return GetObject(buffer);
        }
    }
}
