using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace blqw
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void 测试实体对象()
        {
            var a = new User { id = 1, name = "blqw" };
            var bytes = Serializer.GetBytes(a);
            var b = (User)Serializer.GetObject(bytes);
            Assert.IsNotNull(b);
            Assert.AreEqual(a.id, b.id);
            Assert.AreEqual(a.name, b.name);


            var str = Serializer.GetString(a);
            var c = (User)Serializer.GetObject(str);
            Assert.IsNotNull(c);
            Assert.AreEqual(a.id, c.id);
            Assert.AreEqual(a.name, c.name);
        }

        sealed class MyClass
        {

        }
        [TestMethod]
        public void 测试数组的空值()
        {
            MyClass[] arr = new MyClass[2] { null, new MyClass() };

            var a = Serializer.GetBytes(arr);
            var b = Serializer.GetObject(a) as MyClass[];
            Assert.IsNotNull(b);
            Assert.AreEqual(2, b.Length);
            Assert.IsNull(b[0]);
            Assert.IsNotNull(b[1]);
        }

        [TestMethod]
        public void 测试匿名类()
        {
            var obj = new { id = 456, name = "blqw" };
            var a = Serializer.GetBytes(obj);
            dynamic b = Serializer.GetObject(a);
            Assert.IsNotNull(b);
            Assert.AreEqual(obj.id, b.id);
            Assert.AreEqual(obj.name, b.name);
            Assert.AreEqual(obj.ToString(), b.ToString());
        }
    }
}
