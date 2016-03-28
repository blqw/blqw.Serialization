using blqw.SerializationComponent;
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
    /// 提供枚举类型的序列化和反序列化操作
    /// </summary>
    [Export(typeof(IFormatter))]
    [ExportMetadata("BindType", typeof(Enum))]
    [ExportMetadata("HeadFlag", HeadFlag.Enum)]
    public sealed class EnumFormatter :FormatterBase
    {
        public override object Deserialize(Stream serializationStream)
        {
            TraceDeserialize.WriteName("typeName");
            var type = Binder.DeserializeType(serializationStream);
            var value = Serializer.Read(serializationStream);
            if (type == null)
            {
                return value;
            }
            return Component.Convert(value, type, true);
        }

        public override void Serialize(Stream serializationStream, object graph)
        {
            var type = graph.GetType();
            Binder.SerializeType(serializationStream, type);
            Serializer.Write(serializationStream, Convert.ChangeType(graph, type.GetEnumUnderlyingType()));
        }

    }
}
