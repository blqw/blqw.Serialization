using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization.Formatters
{
    /// <summary>
    /// 提供引用对象的序列化和反序列化操作
    /// </summary>
    [System.ComponentModel.Composition.Export("ObjectFormatter", typeof(ObjectFormatter))]
    public sealed class ReferencedFormatter : ObjectFormatter
    {
        public override FormatterFragmentType FragmentType
        {
            get
            {
                return FormatterFragmentType.Referenced;
            }
        }

        public override object Deserialize(Stream serializationStream)
        {
            var refindex = (int)FormatterCache.Int32Formatter.Deserialize(serializationStream);
            return ReferencedCache.GetAt(refindex);
        }

        public override void Serialize(Stream serializationStream, object graph)
        {
            FormatterCache.Int32Formatter.Serialize(serializationStream, graph);
        }
    }
}
