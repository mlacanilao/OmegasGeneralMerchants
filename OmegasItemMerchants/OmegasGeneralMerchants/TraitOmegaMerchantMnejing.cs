using UnityEngine;

class TraitOmegaMerchantMnejing : TraitMerchant
{
    public override ShopType ShopType
    {
        get
        {
            return ShopType.Specific;
        }
    }
    
    public override bool CanRevive
    {
        get
        {
            return true;
        }
    }
    
    public override bool CanGuide
    {
        get
        {
            return true;
        }
    }
    
    public override bool CanIdentify
    {
        get
        {
            return true;
        }
    }
    
    public override string IDTrainer
    {
        get
        {
            return base.GetParam(i: 1, def: null).IsEmpty(defaultStr: TraitTrainer.ids[EClass.rnd(a: TraitTrainer.ids.Length)]);
        }
    }
    
    public override bool CanSellStolenGoods
    {
        get
        {
            return true;
        }
    }
    
    public override bool CanInvestTown
    {
        get
        {
            return true;
        }
    }
    
    public override bool CanHeal
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
        
        foreach (var item in inventory.things)
        {
            if (item != null)
            {
                Trait trait = item.trait;

                switch (trait)
                {
                    case TraitDrinkMilk traitDrinkMilk:
                    case TraitFoodTravel traitFoodTravel:
                    case TraitGambleChest traitGambleChest:    
                        var num = 2 + EClass.rnd(a: 4);
                        item.SetNum(a: num);
                        break;
                    case TraitBait traitBait:
                    case TraitAmmoArrow traitAmmoArrow:
                    case TraitAmmoBolt traitAmmoBolt:
                    case TraitAmmoBullet traitAmmoBullet:
                    case TraitAmmoEnergy traitAmmoEnergy:
                        var ammoNum = 300 + EClass.rnd(a: 100);
                        item.SetNum(a: ammoNum);
                        break;
                    case TraitMaterialHammer traitMaterialHammer:
                        SourceMaterial.Row randomMaterial = MATERIAL.GetRandomMaterial(lv: ((genLv < 200) ? (genLv / 2) : (genLv % 50 * 2)) + 10, group: (EClass.rnd(a: 2) == 0) ? "metal" : "leather", tryLevelMatTier: true);
                        item.ChangeMaterial(row: randomMaterial, ignoreFixedMaterial: false);
                        if (item.GetValue(priceType: PriceType.Default, sell: false) < 0)
                        {
                            item.c_priceAdd = Mathf.Abs(value: item.GetValue(sell: true)) * 2;
                        }
                        break;
                    case TraitMonsterBall traitMonsterBall:
                        item.LV = genLv;
                        break;
                    case TraitSpellbook traitSpellbook:
                        item.c_charges = 7;
                        break;
                    case TraitModRanged traitModRanged:
                        item.encLV = genLv;
                        break;
                    case TraitChest traitChest:
                        item.c_lockedHard = true;
                        item.c_lockLv = genLv;
                        item.c_revealLock = true;
                        ThingGen.CreateTreasureContent(t: item, lv: genLv, type: TreasureType.BossNefia, clearContent: true);
                        break;
                }
            }
        }

        // Add perfumes to inventory
        int[] perfumeIds = { 9500, 9501, 9502, 9503 };
        foreach (int perfumeId in perfumeIds)
        {
            Thing perfume = ThingGen.CreatePerfume(ele: perfumeId, num: 5);
            if (perfume != null)
            {
                inventory?.AddThing(t: perfume);
            }
        }
        
        // Add scrolls to inventory
        int[] scrollIds = { 8200, 8220, 8221, 8232, 8241, 8251, 8256, 8260, 8280, 8430, 8770, 8780, 9160 };
        foreach (int scrollId in scrollIds)
        {
            int scrollNum = 4 + EClass.rnd(a: 6);
            Thing scroll = ThingGen.CreateScroll(ele: scrollId, num: scrollNum);
            if (scroll != null)
            {
                scroll.c_IDTState = 0;
                inventory?.AddThing(t: scroll);
            }
        }
        
        // Add potions to inventory
        int[] potionIds = { 8480 };
        foreach (int potionId in potionIds)
        {
            Thing potion = ThingGen.CreatePotion(ele: potionId);
            if (potion != null)
            {
                potion.c_IDTState = 0;
                inventory?.AddThing(t: potion);
            }
        }
        
        // Add unlearned recipes to inventory
        string recipeId = RecipeManager.GetRandomRecipe(lvBonus: 0, cat: null, onlyUnlearned: true);
        Thing recipe = ThingGen.CreateRecipe(id: recipeId);
        if (recipe != null)
        {
            inventory?.AddThing(t: recipe);
        }
        
        // Add range mod to inventory
        Thing mod_range = ThingGen.Create(id: "mod_ranged", lv: genLv);
        if (mod_range != null)
        {
            mod_range.encLV = genLv;
            inventory?.AddThing(t: mod_range);
        }
        
        // Add map to inventory
        Thing map = ThingGen.Create(id: "map", lv: genLv);
        if (map != null)
        {
            map.SetInt(id: 25, value: genLv);
            inventory?.AddThing(t: map);
        }
        
        // Add custom equipment to inventory
        CardBlueprint.Set(_bp: new CardBlueprint
        {
            rarity = Rarity.Mythical
        });
        
        Thing kenkonken = ThingGen.Create(id: "martial_kenkonken", lv: genLv);
        if (kenkonken != null)
        {
            kenkonken.c_IDTState = 0;
            inventory?.AddThing(t: kenkonken);
        }
    }
}