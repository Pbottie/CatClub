using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatClub
{
    public class Cat
    {
       // [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("cat_id")]
        public int CatId { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("weight")]
        public double Weight { get; set; }
        [BsonElement("age")]
        public int Age { get; set; }
        [BsonElement("colors")]
        public string[] Colors { get; set; }
        [BsonElement("gender")]
        public string Gender { get; set; }

        public Cat(int cat_id, string name, double weight, int age, string colors, string gender)
        {
            try
            {

                this.CatId = cat_id;
                this.Name = name;
                this.Weight = weight;
                this.Age = age;

                string[] color = colors.Split(',');
                this.Colors = color;

                this.Gender = gender;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Input error! " + ex);
                throw;
            }
        }

        public override string ToString()
        {
            string allColors = "";

            foreach (var color in Colors)
            {
                allColors += color + " ";
            }

            return $"Cat ID: {CatId} \nName: {Name} \nWeight: {Weight} \nAge: {Age} \nColors: {allColors} \nGender: {Gender}";
        }
    }
}
