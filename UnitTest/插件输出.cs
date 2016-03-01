using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition;

namespace blqw
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void 测试插件输出()
        {
            {
                var a = new User { id = 1, name = "blqw" };
                var bytes = Component.GetBytes(a);
                var b = (User)Component.GetObject(bytes);
                Assert.IsNotNull(b);
                Assert.AreEqual(a.id, b.id);
                Assert.AreEqual(a.name, b.name);


                var str = Component.GetString(a);
                var c = (User)Component.GetObjectFormString(str);
                Assert.IsNotNull(c);
                Assert.AreEqual(a.id, c.id);
                Assert.AreEqual(a.name, c.name);
            }
            {
                var obj = new { id = 456, name = "blqw" };
                var a = Component.GetBytes(obj);
                dynamic b = Component.GetObject(a);
                Assert.IsNotNull(b);
                Assert.AreEqual(obj.id, b.id);
                Assert.AreEqual(obj.name, b.name);
                Assert.AreEqual(obj.ToString(), b.ToString());
            }
        }
    }

    class Component
    {
        static Component()
        {
            MEFPart.Import(typeof(Component));
        }

        /// <summary> 序列化
        /// </summary>
        [Import("SerializeToString")]
        public static readonly Func<object, string> GetString = o =>
        {
            throw new NotImplementedException();
        };


        /// <summary> 序列化
        /// </summary>
        [Import("SerializeToBytes")]
        public static readonly Func<object, byte[]> GetBytes = o =>
        {
            throw new NotImplementedException();
        };


        /// <summary> 反序列化
        /// </summary>
        [Import("DeserializeFormBytes")]
        public static readonly Func<byte[], object> GetObject = o =>
        {
            throw new NotImplementedException();
        };


        /// <summary> 反序列化
        /// </summary>
        [Import("DeserializeFromString")]
        public static readonly Func<string, object> GetObjectFormString = o =>
        {
            throw new NotImplementedException();
        };

    }
}
