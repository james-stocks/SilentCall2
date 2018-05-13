using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Amadues
{
    //[Serializable]
    public class Player
    {
        const float METABOLISM_ORC = 0.08f;
        const float METABOLISM_HUMAN = 0.1f;
        const float METABOLISM_ELF = 0.12f;
        const double STARVATION = 5; //Percentage of total HP to remove each turn

        Random random;

        public int x, y = 0;
        public Texture2D texture;
        public String name;

        public int race = (int)SC.Races.HUMAN;
        public int profession = (int)SC.Professions.FIGHTER;

        public double experience = 0;
        public double next = 10;
        public double totalExpToNextLevel = 10;
        public double totalExpToLastLevel = 0;
        public int level = 1;
        public const int MAX_LEVEL = 9999;
        public double MAX_GOLD = 999999999999;
        public int MAX_ITEMS = 200;
        public double gold = 0;

        public bool alive = true;
        public double MAX_HEALTH = 50;
        public double MAX_MAGIK = 10;
        public double currentHealth = 50;
        public double currentMagik = 10;
        public float full_stomach = 100f;
        public float hunger = 100f;
        public float metabolism = METABOLISM_HUMAN;
        public float carryingCapacity = 150f;
        public float totalEncumberance = 170f;
        public bool totallyEncumbered = false;
        public float currentCarriedWeight;
        public int numberOfItems = 0;
        public List<Item> carriedItems;

        public int directionFacing = SC.DIRECTION_DOWN;

        public double attackPower = 1;
        public double defencePower = 1;
        public int intelligence = 10;
        public int strength = 10;
        public int perception = 10;

        public Item equippedSword;
        public Item equippedShield;
        public Item equippedArmour;
        public Item equippedCloak;

        public SpellCompendium spells;

        //Timers
        public int TurnsToRestore1Magik = 4;
        public int CountToRestore1Magik = 0;
        public int TurnsToRestore1Health = 12;
        public int CountToRestore1Health = 0;

        //Statuses
        public bool poisoned = false;
        public double poisonDamage;
        public double poisonDuration;
        public bool deadFromPoison = false;

        public double attackBuff = 0;
        public int attackBuffTurnsLeft = 0;
        //public bool deadFromArrowTrap = false;

        public bool starving = false;
        public bool deadFromStarvation = false;

        public bool killedBySelf = false;

        public bool AddExperience(double aEx)
        {
            if (level < MAX_LEVEL)
            {
                if (aEx >= next)
                {
                    double lastNext = next;
                    experience += next;
                    LevelUp();
                    AddExperience(aEx - lastNext);
                    return true;
                }
                else
                {
                    experience += aEx;
                    next -= aEx;
                    return false;
                }
            }
            return false;
        }

        //Damage the player
        //Return whether the attack was deadly
        public bool Damage(double damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                alive = false;
                return true;
            }
            return false;
        }

        public void LimitedDamage(double damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                currentHealth = 1;
            }
        }

        public bool Heal(double healing)
        {
            currentHealth += healing;
            if (currentHealth >= MAX_HEALTH)
            {
                currentHealth = MAX_HEALTH;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RestoreMagik(double restore)
        {
            currentMagik += restore;
            if (currentMagik >= MAX_MAGIK)
            {
                currentMagik = MAX_MAGIK;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool GainNutrients(float nutrients)
        {
            hunger = Math.Min(full_stomach, hunger + nutrients);
            if (hunger > 0) starving = false;
            if (hunger == full_stomach) return true;
            return false;
        }

        public void ReduceStomachTo10Percent()
        {
            if (hunger >= full_stomach / 10)
            {
                hunger = full_stomach / 10;
            }
        }

        public double AttackPower()
        {
            if (equippedSword == null)
            {
                return attackPower + attackBuff;
            }
            else
            {
                return attackPower + attackBuff + equippedSword.damage;
            }
        }

        public double UnbuffedAttackPower()
        {
            if (equippedSword == null)
            {
                return attackPower;
            }
            else
            {
                return attackPower + equippedSword.damage;
            }
        }

        public double AttackPower(double fireDefence, double iceDefence, double normalDefence)
        {
            double result = Math.Max(1,attackPower + attackBuff + equippedSword.damage - normalDefence);
            if (equippedSword == null)
            {
                return Math.Max(1,attackPower + attackBuff - normalDefence);
            }
            else
            {
                if (equippedSword.elementalOffenceFire > 0)
                {
                    result += Math.Ceiling(equippedSword.elementalOffenceFire * fireDefence);
                }
                if (equippedSword.elementalOffenceIce > 0)
                {
                    result += Math.Ceiling(equippedSword.elementalOffenceIce * iceDefence);
                }
                return result;
                /*double result = attackPower + equippedSword.damage 
                    + equippedSword.damage * equippedSword.elementalOffenceFire * fireDefence
                    + equippedSword.damage * equippedSword.elementalOffenceIce * iceDefence;

                return result;*/
            }
        }

        public double DefencePower()
        {
            double result = defencePower;
            if (equippedArmour != null) result += equippedArmour.defence;
            if (equippedShield != null) result += equippedShield.defence;
            if (equippedCloak != null) result += equippedCloak.defence;

            return result;
        }

        public double IceDefence()
        {
            double result = Math.Min(equippedArmour.elementalDefenceIce, equippedCloak.elementalDefenceIce);
            result = Math.Min(result, equippedShield.elementalDefenceIce);
            return result;
        }

        public double FireDefence()
        {
            double result = Math.Min(equippedArmour.elementalDefenceFire, equippedCloak.elementalDefenceFire);
            result = Math.Min(result, equippedShield.elementalDefenceFire);
            return result;
        }

        private void LevelUp()
        {
            level++;
            double healthIncrease = 0;
            double magikIncrease = 0;
            switch (profession)
            {
                case (int)SC.Professions.FIGHTER:
                    healthIncrease = 2 + (double)(random.Next(2, 3));
                    currentHealth += healthIncrease;
                    MAX_HEALTH += healthIncrease;
                    magikIncrease = 1; //random.Next((int)intelligence / 10, (int)intelligence / 10 + 1);
                    currentMagik += magikIncrease;
                    MAX_MAGIK += magikIncrease;
                    attackPower += 1;
                    if (level % 10 == 0) attackPower += 1;
                    if (level % 10 == 1) defencePower += 1;
                    break;

                case (int)SC.Professions.MAGE:
                    healthIncrease = 1 + (double)(random.Next(1, 2));
                    currentHealth += healthIncrease;
                    MAX_HEALTH += healthIncrease;
                    //magikIncrease = Math.Floor(level/2) + random.Next((int)intelligence / 5, (int)intelligence / 5 + 5);
                    magikIncrease = Math.Ceiling(Math.Log((double)level)) + random.Next((int)intelligence / 5, (int)intelligence / 5 + 3);
                    currentMagik += magikIncrease;
                    MAX_MAGIK += magikIncrease;
                    //if (level % 20 == 0) attackPower += 1;
                    //if (level % 20 == 0) defencePower += 1;
                    break;

                case (int)SC.Professions.EXPLORER:
                    healthIncrease = 2 + (double)(random.Next(1, 2));
                    currentHealth += healthIncrease;
                    MAX_HEALTH += healthIncrease;
                    magikIncrease = 1; // random.Next((int)intelligence / 10, (int)intelligence / 10 + 1);
                    currentMagik += magikIncrease;
                    MAX_MAGIK += magikIncrease;
                    if (level % 20 == 0) attackPower += 1;
                    if (level % 20 == 0) defencePower += 1;
                    break;
            }

            next = 10 * level;
            totalExpToLastLevel = totalExpToNextLevel;
            totalExpToNextLevel = experience + next;
            spells.UnlockSpells(this);
        }

        //TODO: Make initial values only if currentLevel ==1
        //then add an else for increased values for higher levels
        public void SetProfession(int aProfession)
        {
            switch (aProfession)
            {
                case (int)SC.Professions.FIGHTER:
                    if (profession == (int)SC.Professions.EXPLORER)
                    {
                        carryingCapacity -= 20f;
                        totalEncumberance -= 20f;
                    }
                    if (profession == (int)SC.Professions.MAGE)
                    {
                        carryingCapacity += 20f;
                        totalEncumberance += 20f;
                    }
                    profession = aProfession;
                    intelligence = 5;
                    strength = 15;
                    perception = 10;
                    TurnsToRestore1Magik = 8;
                    TurnsToRestore1Health = 6;
                    break;
                case (int)SC.Professions.MAGE:
                    if (profession == (int)SC.Professions.EXPLORER)
                    {
                        carryingCapacity -= 40f;
                        totalEncumberance -= 40f;
                    }
                    if (profession == (int)SC.Professions.FIGHTER)
                    {
                        carryingCapacity -= 20f;
                        totalEncumberance -= 20f;
                    }
                    profession = aProfession;
                    intelligence = 15;
                    strength = 5;
                    perception = 10;
                    TurnsToRestore1Magik = 3;
                    TurnsToRestore1Health = 15;
                    break;
                case (int)SC.Professions.EXPLORER:
                    if (profession == (int)SC.Professions.MAGE)
                    {
                        carryingCapacity += 40f;
                        totalEncumberance += 40f;
                    }
                    if (profession == (int)SC.Professions.FIGHTER)
                    {
                        carryingCapacity += 20f;
                        totalEncumberance += 20f;
                    }
                    profession = aProfession;
                    intelligence = 8;
                    strength = 8;
                    perception = 15;
                    TurnsToRestore1Magik = 8;
                    TurnsToRestore1Health = 10;
                    break;
            }
        }

        //TODO: Make initial values only if currentLevel ==1
        //then add an else for increased values for higher levels
        public void SetRace(int aRace)
        {
            switch (aRace)
            {
                case (int)SC.Races.ELF:
                    if (race == (int)SC.Races.HUMAN)
                    {
                        perception += 2;
                        strength -= 2;
                        carryingCapacity -= 20f;
                        totalEncumberance -= 20f;
                    }
                    if (race == (int)SC.Races.ORC)
                    {
                        perception += 4;
                        strength -= 4;
                        intelligence += 2;
                        carryingCapacity -= 40f;
                        totalEncumberance -= 40f;
                    }
                    race = aRace;
                    metabolism = 0.12f;
                    break;
                case (int)SC.Races.HUMAN:
                    if (race == (int)SC.Races.ELF)
                    {
                        perception -= 2;
                        strength += 2;
                        carryingCapacity += 20f;
                        totalEncumberance += 20f;
                    }
                    if (race == (int)SC.Races.ORC)
                    {
                        strength -= 2;
                        intelligence += 2;
                        perception += 2;
                        carryingCapacity -= 20f;
                        totalEncumberance -= 20f;
                    }
                    race = aRace;
                    metabolism = 0.10f;
                    break;
                case (int)SC.Races.ORC:
                    if (race == (int)SC.Races.ELF)
                    {
                        perception -= 4;
                        strength += 4;
                        intelligence -= 2;
                        carryingCapacity += 40f;
                        totalEncumberance += 40f;
                    }
                    if (race == (int)SC.Races.HUMAN)
                    {
                        intelligence -= 2;
                        strength += 2;
                        perception -= 2;
                        carryingCapacity += 20f;
                        totalEncumberance += 20f;
                    }
                    race = aRace;
                    metabolism = 0.08f;
                    break;
            }
        }

        public bool AddItem(Item aItem)
        {
            carriedItems.Add(aItem);
            numberOfItems++;
            currentCarriedWeight += aItem.weight;
            totallyEncumbered = currentCarriedWeight > totalEncumberance;
            return totallyEncumbered;
        }

        public bool DropItem(Item theItem)
        {
            int aIndex = -1;
            for (int i = 0; i < carriedItems.Count; i++)
            {
                if (carriedItems[i] == theItem)
                {
                    aIndex = i;
                    break;
                }
            }
            if (aIndex == -1) return false;
            currentCarriedWeight -= carriedItems[aIndex].weight;
            numberOfItems--;
            carriedItems.RemoveAt(aIndex);
            totallyEncumbered = currentCarriedWeight > totalEncumberance;
            return totallyEncumbered;
        }

        public void RemoveQuestItemsFromInventory()
        {
            if (carriedItems.Count > 0)
            {
                for (int i = carriedItems.Count - 1; i >= 0; i--)
                {
                    if (carriedItems[i].itemType == (int)SC.ItemTypes.QUEST)
                    {
                        DropItem(carriedItems[i]);
                    }
                }
            }
        }

        //Equip an item from inventory
        //Return false if it cannot be equipped
        public bool EquipItem(Item theItem)
        {
            if (theItem == null) return false;
            int aIndex = -1;
            for (int i = 0; i < carriedItems.Count; i++)
            {
                if (carriedItems[i] == theItem)
                {
                    aIndex = i;
                    break;
                }
            }
            if (aIndex == -1) return false;
            switch (carriedItems[aIndex].itemType)
            {
                case (int)SC.ItemTypes.ARMOUR:
                    currentCarriedWeight -= carriedItems[aIndex].weight;
                    currentCarriedWeight += equippedArmour.weight;
                    carriedItems.Add(equippedArmour);
                    equippedArmour = carriedItems[aIndex];
                    carriedItems.RemoveAt(aIndex);
                    break;
                case (int)SC.ItemTypes.SHIELD:
                    currentCarriedWeight -= carriedItems[aIndex].weight;
                    currentCarriedWeight += equippedShield.weight;
                    carriedItems.Add(equippedShield);
                    equippedShield = carriedItems[aIndex];
                    carriedItems.RemoveAt(aIndex);
                    break;
                case (int)SC.ItemTypes.WEAPON:
                    currentCarriedWeight -= carriedItems[aIndex].weight;
                    currentCarriedWeight += equippedSword.weight;
                    carriedItems.Add(equippedSword);
                    equippedSword = carriedItems[aIndex];
                    carriedItems.RemoveAt(aIndex);
                    break;
                case (int)SC.ItemTypes.CLOAK:
                    currentCarriedWeight -= carriedItems[aIndex].weight;
                    currentCarriedWeight += equippedCloak.weight;
                    carriedItems.Add(equippedCloak);
                    equippedCloak = carriedItems[aIndex];
                    carriedItems.RemoveAt(aIndex);
                    break;
                default:
                    return false;
            }
            return true;
        }

        public bool CanEquip(Item item)
        {
            if (item == null) return false;
            switch (item.itemType)
            {
                case (int)SC.ItemTypes.ARMOUR:
                    return true;
                case (int)SC.ItemTypes.CLOAK:
                    return true;
                case (int)SC.ItemTypes.SHIELD:
                    return true;
                case (int)SC.ItemTypes.WEAPON:
                    if (item.weaponType == (int)SC.WeaponTypes.SWORD && profession == (int)SC.Professions.MAGE) return false;
                    if (item.weaponType == (int)SC.WeaponTypes.MACE && profession == (int)SC.Professions.MAGE) return false;
                    if (item.weaponType == (int)SC.WeaponTypes.MACE && profession == (int)SC.Professions.EXPLORER) return false;
                    if (item.weaponType == (int)SC.WeaponTypes.SPEAR && profession == (int)SC.Professions.MAGE) return false;
                    if (item.weaponType == (int)SC.WeaponTypes.SPEAR && profession == (int)SC.Professions.EXPLORER) return false;
                    if (item.weaponType == (int)SC.WeaponTypes.BOW && profession == (int)SC.Professions.MAGE) return false;
                    if (item.weaponType == (int)SC.WeaponTypes.BOW && profession == (int)SC.Professions.FIGHTER) return false;
                    return true;
                default:
                    return false;
            }
        }

        public bool WillBeAbleToCarry(Item item)
        {
            if (carriedItems.Count >= MAX_ITEMS) return false;
            if (item.weight + currentCarriedWeight > totalEncumberance) return false;
            return true;
        }

        public bool WillBeAbleToCast(String spellName)
        {
            int spellIndex = spells.GetIndexOfKnownSpell(spellName);
            if (spellIndex < 0) return false;
            return (currentMagik >= spells.GetKnownSpells()[spellIndex].castingCost);
        }

        public bool CanBeResurrected()
        {
            if (carriedItems.Count > 0)
            {
                for (int i = 0; i < carriedItems.Count; i++)
                {
                    if (carriedItems[i].itemName == "Life Scroll") return true;
                }
            }
            return false;
        }

        public void Resurrect()
        {
            alive = true;
            currentHealth = Math.Ceiling(MAX_HEALTH / 2);
            if (hunger < full_stomach / 4)
            {
                GainNutrients((float)Math.Ceiling(full_stomach / 4));
            }
            starving = false;
            poisonDuration = 0;
            poisonDamage = 0;
            poisoned = false;

            attackBuff = 0;
            attackBuffTurnsLeft = 0;

            if (carriedItems.Count > 0)
            {
                for (int i = 0; i < carriedItems.Count; i++)
                {
                    if (carriedItems[i].itemName == "Life Scroll")
                    {
                        DropItem(carriedItems[i]);
                        break;
                    }
                }
            }
        }

        public void AddGold(double amount)
        {
            if (gold + amount > MAX_GOLD)
            {
                gold = MAX_GOLD;
            }
            else
            {
                gold = gold + amount;
            }
        }

        public void RemoveGold(double amount)
        {
            gold -= amount;
            if (gold < 0) gold = 0;
        }

        public void HealthBonus(int percent)
        {
            double bonusAmount = Math.Ceiling((MAX_HEALTH / 100) * percent);
            MAX_HEALTH += bonusAmount;
            currentHealth += bonusAmount;
        }

        public void MagikBonus(int percent)
        {
            double bonusAmount = Math.Ceiling((MAX_MAGIK / 100) * percent);
            MAX_MAGIK += bonusAmount;
            currentMagik += bonusAmount;
        }

        public double CompareSword(Item aSword)
        {
            if (equippedSword == null)
            {
                if (aSword == null)
                {
                    return 0;
                }
                else
                {
                    return aSword.damage;
                }
            }
            else
            {
                if (aSword == null)
                {
                    return equippedSword.damage;
                }
                else
                {
                    return aSword.damage - equippedSword.damage;
                }
            }
        }

        public double CompareSwordFire(Item aSword)
        {
            if (equippedSword == null)
            {
                if (aSword == null)
                {
                    return 0;
                }
                else
                {
                    return aSword.elementalOffenceFire;
                }
            }
            else
            {
                if (aSword == null)
                {
                    return equippedSword.elementalOffenceFire;
                }
                else
                {
                    return aSword.elementalOffenceFire - equippedSword.elementalOffenceFire;
                }
            }
        }

        public double CompareSwordIce(Item aSword)
        {
            if (equippedSword == null)
            {
                if (aSword == null)
                {
                    return 0;
                }
                else
                {
                    return aSword.elementalOffenceIce;
                }
            }
            else
            {
                if (aSword == null)
                {
                    return equippedSword.elementalOffenceIce;
                }
                else
                {
                    return aSword.elementalOffenceIce - equippedSword.elementalOffenceIce;
                }
            }
        }

        public double CompareShield(Item aShield)
        {
            if (equippedShield == null)
            {
                if (aShield == null)
                {
                    return 0;
                }
                else
                {
                    return aShield.defence;
                }
            }
            else
            {
                if (aShield == null)
                {
                    return equippedShield.defence;
                }
                else
                {
                    return aShield.defence - equippedShield.defence;
                }
            }
        }

        public double CompareArmour(Item aArmour)
        {
            if (equippedArmour == null)
            {
                if (aArmour == null)
                {
                    return 0;
                }
                else
                {
                    return aArmour.defence;
                }
            }
            else
            {
                if (aArmour == null)
                {
                    return equippedArmour.defence;
                }
                else
                {
                    return aArmour.defence - equippedArmour.defence;
                }
            }
        }

        public double CompareCloak(Item aCloak)
        {
            if (equippedCloak == null)
            {
                if (aCloak == null)
                {
                    return 0;
                }
                else
                {
                    return aCloak.defence;
                }
            }
            else
            {
                if (aCloak == null)
                {
                    return equippedCloak.defence;
                }
                else
                {
                    return aCloak.defence - equippedCloak.defence;
                }
            }
        }

        public double CompareCloakFire(Item aCloak)
        {
            if (equippedCloak == null)
            {
                if (aCloak == null)
                {
                    return 0;
                }
                else
                {
                    return aCloak.elementalDefenceFire;
                }
            }
            else
            {
                if (aCloak == null)
                {
                    return -1 * equippedCloak.elementalDefenceFire;
                }
                else
                {
                    return aCloak.elementalDefenceFire - equippedCloak.elementalDefenceFire;
                }
            }
        }

        public double CompareCloakIce(Item aCloak)
        {
            if (equippedCloak == null)
            {
                if (aCloak == null)
                {
                    return 0;
                }
                else
                {
                    return aCloak.elementalDefenceIce;
                }
            }
            else
            {
                if (aCloak == null)
                {
                    return -1 * equippedCloak.elementalDefenceIce;
                }
                else
                {
                    return aCloak.elementalDefenceIce - equippedCloak.elementalDefenceIce;
                }
            }
        }

        public void Rest(bool inVillage)
        {
            if (!starving && !inVillage) hunger += metabolism / 2;    //Give back half of the nutrients that
                                                        //will be lost at next turn
            if (poisoned)
            {
                poisonDuration -= 1;
                if (poisonDuration <= 0)
                {
                    poisonDuration = 0;
                    poisonDamage = 0;
                    poisoned = false;
                }
            }

            if (currentMagik < MAX_MAGIK)
            {
                CountToRestore1Magik++;
                if (CountToRestore1Magik >= TurnsToRestore1Magik)
                {
                    CountToRestore1Magik = 0;
                    currentMagik += Math.Max(1, Math.Ceiling(MAX_MAGIK / 200));
                }
            }
            if (currentHealth < MAX_HEALTH)
            {
                CountToRestore1Health++;
                if (CountToRestore1Health >= TurnsToRestore1Health)
                {
                    CountToRestore1Health = 0;
                    currentHealth += Math.Max(1, Math.Ceiling(MAX_HEALTH / 200));
                    if (currentHealth > MAX_HEALTH) currentHealth = MAX_HEALTH;
                }
            }

        }

        public void Digest()
        {
            if (profession == (int)SC.Professions.EXPLORER)
            {
                hunger -= (metabolism / 3) * 2;
            }
            else
            {
                hunger -= metabolism;
            }
            if (hunger < 0)
            {
                hunger = 0;
                starving = true;
            }
        }

        public void ClearPoison()
        {
            poisoned = false;
            poisonDamage = 0;
            poisonDuration = 0;
        }

        public void Update(bool inVillage)
        {
            if (!inVillage)
            {
                if (!starving)
                {
                    Digest();
                }
                else
                {
                    currentHealth -= Math.Floor((MAX_HEALTH / 100) * STARVATION);
                    if (currentHealth <= 0)
                    {
                        currentHealth = 0;
                        alive = false;
                        deadFromStarvation = true;
                    }
                }
            }

            if (poisoned)
            {
                currentHealth -= poisonDamage;
                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    alive = false;
                    deadFromPoison = true;
                }
                else
                {
                    poisonDuration -= 1;
                    if (poisonDuration <= 0)
                    {
                        poisonDuration = 0;
                        poisonDamage = 0;
                        poisoned = false;
                    }
                }
            }

            if (attackBuffTurnsLeft > 0) attackBuffTurnsLeft--;
            if (attackBuffTurnsLeft == 0) attackBuff = 0;

            if (currentMagik < MAX_MAGIK)
            {
                CountToRestore1Magik++;
                if (CountToRestore1Magik >= TurnsToRestore1Magik)
                {
                    CountToRestore1Magik = 0;
                    currentMagik += Math.Max(1, Math.Ceiling(MAX_MAGIK / 200));
                }
            }
            if (currentHealth < MAX_HEALTH && !starving)
            {
                CountToRestore1Health++;
                if (CountToRestore1Health >= TurnsToRestore1Health)
                {
                    CountToRestore1Health = 0;
                    currentHealth += Math.Max(1, Math.Ceiling(MAX_HEALTH / 200));
                    if (currentHealth > MAX_HEALTH) currentHealth = MAX_HEALTH;
                }
            }

        }

        public Player()
        {
            random = new Random();
            carriedItems = new List<Item>();
            spells = new SpellCompendium();
        }

        //Initialization code must be kept separate from constructor, else it will trip up
        //XML deserialization
        public void Initialize()
        {
            spells.Initialise();
            name = "Player1";
        }

    }
}