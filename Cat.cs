using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatClub
{
    class Cat
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int cat_id { get; set; }
        public string name { get; set; }
        public double weight { get; set; }
        public int age { get; set; }
        public string[] colors { get; set; }
        public string gender { get; set; }

        public Cat(int cat_id, string name, double weight, int age, string colors, string gender)
        {
            try
            {

            this.cat_id = cat_id;
            this.name = name;
            this.weight = weight;
            this.age = age;
            
            string[] color = colors.Split(',');
            this.colors = color;

            this.gender = gender;
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

            foreach (var color in colors)
            {
                allColors += color + " ";
            }

            return $"Cat ID: {cat_id} \nName: {name} \nWeight: {weight} \nAge: {age} \nColors: {allColors} \nGender: {gender}";
        }
    }
}
