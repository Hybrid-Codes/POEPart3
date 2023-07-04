using System.Collections.Generic;

namespace POEPart3
{
    public class Recipe
    {
        public string Name { get; set; }
        public List<Ingredient> Ingredients { get; set; }

        public Recipe()
        {
            Ingredients = new List<Ingredient>();
        }
    }

    public class Ingredient
    {
        public string Name { get; set; }
        public string Measurement { get; set; }
        public int Quantity { get; set; }
        public int Calories { get; set; }
        public string FoodGroup { get; set; }
        
    }
}