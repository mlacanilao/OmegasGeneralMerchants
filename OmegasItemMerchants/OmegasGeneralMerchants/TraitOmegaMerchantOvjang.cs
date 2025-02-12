using System.Collections.Generic;
using UnityEngine;

class TraitOmegaMerchantOvjang : TraitMerchant
{
    public override ShopType ShopType
    {
        get
        {
            return ShopType.Specific;
        }
    }
    
    public override bool CanGuide
    {
        get
        {
            return true;
        }
    }
    
    public override bool CanServeFood
    {
        get
        {
            return true;
        }
    }
    
    void _OnBarter()
    {
        var inventory = this.owner?.things?.Find(id: "chest_merchant");
        if (inventory is null) 
        {
            inventory = ThingGen.Create(id: "chest_merchant");
            this.owner?.AddThing(t: inventory);
        }
        
        int shopLv = this.ShopLv;
        int depthLv = EClass.player.stats.deepest;
        int genLv = Mathf.Max(a: shopLv, b: depthLv);
        
        // Retrieve all food items from the game
        var allMeals = SpawnList.Get(id: "shop_food").rows;

        foreach (var mealRow in allMeals)
        {
            if (mealRow.tag.Contains(id: "dish_fail"))
            {
                continue;
            }
            
            // Generate the base meal item
            Thing meal = ThingGen.Create(id: mealRow.id, idMat: -1, lv: genLv);
        
            if (meal != null)
            {
                Trait trait = meal.trait;
                if (trait is TraitFoodMeal == false)
                {
                    continue;
                }
                
                MakeDish(food: meal, lv: genLv);
                
                meal.elements?.SetBase(id: 757, v: 1, potential: 0);
                meal.c_dateCooked = EClass.world.date.GetRaw(offsetHours: 0) + 1 * 48 * 60;
                meal.SetNum(a: meal.trait.DefaultStock);
            
                // Add meal to inventory
                inventory?.AddThing(t: meal);
            }
        }
    }
    
    private static void MakeDish(Thing food, int lv, Chara crafter = null)
    {
        RecipeManager.BuildList();
        List<Thing> list = new List<Thing>();
        RecipeSource recipeSource = RecipeManager.Get(id: food.id);
        if (recipeSource == null)
        {
            return;
        }
        int num = lv;
        foreach (Recipe.Ingredient ingredient in recipeSource.GetIngredients())
        {
            Thing thing = ThingGen.Create(id: ingredient.id, idMat: -1, lv: -1);
            TraitOmegaMerchantOvjang.LevelSeed(t: thing, obj: null, num: (lv / 4) + 1);
            thing.SetEncLv(a: thing.encLV / 2);
            if (num > 0)
            {
                thing.elements.SetBase(id: 2, v: num, potential: 0);
            }
            list.Add(item: thing);
        }
        CraftUtil.MakeDish(food: food, ings: list, qualityBonus: num, crafter: crafter);
    }
    
    private static void LevelSeed(Thing t, SourceObj.Row obj, int num)
    {
        for (int i = 0; i < num; i++)
        {
            if (obj == null || obj.objType == "crop")
            {
                if (t.encLV == 0)
                {
                    CraftUtil.AddRandomFoodEnc(t: t);
                }
                else
                {
                    Rand.SetSeed(a: t.c_seed);
                    TraitOmegaMerchantOvjang.ModRandomFoodEnc(t: t);
                }
            }
            t.ModEncLv(a: 1);
        }
    }
    
    private static void ModRandomFoodEnc(Thing t)
    {
        List<Element> list = new List<Element>();
        foreach (Element element in t.elements.dict.Values)
        {
            if (element.IsFoodTrait)
            {
                list.Add(item: element);
            }
        }
        if (list.Count == 0)
        {
            return;
        }
        Element element2 = list.RandomItem<Element>();
        t.elements.ModBase(ele: element2.id, v: 6);
    }
}