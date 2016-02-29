using blqw.SerializationComponent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization
{
    public static class FormatterCache
    {
        [ImportMany("ObjectFormatter")]
        static readonly ObjectFormatter[] Cache = Init();

        static readonly IDictionary BindTypes;

        private static ObjectFormatter[] Init()
        {
            typeof(FormatterCache).GetField("BindTypes", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, new Dictionary<Type, ObjectFormatter>());

            MEFPart.Import(typeof(FormatterCache));
            var list = new ObjectFormatter[256];

            foreach (var item in Cache)
            {
                list[(int)item.FragmentType] = item;
                if (item.BindType != null)
                {
                    var bt = item.BindType;
                    if (bt.IsGenericType && bt.IsGenericTypeDefinition)
                    {
                        throw new NotSupportedException($"{bt.AssemblyQualifiedName} 类型属性BindType有误(绑定类型不能是泛型定义类)");
                    }
                    BindTypes.Add(bt, item);
                }
            }

            return list;
        }

        /// <summary>
        /// 获取用于序列化和反序列化的格式工具
        /// </summary>
        /// <param name="fragmentType">数据类型</param>
        /// <returns></returns>
        public static ObjectFormatter GetFormatter(FormatterFragmentType fragmentType)
        {
            return Cache[(int)fragmentType];
        }


        public static ObjectFormatter GetFormatter(Type type)
        {
            if (type == null)
            {
                return Cache[(int)TypeCode.Empty];
            }
            if (type.IsEnum)
            {
                return Cache[(int)FormatterFragmentType.Enum];
            }
            if (type.IsArray)
            {
                return Cache[(int)FormatterFragmentType.Array];
            }

            var code = Type.GetTypeCode(type);
            if (code != TypeCode.Object)
            {
                return Cache[(int)code];
            }

            var formatter = (ObjectFormatter)BindTypes[type]; //尝试获取绑定类型的格式化器
            if (formatter != null)
            {
                return formatter;
            }

            if (type.IsValueType)
            {
                return Cache[(int)FormatterFragmentType.ValueType];
            }
            return Cache[(int)FormatterFragmentType.Object];
        }

        /// <summary>
        /// 获取用于序列化和反序列化的格式工具
        /// </summary>
        /// <param name="obj">需要序列化的对象</param>
        /// <returns></returns>
        public static ObjectFormatter GetFormatter(object obj)
        {
            return GetFormatter(obj?.GetType());
        }

        static ObjectFormatter _Int32Formatter;
        internal static ObjectFormatter Int32Formatter
        {
            get
            {
                return _Int32Formatter ??
                    (_Int32Formatter = GetFormatter(FormatterFragmentType.Int32));
            }
        }

        static ObjectFormatter _ByteFormatter;
        internal static ObjectFormatter ByteFormatter
        {
            get
            {
                return _ByteFormatter ??
                    (_ByteFormatter = GetFormatter(FormatterFragmentType.Byte));
            }
        }

        static ObjectFormatter _StringFormatter;
        internal static ObjectFormatter StringFormatter
        {
            get
            {
                return _StringFormatter ??
                    (_StringFormatter = GetFormatter(FormatterFragmentType.String));
            }
        }
    }
}
