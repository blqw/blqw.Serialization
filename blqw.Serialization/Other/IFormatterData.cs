using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization
{
    public interface IFormatterData
    {
        /// <summary>
        /// 绑定格式化类型
        /// </summary>
        Type BindType { get; }

        /// <summary>
        /// 格式化头标记
        /// </summary>
        HeadFlag HeadFlag { get; }
    }
}
