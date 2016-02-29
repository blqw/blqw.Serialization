using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization
{
    /// <summary>
    /// 引用对象缓存
    /// </summary>
    static class ReferencedCache
    {
        /// <summary>
        /// using上下文
        /// </summary>
        /// <returns></returns>
        public static IDisposable Context()
        {
            if (Cache == null)
            {
                return new ReferencedCacheContext();
            }
            return null;
        }

        [ThreadStatic]
        static ArrayList Cache;

        class ReferencedCacheContext : IDisposable
        {
            public void Dispose()
            {
                Cache?.Clear();
                Cache = null;
            }
        }

        /// <summary>
        /// 根据索引获取引用对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static object GetAt(int index)
        {
            return Cache[index];
        }

        /// <summary>
        /// 添加或者获取引用对象所在引用位
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int AddOrGet(object obj)
        {
            if (obj == null || obj is ValueType || obj is string)
            {
                return -1;
            }
            if (Cache == null)
            {
                Cache = new ArrayList();
            }
            for (int i = Cache.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(Cache[i], obj))
                {
                    return i;
                }
            }
            Cache.Add(obj);
            return -1;
        }

        /// <summary>
        /// 添加引用对象
        /// </summary>
        public static void Add(object obj)
        {
            if (obj != null && obj is ValueType == false)
            {
                if (Cache == null)
                {
                    Cache = new ArrayList();
                }
                Cache.Add(obj);
            }
        }
    }
}
