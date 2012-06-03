REM SET BUILD=Debug
SET BUILD=Release

COPY ..\src\ServiceStack.Authentication.MongoDB\bin\%BUILD%\ServiceStack.Authentication.MongoDB.* ..\NuGet\ServiceStack.Authentication.MongoDB\lib
COPY ..\src\ServiceStack.Authentication.NHibernate\bin\%BUILD%\ServiceStack.Authentication.NHibernate.* ..\NuGet\ServiceStack.Authentication.NHibernate\lib
