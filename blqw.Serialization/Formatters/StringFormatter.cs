using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization.Formatters
{
    /// <summary>
    /// 提供字符串对象序列化与反序列化操作
    /// </summary>
    [Export(typeof(IFormatter))]
    [ExportMetadata("BindType", typeof(String))]
    [ExportMetadata("HeadFlag", HeadFlag.String)]
    public sealed class StringFormatter :FormatterBase
    {
        public override object Deserialize(Stream serializationStream)
        {
            TraceDeserialize.SetWriting(false);
            var length = (int)FormatterCache.GetInt32Formatter(this).Deserialize(serializationStream);
            TraceDeserialize.SetWriting(true);
            if (length == 0)
            {
                TraceDeserialize.WriteValue("");
                return string.Empty;
            }
            var str = new string('\0', length >> 1);

            unsafe
            {
                fixed (char* c = str)
                {
                    var b = (byte*)c;
                    *b = (byte)serializationStream.ReadByte();
                    for (int i = 1; i < length; i++)
                    {
                        b++;
                        *b = (byte)serializationStream.ReadByte();
                    }
                }
            }
            TraceDeserialize.WriteValue(str);
            return str;
        }

        public override void Serialize(Stream serializationStream, object graph)
        {
            var str = (string)graph;
            var length = str.Length << 1; // ×2 ,一个char = 2个byte
            FormatterCache.GetInt32Formatter(this).Serialize(serializationStream, length);
            if (length == 0)
            {
                return;
            }
            unsafe
            {
                fixed (char* p = str)
                {
                    var p1 = (byte*)p;
                    for (int i = 0; i < length; i++)
                    {
                        serializationStream.WriteByte(p1[i]);
                    }
                }
            }
        }

    }
}
