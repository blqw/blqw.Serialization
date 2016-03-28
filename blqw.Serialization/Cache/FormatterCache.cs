using blqw.SerializationComponent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization
{
    public class FormatterCache
    {
        class MEFImports
        {
            [ImportMany(typeof(IFormatter))]
            public Lazy<IFormatter, IFormatterData>[] Formatters;
        }

        static FormatterProvider[] Cache = Init();

        static IDictionary BindTypes;

        private static FormatterProvider[] Init()
        {
            var imports = new MEFImports();
            MEFPart.Import(imports);
            BindTypes = new Dictionary<Type, FormatterProvider>();
            var providers = new FormatterProvider[256];

            foreach (var item in imports.Formatters)
            {
                var prov = new FormatterProvider(item.Value, item.Metadata);
                providers[(int)item.Metadata.HeadFlag] = prov;
                var bt = item.Metadata.BindType;
                if (bt != typeof(object))
                {
                    if (bt.IsGenericType && bt.IsGenericTypeDefinition)
                    {
                        throw new NotSupportedException($"{bt.AssemblyQualifiedName} 类型属性BindType有误(绑定类型不能是泛型定义类)");
                    }
                    BindTypes.Add(bt, prov);
                }
            }

            return providers;
        }

        /// <summary>
        /// 获取用于序列化和反序列化的格式工具
        /// </summary>
        /// <param name="fragmentType">数据类型</param>
        /// <returns></returns>
        public static FormatterProvider GetProvider(HeadFlag fragmentType)
        {
            return Cache[(int)fragmentType];
        }


        public static FormatterProvider GetProvider(Type type)
        {
            if (type == null)
            {
                return Cache[(int)TypeCode.Empty];
            }
            if (type.IsEnum)
            {
                return Cache[(int)HeadFlag.Enum];
            }
            if (type.IsArray)
            {
                return Cache[(int)HeadFlag.Array];
            }

            var code = Type.GetTypeCode(type);
            if (code != TypeCode.Object)
            {
                return Cache[(int)code];
            }

            var formatter = BindTypes[type] as FormatterProvider;
            if (formatter != null)
            {
                return formatter;
            }

            if (type.IsValueType)
            {
                return Cache[(int)HeadFlag.ValueType];
            }
            return Cache[(int)HeadFlag.Object];
        }

        /// <summary>
        /// 获取用于序列化和反序列化的格式工具
        /// </summary>
        /// <param name="obj">需要序列化的对象</param>
        /// <returns></returns>
        public static FormatterProvider GetProvider(object obj)
        {
            return GetProvider(obj?.GetType());
        }

        static IFormatter _Int32Formatter;
        internal static IFormatter Int32Formatter
        {
            get
            {
                return _Int32Formatter ??
                      (_Int32Formatter = GetProvider(HeadFlag.Int32).Formatter);
            }
        }

        static IFormatter _ByteFormatter;
        internal static IFormatter ByteFormatter
        {
            get
            {
                return  _ByteFormatter ??
                       (_ByteFormatter = GetProvider(HeadFlag.Byte).Formatter);
            }
        }

        static IFormatter _StringFormatter;
        internal static IFormatter StringFormatter
        {
            get
            {
                return  _StringFormatter ??
                       (_StringFormatter = GetProvider(HeadFlag.String).Formatter);
            }
        }
    }
}
