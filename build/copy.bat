REM SET BUILD=Debug
SET BUILD=Release

COPY ..\src\ServiceStack.Authentication.MongoDB\bin\%BUILD%\ServiceStack.Authentication.MongoDB.* ..\NuGet\ServiceStack.Authentication.MongoDB\lib
COPY ..\src\ServiceStack.Authentication.RavenDB\bin\%BUILD%\ServiceStack.Authentication.RavenDB.* ..\NuGet\ServiceStack.Authentication.RavenDB\lib
COPY ..\src\ServiceStack.Authentication.NHibernate\bin\%BUILD%\ServiceStack.Authentication.NHibernate.* ..\NuGet\ServiceStack.Authentication.NHibernate\lib
COPY ..\src\ServiceStack.Caching.Azure\bin\%BUILD%\ServiceStack.Caching.Azure.* ..\NuGet\ServiceStack.Caching.Azure\lib
COPY ..\src\ServiceStack.Caching.AwsDynamoDb\bin\%BUILD%\ServiceStack.Caching.AwsDynamoDb.* ..\NuGet\ServiceStack.Caching.AwsDynamoDb\lib
COPY ..\src\ServiceStack.Caching.Memcached\bin\%BUILD%\ServiceStack.Caching.Memcached.* ..\NuGet\ServiceStack.Caching.Memcached\lib

REM Using EnyimMemcached from NuGet: COPY ..\src\ServiceStack.Caching.Memcached\bin\%BUILD%\Enyim.Caching.* ..\NuGet\ServiceStack.Caching.Memcached\lib
