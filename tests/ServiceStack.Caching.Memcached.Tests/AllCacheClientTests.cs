using System;
using System.Configuration;
using Enyim.Caching.Configuration;
using NUnit.Framework;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.Redis;

namespace ServiceStack.Caching.Memcached.Tests
{
	[TestFixture]
	[Ignore("Ignoring integration tests that require infracture")]
	public class AllCacheClientTests : AllCacheClientsTestBase
	{
        [SetUp]
        public void Setup()
        {
            ServiceStack.Logging.LogManager.LogFactory = new ConsoleLogFactory();
        } 

		[Test]
		public void Memory_GetAll_returns_missing_keys()
		{
			AssertGetAll(new MemoryCacheClient());
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
        }

        [Test]
        public void Can_Store_And_Get_Complex_Type()
        {
            var value = TestType.Create();

            var client = new MemcachedClientCache(TestConfig.MasterHosts);
            Assert.IsTrue(client.Set("asdf", value));

            var target = client.Get<TestType>("asdf");
            Assert.AreEqual(TestType.Create(), target);
        }

        [Test]
        public void Can_create_Instance_using_ConfigurationSection()
        {
            var client = new MemcachedClientCache();
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