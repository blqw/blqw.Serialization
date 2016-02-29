using blqw.SerializationComponent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization.Formatters
{
    /// <summary>
    /// 提供枚举类型的序列化和反序列化操作
    /// </summary>
    [System.ComponentModel.Composition.Export("ObjectFormatter", typeof(ObjectFormatter))]
    public sealed class EnumFormatter : ObjectFormatter
    {
        public override FormatterFragmentType FragmentType
        {
            get
            {
                return FormatterFragmentType.Enum;
            }
        }

        public override object Deserialize(Stream serializationStream)
        {
            var typeName = (string)FormatterCache.StringFormatter.Deserialize(serializationStream);
            var type = Type.GetType(typeName, false);
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
            FormatterCache.StringFormatter.Serialize(serializationStream, type.AssemblyQualifiedName);
            Serializer.Write(serializationStream, Convert.ChangeType(graph, type.GetEnumUnderlyingType()));
        }

    }
}
