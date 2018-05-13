using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amadues
{
    public class SC
    {
        public const int DIRECTION_NONE = -1;
        public const int DIRECTION_UP = 0;
        public const int DIRECTION_UPRIGHT = 1;
        public const int DIRECTION_RIGHT = 2;
        public const int DIRECTION_DOWNRIGHT = 3;
        public const int DIRECTION_DOWN = 4;
        public const int DIRECTION_DOWNLEFT = 5;
        public const int DIRECTION_LEFT = 6;
        public const int DIRECTION_UPLEFT = 7;

        public const int FRAMES_BETWEEN_QUICKMOVE_LB = 10;
        public const int FRAMES_BETWEEN_QUICKMOVE = 30;

        public enum BGMShuffleOption { ONCE_PER_FLOOR, ONCE_PER_QUEST };
        public enum BGMPlayOption { ON, QUIET, OFF };

        public enum ItemTypes { WEAPON, SHIELD, ARMOUR, GOLD, CLOAK, SCROLL, POTION, FOOD, ROCK, QUEST };
        public const int numItemTypes = 10;

        public enum InventoryFilterType { ALL, WEAPON, ARMOUR, FOOD, POTION, SCROLL, OTHER };
        public const int numInventoryFilterTypes = 7;

        public enum RockTypes { ROCK, METAL, GEM, GRINDSTONE };
        public const int numRockTypes = 4;

        public enum FoodTypes { MEAT, VEG, BREAD, OTHER, CORPSE };
        public const int numFoodTypes = 5;

        public enum WeaponTypes { SWORD, DAGGER, MACE, SPEAR, BOW };
        public const int numWeaponTypes = 5;
        public const int RANGE_OF_BOW_WEAPONS = 10;

        public enum FurnTypes { TABLE, SIGNPOST, ALTAR, CANDELABRA, BARREL, STATUE, GRAVESTONE, CRATE, TELEPORTER, CHAIR_LEFT, CHAIR_RIGHT };
        public const int numFurnTypes = 11;

        public enum CreatureTypes {RODENT, BUG, SNAKE, CANINE, GOBLIN, FELINE, HUMAN, ELF, ORC, GHOST, 
                                    ZOMBIE, DEMON, BAT, LIZARD, DRAGON, MERCHANT, SUCCUBUS, SKELETON, SLIME, TROLL,
                                    GOLDRICK, CHEF, VILLAGER, QUESTMASTER, QUESTRESCUEE, SPIDER};
        public const int numCreatureTypes = 26;

        public enum TrapType { ARROW, POISON, TELEPORT, DESCENT };
        public const int numTrapTypes = 4;

        public enum Professions { FIGHTER, MAGE, EXPLORER };
        public enum Races { ORC, HUMAN, ELF };

        public enum SpellCasting { SELF, TARGET, DIRECTION, RADIUS };

        public enum QuestType { ITEM, RESCUE, ASSASSINATE, EXPLORE };
        public const int numQuestTypes = 4;

        public enum TileSet { CAVE, DUNGEON, BRICK, ICE, METAL, DESERT };
        public const int numTileSets = 6;

        public const int FEAT_KILL_ENEMIES = 0;
        public const int FEAT_KILL_DANGEROUS_ENEMIES = 1;
        public const int FEAT_KILL_FROZEN_ENEMIES = 2;
        public const int FEAT_KILL_BOSSES = 3;
        public const int FEAT_RECRUIT_ALLIES = 4;
        public const int FEAT_COMPLETE_QUESTS = 5;
        public const int FEAT_COMPLETE_FIGHTER_QUESTS = 6;
        public const int FEAT_COMPLETE_MAGE_QUESTS = 7;
        public const int FEAT_COMPLETE_EXPLORER_QUESTS = 8;
        public const int FEAT_COLLECT_GEMS = 9;

    }
}
