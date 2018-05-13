using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amadues
{
    public class CreatureDescription
    {
        public int creatureType = 0;

        public String name;
        public String description;
        public String prefix;
        public bool isHostile = true;
        public bool attacksWhenAllied = true;
        public int startingLevel = 1;
        public double experience = 5;
        public double MAX_HEALTH = 50;
        public double attackBonus = 0;
        public double defenceBonus = 0;
        public int width = 1;
        public int height = 1;
        public float displaySize = 1.0f;
        public bool poisonous = false;
        public bool floats = false;
        public double elementDefenceFire = 1;
        public double elementDefenceIce = 1;
        public double elementOffenceFire = 0;
        public double elementOffenceIce = 0;

        //The simple constructor
        public CreatureDescription()
        {
            name = "UNINITIALIZED NAME";
            description = "UNINITIALIZED DESC";
            prefix = "A";
        }

        //The long winded constructor
        public CreatureDescription(String aName, String aDesc, 
                                    int aType, bool hostile, 
                                    int aLevel, double aExp, double aMaxHealth)
        {
            name = aName;
            description = aDesc;
            creatureType = aType;
            isHostile = hostile;
            startingLevel = aLevel;
            experience = aExp;
            MAX_HEALTH = aMaxHealth;
            prefix = "A";
        }

        public CreatureDescription(String aName, String aDesc,
                                    int aType, bool hostile,
                                    int aLevel, double aExp, double aMaxHealth, double attBonus, double defBonus)
        {
            name = aName;
            description = aDesc;
            creatureType = aType;
            isHostile = hostile;
            startingLevel = aLevel;
            experience = aExp;
            MAX_HEALTH = aMaxHealth;
            attackBonus = attBonus;
            defenceBonus = defBonus;
            prefix = "A";
        }

        public CreatureDescription(String aName, String aDesc, String aPre,
                                    int aType, bool hostile, bool pois, bool attWhenAllied,
                                    int aLevel, double aExp, double aMaxHealth, 
                                    double attBonus, double defBonus, double eoFire, double eoIce, double edFire, double edIce, 
                                    int aWidth, int aHeight, float aDisplaySize)
        {
            name = aName;
            description = aDesc;
            prefix = aPre;
            creatureType = aType;
            isHostile = hostile;
            startingLevel = aLevel;
            experience = aExp;
            MAX_HEALTH = aMaxHealth;
            attackBonus = attBonus;
            defenceBonus = defBonus;
            width = aWidth;
            height = aHeight;
            displaySize = aDisplaySize;

            //prefix = "A";
            poisonous = pois;
            attacksWhenAllied = attWhenAllied;
            elementDefenceFire = edFire;
            elementDefenceIce = edIce;
            elementOffenceFire = eoFire;
            elementOffenceIce = eoIce;

        }

        public CreatureDescription Clone()
        {
            return new CreatureDescription(this.name, this.description, this.prefix, this.creatureType, 
                this.isHostile, this.poisonous, this.attacksWhenAllied, this.startingLevel, this.experience, 
                this.MAX_HEALTH, this.attackBonus, this.defenceBonus, 
                this.elementOffenceFire, this.elementOffenceIce, this.elementDefenceFire, this.elementDefenceIce,  
                this.width, this.height, this.displaySize);
        }

    }
}