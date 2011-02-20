using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ServiceStack.ServiceInterface.ServiceModel
{
	[DataContract(Namespace = Config.DefaultNamespace)]
	public class Property
	{
		public string Name { get; set; }
		public string Value { get; set; }
	}

	[CollectionDataContract(Namespace = Config.DefaultNamespace, ItemName = "Property")]
	public class Properties : List<Property>
	{
		public Properties() { }
		public Properties(IEnumerable<Property> collection) : base(collection) { }
	}
}