# MongoDbSample
My first example using MongoDb to create an archive containing a collection of documents, each document containing a list of information and the time it has been collected.

5 Projects are available:
- MongoDbConsoleBasic: A read/write single test, to verify the functionality
- MongoDbConsoleEfficiencyTestBasic: A multiple write test, to verify the efficiency of the Retrieve/ReplaceOne instructions;
- MongoDbConsoleUpsert: A single write test, to verify the functionality of the UpdateOne (Upsert) instruction
- MongoDbConsoleEfficiencyTestUpsert: A multiple write test, to verify the efficiency of the Update One (Upsert) instructions;
- MongoDbConsoleBulkWrite: A multiple write test, to verify the efficiency of the Bulk write. 

In the MongoDbConsoleEfficiencyTestUpsert test I added in the repository cited above I’ve found that the Upsert time is 3 to 5 (depending of the size of the array) faster than the two round trips (MongoDbConsoleEfficiencyTestBasic) approach.
Bulk write demonstrated in MongoDbConsoleBulkWrite is even faster, showing an average time more than 10 times better than the MongoDbConsoleEfficiencyTestUpsert example.

Read MauroPetrini (https://dev.to/mpetrinidev/a-guide-to-bulk-write-operations-in-mongodb-with-c-51fk) for information on a faster approach to write (bulk write) a set of field in a burst way.