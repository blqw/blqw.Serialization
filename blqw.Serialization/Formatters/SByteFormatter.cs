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
    /// 提供 <see cref="sbyte"/>对象的序列化和反序列化操作
    /// </summary>
    [Export(typeof(IFormatter))]
    [ExportMetadata("BindType", typeof(SByte))]
    [ExportMetadata("HeadFlag", HeadFlag.SByte)]
    public sealed class SByteFormatter :FormatterBase
    {
        public override object Deserialize(Stream serializationStream)
        {
            var i = (sbyte)serializationStream.ReadByte();
            TraceDeserialize.WriteValue(i);
            return i;
        }

        public override void Serialize(Stream serializationStream, object graph)
        {
            serializationStream.WriteByte((byte)(sbyte)graph);
        }
    }
}
