using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Amadues
{
    //[Serializable]
    public class GameSavedData
    {
        public String version;
        public double totalBonusXP;
        public double totalBonusGold;
        public Stats totalStats;
        public bool fullDisplay;
        public int tileSize;
        public bool movingWithDpad;
        public int maxStartingLevel;
        public long totalTimeSeconds;
        public List<Boss> bossList;
        public int villageMerchantLevel;
        public byte[][] villageBuildArea;
        public List<Item> villageItems;
        public List<int> tutorialMessagesNeverToSeeAgain;
        public SC.BGMPlayOption bgmPlayOption;
        public SC.BGMShuffleOption bgmShuffleOption;
        public List<Feat> featList;
        public List<Item> lootItemsFromLastPlayer;

        public GameSavedData()
        {

        }

        public GameSavedData(String v, double xp, double g, Stats s, bool fd, int size, int maxLevel, long tts, List<Boss> bl, int vml, byte[][] vba, List<Item> vItems, List<int> tm)
        {
            version = v;
            totalBonusXP = xp;
            totalBonusGold = g;
            totalStats = s;
            fullDisplay = fd;
            tileSize = size;
            maxStartingLevel = maxLevel;
            totalTimeSeconds = tts;
            bossList = bl;
            villageMerchantLevel = vml;
            villageBuildArea = vba;
            villageItems = vItems;
            tutorialMessagesNeverToSeeAgain = tm;
        }

    }
}