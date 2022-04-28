# General
This folder contains the source code for the Azure Functions that do the database inserts. Each Function invocation only inserts a single message into the database, as a __Streaming Ingest__ approach has been taken.

# Setup
It is recommended to deploy the Azure Functions via Visual Studio using [this guide](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs?tabs=in-process#publish-to-azure).