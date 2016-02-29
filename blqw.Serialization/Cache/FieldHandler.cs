using blqw.SerializationComponent;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization
{
    /// <summary>
    /// 用于操作属性的Get和Set
    /// </summary>
    class FieldHandler
    {
        public FieldHandler(FieldInfo field)
        {
            Field = field;
            Name = field.Name;
            FieldType = field.FieldType;
            if (Component.GetGeter != null && Component.GetSeter != null)
            {
                GetValue = Component.GetGeter(field);
                SetValue = Component.GetSeter(field);
                return;
            }
            var o = Expression.Parameter(typeof(object), "o");
            var cast = Expression.Convert(o, field.DeclaringType);
            var p = Expression.Field(cast, field);
            {
                var ret = Expression.Convert(p, typeof(object));
                var get = Expression.Lambda<Func<object, object>>(ret, o);
                GetValue = get.Compile();
            }

            if (field.IsInitOnly)
            {
                SetValue = field.SetValue;
            }
            else if (field.IsLiteral == false)
            {
                var v = Expression.Parameter(typeof(object), "v");
                var val = Expression.Convert(v, FieldType);
                var assign = Expression.MakeBinary(ExpressionType.Assign, p, val);
                var ret = Expression.Convert(assign, typeof(object));
                var set = Expression.Lambda<Action<object, object>>(ret, o, v);
                SetValue = set.Compile();
            }
        }
        public Func<object, object> GetValue { get; }
        public Action<object, object> SetValue { get; }
        public FieldInfo Field { get; }
        public string Name { get; }
        public Type FieldType { get; }
    }
}
