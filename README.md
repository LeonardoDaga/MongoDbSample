# MongoDbSample
My first example using MongoDb to create an archive containing a collection of documents, each document containing a list of information and the time it has been collected.

2 Projects are available:
- MongoDbConsoleBasic: A read/write single test, to verify the functionality
- MongoDbConsoleEfficiencyTestBasic: A multiple write test, to verify the efficiency of the Retrieve/ReplaceOne instructions;
- MongoDbConsoleUpsert: A single write test, to verify the functionality of the UpdateOne (Upsert) instruction
- MongoDbConsoleEfficiencyTestUpsert: A multiple write test, to verify the efficiency of the Update One (Upsert) instructions;

In the MongoDbConsoleEfficiencyTestUpsert test I added in the repository cited above I’ve found that the Upsert time is 3 to 5 (depending of the size of the array) faster than the two round trips (MongoDbConsoleEfficiencyTestBasic) approach.