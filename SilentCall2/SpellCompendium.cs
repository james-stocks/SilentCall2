using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Amadues
{
    public class SpellCompendium
    {

        const int MAX_SPELL_LEVEL = 999;

        public List<Spell> spellList;

        public SpellCompendium()
        {
            spellList = new List<Spell>();
            
            //This needs removed from the constructor, it would be called during
            //XML deserialization and cause spells to double up!
            //LoadSpells();
        }

        public void Initialise()
        {
            LoadSpells();
        }

        //Mark spells known according the player's class and level
        //Return true if a spell is newly unlocked
        public bool UnlockSpells(Player thePlayer)
        {
            bool newSpellKnown = false;
            switch (thePlayer.profession)
            {
                case (int)SC.Professions.MAGE:
                    newSpellKnown = KnowSpell("Fireball") || newSpellKnown;
                    newSpellKnown = KnowSpell("Heal") || newSpellKnown;
                    if (thePlayer.level >= 10)
                    {
                        newSpellKnown = KnowSpell("Freeze") || newSpellKnown;
                        if (thePlayer.level >= 20)
                        {
                            newSpellKnown = KnowSpell("Magic Missile") || newSpellKnown;
                            if (thePlayer.level >= 41)
                            {
                                newSpellKnown = KnowSpell("Inferno") || newSpellKnown;
                                if (thePlayer.level >= 81)
                                {
                                    newSpellKnown = KnowSpell("Blizzard") || newSpellKnown;
                                }
                            }
                        }
                        if (thePlayer.level >= 1000)
                        {
                            newSpellKnown = KnowSpell("Raze") || newSpellKnown;
                        }
                        if (thePlayer.level >= 5000)
                        {
                            newSpellKnown = KnowSpell("Icicle") || newSpellKnown;
                        }
                    }
                    if (thePlayer.level > 40 && thePlayer.level % 60 == 0) UpgradeSpell("Fireball");
                    if (thePlayer.level < 40 && thePlayer.level % 7 == 0) UpgradeSpell("Fireball");
                    if (thePlayer.level > 50 && thePlayer.level < 1000 && thePlayer.level % 25 == 5) UpgradeSpell("Magic Missile");
                    if (thePlayer.level > 1000 && thePlayer.level % 30 == 0) UpgradeSpell("Magic Missile");
                    if (thePlayer.level > 15 && thePlayer.level % 30 == 0) UpgradeSpell("Heal");
                    if (thePlayer.level > 10 && thePlayer.level % 50 == 0) UpgradeSpell("Freeze");
                    if (thePlayer.level > 40 && thePlayer.level % 50 == 0) UpgradeSpell("Inferno");
                    if (thePlayer.level > 80 && thePlayer.level % 50 == 0) UpgradeSpell("Blizzard");
                    if (thePlayer.level > 1000 && thePlayer.level % 50 == 0) UpgradeSpell("Raze");
                    if (thePlayer.level > 5000 && thePlayer.level % 50 == 0) UpgradeSpell("Icicle");
                    break;
                case (int)SC.Professions.FIGHTER:
                    if (thePlayer.level >= 10)
                    {
                        newSpellKnown = KnowSpell("Detect Food") || newSpellKnown;
                        if (thePlayer.level >= 20)
                        {
                            newSpellKnown = KnowSpell("Heal") || newSpellKnown;
                            if (thePlayer.level >= 40)
                            {
                                newSpellKnown = KnowSpell("Freeze") || newSpellKnown;
                            }
                        }
                    }
                    if (thePlayer.level > 20 && thePlayer.level % 10 == 0) UpgradeSpell("Heal");
                    if (thePlayer.level > 40 && thePlayer.level % 20 == 0) UpgradeSpell("Freeze");
                    break;
            }
            return newSpellKnown;
        }

        public List<Spell> GetKnownSpells()
        {
            List<Spell> result = new List<Spell>();
            for (int i = 0; i < spellList.Count; i++)
            {
                if (spellList[i].isKnown)
                {
                    result.Add(spellList[i]);
                }
            }
            return result;
        }

        public int GetIndexOfKnownSpell(String spellName)
        {
            List<Spell> knownSpells = GetKnownSpells();
            for (int i = 0; i < knownSpells.Count; i++)
            {
                if (knownSpells[i].spellName == spellName)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool IsSpellKnown(String spellName)
        {
            for (int i = 0; i < spellList.Count; i++)
            {
                if (spellList[i].isKnown && spellList[i].spellName == spellName)
                {
                    return true;
                }
            }
            return false;
        }

        private bool KnowSpell(String spellName)
        {
            bool result = false;
            for (int i = 0; i < spellList.Count; i++)
            {
                if (spellList[i].spellName == spellName)
                {
                    if (!spellList[i].isKnown)
                    {
                        result = true;
                        spellList[i].isKnown = true;
                        break;
                    }
                    
                }
            }
            return result;
        }

        private void UpgradeSpell(String spellName)
        {
            for (int i = 0; i < spellList.Count; i++)
            {
                if (spellList[i].spellName == spellName)
                {
                    if (spellList[i].isKnown && spellList[i].level < MAX_SPELL_LEVEL)
                    {
                        if (spellName == "Fireball")
                            spellList[i].power += Math.Ceiling((spellList[i].power / spellList[i].level) / 3);
                        if (spellName == "Magic Missile")
                            spellList[i].power += Math.Ceiling(spellList[i].power / spellList[i].level);
                        spellList[i].level++;
                        spellList[i].castingCost = (int)((float)((float)spellList[i].castingCost / (float)(spellList[i].level - 1)) * spellList[i].level);
                    }

                }
            }
        }

        private void LoadSpells()
        {
            AddSpell(1, "Fireball", (int)SC.SpellCasting.DIRECTION, 4, 5);
            AddSpell(2, "Freeze", (int)SC.SpellCasting.DIRECTION, 5, 3);
            AddSpell(3, "Heal", (int)SC.SpellCasting.SELF, 10, 10);
            AddSpell(4, "Detect Gold", (int)SC.SpellCasting.SELF, 10, 1);
            AddSpell(5, "Detect Food", (int)SC.SpellCasting.SELF, 10, 1);
            AddSpell(6, "Detect Stairs", (int)SC.SpellCasting.SELF, 10, 1);
            AddSpell(7, "Teleport", (int)SC.SpellCasting.TARGET, 25, 1);
            AddSpell(8, "Magic Missile", (int)SC.SpellCasting.TARGET, 25, 15);
            AddSpell(9, "Inferno", (int)SC.SpellCasting.RADIUS, 150, 100);
            AddSpell(10, "Blizzard", (int)SC.SpellCasting.RADIUS, 150, 80);
            AddSpell(11, "Raze", (int)SC.SpellCasting.DIRECTION, 50, 250);
            AddSpell(12, "Icicle", (int)SC.SpellCasting.DIRECTION, 50, 180);
        }

        private void AddSpell(int id, String name, int castingType, int castCost, double pow)
        {
            if (spellList.Count > 0)
            {
                bool alreadyExists = false;
                for (int i = 0; i < spellList.Count; i++)
                {
                    if (spellList[i].spellName == name) alreadyExists = true;
                }
                if (!alreadyExists) spellList.Add(new Spell(id, name, castingType, castCost, pow));
            }
            else
            {
                spellList.Add(new Spell(id, name, castingType, castCost, pow));
            }

        }
    }
}