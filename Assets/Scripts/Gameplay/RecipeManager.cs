using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager 
{   
    private static Dictionary<string, string> recipes;
    private static float multiplier = 1.5f;

    static RecipeManager() {
        recipes = new Dictionary<string, string>();
        CreateRecipes();
    }

    public static ItemStack Lookup(List<ItemStack> recipe)
    {
        if(recipe.Count < 1) return new ItemStack();
        // String of ingredients alphabetical
        string ingredients = ItemsToString(recipe);
        // Debug.Log(ingredients);

        // Checks if ingredients can make bakery item
        string food = FindRecipe(ingredients);
        // Debug.Log(food);
        if (food.Equals("-1"))
        {
            UnityEngine.Debug.Log("Invalid");
            return new ItemStack();
        }

        // Returns bakery itema
        float price = EvaluatePrice(recipe);
        return (new ItemStack(food, food, price, 1));
    }

    // Given a list of foods, it returns foods that can be added to make recipes
    public static List<ItemStack> CompatibleFoods (List<ItemStack> ingredients)
    {
        string startRecipe = ItemsToString(ingredients);
        int length = startRecipe.Length;
        List<string> compatible = new List<string>();
        List<ItemStack> output = new List<ItemStack>();

        foreach(string foods in recipes.Keys)
        {
            if (startRecipe.Equals(foods.Substring(0, length)))
            {
                string food = "";
                for (int i = length; i < foods.Length; i++)
                {
                    if (!foods.Substring(i, i+1).Equals(" "))
                    {
                        food += foods.Substring(i, i+1);
                    }
                    else
                    {
                        if (!food.Equals("") && !compatible.Contains(food))
                        {
                            compatible.Add(food);
                        }
                        food = "";
                    }
                }
                if (!food.Equals("") && !compatible.Contains(food))
                {
                    compatible.Add(food);
                }
            }
        }
        
        GameObject reference;
        float price;
        foreach(string food in compatible)
        {
            UnityEngine.Debug.Log(food);
            reference = Resources.Load<GameObject>("Prefabs/Items/" + food);
            price = reference.GetComponent<Item>().GetItemStack().marketValue;
            output.Add(new ItemStack(food, food, price, 1));
        }
        return output;
    }

    // Returns string of items in list
    private static string ItemsToString(List<ItemStack> items) 
    {
        string ingredients = "";
        List<string> itemsString = new List<string>();
        foreach(ItemStack item in items) {
            itemsString.Add(item.itemID);
        }

        itemsString.Sort();
        foreach(string item in itemsString)
        {
            ingredients += item + " ";
        }
        ingredients = ingredients.Substring(0, ingredients.Length-1);
        return ingredients;
    }
    // Returns price of ingredients
    private static float EvaluatePrice(List<ItemStack> recipe) 
    {
        float sum = 0f;
        foreach(ItemStack item in recipe) 
        {
            sum += item.marketValue;
        }

        return (sum * multiplier);
    }

    // Find recipe with ingredients
    private static string FindRecipe(string ingredients) 
    {
        if (recipes.ContainsKey(ingredients))
        {
            return recipes[ingredients];
        }
        else
        {
            return "-1";
        }
    }

    // Yandere dev
    private static void CreateRecipes()
    {
        recipes.Add("Blueberry Wheat", "Blueberry_Tart");
        recipes.Add("Banana Wheat", "Banana_Pie");
        recipes.Add("Sugar Wheat", "Shortbread");
        recipes.Add("Carrot Wheat", "Carrot_Cake");
        recipes.Add("Rhubarb Wheat", "Rhubarb_Pie");
        recipes.Add("Egg Wheat", "Bread");
        recipes.Add("Banana Blueberry", "Fruit_Salad");
        recipes.Add("Carrot Rhubarb", "Smoothie");
        recipes.Add("Blueberry Egg Sugar Wheat", "Blueberry_Cupcake");
        recipes.Add("Banana Egg Sugar Wheat", "Banana_Crumble"); 
        recipes.Add("Egg Rhubarb Sugar Wheat", "Sweet_Rhubarb_Pie"); 
        recipes.Add("Carrot Egg Sugar Wheat", "Carrot_Souffle");
        recipes.Add("Egg Sugar Wheat", "Cake"); 
    }
}
