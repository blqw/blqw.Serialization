using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization
{
    /// <summary>
    /// 用于指示被格式化的数据片段的类型
    /// </summary>
    public enum HeadFlag : byte
    {
        Empty = 0,
        Object = 1,
        DBNull = 2,
        Boolean = 3,
        Char = 4,
        SByte = 5,
        Byte = 6,
        Int16 = 7,
        UInt16 = 8,
        Int32 = 9,
        UInt32 = 10,
        Int64 = 11,
        UInt64 = 12,
        Single = 13,
        Double = 14,
        Decimal = 15,
        DateTime = 16,
        String = 18,
        Enum = 19,
        Guid = 20,
        TimeSpan = 21,
        Uri = 22,
        IntPtr = 23,
        UIntPtr = 24,
        NameValueCollection = 25,
        SerializationInfo = 26,
        /// <summary>
        /// 值类型
        /// </summary>
        ValueType = 252,
        /// <summary>
        /// 数组
        /// </summary>
        Array = 253,
        /// <summary>
        /// <see cref="System.Type"/>
        /// </summary>
        Type = 254,
        /// <summary>
        ///  引用对象
        /// </summary>
        Referenced = 255,
    }
}
