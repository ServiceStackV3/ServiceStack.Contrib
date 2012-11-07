using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using NUnit.Framework;

namespace ServiceStack.CacheAccess.AwsDynamoDb.Tests
{
    [TestFixture]
    public class DynamoDbCacheClientTests
    {
        // Replace these with your prod/test AWS account to run unit tests.
        private const string yourAwsAccountKey = "SOME.KEY";
        private const string yourAwsSecretKey = "SOME.SECRET.KEY";

        public class DummyObject
        {
            public string UserId { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public List<string> Friends { get; set; }        
        }

        private ICacheClient _client;
        private DummyObject _item;
        private string _itemCacheKey = "urn:dummyobject:john.doe";
        private string _counterCacheKey = "urn:counter";        

        [SetUp]
        public void SetupTests()
        {

            _client = new DynamoDbCacheClient(yourAwsAccountKey, yourAwsSecretKey,
                                              RegionEndpoint.USEast1, "ICacheClientDynamoDb", 10, 5, true);
            // The primary item we'll be caching in the tests.
            _item = new DummyObject
                        {
                            UserId = "john.doe",
                            Email = "john.doe@servicestack.net",
                            Phone = "555-555-9876",
                            Friends = new List<string> {"jane.doe", "jack.doe", "some.friend"}
                        };            
        }

        [Test]
        public void DyanmoDb_ExerciseCacheClient()
        {
            // Expecting the Set operation to succeed
            bool setResponse = _client.Set<DummyObject>(_itemCacheKey, _item);
            Assert.AreEqual(true, setResponse);

            // Expecting the Get to return the item cached above
            var actual = _client.Get<DummyObject>(_itemCacheKey);
            Assert.IsNotNull(actual);
            Assert.AreEqual(_item.UserId, actual.UserId);

            // Expecting Add to return false since the item is already cached
            bool addResponse = _client.Add<DummyObject>(_itemCacheKey, _item);
            Assert.AreEqual(false, addResponse);

            // Expecting remove to succeed
            bool removeResponse = _client.Remove(_itemCacheKey);
            Assert.AreEqual(true, removeResponse);

            // Add the item back, expecting success
            addResponse = _client.Add<DummyObject>(_itemCacheKey, _item);
            Assert.AreEqual(true, addResponse);

            // Remove it again
            removeResponse = _client.Remove(_itemCacheKey);

            // Clear the counter if it exists
            removeResponse = _client.Remove(_counterCacheKey);

            // Initialize the counter, incResponse should be equal to 0 since the counter doesn't exist
            long incResponse = _client.Increment(_counterCacheKey, 0);
            
            // Increment by 1
            long updatedIncResponse = _client.Increment(_counterCacheKey, 1);
            Assert.AreEqual(incResponse + 1, updatedIncResponse);
            // Decrement by 1
            long decResponse = _client.Decrement(_counterCacheKey, 1);
            Assert.AreEqual(incResponse, decResponse);

            // Clear out the cache - this will cause a very long delete/re-create DynamoDB table sequence
            _client.FlushAll();
            
        }
    }
}
