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
    /// 提供 <see cref="byte"/>对象的序列化和反序列化操作
    /// </summary>
    [Export(typeof(IFormatter))]
    [ExportMetadata("BindType", typeof(Byte))]
    [ExportMetadata("HeadFlag", HeadFlag.Byte)]
    public sealed class ByteFormatter : FormatterBase
    {
        public override void Serialize(Stream serializationStream, object graph)
        {
            serializationStream.WriteByte((byte)graph);
        }

        public override object Deserialize(Stream serializationStream)
        {
            var i = (byte)serializationStream.ReadByte();
            TraceDeserialize.WriteValue(i);
            return i;
        }

    }
}
