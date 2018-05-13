using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amadues
{
    public class Spell
    {

        public int spellID;
        public String spellName;
        public bool isKnown;
        public int level;
        public int castingCost;
        public int castingType;
        public double power = 1;

        public Spell()
        {
            spellID = -1;
            spellName = "UNDEFINED SPELL";
            isKnown = false;
            level = 1;
            castingCost = 1;
            castingType = (int)SC.SpellCasting.SELF;
        }

        public Spell(int aID, String name, int casting, int cost, double pow)
        {
            spellID = aID;
            spellName = name;
            isKnown = false;
            level = 1;
            castingType = casting;
            castingCost = cost;
            power = pow;
        }
    }
}