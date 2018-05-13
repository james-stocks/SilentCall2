using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amadues
{

    public class Item
    {
        public int itemType;
        public String itemName;
        public String itemDescription;
        public float weight = 0f;
        public double value = 0;
        public int x = 0;
        public int y = 0;
        public bool forSale = false;
        public byte visibility = 0;

        public bool isStackable;
        public int stackCount = 1;

        //Track if this item was part of generated inventory.
        //If so - do not allow to be saved
        public bool loadOut = false;

        //Weapon properties
        public int damage;
        public int range = 1;
        public double elementalOffenceFire = 0;
        public double elementalOffenceIce = 0;
        public int weaponType = (int)SC.WeaponTypes.SWORD;

        //Armour properties
        public int defence = 1;
        public double elementalDefenceFire = 1;
        public double elementalDefenceIce = 1;

        //Food properties
        public float nutrients = 1f;
        public float quality = 1f;
        public float decay = 0.03f;
        public bool rotten = false;
        public int foodType = (int)SC.FoodTypes.BREAD;
        public bool canned = false;
        public float humanAppetiteModifier = 1.0f;
        public float orcAppetiteModifier = 1.0f;
        public float elfAppetiteModifier = 1.0f;
        public Creature deadCreature; //Creature that spawned the corpse

        //Rock properties
        public int rockType = (int)SC.RockTypes.ROCK;

        public Item()
        {
            itemType = (int)SC.ItemTypes.GOLD;
            value = 1;
            itemName = "Gold Coin";
            itemDescription = "A Gold Coin";
        }

        public bool Equals(Item otherItem)
        {
            return itemName == otherItem.itemName
                && itemType == otherItem.itemType
                && itemDescription == otherItem.itemDescription
                && value == otherItem.value;
        }
    }
}