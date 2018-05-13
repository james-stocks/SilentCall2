using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amadues
{
    public class Quest
    {
        public String title; //The title of the quest
        public int level = 1; //The level (difficulty) of the quest, will be somewhere around the player's experience level. This will likely be a hidden value - may just display "Easy" if <= player.level else "Hard"
        public int tileSet = 0; //The dungeon tileset for this quest
        public int numFloors = 0; //The number of dungeon floors for this quest
        public String dungeonName = "Quest Dungeon";
        public int questType = (int)SC.QuestType.ASSASSINATE;

        public Item questItem; //The item to collect if this quest is an item quest
        public Creature questCreature; //The target creature if this is a rescue or assassinate quest

        public double rewardGold = 0; //Gold reward for the quest
        public double rewardExp = 0; //Experience reward for the quest.
        public Item rewardItem; //Item reward for the quest, null if none

        public bool isBossQuest = false;
        public Boss theBoss;
        public bool completed = false;
        public bool failed = false;
        public double forfeitGold = 1; //Penalty the player needs to pay to drop the quest and pick an alternate one

        public Quest()
        {

        }

        public bool Equals(Quest otherQuest)
        {
            return title.Equals(otherQuest.title)
                && level == otherQuest.level
                && rewardGold == otherQuest.rewardGold
                && rewardExp == otherQuest.rewardExp
                && numFloors == otherQuest.numFloors
                && questType == otherQuest.questType;
        }

        public bool IsFinalQuest()
        {
            if (isBossQuest)
            {
                if (theBoss.name == "Gold Rick")
                    return true;
            }
            return false;
        }

    }
}
