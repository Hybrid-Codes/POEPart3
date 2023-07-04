using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace POEPart3
{
    public partial class MainWindow : Window
    {
        private List<Recipe> recipes;
        private Recipe selectedRecipe;
        private List<Recipe> originalRecipes;

        public MainWindow()
        {
            InitializeComponent();
            recipes = new List<Recipe>();
            originalRecipes = new List<Recipe>();
            lvRecipes.ItemsSource = recipes;
            lvIngredients.ItemsSource = null;
            lbSteps.ItemsSource = null;
        }

        private void btnAddRecipe_Click(object sender, RoutedEventArgs e)
        {
            string recipeName = txtRecipeName.Text.Trim();
            if (string.IsNullOrEmpty(recipeName))
            {
                ShowErrorMessage("Please enter a recipe name.");
                return;
            }

            Recipe recipe = new Recipe { Name = recipeName };
            recipes.Add(recipe);
            RefreshRecipeList();
            ClearInputFields();
            SelectRecipe(recipe);
        }

        private void btnScale_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtScale.Text.Trim(), out double scale))
            {
                lvIngredients.Items.Refresh();
            }
            else
            {
                ShowErrorMessage("Invalid scale value. Please enter a valid number.");
            }
        }

        private void btnAddIngredient_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRecipe == null)
            {
                ShowErrorMessage("Please select a recipe.");
                return;
            }

            string ingredientName = txtIngredientName.Text.Trim();
            string unitName = txtMeasurement.Text.Trim();
            int calories, numIngredients;

            if (string.IsNullOrEmpty(ingredientName))
            {
                ShowErrorMessage("Please enter an ingredient name.");
                return;
            }

            if (string.IsNullOrEmpty(unitName))
            {
                ShowErrorMessage("Please enter a unit of measurement.");
                return;
            }

            if (!int.TryParse(txtCalories.Text.Trim(), out calories))
            {
                ShowErrorMessage("Please enter a valid calorie value.");
                return;
            }

            if (!int.TryParse(txtIngredientQuantity.Text.Trim(), out numIngredients))
            {
                ShowErrorMessage("Please enter a valid value.");
                return;
            }

            if (string.IsNullOrEmpty(txtFoodGroup.Text.Trim()))
            {
                ShowErrorMessage("Please select a food group.");
                return;
            }

            Ingredient ingredient = new Ingredient
            {
                Name = ingredientName,
                Quantity = numIngredients,
                Measurement = unitName,
                Calories = calories,
                FoodGroup = txtFoodGroup.Text.Trim()
            };

            selectedRecipe.Ingredients.Add(ingredient);
            RefreshIngredientList();
            ClearInputFields();
            CalculateTotalCalories();
        }

        private void btnAddStep_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRecipe == null)
            {
                ShowErrorMessage("Please select a recipe.");
                return;
            }

            string stepText = txtStep.Text.Trim();
            if (string.IsNullOrEmpty(stepText))
            {
                ShowErrorMessage("Please enter a recipe step.");
                return;
            }

            RecipeStep step = new RecipeStep { Step = stepText };
            selectedRecipe.Steps.Add(step);
            RefreshStepList();
            ClearInputFields();
        }

        private void lvRecipes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedRecipe = (Recipe)lvRecipes.SelectedItem;
            txtSelectedRecipe.Text = selectedRecipe != null ? selectedRecipe.Name : string.Empty;

            if (selectedRecipe != null)
            {
                lvIngredients.ItemsSource = selectedRecipe.Ingredients;
                CalculateTotalCalories();
                lbSteps.ItemsSource = selectedRecipe.Steps;
            }
            else
            {
                ClearIngredientList();
                ClearTotalCalories();
                ClearStepList();
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ingredientNameFilter = txtFilterIngredientName.Text.Trim();
                string foodGroupFilter = txtFilterFoodGroup.Text.Trim();
                int maxCaloriesFilter;

                if (!int.TryParse(txtFilterMaxCalories.Text.Trim(), out maxCaloriesFilter))
                {
                    maxCaloriesFilter = 0;
                }

                var filteredRecipes = originalRecipes.Where(r =>
                    r.Name.ToLower().Contains(ingredientNameFilter.ToLower()) &&
                    r.Ingredients.Any(i => (i.FoodGroup ?? "").ToLower().Contains(foodGroupFilter.ToLower())) &&
                    (maxCaloriesFilter <= 0 || r.Ingredients.Sum(i => i.Calories) <= maxCaloriesFilter)).ToList();

                lvRecipes.ItemsSource = filteredRecipes;
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"An error occurred while filtering recipes: {ex.Message}");
            }
        }

        private void CalculateTotalCalories()
        {
            int totalCalories = selectedRecipe.Ingredients.Sum(i => i.Calories);
            txtTotalCalories.Text = totalCalories.ToString();

            if (totalCalories > 300)
            {
                MessageBox.Show("Warning: Total calories exceed 300.", "Calorie Limit Exceeded", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnClearData_Click(object sender, RoutedEventArgs e)
        {
            ClearRecipeList();
            ClearInputFields();
            ClearIngredientList();
            ClearTotalCalories();
            ClearStepList();
            ClearFilterFields();
        }

        private void RefreshRecipeList()
        {
            recipes = recipes.OrderBy(r => r.Name).ToList();
            lvRecipes.ItemsSource = recipes;
            lvRecipes.Items.Refresh();
        }

        private void RefreshIngredientList()
        {
            lvIngredients.Items.Refresh();
        }

        private void RefreshStepList()
        {
            lbSteps.Items.Refresh();
        }

        private void ClearRecipeList()
        {
            recipes.Clear();
            RefreshRecipeList();
        }

        private void ClearInputFields()
        {
            txtRecipeName.Text = string.Empty;
            txtIngredientName.Text = string.Empty;
            txtIngredientQuantity.Text = string.Empty;
            txtMeasurement.Text = string.Empty;
            txtCalories.Text = string.Empty;
            txtFoodGroup.Text = string.Empty;
            txtScale.Text = string.Empty;
            txtStep.Text = string.Empty;
        }

        private void ClearIngredientList()
        {
            lvIngredients.ItemsSource = null;
            lvIngredients.Items.Refresh();
        }

        private void ClearTotalCalories()
        {
            txtTotalCalories.Text = string.Empty;
        }

        private void ClearStepList()
        {
            lbSteps.ItemsSource = null;
            lbSteps.Items.Clear();
        }

        private void ClearFilterFields()
        {
            txtFilterIngredientName.Text = string.Empty;
            txtFilterFoodGroup.Text = string.Empty;
            txtFilterMaxCalories.Text = string.Empty;
        }

        private void SelectRecipe(Recipe recipe)
        {
            selectedRecipe = recipe;
            txtSelectedRecipe.Text = selectedRecipe.Name;
            lvIngredients.ItemsSource = selectedRecipe.Ingredients;
            CalculateTotalCalories();
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
