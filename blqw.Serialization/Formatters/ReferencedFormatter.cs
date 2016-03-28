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
    /// 提供引用对象的序列化和反序列化操作
    /// </summary>
    [Export(typeof(IFormatter))]
    [ExportMetadata("BindType", typeof(object))]
    [ExportMetadata("HeadFlag", HeadFlag.Referenced)]
    public sealed class ReferencedFormatter :FormatterBase
    {
        public override object Deserialize(Stream serializationStream)
        {
            var refindex = (int)FormatterCache.Int32Formatter.Deserialize(serializationStream);
            TraceDeserialize.WriteValue($">>ref[{refindex}]<<");
            return ReferencedCache.GetAt(refindex);
        }

        public override void Serialize(Stream serializationStream, object graph)
        {
            FormatterCache.Int32Formatter.Serialize(serializationStream, graph);
        }
    }
}
