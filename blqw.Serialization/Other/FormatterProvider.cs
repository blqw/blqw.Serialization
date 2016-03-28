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
        static readonly StreamingContext DefaultContext = new StreamingContext();
        static readonly Func<object, object> MemberwiseCloneHandler = (Func<object, object>)typeof(object).GetMethod("MemberwiseClone", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).CreateDelegate(typeof(Func<object, object>));


        public FormatterProvider(IFormatter formatter, IFormatterData metadata)
        {
            _Formatter = formatter;
            BindType = metadata.BindType;
            Flag = metadata.HeadFlag;
        }


        /// <summary>
        /// 根据上一个 <see cref="IFormatter"/>获取当前可用的 <see cref="IFormatter"/>
        /// </summary>
        /// <param name="previous"></param>
        /// <returns></returns>
        public IFormatter GetFormatter(IFormatter previous)
        {
            if (previous == null)
            {
                return _Formatter;
            }
            if (previous.Binder == null
                && previous.SurrogateSelector == null
                && previous.Context.Equals(DefaultContext))
            {
                return _Formatter;
            }
            var newfmt = (IFormatter)MemberwiseCloneHandler(_Formatter);
            newfmt.Binder = previous.Binder;
            newfmt.Context = previous.Context;
            newfmt.SurrogateSelector = previous.SurrogateSelector;
            return newfmt;
        }

        private IFormatter _Formatter;
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