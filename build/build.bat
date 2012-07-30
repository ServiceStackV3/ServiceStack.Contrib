REM SET BUILD=Debug
SET BUILD=Release

COPY ..\src\ServiceStack.Authentication.MongoDB\bin\%BUILD%\ServiceStack.Authentication.MongoDB.* ..\NuGet\ServiceStack.Authentication.MongoDB\lib
COPY ..\src\ServiceStack.Authentication.NHibernate\bin\%BUILD%\ServiceStack.Authentication.NHibernate.* ..\NuGet\ServiceStack.Authentication.NHibernate\lib
COPY ..\src\ServiceStack.CacheAccess.Azure\bin\%BUILD%\ServiceStack.CacheAccess.Azure.* ..\NuGet\ServiceStack.Caching.Azure\lib
COPY ..\src\ServiceStack.CacheAccess.Memcached\bin\%BUILD%\ServiceStack.CacheAccess.Memcached.* ..\NuGet\ServiceStack.Caching.Memcached\lib
REM Using EnyimMemcached from NuGet: COPY ..\src\ServiceStack.CacheAccess.Memcached\bin\%BUILD%\Enyim.Caching.* ..\NuGet\ServiceStack.Caching.Memcached\lib
