using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amadues
{
    public class Bestiary
    {
        const int NUM_MONSTER_TYPES = 66;
        const int NUM_UNIQUE_MONSTERS = 30;
        public CreatureDescription[] creatureDescriptions;
        public CreatureDescription[] uniqueCreatureDescriptions;
        public List<String> rescueeDescriptions;

        Random random;

        public Bestiary()
        {
            random = new Random();
            creatureDescriptions = new CreatureDescription[NUM_MONSTER_TYPES];
            uniqueCreatureDescriptions = new CreatureDescription[NUM_UNIQUE_MONSTERS];
            rescueeDescriptions = new List<String>();
            LoadDescriptions();
        }

        //Return 
        public Creature CreateCreature(int value)
        {
            Creature aCreature;

            //Randomly pick a creatureDescription <= value
            List<CreatureDescription> possibleCreatures = new List<CreatureDescription>();
            for (int i = 0; i < creatureDescriptions.Length; i++)
            {
                if (creatureDescriptions[i].startingLevel <= value)
                {
                    possibleCreatures.Add(creatureDescriptions[i]);
                }
            }
            int chosenCreature = random.Next(0, possibleCreatures.Count);
            aCreature = new Creature(possibleCreatures[chosenCreature]);
            //Repeatedly level up the monster until it meets the required value
            if (aCreature.currentLevel < value)
            {
                int lowerBoundary = (int)Math.Floor(((double)((double)value - (double)aCreature.currentLevel) / 10) * 9);
                int levelsToIncrease = random.Next(lowerBoundary, value - aCreature.currentLevel);
                if (levelsToIncrease > 0)
                {
                    //Don't loop starting from aCreature.currentLevel because
                    //that value will be changed by the loop!
                    for (int i = aCreature.description.startingLevel; i < aCreature.description.startingLevel + levelsToIncrease; i++)
                    {
                        aCreature.LevelUp();
                    }
                }
            }
            return aCreature;
        }

        public Creature CreateHostileCreature(int value)
        {
            Creature aCreature;

            //Randomly pick a creatureDescription <= value
            List<CreatureDescription> possibleCreatures = new List<CreatureDescription>();
            for (int i = 0; i < creatureDescriptions.Length; i++)
            {
                if (creatureDescriptions[i].startingLevel <= value
                    && creatureDescriptions[i].isHostile)
                {
                    possibleCreatures.Add(creatureDescriptions[i]);
                }
            }
            int chosenCreature = random.Next(0, possibleCreatures.Count);
            aCreature = new Creature(possibleCreatures[chosenCreature]);
            //Repeatedly level up the monster until it meets the required value
            if (aCreature.currentLevel < value)
            {
                int lowerBoundary = (int)Math.Floor(((double)((double)value - (double)aCreature.currentLevel) / 10) * 9);
                int levelsToIncrease = random.Next(lowerBoundary, value - aCreature.currentLevel);
                if (levelsToIncrease > 0)
                {
                    //Don't loop starting from aCreature.currentLevel because
                    //that value will be changed by the loop!
                    for (int i = aCreature.description.startingLevel; i < aCreature.description.startingLevel + levelsToIncrease; i++)
                    {
                        aCreature.LevelUp();
                    }
                }
            }
            return aCreature;
        }

        public Creature CreateMerchant()
        {
            return new Creature(new CreatureDescription("Wandering Merchant", "Wandering Merchant", "A", (int)SC.CreatureTypes.MERCHANT, false, false, false, 1, 1, 50000, 99999999, 999999999, 0, 0, 1, 1, 1, 1, 1f));
        }

        public Creature CreateVillager(int x, int y)
        {
            Creature villager = new Creature(new CreatureDescription("Villager", "Village Commoner", "A", (int)SC.CreatureTypes.VILLAGER, false, false, false, 1, 1, 200, 0, 0, 0, 0, 1, 1, 1, 1, 1f));
            villager.x = x;
            villager.y = y;
            return villager;
        }
        public Creature CreateQuestMaster(int x, int y)
        {
            Creature result = new Creature(new CreatureDescription("Quest Master", "Quest Master", "The", (int)SC.CreatureTypes.QUESTMASTER, false, false, false, 1, 1, 200, 0, 0, 0, 0, 1, 1, 1, 1, 1f));
            result.x = x;
            result.y = y;
            return result;
        }

        public Creature CreateQuestRescuee()
        {
            String description = RandomRescueeDescription();
            String prefix = "A";
            if (IsVowel(description[0])) prefix = "An";
            Creature result = new Creature(new CreatureDescription(description, description, prefix, (int)SC.CreatureTypes.QUESTRESCUEE, false, false, false, 1, 0, 200, 0, 0, 0,0,0,0,1,1, 1f));
            return result;
        }

        //Bosses!
        public Creature SummonKrull(int x, int y)
        {
            Creature krull = new Creature(new CreatureDescription("Krull", "Krull", "", (int)SC.CreatureTypes.SPIDER, true, true, true, 500, 500, 10000, 15, 5, 0, 0, 5, 1, 2, 2, 1.4f));
            krull.x = x;
            krull.y = y;
            return krull;
        }

        public Creature SummonGarathTheTerrible(int x, int y)
        {
            Creature garath = new Creature(new CreatureDescription("Garath The Terrible", "Garath The Terrible", "", (int)SC.CreatureTypes.GOBLIN, true, false, true, 1000, 500, 20000, 15, 5, 0, 0, 1, 1, 2, 2, 1f));
            garath.x = x;
            garath.y = y;
            return garath;
        }

        public Creature SummonChuckyhacks(int x, int y)
        {
            Creature chucky = new Creature(new CreatureDescription("Chuckyhacks", "Chuckyhacks The Troll", "", (int)SC.CreatureTypes.TROLL, true, false, true, 2000, 1000, 20000, 15, 5, 0, 0, 1, 1, 2, 2, 1f));
            chucky.x = x;
            chucky.y = y;
            chucky.description.elementDefenceFire = 5;
            return chucky;
        }
        public Creature SummonDuckbeard(int x, int y)
        {
            Creature duckbeard = new Creature(new CreatureDescription("Duckbeard", "Duckbeard", "", (int)SC.CreatureTypes.CANINE, true, false, true, 1900, 500, 1000, 0, 0, 0, 0, 1, 1, 1, 1, 1f));
            duckbeard.x = x;
            duckbeard.y = y;
            return duckbeard;
        }

        //Giant succubus
        public Creature SummonAyeyerma(int x, int y)
        {
            Creature urma = new Creature(new CreatureDescription("Ayeyerma", "Ayeyerma The Valkyrie", "", (int)SC.CreatureTypes.SUCCUBUS, true, false, true, 3000, 1500, 100000, 30, 35, 0, 0, 1, 1, 2, 2, 1f));
            urma.x = x;
            urma.y = y;
            return urma;
        }

        //Giant skeleton
        public Creature SummonMictlantecuhtli(int x, int y)
        {
            Creature mict = new Creature(new CreatureDescription("Mictlantecuhtli", "Mictlantecuhtli", "", (int)SC.CreatureTypes.SKELETON, true, false, true, 4000, 2000, 100000, 20, 40, 0, 0, 1, 1, 2, 2, 1f));
            mict.x = x;
            mict.y = y;
            mict.description.elementDefenceFire = 0.7;
            mict.description.elementDefenceIce = 0.7;
            return mict;
        }

        //Giant slime
        public Creature SummonCrocell(int x, int y)
        {
            Creature crocell = new Creature(new CreatureDescription("Crocell", "Crocell", "", (int)SC.CreatureTypes.SLIME, true, true, true, 5000, 2500, 100000, 20, 40, 0, 0, 1, 1, 2, 2, 1f));
            crocell.x = x;
            crocell.y = y;
            crocell.description.poisonous = true;
            return crocell;
        }

        //Giant goblin
        public Creature SummonElverkong(int x, int y)
        {
            Creature elverkong = new Creature(new CreatureDescription("Elverkong", "Elverkong, King Of Goblins", "", (int)SC.CreatureTypes.GOBLIN, true, false, true, 6000, 5000, 100000, 30, 30, 0, 0, 1, 1, 2, 2, 1f));
            elverkong.x = x;
            elverkong.y = y;
            return elverkong;
        }

        //Giant dragon
        public Creature SummonAzazel(int x, int y)
        {
            Creature azazel = new Creature(new CreatureDescription("Azazel", "Azazel The Dragon", "", (int)SC.CreatureTypes.DRAGON, true, false, true, 7000, 10000, 200000, 50, 30, 0, 0, 1, 1, 3, 3, 1f));
            azazel.x = x;
            azazel.y = y;
            azazel.description.elementOffenceFire = 20;
            azazel.description.elementDefenceFire = 0.05;
            return azazel;
        }

        //Giant Demon
        public Creature SummonXaphan(int x, int y)
        {
            Creature xaphan = new Creature(new CreatureDescription("Xaphan", "Lord Daemon Xaphan", "", (int)SC.CreatureTypes.DEMON, true, false, true, 8000, 4000, 400000, 30, 20, 0, 0, 1, 1, 4, 4, 1f));
            xaphan.x = x;
            xaphan.y = y;
            return xaphan;
        }

        public Creature SummonGoldRick(int x, int y)
        {
            CreatureDescription goldRick = new CreatureDescription("Gold Rick", "The Dark Lord, Gold Rick", "", (int)SC.CreatureTypes.GOLDRICK, true, false, true, 9001, 4500, 500000, 0, 0, 20, 20, 0.1, 0.1, 1, 2, 1f);
            goldRick.displaySize = 1.4f;
            Creature rick = new Creature(goldRick);
            rick.x = x;
            rick.y = y;
            return rick;
        }

        public Creature CreateBoss(Boss theBoss)
        {
            if (theBoss.level == 500)
            {
                return SummonKrull(0, 0);
            }
            if (theBoss.level == 1000)
            {
                return SummonGarathTheTerrible(0, 0);
            }
            if (theBoss.level == 2000)
            {
                return SummonChuckyhacks(0, 0);
            }
            if (theBoss.level == 3000)
            {
                return SummonAyeyerma(0, 0);
            }
            if (theBoss.level == 4000)
            {
                return SummonMictlantecuhtli(0, 0);
            }
            if (theBoss.level == 5000)
            {
                return SummonCrocell(0, 0);
            }
            if (theBoss.level == 6000)
            {
                return SummonElverkong(0, 0);
            }
            if (theBoss.level == 7000)
            {
                return SummonAzazel(0, 0);
            }
            if (theBoss.level == 8000)
            {
                return SummonXaphan(0, 0);
            }
            if (theBoss.level == 9000)
            {
                return SummonGoldRick(0, 0);
            }
            return SummonGarathTheTerrible(0,0);
        }

        public Creature CreateBoss(Boss theBoss, int aX, int aY)
        {
            Creature result = CreateBoss(theBoss);
            result.x = aX;
            result.y = aY;
            return result;
        }

        public Creature SummonAltarDaemon(int level, int x, int y)
        {
            Creature result = new Creature(new CreatureDescription("Hell Daemon", "Daemon Summoned From Hell", "A", (int)SC.CreatureTypes.DEMON, true, false, false, level + 10, 500, 20 * level, 20, 20, 5, 0, 0.5, 1, 2 ,2, 1f));
            result.x = x;
            result.y = y;
            return result;
        }

        private String RandomRescueeDescription()
        {
            return rescueeDescriptions[random.Next(rescueeDescriptions.Count)];
        }

        private bool IsVowel(char c)
        {
            if (c == 'A') return true;
            if (c == 'E') return true;
            if (c == 'I') return true;
            if (c == 'O') return true;
            if (c == 'U') return true;

            if (c == 'a') return true;
            if (c == 'e') return true;
            if (c == 'i') return true;
            if (c == 'o') return true;
            if (c == 'u') return true;

            return false;

        }
        private void LoadDescriptions()
        {
            creatureDescriptions[0] = new CreatureDescription("Cave Rat", "Common Cave Rat", (int)SC.CreatureTypes.RODENT, true, 1, 8, 5);
            creatureDescriptions[0].elementDefenceFire = 1.3;
            creatureDescriptions[1] = new CreatureDescription("Bat", "Common Bat", (int)SC.CreatureTypes.BAT, true, 1, 5, 5);
            creatureDescriptions[1].elementDefenceFire = 2;
            creatureDescriptions[1].floats = true;
            creatureDescriptions[1].displaySize = 1.4f;
            creatureDescriptions[2] = new CreatureDescription("Beetle", "Large Beetle", (int)SC.CreatureTypes.BUG, true, 2, 10, 15);
            creatureDescriptions[2].elementDefenceFire = 2;
            creatureDescriptions[2].displaySize = 1.4f;
            creatureDescriptions[3] = new CreatureDescription("Skink", "Wall Skink", (int)SC.CreatureTypes.LIZARD, true, 4, 10, 15);
            creatureDescriptions[3].elementDefenceFire = 1.3;
            creatureDescriptions[3].elementDefenceIce = 1.3;
            creatureDescriptions[4] = new CreatureDescription("Wild Dog", "Wild Dog", (int)SC.CreatureTypes.CANINE, true, 5, 15, 20, 1, 0);
            creatureDescriptions[4].elementDefenceFire = 1.3;
            creatureDescriptions[5] = new CreatureDescription("Vampire Bat", "Vampire Bat", (int)SC.CreatureTypes.BAT, true, 6, 20, 25, 0, 1);
            creatureDescriptions[5].elementDefenceFire = 2;
            creatureDescriptions[5].floats = true;
            creatureDescriptions[5].displaySize = 1.4f;
            creatureDescriptions[6] = new CreatureDescription("Scarab Beetle", "Scarab Beetle", (int)SC.CreatureTypes.BUG, true, 7, 20, 25, 0, 2);
            creatureDescriptions[6].displaySize = 1.4f;
            creatureDescriptions[7] = new CreatureDescription("Juvenile Dragon", "Juvenile Dragon", (int)SC.CreatureTypes.DRAGON, true, 100, 1000, 700, 100, 100);  
            creatureDescriptions[7].width = 2;
            creatureDescriptions[7].height = 2;
            creatureDescriptions[7].elementOffenceFire = 10;
            creatureDescriptions[7].elementDefenceFire = 0.01;
            creatureDescriptions[8] = new CreatureDescription("Goblin Grunt", "Goblin Grunt", (int)SC.CreatureTypes.GOBLIN, true, 8, 30, 40);
            creatureDescriptions[9] = new CreatureDescription("Goblin Soldier", "Goblin Soldier", (int)SC.CreatureTypes.GOBLIN, true, 20, 60, 40, 2, 0);
            creatureDescriptions[10] = new CreatureDescription("Goblin Captain", "Goblin Captain", (int)SC.CreatureTypes.GOBLIN, true, 50, 90, 50, 2, 1);
            creatureDescriptions[11] = new CreatureDescription("Goblin Commando", "Goblin Commando", (int)SC.CreatureTypes.GOBLIN, true, 100, 120, 60, 3, 2);
            creatureDescriptions[12] = new CreatureDescription("Goblin Elite", "Goblin Elite", (int)SC.CreatureTypes.GOBLIN, true, 200, 150, 100, 4, 4);
            creatureDescriptions[13] = new CreatureDescription("Goblin Destroyer", "Goblin Destroyer", (int)SC.CreatureTypes.GOBLIN, true, 500, 300, 800, 10, 10);
            creatureDescriptions[14] = new CreatureDescription("Snake", "Snake", (int)SC.CreatureTypes.SNAKE, true, 3, 10, 6);
            creatureDescriptions[14].elementDefenceFire = 1.5;
            creatureDescriptions[14].elementDefenceIce = 1.5;
            creatureDescriptions[15] = new CreatureDescription("Cobra", "Cobra", (int)SC.CreatureTypes.SNAKE, true, 12, 30, 20, 1, 0);
            creatureDescriptions[15].elementDefenceFire = 1.5;
            creatureDescriptions[15].elementDefenceIce = 1.5;
            creatureDescriptions[15].poisonous = true;
            creatureDescriptions[16] = new CreatureDescription("Vile Cobra", "Vile Cobra", (int)SC.CreatureTypes.SNAKE, true, 36, 40, 40, 3, 0);
            creatureDescriptions[16].elementDefenceFire = 1.5;
            creatureDescriptions[16].elementDefenceIce = 1.5;
            creatureDescriptions[16].poisonous = true;
            creatureDescriptions[17] = new CreatureDescription("Sea Krait", "Sea Krait", (int)SC.CreatureTypes.SNAKE, true, 60, 40, 30, 2, 0);
            creatureDescriptions[17].elementDefenceFire = 1.5;
            creatureDescriptions[17].elementDefenceIce = 1.5;
            creatureDescriptions[17].poisonous = true;
            creatureDescriptions[18] = new CreatureDescription("Green Mamba", "Green Mamba", (int)SC.CreatureTypes.SNAKE, true, 90, 50, 60, 4, 2);
            creatureDescriptions[18].elementDefenceFire = 1.5;
            creatureDescriptions[18].elementDefenceIce = 1.5;
            creatureDescriptions[18].poisonous = true;
            creatureDescriptions[19] = new CreatureDescription("Puff Adder", "Puff Adder", (int)SC.CreatureTypes.SNAKE, true, 120, 50, 80, 2, 2);
            creatureDescriptions[19].elementDefenceFire = 1.5;
            creatureDescriptions[19].elementDefenceIce = 1.5;
            creatureDescriptions[19].poisonous = true;
            creatureDescriptions[20] = new CreatureDescription("Death Adder", "Death Adder", (int)SC.CreatureTypes.SNAKE, true, 150, 60, 100, 3, 3);
            creatureDescriptions[20].elementDefenceFire = 1.5;
            creatureDescriptions[20].elementDefenceIce = 1.5;
            creatureDescriptions[20].poisonous = true;
            creatureDescriptions[21] = new CreatureDescription("Succubus", "Succubus", (int)SC.CreatureTypes.SUCCUBUS, true, 200, 120, 200, 3, 3);
            creatureDescriptions[22] = new CreatureDescription("Ancient Succubus", "Ancient Succubus", (int)SC.CreatureTypes.SUCCUBUS, true, 500, 250, 400, 5, 5);
            creatureDescriptions[22].prefix = "An";
            creatureDescriptions[23] = new CreatureDescription("Lust", "Lust", (int)SC.CreatureTypes.SUCCUBUS, true, 1000, 300, 400, 8, 8);
            creatureDescriptions[24] = new CreatureDescription("Lesser Daemon", "Lesser Daemon", (int)SC.CreatureTypes.DEMON, true, 200, 50, 50, 1, 1);
            creatureDescriptions[24].width = 2;
            creatureDescriptions[24].height = 2;
            creatureDescriptions[25] = new CreatureDescription("Daemon", "Daemon", (int)SC.CreatureTypes.DEMON, true, 500, 150, 500, 5, 3);
            creatureDescriptions[25].width = 2;
            creatureDescriptions[25].height = 2;
            creatureDescriptions[26] = new CreatureDescription("Raging Daemon", "Raging Daemon", (int)SC.CreatureTypes.DEMON, true, 1000, 150, 500, 8, 3);
            creatureDescriptions[26].width = 2;
            creatureDescriptions[26].height = 2;
            creatureDescriptions[26].elementOffenceFire = 10;
            creatureDescriptions[27] = new CreatureDescription("Eldritch Daemon", "Eldritch Daemon", (int)SC.CreatureTypes.DEMON, true, 2000, 500, 2000, 10, 5);
            creatureDescriptions[27].width = 2;
            creatureDescriptions[27].height = 2;
            creatureDescriptions[27].prefix = "An";
            creatureDescriptions[28] = new CreatureDescription("Guard", "Guard", (int)SC.CreatureTypes.HUMAN, false, 5, 80, 80, 8, 2);
            creatureDescriptions[29] = new CreatureDescription("Risen Bones", "Risen Bones", (int)SC.CreatureTypes.SKELETON, true, 30, 80, 40);
            creatureDescriptions[29].elementDefenceFire = 0.7;
            creatureDescriptions[29].elementDefenceIce = 0.7;
            creatureDescriptions[30] = new CreatureDescription("Skeleton Guard", "Skeleton Guard", (int)SC.CreatureTypes.SKELETON, true, 50, 80, 60, 1, 1);
            creatureDescriptions[30].elementDefenceFire = 0.7;
            creatureDescriptions[30].elementDefenceIce = 0.7;
            creatureDescriptions[31] = new CreatureDescription("Skeleton Warrior", "Skeleton Warrior", (int)SC.CreatureTypes.SKELETON, true, 100, 100, 80, 2, 2);
            creatureDescriptions[31].elementDefenceFire = 0.7;
            creatureDescriptions[31].elementDefenceIce = 0.7;
            creatureDescriptions[32] = new CreatureDescription("Skeleton Brute", "Skeleton Brute", (int)SC.CreatureTypes.SKELETON, true, 300, 200, 300, 4, 4);
            creatureDescriptions[32].elementDefenceFire = 0.7;
            creatureDescriptions[32].elementDefenceIce = 0.7;
            creatureDescriptions[33] = new CreatureDescription("Elite Skeleton", "Elite Skeleton", (int)SC.CreatureTypes.SKELETON, true, 800, 500, 300, 8, 8);
            creatureDescriptions[33].prefix = "An";
            creatureDescriptions[33].elementDefenceFire = 0.7;
            creatureDescriptions[33].elementDefenceIce = 0.7;
            creatureDescriptions[34] = new CreatureDescription("Poisonous Bug", "Poisonous Bug", (int)SC.CreatureTypes.BUG, true, 8, 20, 14);
            creatureDescriptions[34].elementDefenceFire = 2;
            creatureDescriptions[34].poisonous = true;
            creatureDescriptions[34].displaySize = 1.4f;
            creatureDescriptions[35] = new CreatureDescription("Blazin Bones", "Blazin Bones", (int)SC.CreatureTypes.SKELETON, true, 600, 800, 200, 4, 4);
            creatureDescriptions[35].elementOffenceFire = 10;
            creatureDescriptions[35].elementDefenceFire = 0.05;
            creatureDescriptions[35].elementDefenceIce = 4.0;
            creatureDescriptions[36] = new CreatureDescription("Frost Bones", "Frost Bones", (int)SC.CreatureTypes.SKELETON, true, 600, 800, 200, 2, 2);
            creatureDescriptions[36].elementOffenceIce = 10;
            creatureDescriptions[36].elementDefenceFire = 4.0;
            creatureDescriptions[36].elementDefenceIce = 0.05;
            creatureDescriptions[37] = new CreatureDescription("Wandering Spirit", "Ghost", (int)SC.CreatureTypes.GHOST, false, 20, 5, 10, 0, 10);
            creatureDescriptions[37].floats = true;
            creatureDescriptions[38] = new CreatureDescription("Angry Spirit", "Ghost", (int)SC.CreatureTypes.GHOST, true, 100, 40, 100, 5, 10);
            creatureDescriptions[38].floats = true;
            creatureDescriptions[39] = new CreatureDescription("Dungeon Slime", "Slime", (int)SC.CreatureTypes.SLIME, true, 30, 40, 40);
            creatureDescriptions[40] = new CreatureDescription("Troll", "Troll", (int)SC.CreatureTypes.TROLL, true, 40, 50, 50);
            creatureDescriptions[40].elementDefenceFire = 6.0;
            creatureDescriptions[41] = new CreatureDescription("Gladiator", "Gladiator", (int)SC.CreatureTypes.HUMAN, false, 50, 80, 200, 16, 4);
            creatureDescriptions[42] = new CreatureDescription("Hero", "Hero", (int)SC.CreatureTypes.HUMAN, false, 500, 800, 1000, 32, 8);
            creatureDescriptions[43] = new CreatureDescription("Zombie", "Zombie", (int)SC.CreatureTypes.ZOMBIE, true, 500, 500, 200, 0, 2);
            creatureDescriptions[43].elementDefenceFire = 10;
            creatureDescriptions[43].elementDefenceIce = 0.3;
            creatureDescriptions[44] = new CreatureDescription("Ghoul", "Ghoul", (int)SC.CreatureTypes.ZOMBIE, true, 1500, 1000, 200, 2, 4);
            creatureDescriptions[44].elementDefenceFire = 9;
            creatureDescriptions[44].elementDefenceIce = 0.2;
            creatureDescriptions[45] = new CreatureDescription("Walking Terror", "Walking Terror", (int)SC.CreatureTypes.ZOMBIE, true, 2500, 1000, 200, 4, 8);
            creatureDescriptions[45].elementDefenceFire = 7;
            creatureDescriptions[45].elementDefenceIce = 0.1;
            creatureDescriptions[46] = new CreatureDescription("Dragon", "Dragon", (int)SC.CreatureTypes.DRAGON, true, 1000, 1000, 8000, 200, 200);
            creatureDescriptions[46].width = 2;
            creatureDescriptions[46].height = 2;
            creatureDescriptions[46].elementOffenceFire = 10;
            creatureDescriptions[46].elementDefenceFire = 0.01;
            creatureDescriptions[47] = new CreatureDescription("Ancient Dragon", "Ancient Dragon", (int)SC.CreatureTypes.DRAGON, true, 6000, 3000, 80000, 300, 300);
            creatureDescriptions[47].width = 2;
            creatureDescriptions[47].height = 2;
            creatureDescriptions[47].elementOffenceFire = 10;
            creatureDescriptions[47].elementDefenceFire = 0.01;
            creatureDescriptions[48] = new CreatureDescription("Chef", "Wandering Chef", (int)SC.CreatureTypes.CHEF, false, 5500, 1, 500);
            creatureDescriptions[48].attacksWhenAllied = false;
            creatureDescriptions[49] = new CreatureDescription("Cook", "Wandering Cook", (int)SC.CreatureTypes.CHEF, false, 6500, 1, 500);
            creatureDescriptions[49].attacksWhenAllied = false;
            creatureDescriptions[50] = new CreatureDescription("Butcher", "Wandering Butcher", (int)SC.CreatureTypes.CHEF, false, 7500, 1, 500);
            creatureDescriptions[50].attacksWhenAllied = false;
            creatureDescriptions[51] = new CreatureDescription("Vicious Troll", "Vicious Troll", (int)SC.CreatureTypes.TROLL, true, 500, 800, 500, 8, 5);
            creatureDescriptions[51].elementDefenceFire = 6.0;
            creatureDescriptions[52] = new CreatureDescription("Shadow Troll", "Shadow Troll", (int)SC.CreatureTypes.TROLL, true, 2500, 1000, 700, 12, 8);
            creatureDescriptions[52].elementDefenceFire = 6.0;
            creatureDescriptions[53] = new CreatureDescription("Venom Slime", "Venom Slime", (int)SC.CreatureTypes.SLIME, true, 1200, 400, 1000, 4, 0);
            creatureDescriptions[53].poisonous = true;
            creatureDescriptions[54] = new CreatureDescription("Killer Mould", "Killer Mould", (int)SC.CreatureTypes.SLIME, true, 2500, 800, 2000, 6, 2);
            creatureDescriptions[55] = new CreatureDescription("House Spider", "House Spider", (int)SC.CreatureTypes.SPIDER, true, 3, 13, 20);
            creatureDescriptions[55].elementDefenceFire = 5;
            creatureDescriptions[55].displaySize = 1.4f;
            creatureDescriptions[56] = new CreatureDescription("Dungeon Weaver", "Dungeon Weaver", (int)SC.CreatureTypes.SPIDER, true, 15, 40, 100);
            creatureDescriptions[56].elementDefenceFire = 5;
            creatureDescriptions[56].displaySize = 1.4f;
            creatureDescriptions[57] = new CreatureDescription("Tarantula", "Tarantula", (int)SC.CreatureTypes.SPIDER, true, 40, 120, 320);
            creatureDescriptions[57].poisonous = true;
            creatureDescriptions[57].elementDefenceFire = 5;
            creatureDescriptions[57].displaySize = 1.4f;
            creatureDescriptions[58] = new CreatureDescription("Giant Tarantula", "Giant Tarantula", (int)SC.CreatureTypes.SPIDER, true, 100, 300, 1000, 2, 2);
            creatureDescriptions[58].poisonous = true;
            creatureDescriptions[58].elementDefenceFire = 5;
            creatureDescriptions[58].width = 2;
            creatureDescriptions[58].height = 2;
            creatureDescriptions[58].displaySize = 1.4f;
            creatureDescriptions[59] = new CreatureDescription("Cob", "Cob", (int)SC.CreatureTypes.SPIDER, true, 200, 550, 700, 4, 2);
            creatureDescriptions[59].poisonous = true;
            creatureDescriptions[59].elementDefenceFire = 5;
            creatureDescriptions[59].displaySize = 1.4f;
            creatureDescriptions[60] = new CreatureDescription("Ariadne", "Ariadne", (int)SC.CreatureTypes.SPIDER, true, 300, 700, 850, 5, 3);
            creatureDescriptions[60].prefix = "An";
            creatureDescriptions[60].poisonous = true;
            creatureDescriptions[60].elementDefenceFire = 5;
            creatureDescriptions[60].displaySize = 1.4f;
            creatureDescriptions[61] = new CreatureDescription("Bird Eater", "Bird Eating Spider", (int)SC.CreatureTypes.SPIDER, true, 400, 800, 950, 5, 4);
            creatureDescriptions[61].poisonous = true;
            creatureDescriptions[61].elementDefenceFire = 5;
            creatureDescriptions[61].displaySize = 1.4f;
            creatureDescriptions[62] = new CreatureDescription("Dungeon Recluse", "Dungeon Recluse Spider", (int)SC.CreatureTypes.SPIDER, true, 500, 950, 1000, 5, 5);
            creatureDescriptions[62].poisonous = true;
            creatureDescriptions[62].elementDefenceFire = 5;
            creatureDescriptions[62].displaySize = 1.4f;
            creatureDescriptions[63] = new CreatureDescription("Giant Recluse", "Giant Recluse Spider", (int)SC.CreatureTypes.SPIDER, true, 1100, 1600, 1900, 6, 5);
            creatureDescriptions[63].width = 2;
            creatureDescriptions[63].height = 2;
            creatureDescriptions[63].poisonous = true;
            creatureDescriptions[63].elementDefenceFire = 5;
            creatureDescriptions[63].displaySize = 1.4f;
            creatureDescriptions[64] = new CreatureDescription("Orb Weaver", "Orb Weaver", (int)SC.CreatureTypes.SPIDER, true, 3000, 4000, 5500, 6, 6);
            creatureDescriptions[64].poisonous = true;
            creatureDescriptions[64].elementDefenceFire = 5;
            creatureDescriptions[64].displaySize = 1.4f;
            creatureDescriptions[65] = new CreatureDescription("Tsundra Troll", "Tsundra Troll", (int)SC.CreatureTypes.TROLL, true, 650, 900, 600, 8, 6);
            creatureDescriptions[65].elementDefenceFire = 7.0;
            creatureDescriptions[65].elementOffenceIce = 5.0;


            rescueeDescriptions.Add("Lost Villager");
            rescueeDescriptions.Add("Trapped Villager");
            rescueeDescriptions.Add("Idiot Farmboy");
            rescueeDescriptions.Add("Lost Priest");
            rescueeDescriptions.Add("Confused Grandfather");
            rescueeDescriptions.Add("Poor Adventurer");
            rescueeDescriptions.Add("Cowardly Knight");
            rescueeDescriptions.Add("Unlucky Scribe");
            rescueeDescriptions.Add("Explorer");
            rescueeDescriptions.Add("Rebel Scout");

        }
    }
}