SET NUGET=..\src\.nuget\nuget
%NUGET% pack ServiceStack.Authentication.RavenDB\servicestack.authentication.ravendb.nuspec -symbols
%NUGET% pack ServiceStack.Authentication.MongoDB\servicestack.authentication.mongodb.nuspec -symbols
%NUGET% pack ServiceStack.Authentication.NHibernate\servicestack.authentication.nhibernate.nuspec -symbols
%NUGET% pack ServiceStack.Caching.Azure\servicestack.caching.azure.nuspec -symbols
%NUGET% pack ServiceStack.Caching.AwsDynamoDb\servicestack.caching.awsdynamodb.nuspec -symbols
%NUGET% pack ServiceStack.Caching.Memcached\servicestack.caching.memcached.nuspec -symbols

