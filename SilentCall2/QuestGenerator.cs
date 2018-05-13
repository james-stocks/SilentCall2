using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amadues
{
    class QuestGenerator
    {
        Random random;
        Bestiary bestiary;
        ItemGenerator itemGenerator;
        const int MIN_FLOORS = 5;
        const int MAX_FLOORS = 15;

        public QuestGenerator(Bestiary aB, ItemGenerator aI)
        {
            random = new Random();
            bestiary = aB;
            itemGenerator = aI;
        }

        public Quest GenerateQuest(int aLevel)
        {
            Quest quest = new Quest();
            quest.level = aLevel;
            quest.numFloors = random.Next(MIN_FLOORS, MAX_FLOORS + 1);
            quest.tileSet = random.Next(SC.numTileSets);
            quest.questType = random.Next(SC.numQuestTypes);
            switch (quest.questType)
            {
                case (int)SC.QuestType.ASSASSINATE:
                    quest.questCreature = bestiary.CreateHostileCreature(aLevel + quest.numFloors);
                    break;
                case (int)SC.QuestType.ITEM:
                    quest.questItem = itemGenerator.GenerateQuestItem(aLevel);
                    break;
                case (int)SC.QuestType.RESCUE:
                    quest.questCreature = bestiary.CreateQuestRescuee();
                    break;
                default:
                    break;
            }
            quest.dungeonName = GenerateDungeonName(quest.tileSet);
            quest.title = GenerateQuestTitle(quest);
            quest.rewardExp = quest.level + 10 + ((int)Math.Floor(Math.Log((double)quest.level)) * (quest.numFloors - MIN_FLOORS));
            quest.rewardExp +=  random.Next(1, 2 + (int)(quest.rewardExp * 0.05));
            //Decide on random gold reward according to quest level.
            //Then random chance that some of the gold will be deducted and 
            //a prize item generated instead
            quest.rewardGold = (quest.level + random.Next(0, 3) + quest.numFloors - MIN_FLOORS) * 10;
            quest.rewardGold += random.Next(1, 2 + (int)(quest.rewardGold * 0.05));
            if (random.NextDouble() < 0.4)
            {
                if (quest.rewardGold > 0)
                {
                    quest.rewardGold = Math.Floor(quest.rewardGold/2);
                    quest.rewardItem = itemGenerator.GenerateVillageShopItem((int)Math.Ceiling((quest.level + quest.numFloors) * 1.05));
                }
            }
            quest.forfeitGold = 1 + aLevel;
            return quest;
        }

        public Quest GenerateBossQuest(Boss theBoss)
        {
            Quest quest = new Quest();
            quest.isBossQuest = true;
            quest.theBoss = theBoss;
            quest.level = theBoss.level;
            quest.numFloors = 1;
            quest.tileSet = random.Next(SC.numTileSets);
            quest.questType = (int)SC.QuestType.ASSASSINATE;
            quest.questCreature = bestiary.CreateBoss(theBoss);
            quest.dungeonName = GenerateDungeonName(quest.tileSet);
            quest.title = GenerateQuestTitle(quest);
            quest.rewardExp = quest.level * 2;
            //Decide on random gold reward according to quest level.
            //Then random chance that some of the gold will be deducted and 
            //a prize item generated instead
            quest.rewardGold = (quest.level + random.Next(1, 3)) * 50;
            double prizeItemValue = Math.Floor(quest.rewardGold / 3);
            quest.rewardItem = itemGenerator.GenerateVillageShopItem((int)Math.Ceiling((quest.level + quest.numFloors) * 1.05));
            quest.forfeitGold = 1 + theBoss.level;
            return quest;
        }

        private String GenerateDungeonName(int tileSet)
        {
            String result = "Quest Dungeon";

            if (tileSet == (int)SC.TileSet.BRICK)
            {
                result = "The Workshops";
                if (random.NextDouble() < 0.3) result = "The Ruined Citadel";
                if (random.NextDouble() < 0.3) result = "The Old Elven Tunnels";
            }
            if (tileSet == (int)SC.TileSet.CAVE)
            {
                result = "The Village Caves";
                if (random.NextDouble() < 0.2) result = "Ancient Cave";
                if (random.NextDouble() < 0.2) result = "The Caverns Of Despair";
                if (random.NextDouble() < 0.2) result = "Goblin Hovel";
            }
            if (tileSet == (int)SC.TileSet.DESERT)
            {
                result = "The Desert Temple";
                if (random.NextDouble() < 0.2) result = "Pharaoh's Tomb";
                if (random.NextDouble() < 0.2) result = "The Great Pyramid";
                if (random.NextDouble() < 0.2) result = "The Lesser Pyramid";
            }
            if (tileSet == (int)SC.TileSet.DUNGEON)
            {
                result = "The Forgotten Dungeon";
                if (random.NextDouble() < 0.2) result = "Necromancer's Lair";
                if (random.NextDouble() < 0.2) result = "Ruined Castle";
                if (random.NextDouble() < 0.3) result = "Abandoned " + itemGenerator.GenerateRock().itemName + " Mine";
            }
            if (tileSet == (int)SC.TileSet.ICE)
            {
                result = "Ice Caverns";
                if (random.NextDouble() < 0.2) result = "Cursed Tsundra";
                if (random.NextDouble() < 0.2) result = "The Lost Glacier";
                if (random.NextDouble() < 0.2) result = "Frost Valley";
            }
            if (tileSet == (int)SC.TileSet.METAL)
            {
                result = "Temple Of The Ancients";
                if (random.NextDouble() < 0.3) result = "The Forbidden Bunker";
                if (random.NextDouble() < 0.3) result = "Metal Labyrinth";
            }
            return result;
        }

        private String GenerateQuestTitle(Quest aQuest)
        {
            String result = "UNDEFINED QUEST";
            if (aQuest.isBossQuest)
            {
                result = "Defeat " + aQuest.questCreature.description.description + "!";
            }
            else
            {
                switch (aQuest.questType)
                {
                    case (int)SC.QuestType.ASSASSINATE:
                        result = RandomAssassinateTerm() + " The " + aQuest.questCreature.description.description
                                + " In " + aQuest.dungeonName;
                        break;
                    case (int)SC.QuestType.EXPLORE:
                        result = "Explore " + aQuest.numFloors.ToString() + " Floors Of " + aQuest.dungeonName;
                        break;
                    case (int)SC.QuestType.ITEM:
                        result = RandomFetchTerm() + " The " + aQuest.questItem.itemDescription + " In "
                                + aQuest.dungeonName;
                        break;
                    case (int)SC.QuestType.RESCUE:
                        result = RandomRescueTerm() + " The " + aQuest.questCreature.description.description
                                + " From " + aQuest.dungeonName;
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        private String RandomAssassinateTerm()
        {
            String result = "Assassinate";
            if (random.NextDouble() < 0.15) result = "Kill";
            if (random.NextDouble() < 0.15) result = "Destroy";
            if (random.NextDouble() < 0.15) result = "Dispatch";
            if (random.NextDouble() < 0.15) result = "Slay";
            if (random.NextDouble() < 0.15) result = "Execute";
            if (random.NextDouble() < 0.15) result = "Eradicate";
            return result;
        }

        private String RandomFetchTerm()
        {
            String result = "Fetch Me";
            if (random.NextDouble() < 0.15) result = "Find";
            if (random.NextDouble() < 0.15) result = "Recover";
            if (random.NextDouble() < 0.15) result = "Locate";
            if (random.NextDouble() < 0.15) result = "Return";
            if (random.NextDouble() < 0.15) result = "Retrieve";
            if (random.NextDouble() < 0.15) result = "Go Get";
            if (random.NextDouble() < 0.15) result = "Obtain";
            return result;
        }

        private String RandomRescueTerm()
        {
            String result = "Rescue";
            if (random.NextDouble() < 0.3) result = "Escort";
            if (random.NextDouble() < 0.3) result = "Save";
            if (random.NextDouble() < 0.3) result = "Extricate";
            return result;
        }

    }
}
