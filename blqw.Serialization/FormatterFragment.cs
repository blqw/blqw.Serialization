using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization
{
    /// <summary>
    /// 格式化片段
    /// </summary>
    public struct FormatterFragment
    {
        /// <summary>
        /// 初始化片段数据
        /// </summary>
        /// <param name="type">片段类型</param>
        /// <param name="value">片段值</param>
        public FormatterFragment(FormatterFragmentType type, object value)
        {
            Type = type;
            Value = value;
        }
        /// <summary>
        /// 片段类型
        /// </summary>
        public FormatterFragmentType Type { get; }
        /// <summary>
        /// 片段的值
        /// </summary>
        public object Value { get; }
    }
}
