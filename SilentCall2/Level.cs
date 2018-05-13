using System;
using System.IO;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Amadues
{
    //[Serializable()]
    public class Level
    {
        public bool isVillage = false;
        public bool isBossLevel = false;
        public int DIMENSION = 40;
        public int VILLAGE_DIMENSION = 40;
        public const int PLAYERBUILDAREA_LEFT = 2;
        public const int PLAYERBUILDAREA_TOP = 26;
        public const int PLAYERBUILDAREA_WIDTH = 12;
        public const int PLAYERBUILDAREA_HEIGHT = 12;

        const float ITEM_CHANCE = 0.03f; //Chance of an item appearing on each tile. 3%?
        const float FURNITURE_CHANCE = 0.06f;
        const float CREATURE_CHANCE = 0.1f;
        const float TRAP_CHANCE = 0.01f;
        const float CHANCE_ARENA_OBSTACLE = 0.4f;
        const float CHANCE_DIG_RUBBLE = 0.2f;
        const int MAX_ITEMS_PER_TILE = 7;
        const int MAX_ROOMS = 160;
        const int MIN_ROOMS = 24;
        const byte KNOWN_MIN = 64; //The lower number that known tiles can fall back to
        const byte TILE_EARTH = 0;
        const byte TILE_WALL = 1;
        const byte TILE_DOOR_CLOSED = 2;
        const byte TILE_DOOR_OPEN = 3;
        const byte TILE_STAIRS_UP = 4;
        const byte TILE_STAIRS_DOWN = 5;
        const byte TILE_VILLAGE_GRASS = 0;
        const byte TILE_VILLAGE_WALL = 1;
        const byte TILE_VILLAGE_DOOR_CLOSED = 2;
        const byte TILE_VILLAGE_DOOR_OPEN = 3;
        const byte TILE_VILLAGE_HEDGE = 6;
        const byte TILE_VILLAGE_PATH = 7;
        const byte TILE_VILLAGE_WATER = 8;
        const byte TILE_VILLAGE_TREE = 9;
        const byte TILE_VILLAGE_FLOOR = 10;
        const byte TILE_VOID = 255;
        const int NUM_TILE_SETS = 6;
        const int METAL_TILE_SET = 4;
        public int currentTileSet = 0;
        public byte[][] tiles;
        public int[][] vision;
        public int levelNumber = 0;
        Random random;
        public int startingX, startingY, exitX, exitY = 0;
        public int playerPerception = 10;
        public int playerPerception2 = 100;

        //Boss level automated actions
        public bool doorRemoved = false;
        public bool exitHidden = true;

        //Level items
        public List<Item> itemList;
        public ItemGenerator itemGenerator;
        public List<Furniture> furnitureList;
        public List<Furniture> sightBlockingFurniture;
        public FurnitureGenerator furnitureGenerator;
        public List<Creature> creatureList;
        public Bestiary bestiary;
        public List<Trap> trapList;
        const int ITEM_TYPE_SWORD = 0;
        const int ITEM_TYPE_SHIELD = 1;
        const int ITEM_TYPE_ARMOUR = 2;
        const int ITEM_TYPE_GOLD = 3;
        const int ITEM_TYPE_CLOAK = 4;
        const int ITEM_TYPE_SCROLL = 5;
        const int ITEM_TYPE_POTION = 6;
        const int ITEM_TYPE_FOOD = 7;
        const int ITEM_TYPE_ROCK = 8;

        const int FOOD_TYPE_MEAT = 0;
        const int FOOD_TYPE_VEG = 1;
        const int FOOD_TYPE_BREAD = 2;
        const int FOOD_TYPE_OTHER = 3;
        const int FOOD_TYPE_CORPSE = 4;

        const int ROCK_TYPE_ROCK = 0;
        const int ROCK_TYPE_METAL = 1;
        const int ROCK_TYPE_GEM = 2;

        public bool playerHasPrayed = false;

        public Level()
        {
            tiles = new byte[DIMENSION][];
            vision = new int[DIMENSION][];
            for (int i = 0; i < (int)DIMENSION; i++)
            {
                tiles[i] = new byte[DIMENSION];
                vision[i] = new int[DIMENSION];
                for (int j = 0; j < DIMENSION; j++)
                {
                    tiles[i][j] = TILE_EARTH;
                    vision[i][j] = 0;
                }
            }
            random = new Random();
            itemList = new List<Item>();
            furnitureList = new List<Furniture>();
            creatureList = new List<Creature>();
            trapList = new List<Trap>();
            isVillage = false;
        }

        public Level(bool isVillageLevel, Bestiary aBestiary)
        {
            isVillage = isVillageLevel;
            bestiary = aBestiary;
            furnitureGenerator = new FurnitureGenerator();
            if (!isVillage)
            {
                tiles = new byte[DIMENSION][];
                vision = new int[DIMENSION][];
                for (int i = 0; i < (int)DIMENSION; i++)
                {
                    tiles[i] = new byte[DIMENSION];
                    vision[i] = new int[DIMENSION];
                    for (int j = 0; j < DIMENSION; j++)
                    {
                        tiles[i][j] = TILE_EARTH;
                        vision[i][j] = 0;
                    }
                }
                random = new Random();
                itemList = new List<Item>();
                furnitureList = new List<Furniture>();
                creatureList = new List<Creature>();
                trapList = new List<Trap>();
                startingX = 19;
                startingY = 19;
            }
            else
            {
                tiles = new byte[VILLAGE_DIMENSION][];
                vision = new int[VILLAGE_DIMENSION][];
                for (int i = 0; i < (int)VILLAGE_DIMENSION; i++)
                {
                    tiles[i] = new byte[VILLAGE_DIMENSION];
                    vision[i] = new int[VILLAGE_DIMENSION];
                    for (int j = 0; j < VILLAGE_DIMENSION; j++)
                    {
                        tiles[i][j] = TILE_EARTH;
                        vision[i][j] = 0;
                    }

                }
                random = new Random();
                itemList = new List<Item>();
                itemList = new List<Item>();
                creatureList = new List<Creature>();
                furnitureList = new List<Furniture>();
                trapList = new List<Trap>();
                LoadVillageMap();
                LoadVillageFurn();
                LoadVillageNPCs();
                startingX = 19;
                startingY = 19;
            }
        }

        public Level(int aLevelNum, Quest quest, ItemGenerator aItemGenerator, Bestiary aBestiary)
        {
            random = new Random();
            isVillage = false;
            levelNumber = aLevelNum;
            currentTileSet = quest.tileSet;
            itemGenerator = aItemGenerator;
            bestiary = aBestiary;
            furnitureGenerator = new FurnitureGenerator();
            itemList = new List<Item>();
            furnitureList = new List<Furniture>();
            creatureList = new List<Creature>();
            trapList = new List<Trap>();
            tiles = new byte[DIMENSION][];
            vision = new int[DIMENSION][];
            for (int i = 0; i < (int)DIMENSION; i++)
            {
                tiles[i] = new byte[DIMENSION];
                vision[i] = new int[DIMENSION];
                for (int j = 0; j < DIMENSION; j++)
                {
                    tiles[i][j] = TILE_WALL;
                    vision[i][j] = 0;
                }
            }
            if (quest.isBossQuest)
            {
                isBossLevel = true;
                //Generate Arena
                for (int i = 0; i < DIMENSION; i++)
                {
                    for (int j = 0; j < DIMENSION; j++)
                    {
                        if (DistanceFromCentre(i, j) < DIMENSION / 2 - 4)
                        {
                            tiles[i][j] = TILE_EARTH;
                        }
                    }
                }
                //Add some obstacles around the edge. Only every other space can be an obstacle
                for (int i = 0; i < DIMENSION; i += 2)
                {
                    for (int j = 0; j < DIMENSION; j += 2)
                    {
                        if (random.NextDouble() < CHANCE_ARENA_OBSTACLE && DistanceFromCentre(i, j) > DIMENSION - 30 && DistanceFromCentre(i, j) < DIMENSION - 4)
                        {
                            tiles[i][j] = TILE_WALL;
                        }
                    }
                }
                //Add starting room 
                for (int i = DIMENSION / 2 - 2; i < DIMENSION / 2 + 3; i++)
                {
                    for (int j = DIMENSION - 3; j < DIMENSION - 1; j++)
                    {
                        tiles[i][j] = TILE_EARTH;
                    }
                }
                tiles[DIMENSION / 2][DIMENSION - 4] = TILE_DOOR_CLOSED;
                startingX = DIMENSION / 2;
                startingY = DIMENSION - 2;

                //Summon the boss
                quest.questCreature.x = DIMENSION / 2;
                quest.questCreature.y = DIMENSION / 2;
                creatureList.Add(quest.questCreature);
            }
            else
            {
                //Generate rooms and room contents
                GenerateLevelRoomsAndContents();
                //Generate rocks inside walls
                for (int i = 0; i < DIMENSION; i++)
                {
                    for (int j = 0; j < DIMENSION; j++)
                    {
                        if (tiles[i][j] == TILE_WALL && random.NextDouble() < CHANCE_DIG_RUBBLE)
                        {
                            Item rock = itemGenerator.GenerateRock();
                            rock.x = i;
                            rock.y = j;
                            itemList.Add(rock);
                        }
                    }
                }
                //If final level for quest; add in quest items
                if (levelNumber == quest.level + quest.numFloors - 1)
                {
                    //Replace stairs with exit teleporter
                    for (int i = 0; i < DIMENSION; i++)
                    {
                        for (int j = 0; j < DIMENSION; j++)
                        {
                            if (tiles[i][j] == TILE_STAIRS_DOWN)
                            {
                                tiles[i][j] = TILE_EARTH;
                                furnitureList.Add(furnitureGenerator.GenerateTeleporter(i, j));
                            }
                        }
                    }
                    //Place quest objective(s)
                    Point aPoint = RandomEmptyRoomTile();
                    switch (quest.questType)
                    {
                        case (int)SC.QuestType.ASSASSINATE:
                            //Find an empty space with enough room for the creature
                            aPoint = EmptySpaceNear(aPoint.X, aPoint.Y, quest.questCreature.description.width, quest.questCreature.description.height);
                            quest.questCreature.x = aPoint.X;
                            quest.questCreature.y = aPoint.Y;
                            creatureList.Add(quest.questCreature);
                            break;
                        case (int)SC.QuestType.RESCUE:
                            //Find an empty space with enough room for the creature
                            aPoint = EmptySpaceNear(aPoint.X, aPoint.Y, quest.questCreature.description.width, quest.questCreature.description.height);
                            quest.questCreature.x = aPoint.X;
                            quest.questCreature.y = aPoint.Y;
                            creatureList.Add(quest.questCreature);
                            break;
                        case (int)SC.QuestType.ITEM:
                            quest.questItem.x = aPoint.X;
                            quest.questItem.y = aPoint.Y;
                            itemList.Add(quest.questItem);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public Level(int aLevelNum, ItemGenerator aItemGenerator, Bestiary aBestiary)
        {
            random = new Random();
            isVillage = false;
            levelNumber = aLevelNum;
            currentTileSet = random.Next(0, NUM_TILE_SETS);
            itemGenerator = aItemGenerator;
            bestiary = aBestiary;
            furnitureGenerator = new FurnitureGenerator();
            itemList = new List<Item>();
            furnitureList = new List<Furniture>();
            creatureList = new List<Creature>();
            trapList = new List<Trap>();
            tiles = new byte[DIMENSION][];
            vision = new int[DIMENSION][];
            for (int i = 0; i < (int)DIMENSION; i++)
            {
                tiles[i] = new byte[DIMENSION];
                vision[i] = new int[DIMENSION];
                for (int j = 0; j < DIMENSION; j++)
                {
                    tiles[i][j] = TILE_WALL;
                    vision[i][j] = 0;
                }
                GenerateLevelRoomsAndContents();
            }
            //Generate rocks inside walls
            for (int i = 0; i < DIMENSION; i++)
            {
                for (int j = 0; j < DIMENSION; j++)
                {
                    if (tiles[i][j] == TILE_WALL && random.NextDouble() < CHANCE_DIG_RUBBLE)
                    {
                        Item rock = itemGenerator.GenerateRock();
                        rock.x = i;
                        rock.y = j;
                        itemList.Add(rock);
                    }
                }
            }
        }

        public void GenerateLevelRoomsAndContents()
        {
            int numRooms = random.Next(MIN_ROOMS, MAX_ROOMS);
            int lastRoomWidth = 0, lastRoomHeight = 0, lastRoomTop = 0, lastRoomLeft = 0;
            for (int i = 0; i < numRooms; i++)
            {
                //Randomly define a room
                int roomWidth = random.Next(2, 8);
                int roomHeight = random.Next(2, 8);
                int roomTop = random.Next(1, DIMENSION - roomHeight - 1);
                int roomLeft = random.Next(1, DIMENSION - roomWidth - 1);
                //Check if that space is empty
                bool spaceOK = true;
                for (int j = roomLeft - 1; j <= roomLeft + roomWidth; j++)
                {
                    for (int k = roomTop - 1; k <= roomTop + roomHeight; k++)
                    {
                        if (tiles[j][k] != TILE_WALL) spaceOK = false;
                    }
                }
                if (spaceOK)
                {
                    for (int j = roomLeft; j < roomLeft + roomWidth; j++)
                    {
                        for (int k = roomTop; k < roomTop + roomHeight; k++)
                        {
                            tiles[j][k] = TILE_EARTH;
                        }
                    }
                    //if first room, set the level start somewhere in this room
                    //TODO: Check that i==0 is suitable - will first room ALWAYS be generated OK?
                    if (i == 0)
                    {
                        startingX = random.Next(roomLeft, roomLeft + roomWidth);
                        startingY = random.Next(roomTop, roomTop + roomHeight);
                        //Currently not planning to allow backtracking
                        //tiles[startingX, startingY] = TILE_STAIRS_UP;
                    }

                    //Set the exit somewhere in this room.
                    //TODO: Assuming here that at least 2 rooms will be created, so start and exit will be
                    //      in different rooms, and therefore cannot be on the same spot.
                    //      Actually there is a tiny possibility that only 1 room will be valid
                    exitX = random.Next(roomLeft, roomLeft + roomWidth);
                    exitY = random.Next(roomTop, roomTop + roomHeight);

                    //Create a corridor to the last room
                    if (i != 0)
                    {
                        if (roomLeft > lastRoomLeft + lastRoomWidth - 1)
                        {
                            //Dig left until we hit an empty space or are vertically aligned with
                            //the last room
                            int digX = roomLeft - 1;
                            int digY = random.Next(roomTop, roomTop + roomHeight);
                            tiles[digX][digY] = TILE_DOOR_CLOSED;
                            digX -= 1;
                            bool struckEmpty = false;
                            while ((tiles[digX][digY] == TILE_WALL && tiles[digX + 1][digY - 1] == TILE_WALL
                                && tiles[digX + 1][digY + 1] == TILE_WALL)
                                && digX >= (lastRoomLeft + lastRoomWidth - 1))
                            {
                                tiles[digX][digY] = TILE_EARTH;
                                PossiblyPlaceTrap(digX, digY);
                                digX -= 1;

                            }
                            //If the first tile we looked at was a door then the while above
                            //would not be run, but struckEmpty should be true
                            //So this boolean check needs to be outside the while loop.
                            if (tiles[digX][digY] != TILE_WALL
                                    || tiles[digX + 1][digY - 1] != TILE_WALL
                                    || tiles[digX + 1][digY + 1] != TILE_WALL)
                                struckEmpty = true;
                            //if we didn't hit an empty tile, then we're either above or below the last room
                            if (!struckEmpty)
                            {
                                int debugX = digX;
                                int debugY = digY;
                                //digX needs to go back one, since it's looking at the first solid tile
                                //digX += 1;
                                if (digY > lastRoomTop + lastRoomHeight)
                                {
                                    //Dig Up
                                    //digY -= 1;
                                    while (tiles[digX][digY] == TILE_WALL)
                                    {
                                        tiles[digX][digY] = TILE_EARTH;
                                        PossiblyPlaceTrap(digX, digY);
                                        digY -= 1;
                                    }
                                    tiles[digX][digY + 1] = TILE_DOOR_CLOSED;
                                }
                                else
                                {
                                    //Dig Down
                                    //digY += 1;
                                    while (tiles[digX][digY] == TILE_WALL)
                                    {
                                        tiles[digX][digY] = TILE_EARTH;
                                        PossiblyPlaceTrap(digX, digY);
                                        digY += 1;
                                    }
                                    tiles[digX][digY - 1] = TILE_DOOR_CLOSED;
                                }
                            }
                            //else we did strike empty. If we struck the last room, place a door
                            else
                            {
                                tiles[digX + 1][digY] = TILE_DOOR_CLOSED;
                            }
                        }
                        else //Last room is NOT to the left, either right or aligned
                        {
                            if (roomLeft + roomWidth - 1 < lastRoomLeft)
                            {
                                //Dig right until we hit an empty space or are vertically aligned with
                                //the last room
                                int digX = roomLeft + roomWidth;
                                int digY = random.Next(roomTop, roomTop + roomHeight - 1);
                                tiles[digX][digY] = TILE_DOOR_CLOSED;
                                digX += 1;
                                bool struckEmpty = false;
                                while ((tiles[digX][digY] == TILE_WALL && tiles[digX - 1][digY - 1] == TILE_WALL
                                    && tiles[digX - 1][digY + 1] == TILE_WALL)
                                    && digX <= (lastRoomLeft))
                                {
                                    tiles[digX][digY] = TILE_EARTH;
                                    PossiblyPlaceTrap(digX, digY);
                                    digX += 1;
                                    //if (tiles[digX, digY] != TILE_WALL) struckEmpty = true;
                                }
                                //If the first tile we looked at was a door then the while above
                                //would not be run, but struckEmpty should be true
                                //So this boolean check needs to be outside the while loop.
                                if (tiles[digX][digY] != TILE_WALL
                                    || tiles[digX - 1][digY - 1] != TILE_WALL
                                    || tiles[digX - 1][digY + 1] != TILE_WALL) struckEmpty = true;
                                //if we didn't hit an empty tile, then we're either above or below the last room
                                if (!struckEmpty)
                                {

                                    //debug values, note the point where we stopped digging right
                                    int debugX = digX;
                                    int debugY = digY;

                                    bool struckEmpty2 = false;

                                    //We're currently looking at the first invalid tile, 
                                    //Go back one X to get to the last valid tile
                                    digX--;
                                    //THIS LINE NEEDS CHANGED - IS CURRENT digX, digY
                                    //ABOVE OR BELOW THE LAST ROOM?
                                    if (digY > lastRoomTop + lastRoomHeight)
                                    {
                                        //Dig Up
                                        digY -= 1;
                                        while (tiles[digX][digY] == TILE_WALL && !struckEmpty2)
                                        {
                                            tiles[digX][digY] = TILE_EARTH;
                                            PossiblyPlaceTrap(digX, digY);
                                            digY -= 1;
                                            //If we found a space to the left or right, stop digging.
                                            //Need to also ensure we're above the current room,
                                            //or we might just have dug into the side of the current room!
                                            if (digY < roomTop - 1 && (tiles[digX - 1][digY + 1] != TILE_WALL
                                                || tiles[digX + 1][digY + 1] != TILE_WALL))
                                                struckEmpty2 = true;
                                        }
                                        //if we hit the room, place a door
                                        if (digX >= lastRoomLeft
                                            && digX < lastRoomLeft + lastRoomWidth
                                            && digY == lastRoomTop + lastRoomHeight - 1)
                                        {
                                            tiles[digX][digY + 1] = TILE_DOOR_CLOSED;
                                        }
                                    }
                                    else
                                    {
                                        //Dig Down
                                        digY += 1;
                                        while (tiles[digX][digY] == TILE_WALL && !struckEmpty2)
                                        {
                                            tiles[digX][digY] = TILE_EARTH;
                                            PossiblyPlaceTrap(digX, digY);
                                            digY += 1;
                                            //If we found a space to the left or right, stop digging.
                                            //Need to also ensure we're above the current room,
                                            //or we might just have dug into the side of the current room!
                                            if (digY > roomTop + roomHeight && (tiles[digX - 1][digY - 1] != TILE_WALL
                                                || tiles[digX + 1][digY - 1] != TILE_WALL))
                                                struckEmpty2 = true;
                                        }
                                        //if we hit the room, place a door
                                        if (digX >= lastRoomLeft
                                            && digX < lastRoomLeft + lastRoomWidth
                                            && digY == lastRoomTop)
                                        {
                                            tiles[digX][digY - 1] = TILE_DOOR_CLOSED;
                                        }
                                    }
                                }
                                //else we did strike empty, if we're at the last room
                                //then place a door
                                else
                                {

                                    tiles[digX - 1][digY] = TILE_DOOR_CLOSED;

                                }
                            }
                            else //Isn't left or right, must be above or below
                            {
                                if (roomTop > lastRoomTop)
                                {
                                    //Dig Up!
                                    int digX = random.Next(Math.Max(roomLeft, lastRoomLeft), Math.Min((roomLeft + roomWidth), (lastRoomLeft + lastRoomWidth)));
                                    int digY = roomTop - 1;
                                    tiles[digX][digY] = TILE_DOOR_CLOSED;
                                    digY -= 1;
                                    while (tiles[digX][digY] == TILE_WALL)
                                    {
                                        tiles[digX][digY] = TILE_EARTH;
                                        PossiblyPlaceTrap(digX, digY);
                                        digY -= 1;
                                    }
                                    if (digY == lastRoomTop + lastRoomHeight - 1)
                                    {
                                        tiles[digX][digY + 1] = TILE_DOOR_CLOSED;
                                    }
                                }
                                else //Dig down
                                {
                                    int digX = random.Next(Math.Max(roomLeft, lastRoomLeft), Math.Min((roomLeft + roomWidth), (lastRoomLeft + lastRoomWidth)));
                                    int digY = roomTop + roomHeight;
                                    tiles[digX][digY] = TILE_DOOR_CLOSED;
                                    digY += 1;
                                    while (tiles[digX][digY] == TILE_WALL) // && digY < (DIMENSION - 1))
                                    {
                                        tiles[digX][digY] = TILE_EARTH;
                                        PossiblyPlaceTrap(digX, digY);
                                        digY += 1;
                                    }
                                    if (digY == lastRoomTop)
                                    {
                                        tiles[digX][digY - 1] = TILE_DOOR_CLOSED;
                                    }
                                }
                            }
                        }
                    }

                    //Generate furniture
                    Furniture aFurniture;
                    if (roomWidth > 2 && roomHeight > 2)
                    {
                        for (int j = roomLeft + 1; j < roomLeft + roomWidth - 1; j++)
                        {
                            for (int k = roomTop + 1; k < roomTop + roomHeight - 1; k++)
                            {
                                if (((float)random.Next(0, 100)) / 100f < FURNITURE_CHANCE)
                                {
                                    if (j != startingX || k != startingY)
                                    {
                                        aFurniture = furnitureGenerator.randomFurniture();
                                        aFurniture.x = j;
                                        aFurniture.y = k;
                                        furnitureList.Add(aFurniture);
                                    }
                                }
                            }
                        }
                    }

                    //Generate Items
                    Item anItem;
                    for (int j = roomLeft; j < roomLeft + roomWidth; j++)
                    {
                        for (int k = roomTop; k < roomTop + roomHeight; k++)
                        {
                            if (CanMove(j, k))
                            {
                                if (((float)random.Next(0, 100)) / 100f < ITEM_CHANCE)
                                {
                                    anItem = itemGenerator.GenerateRandomItem(random.Next(levelNumber + 1, levelNumber + random.Next(2, 10)));
                                    anItem.x = j;
                                    anItem.y = k;
                                    itemList.Add(anItem);
                                }
                            }
                        }
                    }

                    //Generate Creatures
                    if (i != 0)
                    {
                        Creature aCreature;
                        for (int j = roomLeft; j < roomLeft + roomWidth; j++)
                        {
                            for (int k = roomTop; k < roomTop + roomHeight; k++)
                            {
                                if (((float)random.Next(0, 100)) / 100f < CREATURE_CHANCE)
                                {
                                    aCreature = bestiary.CreateCreature(levelNumber);
                                    aCreature.x = j;
                                    aCreature.y = k;
                                    if (aCreature.description.width + j - 1 < roomLeft + roomWidth
                                        && aCreature.description.height + k - 1 < roomTop + roomHeight
                                        && CanMove(j, k, aCreature.description.width, aCreature.description.height, -1))
                                        creatureList.Add(aCreature);
                                }
                            }
                        }
                    }

                    //This room is now the last room (for the next iteration)
                    lastRoomHeight = roomHeight;
                    lastRoomLeft = roomLeft;
                    lastRoomTop = roomTop;
                    lastRoomWidth = roomWidth;
                }
            }
            //Add stairs down
            tiles[exitX][exitY] = TILE_STAIRS_DOWN;
            //If furniture exists on top of exit, remove it!
            if (furnitureList.Count > 0)
            {
                for (int j = 0; j < furnitureList.Count; j++)
                {
                    if (furnitureList[j].x == exitX && furnitureList[j].y == exitY)
                    {
                        furnitureList.RemoveAt(j);
                        break;
                    }
                }
            }
        }

        private void LoadVillageMap()
        {
            for (int i = 0; i < VILLAGE_DIMENSION; i++)
            {
                for (int j = 0; j < VILLAGE_DIMENSION; j++)
                {
                    tiles[i][j] = TILE_VILLAGE_WATER;
                }
            }
            for (int i = 1; i < VILLAGE_DIMENSION - 1; i++)
            {
                for (int j = 1; j < VILLAGE_DIMENSION - 1; j++)
                {
                    tiles[i][j] = TILE_VILLAGE_GRASS;
                }
            }
            tiles[0][0] = TILE_VILLAGE_TREE;
            tiles[1][0] = TILE_VILLAGE_TREE;
            tiles[2][0] = TILE_VILLAGE_TREE;
            tiles[3][0] = TILE_VILLAGE_TREE;
            tiles[4][0] = TILE_VILLAGE_TREE;
            tiles[5][0] = TILE_VILLAGE_TREE;
            tiles[6][0] = TILE_VILLAGE_TREE;
            tiles[7][0] = TILE_VILLAGE_TREE;
            tiles[8][0] = TILE_VILLAGE_TREE;
            tiles[0][1] = TILE_VILLAGE_TREE;
            tiles[1][1] = TILE_VILLAGE_TREE;
            tiles[2][1] = TILE_VILLAGE_TREE;
            tiles[3][1] = TILE_VILLAGE_TREE;
            tiles[4][1] = TILE_VILLAGE_TREE;
            tiles[5][1] = TILE_VILLAGE_TREE;
            tiles[6][1] = TILE_VILLAGE_TREE;
            tiles[7][1] = TILE_VILLAGE_TREE;
            tiles[8][1] = TILE_VILLAGE_WATER;
            tiles[9][1] = TILE_VILLAGE_WATER;
            tiles[10][1] = TILE_VILLAGE_WATER;
            tiles[0][2] = TILE_VILLAGE_TREE;
            tiles[1][2] = TILE_VILLAGE_TREE;
            tiles[2][2] = TILE_VILLAGE_TREE;
            tiles[3][2] = TILE_VILLAGE_TREE;
            tiles[4][2] = TILE_VILLAGE_TREE;
            tiles[5][2] = TILE_VILLAGE_TREE;
            tiles[6][2] = TILE_VILLAGE_TREE;
            tiles[7][2] = TILE_VILLAGE_WATER;
            tiles[8][2] = TILE_VILLAGE_WATER;
            tiles[9][2] = TILE_VILLAGE_WATER;
            tiles[0][3] = TILE_VILLAGE_TREE;
            tiles[1][3] = TILE_VILLAGE_TREE;
            tiles[2][3] = TILE_VILLAGE_TREE;
            tiles[3][3] = TILE_VILLAGE_TREE;
            tiles[4][3] = TILE_VILLAGE_TREE;
            tiles[5][3] = TILE_VILLAGE_TREE;
            tiles[6][3] = TILE_VILLAGE_WATER;
            tiles[7][3] = TILE_VILLAGE_WATER;
            tiles[8][3] = TILE_VILLAGE_WATER;
            tiles[0][4] = TILE_VILLAGE_TREE;
            tiles[1][4] = TILE_VILLAGE_TREE;
            tiles[2][4] = TILE_VILLAGE_TREE;
            tiles[3][4] = TILE_VILLAGE_TREE;
            tiles[4][4] = TILE_VILLAGE_TREE;
            tiles[5][4] = TILE_VILLAGE_WATER;
            tiles[6][4] = TILE_VILLAGE_WATER;
            tiles[7][4] = TILE_VILLAGE_WATER;
            tiles[0][5] = TILE_VILLAGE_TREE;
            tiles[1][5] = TILE_VILLAGE_TREE;
            tiles[2][5] = TILE_VILLAGE_TREE;
            tiles[3][5] = TILE_VILLAGE_TREE;
            tiles[4][5] = TILE_VILLAGE_WATER;
            tiles[5][5] = TILE_VILLAGE_WATER;
            tiles[6][5] = TILE_VILLAGE_WATER;
            tiles[0][6] = TILE_VILLAGE_TREE;
            tiles[1][6] = TILE_VILLAGE_TREE;
            tiles[2][6] = TILE_VILLAGE_TREE;
            tiles[3][6] = TILE_VILLAGE_WATER;
            tiles[4][6] = TILE_VILLAGE_WATER;
            tiles[5][6] = TILE_VILLAGE_WATER;
            tiles[0][7] = TILE_VILLAGE_TREE;
            tiles[1][7] = TILE_VILLAGE_TREE;
            tiles[2][7] = TILE_VILLAGE_WATER;
            tiles[3][7] = TILE_VILLAGE_WATER;
            tiles[4][7] = TILE_VILLAGE_WATER;
            tiles[0][8] = TILE_VILLAGE_TREE;
            tiles[1][8] = TILE_VILLAGE_WATER;
            tiles[2][8] = TILE_VILLAGE_WATER;
            tiles[3][8] = TILE_VILLAGE_WATER;
            tiles[0][9] = TILE_VILLAGE_WATER;
            tiles[1][9] = TILE_VILLAGE_WATER;
            tiles[2][9] = TILE_VILLAGE_WATER;
            tiles[0][10] = TILE_VILLAGE_WATER;
            tiles[1][10] = TILE_VILLAGE_WATER;


            //Main path down centre of village
            for (int i = 0; i < 33; i++)
            {
                tiles[19][i] = TILE_VILLAGE_PATH;
                tiles[20][i] = TILE_VILLAGE_PATH;
            }

            //Adventurers' guild
            for (int i = 23; i < 33; i++)
            {
                for (int j = 2; j < 12; j++)
                {
                    tiles[i][j] = TILE_VILLAGE_WALL;
                }
            }
            for (int i = 24; i < 32; i++)
            {
                for (int j = 3; j < 11; j++)
                {
                    tiles[i][j] = TILE_VILLAGE_FLOOR;
                }
            }
            tiles[24][4] = TILE_VILLAGE_WALL;
            tiles[25][4] = TILE_VILLAGE_WALL;
            tiles[26][4] = TILE_VILLAGE_WALL;
            tiles[28][4] = TILE_VILLAGE_WALL;
            tiles[28][3] = TILE_VILLAGE_WALL;
            tiles[29][4] = TILE_VILLAGE_WALL;
            tiles[30][4] = TILE_VILLAGE_WALL;
            tiles[31][4] = TILE_VILLAGE_WALL;
            tiles[26][3] = TILE_VILLAGE_DOOR_CLOSED;

            tiles[21][6] = TILE_VILLAGE_PATH;
            tiles[22][6] = TILE_VILLAGE_PATH;
            tiles[23][6] = TILE_VILLAGE_DOOR_CLOSED;

            //Shop
            for (int i = 9; i < 17; i++)
            {
                for (int j = 8; j < 17; j++)
                {
                    tiles[i][j] = TILE_VILLAGE_WALL;
                }
            }
            for (int i = 10; i < 16; i++)
            {
                for (int j = 9; j < 16; j++)
                {
                    tiles[i][j] = TILE_VILLAGE_FLOOR;
                }
            }
            tiles[16][12] = TILE_VILLAGE_DOOR_CLOSED;
            tiles[17][12] = TILE_VILLAGE_PATH;
            tiles[18][12] = TILE_VILLAGE_PATH;

            //NPC house
            for (int i = 10; i < 17; i++)
            {
                for (int j = 18; j < 25; j++)
                {
                    tiles[i][j] = TILE_VILLAGE_WALL;
                }
            }
            for (int i = 11; i < 16; i++)
            {
                for (int j = 19; j < 24; j++)
                {
                    tiles[i][j] = TILE_VILLAGE_FLOOR;
                }
            }
            tiles[16][21] = TILE_VILLAGE_DOOR_CLOSED;
            tiles[17][21] = TILE_VILLAGE_PATH;
            tiles[18][21] = TILE_VILLAGE_PATH;

            //Player build area
            for (int i = 2; i < 14; i++)
            {
                for (int j = 26; j < 38; j++)
                {
                    tiles[i][j] = TILE_VILLAGE_FLOOR;
                }
            }
            for (int i = 14; i < 19; i++)
            {
                for (int j = 31; j < 33; j++)
                {
                    tiles[i][j] = TILE_VILLAGE_FLOOR;
                }
            }

            //Fountain area
            for (int i = 23; i < 34; i++)
            {
                for (int j = 14; j < 25; j++)
                {
                    tiles[i][j] = TILE_VILLAGE_HEDGE;
                }
            }
            for (int i = 24; i < 33; i++)
            {
                for (int j = 15; j < 24; j++)
                {
                    tiles[i][j] = TILE_VILLAGE_GRASS;
                }
            }
            for (int i = 26; i < 31; i++)
            {
                for (int j = 17; j < 22; j++)
                {
                    tiles[i][j] = TILE_VILLAGE_PATH;
                }
            }
            for (int i = 27; i < 30; i++)
            {
                for (int j = 18; j < 21; j++)
                {
                    tiles[i][j] = TILE_VILLAGE_WATER;
                }
            }
            tiles[28][19] = TILE_VILLAGE_PATH;
            for (int i = 21; i < 26; i++)
            {
                for (int j = 19; j < 21; j++)
                {
                    tiles[i][j] = TILE_VILLAGE_PATH;
                }
            }

            //NPC House
            for (int i = 23; i < 28; i++)
            {
                for (int j = 27; j < 32; j++)
                {
                    tiles[i][j] = TILE_VILLAGE_WALL;
                }
            }
            for (int i = 24; i < 27; i++)
            {
                for (int j = 28; j < 31; j++)
                {
                    tiles[i][j] = TILE_VILLAGE_FLOOR;
                }
            }
            tiles[23][29] = TILE_VILLAGE_DOOR_CLOSED;
            tiles[22][29] = TILE_VILLAGE_PATH;
            tiles[21][29] = TILE_VILLAGE_PATH;
        }

        private void LoadVillageFurn()
        {
            furnitureList.Add(furnitureGenerator.GenerateSignpost(22, 5, "WELCOME TO THE ADVENTURERS GUILD"));
            furnitureList.Add(furnitureGenerator.GenerateSignpost(17, 11, "VILLAGE SHOP"));
            furnitureList.Add(furnitureGenerator.GenerateTeleporter(19, 1));
            furnitureList.Add(furnitureGenerator.GenerateTeleporter(20, 1));
            furnitureList.Add(furnitureGenerator.GenerateStatue(28, 19));
            furnitureList.Add(furnitureGenerator.GenerateRightFacingChair(28, 7));
            furnitureList.Add(furnitureGenerator.GenerateRightFacingChair(28, 9));
            furnitureList.Add(furnitureGenerator.GenerateLeftFacingChair(30, 7));
            furnitureList.Add(furnitureGenerator.GenerateLeftFacingChair(30, 9));
            furnitureList.Add(furnitureGenerator.GenerateTable(29, 7));
            furnitureList.Add(furnitureGenerator.GenerateTable(29, 9));
            furnitureList.Add(furnitureGenerator.GenerateRightFacingChair(12, 22));
            furnitureList.Add(furnitureGenerator.GenerateLeftFacingChair(14, 22));
            furnitureList.Add(furnitureGenerator.GenerateTable(13, 22));
            furnitureList.Add(furnitureGenerator.GenerateRightFacingChair(24, 30));
            furnitureList.Add(furnitureGenerator.GenerateLeftFacingChair(26, 30));
            furnitureList.Add(furnitureGenerator.GenerateTable(25, 30));

            furnitureList.Add(furnitureGenerator.GenerateTable(10, 11));
            furnitureList.Add(furnitureGenerator.GenerateTable(10, 13));

            furnitureList.Add(furnitureGenerator.GenerateCrate(12, 7));
            furnitureList.Add(furnitureGenerator.GenerateCrate(13, 7));
            furnitureList.Add(furnitureGenerator.GenerateCrate(14, 7));
            furnitureList.Add(furnitureGenerator.GenerateCrate(15, 7));
            furnitureList.Add(furnitureGenerator.GenerateCrate(13, 6));
            furnitureList.Add(furnitureGenerator.GenerateCrate(14, 6));

            furnitureList.Add(furnitureGenerator.GenerateCandelabra(24, 5));
            furnitureList.Add(furnitureGenerator.GenerateCandelabra(24, 7));
            furnitureList.Add(furnitureGenerator.GenerateCandelabra(31, 10));
            furnitureList.Add(furnitureGenerator.GenerateCandelabra(15, 11));
            furnitureList.Add(furnitureGenerator.GenerateCandelabra(15, 13));
            furnitureList.Add(furnitureGenerator.GenerateCandelabra(15, 20));
            furnitureList.Add(furnitureGenerator.GenerateCandelabra(15, 22));
            furnitureList.Add(furnitureGenerator.GenerateCandelabra(24, 28));
            //furnitureList.Add(furnitureGenerator.GenerateCandelabra(24, 30));

            furnitureList.Add(furnitureGenerator.GenerateBarrel(24, 32));
            furnitureList.Add(furnitureGenerator.GenerateBarrel(25, 32));
            furnitureList.Add(furnitureGenerator.GenerateBarrel(26, 32));
            furnitureList.Add(furnitureGenerator.GenerateBarrel(24, 33));
            furnitureList.Add(furnitureGenerator.GenerateBarrel(25, 33));
        }

        private void LoadVillageNPCs()
        {
            creatureList.Add(bestiary.CreateMerchant());
            creatureList.ElementAt<Creature>(creatureList.Count() - 1).x = 10;
            creatureList.ElementAt<Creature>(creatureList.Count() - 1).y = 12;
            creatureList.Add(bestiary.CreateQuestMaster(27, 4));
            creatureList.Add(bestiary.CreateVillager(29, 6));
            creatureList.Add(bestiary.CreateVillager(21, 12));
        }

        //A function to return a byte[,] of tiles for a given offset and a given height and width
        //Any tiles outside the map will be returned as TILE_VOID
        public byte[,] GetTiles(int left, int top, int width, int height)
        {
            byte[,] result = new byte[width, height];

            for (int i = left; i < left + width; i++)
            {
                for (int j = top; j < top + height; j++)
                {
                    if (i < 0 || j < 0 || i >= DIMENSION || j >= DIMENSION)
                    {
                        result[i - left, j - top] = TILE_VOID;
                    }
                    else
                    {
                        result[i - left, j - top] = tiles[i][j];
                    }
                }
            }

            return result;
        }

        public byte[][] GetVillageBuildArea()
        {
            byte[][] result = new byte[12][];
            for (int i = 0; i < 12; i++)
            {
                result[i] = new byte[12];
                for (int j = 0; j < 12; j++)
                {
                    result[i][j] = tiles[i + 2][j + 26];
                }
            }
            return result;
        }
        public void SetVillageBuildArea(byte[][] loadedTiles)
        {
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    tiles[i + 2][j + 26] = loadedTiles[i][j];
                }
            }
        }

        public void SpamVillageBuildArea()
        {
            if (!isVillage) return;
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        DropItem(itemGenerator.GenerateTinnedFood(), i + 2, j + 26);
                    }
                }
            }
        }

        //TODO: Slow code! Calculates division which could be in a CONST instead
        // Uses sqrt
        public int DistanceFromCentre(int x, int y)
        {
            int centreX = DIMENSION / 2;
            int centreY = DIMENSION / 2;
            int result = (int)Math.Sqrt((centreX - x) * (centreX - x) + (centreY - y) * (centreY - y));
            return result;
        }

        public void UpdateListOfSightBlockingFurniture(int aX, int aY, int sight)
        {
            sightBlockingFurniture = new List<Furniture>();
            if (furnitureList.Count > 0)
            {
                for (int i = 0; i < furnitureList.Count; i++)
                {
                    if (!furnitureList[i].CanSeeThrough)
                    {
                        int distance2 = (furnitureList[i].x - aX) * (furnitureList[i].x - aX) + (furnitureList[i].y - aY) * (furnitureList[i].y - aY);
                        if (distance2 <= playerPerception2)
                        {
                            sightBlockingFurniture.Add(furnitureList[i]);
                        }
                    }
                }
            }
        }

        public void UpdateVision(int aX, int aY, int sight)
        {
            UpdateListOfSightBlockingFurniture(aX, aY, sight);
            for (int i = 0; i < DIMENSION; i++)
            {
                for (int j = 0; j < DIMENSION; j++)
                {
                    if ((i == aX && j == aY) || IsEdgeOfSight(aX, aY, i, j))
                    {
                        int distance2 = (i - aX) * (i - aX) + (j - aY) * (j - aY);
                        if (distance2 <= playerPerception2)
                        {
                            vision[i][j] = 255;
                        }
                        else
                        {
                            if (vision[i][j] > KNOWN_MIN) vision[i][j] = vision[i][j] - 8;
                        }
                    }
                    else
                    {
                        if (vision[i][j] > KNOWN_MIN) vision[i][j] = vision[i][j] - 8;
                    }
                }
            }

            if (creatureList.Count > 0)
            {
                for (int i = 0; i < creatureList.Count; i++)
                {
                    for (int ix = creatureList[i].x; ix < creatureList[i].x + creatureList[i].description.width; ix++)
                    {
                        for (int iy = creatureList[i].y; iy < creatureList[i].y + creatureList[i].description.height; iy++)
                        {
                            if ((ix == aX && iy == aY) || IsLineOfSight(aX, aY, ix, iy))
                            {
                                int distance2 = (ix - aX) * (ix - aX) + (iy - aY) * (iy - aY);
                                if (distance2 <= playerPerception2)
                                {
                                    creatureList[i].visibility = 255;
                                }
                                else
                                {
                                    if (creatureList[i].visibility > KNOWN_MIN) creatureList[i].visibility -= 8;
                                }
                            }
                            else
                            {
                                if (creatureList[i].visibility > KNOWN_MIN) creatureList[i].visibility -= 8;
                            }
                        }
                    }
                }
            }

            if (itemList.Count > 0)
            {
                for (int i = 0; i < itemList.Count; i++)
                {
                    if ((itemList[i].x == aX && itemList[i].y == aY) || IsLineOfSight(aX, aY, itemList[i].x, itemList[i].y))
                    {
                        int distance2 = (itemList[i].x - aX) * (itemList[i].x - aX) + (itemList[i].y - aY) * (itemList[i].y - aY);
                        if (distance2 <= playerPerception2)
                        {
                            itemList[i].visibility = 255;
                        }
                        else
                        {
                            if (itemList[i].visibility > KNOWN_MIN) itemList[i].visibility -= 8;
                        }
                    }
                    else
                    {
                        if (itemList[i].visibility > KNOWN_MIN) itemList[i].visibility -= 8;
                    }
                }
            }

            if (furnitureList.Count > 0)
            {
                for (int i = 0; i < furnitureList.Count; i++)
                {
                    if ((furnitureList[i].x == aX && furnitureList[i].y == aY) || IsEdgeOfSight(aX, aY, furnitureList[i].x, furnitureList[i].y))
                    {
                        int distance2 = (furnitureList[i].x - aX) * (furnitureList[i].x - aX) + (furnitureList[i].y - aY) * (furnitureList[i].y - aY);
                        if (distance2 <= playerPerception2)
                        {
                            furnitureList[i].visibility = 255;
                        }
                        else
                        {
                            if (furnitureList[i].visibility > KNOWN_MIN) furnitureList[i].visibility -= 8;
                        }
                    }
                    else
                    {
                        if (furnitureList[i].visibility > KNOWN_MIN) furnitureList[i].visibility -= 8;
                    }
                }
            }
        }

        public int[,] GetTileVision(int left, int top, int width, int height)
        {
            int[,] result = new int[width, height];

            for (int i = left; i < left + width; i++)
            {
                for (int j = top; j < top + height; j++)
                {
                    if (i < 0 || j < 0 || i >= DIMENSION || j >= DIMENSION)
                    {
                        result[i - left, j - top] = 0;
                    }
                    else
                    {
                        result[i - left, j - top] = vision[i][j];
                    }
                }
            }

            return result;
        }

        public int GetTileVision(int x, int y)
        {
            if (x < 0 || y < 0 || x > DIMENSION - 1 || y > DIMENSION - 1) return 0;
            return vision[x][y];
        }

        public void ResetVision()
        {
            for (int i = 0; i < (int)DIMENSION; i++)
            {
                vision[i] = new int[DIMENSION];
                for (int j = 0; j < DIMENSION; j++)
                {
                    vision[i][j] = 0;
                }
            }
            if (creatureList.Count > 0)
            {
                for (int i = 0; i < creatureList.Count; i++)
                {
                    creatureList[i].visibility = 0;
                }
            }
            if (itemList.Count > 0)
            {
                for (int i = 0; i < itemList.Count; i++)
                {
                    itemList[i].visibility = 0;
                }
            }
            if (furnitureList.Count > 0)
            {
                for (int i = 0; i < furnitureList.Count; i++)
                {
                    furnitureList[i].visibility = 0;
                }
            }
        }

        //If we want an array of known tiles, the size must be known
        //to declare the array
        public int KnownMapWidth()
        {
            return RightMostKnown() - LeftMostKnown() + 1;
        }
        public int KnownMapHeight()
        {
            int test1 = BottomMostKnown();
            int test2 = TopMostKnown();
            return BottomMostKnown() - TopMostKnown() + 1;
        }

        //The first column of the map to contain a known tile
        public int LeftMostKnown()
        {
            int left = -1;
            for (int i = 0; i < DIMENSION; i++)
            {
                for (int j = 0; j < DIMENSION; j++)
                {
                    if (vision[i][j] > 0)
                    {
                        left = i;
                        break;
                    }
                }
                if (left >= 0) break;
            }
            return Math.Min(left, LeftMostKnownCreature());
        }

        public int LeftMostKnownCreature()
        {
            int result = DIMENSION;
            if (creatureList.Count > 0)
            {
                for (int i = 0; i < creatureList.Count; i++)
                {
                    if (creatureList[i].visibility > 0)
                    {
                        if (creatureList[i].x < result) result = creatureList[i].x;
                    }
                }
            }
            return result;
        }

        //The last column of the map to contain a known tile
        public int RightMostKnown()
        {
            int right = -1;
            for (int i = DIMENSION - 1; i >= 0; i--)
            {
                for (int j = DIMENSION - 1; j >= 0; j--)
                {
                    if (vision[i][j] > 0)
                    {
                        right = i;
                        break;
                    }
                }
                if (right >= 0) break;
            }
            return Math.Max(right, RightMostKnownCreature());
        }

        public int RightMostKnownCreature()
        {
            int result = 0;
            if (creatureList.Count > 0)
            {
                for (int i = 0; i < creatureList.Count; i++)
                {
                    if (creatureList[i].visibility > 0)
                    {
                        if (creatureList[i].x > result) result = creatureList[i].x;
                    }
                }
            }
            return result;
        }

        //The first row of the map to contain a known tile
        public int TopMostKnown()
        {
            int top = -1;
            for (int i = 0; i < DIMENSION; i++)
            {
                for (int j = 0; j < DIMENSION; j++)
                {
                    if (vision[j][i] > 0)
                    {
                        top = i;
                        break;
                    }

                }
                if (top >= 0) break;
            }
            return Math.Min(top, TopMostKnownCreature());
        }

        public int TopMostKnownCreature()
        {
            int result = DIMENSION;
            if (creatureList.Count > 0)
            {
                for (int i = 0; i < creatureList.Count; i++)
                {
                    if (creatureList[i].visibility > 0)
                    {
                        if (creatureList[i].y < result) result = creatureList[i].y;
                    }
                }
            }
            return result;
        }

        //The last row of the map to contain a known tile
        public int BottomMostKnown()
        {
            int bottom = -1;
            for (int i = DIMENSION - 1; i >= 0; i--)
            {
                for (int j = DIMENSION - 1; j >= 0; j--)
                {
                    if (vision[j][i] > 0)
                    {
                        bottom = i;
                        break;
                    }

                }
                if (bottom >= 0) break;
            }
            return Math.Max(bottom, BottomMostKnownCreature());
        }

        public int BottomMostKnownCreature()
        {
            int result = 0;
            if (creatureList.Count > 0)
            {
                for (int i = 0; i < creatureList.Count; i++)
                {
                    if (creatureList[i].visibility > 0)
                    {
                        if (creatureList[i].y > result) result = creatureList[i].y;
                    }
                }
            }
            return result;
        }

        //Get an array of bytes for the known expanse of the map
        //Caller must know the dimensions of the result by calling
        //KnownMapWidth() and KnownMapHeight()
        public byte[,] KnownMap()
        {
            //int test1 = KnownMapWidth();
            //int test2 = KnownMapHeight();
            byte[,] result = new byte[KnownMapWidth(), KnownMapHeight()];

            for (int i = 0; i < KnownMapWidth(); i++)
            {
                for (int j = 0; j < KnownMapHeight(); j++)
                {
                    if (vision[i + LeftMostKnown()][j + TopMostKnown()] > 0)
                    {
                        result[i, j] = tiles[i + LeftMostKnown()][j + TopMostKnown()];
                    }
                    else
                    {
                        result[i, j] = TILE_VOID;
                    }
                }
            }

            return result;
        }

        public List<Item> GetItems(int left, int top, int width, int height)
        {
            List<Item> result = new List<Item>();
            if (itemList != null)
            {
                if (itemList.Count > 0)
                {
                    for (int i = 0; i < itemList.Count; i++)
                    {
                        if (itemList[i].x >= left
                            && itemList[i].x < left + width
                            && itemList[i].y >= top
                            && itemList[i].y < top + height)
                        {
                            result.Add(itemList[i]);
                        }
                    }
                }
            }
            return result;
        }

        public List<Item> GetItems(int aX, int aY)
        {
            List<Item> result = new List<Item>();
            if (itemList != null)
            {
                if (itemList.Count > 0)
                {
                    for (int i = 0; i < itemList.Count; i++)
                    {
                        if (itemList[i].x == aX && itemList[i].y == aY)
                        {
                            result.Add(itemList[i]);
                        }
                    }
                }
            }
            return result;
        }

        public List<Item> GetSavableItemsInPlayerBuildArea()
        {
            List<Item> result = new List<Item>();
            if (!isVillage) return result;
            if (itemList.Count == 0) return result;
            for (int item = 0; item < itemList.Count; item++)
            {
                if (PlayerCanBuild(itemList[item].x, itemList[item].y) && !itemList[item].loadOut)
                {
                    result.Add(itemList[item]);
                }
            }
            return result;
        }

        public void LoadItemsToPlayerBuildArea(List<Item> items)
        {
            if (items != null && items.Count > 0)
            {
                for (int item = 0; item < items.Count; item++)
                {
                    itemList.Add(items[item]);
                }
            }
        }

        public List<Item> GetKnownItems(int aX, int aY)
        {
            List<Item> result = new List<Item>();
            if (itemList != null)
            {
                if (itemList.Count > 0)
                {
                    for (int i = 0; i < itemList.Count; i++)
                    {
                        if (itemList[i].x == aX && itemList[i].y == aY && itemList[i].visibility > 0)
                        {
                            result.Add(itemList[i]);
                        }
                    }
                }
            }
            return result;
        }

        public Furniture GetFurniture(int aX, int aY)
        {
            if (furnitureList != null)
            {
                if (furnitureList.Count > 0)
                {
                    for (int i = 0; i < furnitureList.Count; i++)
                    {
                        if (furnitureList[i].x == aX && furnitureList[i].y == aY) return furnitureList[i];
                    }
                }
            }
            return null;
        }

        public List<Furniture> GetPassibleFurniture(int left, int top, int width, int height)
        {
            List<Furniture> result = new List<Furniture>();
            if (furnitureList != null)
            {
                if (furnitureList.Count > 0)
                {
                    for (int i = 0; i < furnitureList.Count; i++)
                    {
                        if (furnitureList[i].x >= left
                            && furnitureList[i].x < left + width
                            && furnitureList[i].y >= top
                            && furnitureList[i].y < top + height
                            && furnitureList[i].CanWalkOver)
                        {
                            result.Add(furnitureList[i]);
                        }
                    }
                }
            }
            return result;
        }

        public List<Furniture> GetImpassibleFurniture(int left, int top, int width, int height)
        {
            List<Furniture> result = new List<Furniture>();
            if (furnitureList != null)
            {
                if (furnitureList.Count > 0)
                {
                    for (int i = 0; i < furnitureList.Count; i++)
                    {
                        if (furnitureList[i].x >= left
                            && furnitureList[i].x < left + width
                            && furnitureList[i].y >= top
                            && furnitureList[i].y < top + height
                            && !furnitureList[i].CanWalkOver)
                        {
                            result.Add(furnitureList[i]);
                        }
                    }
                }
            }
            return result;
        }

        public List<Furniture> GetFurniture(int left, int top, int width, int height)
        {
            List<Furniture> result = new List<Furniture>();
            if (furnitureList != null)
            {
                if (furnitureList.Count > 0)
                {
                    for (int i = 0; i < furnitureList.Count; i++)
                    {
                        if (furnitureList[i].x >= left
                            && furnitureList[i].x < left + width
                            && furnitureList[i].y >= top
                            && furnitureList[i].y < top + height)
                        {
                            result.Add(furnitureList[i]);
                        }
                    }
                }
            }
            return result;
        }


        //Get the creature standing at a spot
        //null if no creature - caller must check for null!
        public Creature GetCreature(int aX, int aY)
        {
            if (creatureList.Count > 0)
            {
                for (int i = 0; i < creatureList.Count; i++)
                {
                    if (creatureList[i].x <= aX
                        && creatureList[i].x + creatureList[i].description.width - 1 >= aX
                        && creatureList[i].y <= aY
                        && creatureList[i].y + creatureList[i].description.height - 1 >= aY) return creatureList[i];
                }
            }
            return null;
        }

        public int GetCreatureVisibilityAtTile(int aX, int aY)
        {
            if (creatureList == null) return 0;
            if (creatureList.Count == 0) return 0;
            int result = 0;
            for (int i = 0; i < creatureList.Count; i++)
            {
                if (creatureList[i].x <= aX
                    && creatureList[i].x + creatureList[i].description.width - 1 >= aX
                    && creatureList[i].y <= aY
                    && creatureList[i].y + creatureList[i].description.height - 1 >= aY)
                {
                    if (creatureList[i].visibility > result) result = creatureList[i].visibility;
                }
            }
            return result;
        }

        public Creature GetKnownCreature(int aX, int aY)
        {
            if (creatureList.Count > 0)
            {
                for (int i = 0; i < creatureList.Count; i++)
                {
                    if (creatureList[i].visibility > 0
                        && creatureList[i].x <= aX
                        && creatureList[i].x + creatureList[i].description.width - 1 >= aX
                        && creatureList[i].y <= aY
                        && creatureList[i].y + creatureList[i].description.height - 1 >= aY) return creatureList[i];
                }
            }
            return null;
        }

        public int GetCreatureIndex(int aX, int aY)
        {
            if (creatureList.Count > 0)
            {
                for (int i = 0; i < creatureList.Count; i++)
                {
                    if (creatureList[i].x <= aX
                        && creatureList[i].x + creatureList[i].description.width - 1 >= aX
                        && creatureList[i].y <= aY
                        && creatureList[i].y + creatureList[i].description.height - 1 >= aY) return i;
                }
            }
            return -1;
        }

        public Item GetItem(int index, int aX, int aY)
        {
            if (itemList != null)
            {
                if (itemList.Count > 0)
                {
                    int itemsCounted = 0;
                    for (int i = 0; i < itemList.Count; i++)
                    {
                        if (itemList[i].x == aX && itemList[i].y == aY)
                        {
                            if (index == itemsCounted)
                            {
                                return itemList[i];
                            }
                            else
                            {
                                itemsCounted++;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public int GetCorpseIndexAtXY(int aX, int aY)
        {
            if (itemList != null)
            {
                if (itemList.Count > 0)
                {
                    for (int i = 0; i < itemList.Count; i++)
                    {
                        if (itemList[i].x == aX && itemList[i].y == aY
                            && itemList[i].itemType == ITEM_TYPE_FOOD
                            && itemList[i].foodType == FOOD_TYPE_CORPSE)
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        public int GetEthicalCorpseIndexAtXY(int aX, int aY)
        {
            if (itemList != null)
            {
                if (itemList.Count > 0)
                {
                    for (int i = 0; i < itemList.Count; i++)
                    {
                        if (itemList[i].x == aX && itemList[i].y == aY
                            && itemList[i].itemType == ITEM_TYPE_FOOD
                            && itemList[i].foodType == FOOD_TYPE_CORPSE
                            && IsEthicalToEat(itemList[i]))
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        //return the index of a corpse that is ethical to eat and
        //visible to x, y
        public int GetIndexOfVisibleEthicalCorpse(int aX, int aY)
        {
            int result = -1;
            if (itemList != null)
            {
                if (itemList.Count > 0)
                {
                    for (int i = 0; i < itemList.Count; i++)
                    {
                        if (itemList[i].itemType == ITEM_TYPE_FOOD
                            && itemList[i].foodType == FOOD_TYPE_CORPSE
                            && IsEthicalToEat(itemList[i])
                            && IsLineOfSight(aX, aY, itemList[i].x, itemList[i].y))
                        {
                            return i;
                        }
                    }
                }
            }
            return result;
        }

        public bool IsEthicalToEat(Item theItem)
        {
            //TODO - Including non-meat corpses here just because
            //I want to exclude them from 
            if (theItem.itemName == "Elf Corpse") return false;
            if (theItem.itemName == "Orc Corpse") return false;
            if (theItem.itemName == "Human Corpse") return false;
            if (theItem.itemName == "Ectoplasm") return false;
            if (theItem.itemName == "Slime Membrane") return false;
            return true;
        }

        public int GetItemVisibilityAtTile(int aX, int aY)
        {
            if (itemList == null) return 0;
            if (itemList.Count == 0) return 0;
            int max = 0;
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].x == aX && itemList[i].y == aY)
                {
                    if (itemList[i].visibility > max) max = itemList[i].visibility;
                }
            }
            return max;
        }
        public void RemoveItem(int index, int aX, int aY)
        {
            if (itemList != null)
            {
                if (itemList.Count > 0)
                {
                    int itemsCounted = 0;
                    for (int i = 0; i < itemList.Count; i++)
                    {
                        if (itemList[i].x == aX && itemList[i].y == aY)
                        {
                            if (index == itemsCounted)
                            {
                                itemList.RemoveAt(i);
                                break;
                            }
                            else
                            {
                                itemsCounted++;
                            }
                        }
                    }
                }
            }
        }

        //Drop an item on the ground. False if item cannot be dropped;
        //true (and item added to level) if it can be
        public bool DropItem(Item aItem, int aX, int aY)
        {
            if (!isVillage && tiles[aX][aY] != TILE_EARTH)
            {
                return false;
            }
            if (isVillage)
            {
                if (tiles[aX][aY] != TILE_VILLAGE_FLOOR
                    && tiles[aX][aY] != TILE_VILLAGE_GRASS
                    && tiles[aX][aY] != TILE_VILLAGE_PATH)
                    return false;
            }
            if (GetItems(aX, aY).Count >= MAX_ITEMS_PER_TILE)
            {
                return false;
            }
            itemList.Add(aItem);
            itemList[itemList.Count - 1].x = aX;
            itemList[itemList.Count - 1].y = aY;
            itemList[itemList.Count - 1].visibility = 255;
            return true;
        }

        public byte GetTile(int aX, int aY)
        {
            return tiles[aX][aY];
        }

        public bool IsTileSolid(int aX, int aY)
        {
            if (aX >= DIMENSION || aX <= 0 || aY <= 0 || aY >= DIMENSION) return true;
            if (tiles[aX][aY] == TILE_DOOR_CLOSED || tiles[aX][aY] == TILE_WALL) return true;
            return false;
        }

        public List<Creature> GetCreatures(int left, int top, int width, int height)
        {
            List<Creature> result = new List<Creature>();
            bool found = false; //Need a bool to mark if a large creature is found, and not count it multiple times
            if (creatureList != null)
            {
                if (creatureList.Count > 0)
                {
                    for (int i = 0; i < creatureList.Count; i++)
                    {
                        found = false;
                        for (int j = creatureList[i].x; j < creatureList[i].x + creatureList[i].description.width; j++)
                        {
                            for (int k = creatureList[i].y; k < creatureList[i].y + creatureList[i].description.height; k++)
                            {
                                if (j >= left
                                    && j < left + width
                                    && k >= top
                                    && k < top + height)
                                {
                                    result.Add(creatureList[i]);
                                    found = true;
                                    break;
                                }
                                if (found) break;
                            }
                            if (found) break;
                        }
                    }
                }
            }
            return result;
        }

        public List<Creature> GetCreatures(int aX, int aY)
        {
            List<Creature> result = new List<Creature>();
            if (creatureList != null)
            {
                if (creatureList.Count > 0)
                {
                    for (int i = 0; i < creatureList.Count; i++)
                    {
                        if (creatureList[i].x == aX && creatureList[i].y == aY)
                        {
                            result.Add(creatureList[i]);
                        }
                    }
                }
            }
            return result;
        }

        public Point GetHostileCreaturePointNextTo(int aX, int aY)
        {
            if (creatureList.Count > 0)
            {
                for (int i = 0; i < creatureList.Count; i++)
                {
                    if (creatureList[i].hostile)
                    {
                        for (int j = creatureList[i].x; j < creatureList[i].x + creatureList[i].description.width; j++)
                        {
                            for (int k = creatureList[i].y; k < creatureList[i].y + creatureList[i].description.height; k++)
                            {
                                if (Math.Abs(j - aX) <= 1 && Math.Abs(k - aY) <= 1)
                                {
                                    return new Point(j, k);
                                }
                            }
                        }
                    }
                }
            }

            return new Point(-1,-1);
        }

        public List<Trap> GetTraps(int left, int top, int width, int height)
        {
            List<Trap> result = new List<Trap>();
            if (trapList != null)
            {
                if (trapList.Count > 0)
                {
                    for (int i = 0; i < trapList.Count; i++)
                    {
                        if (trapList[i].x >= left
                            && trapList[i].x < left + width
                            && trapList[i].y >= top
                            && trapList[i].y < top + height)
                        {
                            result.Add(trapList[i]);
                        }
                    }
                }
            }
            return result;
        }

        public Trap GetTrap(int x, int y)
        {
            if (trapList != null)
            {
                if (trapList.Count > 0)
                {
                    for (int i = 0; i < trapList.Count; i++)
                    {
                        if (trapList[i].x == x && trapList[i].y == y) return trapList[i];
                    }
                }
            }
            return null;
        }

        public bool IsTrapAt(int x, int y)
        {
            if (trapList != null)
            {
                if (trapList.Count > 0)
                {
                    for (int i = 0; i < trapList.Count; i++)
                    {
                        if (trapList[i].x == x && trapList[i].y == y) return true;
                    }
                }
            }
            return false;
        }

        public bool PlaceMerchant()
        {
            if (creatureList == null) return false;
            Point randomPoint = RandomEmptyRoomTile();
            if (randomPoint.X == 0 && randomPoint.Y == 0) return false;
            Creature merchant = bestiary.CreateMerchant();
            merchant.x = randomPoint.X;
            merchant.y = randomPoint.Y;
            creatureList.Add(merchant);
            return true;
        }

        public void KnowMerchant()
        {
            for (int i = 0; i < creatureList.Count; i++)
            {
                if (creatureList[i].description.creatureType == (int)SC.CreatureTypes.MERCHANT)
                {
                    creatureList[i].visibility = 255;
                }
            }
        }
        public bool MerchantExists()
        {
            for (int i = 0; i < creatureList.Count; i++)
            {
                if (creatureList[i].description.creatureType == (int)SC.CreatureTypes.MERCHANT)
                {
                    return true;
                }
            }
            return false;
        }
        public bool MerchantKnown()
        {
            for (int i = 0; i < creatureList.Count; i++)
            {
                if (creatureList[i].description.creatureType == (int)SC.CreatureTypes.MERCHANT)
                {
                    return creatureList[i].visibility > 0;
                }
            }
            return false;
        }
        public int MerchantX()
        {
            for (int i = 0; i < creatureList.Count; i++)
            {
                if (creatureList[i].description.creatureType == (int)SC.CreatureTypes.MERCHANT)
                {
                    return creatureList[i].x;
                }
            }
            return -1;
        }
        public int MerchantY()
        {
            for (int i = 0; i < creatureList.Count; i++)
            {
                if (creatureList[i].description.creatureType == (int)SC.CreatureTypes.MERCHANT)
                {
                    return creatureList[i].y;
                }
            }
            return -1;
        }

        //Kill a random hostile creature, return the exp for the death
        //return 0 if nothing to kill or other failure
        public double KillRandomHostile()
        {
            if (creatureList.Count == 0) return 0;
            int countOfHostiles = 0;
            for (int i = 0; i < creatureList.Count; i++)
            {
                if (creatureList[i].hostile) countOfHostiles++;
            }
            if (countOfHostiles == 0) return 0;
            if (countOfHostiles == 1)
            {
                for (int i = 0; i < creatureList.Count; i++)
                {
                    if (creatureList[i].hostile)
                    {
                        double result = creatureList[i].description.experience;
                        creatureList.RemoveAt(i);
                        return result;
                    }
                }
            }
            int enemyToKill = random.Next(1, countOfHostiles);
            int secondCount = 0;
            for (int i = 0; i < creatureList.Count; i++)
            {
                if (creatureList[i].hostile) secondCount++;
                if (secondCount == enemyToKill)
                {
                    double result = creatureList[i].description.experience;
                    creatureList.RemoveAt(i);
                    return result;
                }
            }
            return 0;
        }

        public double KillAllHostiles()
        {
            if (isBossLevel) return 0;
            double result = 0;
            if (creatureList.Count == 0) return 0;
            for (int i = creatureList.Count -1; i >=0; i--)
            {
                if (creatureList[i].hostile)
                {
                    result += creatureList[i].description.experience;
                    creatureList.RemoveAt(i);
                }
            }
            return result;
        }

        //Return the neareast empty point near aX, aY
        public Point EmptyPointNear(int aX, int aY)
        {
            return EmptySpaceNear(aX, aY, 1, 1);           
        }

        //Return an empty space near to aX, aY that has an empty space 'width' to the right and 'height' below
        public Point EmptySpaceNear(int aX, int aY, int width, int height)
        {
            int tryDistance = 1;
            int tryDistance2 = 1;
            int maxDistance = 20;

            while (tryDistance < maxDistance)
            {
                for (int i = 0; i < DIMENSION; i++)
                {
                    for (int j = 0; j < DIMENSION; j++)
                    {
                        if ((i - aX) * (i - aX) + (j - aY) * (j - aY) <= tryDistance2
                            && (i != aX || j != aY)
                            && CanMove(i, j))
                        {
                            bool spaceIsOK = true;
                            for (int k = i; k < i + width; k++)
                            {
                                for (int l = j; l < j + height; l++)
                                {
                                    if (!CanMove(k,l)) spaceIsOK = false;
                                }
                            }
                            if (spaceIsOK) return new Point(i, j);
                        }
                    }
                }
                tryDistance++;
                tryDistance2 = tryDistance * tryDistance;
            }
            return new Point(0, 0);
        }

        public bool SummonAltarDaemonNearPlayer(int playerX, int playerY)
        {
            bool result = false;
            Point possiblePoint = EmptySpaceNear(playerX, playerY, 
                                    bestiary.SummonAltarDaemon(1,0,0).description.width,
                                    bestiary.SummonAltarDaemon(1, 0, 0).description.height);
            //Check for 0,0 as this is what EmptyPointNear returns if no point possible
            if (CanSee(playerX, playerY, possiblePoint.X, possiblePoint.Y)
                && (possiblePoint.X != 0 || possiblePoint.Y != 0))
            {
                if (CanMove(possiblePoint.X, possiblePoint.Y + 1)
                    && CanMove(possiblePoint.X + 1, possiblePoint.Y)
                    && CanMove(possiblePoint.X + 1, possiblePoint.Y + 1))
                {
                    if (playerX < possiblePoint.X || playerX > possiblePoint.X + 1 || 
                        playerY < possiblePoint.Y || playerY > possiblePoint.Y + 1)
                    {
                        creatureList.Add(bestiary.SummonAltarDaemon(levelNumber, possiblePoint.X, possiblePoint.Y));
                        result = true;
                    }
                }
            }
            return result;
        }

        public void PossiblyPlaceTrap(int aX, int aY)
        {
            if (random.NextDouble() <= TRAP_CHANCE)
            {
                trapList.Add(new Trap(aX, aY, random.Next(0, SC.numTrapTypes)));
            }
        }
        public void PlaceTrap(int aX, int aY)
        {
            trapList.Add(new Trap(aX, aY, random.Next(0, SC.numTrapTypes)));
        }

        public void TrapMania()
        {
            for (int i = 0; i < DIMENSION; i++)
            {
                for (int j = 0; j < DIMENSION; j++)
                {
                    if (CanMove(i, j))
                    {
                        if (!IsTrapAt(i,j))
                        {
                            PlaceTrap(i, j);
                        }
                    }
                }
            }
        }

        public void PlaceItems(List<Item> listOfItems)
        {
            if (listOfItems == null || listOfItems.Count == 0) return;
            Point pointToPlace;
            for (int i = 0; i < listOfItems.Count; i++)
            {
                if (this.isBossLevel)
                {
                    pointToPlace = EmptyPointNear(startingX, startingY);
                }
                else
                {
                    pointToPlace = this.RandomEmptyTile();
                }
                listOfItems[i].x = pointToPlace.X;
                listOfItems[i].y = pointToPlace.Y;
                listOfItems[i].visibility = 0;
                itemList.Add(listOfItems[i]);
            }
        }

        public void RemoveDoor()
        {
            for (int i = 0; i < DIMENSION; i++)
            {
                for (int j = 0; j < DIMENSION; j++)
                {
                    if (tiles[i][j] == TILE_DOOR_CLOSED || tiles[i][j] == TILE_DOOR_OPEN)
                    {
                        tiles[i][j] = TILE_WALL;
                    }
                }
            }
            doorRemoved = true;
        }

        public void ReplaceDoor()
        {
            tiles[DIMENSION / 2][DIMENSION - 4] = TILE_DOOR_CLOSED;
            doorRemoved = false;
        }

        public void MakeExitAppear()
        {
            furnitureList.Add(furnitureGenerator.GenerateTeleporter(DIMENSION/2, DIMENSION/2));
            exitHidden = false;
        }

        public Point RandomEmptyTile()
        {
            Point result = new Point();

            int countOfEmptyTiles = 0;
            for (int i = 0; i < DIMENSION; i++)
            {
                for (int j = 0; j < DIMENSION; j++)
                {
                    if (CanMove(i, j)) countOfEmptyTiles++;
                }
            }
            int randomChoice = random.Next(1, countOfEmptyTiles);
            int recount = 0;
            for (int i = 0; i < DIMENSION; i++)
            {
                for (int j = 0; j < DIMENSION; j++)
                {
                    if (CanMove(i, j))
                    {
                        recount++;
                        if (recount == randomChoice)
                        {
                            result.X = i;
                            result.Y = j;
                        }
                    }
                }
            }
            return result;
        }

        public Point RandomEmptyRoomTile()
        {
            Point result = new Point();

            int countOfEmptyTiles = 0;
            for (int i = 0; i < DIMENSION; i++)
            {
                for (int j = 0; j < DIMENSION; j++)
                {
                    if (IsEmptyRoomTile(i, j) && (i != startingX || j != startingY)) countOfEmptyTiles++;
                }
            }
            int randomChoice = random.Next(1, countOfEmptyTiles);
            int recount = 0;
            for (int i = 0; i < DIMENSION; i++)
            {
                for (int j = 0; j < DIMENSION; j++)
                {
                    if (IsEmptyRoomTile(i, j) && (i != startingX || j != startingY))
                    {
                        recount++;
                        if (recount == randomChoice)
                        {
                            result.X = i;
                            result.Y = j;
                        }
                    }
                }
            }
            return result;
        }

        private bool IsEmptyRoomTile(int aX, int aY)
        {
            return CanMove(aX, aY) 
                && NumSurroundingSpaces(aX, aY) > 2;
        }

        //For a tile x,y; how many floor tiles surround it.
        //Determines if a tile is in a room or a corridor
        private int NumSurroundingSpaces(int x, int y)
        {
            int result = 0;

            if (x > 0)
            {
                if (y > 0)
                {
                    if (tiles[x - 1][y - 1] == TILE_EARTH) result++;
                }
                if (tiles[x - 1][y] == TILE_EARTH) result++;
                if (y < DIMENSION - 1)
                {
                    if (tiles[x - 1][y + 1] == TILE_EARTH) result++;
                }
            }
            if (y > 0)
            {
                if (tiles[x][y - 1] == TILE_EARTH) result++;
            }
            if (y < DIMENSION - 1)
            {
                if (tiles[x][y + 1] == TILE_EARTH) result++;
            }
            if (x < DIMENSION - 1)
            {
                if (y > 0)
                {
                    if (tiles[x + 1][y - 1] == TILE_EARTH) result++;
                }
                if (tiles[x + 1][y] == TILE_EARTH) result++;
                if (y < DIMENSION - 1)
                {
                    if (tiles[x + 1][y + 1] == TILE_EARTH) result++;
                }
            }
            return result;
        }

        public bool AreAllHostilesDead()
        {
            bool result = true;
            if (creatureList.Count > 0)
            {
                for (int i = 0; i < creatureList.Count; i++)
                {
                    if (creatureList[i].alive && creatureList[i].hostile) result = false;
                }
            }
            return result;
        }

        //Return if at least one ally (other than any quest rescuee) exist in the level
        //This is so the game can warn if allies are being lost when quest is turned in
        public bool AllyExists()
        {
            bool result = false;
            if (creatureList.Count > 0)
            {
                for (int i = 0; i < creatureList.Count; i++)
                {
                    if (creatureList[i].alive && creatureList[i].allied
                        && (creatureList[i].description.creatureType != (int)SC.CreatureTypes.QUESTRESCUEE))
                        result = true;
                }
            }
            return result;
        }

        public bool CanMove(int aX, int aY)
        {
            //Firstly, return false if new co-ordinate is out of map
            if (isVillage)
            {
                if (aX < 0 || aY < 0 || aX >= VILLAGE_DIMENSION || aY >= VILLAGE_DIMENSION) return false;
            }
            else
            {
                if (aX < 0 || aY < 0 || aX >= DIMENSION || aY >= DIMENSION) return false;
            }

            if (furnitureList.Count > 0)
            {
                for (int i = 0; i < furnitureList.Count; i++)
                {
                    if (furnitureList[i].x == aX
                        && furnitureList[i].y == aY
                        && !furnitureList[i].CanWalkOver) return false;
                }
            }
            if (creatureList.Count > 0)
            {
                for (int i = 0; i < creatureList.Count; i++)
                {
                    if (creatureList[i].x <= aX
                        && creatureList[i].x + creatureList[i].description.width - 1 >= aX
                        && creatureList[i].y <= aY
                        && creatureList[i].y + creatureList[i].description.height - 1 >= aY) return false;
                }
            }
            if (isVillage)
            {
                return tiles[aX][aY] == TILE_VILLAGE_DOOR_OPEN
                    || tiles[aX][aY] == TILE_VILLAGE_FLOOR
                    || tiles[aX][aY] == TILE_VILLAGE_GRASS
                    || tiles[aX][aY] == TILE_VILLAGE_PATH;
            }
            else
            {
                return tiles[aX][aY] == TILE_EARTH
                    || tiles[aX][aY] == TILE_DOOR_OPEN
                    || tiles[aX][aY] == TILE_STAIRS_DOWN
                    || tiles[aX][aY] == TILE_STAIRS_UP;
            }
        }
        //CanMove for creatures of variable size
        //Need index of creature so that it is not checked against itself
        public bool CanMove(int aX, int aY, int width, int height, int index)
        {
            bool result = true;

            if (furnitureList.Count > 0)
            {
                for (int i = 0; i < furnitureList.Count; i++)
                {
                    if (furnitureList[i].x <= aX + width - 1
                        && furnitureList[i].x >= aX
                        && furnitureList[i].y <= aY + height - 1
                        && furnitureList[i].y >= aY
                        && !furnitureList[i].CanWalkOver) return false;
                }
            }

            if (creatureList.Count > 0)
            {
                for (int i = 0; i < creatureList.Count; i++)
                {
                    if (i != index)
                    {

                        if (creatureList[i].x <= aX + width - 1
                            && aX <= creatureList[i].x + creatureList[i].description.width - 1
                            && creatureList[i].y <= aY + height - 1
                            && aY <= creatureList[i].y + creatureList[i].description.height - 1) return false;
                    }
                }
            }

            for (int i = aX; i < aX + width; i++)
            {
                for (int j = aY; j < aY + height; j++)
                {
                    result = result && (tiles[i][j] == TILE_EARTH
                || tiles[i][j] == TILE_DOOR_OPEN
                || tiles[i][j] == TILE_STAIRS_DOWN
                || tiles[i][j] == TILE_STAIRS_UP);
                }
            }

            return result;
        }

        public bool CanDig(int aX, int aY)
        {
            if (aX <= 1 || aY <= 1 || aX >= DIMENSION - 1 || aY >= DIMENSION - 1) return false;
            if (GetTile(aX, aY) != TILE_WALL) return false;
            if (currentTileSet == METAL_TILE_SET) return false;
            return true;
        }

        public void Dig(int aX, int aY)
        {
            if (CanDig(aX, aY))
            {
                tiles[aX][aY] = TILE_EARTH;
            }
        }

        //Return if x,y is inside the village's player build area
        public bool PlayerCanBuild(int x, int y)
        {
            return x >= PLAYERBUILDAREA_LEFT 
                    && x < PLAYERBUILDAREA_LEFT + PLAYERBUILDAREA_WIDTH 
                    && y >= PLAYERBUILDAREA_TOP 
                    && y < PLAYERBUILDAREA_TOP + PLAYERBUILDAREA_HEIGHT;
        }

        public bool Open(int aX, int aY)
        {
            if (aX >= DIMENSION || aY >= DIMENSION) return false;
            if (tiles[aX][aY] == TILE_DOOR_CLOSED)
            {
                tiles[aX][aY] = TILE_DOOR_OPEN;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Close(int aX, int aY)
        {
            if (aX >= DIMENSION || aY >= DIMENSION) return false;
            if (!CanMove(aX, aY)) return false;
            if (tiles[aX][aY] == TILE_DOOR_OPEN)
            {
                tiles[aX][aY] = TILE_DOOR_CLOSED;
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<String> GetTileContextMenuOptions(int aX, int aY)
        {
            List<String> aList = new List<string>();
            if (tiles[aX][aY] == TILE_DOOR_CLOSED)
            {
                aList.Add("Open");
            }
            if (tiles[aX][aY] == TILE_DOOR_OPEN)
            {
                aList.Add("Close");
            }
            return aList;
        }

        public void SetPerception(int a)
        {
            playerPerception = a;
            playerPerception2 = a * a;
        }

        //Mark all tiles with gold as known.
        //Return false if no gold was found
        //bool parm states whether metals inside walls should be detected
        public bool KnowGold(bool insideWalls)
        {
            bool result = false;

            if (itemList.Count > 0)
            {
                for (int i = 0; i < itemList.Count; i++)
                {
                    if (itemList[i].itemType == ITEM_TYPE_GOLD || (itemList[i].itemType == ITEM_TYPE_ROCK && itemList[i].rockType == ROCK_TYPE_METAL))
                    {
                        if (insideWalls || tiles[itemList[i].x][itemList[i].y] == TILE_EARTH)
                        {
                            itemList[i].visibility = 255;
                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        public bool KnowItems()
        {
            bool result = false;

            if (itemList.Count > 0)
            {
                for (int i = 0; i < itemList.Count; i++)
                {
                    if (itemList[i].itemType != ITEM_TYPE_GOLD && itemList[i].itemType != ITEM_TYPE_ROCK)
                    {
                        itemList[i].visibility = 255;
                        result = true;
                    }
                }
            }

            return result;
        }

        public bool KnowFood()
        {
            bool result = false;

            if (itemList.Count > 0)
            {
                for (int i = 0; i < itemList.Count; i++)
                {
                    if (itemList[i].itemType == ITEM_TYPE_FOOD
                        && !itemList[i].rotten)
                    {
                        itemList[i].visibility = 255;
                        result = true;
                    }
                }
            }

            return result;
        }

        public bool KnowFurniture()
        {
            bool result = false;

            if (furnitureList.Count > 0)
            {
                result = true;
                for (int i = 0; i < furnitureList.Count; i++)
                {
                    furnitureList[i].visibility = 255;
                    vision[furnitureList[i].x][furnitureList[i].y] = 255;
                }
            }
            return result;
        }

        public bool KnowCreatures()
        {
            bool result = false;

            if (creatureList.Count > 0)
            {
                result = true;
                for (int i = 0; i < creatureList.Count; i++)
                {
                    creatureList[i].visibility = 255;
                }
            }
            return result;
        }

        //Assumption: There will always be doors.
        //So not returning a boolean
        public void KnowDoors()
        {
            for (int i = 1; i < DIMENSION - 1; i++)
            {
                for (int j = 1; j < DIMENSION - 1; j++)
                {
                    if (tiles[i][j] == TILE_DOOR_CLOSED || tiles[i][j] == TILE_DOOR_OPEN)
                    {
                        vision[i][j] = 255;
                    }
                }
            }
        }

        public void KnowExit()
        {
            for (int i = 0; i < DIMENSION; i++)
            {
                for (int j = 0; j < DIMENSION; j++)
                {
                    if (tiles[i][j] == TILE_STAIRS_DOWN)
                    {
                        vision[i][j] = 255;
                    }
                    else
                    {
                        if (furnitureList.Count > 0)
                        {
                            if (GetFurniture(i,j) != null)
                            {
                                if (GetFurniture(i, j).furnType == (int)SC.FurnTypes.TELEPORTER)
                                {
                                    vision[i][j] = 255;
                                    GetFurniture(i, j).visibility = 255;
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool TeleporterKnown()
        {
            if (furnitureList.Count == 0) return false;
            for (int i = 0; i < furnitureList.Count; i++)
            {
                if (furnitureList[i].furnType == (int)SC.FurnTypes.TELEPORTER)
                {
                    if (furnitureList[i].visibility > 0) return true;
                }
            }
            return false;
        }

        public Point KnownTeleporterLocation()
        {
            Point result = new Point(0, 0);
            if (furnitureList.Count == 0) return result;
            for (int i = 0; i < furnitureList.Count; i++)
            {
                if (furnitureList[i].furnType == (int)SC.FurnTypes.TELEPORTER)
                {
                    if (furnitureList[i].visibility > 0)
                    {
                        result.X = furnitureList[i].x;
                        result.Y = furnitureList[i].y;
                    }
                }
            }
            return result;
        }

        //Know traps - return how many were "discovered"
        public int KnowTraps()
        {
            int result = 0;
            if (trapList.Count > 0)
            {
                for (int i = 0; i < trapList.Count; i++)
                {
                    if (!trapList[i].discovered)
                    {
                        trapList[i].discovered = true;
                        vision[trapList[i].x][trapList[i].y] = 255;
                        result++;
                    }
                }
            }
            return result;
        }

        public bool CanSee(int pX, int pY, int dX, int dY)
        {
            return ((pX - dX) * (pX - dX) + (pY - dY) * (pY - dY) < playerPerception2) && IsLineOfSight(pX, pY, dX, dY);
        }

        public bool IsLineOfSight(int sX, int sY, int dX, int dY)
        {
            //Bresenham's equation starts here
            //It's slightly modified so that the line is
            //allways draw from the start point to the end point: beamcasting

            Point p0 = new Point(sX, sY);
            Point p1 = new Point(dX, dY);

            Boolean steep = Math.Abs(p1.Y - p0.Y) > Math.Abs(p1.X - p0.X);

            if (steep == true)
            {
                Point tmpPoint = new Point(p0.X, p0.Y);
                p0 = new Point(tmpPoint.Y, tmpPoint.X);

                tmpPoint = p1;
                p1 = new Point(tmpPoint.Y, tmpPoint.X);
            }

            int deltaX = Math.Abs(p1.X - p0.X);
            int deltaY = Math.Abs(p1.Y - p0.Y);
            int error = 0;
            int deltaError = deltaY;
            int yStep = 0;
            int xStep = 0;
            int y = p0.Y;
            int x = p0.X;

            if (p0.Y < p1.Y)
            {
                yStep = 1;
            }
            else
            {
                yStep = -1;
            }

            if (p0.X < p1.X)
            {
                xStep = 1;
            }
            else
            {
                xStep = -1;
            }

            int tmpX = 0;
            int tmpY = 0;

            while (x != p1.X)
            {

                x += xStep;
                error += deltaError;

                //if the error exceeds the X delta then
                //move one along on the Y axis
                if ((2 * error) > deltaX)
                {
                    y += yStep;
                    error -= deltaX;
                }

                //flip the coords if they're steep
                if (steep)
                {
                    tmpX = y;
                    tmpY = x;
                }
                else
                {
                    tmpX = x;
                    tmpY = y;
                }

                //check coords are legal
                if (tmpX >= 0 & tmpX < DIMENSION & tmpY >= 0 & tmpY < DIMENSION)
                {
                    //bail if the cell ain't empty
                    if (tiles[tmpX][tmpY] == TILE_DOOR_CLOSED 
                        || tiles[tmpX][tmpY] == TILE_WALL)
                        return false;
                }

            }
            return true;
        }

        //Line of sight for a target bigger that 1 tile
        public bool IsLineOfSight(int sX, int sY, int dX, int dY, int dW, int dH)
        {
            bool result = false;

            for (int i = dX; i < dX + dW; i++)
            {
                for (int j = dY; j < dY + dH; j++)
                {
                    result = result || IsLineOfSight(sX, sY, i, j);
                }
            }

            return result;
        }

        public bool IsEdgeOfSight(int sX, int sY, int dX, int dY)
        {
            //Bresenham's equation starts here
            //It's slightly modified so that the line is
            //allways draw from the start point to the end point: beamcasting

            Point p0 = new Point(sX, sY);
            Point p1 = new Point(dX, dY);

            Boolean steep = Math.Abs(p1.Y - p0.Y) > Math.Abs(p1.X - p0.X);

            if (steep == true)
            {
                Point tmpPoint = new Point(p0.X, p0.Y);
                p0 = new Point(tmpPoint.Y, tmpPoint.X);

                tmpPoint = p1;
                p1 = new Point(tmpPoint.Y, tmpPoint.X);
            }

            int deltaX = Math.Abs(p1.X - p0.X);
            int deltaY = Math.Abs(p1.Y - p0.Y);
            int error = 0;
            int deltaError = deltaY;
            int yStep = 0;
            int xStep = 0;
            int y = p0.Y;
            int x = p0.X;

            if (p0.Y < p1.Y)
            {
                yStep = 1;
            }
            else
            {
                yStep = -1;
            }

            if (p0.X < p1.X)
            {
                xStep = 1;
            }
            else
            {
                xStep = -1;
            }

            int tmpX = 0;
            int tmpY = 0;

            while (x != p1.X - xStep)
            {

                x += xStep;
                error += deltaError;

                //if the error exceeds the X delta then
                //move one along on the Y axis
                if ((2 * error) > deltaX)
                {
                    y += yStep;
                    error -= deltaX;
                }

                //flip the coords if they're steep
                if (steep)
                {
                    tmpX = y;
                    tmpY = x;
                }
                else
                {
                    tmpX = x;
                    tmpY = y;
                }

                //check coords are legal
                if (tmpX >= 0 & tmpX < DIMENSION & tmpY >= 0 & tmpY < DIMENSION)
                {
                    //bail if the cell ain't empty
                    if (tiles[tmpX][tmpY] == TILE_DOOR_CLOSED 
                        || tiles[tmpX][tmpY] == TILE_WALL)
                        return false;
                    if (sightBlockingFurniture.Count > 0)
                    {
                        for (int i = 0; i < sightBlockingFurniture.Count; i++)
                        {
                            if (sightBlockingFurniture[i].x == tmpX && sightBlockingFurniture[i].y == tmpY) return false;
                        }
                    }
                }

            }
            return true;
        }

    }
}