using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;


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
            Console.WriteLine("Choose Cat ID");

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
            Console.WriteLine("Choose Cat ID to delete");

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
            Console.WriteLine("Choose Cat ID");

            int catID;

            if (Int32.TryParse(Console.ReadLine(), out catID))
            {
                Console.Write("Field name: ");
                var fieldName = Console.ReadLine();
                Console.Write("Field value: ");
                var fieldValue = Console.ReadLine();

                var cat = cats.UpdateCat(catID, fieldName, fieldValue).Result;

                if (cat)
                    Console.WriteLine($"Cat with ID: {catID} was updated");
                else
                {
                    Console.WriteLine("No cat with that Id was found.");


                    //var enumTest = catDocument.Names;
                    //for (int i = 1; i < fields; i++)
                    //{
                    //    Console.WriteLine($"{i}. {catDocument.GetElement(i)}");
                    //}

                    //int input = GetValidInput(fields - 1);
                    //var updateName = catDocument.GetElement(input).Name;

                    //Console.Write($"Update {updateName}: ");
                    //if (catDocument.GetElement(input).Value.IsBsonArray)
                    //{
                    //    Console.Write("(use , to seperate colors) ");
                    //    BsonArray updateArray = new BsonArray();
                    //    string[] strArr = Console.ReadLine().Split(',');
                    //    foreach (var color in strArr)
                    //    {
                    //        updateArray.Add(color);
                    //    }
                    //    var update = Builders<BsonDocument>.Update.Set(updateName, updateArray);
                    //    collection.UpdateOne(updateFilter, update);

                    //}
                    //else
                    //{
                    //    string updateValue = Console.ReadLine();
                    //    var update = Builders<BsonDocument>.Update.Set(updateName, updateValue);
                    //    collection.UpdateOne(updateFilter, update);

                    //}

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

        public async Task<bool> UpdateCat(int id, string updateFieldName, string updateFieldValue)
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
