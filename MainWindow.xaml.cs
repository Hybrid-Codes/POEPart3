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
        // Lists to store recipes and their original copies
        private List<Recipe> recipes;
        private List<Recipe> originalRecipes;

        // Currently selected recipe
        private Recipe selectedRecipe;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize recipe lists
            recipes = new List<Recipe>();
            originalRecipes = new List<Recipe>();

            // Set recipe list as the data source for the list view
            lvRecipes.ItemsSource = recipes;

            // Set ingredient and step list view data sources to null initially
            lvIngredients.ItemsSource = null;
            lbSteps.ItemsSource = null;
        }

        // Event handler for the "Add Recipe" button
        private void btnAddRecipe_Click(object sender, RoutedEventArgs e)
        {
            // Get the recipe name from the input textbox
            string recipeName = txtRecipeName.Text.Trim();

            // Check if the recipe name is empty
            if (string.IsNullOrEmpty(recipeName))
            {
                ShowErrorMessage("Please enter a recipe name.");
                return;
            }

            // Create a new Recipe object with the entered name
            Recipe recipe = new Recipe { Name = recipeName };

            // Add the recipe to the recipes list
            recipes.Add(recipe);

            // Refresh the recipe list view
            RefreshRecipeList();

            // Clear input fields
            ClearInputFields();

            // Select the newly added recipe
            SelectRecipe(recipe);
        }

        // Event handler for the "Scale" button
        private void btnScale_Click(object sender, RoutedEventArgs e)
        {
            // Check if the entered scale value is a valid number
            if (double.TryParse(txtScale.Text.Trim(), out double scale))
            {
                // Refresh the ingredient list view to apply the scaling
                lvIngredients.Items.Refresh();
            }
            else
            {
                ShowErrorMessage("Invalid scale value. Please enter a valid number.");
            }
        }

        // Event handler for the "Add Ingredient" button
        private void btnAddIngredient_Click(object sender, RoutedEventArgs e)
        {
            // Check if a recipe is selected
            if (selectedRecipe == null)
            {
                ShowErrorMessage("Please select a recipe.");
                return;
            }

            // Get the ingredient details from the input fields
            string ingredientName = txtIngredientName.Text.Trim();
            string unitName = txtMeasurement.Text.Trim();
            int calories, numIngredients;

            // Validate the ingredient name
            if (string.IsNullOrEmpty(ingredientName))
            {
                ShowErrorMessage("Please enter an ingredient name.");
                return;
            }

            // Validate the unit of measurement
            if (string.IsNullOrEmpty(unitName))
            {
                ShowErrorMessage("Please enter a unit of measurement.");
                return;
            }

            // Parse the calorie value
            if (!int.TryParse(txtCalories.Text.Trim(), out calories))
            {
                ShowErrorMessage("Please enter a valid calorie value.");
                return;
            }

            // Parse the ingredient quantity
            if (!int.TryParse(txtIngredientQuantity.Text.Trim(), out numIngredients))
            {
                ShowErrorMessage("Please enter a valid value.");
                return;
            }

            // Check if a food group is selected
            if (string.IsNullOrEmpty(txtFoodGroup.Text.Trim()))
            {
                ShowErrorMessage("Please select a food group.");
                return;
            }

            // Create a new Ingredient object with the entered details
            Ingredient ingredient = new Ingredient
            {
                Name = ingredientName,
                Quantity = numIngredients,
                Measurement = unitName,
                Calories = calories,
                FoodGroup = txtFoodGroup.Text.Trim()
            };

            // Add the ingredient to the selected recipe's ingredient list
            selectedRecipe.Ingredients.Add(ingredient);

            // Refresh the ingredient list view
            RefreshIngredientList();

            // Clear input fields
            ClearInputFields();

            // Recalculate the total calories for the selected recipe
            CalculateTotalCalories();
        }

        // Event handler for the "Add Step" button
        private void btnAddStep_Click(object sender, RoutedEventArgs e)
        {
            // Check if a recipe is selected
            if (selectedRecipe == null)
            {
                ShowErrorMessage("Please select a recipe.");
                return;
            }

            // Get the step text from the input textbox
            string stepText = txtStep.Text.Trim();

            // Validate the step text
            if (string.IsNullOrEmpty(stepText))
            {
                ShowErrorMessage("Please enter a recipe step.");
                return;
            }

            // Create a new RecipeStep object with the entered step
            RecipeStep step = new RecipeStep { Step = stepText };

            // Add the step to the selected recipe's step list
            selectedRecipe.Steps.Add(step);

            // Refresh the step list view
            RefreshStepList();

            // Clear input fields
            ClearInputFields();
        }

        // Event handler for the recipe selection change in the list view
        private void lvRecipes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected recipe from the list view
            selectedRecipe = (Recipe)lvRecipes.SelectedItem;

            // Update the selected recipe text box
            txtSelectedRecipe.Text = selectedRecipe != null ? selectedRecipe.Name : string.Empty;

            if (selectedRecipe != null)
            {
                // Set the selected recipe's ingredients as the data source for the ingredient list view
                lvIngredients.ItemsSource = selectedRecipe.Ingredients;

                // Calculate and display the total calories for the selected recipe
                CalculateTotalCalories();

                // Set the selected recipe's steps as the data source for the step list view
                lbSteps.ItemsSource = selectedRecipe.Steps;
            }
            else
            {
                // Clear ingredient and step lists if no recipe is selected
                ClearIngredientList();
                ClearTotalCalories();
                ClearStepList();
            }
        }

        // Event handler for the "Filter" button
        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the filter values from the input fields
                string ingredientNameFilter = txtFilterIngredientName.Text.Trim();
                string foodGroupFilter = txtFilterFoodGroup.Text.Trim();
                int maxCaloriesFilter;

                // Parse the maximum calories filter value
                if (!int.TryParse(txtFilterMaxCalories.Text.Trim(), out maxCaloriesFilter))
                {
                    maxCaloriesFilter = 0;
                }

                // Apply the filters to the original recipe list
                var filteredRecipes = originalRecipes.Where(r =>
                    r.Name.ToLower().Contains(ingredientNameFilter.ToLower()) &&
                    r.Ingredients.Any(i => (i.FoodGroup ?? "").ToLower().Contains(foodGroupFilter.ToLower())) &&
                    (maxCaloriesFilter <= 0 || r.Ingredients.Sum(i => i.Calories) <= maxCaloriesFilter)).ToList();

                // Update the recipe list view with the filtered recipes
                lvRecipes.ItemsSource = filteredRecipes;
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"An error occurred while filtering recipes: {ex.Message}");
            }
        }

        // Calculate and display the total calories for the selected recipe
        private void CalculateTotalCalories()
        {
            int totalCalories = selectedRecipe.Ingredients.Sum(i => i.Calories);
            txtTotalCalories.Text = totalCalories.ToString();

            // Show a warning if the total calories exceed 300
            if (totalCalories > 300)
            {
                MessageBox.Show("Warning: Total calories exceed 300.", "Calorie Limit Exceeded", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Event handler for the "Clear Data" button
        private void btnClearData_Click(object sender, RoutedEventArgs e)
        {
            // Clear all data fields and lists
            ClearRecipeList();
            ClearInputFields();
            ClearIngredientList();
            ClearTotalCalories();
            ClearStepList();
            ClearFilterFields();
        }

        // Refresh the recipe list view
        private void RefreshRecipeList()
        {
            // Order the recipes alphabetically and update the list view
            recipes = recipes.OrderBy(r => r.Name).ToList();
            lvRecipes.ItemsSource = recipes;
            lvRecipes.Items.Refresh();
        }

        // Refresh the ingredient list view
        private void RefreshIngredientList()
        {
            lvIngredients.Items.Refresh();
        }

        // Refresh the step list view
        private void RefreshStepList()
        {
            lbSteps.Items.Refresh();
        }

        // Clear the recipe list
        private void ClearRecipeList()
        {
            recipes.Clear();
            RefreshRecipeList();
        }

        // Clear all input fields
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

        // Clear the ingredient list view
        private void ClearIngredientList()
        {
            lvIngredients.ItemsSource = null;
            lvIngredients.Items.Refresh();
        }

        // Clear the total calories display
        private void ClearTotalCalories()
        {
            txtTotalCalories.Text = string.Empty;
        }

        // Clear the step list view
        private void ClearStepList()
        {
            lbSteps.ItemsSource = null;
            lbSteps.Items.Clear();
        }

        // Clear the filter input fields
        private void ClearFilterFields()
        {
            txtFilterIngredientName.Text = string.Empty;
            txtFilterFoodGroup.Text = string.Empty;
            txtFilterMaxCalories.Text = string.Empty;
        }

        // Select a recipe and update the UI
        private void SelectRecipe(Recipe recipe)
        {
            selectedRecipe = recipe;
            txtSelectedRecipe.Text = selectedRecipe.Name;
            lvIngredients.ItemsSource = selectedRecipe.Ingredients;
            CalculateTotalCalories();
        }

        // Show an error message in a message box
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
