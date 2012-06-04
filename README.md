# High-level ServiceStack features and extensions contributed by the community

Although ServiceStack is an opinionated web service framework (i.e. includes most components required to make high-performance web services), each feature is built around pure/clean, dependency-free 
C# interfaces enabling the use of alternate, pluggable, xml/config-free and testable C# components.

By default ServiceStack includes high-performance replacements for [ASP.NET's Session, Caching, Logging, Authentication, Membership and Configuration providers](http://www.servicestack.net/mvc-powerpack/) 
yielding config-free, testable and mockable alternatives that can be hosted in or outside of an ASP.NET web host.

Whilst any providers contributed by the community that require any external dependencies are kept here (with links to their NuGet packages):

## Authentication Providers

ServiceStack's built-in [Authentication and Authorization plugin](https://github.com/ServiceStack/ServiceStack/wiki/Authentication-and-authorization) 
provides an extensible and pluggable model supporting multiple [caching providers](https://github.com/ServiceStack/ServiceStack/wiki/Caching) (for fast pre-request session access) as well as 
multiple datastore providers for long-term persistance of User Registration and Authentication information.

### Caching Providers ([ICacheClient](https://github.com/ServiceStack/ServiceStack/blob/master/src/ServiceStack.Interfaces/CacheAccess/ICacheClient.cs))

  - In Memory: `MemoryCacheClient` in [ServiceStack](https://nuget.org/packages/ServiceStack)
  - Redis: `RedisClient`, `PooledRedisClientManager` and `BasicRedisClientManager` in [ServiceStack.Redis](https://nuget.org/packages/ServiceStack.Redis)
  - Memcached: `MemcachedClientCache` in [ServiceStack.Caching.Memcached](https://nuget.org/packages/ServiceStack.Caching.Memcached)
  - Azure: `AzureCacheClient` in [ServiceStack.Caching.Azure](https://nuget.org/packages/ServiceStack.Caching.Azure) - created by [Manuel Nelson](https://gist.github.com/manuelnelson)

### User Auth Repositories ([IUserAuthRepository](https://github.com/ServiceStack/ServiceStack/blob/master/src/ServiceStack.ServiceInterface/Auth/IUserAuthRepository.cs))

  - OrmLite: `OrmLiteAuthRepository` in [ServiceStack](https://nuget.org/packages/ServiceStack)
  - Redis: `RedisAuthRepository` in [ServiceStack](https://nuget.org/packages/ServiceStack)
  - In Memory: `InMemoryAuthRepository` in [ServiceStack](https://nuget.org/packages/ServiceStack)
  - Mongo DB: `MongoDBAuthRepository` in [ServiceStack.Authentication.MongoDB](https://nuget.org/packages/ServiceStack.Authentication.MongoDB) - created by [Assaf Raman](https://github.com/assaframan)
  - NHibernate: `NHibernateUserAuthRepository` in [ServiceStack.Authentication.NHibernate](https://nuget.org/packages/ServiceStack.Authentication.NHibernate) - created by [Joshua Lewis](https://github.com/joshilewis)

## Compression

As the original compression libraries in .NET 2.0 (they're better now) were both slow and yielded in-efficiently large results, we've enabled adapters to plug-in alternative compression libraries.
The [ServiceStack.Compression](https://github.com/ServiceStack/ServiceStack.Contrib/tree/master/src/ServiceStack.Compression) project provides GZip and Deflate compression adapters for the 
excellent [ICSharpCode](http://www.icsharpcode.net/) libraries which you can enable ServiceStack to use with:

    StreamExtensions.DeflateProvider = new ICSharpDeflateProvider();
    StreamExtensions.GZipProvider = new ICSharpGZipProvider();
