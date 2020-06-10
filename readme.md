LightningQueues - Fast persistent queues for .NET
=====================================================
![.NET Core (Win, Mac, Linux)](https://github.com/LightningQueues/LightningQueues/workflows/.NET%20Core/badge.svg)

A fast store and forward message queue for .NET. (aka not a broker or server)

Why not just use MSMQ?
- 0 Administration required
- XCopy deployable
- XPlat supported

API is completely rewritten using reactive extensions from top to bottom. 
Everything is completely asynchronous and provides at-least-once delivery for your messages.

## How to compile
`dotnet build`

### Run the tests
`dotnet test -f netcoreapp3.1` # or other framework option you choose

#### On Linux
You'll need to compile the the native lmdb binaries first. This is only because the LightningQueues nuget does not include any Linux binaries yet. You can refer
to the github actions for what this looks like, but once that's done...
`LD_LIBRARY_PATH=path_to/lmdb/libraries/liblmdb/:$LD_LIBRARY_PATH dotnet test -f netcoreapp3.1`

#### Transport Security
There is an example test that shows the hooks available to use TLS encryption for the stream. The decision is left to the end user on what level of cert validation to perform.
