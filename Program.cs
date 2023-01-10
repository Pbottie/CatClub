using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Reflection;

namespace CatClub
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var catRepository = new CatRepository("mongodb+srv://supersecretuser:supersecretpassword@test.hzlhdoo.mongodb.net/test");


            int mainMenuChoice = 0;

            do
            {
                mainMenuChoice = MainMenu();


                switch (mainMenuChoice)
                {
                    case 1:
                        SelectCat(catRepository);
                        break;
                    case 2:
                        AddCat(catRepository);
                        break;
                    case 3:
                        ListCats(catRepository);
                        break;
                    case 4:
                        RemoveCat(catRepository);
                        break;
                    case 5:
                        UpdateCat(catRepository);
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
            Console.WriteLine();
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







        static async void SelectCat(CatRepository cats)
        {
            Console.WriteLine();
            Console.Write("Choose Cat ID: ");

            int catID;

            if (Int32.TryParse(Console.ReadLine(), out catID))
            {

                var returned = cats.GetCatsByID(catID);
                if (returned.Result.Count == 0)
                    Console.WriteLine("No cat with that Id was found.");
                else
                {
                    foreach (var cat in returned.Result.ToList())
                    {
                        Console.WriteLine(cat);
                    }

                }

            }
            else
                Console.WriteLine("Invalid integer");




        }

        static void AddCat(CatRepository cats)
        {
            Console.WriteLine("Input Cat Data: ");
            Cat cat = null;

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
                Console.WriteLine("Error with last input: " + ex);
            }

            if (cat != null)
                cats.InsertCat(cat);



        }

        static void ListCats(CatRepository cats)
        {
            Console.WriteLine();
            var allCats = cats.GetAllCats().Result.ToList();
            foreach (var cat in allCats)
            {
                Console.WriteLine(cat);
                Console.WriteLine();
            }
        }

        static void RemoveCat(CatRepository cats)
        {
            Console.WriteLine();
            Console.Write("Choose Cat ID to delete: ");

            int catID;

            if (Int32.TryParse(Console.ReadLine(), out catID))
            {

                var cat = cats.DeleteCatById(catID).Result;

                if (cat)
                    Console.WriteLine($"Cat with id: {catID} was deleted");
                else
                {
                    Console.WriteLine("No cat with that Id was found.");
                }

            }
            else
                Console.WriteLine("Invalid integer");

        }

        static void UpdateCat(CatRepository cats)
        {
            Console.WriteLine();
            Console.Write("Choose Cat ID: ");

            int catID;

            if (Int32.TryParse(Console.ReadLine(), out catID))
            {
                var cat = cats.GetCatsByID(catID);


                if (cat.Result.Count == 0)
                {
                    Console.WriteLine("No cat with that Id was found.");
                }
                else
                {
                    var query = cat.Result.FirstOrDefault().GetType().GetFields(BindingFlags.Public |
                                   BindingFlags.NonPublic |
                                   BindingFlags.Instance);

                    Console.WriteLine("Which field would you like to update?");
                    string[] fields = new string[query.Length];

                    for (int i = 1; i < query.Length; i++)
                    {
                        var field = query[i];
                        string name = field.Name.Substring(1, field.Name.IndexOf('>') - 1);
                        fields[i] = name.ToLower();
                        Console.WriteLine(i + ". " + name);
                    }
                    int fieldChoice = GetValidInput(query.Length - 1);
                    var fieldName = fields[fieldChoice];
                    var fieldType = query[fieldChoice].FieldType.Name;


                    if (fieldType != "String[]")
                        Console.Write("Field value: ");
                    else
                        Console.Write("Field value (separate with , ): ");

                    var value = Console.ReadLine();

                    bool updateCat = false;

                    try
                    {
                        switch (fieldType)
                        {
                            case "Int32":
                                var intValue = Int32.Parse(value);
                                updateCat = cats.UpdateCat(catID, fieldName, intValue).Result;
                                break;
                            case "String[]":
                                var stringArrayValue = value.Split(',');
                                updateCat = cats.UpdateCat(catID, fieldName, stringArrayValue).Result;
                                break;
                            case "Double":
                                var fieldValue = Double.Parse(value);
                                updateCat = cats.UpdateCat(catID, fieldName, fieldValue).Result;
                                break;
                            default:
                                updateCat = cats.UpdateCat(catID, fieldName, value).Result;
                                break;
                        }

                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("Error: " + ex);
                    }

                    if (updateCat)
                        Console.WriteLine($"Cat with ID: {catID} was updated");

                }

            }
            else
                Console.WriteLine("Invalid integer");



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

    }


    public class CatRepository
    {

        IMongoClient _client;
        IMongoDatabase _database;
        IMongoCollection<Cat> _catCollection;

        public CatRepository(string connectionString)
        {

            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase("CatClub");
            _catCollection = _database.GetCollection<Cat>("Cats");


        }

        public async Task InsertCat(Cat cat)
        {
            await _catCollection.InsertOneAsync(cat);

        }

        public async Task<List<Cat>> GetAllCats()
        {
            return await _catCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<List<Cat>> GetCatsByID(int fieldValue)
        {
            var filter = Builders<Cat>.Filter.Eq("cat_id", fieldValue);
            var result = await _catCollection.Find(filter).ToListAsync();

            return result;
        }

        public async Task<bool> UpdateCat<T>(int id, string updateFieldName, T updateFieldValue)
        {
            var filter = Builders<Cat>.Filter.Eq("cat_id", id);
            var update = Builders<Cat>.Update.Set(updateFieldName, updateFieldValue);

            var result = await _catCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount != 0;
        }

        public async Task<bool> DeleteCatById(int id)
        {
            var filter = Builders<Cat>.Filter.Eq("cat_id", id);
            var result = await _catCollection.DeleteOneAsync(filter);

            return result.DeletedCount != 0;
        }

    }
}
