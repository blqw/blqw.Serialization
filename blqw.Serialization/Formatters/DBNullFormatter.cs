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
    /// 提供 <see cref="DBNull"/>对象的序列化和反序列化操作
    /// </summary>
    [Export(typeof(IFormatter))]
    [ExportMetadata("BindType", typeof(DBNull))]
    [ExportMetadata("HeadFlag", HeadFlag.DBNull)]
    public sealed class DBNullFormatter:FormatterBase
    {
        public override void Serialize(Stream serializationStream, object graph)
        {

        }
        public override object Deserialize(Stream serializationStream)
        {
            return DBNull.Value;
        }
    }
}
