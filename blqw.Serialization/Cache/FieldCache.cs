using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Reflection;

namespace blqw.Serialization
{
    /// <summary>
    /// 公开属性缓存
    /// </summary>
    static class FieldCache
    {
        static readonly ConcurrentDictionary<FieldInfo, FieldHandler> _PropertyCache = new ConcurrentDictionary<FieldInfo, FieldHandler>();

        static readonly ConcurrentDictionary<Type, FieldHandler[]> _TypeCache = new ConcurrentDictionary<Type, FieldHandler[]>();

        static readonly FieldHandler[] _Empty = new FieldHandler[0];

        /// <summary>
        /// 根据类型获取操作字段的对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FieldHandler[] GetByType(Type type)
        {
            if (type == null)
            {
                return null;
            }
            FieldHandler[] result;
            if (_TypeCache.TryGetValue(type, out result))
            {
                return result;
            }

            var ps = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);

            var length = ps.Length;
            if (length == 0)
            {
                _TypeCache.TryAdd(type, _Empty);
                return _Empty;
            }
            result = new FieldHandler[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = Get(ps[i]);
            }
            _TypeCache.TryAdd(type, result);
            return result;
        }

        /// <summary>
        /// 根据字段获取操作字段的对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FieldHandler Get(FieldInfo property)
        {
            if (property == null)
            {
                return null;
            }
            FieldHandler handler;
            if (_PropertyCache.TryGetValue(property, out handler))
            {
                return handler;
            }
            handler = new FieldHandler(property);
            _PropertyCache.TryAdd(property, handler);
            return handler;
        }
    }
}
