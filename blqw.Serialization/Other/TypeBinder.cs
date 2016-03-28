using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Serialization
{
    public sealed class TypeBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            typeName = typeName + ", " + assemblyName;
            var type = Type.GetType(typeName, false, true);
            if (type == null)
            {
                throw new SerializationException($"没有找到[{typeName}]类型");
            }
            return type;
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = serializedType?.Assembly.FullName;
            typeName = serializedType?.FullName;
        }
    }
}
