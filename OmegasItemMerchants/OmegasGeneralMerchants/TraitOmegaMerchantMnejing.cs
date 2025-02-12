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
                    case TraitBait traitBait:
                    case TraitAmmoArrow traitAmmoArrow:
                    case TraitAmmoBolt traitAmmoBolt:
                    case TraitAmmoBullet traitAmmoBullet:
                    case TraitAmmoEnergy traitAmmoEnergy:
                        var ammoNum = 300 + EClass.rnd(a: 100);
                        item.SetNum(a: ammoNum);
                        break;
                    case TraitFoodTravel traitFoodTravel:
                        var rationNum = 2 + EClass.rnd(a: 4);
                        item.SetNum(a: rationNum);
                        break;
                    case TraitMaterialHammer traitMaterialHammer:
                        SourceMaterial.Row randomMaterial = MATERIAL.GetRandomMaterial(lv: ((genLv < 200) ? (genLv / 2) : (genLv % 50 * 2)) + 10, group: (EClass.rnd(a: 2) == 0) ? "metal" : "leather", tryLevelMatTier: true);
                        item.ChangeMaterial(row: randomMaterial, ignoreFixedMaterial: false);
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
    }
}