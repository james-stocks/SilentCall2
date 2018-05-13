using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amadues
{
    public class Creature
    {

        public bool alive = true;
        public bool hostile = true;
        public bool allied = false;
        public bool kicked = false;
        public bool waiting = false;
        public bool attackedLastRound = false;
        public byte visibility = 0;
        public double recruitCost = 100;
        public bool frozen = false;
        public int freezeTimeRemaining = 0;
        public int thawRate = 2;

        public int currentLevel = 1;

        public int x = 0;
        public int y = 0;

        public double currentHealth = 50;

        public double attackPower = 1;
        public double defencePower = 1;

        public CreatureDescription description;

        public bool seenByPlayer = false;
        public bool heardByPlayer = false;

        public Creature()
        {
            description = new CreatureDescription();
        }

        public Creature(CreatureDescription aCD)
        {
            description = aCD.Clone();
            currentHealth = description.MAX_HEALTH;
            currentLevel = description.startingLevel;
            attackPower = (description.startingLevel + description.attackBonus) * 2;
            defencePower = (description.startingLevel + description.defenceBonus) * 2;
            hostile = description.isHostile;
        }

        //return gross damage total
        public double Attack(double fireDefence, double iceDefence)
        {
            if (description.elementOffenceFire == 0 && description.elementOffenceIce == 0) return attackPower;
            return Math.Floor((attackPower + description.elementOffenceFire) * fireDefence)
                + Math.Floor((attackPower + description.elementOffenceIce) * iceDefence);
        }

        //Do damage to the creature
        //Return bool for whether the attack has killed it
        public bool Damage(double damage)
        {
            attackedLastRound = true;
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                alive = false;
                return true;
            }
            return false;
        }

        //Heal a creature. Return whether the healing brought the creature back to full health
        public bool Heal(double amount)
        {
            if (currentHealth == description.MAX_HEALTH) return false;
            currentHealth = Math.Min(currentHealth + amount, description.MAX_HEALTH);
            return currentHealth == description.MAX_HEALTH;
        }

        public void LevelUp()
        {
            currentLevel++;
            recruitCost = recruitCost + Math.Ceiling(Math.Min((recruitCost / 100) * 0.3, 10)); ;
            //Increase health by 5%
            description.MAX_HEALTH = description.MAX_HEALTH + Math.Ceiling(Math.Min(Math.Max((description.MAX_HEALTH / 100) * 3, 2),8));
            //Have to max out health at this point otherwise spawned enemies will be missing health
            currentHealth = description.MAX_HEALTH;

            //Increase attack and defence
            /*attackPower = attackPower + Math.Ceiling(Math.Max((attackPower/300), 1));
            defencePower = defencePower + Math.Ceiling(Math.Max((defencePower / 300), 1));*/
            if (currentLevel < 100)
            {
                if (currentLevel % 3 == 1) attackPower += 3;
                if (currentLevel % 10 == 0) attackPower += 8;
                if (currentLevel % 20 == 0) attackPower += 8;
                if (currentLevel % 3 == 0) defencePower += 3;
                if (currentLevel % 10 == 5) defencePower += 8;
                if (currentLevel % 20 == 10) defencePower += 10;
            }
            else
            {
                if (currentLevel % 2 == 1) attackPower += 3;
                //if (currentLevel % 4 == 1) attackPower += 2;
                if (currentLevel % 3 == 0) defencePower += 1;
                if (currentLevel % 50 == 0)
                {
                    attackPower += description.attackBonus;
                    defencePower += description.defenceBonus;
                }
            }
        }

        public void Update()
        {
            attackedLastRound = false;
            if (frozen)
            {
                freezeTimeRemaining -= thawRate;
                if (freezeTimeRemaining <= 0)
                {
                    freezeTimeRemaining = 0;
                    frozen = false;
                }
            }
        }

        public int FreezeTimeRemaining()
        {
            if (!frozen) return 0;
            return (int)Math.Ceiling(freezeTimeRemaining / thawRate);
        }

        public bool Equals(Creature otherCreature)
        {
            return this.x == otherCreature.x
                && this.y == otherCreature.y
                && this.description.name == otherCreature.description.name
                && this.currentLevel == otherCreature.currentLevel
                && this.currentHealth == otherCreature.currentHealth;
        }

    }
}