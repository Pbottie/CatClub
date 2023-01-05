using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace CatClub
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var dbClient = new MongoClient("mongodb+srv://kaami:F6YS1WxP5wCm4X3c@test.hzlhdoo.mongodb.net/test");
            var database = dbClient.GetDatabase("CatClub");
            var collection = database.GetCollection<BsonDocument>("Cats");


            int mainMenuChoice = 0;

            do
            {
                mainMenuChoice = MainMenu();


                switch (mainMenuChoice)
                {
                    case 1:
                        SelectCat(collection);
                        break;
                    case 2:
                        AddCat(collection);
                        break;
                    case 3:
                        ListCats(collection);
                        break;
                    case 4:
                        RemoveCat(collection);
                        break;
                    case 5:
                        UpdateCat(collection);
                        break;
                    case 6:
                        Console.WriteLine("Exiting...");
                        break;
                    default:
                        Console.WriteLine("Invalid choice!");
                        break;
                }

            }
            while (mainMenuChoice != 6);



        }

        static int MainMenu()
        {
            Console.WriteLine("~~~~~~~~~~CatClub~~~~~~~~~~");
            Console.WriteLine("1. Select Cat");
            Console.WriteLine("2. Add Cat");
            Console.WriteLine("3. Show all Cats");
            Console.WriteLine("4. Remove Cat");
            Console.WriteLine("5. Update Cat");
            Console.WriteLine("6. Exit");

            int mainMenuOptions = 6;
            int input = GetValidInput(mainMenuOptions);

            return input;


        }

        static void SelectCat(IMongoCollection<BsonDocument> collection)
        {
            Console.WriteLine();
            Console.WriteLine("Choose Cat ID");

            int catID;
            FilterDefinition<BsonDocument> filter;

            if (Int32.TryParse(Console.ReadLine(), out catID))
            {
                filter = Builders<BsonDocument>.Filter.Eq("cat_id", catID);
                var catDocument = collection.Find(filter).FirstOrDefault();

                if (catDocument == null)
                    Console.WriteLine("No cat with that Id was found.");
                else
                {

                    var cat = BsonSerializer.Deserialize<Cat>(catDocument);
                    Console.WriteLine(cat);
                }

            }
            else
                Console.WriteLine("Invalid integer");




        }

        static void AddCat(IMongoCollection<BsonDocument> collection)
        {
            Console.WriteLine("Input Cat Data: ");
            Cat cat;

            try
            {
                Console.Write("Id(int): ");
                int id = Int32.Parse(Console.ReadLine());

                Console.Write("Name(string): ");
                string name = Console.ReadLine();

                Console.Write("Weight(double): ");
                double weight = Double.Parse(Console.ReadLine());

                Console.Write("Age(int): ");
                int age = Int32.Parse(Console.ReadLine());

                Console.Write("colors(string) separate with ,: ");
                string colors = Console.ReadLine();

                Console.Write("Gender(string): ");
                string gender = Console.ReadLine();

                cat = new Cat(id, name, weight, age, colors, gender);


            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                throw;
            }

            if (cat != null)
                collection.InsertOne(cat.ToBsonDocument());



        }

        static void ListCats(IMongoCollection<BsonDocument> collection)
        {
            var allCats = collection.Find(new BsonDocument()).ToList();

            foreach (var catson in allCats)
            {
                var cat = BsonSerializer.Deserialize<Cat>(catson);
                Console.WriteLine(cat);
                Console.WriteLine();
            }
        }

        static void RemoveCat(IMongoCollection<BsonDocument> collection)
        {
            Console.WriteLine();
            Console.WriteLine("Choose Cat ID to delete");

            int catID;
            FilterDefinition<BsonDocument> deleteFilter;

            if (Int32.TryParse(Console.ReadLine(), out catID))
            {
                deleteFilter = Builders<BsonDocument>.Filter.Eq("cat_id", catID);
                var catDocument = collection.Find(deleteFilter).FirstOrDefault();

                if (catDocument == null)
                    Console.WriteLine("No cat with that Id was found.");
                else
                {
                    var cat = BsonSerializer.Deserialize<Cat>(catDocument);
                    collection.DeleteOne(deleteFilter);
                    Console.WriteLine(cat + "\nWas deleted");
                }

            }
            else
                Console.WriteLine("Invalid integer");

        }

        static void UpdateCat(IMongoCollection<BsonDocument> collection)
        {
            Console.WriteLine();
            Console.WriteLine("Choose Cat ID");

            int catID;
            FilterDefinition<BsonDocument> updateFilter;

            if (Int32.TryParse(Console.ReadLine(), out catID))
            {
                updateFilter = Builders<BsonDocument>.Filter.Eq("cat_id", catID);
                var catDocument = collection.Find(updateFilter).FirstOrDefault();
                var fields = catDocument.Count();

                if (catDocument == null)
                    Console.WriteLine("No cat with that Id was found.");
                else
                {
                    Console.WriteLine("What would you like to update?");


                    var enumTest = catDocument.Names;
                    for (int i = 1; i < fields; i++)
                    {
                        Console.WriteLine($"{i}. {catDocument.GetElement(i)}");
                    }

                    int input = GetValidInput(fields - 1);
                    var updateName = catDocument.GetElement(input).Name;

                    Console.Write($"Update {updateName}: ");
                    if (catDocument.GetElement(input).Value.IsBsonArray)
                    {
                        Console.Write("(use , to seperate colors) ");
                        BsonArray updateArray = new BsonArray();
                        string[] strArr = Console.ReadLine().Split(',');
                        foreach (var color in strArr)
                        {
                            updateArray.Add(color);
                        }
                        var update = Builders<BsonDocument>.Update.Set(updateName, updateArray);
                        collection.UpdateOne(updateFilter, update);

                    }
                    else
                    {
                        string updateValue = Console.ReadLine();
                        var update = Builders<BsonDocument>.Update.Set(updateName, updateValue);
                        collection.UpdateOne(updateFilter, update);

                    }

                }

            }
            else
                Console.WriteLine("Invalid integer");



            //Console.WriteLine("Update cat with id of 2");
            //var updateFilter = Builders<BsonDocument>.Filter.Eq("cat_id", 2);


        }



        static int GetValidInput(int menuOptions)
        {
            bool inmatat = false;
            int output = 0;

            while (!inmatat)
            {
                try
                {
                    Console.Write("Make selection: ");
                    inmatat = int.TryParse(Console.ReadLine(), out output);
                    if (!inmatat)
                        Console.WriteLine("Integers only please!");
                    else if (output > menuOptions || output < 1)
                    {
                        Console.WriteLine("Invalid selection");
                        inmatat = false;
                    }

                }
                catch (Exception ex)
                {

                    Console.WriteLine("Readline failed with message: " + ex.Message);
                }

            }
            return output;
        }

        static bool GetValidBool()
        {
            char answer = 'c';

            while (answer != 'y' && answer != 'n')
            {
                Console.WriteLine("Are you sure? (y/n): ");
                try
                {

                    answer = (char)Console.Read();
                    Console.ReadLine();//Gets rid of pesky "". Don't want to do Console.ReadLine().Trim()[0]

                }
                catch (Exception ex)
                {

                    Console.WriteLine("Read failed with message: " + ex.Message);
                }
            }

            if (answer == 'y')
                return true;
            return false;


        }



    }
}
