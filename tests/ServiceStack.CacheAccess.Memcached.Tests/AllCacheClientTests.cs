using System;
using NUnit.Framework;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Redis;

namespace ServiceStack.CacheAccess.Memcached.Tests
{
	[TestFixture]
	[Ignore("Ignoring integration tests that require infracture")]
	public class AllCacheClientTests : AllCacheClientsTestBase
	{
		[Test]
		public void Memory_GetAll_returns_missing_keys()
		{
			AssertGetAll(new MemoryCacheClient());
		}

		[Test]
		public void Redis_GetAll_returns_missing_keys()
		{
			AssertGetAll(new RedisClient(TestConfig.SingleHost));
		}

		[Test]
		public void Memcached_GetAll_returns_missing_keys()
		{
			AssertGetAll(new MemcachedClientCache(TestConfig.MasterHosts));
		}

		[Test]
		public void Memory_GetSetIntValue_returns_missing_keys()
		{
			AssertGetSetIntValue(new MemoryCacheClient());
		}

		[Test]
		public void Redis_GetSetIntValue_returns_missing_keys()
		{
			AssertGetSetIntValue(new RedisClient(TestConfig.SingleHost));
		}

		[Test]
		public void Memcached_GetSetIntValue_returns_missing_keys()
		{
			var client = new MemcachedClientCache(TestConfig.MasterHosts);
			AssertGetSetIntValue((IMemcachedClient)client);
			AssertGetSetIntValue((ICacheClient)client);
		}

        [Test]
        public void Can_Store_Complex_Type()
        {
            var value = TestType.Create();

            var client = new MemcachedClientCache(TestConfig.MasterHosts);
            Assert.IsTrue(client.Set("asdf", value));
            Assert.AreEqual(TestType.Create(), value);
        }
	}

    public class TestType
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }

        public static TestType Create()
        {
            return new TestType
                       {
                            Id = 1,
                            Text = "some text",
                            Time = new DateTime(1983, 5, 27, 17, 17, 17)
                       };
        }

        public override bool Equals(object obj)
        {
            var testType = obj as TestType;

            if (testType == null) return false;

            return Id == testType.Id && Text == testType.Text && Time == testType.Time;
        }
    }
}