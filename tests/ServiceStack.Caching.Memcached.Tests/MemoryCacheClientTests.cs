using System;
using NUnit.Framework;

namespace ServiceStack.Caching.Memcached.Tests
{
    [TestFixture]
    public class MemoryCacheClientTests
    {
        MemoryCacheClient cache;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            cache = new MemoryCacheClient();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            cache.Dispose();
            cache = null;
        }

        [Test]
        public void Get_before_Add_local_DateTime_expires()
        {
            cache.FlushAll();
            string key = "a";
            string value = "aValue";
            cache.Add(key, value, DateTime.Now.AddMilliseconds(200));
            var retVal = cache.Get(key);
            Assert.That(retVal, Is.SameAs(value));
        }

        [Test]
        public void Get_after_Add_local_DateTime_expires()
        {
            cache.FlushAll();
            string key = "a";
            string value = "aValue";
            cache.Add(key, value, DateTime.Now.AddMilliseconds(200));
            System.Threading.Thread.Sleep(250);
            var retVal = cache.Get(key);
            Assert.That(retVal, Is.Null);
        }


        [Test]
        public void Get_before_Add_utc_DateTime_expires()
        {
            cache.FlushAll();
            string key = "a";
            string value = "aValue";
            cache.Add(key, value, DateTime.UtcNow.AddMilliseconds(200));
            var retVal = cache.Get(key);
            Assert.That(retVal, Is.SameAs(value));
        }

        [Test]
        public void Get_after_Add_utc_DateTime_expires()
        {
            cache.FlushAll();
            string key = "a";
            string value = "aValue";
            cache.Add(key, value, DateTime.UtcNow.AddMilliseconds(200));
            System.Threading.Thread.Sleep(250);
            var retVal = cache.Get(key);
            Assert.That(retVal, Is.Null);
        }

        [Test]
        public void Get_before_Add_TimeSpan_expires()
        {
            cache.FlushAll();
            string key = "a";
            string value = "aValue";
            cache.Add(key, value, TimeSpan.FromMilliseconds(200));
            var retVal = cache.Get(key);
            Assert.That(retVal, Is.SameAs(value));
        }

        [Test]
        public void Get_after_Add_TimeSpan_expires()
        {
            cache.FlushAll();
            string key = "a";
            string value = "aValue";
            cache.Add(key, value, TimeSpan.FromMilliseconds(200));
            System.Threading.Thread.Sleep(250);
            var retVal = cache.Get(key);
            Assert.That(retVal, Is.Null);
        }

        [Test]
        public void Get_before_Set_local_DateTime_expires()
        {
            cache.FlushAll();
            string key = "a";
            string value = "aValue";
            cache.Set(key, value, DateTime.Now.AddMilliseconds(200));
            var retVal = cache.Get(key);
            Assert.That(retVal, Is.SameAs(value));
        }

        [Test]
        public void Get_after_Set_local_DateTime_expires()
        {
            cache.FlushAll();
            string key = "a";
            string value = "aValue";
            cache.Set(key, value, DateTime.Now.AddMilliseconds(200));
            System.Threading.Thread.Sleep(250);
            var retVal = cache.Get(key);
            Assert.That(retVal, Is.Null);
        }

        [Test]
        public void Get_before_Set_utc_DateTime_expires()
        {
            cache.FlushAll();
            string key = "a";
            string value = "aValue";
            cache.Set(key, value, DateTime.UtcNow.AddMilliseconds(200));
            var retVal = cache.Get(key);
            Assert.That(retVal, Is.SameAs(value));
        }

        [Test]
        public void Get_after_Set_utc_DateTime_expires()
        {
            cache.FlushAll();
            string key = "a";
            string value = "aValue";
            cache.Set(key, value, DateTime.UtcNow.AddMilliseconds(200));
            System.Threading.Thread.Sleep(250);
            var retVal = cache.Get(key);
            Assert.That(retVal, Is.Null);
        }

        [Test]
        public void Get_before_Set_TimeSpan_expires()
        {
            cache.FlushAll();
            string key = "a";
            string value = "aValue";
            cache.Set(key, value, TimeSpan.FromMilliseconds(200));
            var retVal = cache.Get(key);
            Assert.That(retVal, Is.SameAs(value));
        }

        [Test]
        public void Get_after_Set_TimeSpan_expires()
        {
            cache.FlushAll();
            string key = "a";
            string value = "aValue";
            cache.Set(key, value, TimeSpan.FromMilliseconds(200));
            System.Threading.Thread.Sleep(250);
            var retVal = cache.Get(key);
            Assert.That(retVal, Is.Null);
        }

        [Test]
        public void Get_before_Replace_local_DateTime_expires()
        {
            cache.FlushAll();
            string key = "a";
            string value = "aValue";
            cache.Set(key, value);
            cache.Replace(key, value, DateTime.Now.AddMilliseconds(200));
            var retVal = cache.Get(key);
            Assert.That(retVal, Is.SameAs(value));
        }

        [Test]
        public void Get_after_Replace_local_DateTime_expires()
        {
            cache.FlushAll();
            string key = "a";
            string value = "aValue";
            cache.Set(key, value);
            cache.Replace(key, value, DateTime.Now.AddMilliseconds(200));
            System.Threading.Thread.Sleep(250);
            var retVal = cache.Get(key);
            Assert.That(retVal, Is.Null);
        }

        [Test]
        public void Get_before_Replace_utc_DateTime_expires()
        {
            cache.FlushAll();
            string key = "a";
            string value = "aValue";
            cache.Set(key, value);
            cache.Replace(key, value, DateTime.UtcNow.AddMilliseconds(200));
            var retVal = cache.Get(key);
            Assert.That(retVal, Is.SameAs(value));
        }

        [Test]
        public void Get_after_Replace_utc_DateTime_expires()
        {
            cache.FlushAll();
            string key = "a";
            string value = "aValue";
            cache.Set(key, value);
            cache.Replace(key, value, DateTime.UtcNow.AddMilliseconds(200));
            System.Threading.Thread.Sleep(250);
            var retVal = cache.Get(key);
            Assert.That(retVal, Is.Null);
        }

        [Test]
        public void Get_before_Replace_TimeSpan_expires()
        {
            cache.FlushAll();
            string key = "a";
            string value = "aValue";
            cache.Set(key, value);
            cache.Replace(key, value, TimeSpan.FromMilliseconds(200));
            var retVal = cache.Get(key);
            Assert.That(retVal, Is.SameAs(value));
        }

        [Test]
        public void Get_after_Replace_TimeSpan_expires()
        {
            cache.FlushAll();
            string key = "a";
            string value = "aValue";
            cache.Set(key, value);
            cache.Replace(key, value, TimeSpan.FromMilliseconds(200));
            System.Threading.Thread.Sleep(250);
            var retVal = cache.Get(key);
            Assert.That(retVal, Is.Null);
        }
    }
}