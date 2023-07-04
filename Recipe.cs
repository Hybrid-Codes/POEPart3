using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace POEPart3
{
    public class RecipeStep
    {
        public string Step { get; set; }
        public bool IsCompleted { get; set; }
    }
    public class Recipe : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        private List<RecipeStep> steps;
        public List<RecipeStep> Steps 
        {
            get { return steps; }
            set
            {
                steps = value;
                OnPropertyChanged("Steps");
            }
        }

        public Recipe()
        {
            Ingredients = new List<Ingredient>();
            Steps = new List<RecipeStep>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
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