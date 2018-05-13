using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amadues
{
    //[Serializable()]
    public class ResumableGame
    {
        public String version;
        public Level dungeonLevel;
        public Level villageLevel;
        public bool inVillage;
        public String dungeonMusicTitle;
        public double moveCount; 
        public Player player;
        public Stats stats;
        public List<Item> dungeonMerchantItems;
        public List<Item> villageMerchantItems;
        public int villageMerchantLevel;
        public Quest currentQuest;
        public List<Quest> questList;

        //Class requires a parameterless constructor to be serialized
        public ResumableGame()
        {
        }

        public ResumableGame(String v, Player p, Level dl, Level vl, bool iv, Stats s, List<Item> dmi, List<Item> vmi, int vml, double mc, Quest cq, List<Quest> ql, String aDungeonMusicTitle)
        {
            version = v;
            player = p;
            dungeonLevel = dl;
            villageLevel = vl;
            inVillage = iv;
            stats = s;
            dungeonMerchantItems = dmi;
            villageMerchantItems = vmi;
            villageMerchantLevel = vml;
            moveCount = mc;
            currentQuest = cq;
            questList = ql;
            dungeonMusicTitle = aDungeonMusicTitle;
        }
    }
}