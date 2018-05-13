using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amadues
{
    public class ItemGenerator
    {
        //public static Constants sc;

        public struct WeightedWord
        {
            public int value;
            public String word;

            public WeightedWord(int aValue, String aWord)
            {
                value = aValue;
                word = aWord;
            }
        }

        //How many floors better a merchant's items can be
        const int MERCHANT_VALUE_BONUS = 2;

        const int MAX_TINS_PER_CORPSE = 5;

        Random random;

        List<WeightedWord> qualityAdjectives;
        List<WeightedWord> fireAdjectives;
        List<WeightedWord> iceAdjectives;
        List<WeightedWord> swordNames;
        List<WeightedWord> daggerNames;
        List<WeightedWord> maceNames;
        List<WeightedWord> spearNames;
        List<WeightedWord> bowNames;
        List<WeightedWord> shieldNames;
        List<WeightedWord> armourNames;
        List<WeightedWord> cloakNames;
        List<WeightedWord> questItemNouns;
        List<WeightedWord> questItemAdjectives;

        public ItemGenerator()
        {
            random = new Random();
            qualityAdjectives = new List<WeightedWord>();
            fireAdjectives = new List<WeightedWord>();
            iceAdjectives = new List<WeightedWord>();
            swordNames = new List<WeightedWord>();
            daggerNames = new List<WeightedWord>();
            maceNames = new List<WeightedWord>();
            spearNames = new List<WeightedWord>();
            bowNames = new List<WeightedWord>();
            shieldNames = new List<WeightedWord>();
            armourNames = new List<WeightedWord>();
            cloakNames = new List<WeightedWord>();
            questItemNouns = new List<WeightedWord>();
            questItemAdjectives = new List<WeightedWord>();
            LoadAdjectives();
        }

        public Item GenerateRandomItem(int value)
        {
            Item item = new Item();
            SC.ItemTypes randomType = (SC.ItemTypes)random.Next(SC.numItemTypes);
            //int randomType = random.Next(0, SC.numItemTypes);
            item.itemType = (int)randomType;
            switch (randomType)
            {
                case SC.ItemTypes.WEAPON:
                    item = GenerateWeapon(value);
                    break;
                case SC.ItemTypes.SHIELD:
                    item = GenerateShield(value);
                    break;
                case SC.ItemTypes.ARMOUR:
                    item = GenerateArmour(value);
                    break;
                case SC.ItemTypes.GOLD:
                    item.value = random.Next(1, 10) * value;
                    item.itemName = item.value.ToString() + " Gold";
                    item.itemDescription = item.value.ToString() + " Gold";
                    break;
                case SC.ItemTypes.CLOAK:
                    item = GenerateCloak(value);
                    break;
                case SC.ItemTypes.POTION:
                    item = GeneratePotion(value);
                    break;
                case SC.ItemTypes.SCROLL:
                    item = GenerateScroll(value);
                    break;
                case SC.ItemTypes.FOOD:
                    item = GenerateFood();
                    break;
                case SC.ItemTypes.ROCK:
                    if (value < 500)
                    {
                        item = GenerateScroll(value);
                    }
                    else
                    {
                        item = GenerateGrindStone(value);
                    }
                    break;
                case SC.ItemTypes.QUEST:
                    item = GenerateFood();
                    break;
                default:
                    item.value = 1;
                    item.itemName = "An Odd Gold Piece";
                    item.itemDescription = item.value.ToString() + " Gold";
                    break;
            }
            return item;
        }

        //Same as GenerateRandomItem(int value) except does not 
        //generate gold
        public Item GenerateShopItem(int value)
        {
            Item item = new Item();
            SC.ItemTypes randomType = (SC.ItemTypes)random.Next(SC.numItemTypes);
            item.itemType = (int)randomType;
            switch (randomType)
            {
                case SC.ItemTypes.WEAPON:
                    item = GenerateWeapon(value);
                    break;
                case SC.ItemTypes.SHIELD:
                    item = GenerateShield(value);
                    break;
                case SC.ItemTypes.ARMOUR:
                    item = GenerateArmour(value);
                    break;
                case SC.ItemTypes.GOLD:
                    //If gold, generate a potion instead
                    item = GeneratePotion(value);
                    break;
                case SC.ItemTypes.CLOAK:
                    item = GenerateCloak(value);
                    break;
                case SC.ItemTypes.POTION:
                    item = GeneratePotion(value);
                    break;
                case SC.ItemTypes.SCROLL:
                    item = GenerateScroll(value);
                    break;
                case SC.ItemTypes.FOOD:
                    item = GenerateFood();
                    break;
                case SC.ItemTypes.ROCK:
                    item = GenerateGrindStone(value);
                    break;
                case SC.ItemTypes.QUEST:
                    item = GenerateScroll(value);
                    break;
                default:
                    item.value = 1;
                    item.itemName = "An Odd Gold Piece";
                    item.itemDescription = item.value.ToString() + " Gold";
                    break;
            }
            return item;
        }

        public Item GenerateVillageShopItem(int value)
        {
            Item item = new Item();
            SC.ItemTypes randomType = (SC.ItemTypes)random.Next(SC.numItemTypes);
            item.itemType = (int)randomType;
            switch (randomType)
            {
                case SC.ItemTypes.WEAPON:
                    item = GenerateWeapon(value);
                    break;
                case SC.ItemTypes.SHIELD:
                    item = GenerateShield(value);
                    break;
                case SC.ItemTypes.ARMOUR:
                    item = GenerateArmour(value);
                    break;
                case SC.ItemTypes.GOLD:
                    //If gold, generate a potion instead
                    item = GeneratePotion(value);
                    break;
                case SC.ItemTypes.CLOAK:
                    item = GenerateCloak(value);
                    break;
                case SC.ItemTypes.POTION:
                    item = GeneratePotion(value);
                    break;
                case SC.ItemTypes.SCROLL:
                    item = GenerateScroll(value);
                    break;
                case SC.ItemTypes.FOOD:
                    //If food, generate a grindstone instead
                    item = GenerateGrindStone(value);
                    break;
                case SC.ItemTypes.ROCK:
                    item = GenerateWeapon(value);
                    break;
                case SC.ItemTypes.QUEST:
                    item = GenerateScroll(value);
                    break;
                default:
                    item.value = 1;
                    item.itemName = "An Odd Gold Piece";
                    item.itemDescription = item.value.ToString() + " Gold";
                    break;
            }
            return item;
        }

        public Item GenerateDungeonShopItem(int value)
        {
            Item item = new Item();
            SC.ItemTypes randomType = (SC.ItemTypes)random.Next(SC.numItemTypes);
            item.itemType = (int)randomType;
            switch (randomType)
            {
                case SC.ItemTypes.WEAPON:
                    item = GenerateWeapon(value);
                    break;
                case SC.ItemTypes.SHIELD:
                    //If shield, generate a potion instead
                    item = GeneratePotion(value);
                    break;
                case SC.ItemTypes.ARMOUR:
                    //If armour, generate a potion instead
                    item = GeneratePotion(value);
                    break;
                case SC.ItemTypes.GOLD:
                    //If gold, generate a potion instead
                    item = GeneratePotion(value);
                    break;
                case SC.ItemTypes.CLOAK:
                    item = GenerateCloak(value);
                    break;
                case SC.ItemTypes.POTION:
                    item = GeneratePotion(value);
                    break;
                case SC.ItemTypes.SCROLL:
                    item = GenerateScroll(value);
                    break;
                case SC.ItemTypes.FOOD:
                    item = GenerateTinnedFood();
                    break;
                case SC.ItemTypes.ROCK:
                    item = GenerateGrindStone(value);
                    break;
                case SC.ItemTypes.QUEST:
                    item = GenerateScroll(value);
                    break;
                default:
                    item.value = 1;
                    item.itemName = "An Odd Gold Piece";
                    item.itemDescription = item.value.ToString() + " Gold";
                    break;
            }
            return item;
        }

        public Item GenerateQuestItem(int aLevel)
        {
            Item result = new Item();
            result.value = 0;
            result.weight = 0;
            result.itemType = (int)SC.ItemTypes.QUEST;
            result.itemName = GenerateQuestItemName(aLevel);
            result.itemDescription = result.itemName;
            return result;
        }

        //Generate starting items - do not generate swords shields or armour
        public Item GenerateStartingGearItem(int value)
        {
            Item item = new Item();
            int randomType = random.Next(0, 4);
            item.itemType = randomType;
            switch (randomType)
            {
                case 0:
                    item = GenerateCloak(value);
                    break;
                case 1:
                    item = GeneratePotion(value);
                    break;
                case 2:
                    item = GenerateScroll(value);
                    break;
                case 3:
                    item = GenerateFood();
                    break;
                default:
                    item.value = 1;
                    item.itemName = "An Odd Gold Piece";
                    item.itemDescription = item.value.ToString() + " Gold";
                    break;
            }
            return item;
        }

        public Item GenerateWeapon(int value)
        {
            SC.WeaponTypes choice = (SC.WeaponTypes)random.Next(0, SC.numWeaponTypes);
            switch (choice)
            {
                case SC.WeaponTypes.DAGGER:
                    return GenerateDagger(value);
                case SC.WeaponTypes.MACE:
                    return GenerateMace(value);
                case SC.WeaponTypes.SWORD:
                    return GenerateSword(value);
                case SC.WeaponTypes.SPEAR:
                    return GenerateSpear(value);
                case SC.WeaponTypes.BOW:
                    return GenerateBow(value);
                default:
                    return GenerateDagger(value);
            }
        }

        public Item GenerateSword(int value)
        {
            Item sword = new Item();
            sword.itemType = (int)SC.ItemTypes.WEAPON;
            sword.weaponType = (int)SC.WeaponTypes.SWORD;
            sword.itemName = RandomSwordName(value);
            sword.itemDescription = "A Sword";
            sword.damage = value;
            sword.value = value * 10 + random.Next(1, 10);
            sword.weight = 2.0f + ((float)(random.Next(200))/100f);

            if (random.NextDouble() < 0.2)
            {
                //Fire Sword
                sword.damage = (int)Math.Ceiling(sword.damage / 2);
                sword.elementalOffenceFire = random.Next(1, value);
                sword.itemName = RandomElementalSwordName(value);
                sword.itemName = "+" + sword.elementalOffenceFire + " " + RandomWord(value, fireAdjectives).word + " " + sword.itemName;
            }
            else
            {
                if (random.NextDouble() < 0.3)
                {
                    //Ice Sword
                    sword.damage = (int)Math.Ceiling(sword.damage / 2);
                    sword.elementalOffenceIce = random.Next(1, value);
                    sword.itemName = RandomElementalSwordName(value);
                    sword.itemName = "+" + sword.elementalOffenceIce + " " + RandomWord(value, iceAdjectives).word + " " + sword.itemName;
                }
            }

            sword.value += sword.elementalOffenceFire * 100;
            sword.value += sword.elementalOffenceIce * 100;

            return sword;
        }

        public Item GenerateDagger(int value)
        {
            Item dagger = new Item();
            dagger.itemType = (int)SC.ItemTypes.WEAPON;
            dagger.weaponType = (int)SC.WeaponTypes.DAGGER;
            dagger.itemName = RandomDaggerName(value);
            dagger.itemDescription = "A Dagger";
            dagger.damage = (int)Math.Max(1, Math.Ceiling(value / 2));
            dagger.value = value * 5 + random.Next(1, 5);
            dagger.weight = 0.3f + ((float)(random.Next(50)) / 100f);

            return dagger;
        }

        public Item GenerateMace(int value)
        {
            Item mace = new Item();
            mace.itemType = (int)SC.ItemTypes.WEAPON;
            mace.weaponType = (int)SC.WeaponTypes.MACE;
            mace.itemName = RandomMaceName(value);
            mace.itemDescription = "A Mace";
            mace.damage = value + (int)Math.Ceiling(value / 3);
            mace.value = value * 10 + random.Next(1, 20);
            mace.weight = 6.0f + ((float)(random.Next(600)) / 100f);

            return mace;
        }

        public Item GenerateSpear(int value)
        {
            Item spear = new Item();
            spear.itemType = (int)SC.ItemTypes.WEAPON;
            spear.weaponType = (int)SC.WeaponTypes.SPEAR;
            spear.itemName = RandomSpearName(value);
            spear.itemDescription = "A Spear";
            spear.damage = value + (int)Math.Ceiling(value / 6);
            spear.range = 2;
            spear.value = value * 9 + random.Next(1, 20);
            spear.weight = 6.0f + ((float)(random.Next(600)) / 100f);

            return spear;
        }

        public Item GenerateBow(int value)
        {
            Item bow = new Item();
            bow.itemType = (int)SC.ItemTypes.WEAPON;
            bow.weaponType = (int)SC.WeaponTypes.BOW;
            bow.itemName = RandomBowName(value);
            bow.itemDescription = "A Bow";
            bow.damage = value;
            bow.range = SC.RANGE_OF_BOW_WEAPONS;
            bow.value = value * 9 + random.Next(1, 20);
            bow.weight = 3.0f + ((float)(random.Next(400)) / 100f);

            return bow;
        }

        public Item GenerateShield(int value)
        {
            Item shield = new Item();
            shield.itemType = (int)SC.ItemTypes.SHIELD;
            shield.itemName = RandomShieldName(value);
            shield.itemDescription = "Shield";
            shield.defence = (int)Math.Ceiling(value/3);
            shield.value = value * 10 + random.Next(1, 10);
            shield.weight = 4.0f + ((float)(random.Next(300)) / 100f);

            return shield;
        }

        public Item GenerateArmour(int value)
        {
            Item armour = new Item();
            armour.itemType = (int)SC.ItemTypes.ARMOUR;
            armour.itemName = RandomArmourName(value);
            armour.itemDescription = "Armour";
            armour.defence = (int)Math.Ceiling((double)((double)value/10) * 9);
            armour.value = value * 10 + random.Next(1, 10);
            armour.weight = 18.0f + ((float)(random.Next(400)) / 100f);

            return armour;
        }

        public Item GenerateCloak(int value)
        {
            Item cloak = new Item();
            cloak.itemType = (int)SC.ItemTypes.CLOAK;
            cloak.itemName = RandomCloakName(value);
            cloak.itemDescription = "Cloak";
            cloak.defence = (int)Math.Max(1,Math.Ceiling(Math.Log((double)value)));
            cloak.value = value * random.Next(2,3);
            cloak.weight = 2.0f + ((float)(random.Next(200)) / 100f);
            if (random.NextDouble() < 0.5)
            {
                cloak.elementalDefenceFire = Math.Round(random.NextDouble(), 2);
                if (cloak.elementalDefenceFire < 0.1) cloak.elementalDefenceFire = 0.1;
            }
            if (random.NextDouble() < 0.5)
            {
                cloak.elementalDefenceIce = Math.Round(random.NextDouble(), 2);
                if (cloak.elementalDefenceIce < 0.1) cloak.elementalDefenceIce = 0.1;
            }
            //adjust value according to actual worth
            cloak.value = 20 * Math.Ceiling(((double)cloak.defence) / cloak.elementalDefenceFire / cloak.elementalDefenceIce);
            return cloak;
        }

        public Item GeneratePotion(int value)
        {

            float randomChoice = (float)random.NextDouble();

            if (randomChoice < 0.2)
            {
                return ItemMagikPotion();
            }

            if (randomChoice < 0.4)
            {
                return ItemHealingPotion();
            }

            if (randomChoice < 0.5)
            {
                return ItemAntidote();
            }

            if (randomChoice < 0.6)
            {
                return ItemAttackPotion();
            }

            if (value > 1000)
            {
                if (randomChoice < 0.8)
                {
                    return ItemVitalityElixir();
                }

                return ItemWillElixir();
            }

            return ItemHealingPotion();

        }

        public Item ItemMagikPotion()
        {
            Item potion = new Item();
            potion.itemType = (int)SC.ItemTypes.POTION;
            potion.itemName = "Magic Potion";
            potion.itemDescription = "Potion To Restore Some Magic";
            potion.value = 30;
            potion.weight = 0.1f;

            return potion;
        }
        public Item ItemHealingPotion()
        {
            Item potion = new Item();
            potion.itemType = (int)SC.ItemTypes.POTION;
            potion.itemName = "Potion of Healing";
            potion.itemDescription = "Potion To Heal Wounds";
            potion.value = 30;
            potion.weight = 0.1f;

            return potion;
        }
        public Item ItemAntidote()
        {
            Item potion = new Item();
            potion.itemType = (int)SC.ItemTypes.POTION;
            potion.itemName = "Antidote";
            potion.itemDescription = "Cures Poison";
            potion.value = 30;
            potion.weight = 0.1f;

            return potion;
        }
        public Item ItemAttackPotion()
        {
            Item potion = new Item();
            potion.itemType = (int)SC.ItemTypes.POTION;
            potion.itemName = "Attack Potion";
            potion.itemDescription = "Temporarily Increases Attack Power";
            potion.value = 200;
            potion.weight = 0.1f;

            return potion;
        }
        public Item ItemVitalityElixir()
        {
            Item potion = new Item();
            potion.itemType = (int)SC.ItemTypes.POTION;
            potion.itemName = "Vitality Elixir";
            potion.itemDescription = "Slightly Increases Max Health";
            potion.value = 1000;
            potion.weight = 0.1f;

            return potion;
        }
        public Item ItemWillElixir()
        {
            Item potion = new Item();
            potion.itemType = (int)SC.ItemTypes.POTION;
            potion.itemName = "Will Elixir";
            potion.itemDescription = "Slightly Increases Max Magic";
            potion.value = 1000;
            potion.weight = 0.1f;

            return potion;
        }

        public Item GenerateScroll(int value)
        {
            Item scroll = new Item();
            scroll.itemType = (int)SC.ItemTypes.SCROLL;
            scroll.itemDescription = "Scroll inscribed with a spell";
            scroll.weight = 0.01f;

            int randomSelect = random.Next(0, 100);
            if (randomSelect >= 0 && randomSelect < 10)
            {
                if (value > 1000)
                {
                    scroll.itemName = "Life Scroll";
                    scroll.itemDescription = "Used Automatically If Your Health Reaches 0";
                    scroll.value = 80 * value;
                }
                else
                {
                    scroll.itemName = "Scroll of Detect Gold";
                    scroll.value = 20;
                }
            }
            if (randomSelect >= 10 && randomSelect < 20)
            {
                scroll.itemName = "Scroll of Detect Gold";
                scroll.value = 20;
            }
            if (randomSelect >= 20 && randomSelect < 30)
            {
                scroll.itemName = "Scroll of Detect Item";
                scroll.value = 30;
            }
            if (randomSelect >= 30 && randomSelect < 40)
            {
                scroll.itemName = "Scroll of Cowardice";
                scroll.value = 30;
            }
            if (randomSelect >= 40 && randomSelect < 50)
            {
                scroll.itemName = "Scroll of Detect Creature";
                scroll.value = 30;
            }
            if (randomSelect >= 50 && randomSelect < 65)
            {
                scroll.itemName = "Scroll of Detect Furniture";
                scroll.value = 20;
            }
            if (randomSelect >= 65 && randomSelect < 80)
            {
                scroll.itemName = "Scroll of Wild Descent";
                scroll.itemDescription = "Read this to descend deeper into the dungeon...";
                scroll.value = 100;
            }
            if (randomSelect >= 80 && randomSelect < 90)
            {
                scroll.itemName = "Karma Scroll";
                scroll.itemDescription = "This scroll can change your odds!";
                scroll.value = 20;
            }
            if (randomSelect >= 90 && randomSelect < 100)
            {
                scroll.itemName = "Scroll of Detect Traps";
                scroll.value = 20;
            }

            return scroll;
        }

        public Item GenerateGrindStone(int value)
        {
            Item gs = new Item();
            gs.itemType = (int)SC.ItemTypes.ROCK;
            gs.rockType = (int)SC.RockTypes.GRINDSTONE;
            gs.weight = 1;
            gs.damage = 1 + random.Next(Math.Max(1, (int)(Math.Log((double)value)) / 2));
            gs.value = 300 * gs.damage;
            gs.itemName = "+" + gs.damage.ToString() + " Grindstone";
            gs.itemDescription = "Improves weapon +" + gs.damage.ToString() + " damage";
            if (random.NextDouble() < 0.15)
            {
                gs.elementalOffenceFire = 1 + random.Next(Math.Max(1, (int)(Math.Log((double)value)) / 4));
                gs.value += 400 * gs.elementalOffenceFire;
                gs.itemName = BestWord((int)gs.elementalOffenceFire, fireAdjectives).word + " " + gs.itemName;
                gs.itemDescription = gs.itemDescription + " +"
                                        + gs.elementalOffenceFire.ToString() + " Fire Damage -"
                                        + gs.elementalOffenceFire.ToString() + " Ice Damage";
            }
            else
            {
                if (random.NextDouble() < 0.2)
                {
                    gs.elementalOffenceIce = 1 + random.Next(Math.Max(1, (int)(Math.Log((double)value)) / 4));
                    gs.value += 400 * gs.elementalOffenceIce;
                    gs.itemName = BestWord((int)gs.elementalOffenceIce, iceAdjectives).word + " " + gs.itemName;
                    gs.itemDescription = gs.itemDescription + " +"
                                            + gs.elementalOffenceIce.ToString() + " Ice Damage -"
                                            + gs.elementalOffenceIce.ToString() + " Fire Damage";
                }
            }
            return gs;
        }

        public Item GenerateFood()
        {
            double randomChoice = random.NextDouble();
            if (randomChoice < 0.30)
            {
                return GenerateMeat();
            }
            if (randomChoice < 0.65)
            {
                return GenerateBread();
            }
            if (randomChoice < 0.93)
            {
                return GenerateFruitAndVeg();
            }
            return GenerateTinnedFood();
        }

        public Item GenerateFruitAndVeg()
        {
            Item food = new Item();
            food.itemType = (int)SC.ItemTypes.FOOD;
            float randomChoice = (float)random.NextDouble();
            if (randomChoice < 0.15f)
            {
                food.itemName = "Lettuce";
                food.itemDescription = "Iceberg Lettuce";
                food.weight = 0.4f;
                food.foodType = (int)SC.FoodTypes.VEG;
                food.nutrients = 20f;
                food.decay = 0.0008f;
                food.orcAppetiteModifier = 1.00f;
                food.humanAppetiteModifier = 1.00f;
                food.elfAppetiteModifier = 1.00f;
                return food;
            }
            if (randomChoice < 0.30f)
            {
                food.itemName = "Cabbage";
                food.itemDescription = "Fresh head of cabbage";
                food.weight = 0.6f;
                food.foodType = (int)SC.FoodTypes.VEG;
                food.nutrients = 40f;
                food.decay = 0.0008f;
                food.orcAppetiteModifier = 1.00f;
                food.humanAppetiteModifier = 1.00f;
                food.elfAppetiteModifier = 1.00f;
                return food;
            }
            if (randomChoice < 0.45f)
            {
                food.itemName = "Cauliflower";
                food.itemDescription = "Head of cauliflower";
                food.weight = 1.0f;
                food.foodType = (int)SC.FoodTypes.VEG;
                food.nutrients = 40f;
                food.decay = 0.0008f;
                food.orcAppetiteModifier = 0.80f;
                food.humanAppetiteModifier = 0.80f;
                food.elfAppetiteModifier = 0.80f;
                return food;
            }
            if (randomChoice < 0.60f)
            {
                food.itemName = "Tomatoes";
                food.itemDescription = "Bunch of ripe tomatoes";
                food.weight = 0.4f;
                food.foodType = (int)SC.FoodTypes.VEG;
                food.nutrients = 25f;
                food.decay = 0.0008f;
                food.orcAppetiteModifier = 1.00f;
                food.humanAppetiteModifier = 1.00f;
                food.elfAppetiteModifier = 1.00f;
                return food;
            }
            if (randomChoice < 0.75f)
            {
                food.itemName = "Celery";
                food.itemDescription = "Sticks of crunchy celery";
                food.weight = 0.1f;
                food.foodType = (int)SC.FoodTypes.VEG;
                food.nutrients = 1f;
                food.decay = 0.0008f;
                food.orcAppetiteModifier = 1.00f;
                food.humanAppetiteModifier = 1.00f;
                food.elfAppetiteModifier = 1.00f;
                return food;
            }
                food.itemName = "Carrot";
                food.itemDescription = "Crunchy Carrot";
                food.weight = 0.08f;
                food.foodType = (int)SC.FoodTypes.VEG;
                food.nutrients = 8f;
                food.decay = 0.0008f;
                food.orcAppetiteModifier = 1.00f;
                food.humanAppetiteModifier = 1.00f;
                food.elfAppetiteModifier = 1.00f;
                return food;
        }

        public Item GenerateMeat()
        {
            Item food = new Item();
            food.itemType = (int)SC.ItemTypes.FOOD;
            float randomChoice = (float)random.NextDouble();
            if (randomChoice < 0.15)
            {
                food.itemName = "Ham";
                food.itemDescription = "Delicious Ham";
                food.weight = 1.5f;
                food.foodType = (int)SC.FoodTypes.MEAT;
                food.nutrients = 40f;
                food.decay = 0.0005f;
                food.orcAppetiteModifier = 1.05f;
                food.humanAppetiteModifier = 1.05f;
                food.elfAppetiteModifier = 0.40f;
                return food;
            }
            if (randomChoice < 0.30)
            {
                food.itemName = "Pork Chop";
                food.itemDescription = "Juicy pork chop";
                food.weight = 0.5f;
                food.foodType = (int)SC.FoodTypes.MEAT;
                food.nutrients = 30f;
                food.decay = 0.0008f;
                food.orcAppetiteModifier = 1.05f;
                food.humanAppetiteModifier = 1.05f;
                food.elfAppetiteModifier = 0.40f;
                return food;
            }
            if (randomChoice < 0.45)
            {
                food.itemName = "Mutton Chop";
                food.itemDescription = "Tender campfire-cooked mutton";
                food.weight = 0.5f;
                food.foodType = (int)SC.FoodTypes.MEAT;
                food.nutrients = 30f;
                food.decay = 0.0008f;
                food.orcAppetiteModifier = 1.05f;
                food.humanAppetiteModifier = 1.05f;
                food.elfAppetiteModifier = 0.40f;
                return food;
            }
            if (randomChoice < 0.60)
            {
                food.itemName = "Fish";
                food.itemDescription = "Nutritious fish";
                food.weight = 1.0f;
                food.foodType = (int)SC.FoodTypes.MEAT;
                food.nutrients = 50f;
                food.decay = 0.0004f;
                food.orcAppetiteModifier = 1.05f;
                food.humanAppetiteModifier = 1.05f;
                food.elfAppetiteModifier = 0.40f;
                return food;
            }
            if (randomChoice < 0.75)
            {
                food.itemName = "Hogmeat";
                food.itemDescription = "Roasted leg of warthog";
                food.weight = 1.0f;
                food.foodType = (int)SC.FoodTypes.MEAT;
                food.nutrients = 50f;
                food.decay = 0.0002f;
                food.orcAppetiteModifier = 1.05f;
                food.humanAppetiteModifier = 1.05f;
                food.elfAppetiteModifier = 0.40f;
                return food;
            }
            food.itemName = "Fillet Steak";
            food.itemDescription = "Lightly seasoned and pink in the middle";
            food.weight = 0.5f;
            food.foodType = (int)SC.FoodTypes.MEAT;
            food.nutrients = 20f;
            food.decay = 0.001f;
            food.orcAppetiteModifier = 1.05f;
            food.humanAppetiteModifier = 1.05f;
            food.elfAppetiteModifier = 0.40f;
            return food;
        }

        public Item GenerateBread()
        {
            Item food = new Item();
            food.itemType = (int)SC.ItemTypes.FOOD;
            float randomChoice = (float)random.NextDouble();

            if (randomChoice < 0.20)
            {
                food.itemName = "Elven Bread Loaf";
                food.itemDescription = "Unappetising but long lasting";
                food.weight = 0.1f;
                food.foodType = (int)SC.FoodTypes.BREAD;
                food.nutrients = 20f;
                food.decay = 0.0001f;
                food.orcAppetiteModifier = 1.05f;
                food.humanAppetiteModifier = 1f;
                food.elfAppetiteModifier = 1.05f;
                return food;
            }

            if (randomChoice < 0.40)
            {
                food.itemName = "Wheaten Loaf";
                food.itemDescription = "A whole wheaten loaf!";
                food.weight = 0.7f;
                food.foodType = (int)SC.FoodTypes.BREAD;
                food.nutrients = 50f;
                food.decay = 0.0003f;
                food.orcAppetiteModifier = 1.05f;
                food.humanAppetiteModifier = 1f;
                food.elfAppetiteModifier = 1.05f;
                return food;
            }

            food.itemName = "Bread Loaf";
            food.itemDescription = "Wholesome Loaf of Bread";
            food.weight = 0.6f;
            food.foodType = (int)SC.FoodTypes.BREAD;
            food.nutrients = 40f;
            food.decay = 0.0004f;
            food.orcAppetiteModifier = 1.05f;
            food.humanAppetiteModifier = 1f;
            food.elfAppetiteModifier = 1.05f;
            return food;
        }

        public Item GenerateTinnedFood()
        {
            Item food = new Item();
            food.itemType = (int)SC.ItemTypes.FOOD;
            float randomChoice = (float)random.NextDouble();
            if (randomChoice < 0.2f)
            {
                food.itemName = "Tin Of Peaches";
                food.itemDescription = "Peaches from a can";
                food.weight = 0.2f;
                food.foodType = (int)SC.FoodTypes.VEG;
                food.nutrients = 15f;
                food.canned = true;
                food.orcAppetiteModifier = 1f;
                food.humanAppetiteModifier = 1f;
                food.elfAppetiteModifier = 1.10f;
                return food;
            }

            if (randomChoice < 0.4f)
            {
                food.itemName = "Tin Of Tomatoes";
                food.itemDescription = "Skinned tomatoes preserved in their own juice";
                food.weight = 0.2f;
                food.foodType = (int)SC.FoodTypes.VEG;
                food.nutrients = 20f;
                food.canned = true;
                food.orcAppetiteModifier = 1f;
                food.humanAppetiteModifier = 1f;
                food.elfAppetiteModifier = 1.10f;
                return food;
            }
            
            if (randomChoice < 0.6f)
            {
                food.itemName = "Tin Of Sardines";
                food.itemDescription = "Sardines preserved in brine, ready to eat";
                food.weight = 0.2f;
                food.foodType = (int)SC.FoodTypes.VEG;
                food.nutrients = 25f;
                food.canned = true;
                food.orcAppetiteModifier = 1.1f;
                food.humanAppetiteModifier = 1.1f;
                food.elfAppetiteModifier = 0.40f;
                return food;
            }

            if (randomChoice < 0.8f)
            {
                food.itemName = "Tinned Pudding";
                food.itemDescription = "Chocolately sponge dessert sealed in a tin";
                food.weight = 0.2f;
                food.foodType = (int)SC.FoodTypes.VEG;
                food.nutrients = 25f;
                food.canned = true;
                food.orcAppetiteModifier = 1.1f;
                food.humanAppetiteModifier = 1.1f;
                food.elfAppetiteModifier = 1.1f;
                return food;
            }

            food.itemName = "Tin Of Corned Beef";
            food.itemDescription = "Tin of corned beef";
            food.weight = 0.3f;
            food.foodType = (int)SC.FoodTypes.MEAT;
            food.nutrients = 15f;
            food.canned = true;
            food.orcAppetiteModifier = 1f;
            food.humanAppetiteModifier = 1f;
            food.elfAppetiteModifier = 0.40f;
            return food;
        }

        public Item GenerateTinnedMeat()
        {
            Item tinnedMeat = new Item();
            tinnedMeat.itemType = (int)SC.ItemTypes.FOOD;
            tinnedMeat.itemName = "Tinned Meat";
            tinnedMeat.itemDescription = "Well Prepared And Preserved Meat";
            tinnedMeat.weight = 0.6f;
            tinnedMeat.foodType = (int)SC.FoodTypes.MEAT;
            tinnedMeat.nutrients = 20f;
            tinnedMeat.value = 30;
            tinnedMeat.canned = true;
            tinnedMeat.orcAppetiteModifier = 1f;
            tinnedMeat.humanAppetiteModifier = 1f;
            tinnedMeat.elfAppetiteModifier = 0.40f;
            return tinnedMeat;
        }

        //Generate a corpse/meat. For larger enemies, also pass offset for X and Y
        //for where the tile is relative to the creature's Top/Left
        public Item GenerateCorpse(Creature theCreature, int offsetX, int offsetY)
        {
            Item corpse = new Item();
            corpse.x = theCreature.x + offsetX;
            corpse.y = theCreature.y + offsetY;
            corpse.itemType = (int)SC.ItemTypes.FOOD;
            corpse.foodType = (int)SC.FoodTypes.CORPSE;
            corpse.itemDescription = "The Remains of a Defeated Foe";
            corpse.decay = 0.001f;
            corpse.deadCreature = theCreature;
            switch (theCreature.description.creatureType)
            {
                case (int)SC.CreatureTypes.BAT:
                    corpse.nutrients = 5f;
                    corpse.weight = 1f;
                    corpse.itemName = "Bat Carcass";
                    break;
                case (int)SC.CreatureTypes.BUG:
                    corpse.nutrients = 5f;
                    corpse.weight = 0.5f;
                    corpse.itemName = "Dead Bug";
                    break;
                case (int)SC.CreatureTypes.CANINE:
                    corpse.nutrients = 90f;
                    corpse.weight = 20f;
                    corpse.itemName = "Dog Corpse";
                    break;
                case (int)SC.CreatureTypes.DRAGON:
                    corpse.nutrients = 80f;
                    corpse.weight = 15f;
                    corpse.itemName = "Lump of Dragon Flesh";
                    break;
                case (int)SC.CreatureTypes.ELF:
                    corpse.nutrients = 80f;
                    corpse.weight = 75f;
                    corpse.itemName = "Elf Corpse";
                    corpse.orcAppetiteModifier = 0.1f;
                    corpse.humanAppetiteModifier = 0.1f;
                    corpse.elfAppetiteModifier = 0.02f;
                    break;
                case (int)SC.CreatureTypes.FELINE:
                    corpse.nutrients = 10f;
                    corpse.weight = 6f;
                    corpse.itemName = "Feline Carcass";
                    break;
                case (int)SC.CreatureTypes.GHOST:
                    corpse.nutrients = 5f;
                    corpse.weight = 0.4f;
                    corpse.value = 100;
                    corpse.orcAppetiteModifier = 0.3f;
                    corpse.humanAppetiteModifier = 0.1f;
                    corpse.elfAppetiteModifier = 0.1f;
                    corpse.itemName = "Ectoplasm";
                    break;
                case (int)SC.CreatureTypes.GOBLIN:
                    corpse.nutrients = 75f;
                    corpse.weight = 70f;
                    corpse.itemName = "Goblin Corpse";
                    corpse.orcAppetiteModifier = 0.2f;
                    corpse.humanAppetiteModifier = 0.1f;
                    corpse.elfAppetiteModifier = 0.1f;
                    break;
                case (int)SC.CreatureTypes.HUMAN:
                    corpse.nutrients = 90f;
                    corpse.weight = 85f;
                    corpse.itemName = "Human Corpse";
                    corpse.orcAppetiteModifier = 0.1f;
                    corpse.humanAppetiteModifier = 0.02f;
                    corpse.elfAppetiteModifier = 0.1f;
                    break;
                case (int)SC.CreatureTypes.LIZARD:
                    corpse.nutrients = 30f;
                    corpse.weight = 1f;
                    corpse.itemName = "Reptile Carcass";
                    corpse.orcAppetiteModifier = 1.5f;
                    break;
                case (int)SC.CreatureTypes.ORC:
                    corpse.nutrients = 100f;
                    corpse.weight = 95f;
                    corpse.itemName = "Orc Corpse";
                    corpse.orcAppetiteModifier = 0.02f;
                    corpse.humanAppetiteModifier = 0.1f;
                    corpse.elfAppetiteModifier = 0.1f;
                    break;
                case (int)SC.CreatureTypes.RODENT:
                    corpse.nutrients = 30f;
                    corpse.weight = 5f;
                    corpse.itemName = "Rodent Carcass";
                    break;
                case (int)SC.CreatureTypes.SNAKE:
                    corpse.nutrients = 25f;
                    corpse.weight = 1.5f;
                    corpse.itemName = "Snake Carcass";
                    break;
                case (int)SC.CreatureTypes.SUCCUBUS:
                    corpse.nutrients = 80f;
                    corpse.weight = 80f;
                    corpse.itemName = "Succubus Carcass";
                    break;
                case (int)SC.CreatureTypes.SLIME:
                    corpse.nutrients = 50f;
                    corpse.weight = 20f;
                    corpse.itemName = "Slime Membrane";
                    break;
                case (int)SC.CreatureTypes.TROLL:
                    corpse.nutrients = 50f;
                    corpse.weight = 60f;
                    corpse.itemName = "Troll Corpse";
                    break;
                case (int)SC.CreatureTypes.CHEF:
                    corpse.nutrients = 90f;
                    corpse.weight = 85f;
                    corpse.itemName = "Human Corpse";
                    corpse.orcAppetiteModifier = 0.1f;
                    corpse.humanAppetiteModifier = 0.02f;
                    corpse.elfAppetiteModifier = 0.1f;
                    break;
                case (int)SC.CreatureTypes.SPIDER:
                    corpse.nutrients = 20f;
                    corpse.weight = 5f;
                    corpse.itemName = "Dead Spider";
                    break;
                default:
                    corpse.nutrients = 90f;
                    corpse.weight = 70f;
                    corpse.itemName = "Unidentified Remains";
                    break;

            }
            corpse.value = 4 * corpse.weight;
            return corpse;
        }

        public Item GenerateRock()
        {
            Item rock = new Item();
            rock.itemType = (int)SC.ItemTypes.ROCK;
            rock.rockType = (int)SC.RockTypes.ROCK;

            float randomChoice = (float)random.NextDouble();
            if (randomChoice < 0.002)
            {
                rock.itemName = "Diamond";
                rock.itemDescription = "Precious Diamond";
                rock.rockType = (int)SC.RockTypes.GEM;
                rock.value = 100000;
                rock.weight = 0.1f;
                return rock;
            }
            if (randomChoice < 0.005)
            {
                rock.itemName = "Emerald";
                rock.itemDescription = "Precious Emerald";
                rock.rockType = (int)SC.RockTypes.GEM;
                rock.value = 30000;
                rock.weight = 0.1f;
                return rock;
            }
            if (randomChoice < 0.050)
            {
                rock.itemName = "Ruby";
                rock.itemDescription = "Precious Ruby";
                rock.rockType = (int)SC.RockTypes.GEM;
                rock.value = 20000;
                rock.weight = 0.1f;
                return rock;
            }
            if (randomChoice < 0.1)
            {
                rock.itemName = "Sapphire";
                rock.itemDescription = "Precious Sapphire";
                rock.rockType = (int)SC.RockTypes.GEM;
                rock.value = 8000;
                rock.weight = 0.1f;
                return rock;
            }
            if (randomChoice < 0.20)
            {
                rock.itemName = "Zircon";
                rock.itemDescription = "Precious Zircon";
                rock.rockType = (int)SC.RockTypes.GEM;
                rock.value = 200;
                rock.weight = 0.1f;
                return rock;
            }
            if (randomChoice < 0.30)
            {
                rock.itemName = "Iron Ore";
                rock.itemDescription = "Lump of Iron Ore";
                rock.rockType = (int)SC.RockTypes.METAL;
                rock.value = 200;
                rock.weight = 6.0f;
                return rock;
            }
            if (randomChoice < 0.35)
            {
                rock.itemName = "Silver Ore";
                rock.itemDescription = "Lump of Precious Silver Ore";
                rock.rockType = (int)SC.RockTypes.METAL;
                rock.value = 400;
                rock.weight = 6.0f;
                return rock;
            }
            if (randomChoice < 0.4)
            {
                rock.itemName = "Gold";
                rock.itemDescription = "Heavy Lump of Precious Gold";
                rock.rockType = (int)SC.RockTypes.METAL;
                rock.value = 2500;
                rock.weight = 12.0f;
                return rock;
            }

            rock.itemName = "Rubble";
            rock.itemDescription = "Loose Rubble";
            rock.rockType = (int)SC.RockTypes.ROCK;
            rock.value = 1;
            rock.weight = 30f;
            return rock;
        }

        //Generate a merchant's inventory of n items. Empty list if less than 2 items.
        //Generate n - 1 items then ensure the n'th item is food.
        public List<Item> MerchantInventory(int value, int numberOfItems)
        {
            List<Item> result = new List<Item>();

            if (numberOfItems < 2) return result;

            for (int i = 0; i < numberOfItems - 1; i++)
            {
                int chosenValue = (int)Math.Ceiling(random.Next((value + MERCHANT_VALUE_BONUS), ((value + MERCHANT_VALUE_BONUS)/10 + 1)*11 + 1));
                result.Add(GenerateShopItem(chosenValue));
            }
            result.Add(GenerateFood());

            return result;
        }

        //Items that the village merchant might sell. Does not sell food!
        public List<Item> VillageMerchantInventory(int value, int numberOfItems)
        {
            List<Item> result = new List<Item>();

            if (numberOfItems < 1) return result;

            for (int i = 0; i < numberOfItems; i++)
            {
                int chosenValue = (int)Math.Ceiling(random.Next((value + MERCHANT_VALUE_BONUS), ((value + MERCHANT_VALUE_BONUS) / 10 + 1) * 11 + 1));
                result.Add(GenerateVillageShopItem(chosenValue));
            }
            return result;
        }

        //Items that a dungeon merchant might sell. Does not sell shields or armour!
        public List<Item> DungeonMerchantInventory(int value, int numberOfItems)
        {
            List<Item> result = new List<Item>();

            if (numberOfItems < 1) return result;

            for (int i = 0; i < numberOfItems; i++)
            {
                int chosenValue = (int)Math.Ceiling(random.Next((value + MERCHANT_VALUE_BONUS), ((value + MERCHANT_VALUE_BONUS) / 10 + 1) * 11 + 1));
                result.Add(GenerateDungeonShopItem(chosenValue));
            }
            return result;
        }

        //Turn one corpse into a list of food items, roughly equal
        //to the corpses weight
        public List<Item> CorpseToFoodItems(Item theCorpse)
        {
            List<Item> result = new List<Item>();
            float weightRemaining = theCorpse.weight;
            while (weightRemaining > 0 && result.Count < MAX_TINS_PER_CORPSE)
            {
                Item tin = GenerateTinnedMeat();
                tin.x = theCorpse.x;
                tin.y = theCorpse.y;
                result.Add(tin);
                weightRemaining -= tin.weight;
            }
            return result;
        }

        public String ImproveItemName(Item aItem, int increase)
        {
            String old = aItem.itemName;

            if (old.ToCharArray()[0] != '+')
            {
                return "+" + increase.ToString() + " " + old;
            }
            else
            {
                char[] numberChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                int indexOfFirstNumberCharacter = old.IndexOfAny(numberChars);
                if (indexOfFirstNumberCharacter == -1) return old;
                int indexOfLastNumberCharacter = indexOfFirstNumberCharacter;
                while (numberChars.Contains<char>(old[indexOfLastNumberCharacter]))
                {
                    indexOfLastNumberCharacter++;
                }
                indexOfLastNumberCharacter--;

                int oldValue = Int32.Parse(old.Substring(indexOfFirstNumberCharacter, 1 + indexOfLastNumberCharacter - indexOfFirstNumberCharacter));
                String oldName = old.Substring(indexOfLastNumberCharacter + 2);

                return "+" + ((int)(oldValue + increase)).ToString() + " " + oldName;
            }

        }

        private String RandomSwordName(int aValue)
        {
            String aName = "";
            int remainder = aValue;
            WeightedWord pickAWord = RandomWord(aValue, swordNames);
            aName = pickAWord.word;
            remainder -= pickAWord.value;
            if (remainder > 0)
            {
                pickAWord = RandomWord(remainder, qualityAdjectives);
                aName = pickAWord.word + " " + aName;
                remainder -= pickAWord.value;
                if (remainder > 0) aName = "+" + remainder.ToString() + " " + aName;
            }
            return aName;
        }

        private String RandomElementalSwordName(int aValue)
        {
            String aName = "";
            int remainder = aValue;
            WeightedWord pickAWord = RandomWord(aValue, swordNames);
            aName = pickAWord.word;
            remainder -= pickAWord.value;
            if (remainder > 0)
            {
                pickAWord = RandomWord(remainder, qualityAdjectives);
                aName = pickAWord.word + " " + aName;
                remainder -= pickAWord.value;
            }
            return aName;
        }

        private String RandomDaggerName(int aValue)
        {
            String aName = "";
            int remainder = aValue;
            WeightedWord pickAWord = RandomWord(aValue, daggerNames);
            aName = pickAWord.word;
            remainder -= pickAWord.value;
            if (remainder > 0)
            {
                pickAWord = RandomWord(remainder, qualityAdjectives);
                aName = pickAWord.word + " " + aName;
                remainder -= pickAWord.value;
                if (remainder > 0) aName = "+" + remainder.ToString() + " " + aName;
            }
            return aName;
        }

        private String RandomMaceName(int aValue)
        {
            String aName = "";
            int remainder = aValue;
            WeightedWord pickAWord = RandomWord(aValue, maceNames);
            aName = pickAWord.word;
            remainder -= pickAWord.value;
            if (remainder > 0)
            {
                pickAWord = RandomWord(remainder, qualityAdjectives);
                aName = pickAWord.word + " " + aName;
                remainder -= pickAWord.value;
                if (remainder > 0) aName = "+" + remainder.ToString() + " " + aName;
            }
            return aName;
        }

        private String RandomSpearName(int aValue)
        {
            String aName = "";
            int remainder = aValue;
            WeightedWord pickAWord = RandomWord(aValue, spearNames);
            aName = pickAWord.word;
            remainder -= pickAWord.value;
            if (remainder > 0)
            {
                pickAWord = RandomWord(remainder, qualityAdjectives);
                aName = pickAWord.word + " " + aName;
                remainder -= pickAWord.value;
                if (remainder > 0) aName = "+" + remainder.ToString() + " " + aName;
            }
            return aName;
        }

        private String RandomBowName(int aValue)
        {
            String aName = "";
            int remainder = aValue;
            WeightedWord pickAWord = RandomWord(aValue, bowNames);
            aName = pickAWord.word;
            remainder -= pickAWord.value;
            if (remainder > 0)
            {
                pickAWord = RandomWord(remainder, qualityAdjectives);
                aName = pickAWord.word + " " + aName;
                remainder -= pickAWord.value;
                if (remainder > 0) aName = "+" + remainder.ToString() + " " + aName;
            }
            return aName;
        }

        private String RandomShieldName(int aValue)
        {
            String aName = "";
            int remainder = aValue;
            WeightedWord ww = RandomWord(remainder, shieldNames);
            aName = ww.word;
            remainder -= ww.value;
            if (remainder > 0)
            {
                ww = RandomWord(remainder, qualityAdjectives);
                aName = ww.word + " " + aName;
                remainder -= ww.value;
            }
            if (remainder > 0) aName = "+" + remainder.ToString() + " " + aName;
            return aName;
        }

        private String RandomArmourName(int aValue)
        {
            String aName = "";
            int remainder = aValue;
            WeightedWord ww = RandomWord(remainder, armourNames);
            aName = ww.word;
            remainder -= ww.value;
            if (remainder > 0)
            {
                ww = RandomWord(remainder, qualityAdjectives);
                aName = ww.word + " " + aName;
                remainder -= ww.value;
            }
            if (remainder > 0) aName = "+" + remainder.ToString() + " " + aName;
            return aName;
        }

        private String RandomCloakName(int aValue)
        {
            String aName = "";
            int remainder = aValue;
            WeightedWord cloakName = RandomWord(remainder, cloakNames);
            aName = cloakName.word;
            remainder -= cloakName.value;
            aName = BestWord(remainder, qualityAdjectives).word + " " + aName;
            remainder -= BestWord(remainder, qualityAdjectives).value;
            //if (remainder > 0) aName = "+" + remainder.ToString() + " " + aName;
            return aName;
        }

        private String GenerateQuestItemName(int aLevel)
        {
            WeightedWord noun = RandomWord(aLevel, questItemNouns);
            int remainder = aLevel - noun.value;
            if (remainder > 0)
            {
                return RandomWord(aLevel, questItemAdjectives).word + " " + noun.word;
            }
            else
            {
                return noun.word;
            }
        }

        private WeightedWord BestWord(int aValue, List<WeightedWord> aWordList)
        {
            if (aWordList == null) return new WeightedWord(aValue, "NullListFound");
            if (aWordList.Count == 0) return new WeightedWord(aValue, "EmptyListFound");
            int indexofBestWord = 0;
            for (int i = 0; i < aWordList.Count; i++)
            {
                if (aWordList[i].value <= aValue)
                {
                    if (aValue - aWordList[i].value < aValue - aWordList[indexofBestWord].value)
                    {
                        indexofBestWord = i;
                    }
                }
            }
            return new WeightedWord(aWordList[indexofBestWord].value, 
                                    aWordList[indexofBestWord].word);
        }

        private WeightedWord RandomWord(int aValue, List<WeightedWord> aWordList)
        {
            if (aWordList == null) return new WeightedWord(aValue, "NullListFound");
            if (aWordList.Count == 0) return new WeightedWord(aValue, "EmptyListFound");
            List<WeightedWord> possibleWords = new List<WeightedWord>();
            for (int i = 0; i < aWordList.Count; i++)
            {
                if (aWordList[i].value <= aValue)
                {
                    possibleWords.Add(aWordList[i]);
                }
            }
            if (possibleWords.Count <= 0) return new WeightedWord(aValue, "EmptyListFound");
            int randomChoice = random.Next(0, possibleWords.Count);
            return new WeightedWord(aWordList[randomChoice].value, aWordList[randomChoice].word);
        }

        private void LoadAdjectives()
        {
            qualityAdjectives.Add(new WeightedWord(1, "Poor"));
            qualityAdjectives.Add(new WeightedWord(3, "Weak"));
            qualityAdjectives.Add(new WeightedWord(4, "Rubbish"));
            qualityAdjectives.Add(new WeightedWord(5, "Defective"));
            qualityAdjectives.Add(new WeightedWord(6, "Inferior"));
            qualityAdjectives.Add(new WeightedWord(7, "Shabby"));
            qualityAdjectives.Add(new WeightedWord(8, "Impaired"));
            qualityAdjectives.Add(new WeightedWord(9, "Marred"));
            qualityAdjectives.Add(new WeightedWord(10, "Lowly"));
            qualityAdjectives.Add(new WeightedWord(11, "Paltry"));
            qualityAdjectives.Add(new WeightedWord(12, "Mediocre"));
            qualityAdjectives.Add(new WeightedWord(13, "Humble"));
            qualityAdjectives.Add(new WeightedWord(14, "Feeble"));
            qualityAdjectives.Add(new WeightedWord(15, "Modest"));
            qualityAdjectives.Add(new WeightedWord(16, "Meager"));
            qualityAdjectives.Add(new WeightedWord(17, "Ordinary"));
            qualityAdjectives.Add(new WeightedWord(18, "Middling"));
            qualityAdjectives.Add(new WeightedWord(19, "Fair"));
            qualityAdjectives.Add(new WeightedWord(20, "Second-Rate"));
            qualityAdjectives.Add(new WeightedWord(21, "Average"));
            qualityAdjectives.Add(new WeightedWord(22, "Common"));
            qualityAdjectives.Add(new WeightedWord(23, "So-So"));
            qualityAdjectives.Add(new WeightedWord(24, "Simple"));
            qualityAdjectives.Add(new WeightedWord(25, "Adequate"));
            qualityAdjectives.Add(new WeightedWord(26, "Moderate"));
            qualityAdjectives.Add(new WeightedWord(27, "Decent"));
            qualityAdjectives.Add(new WeightedWord(28, "Standard"));
            qualityAdjectives.Add(new WeightedWord(29, "Worthy"));
            qualityAdjectives.Add(new WeightedWord(30, "Dependable"));
            qualityAdjectives.Add(new WeightedWord(31, "Solid"));
            qualityAdjectives.Add(new WeightedWord(32, "Dandy"));
            qualityAdjectives.Add(new WeightedWord(33, "Fine"));
            qualityAdjectives.Add(new WeightedWord(34, "Great"));
            qualityAdjectives.Add(new WeightedWord(35, "Superb"));
            qualityAdjectives.Add(new WeightedWord(36, "Remarkable"));
            qualityAdjectives.Add(new WeightedWord(37, "Rare"));
            qualityAdjectives.Add(new WeightedWord(38, "Superior"));
            qualityAdjectives.Add(new WeightedWord(39, "Exceptional"));
            qualityAdjectives.Add(new WeightedWord(40, "Peerless"));
            qualityAdjectives.Add(new WeightedWord(41, "Excellent"));
            qualityAdjectives.Add(new WeightedWord(42, "Magnificent"));
            qualityAdjectives.Add(new WeightedWord(43, "Brilliant"));
            qualityAdjectives.Add(new WeightedWord(44, "Supreme"));
            qualityAdjectives.Add(new WeightedWord(45, "Legendary"));
            qualityAdjectives.Add(new WeightedWord(46, "Ultimate"));
            qualityAdjectives.Add(new WeightedWord(47, "Almighty"));

            fireAdjectives.Add(new WeightedWord(1, "Whisp"));
            fireAdjectives.Add(new WeightedWord(25, "Tinder"));
            fireAdjectives.Add(new WeightedWord(100, "Flame"));
            fireAdjectives.Add(new WeightedWord(500, "Burning"));
            fireAdjectives.Add(new WeightedWord(1000, "Fiery"));
            fireAdjectives.Add(new WeightedWord(3000, "Blaze"));
            fireAdjectives.Add(new WeightedWord(6000, "Inferno"));
            fireAdjectives.Add(new WeightedWord(8000, "Immolation"));

            iceAdjectives.Add(new WeightedWord(1, "Cool"));
            iceAdjectives.Add(new WeightedWord(25, "Chilled"));
            iceAdjectives.Add(new WeightedWord(100, "Icey"));
            iceAdjectives.Add(new WeightedWord(500, "Cold"));
            iceAdjectives.Add(new WeightedWord(1000, "Frosty"));
            iceAdjectives.Add(new WeightedWord(5000, "Winter"));
            iceAdjectives.Add(new WeightedWord(9000, "Blizzard"));

            swordNames.Add(new WeightedWord(1, "Training Sword"));
            swordNames.Add(new WeightedWord(5, "Short Sword"));
            swordNames.Add(new WeightedWord(20, "Bastard Sword"));
            swordNames.Add(new WeightedWord(40, "Infantry Sword"));
            swordNames.Add(new WeightedWord(80, "Cutlass"));
            swordNames.Add(new WeightedWord(150, "Rapier"));
            swordNames.Add(new WeightedWord(250, "Scimitar"));
            swordNames.Add(new WeightedWord(350, "Hunter Sword"));
            swordNames.Add(new WeightedWord(400, "Saber"));
            swordNames.Add(new WeightedWord(500, "Gladius"));
            swordNames.Add(new WeightedWord(1000, "Tulwar"));
            swordNames.Add(new WeightedWord(2000, "Knight's Sword"));
            swordNames.Add(new WeightedWord(3000, "Katana"));
            swordNames.Add(new WeightedWord(4000, "Wind Sword"));
            swordNames.Add(new WeightedWord(5000, "Nodachi"));
            swordNames.Add(new WeightedWord(6000, "Demon Blade"));
            swordNames.Add(new WeightedWord(7000, "Void Foil"));
            swordNames.Add(new WeightedWord(8000, "Lord's Blade"));

            daggerNames.Add(new WeightedWord(1, "Basic Knife"));
            daggerNames.Add(new WeightedWord(5, "Dagger"));
            daggerNames.Add(new WeightedWord(20, "Combat Knife"));
            daggerNames.Add(new WeightedWord(40, "Fine Dagger"));
            daggerNames.Add(new WeightedWord(80, "Machete"));
            daggerNames.Add(new WeightedWord(150, "Assassins Blade"));
            daggerNames.Add(new WeightedWord(250, "Baselard"));
            daggerNames.Add(new WeightedWord(350, "Tanto"));
            daggerNames.Add(new WeightedWord(500, "Dirk"));
            daggerNames.Add(new WeightedWord(1000, "Misericorde"));
            daggerNames.Add(new WeightedWord(1500, "Jackblade"));
            daggerNames.Add(new WeightedWord(2000, "Main-Gauche"));
            daggerNames.Add(new WeightedWord(3000, "Stiletto"));
            daggerNames.Add(new WeightedWord(4000, "Hapkido Dagger"));
            daggerNames.Add(new WeightedWord(5000, "Khukri"));
            daggerNames.Add(new WeightedWord(6000, "Pugio Dagger"));
            daggerNames.Add(new WeightedWord(7000, "Holbein Blade"));
            daggerNames.Add(new WeightedWord(8000, "Ultimate Dagger"));

            maceNames.Add(new WeightedWord(1, "Rusty Mace"));
            maceNames.Add(new WeightedWord(5, "Cudgel"));
            maceNames.Add(new WeightedWord(20, "Mace"));
            maceNames.Add(new WeightedWord(40, "Club"));
            maceNames.Add(new WeightedWord(90, "Morning Star"));
            maceNames.Add(new WeightedWord(200, "Flail"));
            maceNames.Add(new WeightedWord(500, "Spiked Mace"));
            maceNames.Add(new WeightedWord(1000, "Nunchaku"));
            maceNames.Add(new WeightedWord(2000, "Chain Sickle"));
            maceNames.Add(new WeightedWord(3000, "Monk's Mace")); 
            maceNames.Add(new WeightedWord(4000, "Kusarigama"));
            maceNames.Add(new WeightedWord(6000, "Sansetsukon"));
            maceNames.Add(new WeightedWord(8000, "Lord's Mace"));

            spearNames.Add(new WeightedWord(1, "Bamboo Spear"));
            spearNames.Add(new WeightedWord(10, "Fishing Spear"));
            spearNames.Add(new WeightedWord(30, "Spear"));
            spearNames.Add(new WeightedWord(60, "Troglodyte Spear"));
            spearNames.Add(new WeightedWord(100, "Infantry Spear"));
            spearNames.Add(new WeightedWord(250, "Halberd"));
            spearNames.Add(new WeightedWord(500, "Pole-Arm"));
            spearNames.Add(new WeightedWord(1000, "Pike"));
            spearNames.Add(new WeightedWord(1500, "Pila"));
            spearNames.Add(new WeightedWord(2000, "Lance"));
            spearNames.Add(new WeightedWord(3000, "Iklwa"));
            spearNames.Add(new WeightedWord(4000, "Naginata"));
            spearNames.Add(new WeightedWord(5000, "Obsidian Spear"));
            spearNames.Add(new WeightedWord(6000, "Spear Of Lugh"));
            spearNames.Add(new WeightedWord(7000, "Holy Lance"));
            spearNames.Add(new WeightedWord(8000, "Gungnir"));

            bowNames.Add(new WeightedWord(1, "Practice Bow"));
            bowNames.Add(new WeightedWord(25, "Bow"));
            bowNames.Add(new WeightedWord(50, "Longbow"));
            bowNames.Add(new WeightedWord(100, "Compound Bow"));
            bowNames.Add(new WeightedWord(250, "Hunting Bow"));
            bowNames.Add(new WeightedWord(500, "Yew Longbow"));
            bowNames.Add(new WeightedWord(1000, "Composite Bow"));
            bowNames.Add(new WeightedWord(1500, "Holmegaard Bow"));
            bowNames.Add(new WeightedWord(2000, "Recurve"));
            bowNames.Add(new WeightedWord(3000, "Yew Longbow"));
            bowNames.Add(new WeightedWord(4000, "Elven Bow"));
            bowNames.Add(new WeightedWord(5000, "Yumi"));
            bowNames.Add(new WeightedWord(6000, "Sniping Bow"));
            bowNames.Add(new WeightedWord(7000, "Hell Bow"));
            bowNames.Add(new WeightedWord(8000, "Balore's Longbow"));

            shieldNames.Add(new WeightedWord(1, "Training Shield"));
            shieldNames.Add(new WeightedWord(50, "Small Shield"));
            shieldNames.Add(new WeightedWord(200, "Round Shield"));
            shieldNames.Add(new WeightedWord(350, "Buckler"));
            shieldNames.Add(new WeightedWord(500, "Targe"));
            shieldNames.Add(new WeightedWord(650, "Infantry Shield"));
            shieldNames.Add(new WeightedWord(800, "Yetholm Shield"));
            shieldNames.Add(new WeightedWord(900, "Heater Shield"));
            shieldNames.Add(new WeightedWord(1000, "Kite Shield"));
            shieldNames.Add(new WeightedWord(1100, "Pavise"));
            shieldNames.Add(new WeightedWord(1200, "Knight's Shield"));
            shieldNames.Add(new WeightedWord(1300, "Dragon Shield"));
            shieldNames.Add(new WeightedWord(1400, "Aegis"));
            shieldNames.Add(new WeightedWord(1500, "Lord's Shield"));

            armourNames.Add(new WeightedWord(0, "Leather Cuirass"));
            armourNames.Add(new WeightedWord(50, "Iron Cuirass"));
            armourNames.Add(new WeightedWord(100, "Bronze Cuirass"));
            armourNames.Add(new WeightedWord(200, "Steel Cuirass"));
            armourNames.Add(new WeightedWord(300, "Dwarven Armour"));
            armourNames.Add(new WeightedWord(400, "Chainmail Armour"));
            armourNames.Add(new WeightedWord(500, "Splintmail Armour"));
            armourNames.Add(new WeightedWord(600, "Elven Chainmail"));
            armourNames.Add(new WeightedWord(700, "Plate Armour"));
            armourNames.Add(new WeightedWord(800, "Carburised Cuirass"));
            armourNames.Add(new WeightedWord(900, "Dwarven Plate"));
            armourNames.Add(new WeightedWord(1000, "Mycenaean Plate"));
            armourNames.Add(new WeightedWord(1100, "Royal Chainmail"));
            armourNames.Add(new WeightedWord(1200, "Heavenly Plate"));
            armourNames.Add(new WeightedWord(1300, "Daemon Armour"));
            armourNames.Add(new WeightedWord(1400, "Dragonscale"));
            armourNames.Add(new WeightedWord(1500, "Lord's Armour"));

            cloakNames.Add(new WeightedWord(0, "Cloak"));
            cloakNames.Add(new WeightedWord(50, "Waxed Cloak"));
            cloakNames.Add(new WeightedWord(100, "Scout Cloak"));
            cloakNames.Add(new WeightedWord(200, "Elven Cloak"));
            cloakNames.Add(new WeightedWord(300, "Star Cloak"));
            cloakNames.Add(new WeightedWord(400, "Dragonhide Cape"));
            cloakNames.Add(new WeightedWord(500, "Lord's Robe"));

            questItemNouns.Add(new WeightedWord(0, "Bucket"));
            questItemNouns.Add(new WeightedWord(1, "Fork"));
            questItemNouns.Add(new WeightedWord(2, "Rock"));
            questItemNouns.Add(new WeightedWord(3, "Spoon"));
            questItemNouns.Add(new WeightedWord(4, "Scroll"));
            questItemNouns.Add(new WeightedWord(6, "Flute"));
            questItemNouns.Add(new WeightedWord(8, "Necklace"));
            questItemNouns.Add(new WeightedWord(12, "Fleece"));
            questItemNouns.Add(new WeightedWord(16, "Tome"));
            questItemNouns.Add(new WeightedWord(32, "Orb"));
            questItemNouns.Add(new WeightedWord(64, "Staff"));
            questItemNouns.Add(new WeightedWord(128, "Pocketwatch"));
            questItemNouns.Add(new WeightedWord(192, "Locket"));
            questItemNouns.Add(new WeightedWord(256, "Ring"));
            questItemNouns.Add(new WeightedWord(512, "Relic"));
            questItemNouns.Add(new WeightedWord(1024, "Crown"));
            questItemNouns.Add(new WeightedWord(2048, "Ark"));

            questItemAdjectives.Add(new WeightedWord(0, "Humble"));
            questItemAdjectives.Add(new WeightedWord(1, "Village"));
            questItemAdjectives.Add(new WeightedWord(2, "Personal"));
            questItemAdjectives.Add(new WeightedWord(4, "Heirloom"));
            questItemAdjectives.Add(new WeightedWord(8, "True"));
            questItemAdjectives.Add(new WeightedWord(16, "Virtuous"));
            questItemAdjectives.Add(new WeightedWord(32, "Holy"));
            questItemAdjectives.Add(new WeightedWord(64, "Sacred"));
        }
    }
}