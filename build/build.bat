REM SET BUILD=Debug
SET BUILD=Release

COPY ..\src\ServiceStack.ServiceInterface\bin\%BUILD%\ServiceStack.ServiceInterface.* ..\..\ServiceStack\release\latest

COPY ..\src\ServiceStack.ServiceInterface\bin\%BUILD%\ServiceStack.ServiceInterface.* ..\..\ServiceStack\lib
COPY ..\src\ServiceStack.ServiceInterface\bin\%BUILD%\ServiceStack.ServiceInterface.* ..\..\ServiceStack.Examples\lib
COPY ..\src\ServiceStack.ServiceInterface\bin\%BUILD%\ServiceStack.ServiceInterface.* ..\..\ServiceStack.RedisWebServices\lib

