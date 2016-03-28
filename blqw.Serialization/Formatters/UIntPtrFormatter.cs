using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization.Formatters
{
    /// <summary>
    /// 提供 <see cref="UIntPtr"/>对象的序列化和反序列化操作
    /// </summary>
    [Export(typeof(IFormatter))]
    [ExportMetadata("BindType", typeof(UIntPtr))]
    [ExportMetadata("HeadFlag", HeadFlag.UIntPtr)]
    public sealed class UIntPtrFormatter : SerializableFormatter
    {
    }
}
