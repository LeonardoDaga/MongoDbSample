using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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
            Console.WriteLine("Enter User ID:");
            // Get ID from console
            var userIDStr = Console.ReadLine();

            Console.WriteLine("Enter Day:");
            // Get data from console
            var dataDayStr = Console.ReadLine();

            Console.WriteLine("Enter Position:");
            // Get some input from user
            var dataPosStr = Console.ReadLine();

            if (!int.TryParse(userIDStr, out int userID))
            {
                Console.WriteLine("Error reading user ID");
                return;
            }
            if (!int.TryParse(dataDayStr, out int positionDay))
            {
                Console.WriteLine("Error reading position day");
                return;
            }

            // Retrieve actual 
            var positionItem = new PositionItem()
            {
                PositionDate = positionDay,
                Position = dataPosStr
            };

            // get a mongoclient using the default connection string
            var mongo = new MongoClient();

            // get (and create if doesn't exist) a database from the mongoclient
            var db = mongo.GetDatabase("UserPositionsDb");

            // get a collection of User (and create if it doesn't exist)
            var collection = db.GetCollection<User>("UserCollection");

            var user = collection.AsQueryable()
                .SingleOrDefault(p => p.UserId == userID);

            // If the user does not exist, create a new one
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

            Console.WriteLine($"User before the item addition");
            Console.Write(user);

            user.UserPositions.Add(positionItem);

            Console.WriteLine($"User after the item addition");
            Console.Write(user);

            // Add the entered item to the collection
            if (newUser)
                collection.InsertOne(user);
            else
                collection.ReplaceOne(u => u.UserId == userID, user);

            // Count the items in the collection post insert
            var count = user.UserPositions.Count();
            Console.WriteLine($"Number of items in the collection after insert: {count}");
        }
    }
}
