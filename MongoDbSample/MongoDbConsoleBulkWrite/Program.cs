using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics;

namespace MongoDbConsoleBasic
{
    public class User
    {
        // The unique user identity - Mongo can provide a unique Id but if you're linking back to SQL stick to an in
        [BsonId]
        public int UserId;

        // The users position data as a nested object, see class below for details
        public List<PositionItem> UserPositions;

        public override string ToString()
        {
            string res = string.Format("UserId = {0}\n", UserId);
            res += string.Format("UserPositions (count = {0})\n", UserPositions.Count);

            foreach (var item in UserPositions)
            {
                res += item.ToString();
            }

            return res;
        }
    }

    // Supporting/Partial Class for position data
    public class PositionItem
    {
        // The date time for this position
        public int PositionDate;

        // Doesn't have to be a string, but basically whatever data identifies the position
        public string Position;

        public override string ToString()
        {
            string res = string.Format("PositionItem: Date = {0}, Position = {1}\n", PositionDate, Position);
            return res;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // get a mongoclient using the default connection string
            var mongo = new MongoClient();

            // get (and create if doesn't exist) a database from the mongoclient
            var db = mongo.GetDatabase("UserPositionsDb");

            // get a collection of User (and create if it doesn't exist)
            var collection = db.GetCollection<User>("UserCollection");

            // Random positions generator
            Random rand = new Random(DateTime.Now.Second);

            Stopwatch sw = new Stopwatch();


            // Increase the day number to simulate the daily update for many users
            // to check how the record addition slows the db when the records number grows
            for (int day = 0; day < 100; day++)
            {
                List<WriteModel<User>> _listBulkUserWrites = new List<WriteModel<User>>();

                sw.Reset();
                sw.Start();

                // Loop for 16*12 users data update
                for (int userID = 1000; userID < 1000 + 16 * 12; userID++)
                {
                    // Add the new record to the document
                    var positionItem = new PositionItem
                    {
                        Position = (41 + rand.NextDouble() * 2).ToString() + "," + (11 + rand.NextDouble() * 2).ToString(),
                        PositionDate = day
                    };

                    var updateUserFilter = Builders<User>.Filter.Eq(u => u.UserId, userID);
                    var updateUserDefinition = Builders<User>.Update.Push(u => u.UserPositions, positionItem);

                    _listBulkUserWrites.Add(new UpdateOneModel<User>(updateUserFilter,
                                updateUserDefinition)
                                { IsUpsert = true }
                    );
                }

                if (_listBulkUserWrites.Count > 0)
                    collection.BulkWriteAsync(_listBulkUserWrites);

                sw.Stop();

                Console.WriteLine("Elapsed Time = {0}ms", sw.Elapsed.TotalMilliseconds);
            }
        }
    }
}
