using MongoDB.Driver;
using MongoDB.Bson;

namespace CatClub
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://kaami:F6YS1WxP5wCm4X3c@test.hzlhdoo.mongodb.net/test");

            var database = dbClient.GetDatabase("CatClub");
            var collection = database.GetCollection<BsonDocument>("Cats");

            #region Populate with Cats
            List<BsonDocument> documents = new List<BsonDocument>{
                new BsonDocument{
                { "cat_id", 1 },
                { "name", "Ginger"},
                { "weight", 4.8},
                {"age", 2 }
                },  new BsonDocument{
                { "cat_id", 2 },
                { "name", "Max"},
                { "weight", 3.6},
                {"age", 3 }
                },  new BsonDocument{
                { "cat_id", 3 },
                { "name", "Miku"},
                { "weight", 5.75},
                {"age", 10 }
                },
                new BsonDocument{
                { "cat_id", 4 },
                { "name", "Joker"},
                { "weight", 3.8},
                {"age", 5 }
                },
                new BsonDocument{
                { "cat_id", 5 },
                { "name", "Luna"},
                { "weight", 5.8},
                {"age", 15 }
                },
                new BsonDocument{
                { "cat_id", 6 },
                { "name", "Fren"},
                { "weight", 4.82},
                {"age", 7 }
                }
            };

            //collection.InsertMany(documents);
            #endregion

            #region Read Cats
            Console.WriteLine("First Cat");
            var firstDocument = collection.Find(new BsonDocument()).FirstOrDefault();

            Console.WriteLine(firstDocument.ToString());


            Console.WriteLine("Cat with id of 2");
            var filter = Builders<BsonDocument>.Filter.Eq("cat_id", 2);
            var catDocument = collection.Find(filter).FirstOrDefault();
            Console.WriteLine(catDocument.ToString());


            Console.WriteLine("All cats");
            var allCats = collection.Find(new BsonDocument()).ToList();
            foreach (var cat in allCats)
            {
                Console.WriteLine(cat.ToString());
            }

            Console.WriteLine("Fat cat");

            var fattestCatFilter = Builders<BsonDocument>.Filter.Eq(
                 "weight", new BsonDocument { { "$gte", 4 } });

            var sort = Builders<BsonDocument>.Sort.Descending("weight");

            var fattestCat = collection.Find(fattestCatFilter).Sort(sort).First();

            Console.WriteLine(fattestCat);
            #endregion

            #region Update Cats
            Console.WriteLine("Update cat with id of 2");
            var updateFilter = Builders<BsonDocument>.Filter.Eq("cat_id", 2);
            var update = Builders<BsonDocument>.Update.Set("name", "Maxilluim");
            collection.UpdateOne(updateFilter, update);
            catDocument = collection.Find(filter).FirstOrDefault();
            Console.WriteLine(catDocument.ToString());


            #endregion


            #region Delete Cats


            #endregion

        }





















    }
}