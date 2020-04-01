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
            var mongo = new MongoClient("mongodb://ruserTest:12345678@localhost/UserPositionsDb");

            // get (and create if doesn't exist) a database from the mongoclient
            var db = mongo.GetDatabase("UserPositionsDb");

            // get a collection of User (and create if it doesn't exist)
            var collection = db.GetCollection<User>("UserCollection");

            var resultString = "";

            // Random positions generator
            Random rand = new Random(DateTime.Now.Second);

            Stopwatch sw = new Stopwatch();

            sw.Reset();

            var SolutionToApply = 4;
            double average = 0;

            // Loop for 16*12 users data update
            for (int n = 0; n < 100; n++)
            {
                sw.Reset();
                for (int userID = 1000; userID < 1000 + 16 * 12; userID++)
                {
                    sw.Start();

                    IEnumerable<PositionItem> lastTwoDaysPositions = null;

                    switch (SolutionToApply)
                    {
                        case 1:
                            lastTwoDaysPositions = collection.AsQueryable()
                                .Where(p => p.UserId == userID)
                                .Select(p => p.UserPositions.Take(-2))
                                .SingleOrDefault();
                            break;
                        case 2:
                            lastTwoDaysPositions = (from t in collection.AsQueryable()
                                                        where t.UserId == userID
                                                        select t.UserPositions.Take(-2))
                                                       .SingleOrDefault();
                            break;
                        case 3:
                            lastTwoDaysPositions = collection.AsQueryable()
                                .SingleOrDefault(p => p.UserId == userID)
                                .UserPositions
                                .TakeLast(2);
                            break;
                        case 4:
                            var playerDoc = collection.AsQueryable()
                                .SingleOrDefault(p => p.UserId == userID);

                            lastTwoDaysPositions = playerDoc.UserPositions
                                .TakeLast(2);
                            break;
                    }

                    sw.Stop();

                    // Create a silly operation to perform to avoid any optimization issue
                    resultString = "";
                    foreach (var position in lastTwoDaysPositions)
                    {
                        resultString += position.Position + ";";
                    }
                }

                sw.Stop();
                Console.WriteLine("Elapsed Time = {0}ms", sw.Elapsed.TotalMilliseconds);
                average += sw.Elapsed.TotalMilliseconds;
            }

            Console.WriteLine("Solution {0}, Debug String = {1}", SolutionToApply, resultString);
            Console.WriteLine("Average = {0}", average/100);
        }
    }
}
