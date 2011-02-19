# Alternative Compression implementations for ServiceStack

Earlier versions of .NET (i.e. 2.0) had a very poor implementation of GZip/DeflateStream 
which was much slower and took longer than the equivalent compression routines provided by
SharpZipLib excellent implementation.

## Usage

To configure ServiceStack to use the deserialization libs in this project add this to your AppHost:

	StreamExtensions.DeflateProvider = new ICSharpDeflateProvider();
	StreamExtensions.GZipProvider = new ICSharpGZipProvider();

