using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.SerializationComponent
{
    [Export("Component")]
    class Component
    {

        static Component()
        {
            MEFPart.Import(typeof(Component));
        }

        [Import("CreateGetter")]
        public static Func<MemberInfo, Func<object, object>> GetGeter;

        [Import("CreateSetter")]
        public static Func<MemberInfo, Action<object, object>> GetSeter;
      
        /// <summary> 用于数据转换的输出插件
        /// </summary>
        [Import("Convert")]
        public static readonly Func<object, Type, bool, object> Convert =
            delegate (object value, Type type, bool throwError)
            {
                if (type == null)
                {
                    throw new ArgumentNullException("type");
                }
                var str = value as string;
                if (str == null)
                {
                    if (value == null)
                    {
                        return null;
                    }
                    try
                    {
                        if (type.IsEnum)
                        {
                            return Enum.ToObject(type, value);
                        }
                        return System.Convert.ChangeType(value, type);
                    }
                    catch
                    {
                        if (throwError)
                        {
                            throw;
                        }
                        return null;
                    }
                }
                if (type == typeof(Guid))
                {
                    Guid g;
                    if (Guid.TryParse(str, out g))
                    {
                        return g;
                    }
                }
                else if (type == typeof(Uri))
                {
                    Uri u;
                    if (Uri.TryCreate(str, UriKind.RelativeOrAbsolute, out u))
                    {
                        return u;
                    }
                }
                else if (type == typeof(TimeSpan))
                {
                    TimeSpan t;
                    if (TimeSpan.TryParse(str, out t))
                    {
                        return t;
                    }
                }
                else if (type == typeof(Type))
                {
                    return Type.GetType(str, false, true);
                }
                else
                {
                    try
                    {
                        return System.Convert.ChangeType(value, type);
                    }
                    catch
                    {
                        if (throwError)
                        {
                            throw;
                        }
                        return null;
                    }
                }
                if (throwError)
                {
                    throw new InvalidCastException("字符串: " + str + " 转为类型:" + Component.Convert(type, typeof(string), false) + " 失败");
                }
                return null;
            };
    }
}
