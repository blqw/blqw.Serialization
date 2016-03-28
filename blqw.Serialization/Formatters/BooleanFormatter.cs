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
    /// 提供 <see cref="bool"/>对象的序列化和反序列化操作
    /// </summary>
    [Export(typeof(IFormatter))]
    [ExportMetadata("BindType", typeof(bool))]
    [ExportMetadata("HeadFlag", HeadFlag.Boolean)]
    public sealed class BooleanFormatter : FormatterBase
    {
        public override void Serialize(Stream serializationStream, object graph)
        {
            serializationStream.WriteByte((bool)graph ? (byte)1 : (byte)0);
        }

        public override object Deserialize(Stream serializationStream)
        {
            var i = serializationStream.ReadByte() > 0;
            TraceDeserialize.WriteValue(i);
            return i;
        }
    }
}
