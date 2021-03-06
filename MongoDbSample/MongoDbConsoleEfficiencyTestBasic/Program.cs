﻿using System;
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

            // Random positions generator
            Random rand = new Random(DateTime.Now.Second);

            Stopwatch sw = new Stopwatch();

            // Increase the day number to simulate the daily update for many users
            // to check how the record addition slows the db when the records number grows
            for (int day = 0; day < 100; day++)
            {
                sw.Reset();
                sw.Start();

                // Loop for 16*12 users data update
                for (int userID = 0; userID < 16 * 12; userID++)
                {
                    // Get the user document
                    var user = collection.AsQueryable()
                        .SingleOrDefault(p => p.UserId == userID);

                    // If the user document does not exist, create a new one
                    bool newUser = false;
                    if (user == null)
                    {
                        user = new User
                        {
                            UserId = userID,
                            UserPositions = new List<PositionItem>()
                        };
                        newUser = true;
                    }

                    // Add the new record to the document
                    user.UserPositions.Add(new PositionItem
                    {
                        Position = (41 + rand.NextDouble() * 2).ToString() + "," + (11 + rand.NextDouble() * 2).ToString(),
                        PositionDate = day
                    });

                    // Add/Update the updated document to the collection
                    if (newUser)
                        collection.InsertOne(user);
                    else
                        collection.ReplaceOne(u => u.UserId == userID, user);
                }


                sw.Stop();

                Console.WriteLine("Elapsed Time = {0}ms", sw.Elapsed.TotalMilliseconds);
            }
        }
    }
}
