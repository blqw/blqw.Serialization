using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization
{

    public class FormatterProvider
    {
        public FormatterProvider(IFormatter formatter, IFormatterData metadata)
        {
            Formatter = formatter;
            BindType = metadata.BindType;
            Flag = metadata.HeadFlag;
        }
        public IFormatter Formatter { get; }
        /// <summary>
        /// 绑定格式化类型
        /// </summary>
        public Type BindType { get; }

        /// <summary>
        /// 格式化头标记
        /// </summary>
        public HeadFlag Flag { get; }
    }
}