using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization.Formatters
{
    /// <summary>
    /// 提供 <see cref="ISerializable"/>对象的序列化和反序列化操作
    /// </summary>
    public abstract class SerializableFormatter : FormatterBase
    {
        public override object Deserialize(Stream serializationStream)
        {
            TraceDeserialize.Write("(");
            var length = 0;
            unsafe
            {
                var p = (byte*)&length;
                p[0] = (byte)serializationStream.ReadByte();
                p[1] = (byte)serializationStream.ReadByte();
                p[2] = (byte)serializationStream.ReadByte();
                p[3] = (byte)serializationStream.ReadByte();
            }
            if (length == 0)
            {
                TraceDeserialize.WriteValue(null);
                return null;
            }
            TraceDeserialize.WriteValue(length);
            if (_Formatter == null)
            {
                _Formatter = new BinaryFormatter();
            }
            TraceDeserialize.WriteValue(">>BinaryFormatter<<");
            TraceDeserialize.Write(")");
            byte[] bytes = new byte[length];
            serializationStream.Read(bytes, 0, length);
            using (var stream = new MemoryStream(bytes))
            {
                return _Formatter.Deserialize(stream);
            }
        }

        [ThreadStatic]
        static BinaryFormatter _Formatter;

        public override void Serialize(Stream serializationStream, object graph)
        {
            var start = serializationStream.Position;
            serializationStream.WriteByte(0);
            serializationStream.WriteByte(0);
            serializationStream.WriteByte(0);
            serializationStream.WriteByte(0);

            if (graph == null)
            {
                return;
            }

            if (_Formatter == null)
            {
                _Formatter = new BinaryFormatter();
            }

            _Formatter.Serialize(serializationStream, graph);
            var end = serializationStream.Position;

            var length = end - start - 4;
            serializationStream.Position = start;
            unsafe
            {
                var p = (byte*)&length;
                serializationStream.WriteByte(p[0]);
                serializationStream.WriteByte(p[1]);
                serializationStream.WriteByte(p[2]);
                serializationStream.WriteByte(p[3]);
            }
            serializationStream.Position = end;
        }
    }
}
