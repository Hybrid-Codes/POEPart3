using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace POEPart3
{
    // Represents a step in a recipe
    public class RecipeStep
    {
        public string Step { get; set; } // The text of the recipe step
        public bool IsCompleted { get; set; } // Indicates whether the step is completed
    }
    
    // Represents a recipe
    public class Recipe : INotifyPropertyChanged
    {
        // Properties
        public string Name { get; set; } // The name of the recipe
        public List<Ingredient> Ingredients { get; set; } // The list of ingredients required for the recipe
        
        private List<RecipeStep> steps; // The list of steps to prepare the recipe
        public List<RecipeStep> Steps 
        {
            get { return steps; }
            set
            {
                steps = value;
                OnPropertyChanged("Steps");
            }
        }

        // Constructor
        public Recipe()
        {
            Ingredients = new List<Ingredient>();
            Steps = new List<RecipeStep>();
        }

        // INotifyPropertyChanged event
        public event PropertyChangedEventHandler? PropertyChanged;

        // Property change event handler
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Generic field setter method
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    // Represents an ingredient
    public class Ingredient
    {
        public string Name { get; set; } // The name of the ingredient
        public string Measurement { get; set; } // The measurement unit for the ingredient
        public int Quantity { get; set; } // The quantity of the ingredient required
        public int Calories { get; set; } // The calorie content of the ingredient
        public string FoodGroup { get; set; } // The food group to which the ingredient belongs
    }
}
