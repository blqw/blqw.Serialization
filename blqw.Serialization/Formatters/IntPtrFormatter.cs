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
    /// 提供 <see cref="IntPtr"/>对象的序列化和反序列化操作
    /// </summary>
    [Export(typeof(IFormatter))]
    [ExportMetadata("BindType", typeof(IntPtr))]
    [ExportMetadata("HeadFlag", HeadFlag.IntPtr)]
    public sealed class IntPtrFormatter:SerializableFormatter
    {

    }
}
