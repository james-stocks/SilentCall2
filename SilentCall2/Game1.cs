using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Amadues
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        const int DISPLAYWIDTH = 1280;
        const int DISPLAYHEIGHT = 720;
        const int RENDERSIZE = 1024;
        Rectangle TITLE_SAFE_RECT;
        const String TITLE = "SilentCall2";
        const String TITLE_STRING = "Silent Call 2";
        const String VERSION = "v2.0";
        const int MAX_INT = System.Int32.MaxValue;
        bool isTrialMode = false;

        //Color to wipe undrawn parts of the screen as
        Color clearColor = Color.Black;

        int NUM_MESSAGES_SHOWN = 5;

        float controllerVibrate = 0f;
        float controllerVibrateDropOff = 0.95f;

        //Boolean displayScreenCorners = false;
        const Boolean DISPLAY_TITLESAFE = false;
        const Boolean DISPLAY_FULLSCREEN = false;
        const float FULLSCREENZOOMY = 16f;
        const float FULLSCREENZOOMX = 19f;
        Boolean displayFullScreen = false;
        int GAMEFIELDWIDTH = 1024;
        int GAMEFIELDHEIGHT = 600;
        int TILE_SIZE = 64;
        int tilesInViewX = 0;
        int tilesInViewY = 0;
        int halfTilesInViewX = 0;
        int halfTilesInViewY = 0;

        //Cheating
        bool cheating = false;
        const int NUM_CHEATCODE_DIGITS = 5;
        char[] cheatCodeDigits;
        int correctCheatCodeDigits = 0;
        DateTime DATE_CHEAT_EXPIRES = new DateTime(2011, 12, 10);

        //3D stuff
        //DepthStencilBuffer playFieldDSB;
        RenderTarget2D playFieldRenderTarget;
        Matrix View;
        Matrix Projection;
        BasicEffect basicEffect;
        //VertexDeclaration quadVertexDeclaration;
        Quad playfieldQuad;
        Quad backgroundQuad;
        Matrix playFieldTranslateMatrix;
        Texture2D shadowTexture;

        //Textures
        Texture2D txTitleScreenLogo;
        Texture2D txGameOverLogo;
        Texture2D tx68KLogo;
        Texture2D txOrawartLogo;
        Texture2D txMusicLogo;
        Texture2D txMapPaper;
        Texture2D txMapDoorClosed;
        Texture2D txMapDoorOpen;
        Texture2D txMapGround;
        Texture2D txMapWall;
        Texture2D txMapStairs;
        Texture2D txMapPlayer;
        Texture2D txMapMerchant;
        Texture2D txMapTeleporter;
        Texture2D txPlayer;
        Texture2D txGUIArrowDown;
        Texture2D txGUIArrowUp;
        Texture2D txGUIScrollTab;
        Texture2D txIconBoss;
        Texture2D txIconFlame;
        Texture2D txIconIce;

        //Variables for the duration/cycling of GUI elements
        byte guiArrowTint = 0;
        byte guiStorageNotifyFade = 0;
        int featPanelDisplayDuration = 400;
        int featPanelDisplayTimer = 400;
        Feat featToDisplay;

        int logoScreenTimer = 0;
        int logoScreenEarliestSkip = 100;
        int logoScreenTimeMax = 600;

        //Color textures
        Texture2D txBlack;
        Texture2D txRed;
        Texture2D txWhite;
        Texture2D txLightBlue;
        Texture2D txBlue;
        Texture2D txGreen;
        Texture2D txOrange;
        Texture2D txDarkBlue;

        //Fonts
        SpriteFont sfSylfaen14;
        SpriteFont sfFranklinGothic;

        //List for particle effect clusters
        List<ParticleCluster> particleClusters;

        //Storage
        StorageDevice gamerStorageDevice;

        //Game States
        const int GAMESTATE_LOGOSCREEN = 0;
        const int GAMESTATE_ORAWART_LOGO = 8;
        const int GAMESTATE_MUSIC_LOGO = 9;
        const int GAMESTATE_STORAGE_WARNING = 10;
        const int GAMESTATE_TITLESCREEN = 1;
        const int GAMESTATE_PLAYING = 2;
        const int GAMESTATE_CHARACTERCREATION = 3;
        const int GAMESTATE_GAMEOVERSCREEN = 4;
        const int GAMESTATE_MAINMENU = 5;
        const int GAMESTATE_INTRO = 6;
        const int GAMESTATE_OUTRO = 7;
        int currentGameState = GAMESTATE_LOGOSCREEN;

        String titleScreenStatus = "Press Start";

        List<String> mainMenuStringList;
        bool showingCannotBuyWarning = false;
        bool showingExitConfirmation = false;
        bool showingResumableExistsWarning = false;
        bool resumableGameExists = false;
        bool settingsLoadComplete = false;
        bool notCurrentlySaving = true;
        bool notCurrentlyLoading = true;
        volatile bool resumableGameLoading = false;
        volatile bool resumableGameSaving = false;
        int currentMainMenuSelection = 0;

        int creationScreenBackgroundOffset = 0;
        const int BACKGROUND_TILE_SIZE = 64;

        //dialogs
        String magicName = "Magic";
        bool tutorialMessageOpen = false;
        int currentTutorialMessageOpen = -1;
        bool dialogOpen = false;

        bool totalStatsOpen = false;
        bool helpOpen = false;
        bool creditsOpen = false;
        bool featSummaryOpen = false;
        int currentStatScreenOffset = 0;
        const int STATS_PER_PAGE = 20;

        int currentHelpPage = 1;
        const int NUM_HELP_PAGES = 4;

        bool contextDialogOpen = false;
        List<String> contextMenuStringList;
        int currentContextMenuSelection = 0;

        bool contextDialogSubMenuOpen = false;
        List<String> contextSubMenuStringList;
        string subMenuAction = "";
        int currentContextSubMenuSelection = 0;

        bool startMenuOpen = false;
        List<String> startMenuStringList;
        int currentStartMenuSelection = 0;

        bool optionsMenuOpen = false;
        int currentOptionMenuSelection = 0;
        int MAX_OPTION_MENU_ITEMS = 6;

        bool inventoryOpen = false;
        int currentInventorySelection = 0;
        int inventoryOffset = 0;
        int inventoryItemsPerPage = 8;
        List<InventoryTab> inventoryTabs;
        int currentInventoryTab = 0;
        const int MAX_INVENTORY_TAB = 5;

        bool characterSheetOpen = false;

        bool spellListOpen = false;
        int currentSpellListSelection = 0;

        bool runStatsOpen = false;
        int currentStatOffset = 0;
        int statsOnScreen = 20;

        bool shopOpen = false;
        bool shopConfirmOpen = false;
        int currentShopSelection = 0;
        String lastShopMessage = "";

        bool questListOpen = false;
        bool questForfeitOpen = false;
        bool questFailedOpen = false;
        bool questTurnInOpen = false;
        int currentQuestListSelection = 0;

        List<Feat> featList;

        bool gameQuitConfirmOpen = false;
        bool gameSuspendConfirm = false;

        bool mapOpen = false;
        byte[,] mapTiles;
        bool mapOpenBackNeedsReleased = false; //Need a bool to stop map re-opening

        int FRAMES_PER_PARTICLE_UPDATE = 5;
        int currentParticleUpdateFrameDelay = 0;

        List<LogMessage> messages;
        int messageDisplayOffset = 0;

        List<String> randomTips;
        int selectedRandomTip = 0;

        List<String> introText;
        List<String> outroText;
        int introTextOffset = 0;
        int outroTextOffset = 0;
        int FRAMES_PER_TEXT_SHIFT = 5;
        int currentFrameTextShiftDelay = 0;

        int currentCharacterCreationOption = 0;
        int TOTAL_CHAR_CREATE_OPTIONS = 4;
        const int CHAR_CREATE_OPTION_NAME = 0;
        const int CHAR_CREATE_OPTION_PROFESSION = 1;
        const int CHAR_CREATE_OPTION_RACE = 2;
        const int CHAR_CREATE_OPTION_TUTORIALMESSAGES = 3;
        int currentLevelSelectDigit = 0;
        //int MAX_LEVEL_SELECT_DIGIT = 3; //How many powers of 10 there may be in the level select
        bool changingStartingLevel = false;

        List<TutorialMessage> tutorialMessages;
        List<String> currentTutorialMessage;
        const int TUTORIAL_WINDOW_WIDTH = 600;
        List<int> tutorialMessagesNeverToSeeAgain;
        bool neverSeeTutorialMessages = false;

        //Level
        Level villageLevel;
        Level dungeonLevel;
        bool inVillage = false;
        Level currentLevel;
        int currentLevelNumber = 1;
        const int MAX_LEVELS = 9001;
        bool isLevelComplete = false;
        int startingLevel = 1;
        int MAX_STARTING_QUEST_LEVEL = 1;

        const double LARGE_EXP_CHUNK = 9999;    //Biggest single chunk of exp allowed
        //to prevent the recursive Player.AddExperience() from
        //recursing too much
        int lastLevel = 1;  //Track when player levels up
        float lastHunger = 0f;

        //Item generator
        ItemGenerator itemGenerator;

        const double CHANCE_OF_CORPSE = 0.2;
        const double BAD_FOOD_POISON_CHANCE = 0.3;
        const double CHANCE_TO_TRIGGER_TRAP = 0.6;
        const double CHANCE_TO_DODGE_TRAP = 0.3;
        const double CHANCE_TO_DISCOVER_TRAP = 0.2;
        const double CHANCE_TO_DISARM_TRAP = 0.1;
        const double CHANCE_ENEMY_WILL_POISON = 0.1;

        //List of Items held on last killed player 
        List<Item> lootItemsFromLastPlayer;

        //Bestiary
        Bestiary bestiary;

        //Tiles
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
        Texture2D txTileEarth;
        Texture2D txTileWall;
        Texture2D txTileDoorOpen;
        Texture2D txTileDoorClosed;
        Texture2D txTileStairsUp;
        Texture2D txTileStairsDown;

        //Quest Generator
        QuestGenerator questGenerator;
        Quest currentQuest;
        List<Quest> currentQuestList;
        const int QUEST_LIST_SIZE = 7;

        //List of bosses
        List<Boss> bossList;

        const int NUM_TILE_SETS = 6;
        Texture2D txTileSet1Wall;
        Texture2D txTileSet1DoorOpen;
        Texture2D txTileSet1DoorClosed;
        Texture2D txTileSet2Wall;
        Texture2D txTileSet2DoorOpen;
        Texture2D txTileSet2DoorClosed;
        Texture2D txTileSet3Wall;
        Texture2D txTileSet3DoorOpen;
        Texture2D txTileSet3DoorClosed;
        Texture2D txTileSet4Wall;
        Texture2D txTileSet4DoorOpen;
        Texture2D txTileSet4DoorClosed;
        Texture2D txTileSet5Wall;
        Texture2D txTileSet5DoorOpen;
        Texture2D txTileSet5DoorClosed;

        //Village textures
        Texture2D txVillageDoorClosed;
        Texture2D txVillageDoorOpen;
        Texture2D txVillageGrass;
        Texture2D txVillageHedge;
        Texture2D txVillagePath;
        Texture2D txVillageWall;
        Texture2D txVillageWater;
        Texture2D txVillageTree;
        Texture2D txVillageFloor;

        //Item textures
        Texture2D txItemSword;
        Texture2D txItemDagger;
        Texture2D txItemMace;
        Texture2D txItemBow;
        Texture2D txItemSpear;
        Texture2D txItemShield;
        Texture2D txItemArmour;
        Texture2D txItemCoins;
        Texture2D txItemCloak;
        Texture2D txItemPotion;
        Texture2D txItemSuperPotion;
        Texture2D txItemScroll;
        Texture2D txItemFoodMeat;
        Texture2D txItemFoodVeg;
        Texture2D txItemFoodBread;
        Texture2D txItemFoodCanned;
        Texture2D txItemFoodCorpse;
        Texture2D txRockRock;
        Texture2D txRockMetal;
        Texture2D txRockGem;
        Texture2D txRockGrindStone;
        Texture2D txItemQuest;

        Texture2D txFurnAltar;
        Texture2D txFurnBarrel;
        Texture2D txFurnCandelabra;
        Texture2D txFurnCrate;
        Texture2D txFurnGravestone;
        Texture2D txFurnSignpost;
        Texture2D txFurnStatue;
        Texture2D txFurnTable;
        Texture2D txFurnTeleporter;
        Texture2D txFurnChairLeft;
        Texture2D txFurnChairRight;

        //Creature textures
        Texture2D txPlayerElf;
        Texture2D txPlayerElf_Back;
        Texture2D txPlayerElf_Left;
        Texture2D txPlayerElf_Right;
        Texture2D txPlayerHuman;
        Texture2D txPlayerHuman_Back;
        Texture2D txPlayerHuman_Left;
        Texture2D txPlayerHuman_Right;
        Texture2D txPlayerOrc;
        Texture2D txPlayerOrc_Back;
        Texture2D txPlayerOrc_Left;
        Texture2D txPlayerOrc_Right;
        Texture2D txCreatureBat;
        Texture2D txCreatureRodent;
        Texture2D txCreatureLizard;
        Texture2D txCreatureBug;
        Texture2D txCreatureCanine;
        Texture2D txCreatureMerchant;
        Texture2D txCreatureDragon;
        Texture2D txCreatureGoblin;
        Texture2D txCreatureSnake;
        Texture2D txCreatureSuccubus;
        Texture2D txCreatureSpider;
        Texture2D txCreatureDemon;
        Texture2D txCreatureSkeleton;
        Texture2D txCreatureHuman;
        Texture2D txCreatureGhost;
        Texture2D txCreatureSlime;
        Texture2D txCreatureTroll;
        Texture2D txCreatureGoldrick;
        Texture2D txCreatureZombie;
        Texture2D txCreatureCook;
        Texture2D txCreatureVillager;
        Texture2D txCreatureQuestMaster;
        Texture2D txLargeCreatureQuestMaster;
        Texture2D txLargeCreatureMerchant;
        Texture2D txLargePlayerElf;
        Texture2D txLargePlayerHuman;
        Texture2D txLargePlayerOrc;

        //Trap textures
        Texture2D txTrapArrow;
        Texture2D txTrapPoison;
        Texture2D txTrapTeleport;
        Texture2D txTrapDescent;

        //Particle Effects
        Texture2D txEffectDarkness;
        Texture2D txEffectFire;
        Texture2D txEffectIce;
        Texture2D txEffectImpact;
        Texture2D txEffectGas;
        Texture2D txEffectHeal;
        Texture2D txEffectLevelUp;

        Texture2D txAlliedIndicator;
        Texture2D txHostileIndicator;

        //Button textures
        Texture2D btnTxButtonB;
        Texture2D btnTxButtonX;
        Texture2D btnTxButtonA;
        Texture2D btnTxButtonY;
        Texture2D btnTxButtonDPad;
        Texture2D btnTxLeftShoulder;
        Texture2D btxTxRightShoulder;
        Texture2D btnTxStart;
        Texture2D btnTxGuide;
        Texture2D btnTxLeftTrigger;
        Texture2D btnTxRightTrigger;
        Texture2D btnTxBack;
        Texture2D btnTxLeftStick;
        Texture2D btnTxRightStick;

        //Sound effects
        SoundEffect sfxDanger;
        SoundEffect sfxDeath;
        SoundEffect sfxEnemyAttack;
        SoundEffect sfxFail;
        SoundEffect sfxFanfare;
        SoundEffect sfxMagicFire;
        SoundEffect sfxMagicHeal;
        SoundEffect sfxMagicIce;
        SoundEffect sfxMagicMissile;
        SoundEffect sfxMenuCancel;
        SoundEffect sfxMenuConfirm;
        SoundEffect sfxMenuSelect;
        SoundEffect sfxRecruit;
        SoundEffect sfxSelectBad;
        SoundEffect sfxStep;
        SoundEffect sfxStepFiltered;
        SoundEffect sfxTeleport;
        SoundEffect sfxBlade;
        SoundEffect sfxClub;
        SoundEffect sfxArrow;
        SoundEffect sfxCoins;
        SoundEffect sfxPaper;
        SoundEffect sfxLevelUp;

        Song songTitle;
        Song songLogo;
        Song songOutro;
        BackgroundMusicTrack villageMusic;
        List<BackgroundMusicTrack> dungeonMusicList;
        BackgroundMusicTrack dungeonMusic;
        SC.BGMPlayOption bgmPlayOption = SC.BGMPlayOption.ON;
        SC.BGMShuffleOption bgmShuffleOption = SC.BGMShuffleOption.ONCE_PER_QUEST;

        //bools to control sound effect repetion
        bool isSfxDeathPlayedThisGame = false;

        //player variables
        Player player1;
        List<Spell> spellListAtLastTurn;
        PlayerIndex playerIndex;
        Controller controller;
        GamePadState lastButtons;
        int movementHoldDirection = SC.DIRECTION_NONE;
        int movementHoldDuration = 0;
        bool playerIndexSet = false;
        bool playerLooking = false;
        int playerLookAtX = 0;
        int playerLookAtY = 0;
        double moveCount = 0;
        const double MAX_MOVES = 99999;
        bool hasMadeMove = false;
        String death;
        Creature fatalEnemy;
        bool playerQuit = false;
        //Spell casting
        bool isCastingSpell = false;
        int spellTypeBeingCast = (int)SC.SpellCasting.DIRECTION;
        int spellTargetX = 0;
        int spellTargetY = 0;
        int spellIndexToCast = 0;
        //int spellRadius = 4;
        int spellRadiusSquared = 16;

        List<Creature> descendingCreatures; //Enemies that will be carried forward when moving floor

        //Persistant player variables
        double runBonusXP = 0;
        double totalBonusXP = 0;
        double runBonusGold = 0;
        double totalBonusGold = 0;
        double goldBonusCorrection = 0; //Need to track how much profit player makes from original loadout
        Stats runStats;
        Stats totalStats;
        Stopwatch runTime;
        long totalSecondsPlayed; //Total time played in seconds
        bool playerIdle = false;
        int currentFramesIdle = 0;
        int framesUntilIdle = 3600;

        //Merchant stuff
        List<Item> dungeonMerchantItems;
        List<Item> villageMerchantItems;
        int villageMerchantLevel = 1;
        const int MAX_VILLAGE_MERCHANT_LEVEL = 9999;
        const int GOLD_PER_VILLAGE_MERCHANT_LEVEL = 50;
        int levelsPerMerchant = 6;
        int itemsPerMerchant = 7;
        double merchantMarkUp = 0.7;
        double explorerPriceBonus = 1.15;
        List<String> villagerResponses;

        //village construction costs
        const int VILLAGE_BUILD_WALL_COST = 100;
        const int VILLAGE_DEMOLISH_WALL_COST = 10;
        const int VILLAGE_BUILD_DOOR_COST = 200;
        const int VILLAGE_DEMOLISH_DOOR_COST = 10;
        byte[][] loadedVillageBuildArea;
        List<Item> loadedVillageItems;

        //Outro stuff
        byte[][] outroTiles;
        int outroTileSet;
        int outroWidth = DISPLAYWIDTH / 16;
        int outroHeight = DISPLAYHEIGHT / 16;

        //delays
        int DELAY_MOVE_LOOKING = 12;
        int delay_current_move_looking = 0;
        int DELAY_SPELL_TARGETING = 6;
        int delay_current_spell_targeting = 0;

        int CREATURE_TINT_PERIOD = 3;
        int currentCreatureTint = 0;

        int DELAY_CHECK_SIGNEDIN = 50;
        int delay_current_signin_check = 0;

        int DELAY_MAINMENU_INPUT = 180;
        int delay_current_mainmenu_input = 0;

        //random
        Random random;

        public Game1()
        {
            Components.Add(new GamerServicesComponent(this));
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = DISPLAYWIDTH;
            graphics.PreferredBackBufferHeight = DISPLAYHEIGHT;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            TITLE_SAFE_RECT = graphics.GraphicsDevice.Viewport.TitleSafeArea;

            if (isTrialMode)
            {
                totalBonusGold = 100;
                totalBonusXP = 500;
                MAX_STARTING_QUEST_LEVEL = 100;
            }


            random = new Random();

            Color[] fillColor = new Color[16 * 16];
            for (int i = 0; i < 16 * 16; i++)
            {
                fillColor[i] = new Color(0, 0, 0, 255);
            }
            txBlack = new Texture2D(GraphicsDevice, 16, 16);
            txBlack.SetData<Color>(fillColor);

            for (int i = 0; i < 16 * 16; i++)
            {
                fillColor[i] = new Color(255, 80, 80, 255);
            }
            txRed = new Texture2D(GraphicsDevice, 16, 16);
            txRed.SetData<Color>(fillColor);

            for (int i = 0; i < 16 * 16; i++)
            {
                fillColor[i] = new Color(255, 255, 255, 255);
            }
            txWhite = new Texture2D(GraphicsDevice, 16, 16);
            txWhite.SetData<Color>(fillColor);

            for (int i = 0; i < 16 * 16; i++)
            {
                fillColor[i] = new Color(100, 100, 255, 255);
            }
            txLightBlue = new Texture2D(GraphicsDevice, 16, 16);
            txLightBlue.SetData<Color>(fillColor);

            for (int i = 0; i < 16 * 16; i++)
            {
                fillColor[i] = new Color(0, 0, 160, 255);
            }
            txBlue = new Texture2D(GraphicsDevice, 16, 16);
            txBlue.SetData<Color>(fillColor);

            for (int i = 0; i < 16 * 16; i++)
            {
                fillColor[i] = new Color(0, 255, 0, 255);
            }
            txGreen = new Texture2D(GraphicsDevice, 16, 16);
            txGreen.SetData<Color>(fillColor);

            for (int i = 0; i < 16 * 16; i++)
            {
                fillColor[i] = new Color(255, 100, 0, 255);
            }
            txOrange = new Texture2D(GraphicsDevice, 16, 16);
            txOrange.SetData<Color>(fillColor);

            for (int i = 0; i < 16 * 16; i++)
            {
                fillColor[i] = new Color(0, 0, 140, 255);
            }
            txDarkBlue = new Texture2D(GraphicsDevice, 16, 16);
            txDarkBlue.SetData<Color>(fillColor);

            playFieldRenderTarget = new RenderTarget2D(GraphicsDevice, RENDERSIZE, RENDERSIZE);

            View = Matrix.CreateLookAt(new Vector3(0f, 0f, 200f), new Vector3(0f, 0f, 0f), Vector3.Up);

            Projection = Matrix.CreateOrthographicOffCenter((float)-DISPLAYWIDTH / 2, (float)DISPLAYWIDTH / 2, (float)-DISPLAYHEIGHT / 2, (float)DISPLAYHEIGHT / 2, 200f, 0f);

            if (displayFullScreen)
            {
                Projection = Matrix.CreateOrthographicOffCenter((float)-DISPLAYWIDTH / 2 + (float)DISPLAYWIDTH / 200f * FULLSCREENZOOMX,
                                                                        (float)DISPLAYWIDTH / 2 - (float)DISPLAYWIDTH / 200f * FULLSCREENZOOMX,
                                                                        (float)-DISPLAYHEIGHT / 2 + (float)DISPLAYHEIGHT / 200f * FULLSCREENZOOMY,
                                                                        (float)DISPLAYHEIGHT / 2 - (float)DISPLAYHEIGHT / 200f * FULLSCREENZOOMY,
                                                                        1f, 200f
                                                                        );
            }
            else
            {
                Projection = Matrix.CreateOrthographicOffCenter((float)-DISPLAYWIDTH / 2 + (float)DISPLAYWIDTH / 200f,
                                                                        (float)DISPLAYWIDTH / 2 - (float)DISPLAYWIDTH / 200f,
                                                                        (float)-DISPLAYHEIGHT / 2 + (float)DISPLAYHEIGHT / 200f,
                                                                        (float)DISPLAYHEIGHT / 2 - (float)DISPLAYHEIGHT / 200f,
                                                                        1f, 200f
                                                                        );
            }

            playFieldTranslateMatrix = Matrix.CreateTranslation(new Vector3(0f, -210f, 0f));

            basicEffect = new BasicEffect(GraphicsDevice);

            basicEffect.World = Matrix.Identity * playFieldTranslateMatrix;
            basicEffect.View = View;
            basicEffect.Projection = Projection;

            basicEffect.TextureEnabled = true;
            playfieldQuad = new Quad(Vector3.Zero, Vector3.Backward, Vector3.Up, RENDERSIZE, RENDERSIZE);
            backgroundQuad = new Quad(Vector3.Zero, Vector3.Backward, Vector3.Up, DISPLAYWIDTH, DISPLAYWIDTH);

            shadowTexture = new Texture2D(GraphicsDevice, GAMEFIELDWIDTH, GAMEFIELDHEIGHT);

            tilesInViewX = GAMEFIELDWIDTH / TILE_SIZE;
            tilesInViewY = GAMEFIELDHEIGHT / TILE_SIZE;
            halfTilesInViewX = tilesInViewX / 2;
            halfTilesInViewY = tilesInViewY / 2;

            particleClusters = new List<ParticleCluster>();

            player1 = new Player();
            player1.Initialize();
            player1.texture = txPlayer;
            death = "Died ";

            controller = new Controller();

            totalStats = new Stats();

            runTime = new Stopwatch();
            totalSecondsPlayed = 0;

            itemGenerator = new ItemGenerator();
            bestiary = new Bestiary();
            questGenerator = new QuestGenerator(bestiary, itemGenerator);
            currentQuestList = new List<Quest>();

            lootItemsFromLastPlayer = new List<Item>();

            inventoryTabs = new List<InventoryTab>();
            inventoryTabs.Add(new InventoryTab("ALL", SC.InventoryFilterType.ALL, player1.carriedItems));
            inventoryTabs.Add(new InventoryTab("Weapons", SC.InventoryFilterType.WEAPON, player1.carriedItems));
            inventoryTabs.Add(new InventoryTab("Armour", SC.InventoryFilterType.ARMOUR, player1.carriedItems));
            inventoryTabs.Add(new InventoryTab("Food", SC.InventoryFilterType.FOOD, player1.carriedItems));
            inventoryTabs.Add(new InventoryTab("Potions", SC.InventoryFilterType.POTION, player1.carriedItems));
            inventoryTabs.Add(new InventoryTab("Scrolls", SC.InventoryFilterType.SCROLL, player1.carriedItems));
            inventoryTabs.Add(new InventoryTab("Other", SC.InventoryFilterType.OTHER, player1.carriedItems));

            InitBossList();

            InitFeats();

            InitMainMenu();

            startMenuStringList = new List<String>();
            startMenuStringList.Add("Inventory");
            startMenuStringList.Add("Character");
            startMenuStringList.Add("Spells");
            startMenuStringList.Add("Stats");
            startMenuStringList.Add("Map");
            startMenuStringList.Add("Options");
            startMenuStringList.Add("Help");
            startMenuStringList.Add("Suspend");
            startMenuStringList.Add("End Game");

            randomTips = new List<string>();
            randomTips.Add("Each game you play slightly improves your starting character in \nall subsequent games.");
            randomTips.Add("Looking around doesn't use up turns or increase hunger.\nYou can look at distant objects by holding Left Bumper\nthen moving the Right Stick.");
            randomTips.Add("Moving diagonally only uses up one turn.\nMoving across then up or down uses two turns.");
            randomTips.Add("Casting a spell earns a small amount of EXP - \nbut only if the spell did something useful!");
            randomTips.Add("A Scroll of Wild Descent will teleport you deeper into the dungeon.\nUse it if you feel that the current floor isn't rewarding enough.");
            randomTips.Add("Each floor is slightly tougher than the last. Think twice before\nleaving a floor you haven't fully explored.");
            randomTips.Add("Fighters naturally regain health faster. Mages regain magic faster.");
            randomTips.Add("Move quickly by holding the Left Bumper whilst also holding Left Thumbstick\nin a direction. Beware that monsters will also move quickly!");
            randomTips.Add("Explorers always know where the floor exis it.\nTry to find the safest route to it!");
            randomTips.Add("Explorers have neither the brawn of Fighters nor the spellpower of Mages.\nTreat every monster with caution.");
            randomTips.Add("Use the \"Switch Places\" command to swap tiles with a friendly creature");
            randomTips.Add("Many enemies are susceptible to flame or frost. If you know \ntheir weakness, swap weapon (It doesn't use up a turn).");
            randomTips.Add("Cold blooded enemies are adversely affected by both frost and flame.");
            randomTips.Add("Cloaks give you a little extra physical defence,\nbut may also protect you from the elements");
            randomTips.Add("Frozen enemies are denoted by shards of ice each turn.\nThe shards don't appear a couple of turns before the creature thaws!");
            randomTips.Add("Fighters can knock enemies away with \"Kick\".\nUse \"Kick\" to put distance between you and a foe");
            randomTips.Add("Fighters can handle dangerous enemies using \"Kick\" and \"Freeze\".");
            randomTips.Add("Explorers Can Dig Through Most Walls To Create Shortcuts And Reveal Treasure.");
            randomTips.Add("When Approaching An Enemy, Press B To Wait A Turn,\nAnd Allow The Enemy To Come To You!");
            randomTips.Add("You can sell unwanted items to merchants. Mark the \nitems you wish to sell by pressing Y in the inventory");
            randomTips.Add("Creatures Described As \"Dangerous\" Will Kill You In 2-3 Hits.\nA Certain Sound Will Play When Such An Enemy Is First Spotted.");
            randomTips.Add("The Number In An Elemental Weapon's Name Reveals The \nAmount Of Elemental Damage It Will Do.");
            randomTips.Add("If You're Becoming Desperately Hungry; Try Eating A Corpse\nOr Find A Merchant To Buy Food From");
            randomTips.Add("If you're about to die, check your inventory.\nYou may have something that will improve your situation.");
            randomTips.Add("Later in the game you may venture upon chefs who can be\nrecruited to prepare and preserve food.");
            randomTips.Add("Certain Enemies Attack With Elemental Power. Watch\nFor Ice Or Flame When The Enemy Strikes You!");
            randomTips.Add("When you start a new game, the quest master will always\noffer you one quest that's slightly easier than the others.");
            randomTips.Add("If you have a surplus of useful items, store some in your house\nso that they're available in future games.");
            randomTips.Add("A Karma Scroll can greatly change your odds.\nIt's behaviour is not as random as it might first seem...");
            randomTips.Add("When you turn in a quest, any allied creatures must be left\nbehind in the dungeon.");
            randomTips.Add("When rescuing a character from a dungeon, consider clearing the\narea of danger before having them follow you.");
            randomTips.Add("Only Explorers can wield bows. Only Fighters can wield maces and spears.");
            randomTips.Add("Spears can attack enemies two spaces away. Use Left Trigger to\nattack quickly in a direction.");
            randomTips.Add("Pull the Right Trigger to open up a list of possible commands.\nPull the left trigger to act automatically for a turn.");
            randomTips.Add("Quests with deeper dungeons have proportionally better rewards\nThey also bring you further towards the next boss.");
            randomTips.Add("Each quest you turn in brings you closer to the next boss.");
            randomTips.Add("When it's time to face a boss, you can continue questing for\nexperience and items. Quests will not increase in difficulty until\nthe boss is defeated.");
            randomTips.Add("If you find an altar, you can pray for help.\nTreat altars with respect.");
            randomTips.Add("Surplus gold can be invested in the village shopkeeper.\nGold carried on you when you die isn't entirely wasted though...");
            randomTips.Add("On the first dungeon floor of each quest, you have a chance\nto find good items from previous games.");
            randomTips.Add("If you find a weapon you can't equip, consider storing it\nfor a future game instead of selling it.");
            randomTips.Add("Your allies can engage an enemies attention, but the enemy will\n still strike you if you step close. Use ranged attacks to\nattack them while they focus on your ally!");

            villagerResponses = new List<string>();
            villagerResponses.Add("Please Help Us! Chat To The Quest Master!");
            villagerResponses.Add("You Can Build A House To The South-West.");
            villagerResponses.Add("Your House Will Survive Even If You Die. Neat, Huh?");
            villagerResponses.Add("Investing In The Shopkeeper Permanently Improves His Stock.");
            villagerResponses.Add("I Fear For Our Land...");
            villagerResponses.Add("Our Crops Have Failed Again.");
            villagerResponses.Add("Hey, Seen Any Elves?");
            villagerResponses.Add("Elves Get Hungry Easy");
            villagerResponses.Add("Orcs Are Harder To Starve!");
            villagerResponses.Add("Don't Descend Too Hastily, The Next Floor Will Be More Dangerous!");
            villagerResponses.Add("If I Were An Explorer, I'd Avoid Fights And Collect Treasure.");
            villagerResponses.Add("Gold Rick Is An Evil Man. He Must Be Stopped!");

            introText = new List<string>();
            outroText = new List<string>();
            outroText.Add("Well Done.");
            outroText.Add("");
            outroText.Add("Gold Rick Is Dead!");
            outroText.Add("");
            outroText.Add("You Have Defeated The Empire Of Jem And Saved Your Lands");
            outroText.Add("From Darkness.");
            outroText.Add("");
            outroText.Add("Thank You For Playing!");
            outroText.Add("Visit www.68ST0X20.com To See What's Next!");
            outroText.Add("");
            outroText.Add("");
            outroText.Add("");
            outroText.Add("Current Music Track - \"Library Acid\" By Scheme Boy");

            cheatCodeDigits = new char[NUM_CHEATCODE_DIGITS];
            cheatCodeDigits[0] = 'X';
            cheatCodeDigits[1] = 'Y';
            cheatCodeDigits[2] = 'X';
            cheatCodeDigits[3] = 'X';
            cheatCodeDigits[4] = 'Y';

            tutorialMessages = new List<TutorialMessage>();
            tutorialMessages.Add(new TutorialMessage(0, "Welcome To The Land Of Jem", "Welcome to the land of Jem! This once-peaceful land is now terrorised by the "
                                        + "Dark Master, Gold Rick. This story is about the multitude of heroes your home village sent forth to battle Gold Rick. "
                                        + "You now play just one of those heroes. To begin, move North-East to the Adventurers' Guild; open the door; and Chat to "
                                        + "the village's Guild Master."));
            tutorialMessages.Add(new TutorialMessage(1, "Hey, Listen!", "This is a Tutorial Message. Press B to close it. Press X to close it and never see it again "
                                        + "in future games. You can choose to hide ALL Tutorial Messages when you start the game, or from the Options page."));
            tutorialMessages.Add(new TutorialMessage(2, "Interacting", "You're standing next to the Adventurers' Guild. To Open the door, Hold the Right Stick "
                                        + "towards the Door so that it's highlighted, then pull the Right Trigger. You can either \"Look\" or \"Open\". Choose "
                                        + "to Open the door."));
            tutorialMessages.Add(new TutorialMessage(3, "Interacting (2)", "Welcome to the Adventurers' Guild. The Quest Master is standing just to the North. "
                                        + "Chat to him by moving beside him, highlighting him with the Right Stick, and pulling the Right Trigger. Then choose "
                                        + "the \"Chat\" command."));
            tutorialMessages.Add(new TutorialMessage(4, "Quest Accepted", "You just accepted your first Quest. To begin, walk back out to the village then follow "
                                        + "the path North to the Dungeon Teleporter. Interact with the teleporter to warp to the dungeon for the Quest. Good luck!"));
            tutorialMessages.Add(new TutorialMessage(5, "Merchants", "You can buy and sell items with merchants. To sell items, open your inventory and mark items as "
                                        + "\"For Sale\" using the Y button. Then interact with the merchant and choose \"Trade\". The merchant will buy your unwanted "
                                        + "items and show you his list of available items."));
            tutorialMessages.Add(new TutorialMessage(6, "Quest Completed!", "Congratulations, you have completed your current Quest. Return to the Quest Master in "
                                        + "the village to Turn In the Quest and receive your reward."));
            tutorialMessages.Add(new TutorialMessage(7, "Next Boss", "The progress bar underneath the Quest List shows how close you are to confronting the next boss. "
                                        + "Complete quests to increase your progress."));
            tutorialMessages.Add(new TutorialMessage(8, "Descending In Dungeons", "When you descend to a new floor, enemies will be slightly tougher; but you will "
                                        + "gain more XP and find better loot. If you are struggling to survive, find all the loot on the current floor before "
                                        + "descending. "));
            tutorialMessages.Add(new TutorialMessage(9, "It's A Trap!", "Uh oh. You just found or triggered a trap! You can try to find an alternative path; or attempt to disarm "
                                        + "it; or risk walking over it (if you're feeling lucky or are prepared to suffer the consequences!)."));
            tutorialMessages.Add(new TutorialMessage(10, "Incoming Enemy", "You've spotted an enemy! You can attack by moving towards the enemy; or if you're a "
                                        + "Mage, press Right Bumper to pick a spell to cast. Enemies might be described as \"Puny\" or \"Dangerous\" - pay "
                                        + "attention to this! Tip: press B to wait a turn and let the enemy come to you!"));
            tutorialMessages.Add(new TutorialMessage(11, "Poisoned!", "You have just been poisoned! You will take damage every turn until the poison wears off. "
                                        + "You should rest and heal until it has worn off, or drink an Antidote. Or just ignore it, if you think you're hard enough."));
            tutorialMessages.Add(new TutorialMessage(12, "Ding! Level Up!", "You just gained a new Experience Level. Depending on your profession, leveling up "
                                        + "means more Health, more " + magicName + ", more Attack, more Defence, and possibly new spells!"));
            tutorialMessages.Add(new TutorialMessage(13, "It's Dangerous To Go Alone!", "You have just spotted a potential ally. Avoid attacking them. If you have "
                                        + "enough money you can recruit them."));
            tutorialMessages.Add(new TutorialMessage(14, "You Need Food, Badly", "You are getting hungry. Your Hunger meter is at the top-left corner. If it empties, "
                                        + "you will starve. Press Y to open your inventory, and see if you have an item of food to eat. If not, you better find "
                                        + "something soon..."));
            tutorialMessages.Add(new TutorialMessage(15, "Fast Input", "Hold the Left Bumper to do everything quicker! Holding it will let you move quickly until "
                                        + "you hit a solid surface or see something dangerous. Holding Left Bumper while holding B will let you rest and heal "
                                        + "quickly."));
            tutorialMessages.Add(new TutorialMessage(16, "Saving Items", "You can drop items in the area to the South West of the village. You own this area, so "
                                        + "the items you drop there will remain for future heroes to collect! Note: The \"free\" items you start the game "
                                        + "with will NOT be saved."));
            tutorialMessages.Add(new TutorialMessage(17, "Heal!", "You have learned the spell 'Heal'. Press the Right Bumper to access your spell list. "
                                        + "'Heal' will restore a small amount of your health. Since " + magicName + " restores over time, be sure to use it if "
                                        + "your " + magicName + " is filled but your health is low."));
            tutorialMessages.Add(new TutorialMessage(18, "Freeze!", "You have learned the spell 'Freeze'. Press the Right Bumper to access your spell list. "
                                        + "'Freeze' deals ice damage to an enemy and has a chance to temporarily freeze them where they stand! A frozen enemy is "
                                        + "totally vulnerable to further attacks, but will only remain frozen for a short time!"));
            tutorialMessages.Add(new TutorialMessage(19, "I Want To Cast... MAGIC MISSILE!", "You have learned 'Magic Missile'! This spell deals heavy damage "
                                        + "and can be directed at any spot you can see around you. It costs a lot of " + magicName + " to cast, so is not efficient "
                                        + "for killing weaker enemies."));
            tutorialMessages.Add(new TutorialMessage(20, "Ranged Weapons", "You have picked up a weapon that can attack at range. You should take advantage of "
                                        + "being able to hit enemies at a distance. Highlight a distant enemy by moving RS while holding down the Left Bumper. "
                                        + "Alternatively, pull Left Trigger while pointing in a direction with RS to automatically attack in that direction."));

            tutorialMessagesNeverToSeeAgain = new List<int>();

            base.Initialize();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sfSylfaen14 = Content.Load<SpriteFont>("Sylfaen14");
            sfFranklinGothic = Content.Load<SpriteFont>("FranklinGothic");

            txTitleScreenLogo = Content.Load<Texture2D>("title_logo5");
            txPlayer = Content.Load<Texture2D>("knight");
            txGUIArrowDown = Content.Load<Texture2D>("white_arrow_down");
            txGUIArrowUp = Content.Load<Texture2D>("white_arrow_up");
            txGUIScrollTab = Content.Load<Texture2D>("scroll_tab");
            txIconBoss = Content.Load<Texture2D>("icon_boss");
            txIconFlame = Content.Load<Texture2D>("icon_flame");
            txIconIce = Content.Load<Texture2D>("icon_ice");
            txGameOverLogo = Content.Load<Texture2D>("GameOver");
            tx68KLogo = Content.Load<Texture2D>("68kLogo");
            txOrawartLogo = Content.Load<Texture2D>("OrawartLogo");
            txMusicLogo = Content.Load<Texture2D>("MusicLogo");
            txMapPaper = Content.Load<Texture2D>("map_paper");
            txMapDoorClosed = Content.Load<Texture2D>("map_doorclosed");
            txMapDoorOpen = Content.Load<Texture2D>("map_dooropen");
            txMapGround = Content.Load<Texture2D>("map_ground");
            txMapWall = Content.Load<Texture2D>("map_wall");
            txMapStairs = Content.Load<Texture2D>("map_stairs");
            txMapPlayer = Content.Load<Texture2D>("map_player");
            txMapMerchant = Content.Load<Texture2D>("map_merchant");
            txMapTeleporter = Content.Load<Texture2D>("map_teleporter");

            //Tile textures
            txTileEarth = Content.Load<Texture2D>("earth");
            txTileWall = Content.Load<Texture2D>("wall");
            txTileDoorClosed = Content.Load<Texture2D>("door");
            txTileDoorOpen = Content.Load<Texture2D>("open_door");
            txTileStairsUp = Content.Load<Texture2D>("upstairs");
            txTileStairsDown = Content.Load<Texture2D>("downstairs");

            txTileSet1Wall = Content.Load<Texture2D>("set1_wall");
            txTileSet1DoorOpen = Content.Load<Texture2D>("set1_dooropen");
            txTileSet1DoorClosed = Content.Load<Texture2D>("set1_doorclosed");
            txTileSet2Wall = Content.Load<Texture2D>("set2_wall");
            txTileSet2DoorOpen = Content.Load<Texture2D>("set2_dooropen");
            txTileSet2DoorClosed = Content.Load<Texture2D>("set2_doorclosed");
            txTileSet3Wall = Content.Load<Texture2D>("set3_wall");
            txTileSet3DoorOpen = Content.Load<Texture2D>("set3_dooropen");
            txTileSet3DoorClosed = Content.Load<Texture2D>("set3_doorclosed");
            txTileSet4Wall = Content.Load<Texture2D>("set4_wall");
            txTileSet4DoorOpen = Content.Load<Texture2D>("set4_dooropen");
            txTileSet4DoorClosed = Content.Load<Texture2D>("set4_doorclosed");
            txTileSet5Wall = Content.Load<Texture2D>("set5_wall");
            txTileSet5DoorOpen = Content.Load<Texture2D>("set5_dooropen");
            txTileSet5DoorClosed = Content.Load<Texture2D>("set5_doorclosed");

            //Village textures
            txVillageDoorClosed = Content.Load<Texture2D>("tile_village_doorclosed");
            txVillageDoorOpen = Content.Load<Texture2D>("tile_village_dooropen");
            txVillageGrass = Content.Load<Texture2D>("tile_village_grass");
            txVillageHedge = Content.Load<Texture2D>("tile_village_hedge");
            txVillagePath = Content.Load<Texture2D>("tile_village_path");
            txVillageWall = Content.Load<Texture2D>("tile_village_wall");
            txVillageWater = Content.Load<Texture2D>("tile_village_water");
            txVillageTree = Content.Load<Texture2D>("tile_village_tree");
            txVillageFloor = Content.Load<Texture2D>("tile_village_floor");

            //Item Textures
            txItemArmour = Content.Load<Texture2D>("item_armour");
            txItemCoins = Content.Load<Texture2D>("item_coins");
            txItemShield = Content.Load<Texture2D>("item_shield");
            txItemSword = Content.Load<Texture2D>("item_sword");
            txItemDagger = Content.Load<Texture2D>("item_dagger");
            txItemMace = Content.Load<Texture2D>("item_mace");
            txItemBow = Content.Load<Texture2D>("item_bow");
            txItemSpear = Content.Load<Texture2D>("item_spear");
            txItemCloak = Content.Load<Texture2D>("item_cloak");
            txItemPotion = Content.Load<Texture2D>("item_potion");
            txItemSuperPotion = Content.Load<Texture2D>("item_superpotion");
            txItemScroll = Content.Load<Texture2D>("item_scroll");
            txItemFoodBread = Content.Load<Texture2D>("food_bread");
            txItemFoodCanned = Content.Load<Texture2D>("food_canned");
            txItemFoodMeat = Content.Load<Texture2D>("food_meat");
            txItemFoodVeg = Content.Load<Texture2D>("food_veg");
            txItemFoodCorpse = Content.Load<Texture2D>("item_corpse");
            txRockRock = Content.Load<Texture2D>("rock_rock");
            txRockMetal = Content.Load<Texture2D>("rock_metal");
            txRockGem = Content.Load<Texture2D>("rock_gem");
            txRockGrindStone = Content.Load<Texture2D>("rock_grindstone");
            txItemQuest = Content.Load<Texture2D>("item_quest");

            //Furniture
            txFurnAltar = Content.Load<Texture2D>("furn_altar");
            txFurnBarrel = Content.Load<Texture2D>("furn_barrel");
            txFurnCandelabra = Content.Load<Texture2D>("furn_candelabra");
            txFurnCrate = Content.Load<Texture2D>("furn_crate");
            txFurnGravestone = Content.Load<Texture2D>("furn_gravestone");
            txFurnSignpost = Content.Load<Texture2D>("furn_signpost");
            txFurnStatue = Content.Load<Texture2D>("furn_statue");
            txFurnTable = Content.Load<Texture2D>("furn_table");
            txFurnTeleporter = Content.Load<Texture2D>("furn_teleporter");
            txFurnChairLeft = Content.Load<Texture2D>("furn_chair_left");
            txFurnChairRight = Content.Load<Texture2D>("furn_chair_right");

            //Creatures
            txPlayerElf = Content.Load<Texture2D>("player_elf_gor");
            txPlayerElf_Back = Content.Load<Texture2D>("player_elf_back_gor");
            txPlayerElf_Left = Content.Load<Texture2D>("player_elf_left_gor");
            txPlayerElf_Right = Content.Load<Texture2D>("player_elf_right_gor");
            txPlayerHuman = Content.Load<Texture2D>("player_human_gor");
            txPlayerHuman_Back = Content.Load<Texture2D>("player_human_back_gor");
            txPlayerHuman_Left = Content.Load<Texture2D>("player_human_left_gor");
            txPlayerHuman_Right = Content.Load<Texture2D>("player_human_right_gor");
            txPlayerOrc = Content.Load<Texture2D>("player_orc_gor");
            txPlayerOrc_Back = Content.Load<Texture2D>("player_orc_back_gor");
            txPlayerOrc_Left = Content.Load<Texture2D>("player_orc_left_gor");
            txPlayerOrc_Right = Content.Load<Texture2D>("player_orc_right_gor");
            txCreatureBat = Content.Load<Texture2D>("creature_bat_gor");
            txCreatureRodent = Content.Load<Texture2D>("creature_rodent_gor");
            txCreatureLizard = Content.Load<Texture2D>("creature_lizard_gor");
            txCreatureBug = Content.Load<Texture2D>("creature_beetle_gor");
            txCreatureCanine = Content.Load<Texture2D>("creature_canine_gor");
            txCreatureMerchant = Content.Load<Texture2D>("creature_merchant_gor");
            txCreatureDragon = Content.Load<Texture2D>("creature_dragon_gor");
            txCreatureGoblin = Content.Load<Texture2D>("creature_goblin_gor");
            txCreatureSnake = Content.Load<Texture2D>("creature_serpent_gor");
            txCreatureSuccubus = Content.Load<Texture2D>("creature_succubus_gor");
            txCreatureSpider = Content.Load<Texture2D>("creature_spider_gor");
            txCreatureDemon = Content.Load<Texture2D>("creature_demon_gor");
            txCreatureHuman = Content.Load<Texture2D>("creature_human_gor");
            txCreatureSkeleton = Content.Load<Texture2D>("creature_skeleton_gor");
            txCreatureGhost = Content.Load<Texture2D>("creature_ghost_gor");
            txCreatureSlime = Content.Load<Texture2D>("creature_slime_gor");
            txCreatureTroll = Content.Load<Texture2D>("creature_troll_gor");
            txCreatureGoldrick = Content.Load<Texture2D>("creature_goldrick_gor");
            txCreatureZombie = Content.Load<Texture2D>("creature_zombie_gor");
            txCreatureCook = Content.Load<Texture2D>("creature_cook_gor");
            txCreatureVillager = Content.Load<Texture2D>("creature_villager_gor");
            txCreatureQuestMaster = Content.Load<Texture2D>("creature_questmaster_gor");
            txLargeCreatureQuestMaster = Content.Load<Texture2D>("largecreature_questmaster_gor");
            txLargeCreatureMerchant = Content.Load<Texture2D>("largecreature_merchant_gor");
            txLargePlayerElf = Content.Load<Texture2D>("largecreature_player_elf");
            txLargePlayerHuman = Content.Load<Texture2D>("largecreature_player_human");
            txLargePlayerOrc = Content.Load<Texture2D>("largecreature_player_orc");

            //Traps
            txTrapArrow = Content.Load<Texture2D>("trap_arrow");
            txTrapDescent = Content.Load<Texture2D>("trap_descent");
            txTrapPoison = Content.Load<Texture2D>("trap_poison");
            txTrapTeleport = Content.Load<Texture2D>("trap_teleport");

            //Particles
            txEffectDarkness = Content.Load<Texture2D>("effect_darkness");
            txEffectFire = Content.Load<Texture2D>("effect_fire");
            txEffectIce = Content.Load<Texture2D>("effect_ice");
            txEffectImpact = Content.Load<Texture2D>("effect_impact");
            txEffectGas = Content.Load<Texture2D>("effect_poison");
            txEffectHeal = Content.Load<Texture2D>("effect_heal");
            txEffectLevelUp = Content.Load<Texture2D>("effect_levelup");

            txAlliedIndicator = Content.Load<Texture2D>("indicator_allied");
            txHostileIndicator = Content.Load<Texture2D>("indicator_hostile");

            //Buttons
            btnTxButtonB = Content.Load<Texture2D>("xboxControllerButtonB");
            btnTxButtonX = Content.Load<Texture2D>("xboxControllerButtonX");
            btnTxButtonA = Content.Load<Texture2D>("xboxControllerButtonA");
            btnTxButtonY = Content.Load<Texture2D>("xboxControllerButtonY");
            btnTxButtonDPad = Content.Load<Texture2D>("xboxControllerDPad");
            btnTxLeftShoulder = Content.Load<Texture2D>("xboxControllerLeftShoulder");
            btxTxRightShoulder = Content.Load<Texture2D>("xboxControllerRightShoulder");
            btnTxStart = Content.Load<Texture2D>("xboxControllerStart");
            btnTxGuide = Content.Load<Texture2D>("xboxControllerButtonGuide");
            btnTxLeftTrigger = Content.Load<Texture2D>("xboxControllerLeftTrigger");
            btnTxRightTrigger = Content.Load<Texture2D>("xboxControllerRightTrigger");
            btnTxBack = Content.Load<Texture2D>("xboxControllerBack");
            btnTxLeftStick = Content.Load<Texture2D>("xboxControllerLeftThumbstick");
            btnTxRightStick = Content.Load<Texture2D>("xboxControllerRightThumbstick");

            //Sound Effects
            sfxDanger = Content.Load<SoundEffect>("SoundEffects/sfxDanger");
            sfxDeath = Content.Load<SoundEffect>("SoundEffects/sfxDeath");
            sfxEnemyAttack = Content.Load<SoundEffect>("SoundEffects/sfxEnemyAttackPlayer");
            sfxFail = Content.Load<SoundEffect>("SoundEffects/sfxFail");
            sfxFanfare = Content.Load<SoundEffect>("SoundEffects/sfxFanfare2");
            sfxMagicFire = Content.Load<SoundEffect>("SoundEffects/sfxMagicFire");
            sfxMagicHeal = Content.Load<SoundEffect>("SoundEffects/sfxMagicHeal");
            sfxMagicIce = Content.Load<SoundEffect>("SoundEffects/sfxMagicIce");
            sfxMagicMissile = Content.Load<SoundEffect>("SoundEffects/sfxMagicMissile");
            sfxMenuCancel = Content.Load<SoundEffect>("SoundEffects/sfxMenuCancel");
            sfxMenuConfirm = Content.Load<SoundEffect>("SoundEffects/sfxMenuConfirm");
            sfxMenuSelect = Content.Load<SoundEffect>("SoundEffects/sfxMenuSelect2");
            sfxRecruit = Content.Load<SoundEffect>("SoundEffects/sfxRecruit");
            sfxSelectBad = Content.Load<SoundEffect>("SoundEffects/sfxSelectBad");
            sfxStep = Content.Load<SoundEffect>("SoundEffects/sfxStep");
            sfxStepFiltered = Content.Load<SoundEffect>("SoundEffects/sfxStepFiltered");
            sfxTeleport = Content.Load<SoundEffect>("SoundEffects/sfxTeleport2");
            sfxBlade = Content.Load<SoundEffect>("SoundEffects/sfxBlade");
            sfxClub = Content.Load<SoundEffect>("SoundEffects/sfxClub");
            sfxArrow = Content.Load<SoundEffect>("SoundEffects/sfxArrow");
            sfxCoins = Content.Load<SoundEffect>("SoundEffects/sfxCoins");
            sfxPaper = Content.Load<SoundEffect>("SoundEffects/sfxPaper");
            sfxLevelUp = Content.Load<SoundEffect>("SoundEffects/sfxLevelUp2");

            songTitle = Content.Load<Song>("Music/Title");
            songLogo = Content.Load<Song>("Music/Logo_LOUD");
            songOutro = Content.Load<Song>("Music/Ending");

            villageMusic = new BackgroundMusicTrack("Calatrava", "Citezen", Content.Load<Song>("Music/citezen_calatrava"));
            dungeonMusicList = new List<BackgroundMusicTrack>();
            dungeonMusicList.Add(new BackgroundMusicTrack("Fricks Dub", "Citezen", Content.Load<Song>("Music/citezen_fricks_dub")));
            dungeonMusicList.Add(new BackgroundMusicTrack("Hotel", "Citezen", Content.Load<Song>("Music/citezen_hotel")));
            dungeonMusicList.Add(new BackgroundMusicTrack("Vessels", "Citezen", Content.Load<Song>("Music/citezen_vessels")));
            dungeonMusicList.Add(new BackgroundMusicTrack("East Angular", "Scheme Boy", Content.Load<Song>("Music/schemeboy_eastangular")));
            dungeonMusicList.Add(new BackgroundMusicTrack("London Boys & City Traders", "Scheme Boy", Content.Load<Song>("Music/schemeboy_lbct")));
            dungeonMusicList.Add(new BackgroundMusicTrack("Plinky", "Scheme Boy", Content.Load<Song>("Music/schemeboy_plinky")));
            dungeonMusicList.Add(new BackgroundMusicTrack("Sod Off October", "Scheme Boy", Content.Load<Song>("Music/schemeboy_sodoffoctober")));
            dungeonMusic = dungeonMusicList[0];
            //TODO - Shouldn't put code in this method! But need to play title music at start
            MediaPlayer.Play(songLogo);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void InitBossList()
        {
            bossList = new List<Boss>();
            bossList.Add(new Boss(500, "Krull", false));
            bossList.Add(new Boss(1000, "Garath The Terrible", false));
            bossList.Add(new Boss(2000, "Chuckyhacks and Duckbeard", false));
            bossList.Add(new Boss(3000, "Ayeyerma", false));
            bossList.Add(new Boss(4000, "Mictlantecuhtli", false));
            bossList.Add(new Boss(5000, "Crocell", false));
            bossList.Add(new Boss(6000, "ElverKong", false));
            bossList.Add(new Boss(7000, "Azazel", false));
            bossList.Add(new Boss(8000, "Xaphan", false));
            bossList.Add(new Boss(9000, "Gold Rick", false));
        }

        public void InitMainMenu()
        {
            mainMenuStringList = new List<string>();
            mainMenuStringList.Add("New Game");
            mainMenuStringList.Add("Resume");
            mainMenuStringList.Add("Stats");
            mainMenuStringList.Add("Feats");
            mainMenuStringList.Add("Help");
            mainMenuStringList.Add("Credits");
            mainMenuStringList.Add("Exit To Dashboard");
            if (isTrialMode) mainMenuStringList.Add("Unlock Full Game");
        }

        public void InitFeats()
        {
            featList = new List<Feat>();
            Feat newFeat;

            //FEAT_KILL_ENEMIES = 0;
            newFeat = new Feat();
            newFeat.ID = SC.FEAT_KILL_ENEMIES;
            newFeat.title = "MONSTER HUNTER";
            newFeat.description = "Awarded for defeating 100 enemies";
            newFeat.descriptionWhenHidden = "HINT: Defeat enemies...";
            newFeat.progress = 0;
            newFeat.goal = 100;
            newFeat.rank = 0;
            newFeat.MAX_RANK = 50;
            newFeat.rewardXP = 500;
            newFeat.rewardGold = 200;
            featList.Add(newFeat);

            //FEAT_KILL_DANGEROUS_ENEMIES = 1;
            newFeat = new Feat();
            newFeat.ID = SC.FEAT_KILL_DANGEROUS_ENEMIES;
            newFeat.title = "GIANT KILLER";
            newFeat.description = "Awarded for defeating dangerous enemies";
            newFeat.descriptionWhenHidden = "HINT: Can you take down dangerous enemies?";
            newFeat.progress = 0;
            newFeat.goal = 50;
            newFeat.rank = 0;
            newFeat.MAX_RANK = 50;
            newFeat.rewardXP = 1000;
            newFeat.rewardGold = 400;
            featList.Add(newFeat);

            //FEAT_KILL_FROZEN_ENEMIES = 2;
            newFeat = new Feat();
            newFeat.ID = SC.FEAT_KILL_FROZEN_ENEMIES;
            newFeat.title = "ICE TO SEE YOU";
            newFeat.description = "Awarded for killing enemies while frozen";
            newFeat.descriptionWhenHidden = "HINT: Use the spell \"Freeze\"";
            newFeat.progress = 0;
            newFeat.goal = 50;
            newFeat.rank = 0;
            newFeat.MAX_RANK = 50;
            newFeat.rewardXP = 500;
            newFeat.rewardGold = 100;
            featList.Add(newFeat);

            //FEAT_KILL_BOSSES = 3;
            newFeat = new Feat();
            newFeat.ID = SC.FEAT_KILL_BOSSES;
            newFeat.title = "LEGENDARY VICTORY";
            newFeat.description = "Awarded for defeating a boss";
            newFeat.descriptionWhenHidden = "??????????";
            newFeat.progress = 0;
            newFeat.goal = 1;
            newFeat.rank = 0;
            newFeat.MAX_RANK = 10;
            newFeat.rewardXP = 1000;
            newFeat.rewardGold = 200;
            featList.Add(newFeat);

            //FEAT_RECRUIT_ALLIES = 4;
            newFeat = new Feat();
            newFeat.ID = SC.FEAT_RECRUIT_ALLIES;
            newFeat.title = "TEAM PLAYER";
            newFeat.description = "Awarded for recruiting creatures to aid you";
            newFeat.descriptionWhenHidden = "HINT: It's Dangerous To Go Alone!";
            newFeat.progress = 0;
            newFeat.goal = 50;
            newFeat.rank = 0;
            newFeat.MAX_RANK = 50;
            newFeat.rewardXP = 200;
            newFeat.rewardGold = 100;
            featList.Add(newFeat);

            //FEAT_COMPLETE_QUESTS = 5;
            newFeat = new Feat();
            newFeat.ID = SC.FEAT_COMPLETE_QUESTS;
            newFeat.title = "VILLAGE HERO";
            newFeat.description = "Awarded for completing quests";
            newFeat.descriptionWhenHidden = "??????????";
            newFeat.progress = 0;
            newFeat.goal = 5;
            newFeat.rank = 0;
            newFeat.MAX_RANK = 200;
            newFeat.rewardXP = 500;
            newFeat.rewardGold = 100;
            featList.Add(newFeat);

            //FEAT_COMPLETE_FIGHTER_QUESTS = 6;
            newFeat = new Feat();
            newFeat.ID = SC.FEAT_COMPLETE_FIGHTER_QUESTS;
            newFeat.title = "FIGHTING SPIRIT";
            newFeat.description = "Awarded for completing quests as a Fighter";
            newFeat.descriptionWhenHidden = "??????????";
            newFeat.progress = 0;
            newFeat.goal = 10;
            newFeat.rank = 0;
            newFeat.MAX_RANK = 50;
            newFeat.rewardXP = 500;
            newFeat.rewardGold = 50;
            featList.Add(newFeat);

            //FEAT_COMPLETE_MAGE_QUESTS = 7;
            newFeat = new Feat();
            newFeat.ID = SC.FEAT_COMPLETE_MAGE_QUESTS;
            newFeat.title = "MAGIC TOUCH";
            newFeat.description = "Awarded for completing quests as a Mage";
            newFeat.descriptionWhenHidden = "??????????";
            newFeat.progress = 0;
            newFeat.goal = 10;
            newFeat.rank = 0;
            newFeat.MAX_RANK = 50;
            newFeat.rewardXP = 500;
            newFeat.rewardGold = 50;
            featList.Add(newFeat);

            //FEAT_COMPLETE_EXPLORER_QUESTS = 8;
            newFeat = new Feat();
            newFeat.ID = SC.FEAT_COMPLETE_EXPLORER_QUESTS;
            newFeat.title = "FEARLESS ADVENTURER";
            newFeat.description = "Awarded for completing quests as an Explorer";
            newFeat.descriptionWhenHidden = "??????????";
            newFeat.progress = 0;
            newFeat.goal = 10;
            newFeat.rank = 0;
            newFeat.MAX_RANK = 50;
            newFeat.rewardXP = 2000;
            newFeat.rewardGold = 150;
            featList.Add(newFeat);

            //FEAT_COLLECT_GEMS = 9;
            newFeat = new Feat();
            newFeat.ID = SC.FEAT_COLLECT_GEMS;
            newFeat.title = "BEJEWELED";
            newFeat.description = "Awarded for collecting precious gems";
            newFeat.descriptionWhenHidden = "HINT: Great treasures lie behind dungeon walls...";
            newFeat.progress = 0;
            newFeat.goal = 10;
            newFeat.rank = 0;
            newFeat.MAX_RANK = 50;
            newFeat.rewardXP = 200;
            newFeat.rewardGold = 50;
            featList.Add(newFeat);
        }

        private void UpdateFeats()
        {
            for (int i = 0; i < featList.Count; i++)
            {
                if (featList[i].rank < featList[i].MAX_RANK)
                {
                    if (featList[i].progress >= featList[i].goal)
                    {
                        featList[i].rank++;
                        featList[i].progress = 0;
                        featList[i].hidden = false;
                        totalBonusGold += featList[i].rewardGold;
                        totalBonusXP += featList[i].rewardXP;
                        sfxRecruit.Play();
                        featPanelDisplayTimer = 0;
                        featToDisplay = featList[i];
                        //Goal and reward increases depend on which feat it is
                        switch (featList[i].ID)
                        {
                            case SC.FEAT_COLLECT_GEMS:
                                featList[i].goal += 10;
                                featList[i].rewardGold += 50;
                                featList[i].rewardXP += 50;
                                break;
                            case SC.FEAT_COMPLETE_EXPLORER_QUESTS:
                                featList[i].rewardGold += 50;
                                featList[i].rewardXP += 100;
                                break;
                            case SC.FEAT_COMPLETE_FIGHTER_QUESTS:
                                featList[i].rewardGold += 50;
                                featList[i].rewardXP += 50;
                                break;
                            case SC.FEAT_COMPLETE_MAGE_QUESTS:
                                featList[i].rewardGold += 50;
                                featList[i].rewardXP += 50;
                                break;
                            case SC.FEAT_COMPLETE_QUESTS:
                                featList[i].rewardGold += 50;
                                featList[i].rewardXP += 100;
                                break;
                            case SC.FEAT_KILL_DANGEROUS_ENEMIES:
                                featList[i].goal += featList[i].goal;
                                featList[i].rewardGold += featList[i].rewardGold;
                                featList[i].rewardXP += featList[i].rewardXP;
                                break;
                            case SC.FEAT_KILL_ENEMIES:
                                featList[i].goal += featList[i].goal;
                                featList[i].rewardGold += featList[i].rewardGold;
                                featList[i].rewardXP += featList[i].rewardXP;
                                break;
                            case SC.FEAT_KILL_FROZEN_ENEMIES:
                                featList[i].goal += 10;
                                featList[i].rewardGold += 50;
                                featList[i].rewardXP += 50;
                                break;
                            case SC.FEAT_RECRUIT_ALLIES:
                                featList[i].goal += 10;
                                featList[i].rewardGold += 50;
                                featList[i].rewardXP += 50;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private void IncreaseFeat(int featID, int amount)
        {
            if (!cheating)
            {
                for (int i = 0; i < featList.Count; i++)
                {
                    if (featList[i].ID == featID)
                    {
                        featList[i].progress += amount;
                        if (featList[i].progress > featList[i].goal) featList[i].progress = featList[i].goal;
                    }
                }
            }
        }

        private void UpdateGameOver()
        {
            creationScreenBackgroundOffset++;
            if (creationScreenBackgroundOffset >= 32) creationScreenBackgroundOffset = 0;

            if (controller.AIsPressed())
            {
                currentGameState = GAMESTATE_TITLESCREEN;
                MediaPlayer.Stop();
                MediaPlayer.Play(songTitle);
                settingsLoadComplete = false;
                if (!cheating)
                {
                    totalBonusXP += runBonusXP;
                    totalBonusGold += Math.Max(0, runBonusGold - goldBonusCorrection);
                }
                if (!player1.alive) totalStats.IncrementStat("Number of Deaths");
                totalStats.AddStats(runStats);
                totalStats.Sort();
                totalSecondsPlayed += (long)runTime.Elapsed.TotalSeconds;
                runTime.Stop();
                isTrialMode = Guide.IsTrialMode;
                if (!isTrialMode && !cheating) SaveSettings();
            }
        }

        private void UpdateGamePlaying()
        {
            if (currentLevel.isBossLevel)
            {
                if (!currentLevel.doorRemoved)
                {
                    if (CanRemoveBossDoor())
                    {
                        currentLevel.RemoveDoor();
                        AddMessage("The Door Behind You Disappears!", true);
                        sfxSelectBad.Play();
                    }
                }
                if (currentLevel.exitHidden)
                {
                    if (currentLevel.AreAllHostilesDead())
                    {
                        currentLevel.MakeExitAppear();
                        AddMessage("The Exit Appeared...");
                        sfxRecruit.Play();
                    }
                }
            }

            if (playerIdle)
            {
                if (AnyInputDetected(playerIndex))
                {
                    currentFramesIdle = 0;
                    playerIdle = false;
                    runTime.Start();
                }
            }
            else
            {
                if (AnyInputDetected(playerIndex))
                {
                    currentFramesIdle = 0;
                }
                else
                {
                    currentFramesIdle++;
                    if (currentFramesIdle > framesUntilIdle)
                    {
                        playerIdle = true;
                        runTime.Stop();
                    }
                }
            }

            if (controller.LogDownIsPressed() && !dialogOpen)
            {
                if (messages.Count > NUM_MESSAGES_SHOWN
                    && messageDisplayOffset + NUM_MESSAGES_SHOWN < messages.Count)
                {
                    messageDisplayOffset++;
                }
            }
            if (controller.LogUpIsPressed() && !dialogOpen)
            {
                if (messageDisplayOffset > 0) messageDisplayOffset--;
            }

            if (mapOpenBackNeedsReleased)
            {
                if (controller.BackIsReleased()) mapOpenBackNeedsReleased = false;
            }

            if (!player1.alive)
            {
                if (player1.CanBeResurrected())
                {
                    player1.Resurrect();
                    if (currentLevel.isBossLevel) currentLevel.ReplaceDoor();
                    player1.x = currentLevel.startingX;
                    player1.y = currentLevel.startingY;
                    AddMessage("You Have Been Resurrected");
                    StopFastMovement();
                    sfxDeath.Play();
                }
                else
                {
                    if (!isSfxDeathPlayedThisGame)
                    {
                        isSfxDeathPlayedThisGame = true;
                        sfxDeath.Play();
                    }
                    if (controller.YIsPressed())
                        GameOver();
                }
            }
            else
            {
                if (tutorialMessageOpen)
                {
                    if (controller.BIsPressed())
                    {
                        tutorialMessageOpen = false;
                    }
                    if (controller.XIsPressed())
                    {
                        NeverShowTutorialMessageAgain(currentTutorialMessageOpen);
                        tutorialMessageOpen = false;
                    }
                }
                else
                {
                    
                    if (!dialogOpen && !isCastingSpell)
                    {
                        bool movementDetected = false;
                        if (controller.UpRightDiagonalHeld() && !hasMadeMove && !movementDetected)
                        {
                            movementDetected = true;
                            if (movementHoldDirection == SC.DIRECTION_UPRIGHT)
                            {
                                movementHoldDuration++;
                            }
                            else
                            {
                                movementHoldDuration = 0;
                                movementHoldDirection = SC.DIRECTION_UPRIGHT;
                            }
                            if (controller.UpRightDiagonalPressed() || IsFastMoving())
                            {
                                MovePlayer(SC.DIRECTION_UPRIGHT, 1, -1);
                            }
                        }

                        if (controller.DownRightDiagonalHeld() && !hasMadeMove && !movementDetected)
                        {
                            movementDetected = true;
                            if (movementHoldDirection == SC.DIRECTION_DOWNRIGHT)
                            {
                                movementHoldDuration++;
                            }
                            else
                            {
                                movementHoldDuration = 0;
                                movementHoldDirection = SC.DIRECTION_DOWNRIGHT;
                            }
                            if (controller.DownRightDiagonalPressed() || IsFastMoving())
                            {
                                MovePlayer(SC.DIRECTION_DOWNRIGHT, 1, 1);
                            }
                        }

                        if (controller.DownLeftDiagonalHeld() && !hasMadeMove && !movementDetected)
                        {
                            movementDetected = true;
                            if (movementHoldDirection == SC.DIRECTION_DOWNLEFT)
                            {
                                movementHoldDuration++;
                            }
                            else
                            {
                                movementHoldDuration = 0;
                                movementHoldDirection = SC.DIRECTION_DOWNLEFT;
                            }
                            if (controller.DownLeftDiagonalPressed() || IsFastMoving())
                            {
                                MovePlayer(SC.DIRECTION_DOWNLEFT, -1, 1);
                            }
                        }

                        if (controller.UpLeftDiagonalHeld() && !hasMadeMove && !movementDetected)
                        {
                            movementDetected = true;
                            if (movementHoldDirection == SC.DIRECTION_UPLEFT)
                            {
                                movementHoldDuration++;
                            }
                            else
                            {
                                movementHoldDuration = 0;
                                movementHoldDirection = SC.DIRECTION_UPLEFT;
                            }
                            if (controller.UpLeftDiagonalPressed() || IsFastMoving())
                            {
                                MovePlayer(SC.DIRECTION_UPLEFT, -1, -1);
                            }
                        }

                        if (controller.RightIsHeld() && !hasMadeMove && !movementDetected)
                        {
                            movementDetected = true;
                            if (movementHoldDirection == SC.DIRECTION_RIGHT)
                            {
                                movementHoldDuration++;
                            }
                            else
                            {
                                movementHoldDuration = 0;
                                movementHoldDirection = SC.DIRECTION_RIGHT;
                            }
                            if (controller.LastRightReleased() || IsFastMoving())
                            {
                                MovePlayer(SC.DIRECTION_RIGHT, 1, 0);
                            }
                        }
                        if (controller.LeftIsHeld() && !hasMadeMove && !movementDetected)
                        {
                            movementDetected = true;
                            if (movementHoldDirection == SC.DIRECTION_LEFT)
                            {
                                movementHoldDuration++;
                            }
                            else
                            {
                                movementHoldDuration = 0;
                                movementHoldDirection = SC.DIRECTION_LEFT;
                            }
                            if (controller.LastLeftReleased() || IsFastMoving())
                            {
                                MovePlayer(SC.DIRECTION_LEFT, -1, 0);
                            }
                        }

                        if (controller.UpIsHeld() && !hasMadeMove && !movementDetected)
                        {
                            movementDetected = true;
                            if (movementHoldDirection == SC.DIRECTION_UP)
                            {
                                movementHoldDuration++;
                            }
                            else
                            {
                                movementHoldDuration = 0;
                                movementHoldDirection = SC.DIRECTION_UP;
                            }
                            if (controller.LastUpReleased() || IsFastMoving())
                            {
                                MovePlayer(SC.DIRECTION_UP, 0, -1);
                            }
                        }
                        if (controller.DownIsHeld() && !hasMadeMove && !movementDetected)
                        {
                            movementDetected = true;
                            if (movementHoldDirection == SC.DIRECTION_DOWN)
                            {
                                movementHoldDuration++;
                            }
                            else
                            {
                                movementHoldDuration = 0;
                                movementHoldDirection = SC.DIRECTION_DOWN;
                            }
                            if (controller.LastDownReleased() || IsFastMoving())
                            {
                                MovePlayer(SC.DIRECTION_DOWN, 0, 1);
                            }
                        }
                        //If not pushing in any direction, hold duration goes back to zero
                        if (!controller.MovementAnyDirectionHeld())
                        {
                            movementHoldDuration = 0;
                        }

                        //If Right Stick is pushed in any direction, the player is looking around
                        playerLooking = controller.RightStickAnyDirectionHeld()
                            || (controller.LeftShoulderIsHeld() && (playerLookAtX != player1.x || playerLookAtY != player1.y));
                        if (playerLooking)
                        {
                            if (controller.LeftShoulderIsReleased())
                            {
                                playerLookAtX = player1.x;
                                playerLookAtY = player1.y;
                                if (controller.RightStickDownHeld())
                                {
                                    playerLookAtY = player1.y + 1;
                                }
                                if (controller.RightStickUpHeld())
                                {
                                    playerLookAtY = player1.y - 1;
                                }
                                if (controller.RightStickLeftHeld())
                                {
                                    playerLookAtX = player1.x - 1;
                                }
                                if (controller.RightStickRightHeld())
                                {
                                    playerLookAtX = player1.x + 1;
                                }
                            }
                            else
                            {
                                if (delay_current_move_looking == DELAY_MOVE_LOOKING)
                                {
                                    if (controller.RightStickDownHeld() && playerLookAtY < currentLevel.DIMENSION - 1)
                                    {
                                        playerLookAtY += 1;
                                        delay_current_move_looking = 0;
                                    }
                                    if (controller.RightStickUpHeld() && playerLookAtY > 0)
                                    {
                                        playerLookAtY -= 1;
                                        delay_current_move_looking = 0;
                                    }
                                    if (controller.RightStickLeftHeld() && playerLookAtX > 0)
                                    {
                                        playerLookAtX -= 1;
                                        delay_current_move_looking = 0;
                                    }
                                    if (controller.RightStickRightHeld() && playerLookAtX < currentLevel.DIMENSION - 1)
                                    {
                                        playerLookAtX += 1;
                                        delay_current_move_looking = 0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            playerLookAtX = player1.x;
                            playerLookAtY = player1.y;
                        }

                        if (controller.LeftTriggerIsPulled() && !hasMadeMove)
                        {
                            hasMadeMove = PlayerAction(playerLookAtX, playerLookAtY);
                        }

                        if (controller.RightTriggerIsPulled() && !hasMadeMove)
                        {
                            if (playerLookAtX >= 0 && playerLookAtY >= 0
                                && playerLookAtX < currentLevel.DIMENSION && playerLookAtY < currentLevel.DIMENSION)
                            {
                                contextDialogOpen = true;
                                controller.Update(); //Update the controller state so that trigger pull is not re-used
                                dialogOpen = true;
                                currentContextMenuSelection = 0;
                                if (playerLooking)
                                {
                                    contextMenuStringList = GetTileContextMenuOptions(playerLookAtX, playerLookAtY);
                                }
                                else
                                {
                                    contextMenuStringList = GetTileContextMenuOptions(player1.x, player1.y);
                                    playerLookAtX = player1.x;
                                    playerLookAtY = player1.y;
                                }
                            }
                        }
                        if (controller.StartIsPressed())
                        {
                            startMenuOpen = true;
                            dialogOpen = true;
                            currentStartMenuSelection = 0;
                            sfxMenuConfirm.Play();
                        }
                        if (controller.RightShoulderIsPressed())
                        {
                            if (inVillage && player1.spells.GetKnownSpells().Count > 0)
                            {
                                AddMessage("Casting spells in the village is forbidden!", true);
                                sfxSelectBad.Play();
                            }
                            else
                            {
                                if (player1.spells.GetKnownSpells().Count > 0)
                                {
                                    spellListOpen = true;
                                    dialogOpen = true;
                                    currentSpellListSelection = 0;
                                }
                            }
                        }

                        //Rest
                        if (controller.BIsPressed() && !hasMadeMove)
                        {
                            player1.Rest(inVillage);
                            hasMadeMove = true;
                        }
                        else
                        {
                            if (controller.BIsHeld() && controller.LeftShoulderIsHeld() && !hasMadeMove)
                            {
                                player1.Rest(inVillage);
                                hasMadeMove = true;
                            }
                        }

                        if (controller.BackIsPressed() && !hasMadeMove && !mapOpenBackNeedsReleased)
                        {
                            sfxPaper.Play();
                            dialogOpen = true;
                            mapOpen = true;
                            mapTiles = currentLevel.KnownMap();
                            startMenuOpen = false;
                        }

                        if (controller.YIsPressed() && !hasMadeMove)
                        {
                            dialogOpen = true;
                            inventoryOpen = true;
                            inventoryOffset = 0;
                            currentInventorySelection = 0;
                            UpdateInventoryTabs();
                        }

                    }
                    else //dialogOpen or SpellCasting
                    {
                        if (isCastingSpell)
                        {
                            if (controller.BIsPressed())
                            {
                                isCastingSpell = false;
                            }
                            else
                            {
                                if (spellTypeBeingCast == (int)SC.SpellCasting.DIRECTION)
                                {
                                    if (delay_current_spell_targeting == DELAY_SPELL_TARGETING)
                                    {
                                        if (spellTargetX == player1.x && spellTargetY == player1.y)
                                        {
                                            spellTargetX = player1.x - 1;
                                        }

                                        if (controller.DownLeftDiagonalHeld() || controller.RightStickDownLeftDiagonalHeld())
                                        {
                                            spellTargetX = player1.x - 1;
                                            spellTargetY = player1.y + 1;
                                        }
                                        else if (controller.DownRightDiagonalHeld() || controller.RightStickDownRightDiagonalHeld())
                                        {
                                            spellTargetX = player1.x + 1;
                                            spellTargetY = player1.y + 1;
                                        }
                                        else if (controller.UpLeftDiagonalHeld() || controller.RightStickUpLeftDiagonalHeld())
                                        {
                                            spellTargetX = player1.x - 1;
                                            spellTargetY = player1.y - 1;
                                        }
                                        else if (controller.UpRightDiagonalHeld() || controller.RightStickUpRightDiagonalHeld())
                                        {
                                            spellTargetX = player1.x + 1;
                                            spellTargetY = player1.y - 1;
                                        }
                                        else if (controller.UpIsHeld() || controller.RightStickUpHeld())
                                        {
                                            spellTargetY = player1.y - 1;
                                            spellTargetX = player1.x;
                                        }
                                        else if (controller.DownIsHeld() || controller.RightStickDownHeld())
                                        {
                                            spellTargetY = player1.y + 1;
                                            spellTargetX = player1.x;
                                        }
                                        else if (controller.LeftIsHeld() || controller.RightStickLeftHeld())
                                        {
                                            spellTargetX = player1.x - 1;
                                            spellTargetY = player1.y;
                                        }
                                        else if (controller.RightIsHeld() || controller.RightStickRightHeld())
                                        {
                                            spellTargetX = player1.x + 1;
                                            spellTargetY = player1.y;
                                        }
                                    }
                                }
                                if (spellTypeBeingCast == (int)SC.SpellCasting.TARGET)
                                {
                                    if (delay_current_spell_targeting == DELAY_SPELL_TARGETING)
                                    {
                                        bool moved = false;
                                        if (controller.UpLeftDiagonalHeld() || controller.RightStickUpLeftDiagonalHeld())
                                        {
                                            if (spellTargetY > 0)
                                            {
                                                spellTargetY -= 1;
                                                delay_current_spell_targeting = 0;
                                                moved = true;
                                            }
                                            if (spellTargetX > 0)
                                            {
                                                spellTargetX -= 1;
                                                delay_current_spell_targeting = 0;
                                                moved = true;
                                            }
                                        }
                                        if ((controller.UpRightDiagonalHeld() || controller.RightStickUpRightDiagonalHeld()) && !moved)
                                        {
                                            if (spellTargetY > 0)
                                            {
                                                spellTargetY -= 1;
                                                delay_current_spell_targeting = 0;
                                                moved = true;
                                            }
                                            if (spellTargetX < currentLevel.DIMENSION - 1)
                                            {
                                                spellTargetX += 1;
                                                delay_current_spell_targeting = 0;
                                                moved = true;
                                            }
                                        }
                                        if ((controller.DownLeftDiagonalHeld() || controller.RightStickDownLeftDiagonalHeld()) && !moved)
                                        {
                                            if (spellTargetY < currentLevel.DIMENSION - 1)
                                            {
                                                spellTargetY += 1;
                                                delay_current_spell_targeting = 0;
                                                moved = true;
                                            }
                                            if (spellTargetX > 0)
                                            {
                                                spellTargetX -= 1;
                                                delay_current_spell_targeting = 0;
                                                moved = true;
                                            }
                                        }
                                        if ((controller.DownRightDiagonalHeld() || controller.RightStickDownRightDiagonalHeld()) && !moved)
                                        {
                                            if (spellTargetY < currentLevel.DIMENSION - 1)
                                            {
                                                spellTargetY += 1;
                                                delay_current_spell_targeting = 0;
                                                moved = true;
                                            }
                                            if (spellTargetX < currentLevel.DIMENSION - 1)
                                            {
                                                spellTargetX += 1;
                                                delay_current_spell_targeting = 0;
                                                moved = true;
                                            }
                                        }
                                        if ((controller.DownIsHeld() || controller.RightStickDownHeld()) && !moved)
                                        {
                                            if (spellTargetY < currentLevel.DIMENSION - 1)
                                            {
                                                spellTargetY += 1;
                                                delay_current_spell_targeting = 0;
                                                moved = true;
                                            }
                                        }
                                        if ((controller.UpIsHeld() || controller.RightStickUpHeld()) && !moved)
                                        {
                                            if (spellTargetY > 0)
                                            {
                                                spellTargetY -= 1;
                                                delay_current_spell_targeting = 0;
                                                moved = true;
                                            }
                                        }
                                        if ((controller.LeftIsHeld() || controller.RightStickLeftHeld()) && !moved)
                                        {
                                            if (spellTargetX > 0)
                                            {
                                                spellTargetX -= 1;
                                                delay_current_spell_targeting = 0;
                                                moved = true;
                                            }
                                        }
                                        if ((controller.RightIsHeld() || controller.RightStickRightHeld()) && !moved)
                                        {
                                            if (spellTargetX < currentLevel.DIMENSION - 1)
                                            {
                                                spellTargetX += 1;
                                                delay_current_spell_targeting = 0;
                                                moved = true;
                                            }
                                        }
                                    }
                                }
                                if (controller.AIsPressed() || controller.RightTriggerIsPulled())
                                {
                                    isCastingSpell = false;
                                    hasMadeMove = CastSpell(spellIndexToCast, spellTargetX, spellTargetY);
                                    if (hasMadeMove) runStats.IncrementStat("Spells Cast");
                                }
                            }
                        }
                        else
                        {
                            if (contextDialogOpen && !contextDialogSubMenuOpen)
                            {
                                if (controller.BIsPressed())
                                {
                                    contextDialogOpen = false;
                                    dialogOpen = false;
                                    sfxMenuCancel.Play();
                                }
                                else
                                {
                                    if (controller.UpIsPressed() && currentContextMenuSelection > 0)
                                    {
                                        currentContextMenuSelection--;
                                        sfxMenuSelect.Play();
                                    }
                                    if (controller.DownIsPressed() && currentContextMenuSelection < contextMenuStringList.Count - 1)
                                    {
                                        currentContextMenuSelection++;
                                        sfxMenuSelect.Play();
                                    }
                                    if (controller.AIsPressed() || controller.RightTriggerIsPulled())
                                    {
                                        if (contextMenuStringList != null)
                                        {
                                            if (contextMenuStringList.Count > 0)
                                            {
                                                contextDialogOpen = false;
                                                dialogOpen = false;
                                                controller.Update(); //update controller so trigger pull is not re-used
                                                hasMadeMove = PlayerAction(contextMenuStringList[currentContextMenuSelection], playerLookAtX, playerLookAtY);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (contextDialogSubMenuOpen)
                                {
                                    if (controller.BIsPressed())
                                    {
                                        contextDialogSubMenuOpen = false;
                                        currentContextSubMenuSelection = 0;
                                        sfxMenuCancel.Play();
                                    }
                                    if (controller.DownIsPressed())
                                    {
                                        if (currentContextSubMenuSelection < currentLevel.GetItems(playerLookAtX, playerLookAtY).Count - 1)
                                        {
                                            currentContextSubMenuSelection++;
                                            sfxMenuSelect.Play();
                                        }
                                    }
                                    if (controller.UpIsPressed())
                                    {
                                        if (currentContextSubMenuSelection > 0)
                                        {
                                            currentContextSubMenuSelection--;
                                            sfxMenuSelect.Play();
                                        }
                                    }
                                    if (controller.AIsPressed() || controller.RightTriggerIsPulled())
                                    {
                                        contextDialogSubMenuOpen = false;
                                        contextDialogOpen = false;
                                        dialogOpen = false;
                                        hasMadeMove = PickUp(currentContextSubMenuSelection, playerLookAtX, playerLookAtY);
                                        currentContextSubMenuSelection = 0;
                                    }
                                }
                            }

                            if (mapOpen)
                            {
                                if (controller.BIsPressed() || controller.StartIsPressed() || controller.BackIsPressed())
                                {
                                    mapOpen = false;
                                    mapOpenBackNeedsReleased = true;
                                    dialogOpen = false;
                                    sfxMenuCancel.Play();
                                }
                            }

                            if (inventoryOpen)
                            {
                                List<Item> items = inventoryTabs[currentInventoryTab].items;
                                if (items.Count > 0)
                                {
                                    if (controller.MenuDownIsPressed())
                                    {
                                        if (currentInventorySelection < items.Count - 1)
                                        {
                                            currentInventorySelection++;
                                            sfxMenuSelect.Play();
                                            if (currentInventorySelection > inventoryOffset + inventoryItemsPerPage - 1)
                                            {
                                                inventoryOffset = currentInventorySelection - inventoryItemsPerPage + 1;
                                            }
                                        }
                                    }
                                    if (controller.MenuUpIsPressed())
                                    {
                                        if (currentInventorySelection > 0)
                                        {
                                            currentInventorySelection--;
                                            sfxMenuSelect.Play();
                                            if (currentInventorySelection < inventoryOffset)
                                            {
                                                inventoryOffset = currentInventorySelection;
                                            }
                                        }
                                    }

                                    if (items.Count > inventoryItemsPerPage)
                                    {
                                        if (controller.LeftTriggerIsPulled())
                                        {
                                            if (currentInventorySelection > inventoryItemsPerPage)
                                            {
                                                currentInventorySelection -= inventoryItemsPerPage;
                                                inventoryOffset = currentInventorySelection;
                                            }
                                            else
                                            {
                                                currentInventorySelection = 0;
                                                inventoryOffset = 0;
                                            }
                                        }

                                        if (controller.RightTriggerIsPulled())
                                        {
                                            if (currentInventorySelection + inventoryItemsPerPage < items.Count)
                                            {
                                                currentInventorySelection += inventoryItemsPerPage;
                                                inventoryOffset += inventoryItemsPerPage;
                                                if (inventoryOffset > items.Count - inventoryItemsPerPage) inventoryOffset = items.Count - inventoryItemsPerPage;
                                            }
                                            else
                                            {
                                                currentInventorySelection = items.Count - 1;
                                                inventoryOffset = items.Count - inventoryItemsPerPage;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (controller.LeftTriggerIsPulled())
                                        {
                                            currentInventorySelection = 0;
                                        }
                                        if (controller.RightTriggerIsPulled())
                                        {
                                            currentInventorySelection = items.Count - 1;
                                        }
                                    }

                                    if (controller.XIsPressed() && items[currentInventorySelection].itemType != (int)SC.ItemTypes.QUEST)
                                    {
                                        if (inVillage && items[currentInventorySelection].loadOut)
                                        {
                                            AddMessage("Items from your starting inventory cannot be dropped in the village.", true);
                                            sfxSelectBad.Play();
                                        }
                                        else
                                        {
                                            if (currentLevel.DropItem(items[currentInventorySelection], player1.x, player1.y))
                                            {
                                                AddMessage("You drop the " + items[currentInventorySelection].itemName);
                                                RemoveItemFromLootList(items[currentInventorySelection]);
                                                player1.DropItem(items[currentInventorySelection]);
                                                UpdateInventoryTabs();
                                                if (currentInventorySelection > inventoryOffset) currentInventorySelection -= 1;
                                                if (inventoryOffset > 0) inventoryOffset--;
                                            }
                                            else
                                            {
                                                AddMessage("No space to drop that here!", true);
                                                sfxSelectBad.Play();
                                            }
                                        }
                                    }
                                    if (controller.AIsPressed())
                                    {
                                        switch (items[currentInventorySelection].itemType)
                                        {
                                            case (int)SC.ItemTypes.ARMOUR:
                                                player1.EquipItem(items[currentInventorySelection]);
                                                break;
                                            case (int)SC.ItemTypes.SHIELD:
                                                player1.EquipItem(items[currentInventorySelection]);
                                                break;
                                            case (int)SC.ItemTypes.WEAPON:
                                                if (player1.CanEquip(items[currentInventorySelection]))
                                                {
                                                    player1.EquipItem(items[currentInventorySelection]);
                                                }
                                                else
                                                {
                                                    AddMessage("Your Character Can't Wield That!", true);
                                                    sfxSelectBad.Play();
                                                }
                                                break;
                                            case (int)SC.ItemTypes.CLOAK:
                                                player1.EquipItem(items[currentInventorySelection]);
                                                break;
                                            case (int)SC.ItemTypes.SCROLL:
                                                if (UseScroll(items[currentInventorySelection]))
                                                {
                                                    player1.DropItem(items[currentInventorySelection]);
                                                    UpdateInventoryTabs();
                                                    if (currentInventorySelection > 0) currentInventorySelection -= 1;
                                                    if (inventoryOffset > 0) inventoryOffset--;
                                                    AddMessage("Its Magic Consumed, The Scroll Disappears!");
                                                    runStats.IncrementStat("Scrolls Read");
                                                }
                                                break;
                                            case (int)SC.ItemTypes.ROCK:
                                                if (items[currentInventorySelection].rockType == (int)SC.RockTypes.GRINDSTONE)
                                                {
                                                    UseGrindStone(items[currentInventorySelection]);
                                                    player1.DropItem(items[currentInventorySelection]);
                                                    UpdateInventoryTabs();
                                                    if (currentInventorySelection > 0) currentInventorySelection -= 1;
                                                    if (inventoryOffset > 0) inventoryOffset--;
                                                    AddMessage("You Have Improved Your Weapon!");
                                                }
                                                break;
                                            case (int)SC.ItemTypes.FOOD:
                                                if (EatFood(items[currentInventorySelection]))
                                                {
                                                    player1.DropItem(items[currentInventorySelection]);
                                                    //items = inventoryTabs[currentInventoryTab].items;
                                                    UpdateInventoryTabs();
                                                    if (currentInventorySelection > 0) currentInventorySelection -= 1;
                                                    if (inventoryOffset > 0) inventoryOffset--;
                                                }
                                                break;
                                            case (int)SC.ItemTypes.POTION:
                                                if (DrinkPotion(items[currentInventorySelection]))
                                                {
                                                    player1.DropItem(items[currentInventorySelection]);
                                                    UpdateInventoryTabs();
                                                    if (currentInventorySelection > 0) currentInventorySelection -= 1;
                                                    if (inventoryOffset > 0) inventoryOffset--;
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                        UpdateInventoryTabs();
                                        if (currentInventorySelection >= items.Count) currentInventorySelection = items.Count - 1;
                                    }
                                    if (controller.YIsPressed() && items[currentInventorySelection].itemType != (int)SC.ItemTypes.QUEST)
                                    {
                                        items[currentInventorySelection].forSale = !items[currentInventorySelection].forSale;
                                        UpdateInventoryTabs();
                                    }
                                }
                                if (controller.LeftShoulderIsPressed())
                                {
                                    if (currentInventoryTab > 0)
                                    {
                                        currentInventoryTab--;
                                    }
                                    else
                                    {
                                        currentInventoryTab = inventoryTabs.Count - 1;
                                    }
                                    currentInventorySelection = 0;
                                    inventoryOffset = 0;
                                    UpdateInventoryTabs();
                                }
                                if (controller.RightShoulderIsPressed())
                                {
                                    if (currentInventoryTab == inventoryTabs.Count - 1)
                                    {
                                        currentInventoryTab = 0;
                                    }
                                    else
                                    {
                                        currentInventoryTab++;
                                    }
                                    currentInventorySelection = 0;
                                    inventoryOffset = 0;
                                    UpdateInventoryTabs();
                                }
                                if (controller.BIsPressed() || controller.StartIsPressed())
                                {
                                    inventoryOpen = false;
                                    dialogOpen = false;
                                    sfxMenuCancel.Play();
                                }
                            }

                            if (spellListOpen)
                            {
                                if (controller.BIsPressed() || controller.StartIsPressed())
                                {
                                    spellListOpen = false;
                                    dialogOpen = false;
                                    sfxMenuCancel.Play();
                                }

                                if (controller.DownIsPressed())
                                {
                                    if (currentSpellListSelection < player1.spells.GetKnownSpells().Count - 1)
                                    {
                                        currentSpellListSelection++;
                                        sfxMenuSelect.Play();
                                    }
                                }
                                if (controller.UpIsPressed())
                                {
                                    if (currentSpellListSelection > 0)
                                    {
                                        currentSpellListSelection--;
                                        sfxMenuSelect.Play();
                                    }
                                }

                                if (controller.AIsPressed() || controller.RightTriggerIsPulled())
                                {
                                    if (player1.spells.GetKnownSpells()[currentSpellListSelection].castingType == (int)SC.SpellCasting.SELF)
                                    {
                                        hasMadeMove = CastSpell(currentSpellListSelection, player1.x, player1.y);
                                        dialogOpen = false;
                                        spellListOpen = false;
                                        currentSpellListSelection = 0;
                                    }
                                    else
                                    {
                                        isCastingSpell = true;
                                        spellTypeBeingCast = player1.spells.GetKnownSpells()[currentSpellListSelection].castingType;
                                        spellIndexToCast = currentSpellListSelection;
                                        spellTargetX = player1.x - 1;
                                        spellTargetY = player1.y;
                                        dialogOpen = false;
                                        spellListOpen = false;
                                        playerLooking = false;
                                        currentSpellListSelection = 0;
                                    }
                                }

                            }

                            if (questListOpen)
                            {
                                if (controller.BIsPressed())
                                {
                                    questListOpen = false;
                                    dialogOpen = false;
                                    sfxMenuCancel.Play();
                                }

                                if (controller.MenuDownIsPressed())
                                {
                                    if (currentQuestListSelection < currentQuestList.Count - 1)
                                    {
                                        currentQuestListSelection++;
                                        sfxMenuSelect.Play();
                                    }
                                }
                                if (controller.MenuUpIsPressed())
                                {
                                    if (currentQuestListSelection > 0)
                                    {
                                        currentQuestListSelection--;
                                        sfxMenuSelect.Play();
                                    }
                                }
                                if (controller.AIsPressed())
                                {
                                    AcceptQuest(currentQuestList[currentQuestListSelection]);
                                    sfxMenuConfirm.Play();
                                    questListOpen = false;
                                    questForfeitOpen = true;
                                    controller.Update(); //reset the last button state, else the game will read input for the quest dialog
                                }
                            }

                            if (questForfeitOpen)
                            {
                                if (controller.BIsPressed())
                                {
                                    questForfeitOpen = false;
                                    dialogOpen = false;
                                    sfxMenuCancel.Play();
                                }
                                if (controller.YIsPressed())
                                {
                                    questForfeitOpen = false;
                                    ForfeitQuest();
                                    runStats.IncrementStat("Quests Forfeited");
                                    questListOpen = true;
                                    sfxMenuCancel.Play();
                                }
                            }
                            if (questTurnInOpen)
                            {
                                if (controller.AIsPressed())
                                {
                                    TurnInQuest();
                                    sfxFanfare.Play();
                                    questTurnInOpen = false;
                                    questListOpen = true;
                                }
                            }
                            if (questFailedOpen)
                            {
                                if (controller.BIsPressed())
                                {
                                    questFailedOpen = false;
                                    dialogOpen = false;
                                    sfxMenuCancel.Play();
                                }
                                if (controller.YIsPressed())
                                {
                                    questFailedOpen = false;
                                    ForfeitQuest();
                                    runStats.IncrementStat("Quests Failed");
                                    questListOpen = true;
                                    sfxMenuCancel.Play();
                                }
                            }

                            if (helpOpen)
                            {
                                if (controller.RightShoulderIsPressed())
                                {
                                    if (currentHelpPage < NUM_HELP_PAGES)
                                    {
                                        currentHelpPage++;
                                        sfxMenuSelect.Play();
                                    }
                                }
                                if (controller.LeftShoulderIsPressed())
                                {
                                    if (currentHelpPage > 1)
                                    {
                                        currentHelpPage--;
                                        sfxMenuSelect.Play();
                                    }
                                }
                                if (controller.BIsPressed())
                                {
                                    helpOpen = false;
                                    dialogOpen = false;
                                    sfxMenuCancel.Play();
                                }
                            }

                            if (startMenuOpen)
                            {
                                if (controller.MenuDownIsPressed())
                                {
                                    if (currentStartMenuSelection < startMenuStringList.Count - 1)
                                    {
                                        currentStartMenuSelection++;
                                        sfxMenuSelect.Play();
                                    }
                                    else
                                    {
                                        currentStartMenuSelection = 0;
                                        sfxMenuSelect.Play();
                                    }
                                }
                                if (controller.MenuUpIsPressed())
                                {
                                    if (currentStartMenuSelection > 0)
                                    {
                                        currentStartMenuSelection--;
                                        sfxMenuSelect.Play();
                                    }
                                    else
                                    {
                                        currentStartMenuSelection = startMenuStringList.Count - 1;
                                        sfxMenuSelect.Play();
                                    }
                                }
                                if (controller.BIsPressed() || controller.StartIsPressed())
                                {
                                    startMenuOpen = false;
                                    dialogOpen = false;
                                    sfxMenuCancel.Play();
                                }
                                if (controller.AIsPressed())
                                {
                                    if (startMenuStringList[currentStartMenuSelection] == "Options")
                                    {
                                        startMenuOpen = false;
                                        optionsMenuOpen = true;
                                        currentOptionMenuSelection = 0;
                                        sfxMenuConfirm.Play();
                                    }
                                    if (startMenuStringList[currentStartMenuSelection] == "Inventory")
                                    {
                                        startMenuOpen = false;
                                        inventoryOpen = true;
                                        UpdateInventoryTabs();
                                        currentInventorySelection = 0;
                                        inventoryOffset = 0;
                                        sfxMenuConfirm.Play();
                                    }
                                    if (startMenuStringList[currentStartMenuSelection] == "Character")
                                    {
                                        startMenuOpen = false;
                                        characterSheetOpen = true;
                                        sfxMenuConfirm.Play();
                                    }
                                    if (startMenuStringList[currentStartMenuSelection] == "Spells")
                                    {
                                        if (player1.spells.GetKnownSpells().Count > 0)
                                        {
                                            if (!inVillage)
                                            {
                                                spellListOpen = true;
                                                startMenuOpen = false;
                                                currentSpellListSelection = 0;
                                                sfxMenuConfirm.Play();
                                            }
                                            else
                                            {
                                                sfxSelectBad.Play();
                                                AddMessage("Casting spells in the village is forbidden!", true);
                                            }
                                        }
                                    }
                                    if (startMenuStringList[currentStartMenuSelection] == "Stats")
                                    {
                                        runStatsOpen = true;
                                        startMenuOpen = false;
                                        currentStatOffset = 0;
                                        sfxMenuConfirm.Play();
                                    }
                                    if (startMenuStringList[currentStartMenuSelection] == "Help")
                                    {
                                        startMenuOpen = false;
                                        helpOpen = true;
                                        currentHelpPage = 1;
                                        controller.Update();
                                        sfxMenuConfirm.Play();
                                    }
                                    if (startMenuStringList[currentStartMenuSelection] == "Map")
                                    {
                                        sfxPaper.Play();
                                        mapOpen = true;
                                        mapTiles = currentLevel.KnownMap();
                                        startMenuOpen = false;
                                        //sfxMenuConfirm.Play();
                                    }
                                    if (startMenuStringList[currentStartMenuSelection] == "Suspend")
                                    {
                                        if (!cheating && !isTrialMode)
                                        {
                                            gameSuspendConfirm = true;
                                            startMenuOpen = false;
                                            controller.Update();
                                            sfxMenuConfirm.Play();
                                        }
                                        else
                                        {
                                            sfxSelectBad.Play();
                                            if (cheating)
                                            {
                                                AddMessage("Sorry, You Can't Suspend Game While Cheating", true);
                                            }
                                            else
                                            {
                                                AddMessage("Suspend Your Game At Any Time - Not Available In This Trial", true);
                                            }
                                        }
                                    }
                                    if (startMenuStringList[currentStartMenuSelection] == "End Game")
                                    {
                                        startMenuOpen = false;
                                        gameQuitConfirmOpen = true;
                                        controller.Update();
                                        sfxMenuConfirm.Play();
                                    }
                                }
                            }

                            if (characterSheetOpen)
                            {
                                if (controller.BIsPressed() || controller.StartIsPressed())
                                {
                                    characterSheetOpen = false;
                                    dialogOpen = false;
                                    sfxMenuCancel.Play();
                                }
                            }

                            if (runStatsOpen)
                            {
                                if (controller.BIsPressed() || controller.StartIsPressed())
                                {
                                    runStatsOpen = false;
                                    dialogOpen = false;
                                    sfxMenuCancel.Play();
                                }
                                if (runStats.statList.Count > statsOnScreen)
                                {
                                    if (controller.MenuDownIsPressed())
                                    {
                                        if (currentStatOffset + statsOnScreen > 0)
                                        {
                                            currentStatOffset++;
                                            sfxMenuSelect.Play();
                                        }
                                    }
                                    if (controller.MenuUpIsPressed())
                                    {
                                        if (currentStatOffset > 0)
                                        {
                                            currentStatOffset--;
                                            sfxMenuSelect.Play();
                                        }
                                    }
                                }
                            }

                            if (optionsMenuOpen)
                            {
                                if (controller.BIsPressed() || controller.StartIsPressed())
                                {
                                    optionsMenuOpen = false;
                                    dialogOpen = false;
                                    sfxMenuCancel.Play();
                                }
                                if (controller.MenuDownIsPressed())
                                {
                                    if (currentOptionMenuSelection < MAX_OPTION_MENU_ITEMS)
                                    {
                                        currentOptionMenuSelection++;
                                        sfxMenuSelect.Play();
                                    }
                                }
                                if (controller.MenuUpIsPressed())
                                {
                                    if (currentOptionMenuSelection > 0)
                                    {
                                        currentOptionMenuSelection--;
                                        sfxMenuSelect.Play();
                                    }
                                }
                                //change tile size
                                if (currentOptionMenuSelection == 0)
                                {
                                    if (controller.MenuLeftIsPressed())
                                    {
                                        if (TILE_SIZE == 32) SetTileSize(64);
                                        if (TILE_SIZE == 16) SetTileSize(32);
                                        sfxMenuSelect.Play();
                                    }
                                    if (controller.MenuRightIsPressed())
                                    {
                                        if (TILE_SIZE == 32) SetTileSize(16);
                                        if (TILE_SIZE == 64) SetTileSize(32);
                                        sfxMenuSelect.Play();
                                    }
                                }
                                //Change display size
                                if (currentOptionMenuSelection == 1)
                                {
                                    if (controller.MenuRightIsPressed()
                                        && !displayFullScreen)
                                    {
                                        displayFullScreen = true;
                                        SetScreenProjection();
                                        sfxMenuSelect.Play();
                                    }
                                    if (controller.MenuLeftIsPressed()
                                        && displayFullScreen)
                                    {
                                        displayFullScreen = false;
                                        SetScreenProjection();
                                        sfxMenuSelect.Play();
                                    }
                                }
                                //Change controller input
                                if (currentOptionMenuSelection == 2)
                                {
                                    if (controller.MenuRightIsPressed() && !controller.MovingWithDPad())
                                    {
                                        controller.SetMovingWithDPad(true);
                                        sfxMenuSelect.Play();
                                    }
                                    if (controller.MenuLeftIsPressed() && controller.MovingWithDPad())
                                    {
                                        controller.SetMovingWithDPad(false);
                                        sfxMenuSelect.Play();
                                    }
                                }

                                //Turn BGM On or Off
                                if (currentOptionMenuSelection == 3)
                                {
                                    if (controller.MenuLeftIsPressed() && bgmPlayOption == SC.BGMPlayOption.OFF)
                                    {
                                        bgmPlayOption = SC.BGMPlayOption.ON;
                                        sfxMenuSelect.Play();
                                        if (inVillage)
                                        {
                                            PlayBGM(villageMusic);
                                        }
                                        else
                                        {
                                            PlayBGM(dungeonMusic);
                                        }
                                    }
                                    if (controller.MenuRightIsPressed() && bgmPlayOption == SC.BGMPlayOption.ON)
                                    {
                                        bgmPlayOption = SC.BGMPlayOption.OFF;
                                        sfxMenuSelect.Play();
                                        MediaPlayer.Stop();
                                    }
                                }

                                //Change BGM shuffle
                                if (currentOptionMenuSelection == 4)
                                {
                                    if (controller.MenuLeftIsPressed() && bgmShuffleOption == SC.BGMShuffleOption.ONCE_PER_FLOOR)
                                    {
                                        bgmShuffleOption = SC.BGMShuffleOption.ONCE_PER_QUEST;
                                        sfxMenuSelect.Play();
                                    }
                                    if (controller.MenuRightIsPressed() && bgmShuffleOption == SC.BGMShuffleOption.ONCE_PER_QUEST)
                                    {
                                        bgmShuffleOption = SC.BGMShuffleOption.ONCE_PER_FLOOR;
                                        sfxMenuSelect.Play();
                                    }
                                }

                                //Change tutorial message display option
                                if (currentOptionMenuSelection == 5)
                                {
                                    if (controller.MenuRightIsPressed() && !neverSeeTutorialMessages)
                                    {
                                        neverSeeTutorialMessages = true;
                                        sfxMenuSelect.Play();
                                    }
                                    if (controller.MenuLeftIsPressed() && neverSeeTutorialMessages)
                                    {
                                        neverSeeTutorialMessages = false;
                                        sfxMenuSelect.Play();
                                    }
                                }

                                //Reset all tutorial messages
                                if (currentOptionMenuSelection == 6 && controller.AIsPressed())
                                {
                                    ShowAllTutorialMessagesAgain();
                                    AddMessage("In Your Next Game, All Tutorial Messages Will Appear Again!");
                                    sfxMenuSelect.Play();
                                }
                            }
                            if (shopOpen)
                            {
                                if (shopConfirmOpen)
                                {
                                    if (controller.BIsPressed())
                                    {
                                        shopConfirmOpen = false;
                                        sfxMenuCancel.Play();
                                    }
                                    if (controller.AIsPressed())
                                    {
                                        BuyItem(currentShopSelection);
                                        shopConfirmOpen = false;
                                        currentShopSelection = 0;
                                        sfxCoins.Play();
                                    }
                                }
                                else
                                {
                                    List<Item> theItems;
                                    if (inVillage)
                                    {
                                        theItems = villageMerchantItems;
                                    }
                                    else
                                    {
                                        theItems = dungeonMerchantItems;
                                    }
                                    if (controller.BIsPressed())
                                    {
                                        shopOpen = false;
                                        dialogOpen = false;
                                        sfxMenuCancel.Play();
                                    }
                                    if (controller.AIsPressed())
                                    {

                                        if (theItems.Count > 0 && CanBuy(theItems[currentShopSelection]))
                                        {
                                            shopConfirmOpen = true;
                                            sfxMenuSelect.Play();
                                        }
                                        else
                                        {
                                            lastShopMessage = "Sorry, You Can't Afford That!";
                                            AddMessage(lastShopMessage, true);
                                            sfxSelectBad.Play();
                                        }
                                    }
                                    if (controller.MenuDownIsPressed())
                                    {
                                        if (currentShopSelection < theItems.Count - 1)
                                        {
                                            currentShopSelection++;
                                            lastShopMessage = "";
                                            sfxMenuSelect.Play();
                                        }
                                    }
                                    if (controller.MenuUpIsPressed())
                                    {
                                        if (currentShopSelection > 0)
                                        {
                                            currentShopSelection--;
                                            lastShopMessage = "";
                                            sfxMenuSelect.Play();
                                        }
                                    }
                                }
                            }
                            if (gameQuitConfirmOpen)
                            {
                                if (controller.YIsPressed())
                                {
                                    gameQuitConfirmOpen = false;
                                    dialogOpen = false;
                                    playerQuit = true;
                                    GameOver();
                                }
                                if (controller.BIsPressed())
                                {
                                    gameQuitConfirmOpen = false;
                                    dialogOpen = false;
                                }
                            }
                            if (gameSuspendConfirm)
                            {
                                if (controller.YIsPressed())
                                {
                                    gameSuspendConfirm = false;
                                    dialogOpen = false;
                                    EndGame();
                                    //Need to save settings here; to save the updated time stat
                                    //Put SaveSettings insode SaveResumableBody
                                    SaveResumableGame();
                                }
                                if (controller.BIsPressed())
                                {
                                    gameSuspendConfirm = false;
                                    dialogOpen = false;
                                }
                            }
                        }
                    }
                }
                if (hasMadeMove)
                {
                    UpdateGame();
                    if (player1.alive) player1.Update(inVillage);
                    if (player1.poisoned)
                    {
                        particleClusters.Add(new ParticleCluster(20,
                                                        halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 4, 1, 5, txEffectGas, 5));
                    }
                    if (player1.profession == (int)SC.Professions.EXPLORER) currentLevel.KnowMerchant();
                    if (!inVillage) runStats.IncrementStat("Moves");
                    UpdateFeats();

                    if (player1.hunger / player1.full_stomach < 0.25 && lastHunger / player1.full_stomach >= 0.25)
                    {
                        AddMessage("You're Feeling A Bit Hungry...", true);
                        StopFastMovement();
                    }
                    else
                    {
                        if (player1.hunger / player1.full_stomach < 0.1 && lastHunger / player1.full_stomach >= 0.1)
                        {
                            AddMessage("You're Feeling Very Hungry. You Need To Eat Soon.", true);
                            StopFastMovement();
                        }
                        else
                        {
                            if (player1.hunger / player1.full_stomach < 0.03 && lastHunger / player1.full_stomach >= 0.03)
                            {
                                AddMessage("You're Almost Starving. You Will Die If You Don't Eat.", true);
                                StopFastMovement();
                            }
                        }
                    }
                    if (player1.starving && lastHunger > 0)
                    {
                        AddMessage("You Are Now Starving.", true);
                        sfxDanger.Play();
                    }
                    lastHunger = player1.hunger;
                    //Catch if player levelled up
                    if (player1.level > lastLevel)
                    {
                        particleClusters.Add(new ParticleCluster(80,
                                                        GAMEFIELDWIDTH / 2 - 180,
                                                        10,
                                                        TILE_SIZE / 16, 5, 8, txEffectLevelUp, 15));
                        sfxLevelUp.Play();
                        ShowTutorialMessage(12);
                        if (player1.profession == (int)SC.Professions.FIGHTER)
                        {
                            if (LevelUpAlliedCreatures())
                            {
                                AddMessage("Your Allies Have Become Stronger");
                            }
                        }
                        if (spellListAtLastTurn != null)
                        {
                            if (SpellIsNewlyKnown("Heal", spellListAtLastTurn, player1.spells.GetKnownSpells())) ShowTutorialMessage(17);
                            if (SpellIsNewlyKnown("Freeze", spellListAtLastTurn, player1.spells.GetKnownSpells())) ShowTutorialMessage(18);
                            if (SpellIsNewlyKnown("Magic Missile", spellListAtLastTurn, player1.spells.GetKnownSpells())) ShowTutorialMessage(19);
                        }
                        lastLevel = player1.level;
                        spellListAtLastTurn = player1.spells.GetKnownSpells();
                    }
                }
                if (UpdateQuestFailed())
                {
                    sfxFail.Play();
                    currentQuest.failed = true;
                }
                if (isLevelComplete)
                {
                    if (!QuestListContainsBoss())
                        currentLevelNumber++;
                    PullDescendingEnemies();
                    currentLevel = new Level(currentLevel.levelNumber + 1, currentQuest, itemGenerator, bestiary);
                    dungeonLevel = currentLevel;
                    AddMessage("You Descend To Floor " + ((currentLevel.levelNumber - currentQuest.level) + 1).ToString() + "...");
                    ShowTutorialMessage(8);
                    player1.x = currentLevel.startingX;
                    player1.y = currentLevel.startingY;
                    PushDescendingEnemies();
                    if ((player1.profession != (int)SC.Professions.EXPLORER && currentLevelNumber % levelsPerMerchant == 0)
                        || (player1.profession == (int)SC.Professions.EXPLORER && currentLevelNumber % (levelsPerMerchant - 1) == 0))
                    {
                        if (!currentLevel.isBossLevel)
                        {
                            if (currentLevel.PlaceMerchant())
                            {
                                AddMessage("There Is A Merchant Trading Somewhere On This Floor");
                                dungeonMerchantItems = itemGenerator.DungeonMerchantInventory(currentLevelNumber, itemsPerMerchant);
                                ShowTutorialMessage(5);
                            }
                        }
                    }
                    if (inVillage)
                    {
                        dungeonMerchantItems = itemGenerator.MerchantInventory(currentLevelNumber, itemsPerMerchant);
                    }
                    if (!inVillage && player1.profession == (int)SC.Professions.EXPLORER)
                    {
                        currentLevel.KnowExit();
                        currentLevel.KnowMerchant();
                    }
                    UpdateVision();
                    if (bgmShuffleOption == SC.BGMShuffleOption.ONCE_PER_FLOOR)
                    {
                        ShuffleDungeonBGM();
                        PlayBGM(dungeonMusic);
                    }

                    isLevelComplete = false;
                }
            }
        }

        private void UpdateVision()
        {
            currentLevel.UpdateVision(player1.x, player1.y, 0);
            //UpdateShadowTexture();
        }

        private void UpdateInventoryTabs()
        {
            if (inventoryTabs.Count > 0)
            {
                for (int i = 0; i < inventoryTabs.Count; i++)
                {
                    inventoryTabs[i].UpdateList(player1.carriedItems);
                }
            }
        }

        private void UpdateShadowTexture()
        {
            //TODO - put this 'new' into the code that updates tile size
            shadowTexture = new Texture2D(GraphicsDevice, tilesInViewX * TILE_SIZE, tilesInViewY * TILE_SIZE);
            int[,] visibility = currentLevel.GetTileVision(player1.x - halfTilesInViewX,
                                                            player1.y - halfTilesInViewY,
                                                            tilesInViewX, tilesInViewY);

            Color[] shadeFill = new Color[tilesInViewX * TILE_SIZE * tilesInViewY * TILE_SIZE];
            for (int i = 0; i < visibility.GetLength(0); i++)
            {
                for (int j = 0; j < visibility.GetLength(1); j++)
                {
                    Color shade = new Color(0, 0, 0, (byte)255 - visibility[i, j]);
                    for (int k = i * TILE_SIZE; k < i * TILE_SIZE + TILE_SIZE; k++)
                    {
                        for (int l = j * TILE_SIZE; l < j * TILE_SIZE + TILE_SIZE; l++)
                        {
                            shadeFill[l * TILE_SIZE * tilesInViewX + k] = shade;
                        }
                    }
                }
            }
            shadowTexture.SetData<Color>(shadeFill);
        }

        private void UpdateCharacterCreation()
        {
            creationScreenBackgroundOffset++;
            if (creationScreenBackgroundOffset >= BACKGROUND_TILE_SIZE) creationScreenBackgroundOffset = 0;

            if (controller.MenuDownIsPressed())
            {
                if (!changingStartingLevel)
                {
                    if (currentCharacterCreationOption < (TOTAL_CHAR_CREATE_OPTIONS - 1))
                    {
                        currentCharacterCreationOption++;
                        sfxMenuSelect.Play();
                    }
                }
                else
                {
                    int diff = (int)Math.Pow(10, (double)currentLevelSelectDigit);
                    startingLevel = Math.Max(1, startingLevel - diff);
                    sfxStep.Play();
                }
            }

            if (controller.MenuUpIsPressed())
            {
                if (!changingStartingLevel)
                {
                    if (currentCharacterCreationOption > 0)
                    {
                        currentCharacterCreationOption--;
                        sfxMenuSelect.Play();
                    }
                }
                else
                {
                    int max = MAX_STARTING_QUEST_LEVEL;
                    if (cheating) max = MAX_LEVELS;
                    int diff = (int)Math.Pow(10, (double)currentLevelSelectDigit);
                    startingLevel = Math.Min(max, startingLevel + diff);
                    sfxStep.Play();
                }
            }

            if (controller.MenuLeftIsPressed())
            {
                if (currentCharacterCreationOption == CHAR_CREATE_OPTION_PROFESSION)
                {
                    if (player1.profession == (int)SC.Professions.MAGE)
                    {
                        player1.SetProfession((int)SC.Professions.FIGHTER);
                        sfxStep.Play();
                    }
                    if (player1.profession == (int)SC.Professions.EXPLORER)
                    {
                        player1.SetProfession((int)SC.Professions.MAGE);
                        sfxStep.Play();
                    }
                }
                if (currentCharacterCreationOption == CHAR_CREATE_OPTION_RACE)
                {
                    if (player1.race == (int)SC.Races.HUMAN)
                    {
                        player1.SetRace((int)SC.Races.ORC);
                        sfxStep.Play();
                    }
                    if (player1.race == (int)SC.Races.ELF)
                    {
                        player1.SetRace((int)SC.Races.HUMAN);
                        sfxStep.Play();
                    }
                }
                if (currentCharacterCreationOption == CHAR_CREATE_OPTION_TUTORIALMESSAGES)
                {
                    if (neverSeeTutorialMessages)
                    {
                        neverSeeTutorialMessages = false;
                        sfxStep.Play();
                    }
                }
            }

            if (controller.MenuRightIsPressed())
            {
                if (currentCharacterCreationOption == CHAR_CREATE_OPTION_PROFESSION)
                {
                    if (player1.profession == (int)SC.Professions.MAGE)
                    {
                        player1.SetProfession((int)SC.Professions.EXPLORER);
                        sfxStep.Play();
                    }
                    if (player1.profession == (int)SC.Professions.FIGHTER)
                    {
                        player1.SetProfession((int)SC.Professions.MAGE);
                        sfxStep.Play();
                    }
                }
                if (currentCharacterCreationOption == CHAR_CREATE_OPTION_RACE)
                {
                    if (player1.race == (int)SC.Races.HUMAN)
                    {
                        player1.SetRace((int)SC.Races.ELF);
                        sfxStep.Play();
                    }
                    if (player1.race == (int)SC.Races.ORC)
                    {
                        player1.SetRace((int)SC.Races.HUMAN);
                        sfxStep.Play();
                    }
                }
                if (currentCharacterCreationOption == CHAR_CREATE_OPTION_TUTORIALMESSAGES)
                {
                    if (!neverSeeTutorialMessages)
                    {
                        neverSeeTutorialMessages = true;
                        sfxStep.Play();
                    }
                }
            }

            if (controller.AIsPressed())
            {
                if (currentCharacterCreationOption == CHAR_CREATE_OPTION_NAME)
                {
                    Guide.BeginShowKeyboardInput(playerIndex, "Hero Name", "Enter Your Name, Hero!\nFuture generations will know of your great deeds!\n(But the history books will only remember the first 10 characters)", player1.name, NameEntryCallback, null);
                }
            }

            if (controller.StartIsPressed())
            {
                changingStartingLevel = false;
                currentGameState = GAMESTATE_PLAYING;
                if (!cheating)
                {
                    AddRawExperience(totalBonusXP);
                    changingStartingLevel = false;
                }
                else
                {
                    //AddRawExperience(2700000); // Level 500
                    //AddRawExperience(5099999); // Level 1009
                    //AddRawExperience(19999999); // Level 2000
                    //AddRawExperience(45299999); // Level 3000
                    //AddRawExperience(100500000);
                    //AddRawExperience(125000000); // Level 5000
                    //AddRawExperience(180000000); // Level 6000
                    //AddRawExperience(250000000); // Level 7071
                    //AddRawExperience(320000000); // Level 8000
                    AddRawExperience(405000000); // Level 9000
                    MAX_STARTING_QUEST_LEVEL = 9000;
                }
                NewGame();
                lastLevel = player1.level;
                lastHunger = player1.hunger;
            }

            if (controller.BIsPressed())
            {
                if (changingStartingLevel)
                {
                    changingStartingLevel = false;
                }
                else
                {
                    currentGameState = GAMESTATE_MAINMENU;
                    currentMainMenuSelection = 0;
                    MediaPlayer.Stop();
                }
            }
        }

        private void UpdateMainMenu()
        {
            creationScreenBackgroundOffset++;
            if (creationScreenBackgroundOffset >= BACKGROUND_TILE_SIZE) creationScreenBackgroundOffset = 0;

            if (delay_current_mainmenu_input < DELAY_MAINMENU_INPUT) delay_current_mainmenu_input++;

            if (!dialogOpen)
            {
                if (controller.MenuDownIsPressed())
                {
                    if (currentMainMenuSelection < mainMenuStringList.Count - 1)
                    {
                        currentMainMenuSelection++;
                        sfxMenuSelect.Play();
                        showingCannotBuyWarning = !CanPlayerBuyGame(playerIndex) && (mainMenuStringList[currentMainMenuSelection] == "Unlock Full Game");
                    }
                }
                if (controller.MenuUpIsPressed())
                {
                    if (currentMainMenuSelection > 0)
                    {
                        currentMainMenuSelection--;
                        sfxMenuSelect.Play();
                        showingCannotBuyWarning = !CanPlayerBuyGame(playerIndex) && (mainMenuStringList[currentMainMenuSelection] == "Unlock Full Game");
                    }
                }
                if (controller.AIsPressed()
                    && (settingsLoadComplete || isTrialMode))
                {
                    if (mainMenuStringList[currentMainMenuSelection] == "New Game")
                    {
                        if (resumableGameExists)
                        {
                            dialogOpen = true;
                            showingResumableExistsWarning = true;
                        }
                        else
                        {
                            MainMenuCharacterCreationTransition();
                        }
                        currentMainMenuSelection = 0;
                        sfxMenuConfirm.Play();
                    }
                    if (mainMenuStringList[currentMainMenuSelection] == "Resume")
                    {
                        if (resumableGameExists)
                        {
                            currentGameState = GAMESTATE_PLAYING;
                            resumableGameLoading = true;
                            new Thread(ResumeGame).Start();
                            currentMainMenuSelection = 0;
                        }
                    }
                    if (mainMenuStringList[currentMainMenuSelection] == "Exit To Dashboard")
                    {
                        showingExitConfirmation = true;
                        dialogOpen = true;
                    }
                    if (mainMenuStringList[currentMainMenuSelection] == "Stats")
                    {
                        dialogOpen = true;
                        totalStatsOpen = true;
                        sfxMenuConfirm.Play();
                    }
                    if (mainMenuStringList[currentMainMenuSelection] == "Feats")
                    {
                        dialogOpen = true;
                        featSummaryOpen = true;
                        sfxMenuConfirm.Play();
                    }
                    if (mainMenuStringList[currentMainMenuSelection] == "Help")
                    {
                        dialogOpen = true;
                        helpOpen = true;
                        currentHelpPage = 1;
                        sfxMenuConfirm.Play();
                    }
                    if (mainMenuStringList[currentMainMenuSelection] == "Credits")
                    {
                        dialogOpen = true;
                        creditsOpen = true;
                        sfxMenuConfirm.Play();
                    }
                    if (mainMenuStringList[currentMainMenuSelection] == "Unlock Full Game")
                    {
                        if (CanPlayerBuyGame(playerIndex))
                        {
                            Guide.ShowMarketplace(playerIndex);
                            currentMainMenuSelection = 0;
                            currentGameState = GAMESTATE_TITLESCREEN;
                        }
                        else
                        {
                            showingCannotBuyWarning = true;
                            sfxSelectBad.Play();
                        }
                    }
                }
                if (!cheating)
                {
                    if (controller.XIsPressed())
                    {
                        if (cheatCodeDigits[correctCheatCodeDigits] == 'X')
                        {
                            correctCheatCodeDigits++;
                        }
                        else
                        {
                            correctCheatCodeDigits = 0;
                        }
                    }
                    else
                    {
                        if (controller.YIsPressed())
                        {
                            if (cheatCodeDigits[correctCheatCodeDigits] == 'Y')
                            {
                                correctCheatCodeDigits++;
                            }
                            else
                            {
                                correctCheatCodeDigits = 0;
                            }
                        }
                    }
                    if (correctCheatCodeDigits == NUM_CHEATCODE_DIGITS)
                    {
                        if (System.DateTime.Today < DATE_CHEAT_EXPIRES && !isTrialMode)
                        {
                            cheating = true;
                            sfxRecruit.Play();
                        }
                        else
                        {
                            correctCheatCodeDigits = 0;
                        }
                    }
                }
            }
            else //Else, dialog is open
            {
                if (showingExitConfirmation)
                {
                    if (controller.YIsPressed())
                    {
                        dialogOpen = false;
                        showingExitConfirmation = false;
                        this.Exit();
                    }
                }

                if (showingResumableExistsWarning)
                {
                    if (controller.YIsPressed())
                    {
                        dialogOpen = false;
                        showingResumableExistsWarning = false;
                        DeleteResumableGame();
                        MainMenuCharacterCreationTransition();
                    }
                }

                if (helpOpen)
                {
                    if (controller.RightShoulderIsPressed())
                    {
                        if (currentHelpPage < NUM_HELP_PAGES)
                        {
                            currentHelpPage++;
                            sfxMenuSelect.Play();
                        }
                    }
                    if (controller.LeftShoulderIsPressed())
                    {
                        if (currentHelpPage > 1)
                        {
                            currentHelpPage--;
                            sfxMenuSelect.Play();
                        }
                    }
                }

                if (featSummaryOpen)
                {
                    if (controller.AIsPressed() || controller.StartIsPressed() || controller.BackIsPressed())
                    {
                        dialogOpen = false;
                        featSummaryOpen = false;
                        sfxMenuCancel.Play();
                    }
                }

                if (controller.BIsPressed())
                {
                    dialogOpen = false;
                    totalStatsOpen = false;
                    featSummaryOpen = false;
                    helpOpen = false;
                    creditsOpen = false;
                    showingExitConfirmation = false;
                    showingResumableExistsWarning = false;
                    sfxMenuCancel.Play();
                }
            }
        }

        private void MovePlayer(int direction, int deltaX, int deltaY)
        {
            if (currentLevel.CanMove(player1.x + deltaX, player1.y + deltaY))
            {
                player1.x += deltaX;
                player1.y += deltaY;
                player1.directionFacing = direction;
                hasMadeMove = true;
                if (IsFastMoving())
                {
                    sfxStepFiltered.Play();
                }
                else
                {
                    sfxStep.Play();
                }
            }
            else
            {
                Creature potentialEnemy = currentLevel.GetCreature(player1.x + deltaX, player1.y + deltaY);
                if (potentialEnemy != null)
                {
                    if (potentialEnemy.hostile)
                    {
                        Attack(player1.x + deltaX, player1.y + deltaY);
                        hasMadeMove = true;
                    }
                    else
                    {
                        if (potentialEnemy.allied)
                        {
                            hasMadeMove = SwitchPlaces(potentialEnemy);
                        }
                        else
                        {
                            AddMessage("The " + potentialEnemy.description.description + " Is Friendly!");
                        }
                    }
                }
            }
        }

        private void MainMenuCharacterCreationTransition()
        {
            currentGameState = GAMESTATE_CHARACTERCREATION;
            currentCharacterCreationOption = 0;

            String retainName = player1.name;
            int retainRace = player1.race;
            int retainProf = player1.profession;
            player1 = new Player();
            player1.Initialize();
            player1.name = retainName;
            if (player1.name == "Player1") player1.name = PlayerProfileString();
            if (player1.name == "---") player1.name = "Guest";
            player1.race = retainRace;
            player1.profession = retainProf;

            PlayBGM(villageMusic);
        }

        private void PlayingGameOutroTransition()
        {
            TurnInQuest();
            currentGameState = GAMESTATE_OUTRO;
            isLevelComplete = false;
            settingsLoadComplete = false;
            runBonusXP = Math.Max(Math.Ceiling((player1.experience - totalBonusXP) / 5), 10); //2% bonus
            runBonusGold = Math.Ceiling(player1.gold / 50);
            if (!cheating)
            {
                totalBonusXP += runBonusXP;
                totalBonusGold += runBonusGold;
                //totalStats.IncrementStat("Games Cleared");
            }
            totalStats.AddStats(runStats);
            totalStats.Sort();
            totalSecondsPlayed += (long)runTime.Elapsed.TotalSeconds;
            runTime.Stop();
            if (!isTrialMode && !cheating) SaveSettings();
            outroTextOffset = 0;
            InitOutro();
            //MediaPlayer.Stop();
            MediaPlayer.Play(songOutro);
        }

        private void UpdateLogoScreen()
        {
            if (logoScreenTimer < logoScreenTimeMax)
            {
                logoScreenTimer++;
                if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed
                    || GamePad.GetState(PlayerIndex.Two).Buttons.A == ButtonState.Pressed
                    || GamePad.GetState(PlayerIndex.Three).Buttons.A == ButtonState.Pressed
                    || GamePad.GetState(PlayerIndex.Four).Buttons.A == ButtonState.Pressed
                    )
                {
                    if (logoScreenTimer > logoScreenEarliestSkip)
                    {
                        currentGameState = GAMESTATE_ORAWART_LOGO;
                        logoScreenTimer = 0;
                        MediaPlayer.Stop();
                    }
                }
            }
            else
            {
                currentGameState = GAMESTATE_ORAWART_LOGO;
                logoScreenTimer = 0;
                MediaPlayer.Stop();
            }
        }

        private void UpdateOrawartLogoScreen()
        {
            if (logoScreenTimer < logoScreenTimeMax)
            {
                logoScreenTimer++;
                if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed
                    || GamePad.GetState(PlayerIndex.Two).Buttons.A == ButtonState.Pressed
                    || GamePad.GetState(PlayerIndex.Three).Buttons.A == ButtonState.Pressed
                    || GamePad.GetState(PlayerIndex.Four).Buttons.A == ButtonState.Pressed
                    )
                {
                    if (logoScreenTimer > logoScreenEarliestSkip)
                    {
                        currentGameState = GAMESTATE_MUSIC_LOGO;
                        logoScreenTimer = 0;
                    }
                }
            }
            else
            {
                currentGameState = GAMESTATE_MUSIC_LOGO;
                logoScreenTimer = 0;
            }
        }

        private void UpdateMusicLogoScreen()
        {
            if (logoScreenTimer < logoScreenTimeMax)
            {
                logoScreenTimer++;
                if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed
                    || GamePad.GetState(PlayerIndex.Two).Buttons.A == ButtonState.Pressed
                    || GamePad.GetState(PlayerIndex.Three).Buttons.A == ButtonState.Pressed
                    || GamePad.GetState(PlayerIndex.Four).Buttons.A == ButtonState.Pressed
                    )
                {
                    if (logoScreenTimer > logoScreenEarliestSkip)
                    {
                        currentGameState = GAMESTATE_STORAGE_WARNING;
                        logoScreenTimer = 0;
                    }
                }
            }
            else
            {
                currentGameState = GAMESTATE_STORAGE_WARNING;
                logoScreenTimer = 0;
            }
        }

        private void UpdateStorageWarningScreen()
        {
            if (logoScreenTimer < logoScreenTimeMax)
            {
                logoScreenTimer++;
                if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed
                    || GamePad.GetState(PlayerIndex.Two).Buttons.A == ButtonState.Pressed
                    || GamePad.GetState(PlayerIndex.Three).Buttons.A == ButtonState.Pressed
                    || GamePad.GetState(PlayerIndex.Four).Buttons.A == ButtonState.Pressed
                    )
                {
                    if (logoScreenTimer > logoScreenEarliestSkip)
                    {
                        currentGameState = GAMESTATE_TITLESCREEN;
                        logoScreenTimer = 0;
                        MediaPlayer.Play(songTitle);
                    }
                }
            }
            else
            {
                currentGameState = GAMESTATE_TITLESCREEN;
                logoScreenTimer = 0;
                MediaPlayer.Play(songTitle);
            }
        }

        private void UpdateTitleScreen()
        {
            if (lastButtons == null || lastButtons.Buttons.Start == ButtonState.Released)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                {
                    if (!IsSignedIn(PlayerIndex.One))
                    {
                        titleScreenStatus = "Please Press       And Sign In";
                    }
                    else
                    {
                        LeaveTitleScreen(PlayerIndex.One);
                    }
                }
                if (GamePad.GetState(PlayerIndex.Two).Buttons.Start == ButtonState.Pressed)
                {
                    if (!IsSignedIn(PlayerIndex.Two))
                    {
                        titleScreenStatus = "Please Press       And Sign In";
                    }
                    else
                    {
                        LeaveTitleScreen(PlayerIndex.Two);
                    }
                }
                if (GamePad.GetState(PlayerIndex.Three).Buttons.Start == ButtonState.Pressed)
                {
                    if (!IsSignedIn(PlayerIndex.Three))
                    {
                        titleScreenStatus = "Please Press       And Sign In";
                    }
                    else
                    {
                        LeaveTitleScreen(PlayerIndex.Three);
                    }
                }
                if (GamePad.GetState(PlayerIndex.Four).Buttons.Start == ButtonState.Pressed)
                {
                    if (!IsSignedIn(PlayerIndex.Four))
                    {
                        titleScreenStatus = "Please Press       And Sign In";
                    }
                    else
                    {
                        LeaveTitleScreen(PlayerIndex.Four);
                    }
                }
            }
        }

        private void LeaveTitleScreen(PlayerIndex aPI)
        {
            if (!notCurrentlySaving) return;
            if (!playerIndex.Equals(aPI)) gamerStorageDevice = null;
            playerIndex = aPI;
            controller.SetPlayerIndex(aPI);
            playerIndexSet = true;
            currentGameState = GAMESTATE_MAINMENU;
            delay_current_mainmenu_input = 0;
            isTrialMode = Guide.IsTrialMode;
            if (!isTrialMode)
            {
                LoadSettings();
            }
            //CheckIfResumableExists();
            InitMainMenu();
            titleScreenStatus = "Press Start";
            showingCannotBuyWarning = false;
            correctCheatCodeDigits = 0;
            cheating = false;
            //CheckIfResumableExists();
        }

        public bool CanPlayerBuyGame(PlayerIndex player)
        {
            SignedInGamer gamer = Gamer.SignedInGamers[player];

            // if the player isn't signed in, they can't buy games
            if (gamer == null)
                return false;

            // lastly check to see if the account is allowed to buy games
            return gamer.Privileges.AllowPurchaseContent;
        }

        public bool QuestListContainsBoss()
        {
            if (currentQuestList == null) return false;
            if (currentQuestList.Count == 0) return false;
            for (int i = 0; i < currentQuestList.Count; i++)
            {
                if (currentQuestList[i].isBossQuest) return true;
            }
            return false;
        }

        public bool SpellIsNewlyKnown(String spellName, List<Spell> oldList, List<Spell> newList)
        {
            if (oldList == null || newList == null) return false;

            if (oldList.Count > 0)
            {
                for (int i = 0; i < oldList.Count; i++)
                {
                    if (oldList[i].spellName == spellName) return false;
                }
            }

            if (newList.Count > 0)
            {
                for (int i = 0; i < newList.Count; i++)
                {
                    if (newList[i].spellName == spellName) return true;
                }
            }

            return false;
        }

        private void UpdateParticleClusters()
        {
            if (particleClusters.Count() > 0)
            {
                for (int i = particleClusters.Count() - 1; i >= 0; i--)
                {
                    particleClusters[i].Update();
                    if (particleClusters[i].IsExpired()) particleClusters.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Update delays
            if (delay_current_move_looking < DELAY_MOVE_LOOKING)
            {
                delay_current_move_looking++;
            }
            if (delay_current_spell_targeting < DELAY_SPELL_TARGETING)
            {
                delay_current_spell_targeting++;
            }
            currentCreatureTint++;
            if (currentCreatureTint > CREATURE_TINT_PERIOD) currentCreatureTint = 0;

            currentParticleUpdateFrameDelay++;
            if (currentParticleUpdateFrameDelay > FRAMES_PER_PARTICLE_UPDATE)
            {
                currentParticleUpdateFrameDelay = 0;
                UpdateParticleClusters();
            }

            delay_current_signin_check++;
            if (delay_current_signin_check >= DELAY_CHECK_SIGNEDIN)
            {
                delay_current_signin_check = 0;
                if (playerIndexSet)
                {
                    if (currentGameState != GAMESTATE_TITLESCREEN
                        && currentGameState != GAMESTATE_LOGOSCREEN
                        && currentGameState != GAMESTATE_MUSIC_LOGO
                        && currentGameState != GAMESTATE_ORAWART_LOGO)
                    {
                        if (!IsSignedIn(playerIndex))
                        {
                            CloseAllDialogs();
                            currentGameState = GAMESTATE_TITLESCREEN;
                            MediaPlayer.Stop();
                            MediaPlayer.Play(songTitle);
                            titleScreenStatus = "PLAYER HAS SIGNED OUT\n\nPLEASE PRESS START";
                        }
                    }
                }
            }
            else
            {
                if (delay_current_signin_check == DELAY_CHECK_SIGNEDIN / 2)
                {
                    if (isTrialMode)
                    {
                        isTrialMode = Guide.IsTrialMode;
                    }
                }
            }

            if (featPanelDisplayTimer < featPanelDisplayDuration)
            {
                featPanelDisplayTimer++;
            }

            UpdateVibration();
            controller.SetVibration(controllerVibrate, controllerVibrate);

            switch (currentGameState)
            {
                case GAMESTATE_GAMEOVERSCREEN:
                    UpdateGameOver();
                    break;
                case GAMESTATE_PLAYING:
                    if (!resumableGameLoading) UpdateGamePlaying();
                    break;
                case GAMESTATE_CHARACTERCREATION:
                    UpdateCharacterCreation();
                    break;
                case GAMESTATE_MAINMENU:
                    UpdateMainMenu();
                    break;
                case GAMESTATE_TITLESCREEN:
                    if (!resumableGameSaving) UpdateTitleScreen();
                    break;
                case GAMESTATE_OUTRO:
                    UpdateOutro();
                    break;
                case GAMESTATE_LOGOSCREEN:
                    UpdateLogoScreen();
                    break;
                case GAMESTATE_MUSIC_LOGO:
                    UpdateMusicLogoScreen();
                    break;
                case GAMESTATE_ORAWART_LOGO:
                    UpdateOrawartLogoScreen();
                    break;
                case GAMESTATE_STORAGE_WARNING:
                    UpdateStorageWarningScreen();
                    break;
            }

            CheckIfTutorialMessageShouldShow();
            controller.Update();

            base.Update(gameTime);
        }

        private void NeverShowTutorialMessageAgain(int aID)
        {
            if (tutorialMessages != null & tutorialMessages.Count > 0)
            {
                for (int i = 0; i < tutorialMessages.Count; i++)
                {
                    if (tutorialMessages[i].ID == aID)
                    {
                        tutorialMessages[i].neverShowAgain = true;
                        break;
                    }
                }
            }
            if (tutorialMessagesNeverToSeeAgain.Count > 0)
            {
                for (int i = 0; i < tutorialMessagesNeverToSeeAgain.Count; i++)
                {
                    if (tutorialMessagesNeverToSeeAgain[i] == aID) break;
                }
                tutorialMessagesNeverToSeeAgain.Add(aID);
            }
            else
            {
                tutorialMessagesNeverToSeeAgain.Add(aID);
            }
        }

        private void ShowTutorialMessage(int aID)
        {
            if (!tutorialMessageOpen && !neverSeeTutorialMessages)
            {
                if (tutorialMessagesNeverToSeeAgain.Count > 0)
                {
                    for (int i = 0; i < tutorialMessagesNeverToSeeAgain.Count; i++)
                    {
                        if (tutorialMessagesNeverToSeeAgain[i] == aID) return;
                    }
                }
                for (int i = 0; i < tutorialMessages.Count; i++)
                {
                    if (tutorialMessages[i].ID == aID && !tutorialMessages[i].neverShowAgain && !tutorialMessages[i].shown)
                    {
                        tutorialMessages[i].shown = true;
                        currentTutorialMessage = SplitText(TUTORIAL_WINDOW_WIDTH - 4, tutorialMessages[i].text, sfFranklinGothic);
                        tutorialMessageOpen = true;
                        currentTutorialMessageOpen = aID;
                        break;
                    }
                }
            }
        }

        private void ShowAllTutorialMessagesAgain()
        {
            tutorialMessagesNeverToSeeAgain = new List<int>();
        }

        private void CheckIfTutorialMessageShouldShow()
        {
            if (moveCount == 1) ShowTutorialMessage(1);
            if (player1.poisoned) ShowTutorialMessage(11);
            if (player1.hunger < player1.full_stomach / 2) ShowTutorialMessage(14);
            if (inVillage)
            {
                if (player1.x > 21 && player1.x < 25 && player1.y > 4 && player1.y < 8)
                {
                    ShowTutorialMessage(2);
                }
                if (player1.x == 24 && player1.y == 6)
                {
                    ShowTutorialMessage(3);
                }
                if (player1.x == 16 && player1.y == 12)
                {
                    ShowTutorialMessage(5);
                }
                if (player1.y > 24 && player1.x > 15 && player1.x < 24)
                {
                    ShowTutorialMessage(16);
                }
            }
        }

        private void NameEntryCallback(IAsyncResult iaSR)
        {
            String enteredName = Guide.EndShowKeyboardInput(iaSR);
            if (enteredName != null && !enteredName.Equals(""))
            {
                if (enteredName.Length > 10) enteredName = enteredName.Substring(0, 10);
                player1.name = RemoveMissingCharacters(enteredName, sfSylfaen14);
            }
        }

        private String RemoveMissingCharacters(String aString, SpriteFont aSF)
        {
            String result = "";
            char[] chars = aString.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (sfSylfaen14.Characters.Contains(chars[i]))
                {
                    result = result + chars[i];
                }
                else
                {
                    result = result + '_';
                }
            }
            return result;
        }

        private void SetTileSize(int newSize)
        {
            TILE_SIZE = newSize;
            tilesInViewX = GAMEFIELDWIDTH / TILE_SIZE;
            tilesInViewY = GAMEFIELDHEIGHT / TILE_SIZE;
            halfTilesInViewX = tilesInViewX / 2;
            halfTilesInViewY = tilesInViewY / 2;
        }

        // - Pick up an item at tile aX, aY
        // - TODO: Make a sub-method to list items on a tile so that this method
        //and the method that lists item to pick up will always match
        // - Returns a bool to indicate if the user has used up their turn
        private bool PickUp(int itemIndex, int aX, int aY)
        {
            Item item = currentLevel.GetItem(itemIndex, aX, aY);
            if (item == null)
            {
                AddMessage("ERROR: Tried to pick up nonexistant item", true);
                return false;
            }
            else
            {
                if (item.itemType == (int)SC.ItemTypes.GOLD)
                {
                    player1.AddGold(item.value);
                    AddMessage("You Pick Up " + item.itemName);
                    sfxCoins.Play();
                    currentLevel.RemoveItem(itemIndex, aX, aY);
                    runStats.IncrementStat("Items Picked Up");
                    return true;
                }
                else
                {
                    if (player1.numberOfItems >= player1.MAX_ITEMS)
                    {
                        AddMessage("Inventory Full. Drop Something First!", true);
                        sfxSelectBad.Play();
                        return false;
                    }
                    else
                    {

                        if (item.weight + player1.currentCarriedWeight >= player1.totalEncumberance)
                        {
                            AddMessage("It Is Too Heavy To Carry!", true);
                            sfxSelectBad.Play();
                            return true;
                        }
                        else
                        {
                            if ((item.weight > 0) && (item.weight + player1.currentCarriedWeight >= player1.carryingCapacity))
                            {
                                AddMessage("With Difficulty, You Pick Up The " + item.itemName);
                            }
                            else
                            {
                                AddMessage("You Pick Up The " + item.itemName);
                            }
                            player1.carriedItems.Add(item);
                            player1.currentCarriedWeight += item.weight;
                            player1.numberOfItems += 1;
                            currentLevel.RemoveItem(itemIndex, aX, aY);
                            runStats.IncrementStat("Items Picked Up");
                            if (item.itemType == (int)SC.ItemTypes.ROCK
                                && item.rockType == (int)SC.RockTypes.GEM)
                                    IncreaseFeat(SC.FEAT_COLLECT_GEMS, 1);
                            if (item.itemType == (int)SC.ItemTypes.WEAPON
                                && item.range > 1)
                                    ShowTutorialMessage(20);
                            return true;
                        }
                    }
                }
            }
        }

        //Read a scroll. Return true if the scroll is consumed by the reading
        private bool UseScroll(Item aItem)
        {
            if (aItem.itemName == "Scroll of Detect Gold")
            {
                if (currentLevel.KnowGold(player1.profession == (int)SC.Professions.EXPLORER))
                {
                    AddMessage("You suddenly realise where precious metals are!");
                    sfxMagicHeal.Play();
                }
                else
                {
                    AddMessage("Your gut tells you there is no gold nearby", true);
                    //AddMessage("You Feel A Little Sad; Like You Remembered Something You Lost.");
                    sfxFail.Play();
                }
                return true;
            }
            if (aItem.itemName == "Scroll of Detect Item")
            {
                if (currentLevel.KnowItems())
                {
                    AddMessage("You suddenly remember where all the items are!");
                    sfxMagicHeal.Play();
                }
                else
                {
                    AddMessage("Your gut tells you there are no items nearby...", true);
                    //AddMessage("You Feel A Little Sad; Like You Remembered Something You Lost.");
                    sfxFail.Play();
                }
                return true;
            }
            if (aItem.itemName == "Scroll of Detect Creature")
            {
                if (currentLevel.KnowCreatures())
                {
                    AddMessage("You Sense Creatures Nearby...");
                    sfxMagicHeal.Play();
                }
                else
                {
                    AddMessage("You Get A Feeling You Are Alone. For Now, At Least");
                    sfxFail.Play();
                }
                return true;
            }
            if (aItem.itemName == "Scroll of Detect Furniture")
            {
                currentLevel.KnowDoors();
                currentLevel.KnowFurniture();
                AddMessage("You Suddenly Remember Where All The Furniture Goes!");
                sfxMagicHeal.Play();
                return true;
            }

            if (aItem.itemName == "Scroll of Wild Descent")
            {
                if (inVillage || currentLevel.isBossLevel)
                {
                    sfxSelectBad.Play();
                    if (inVillage)
                    {
                        AddMessage("This scroll has no effect outside of a dungeon", true);
                    }
                    if (currentLevel.isBossLevel)
                    {
                        AddMessage("Nothing happened...", true);
                    }
                    return false;
                }

                int beforeFloor = currentLevelNumber;
                WildDescent(5);
                if (beforeFloor != currentLevelNumber)
                {
                    AddMessage("When You Look Up From Reading The Scroll, You Find That The Room Has Changed");
                }
                return true;
            }

            if (aItem.itemName == "Karma Scroll")
            {
                if (inVillage || currentLevel.isBossLevel)
                {
                    sfxSelectBad.Play();
                    if (inVillage)
                    {
                        AddMessage("This scroll has no effect outside of a dungeon", true);
                    }
                    if (currentLevel.isBossLevel)
                    {
                        AddMessage("Nothing happened...", true);
                        return true; //Scroll wasted if player attempts to use it against boss...
                    }
                    return false;
                }
                //The logic of the scroll is something good if health or hunger or magic are under 33%
                //else something bad
                if (player1.currentHealth <= player1.MAX_HEALTH / 3
                    || player1.currentMagik <= player1.MAX_MAGIK / 3
                    || player1.hunger <= player1.full_stomach / 3)
                {
                    //Have something good happen. If something good doesn't happen, play a bad SFX.
                    if (!ExecuteGoodKarmaEvent())
                    {
                        sfxSelectBad.Play();
                    }
                }
                else
                {
                    ExecuteBadKarmaEvent();
                }
                return true;
            }

            if (aItem.itemName == "Life Scroll")
            {
                AddMessage("You Examine The Scroll, Its Magic Does Not Require You To Read It");
                return false;
            }

            if (aItem.itemName == "Scroll of Detect Traps")
            {
                int numDiscovered = currentLevel.KnowTraps();
                sfxMagicHeal.Play();
                if (numDiscovered == 0)
                {
                    AddMessage("You Do Not Detect Any Hidden Traps On This Floor...");
                }
                else
                {
                    AddMessage("You Detect The Presence Of Hidden Traps On This Floor!", true);
                    runStats.IncreaseStat("Traps Discovered", numDiscovered);
                }
                return true;
            }

            if (aItem.itemName == "Scroll of Cowardice")
            {
                if (inVillage)
                {
                    AddMessage("Use This Scroll In The Dungeon...");
                    return false;
                }
                if (currentLevel.isBossLevel)
                {
                    AddMessage("There's No Turning Back Now!", true);
                    sfxSelectBad.Play();
                    return false;
                }
                AddMessage("You Find Yourself Back In The Village!");
                ReturnToVillage();
                return true;
            }
            return false;
        }

        //Use a grindstone to improve current weapon
        private void UseGrindStone(Item gs)
        {
            if (gs == null || gs.itemType != (int)SC.ItemTypes.ROCK || gs.rockType != (int)SC.RockTypes.GRINDSTONE) return;
            player1.equippedSword.damage += gs.damage;
            if (gs.elementalOffenceFire > 0)
            {
                player1.equippedSword.elementalOffenceFire += gs.elementalOffenceFire;
                player1.equippedSword.elementalOffenceIce -= gs.elementalOffenceFire;
                if (player1.equippedSword.elementalOffenceIce < 0) player1.equippedSword.elementalOffenceIce = 0;
            }
            if (gs.elementalOffenceIce > 0)
            {
                player1.equippedSword.elementalOffenceIce += gs.elementalOffenceIce;
                player1.equippedSword.elementalOffenceFire -= gs.elementalOffenceIce;
                if (player1.equippedSword.elementalOffenceFire < 0) player1.equippedSword.elementalOffenceFire = 0;
            }
            player1.equippedSword.itemName = itemGenerator.ImproveItemName(player1.equippedSword, gs.damage);
            runStats.IncrementStat("Weapon Improvements");
            sfxLevelUp.Play();
        }


        //Cast a spell focused at aX, aY; referenced by spellname
        private bool CastSpell(String spellName, int aX, int aY)
        {
            if (player1.spells.GetIndexOfKnownSpell(spellName) == -1)
            {
                AddMessage("You Don't Know The Spell \"" + spellName + "\".");
                return false;
            }
            return CastSpell(player1.spells.GetIndexOfKnownSpell(spellName), aX, aY);
        }

        // Cast a spell focused at aX, aY
        private bool CastSpell(int spellIndex, int aX, int aY)
        {
            bool result = false;
            bool useful = false;
            Spell spellToCast = player1.spells.GetKnownSpells()[spellIndex];
            if (spellToCast.castingCost > player1.currentMagik)
            {
                AddMessage("You Do Not Have Enough Magic To Cast That!", true);
                result = false;
                sfxSelectBad.Play();
            }
            else
            {
                if (spellToCast.spellName.Equals("Heal"))
                {
                    player1.currentMagik -= spellToCast.castingCost;
                    sfxMagicHeal.Play();
                    particleClusters.Add(new ParticleCluster(16,
                                                        aX * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        aY * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 2, 3, 4, txEffectHeal, 4));
                    if (player1.currentHealth == player1.MAX_HEALTH)
                    {
                        AddMessage("You Feel A Slight Giddy Rush. You Didn't Need Healed!");
                        result = true;
                        useful = false;
                    }
                    else
                    {
                        if (player1.Heal(spellToCast.power * spellToCast.level))
                        {
                            AddMessage("Your Wounds Tickle As The Magic Repairs Them. Good As New!");
                            result = true;
                            useful = true;
                        }
                        else
                        {
                            AddMessage("You Feel The Magic Repair Your Flesh. Your Wounds Lessen.");
                            result = true;
                            useful = true;
                        }
                    }
                }
                if (spellToCast.spellName.Equals("Fireball"))
                {

                    player1.currentMagik -= spellToCast.castingCost;

                    int dX = 0;
                    int dY = 0;
                    int tX = aX;
                    int tY = aY;

                    if (aX > player1.x)
                    {
                        dX = 1;
                    }
                    else
                    {
                        if (aX < player1.x)
                        {
                            dX = -1;
                        }
                    }
                    if (aY > player1.y)
                    {
                        dY = 1;
                    }
                    else
                    {
                        if (aY < player1.y)
                        {
                            dY = -1;
                        }
                    }

                    bool spellHasStruck = false;
                    sfxMagicFire.Play();

                    while (!spellHasStruck)
                    {
                        if (currentLevel.IsTileSolid(tX, tY))
                        {
                            AddMessage("With A Roar, The Ball Of Flame Slams Into A Solid Surface");
                            particleClusters.Add(new ParticleCluster(8,
                                                        tX * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        tY * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 4, 3, 6, txEffectFire, 4));
                            spellHasStruck = true;
                            result = true;
                            useful = false;
                        }
                        else
                        {
                            Creature potentialCreature = currentLevel.GetCreature(tX, tY);
                            if (potentialCreature != null)
                            {
                                particleClusters.Add(new ParticleCluster(20,
                                                        tX * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        tY * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 4, 3, 6, txEffectFire, 5));
                                if (potentialCreature.Damage(spellToCast.level * spellToCast.power * potentialCreature.description.elementDefenceFire))
                                {
                                    if (potentialCreature.description.prefix == "")
                                    {
                                        AddMessage(potentialCreature.description.name + " Is Burnt To A Crisp!");
                                    }
                                    else
                                    {
                                        AddMessage("The " + potentialCreature.description.name + " Is Burnt To A Crisp!");
                                    }
                                    AddExperience(potentialCreature);
                                }
                                else
                                {
                                    if (potentialCreature.description.prefix == "")
                                    {
                                        AddMessage("The Fireball Smashes Into " + potentialCreature.description.name);
                                    }
                                    else
                                    {
                                        AddMessage("The Fireball Smashes Into The " + potentialCreature.description.name);
                                    }
                                    if (!potentialCreature.hostile)
                                    {
                                        potentialCreature.hostile = true;
                                        potentialCreature.allied = false;
                                        AddMessage("You Have Enraged The Friendly " + potentialCreature.description.description + "!", true);
                                    }
                                }
                                spellHasStruck = true;
                                result = true;
                                useful = true;
                            }
                            else
                            {
                                tX += dX;
                                tY += dY;
                            }
                        }
                    }

                }
                if (spellToCast.spellName.Equals("Freeze"))
                {

                    player1.currentMagik -= spellToCast.castingCost;
                    sfxMagicIce.Play();

                    int dX = 0;
                    int dY = 0;
                    int tX = aX;
                    int tY = aY;

                    if (aX > player1.x)
                    {
                        dX = 1;
                    }
                    else
                    {
                        if (aX < player1.x)
                        {
                            dX = -1;
                        }
                    }
                    if (aY > player1.y)
                    {
                        dY = 1;
                    }
                    else
                    {
                        if (aY < player1.y)
                        {
                            dY = -1;
                        }
                    }

                    bool spellHasStruck = false;

                    while (!spellHasStruck)
                    {
                        if (currentLevel.IsTileSolid(tX, tY))
                        {
                            AddMessage("Your Frosty Projectile Smashes Harmlessly Into A Solid Surface");
                            spellHasStruck = true;
                            result = true;
                            useful = false;
                            particleClusters.Add(new ParticleCluster(8,
                                                        tX * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        tY * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 4, 3, 6, txEffectIce, 4));
                        }
                        else
                        {
                            Creature potentialCreature = currentLevel.GetCreature(tX, tY);
                            if (potentialCreature != null)
                            {
                                particleClusters.Add(new ParticleCluster(20,
                                                        tX * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        tY * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 4, 3, 6, txEffectIce, 5));
                                if (potentialCreature.Damage(spellToCast.level * spellToCast.power * potentialCreature.description.elementDefenceIce))
                                {
                                    if (potentialCreature.description.prefix == "")
                                    {
                                        AddMessage(potentialCreature.description.name + " Is Killed By The Icey Onslaught!");
                                    }
                                    else
                                    {
                                        AddMessage("The " + potentialCreature.description.name + " Is Killed By The Icey Onslaught!");
                                    }
                                    AddExperience(potentialCreature);
                                }
                                else
                                {
                                    if (potentialCreature.description.prefix == "")
                                    {
                                        AddMessage("The Frost Blast Smashes Into " + potentialCreature.description.name);
                                    }
                                    else
                                    {
                                        AddMessage("The Frost Blast Smashes Into The " + potentialCreature.description.name);
                                    }
                                    if (!potentialCreature.hostile)
                                    {
                                        potentialCreature.hostile = true;
                                        potentialCreature.allied = false;
                                        AddMessage("You Have Enraged The Friendly " + potentialCreature.description.description + "!", true);
                                    }
                                    if ((float)((player1.level + spellToCast.level) * potentialCreature.description.elementDefenceIce)
                                            / (float)(potentialCreature.currentLevel * 2)
                                            * random.Next(0, 100)
                                            > 40)
                                    {
                                        potentialCreature.frozen = true;
                                        potentialCreature.freezeTimeRemaining += (int)Math.Ceiling(Math.Max(16, (player1.level + spellToCast.level) / (potentialCreature.currentLevel * 1.5)));
                                        if (potentialCreature.description.prefix == "")
                                        {
                                            AddMessage(potentialCreature.description.name + " Is Frozen Solid!");
                                        }
                                        else
                                        {
                                            AddMessage("The " + potentialCreature.description.name + " Is Frozen Solid!");
                                        }
                                    }
                                }

                                spellHasStruck = true;
                                result = true;
                                useful = true;
                            }
                            else
                            {
                                tX += dX;
                                tY += dY;
                            }
                        }
                    }
                }
                if (spellToCast.spellName.Equals("Magic Missile"))
                {
                    if (!currentLevel.IsLineOfSight(player1.x, player1.y, aX, aY))
                    {
                        AddMessage("You Can Only Strike Places You Can See!");
                        sfxSelectBad.Play();
                    }
                    else
                    {
                        player1.currentMagik -= spellToCast.castingCost;
                        sfxMagicMissile.Play();
                        Creature potentialCreature = currentLevel.GetCreature(aX, aY);
                        if (potentialCreature == null)
                        {
                            if (player1.x == aX && player1.y == aY)
                            {
                                particleClusters.Add(new ParticleCluster(20,
                                                        aX * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        aY * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 4, 3, 6, txEffectImpact, 5));
                                if (player1.Damage(spellToCast.power * player1.intelligence))
                                {
                                    AddMessage("The Last Thing You See Is Blinding White Energy. You Are Dead.", true);
                                    player1.killedBySelf = true;
                                }
                                else
                                {
                                    AddMessage("Ouch! That Stings!");
                                }
                            }
                            else
                            {
                                AddMessage("Your Magic Missile Doesn't Hit Anything");
                            }
                            result = true;
                            useful = false;
                        }
                        else
                        {
                            particleClusters.Add(new ParticleCluster(20,
                                                        aX * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        aY * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 4, 3, 6, txEffectImpact, 5));
                            if (potentialCreature.Damage(spellToCast.power * player1.intelligence))
                            {
                                if (potentialCreature.description.prefix == "")
                                {
                                    AddMessage(potentialCreature.description.name + " Is Killed!");
                                }
                                else
                                {
                                    AddMessage("The " + potentialCreature.description.name + " Is Killed!");
                                }
                                result = true;
                                useful = true;
                                AddExperience(potentialCreature);
                            }
                            else
                            {
                                if (potentialCreature.description.prefix == "")
                                {
                                    AddMessage("Pure Magic Energy Slams Into " + potentialCreature.description.name + "!");
                                }
                                else
                                {
                                    AddMessage("Pure Magic Energy Slams Into The " + potentialCreature.description.name + "!");
                                }
                                if (!potentialCreature.hostile)
                                {
                                    potentialCreature.hostile = true;
                                    potentialCreature.allied = false;
                                    AddMessage("You Have Enraged The Friendly " + potentialCreature.description.description + "!", true);
                                }
                                result = true;
                                useful = true;
                            }
                        }
                    }
                }
                if (spellToCast.spellName.Equals("Inferno"))
                {
                    player1.currentMagik -= spellToCast.castingCost;
                    sfxMagicFire.Play();
                    AddMessage("The Air Around You Roars With Spontaneous Flame!");
                    result = true;
                    particleClusters.Add(new ParticleCluster(4,
                                                        halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE * 2, 3, 5, txEffectFire, 5));
                    if (currentLevel.creatureList.Count > 0)
                    {
                        for (int i = 0; i < currentLevel.creatureList.Count; i++)
                        {
                            Creature theCreature = currentLevel.creatureList[i];
                            for (int kX = theCreature.x; kX < theCreature.x + theCreature.description.width; kX++)
                            {
                                for (int kY = theCreature.y; kY < theCreature.y + theCreature.description.height; kY++)
                                {
                                    //NOTE: Using "CanSee()" to stop enemies behind cover getting hit :)
                                    if (currentLevel.CanSee(player1.x, player1.y, kX, kY) && (player1.x - kX) * (player1.x - kX) + (player1.y - kY) * (player1.y - kY) <= spellRadiusSquared)
                                    {
                                        useful = true;
                                        particleClusters.Add(new ParticleCluster(20,
                                                        kX * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        kY * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 4, 3, 6, txEffectFire, 5));
                                        if (theCreature.Damage(spellToCast.level * spellToCast.power * theCreature.description.elementDefenceFire))
                                        {
                                            if (theCreature.description.prefix == "")
                                            {
                                                AddMessage(theCreature.description.description + " Is Burnt To A Crisp!");
                                            }
                                            else
                                            {
                                                AddMessage("The " + theCreature.description.description + " Is Burnt To A Crisp!");
                                            }
                                            AddExperience(theCreature);
                                        }
                                        if (!theCreature.hostile)
                                        {
                                            theCreature.hostile = true;
                                            theCreature.allied = false;
                                            AddMessage("You Have Enraged The Friendly " + theCreature.description.description + "!", true);
                                        }
                                    }
                                    if (!theCreature.alive) break;
                                }
                                if (!theCreature.alive) break;
                            }
                        }
                    }
                }

                if (spellToCast.spellName.Equals("Blizzard"))
                {
                    player1.currentMagik -= spellToCast.castingCost;
                    sfxMagicIce.Play();
                    AddMessage("The Air Around You Crackles As The Temperature Drops To Sub Zero!");
                    result = true;
                    particleClusters.Add(new ParticleCluster(4,
                                                        halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE * 2, 2, 4, txEffectIce, 5));
                    if (currentLevel.creatureList.Count > 0)
                    {
                        for (int i = 0; i < currentLevel.creatureList.Count; i++)
                        {
                            Creature theCreature = currentLevel.creatureList[i];
                            for (int kX = theCreature.x; kX < theCreature.x + theCreature.description.width; kX++)
                            {
                                for (int kY = theCreature.y; kY < theCreature.y + theCreature.description.height; kY++)
                                {
                                    //NOTE: Using "CanSee()" to stop enemies behind cover getting hit :)
                                    if (currentLevel.CanSee(player1.x, player1.y, kX, kY) && (player1.x - kX) * (player1.x - kX) + (player1.y - kY) * (player1.y - kY) <= spellRadiusSquared)
                                    {
                                        useful = true;
                                        particleClusters.Add(new ParticleCluster(20,
                                                        kX * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        kY * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 4, 1, 5, txEffectIce, 5));
                                        if (theCreature.Damage(spellToCast.level * spellToCast.power * theCreature.description.elementDefenceIce))
                                        {
                                            if (theCreature.description.prefix == "")
                                            {
                                                AddMessage(theCreature.description.name + " Is Killed By The Icey Onslaught!");
                                            }
                                            else
                                            {
                                                AddMessage("The " + theCreature.description.name + " Is Killed By The Icey Onslaught!");
                                            }
                                            AddExperience(theCreature);
                                        }
                                        else
                                        {
                                            if (!theCreature.hostile)
                                            {
                                                theCreature.hostile = true;
                                                theCreature.allied = false;
                                                AddMessage("You Have Enraged The Friendly " + theCreature.description.description + "!", true);
                                            }
                                            if ((float)((player1.level + spellToCast.level) * theCreature.description.elementDefenceIce) / (float)(theCreature.currentLevel * 2) * random.Next(0, 100) > 40)
                                            {
                                                theCreature.frozen = true;
                                                theCreature.freezeTimeRemaining += Math.Max(2, 2 * player1.level + spellToCast.level / (theCreature.currentLevel * 2));
                                                if (theCreature.description.prefix == "")
                                                {
                                                    AddMessage(theCreature.description.name + " Is Frozen Solid!");
                                                }
                                                else
                                                {
                                                    AddMessage("The " + theCreature.description.name + " Is Frozen Solid!");
                                                }
                                            }
                                        }
                                    }
                                    if (!theCreature.alive) break;
                                }
                                if (!theCreature.alive) break;
                            }
                        }
                    }
                }
                if (spellToCast.spellName.Equals("Raze"))
                {
                    player1.currentMagik -= spellToCast.castingCost;
                    sfxMagicFire.Play();

                    int dX = 0;
                    int dY = 0;
                    int tX = aX;
                    int tY = aY;

                    if (aX > player1.x)
                    {
                        dX = 1;
                    }
                    else
                    {
                        if (aX < player1.x)
                        {
                            dX = -1;
                        }
                    }
                    if (aY > player1.y)
                    {
                        dY = 1;
                    }
                    else
                    {
                        if (aY < player1.y)
                        {
                            dY = -1;
                        }
                    }

                    bool spellHasStruck = false;

                    while (!spellHasStruck)
                    {
                        if (currentLevel.IsTileSolid(tX, tY))
                        {
                            //AddMessage("With A Roar, The Ball Of Flame Slams Into A Solid Surface");
                            particleClusters.Add(new ParticleCluster(8,
                                                        tX * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        tY * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 4, 3, 6, txEffectFire, 4));
                            spellHasStruck = true;
                            result = true;
                            //useful = false;
                        }
                        else
                        {
                            Creature potentialCreature = currentLevel.GetCreature(tX, tY);
                            if (potentialCreature != null)
                            {
                                particleClusters.Add(new ParticleCluster(20,
                                                        tX * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        tY * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 4, 3, 6, txEffectFire, 5));
                                if (potentialCreature.Damage(spellToCast.level * spellToCast.power * potentialCreature.description.elementDefenceFire))
                                {
                                    if (potentialCreature.description.prefix == "")
                                    {
                                        AddMessage(potentialCreature.description.name + " Is Burnt To A Crisp!");
                                    }
                                    else
                                    {
                                        AddMessage("The " + potentialCreature.description.name + " Is Burnt To A Crisp!");
                                    }
                                    AddExperience(potentialCreature);
                                }
                                else
                                {
                                    if (potentialCreature.description.prefix == "")
                                    {
                                        AddMessage("The Line Of Fire Rips Through " + potentialCreature.description.name);
                                    }
                                    else
                                    {
                                        AddMessage("The Line Of Fire Rips Through The " + potentialCreature.description.name);
                                    }
                                    if (!potentialCreature.hostile)
                                    {
                                        potentialCreature.hostile = true;
                                        potentialCreature.allied = false;
                                        AddMessage("You Have Enraged The Friendly " + potentialCreature.description.description + "!", true);
                                    }
                                }
                                //spellHasStruck = true;    //Continue :)
                                result = true;
                                useful = true;
                            }
                            else
                            {
                                particleClusters.Add(new ParticleCluster(8,
                                                        tX * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        tY * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 4, 3, 6, txEffectFire, 4));

                            }
                            tX += dX;
                            tY += dY;
                        }
                    }
                }

                if (spellToCast.spellName.Equals("Icicle"))
                {
                    player1.currentMagik -= spellToCast.castingCost;
                    sfxMagicIce.Play();

                    int dX = 0;
                    int dY = 0;
                    int tX = aX;
                    int tY = aY;

                    if (aX > player1.x)
                    {
                        dX = 1;
                    }
                    else
                    {
                        if (aX < player1.x)
                        {
                            dX = -1;
                        }
                    }
                    if (aY > player1.y)
                    {
                        dY = 1;
                    }
                    else
                    {
                        if (aY < player1.y)
                        {
                            dY = -1;
                        }
                    }

                    bool spellHasStruck = false;

                    while (!spellHasStruck)
                    {
                        if (currentLevel.IsTileSolid(tX, tY))
                        {
                            //AddMessage("Your Frosty Projectile Smashes Harmlessly Into A Solid Surface");
                            spellHasStruck = true;
                            result = true;
                            //useful = false;
                            particleClusters.Add(new ParticleCluster(8,
                                                        tX * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        tY * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 4, 3, 6, txEffectIce, 4));
                        }
                        else
                        {
                            Creature potentialCreature = currentLevel.GetCreature(tX, tY);
                            if (potentialCreature != null)
                            {
                                particleClusters.Add(new ParticleCluster(20,
                                                        tX * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        tY * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 4, 3, 6, txEffectIce, 5));
                                if (potentialCreature.Damage(spellToCast.level * spellToCast.power * potentialCreature.description.elementDefenceIce))
                                {
                                    if (potentialCreature.description.prefix == "")
                                    {
                                        AddMessage(potentialCreature.description.name + " Is Killed By The Icey Onslaught!");
                                    }
                                    else
                                    {
                                        AddMessage("The " + potentialCreature.description.name + " Is Killed By The Icey Onslaught!");
                                    }
                                    AddExperience(potentialCreature);
                                }
                                else
                                {
                                    if (potentialCreature.description.prefix == "")
                                    {
                                        AddMessage("The Icey Wave Engulfs " + potentialCreature.description.name);
                                    }
                                    else
                                    {
                                        AddMessage("The Icey Wave Engulfs The " + potentialCreature.description.name);
                                    }
                                    if (!potentialCreature.hostile)
                                    {
                                        potentialCreature.hostile = true;
                                        potentialCreature.allied = false;
                                        AddMessage("You Have Enraged The Friendly " + potentialCreature.description.description + "!", true);
                                    }
                                    if ((float)((player1.level + spellToCast.level) * potentialCreature.description.elementDefenceIce) / (float)(potentialCreature.currentLevel * 2) * random.Next(0, 100) > 40)
                                    {
                                        potentialCreature.frozen = true;
                                        potentialCreature.freezeTimeRemaining += Math.Max(2, 2 * player1.level + spellToCast.level / (potentialCreature.currentLevel * 2));
                                        if (potentialCreature.description.prefix == "")
                                        {
                                            AddMessage(potentialCreature.description.name + " Is Frozen Solid!");
                                        }
                                        else
                                        {
                                            AddMessage("The " + potentialCreature.description.name + " Is Frozen Solid!");
                                        }
                                    }
                                }

                                //spellHasStruck = true;
                                result = true;
                                useful = true;
                            }
                            else
                            {
                                particleClusters.Add(new ParticleCluster(8,
                                                        tX * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        tY * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 4, 3, 6, txEffectIce, 4));
                            }
                            tX += dX;
                            tY += dY;
                        }
                    }
                }

                if (spellToCast.spellName.Equals("Detect Food"))
                {
                    player1.currentMagik -= spellToCast.castingCost;

                    result = true;
                    useful = currentLevel.KnowFood();
                    if (useful)
                    {
                        sfxTeleport.Play();
                        AddMessage("You Detect The Location Of Food On This Floor");
                    }
                    else
                    {
                        sfxFail.Play();
                        AddMessage("You Fail To Detect Any Food", true);
                    }
                }
            }
            if (result && useful)
            {
                AddExperience(spellToCast.level);
            }
            return result;
        }

        //Try and eat 
        private bool EatFood(Item food)
        {
            if (food == null || food.itemType != (int)SC.ItemTypes.FOOD)
            {
                AddMessage("That's Not Food!", true);
                sfxSelectBad.Play();
                return false;
            }

            if (!food.rotten)
            {
                sfxMagicHeal.Play();
                if (player1.GainNutrients(food.nutrients))
                {
                    AddMessage("You're Full Up Now.");
                }
                else
                {
                    AddMessage("You Feel Less Hungry.");
                }
            }
            else
            {
                //Trying to eat rotten food

                //If in village, do not allow - the poison would not be effective but would still get the nutrients
                if (inVillage)
                {
                    AddMessage("Eating Rotten Food In The Village Is Forbidden!", true);
                    sfxSelectBad.Play();
                    return false;
                }

                bool poisoned = random.NextDouble() < BAD_FOOD_POISON_CHANCE;
                player1.GainNutrients(random.Next(1, (int)Math.Max(3, Math.Ceiling(food.nutrients / 2))));
                if (poisoned)
                {
                    sfxFail.Play();
                    player1.poisoned = true;
                    player1.poisonDamage += Math.Ceiling(player1.MAX_HEALTH / 100);
                    player1.poisonDuration += 20;
                    AddMessage("That Tasted Foul... You Feel Very Sick...", true);
                }
                else
                {
                    sfxMagicHeal.Play();
                    AddMessage("Rotten... But You Force It Down...");
                }
            }
            runStats.IncrementStat("Food Eaten");
            return true;
        }

        //Try to drink
        private bool DrinkPotion(Item potion)
        {
            if (potion.itemType != (int)SC.ItemTypes.POTION)
            {
                AddMessage("You Can't Drink That!", true);
                sfxSelectBad.Play();
                return false;
            }

            runStats.IncrementStat("Potions Drunk");

            if (potion.itemName == "Potion of Healing")
            {
                //Fighters get 30% health, else 20%
                double amountToHeal;
                sfxMagicHeal.Play();
                if (player1.profession == (int)SC.Professions.FIGHTER)
                {
                    amountToHeal = Math.Ceiling((player1.MAX_HEALTH / 100) * 40);
                }
                else
                {
                    amountToHeal = Math.Ceiling((player1.MAX_HEALTH / 100) * 30);
                }
                if (player1.Heal(amountToHeal))
                {
                    AddMessage("You Are Fully Healed!");
                    AddMessage("The Feel The Liquid Rush Through Every Muscle...");
                }
                else
                {
                    AddMessage("Tissue Is Re-Arranged");
                    AddMessage("The Potion Quickly Causes Your Wounds To Ache As The Damaged ");
                }
                return true;
            }
            if (potion.itemName == "Magic Potion")
            {
                sfxMagicHeal.Play();
                if (player1.RestoreMagik(Math.Ceiling((player1.MAX_MAGIK / 100) * 50)))
                {
                    AddMessage("Your Mind Feels Fully Focused");
                }
                else
                {
                    AddMessage("Your Mind Feels More Focused");
                }
                return true;
            }
            if (potion.itemName == "Antidote")
            {
                if (player1.poisoned)
                {
                    sfxMagicHeal.Play();
                    player1.ClearPoison();
                    AddMessage("You Feel Better");
                }
                else
                {
                    sfxSelectBad.Play();
                    AddMessage("The Potion Didn't Seem To Do Anything", true);
                }

                return true;
            }

            if (potion.itemName == "Attack Potion")
            {
                player1.attackBuff = player1.UnbuffedAttackPower();
                player1.attackBuffTurnsLeft = 400;
                sfxMagicHeal.Play();
                AddMessage("You Feel Mighty!");
                return true;
            }

            if (potion.itemName == "Vitality Elixir")
            {
                sfxRecruit.Play();
                player1.HealthBonus(2); //2% Health bonus
                AddMessage("Your Vitality Has Increased");
                return true;
            }

            if (potion.itemName == "Will Elixir")
            {
                sfxRecruit.Play();
                player1.MagikBonus(2); //2% Mana Bonus
                AddMessage("Your " + magicName + " Has Increased");
                return true;
            }

            AddMessage("ERROR: You Have No Idea What You Just Drank. You Hope It Was Safe", true);
            return true;
        }

        private void CreatureTrapCheck(Creature theCreature)
        {
            for (int iX = theCreature.x; iX < theCreature.x + theCreature.description.width; iX++)
            {
                for (int iY = theCreature.y; iY < theCreature.y + theCreature.description.width; iY++)
                {
                    Trap possibleTrap = currentLevel.GetTrap(iX, iY);
                    if (possibleTrap != null && !possibleTrap.disarmed)
                    {
                        double triggerRoll = random.NextDouble();
                        if (triggerRoll < CHANCE_TO_TRIGGER_TRAP)
                        {
                            double dodgeRoll = random.NextDouble();
                            if (dodgeRoll > CHANCE_TO_DODGE_TRAP)
                            {
                                switch (possibleTrap.trapType)
                                {
                                    case (int)SC.TrapType.ARROW:
                                        if (currentLevel.CanSee(player1.x, player1.y, iX, iY))
                                            sfxArrow.Play();
                                        if (theCreature.Damage(Math.Min(currentLevel.levelNumber * random.Next(1, 3), Math.Ceiling(theCreature.description.MAX_HEALTH / 3))))
                                        {
                                            if (currentLevel.CanSee(player1.x, player1.y, iX, iY))
                                                AddMessage("The " + theCreature.description.description + " Is Killed By An Arrow!");
                                        }
                                        if (!possibleTrap.discovered && currentLevel.CanSee(player1.x, player1.y, iX, iY))
                                        {
                                            AddMessage("The " + theCreature.description.description + " Stepped On An Arrow Trap!");
                                            possibleTrap.discovered = true;
                                            runStats.IncrementStat("Traps Discovered");
                                        }
                                        break;
                                    case (int)SC.TrapType.DESCENT:
                                        theCreature.alive = false;  //Remove the enemy from play
                                        theCreature.x = 0;          //Crap hack here, corpse will not appear.
                                        theCreature.y = 0;
                                        if (currentLevel.CanSee(player1.x, player1.y, iX, iY))
                                        {
                                            AddMessage("The " + theCreature.description.description + " Vanishes Into Thin Air!");
                                        }
                                        break;
                                    case (int)SC.TrapType.TELEPORT:
                                        if (currentLevel.CanSee(player1.x, player1.y, iX, iY))
                                        {
                                            AddMessage("The " + theCreature.description.description + " Vanishes Into Thin Air!");
                                        }
                                        RandomTeleportCreature(theCreature);
                                        break;
                                    case (int)SC.TrapType.POISON:
                                        if (currentLevel.CanSee(player1.x, player1.y, iX, iY))
                                        {
                                            //TODO: Poison the creature.
                                            AddMessage("The " + theCreature.description.description + " Triggered A Poison Gas Trap!");
                                            possibleTrap.discovered = true;
                                            runStats.IncrementStat("Traps Discovered");
                                        }
                                        break;
                                }
                            }
                            else //else monster dodged the trap - but player may have seen!
                            {
                                switch (possibleTrap.trapType)
                                {
                                    case (int)SC.TrapType.ARROW:
                                        if (!possibleTrap.discovered && currentLevel.CanSee(player1.x, player1.y, iX, iY))
                                        {
                                            sfxArrow.Play();
                                            AddMessage("An Arrow Whizzes Past The " + theCreature.description.description + "!");
                                            AddMessage("It Must Have Stepped On An Arrow Trap.");
                                            possibleTrap.discovered = true;
                                            runStats.IncrementStat("Traps Discovered");
                                        }
                                        break;
                                    case (int)SC.TrapType.POISON:
                                        if (currentLevel.CanSee(player1.x, player1.y, iX, iY))
                                        {
                                            AddMessage("The " + theCreature.description.description + " Triggered A Poison Gas Trap!");
                                            possibleTrap.discovered = true;
                                            runStats.IncrementStat("Traps Discovered");
                                        }
                                        break;
                                }

                            }
                        }
                        if (!theCreature.alive) break;
                    }
                    if (!theCreature.alive) break;
                }
            }

            //find and remove any dead creatures
            //Need to do this here, otherwise dead enemies will remain on screen
            for (int i = currentLevel.creatureList.Count - 1; i >= 0; i--)
            {
                if (!currentLevel.creatureList[i].alive)
                {
                    KillCreature(i);
                    //currentLevel.creatureList.RemoveAt(i);
                }
            }

        }

        private bool IsFastMoving()
        {
            return (movementHoldDuration >= SC.FRAMES_BETWEEN_QUICKMOVE_LB && controller.LeftShoulderIsHeld())
                                || (movementHoldDuration >= SC.FRAMES_BETWEEN_QUICKMOVE);
        }

        private void StopFastMovement()
        {
            //TODO: More crappy code in resetting leftStickHoldDuration to stop fast movement
            movementHoldDuration = 0;
        }

        //After player moves, check for traps
        private void PlayerTrapCheck()
        {
            //TODO: Crappy code here that pulls one trap, then gets ALL traps.

            Trap possibleTrap = currentLevel.GetTrap(player1.x, player1.y);
            if (possibleTrap != null)
            {
                if (!possibleTrap.disarmed)
                {
                    double triggerRoll = random.NextDouble();
                    if (player1.profession == (int)SC.Professions.EXPLORER) triggerRoll = triggerRoll * 1.2;
                    if (triggerRoll < CHANCE_TO_TRIGGER_TRAP)
                    {
                        runStats.IncrementStat("Traps Triggered");
                        double dodgeRoll = random.NextDouble();
                        if (player1.profession == (int)SC.Professions.EXPLORER) dodgeRoll = dodgeRoll * 0.8;
                        switch (possibleTrap.trapType)
                        {
                            case (int)SC.TrapType.ARROW:
                                sfxArrow.Play();
                                if (dodgeRoll < CHANCE_TO_DODGE_TRAP)
                                {
                                    runStats.IncrementStat("Traps Dodged");
                                    if (possibleTrap.discovered)
                                    {
                                        AddMessage("The Arrow Trap Fires But Misses!");
                                    }
                                    else
                                    {
                                        AddMessage("An Arrow Suddenly Whizzes Past Your Head!", true);
                                        possibleTrap.discovered = true;
                                        ShowTutorialMessage(9);
                                        StopFastMovement();
                                        runStats.IncrementStat("Traps Discovered");
                                    }
                                }
                                else
                                {
                                    double damage = currentLevel.levelNumber * random.Next(1, 4);
                                    player1.LimitedDamage(damage);
                                    SetVibration(damage);
                                    particleClusters.Add(new ParticleCluster(20,
                                                        halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 4, 1, 5, txEffectImpact, 5));
                                    AddMessage("An Arrow Pierces Your Flesh!", true);
                                    sfxEnemyAttack.Play();
                                    StopFastMovement();
                                    if (!possibleTrap.discovered)
                                    {
                                        AddMessage("You Triggered An Arrow Trap!", true);
                                        possibleTrap.discovered = true;
                                        ShowTutorialMessage(9);
                                        runStats.IncrementStat("Traps Discovered");
                                    }
                                }
                                break;
                            case (int)SC.TrapType.POISON:
                                if (dodgeRoll < CHANCE_TO_DODGE_TRAP)
                                {
                                    runStats.IncrementStat("Traps Dodged");
                                    if (possibleTrap.discovered)
                                    {
                                        AddMessage("Poison Gas Hisses Around You, But You Are Unaffected");
                                        StopFastMovement();
                                        particleClusters.Add(new ParticleCluster(20,
                                                        halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 4, 1, 5, txEffectGas, 5));
                                    }
                                    else
                                    {
                                        AddMessage("You Hear A Hiss Of Gas Being Released", true);
                                        possibleTrap.discovered = true;
                                        ShowTutorialMessage(9);
                                        StopFastMovement();
                                        runStats.IncrementStat("Traps Discovered");
                                    }
                                }
                                else
                                {
                                    PoisonPlayerScaledToLevel();

                                    AddMessage("You Breath In Poisonous Gas!", true);
                                    sfxFail.Play();
                                    StopFastMovement();
                                    if (!possibleTrap.discovered)
                                    {
                                        AddMessage("You Triggered A Poison Gas Trap!", true);
                                        ShowTutorialMessage(9);
                                        possibleTrap.discovered = true;
                                        runStats.IncrementStat("Traps Discovered");
                                    }
                                }
                                break;
                            case (int)SC.TrapType.TELEPORT:
                                if (dodgeRoll < CHANCE_TO_DODGE_TRAP)
                                {
                                    runStats.IncrementStat("Traps Dodged");
                                    if (possibleTrap.discovered)
                                    {
                                        AddMessage("The Teleport Trap Fails To Affect You");
                                    }
                                }
                                else
                                {
                                    RandomTeleport();
                                    StopFastMovement();
                                    AddMessage("What Just Happened?! Where Are You?");
                                    sfxTeleport.Play();
                                }
                                break;
                            case (int)SC.TrapType.DESCENT:
                                if (dodgeRoll < CHANCE_TO_DODGE_TRAP)
                                {
                                    runStats.IncrementStat("Traps Dodged");
                                    if (possibleTrap.discovered)
                                    {
                                        AddMessage("The Wild Descent Trap Fails To Affect You");
                                    }
                                }
                                else
                                {
                                    StopFastMovement();
                                    AddMessage("You Must Have Stepped On A Wild Descent Trap!", true);
                                    possibleTrap.discovered = true;
                                    ShowTutorialMessage(9);
                                    runStats.IncrementStat("Traps Discovered");
                                    WildDescent(random.Next(1, 10));
                                }
                                break;
                        }
                    }
                    else //Didn't trigger it. Chance to discover it instead.
                    {
                        double discoverRoll = random.NextDouble();
                        if (player1.profession == (int)SC.Professions.EXPLORER) discoverRoll = discoverRoll * 0.6;
                        if (discoverRoll < CHANCE_TO_DISCOVER_TRAP && !possibleTrap.discovered && !possibleTrap.disarmed)
                        {
                            possibleTrap.discovered = true;
                            AddMessage("You Are Standing On A Trap!", true);
                            ShowTutorialMessage(9);
                            StopFastMovement();
                            runStats.IncrementStat("Traps Discovered");
                        }
                    }
                }
            }

            List<Trap> allTraps = currentLevel.trapList;
            if (allTraps != null && allTraps.Count > 0)
            {
                for (int i = 0; i < allTraps.Count; i++)
                {
                    //If already discovered and beside player, stop fast movement
                    if (allTraps[i].discovered && !allTraps[i].disarmed)
                    {
                        if (Math.Abs(allTraps[i].x - player1.x) <= 1 && Math.Abs(allTraps[i].y - player1.y) <= 1)
                        {
                            StopFastMovement();
                        }
                    }

                    //If not discovered and within range of sight, chance to spot it
                    if (!allTraps[i].discovered && currentLevel.CanSee(player1.x, player1.y, allTraps[i].x, allTraps[i].y))
                    {
                        if (allTraps[i].x != player1.x || allTraps[i].y != player1.y)
                        {
                            double discoverRoll = random.NextDouble();
                            if (player1.profession == (int)SC.Professions.EXPLORER) discoverRoll = discoverRoll * 0.6;
                            int distanceToTrap = (int)Math.Ceiling(Math.Sqrt((player1.x - allTraps[i].x) * (player1.x - allTraps[i].x) + (player1.y - allTraps[i].y) * (player1.y - allTraps[i].y)));
                            for (int j = 0; j < distanceToTrap; j++)
                            {
                                discoverRoll = discoverRoll * 1.25;
                            }
                            if (discoverRoll < CHANCE_TO_DISCOVER_TRAP)
                            {
                                allTraps[i].discovered = true;
                                ShowTutorialMessage(9);
                                StopFastMovement();
                                switch (allTraps[i].trapType)
                                {
                                    case (int)SC.TrapType.ARROW:
                                        AddMessage("You Notice An Arrow Trap.", true);
                                        break;
                                    case (int)SC.TrapType.DESCENT:
                                        AddMessage("You Notice A Trap Of Wild Descent.", true);
                                        break;
                                    case (int)SC.TrapType.POISON:
                                        AddMessage("You Notice A Poison Gas Trap, Be Careful.", true);
                                        break;
                                    case (int)SC.TrapType.TELEPORT:
                                        AddMessage("You Notice A Teleport Trap.", true);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void PoisonPlayerScaledToLevel()
        {
            //NOTE: No point calling LimitedDamage here
            //If player left at 1 health; they will die anyway because poisoned!
            player1.LimitedDamage(Math.Ceiling(currentLevel.levelNumber / 5));
            SetVibration(currentLevel.levelNumber);
            particleClusters.Add(new ParticleCluster(20,
                                halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                TILE_SIZE / 4, 1, 5, txEffectGas, 5));
            player1.poisoned = true;
            player1.poisonDamage = Math.Ceiling(currentLevel.levelNumber / 5);
            player1.poisonDuration = random.Next(4, 50);
        }

        private void RandomTeleport()
        {
            Point newLoc = currentLevel.RandomEmptyTile();
            player1.x = newLoc.X;
            player1.y = newLoc.Y;
            if (player1.profession != (int)SC.Professions.EXPLORER) currentLevel.ResetVision();
            UpdateVision();
            CreatureVisionCheck();
        }

        private void RandomTeleportCreature(Creature theCreature)
        {
            Point newLoc = currentLevel.RandomEmptyTile();
            theCreature.x = newLoc.X;
            theCreature.y = newLoc.Y;
        }

        private void WildDescent(int numFloors)
        {
            //int resultingFloor = currentLevel.CantDescendBelowBoss(currentLevelNumber, currentLevelNumber + numFloors);
            int resultingFloor = currentLevel.levelNumber + numFloors;
            if (resultingFloor >= currentQuest.level + currentQuest.numFloors)
            {
                //If descent would exceed level size then try and move to last floor of dungeon, else fail
                if (currentLevel.levelNumber < currentQuest.level + currentQuest.numFloors - 1)
                {
                    sfxTeleport.Play();
                    currentLevel.levelNumber = currentQuest.level + currentQuest.numFloors - 2; //Level Complete Code Will Descend By 1, So Subtract One Now
                    isLevelComplete = true;
                }
                else
                {
                    AddMessage("A Strange Force Prevents Your Descent...", true);
                    sfxFail.Play();
                }
            }
            else
            {
                sfxTeleport.Play();
                currentLevel.levelNumber += numFloors - 1; //Level Complete Code Will Descend By 1, So Subtract One Now
                isLevelComplete = true;
            }
        }

        private void PullDescendingEnemies()
        {
            descendingCreatures = new List<Creature>();
            if (currentLevel.creatureList == null) return;
            if (currentLevel.creatureList.Count < 1) return;
            for (int i = 0; i < currentLevel.creatureList.Count; i++)
            {
                Creature creature = currentLevel.creatureList[i];
                if (currentLevel.CanSee(player1.x, player1.y, creature.x, creature.y))
                {
                    if (creature.allied)
                    {
                        descendingCreatures.Add(creature);
                    }
                }
            }
        }

        private void PushDescendingEnemies()
        {
            if (currentLevel.creatureList == null || descendingCreatures == null) return;
            if (descendingCreatures.Count < 1) return;
            for (int i = 0; i < descendingCreatures.Count; i++)
            {
                Point freePosition = currentLevel.EmptyPointNear(player1.x, player1.y);
                if (freePosition != null)
                {
                    descendingCreatures[i].x = freePosition.X;
                    descendingCreatures[i].y = freePosition.Y;
                    currentLevel.creatureList.Add(descendingCreatures[i]);
                }
            }
        }

        //Sell items marked as For Sale. False if none were marked
        private double SellItems()
        {
            if (player1.carriedItems == null || player1.carriedItems.Count == 0) return 0;
            double result = 0;

            for (int i = player1.carriedItems.Count - 1; i >= 0; i--)
            {
                if (player1.carriedItems[i].forSale)
                {
                    result += Math.Ceiling(player1.carriedItems[i].value * merchantMarkUp);
                    player1.AddGold(Math.Ceiling(player1.carriedItems[i].value * merchantMarkUp));
                    if (player1.carriedItems[i].loadOut) goldBonusCorrection += Math.Ceiling(player1.carriedItems[i].value * merchantMarkUp);
                    RemoveItemFromLootList(player1.carriedItems[i]);
                    player1.DropItem(player1.carriedItems[i]);
                    runStats.IncrementStat("Items Sold");
                }
            }
            if (player1.profession == (int)SC.Professions.EXPLORER) result = Math.Ceiling(result * explorerPriceBonus);
            return result;
        }

        private bool CanBuy(Item aItem)
        {
            return (player1.gold >= MerchantPrice(aItem));
        }

        private double MerchantPrice(Item aItem)
        {
            double result = aItem.value + Math.Floor(aItem.value * merchantMarkUp);
            if (aItem.itemType == (int)SC.ItemTypes.FOOD) result = result + (currentLevelNumber * 3);
            if (player1.profession == (int)SC.Professions.EXPLORER) result = Math.Ceiling(result / explorerPriceBonus);
            return result;
        }

        private void BuyItem(int aIndex)
        {
            List<Item> theItems;
            if (inVillage)
            {
                theItems = villageMerchantItems;
            }
            else
            {
                theItems = dungeonMerchantItems;
            }
            player1.RemoveGold(MerchantPrice(theItems[aIndex]));
            player1.AddItem(theItems[aIndex]);
            theItems.RemoveAt(aIndex);
            runStats.IncrementStat("Items Bought");
        }

        private void NewGame()
        {
            messages = new List<LogMessage>();
            runStats = new Stats();
            runTime.Reset();
            runTime.Start();
            totalStats.IncrementStat("Games Played");
            moveCount = 0;
            AddMessage("Welcome to " + TITLE_STRING);
            playerQuit = false;
            isSfxDeathPlayedThisGame = false;
            inVillage = true;
            villageLevel = new Level(true, bestiary);
            currentLevel = villageLevel;
            villageMerchantItems = itemGenerator.VillageMerchantInventory(villageMerchantLevel, 7);
            if (loadedVillageBuildArea != null) villageLevel.SetVillageBuildArea(loadedVillageBuildArea);
            villageLevel.LoadItemsToPlayerBuildArea(loadedVillageItems);
            villageLevel.itemGenerator = itemGenerator;
            //villageLevel.SpamVillageBuildArea();
            dungeonLevel = null;
            currentQuest = null;
            currentLevelNumber = startingLevel;
            player1.x = currentLevel.startingX;
            player1.y = currentLevel.startingY;
            currentLevel.SetPerception(player1.perception);
            UpdateVision();
            if (player1.profession == (int)SC.Professions.EXPLORER) currentLevel.KnowExit();
            player1.equippedArmour = itemGenerator.GenerateArmour(5);
            player1.equippedShield = itemGenerator.GenerateShield(5);
            player1.equippedSword = itemGenerator.GenerateSword(5);
            player1.equippedCloak = itemGenerator.GenerateCloak(5);
            player1.spells.UnlockSpells(player1); //Need to set known spells at this point
            GenerateStartingGear();
            currentInventoryTab = 0; //Reset inventory tab to "All"
            goldBonusCorrection = 0;
            GenerateQuestList();
            PlaceEasyQuestInQuestList();
            SetTileSize(TILE_SIZE);
            SetScreenProjection();
            for (int i = 0; i < tutorialMessages.Count; i++)
            {
                tutorialMessages[i].shown = false;
            }
            ShowTutorialMessage(0);
        }

        private void ResumeGame()
        {
            resumableGameLoading = true;
            LoadResumableGame();
            MediaPlayer.Stop();
        }

        private void PlayBGM(BackgroundMusicTrack bgm)
        {
            if (bgmPlayOption != SC.BGMPlayOption.OFF)
            {
                MediaPlayer.Play(bgm.track);
            }
        }

        private void PlayDungeonBGM(String bgmTitle)
        {
            if (bgmPlayOption != SC.BGMPlayOption.OFF)
            {
                for (int i = 0; i < dungeonMusicList.Count; i++)
                {
                    if (dungeonMusicList[i].title == bgmTitle)
                    {
                        dungeonMusic = dungeonMusicList[i];
                        MediaPlayer.Play(dungeonMusic.track);
                        break;
                    }
                }
            }
        }

        private void ShuffleDungeonBGM()
        {
            dungeonMusic = dungeonMusicList[random.Next(dungeonMusicList.Count)];
        }

        private void SetScreenProjection()
        {
            if (displayFullScreen)
            {
                Projection = Matrix.CreateOrthographicOffCenter((float)-DISPLAYWIDTH / 2 + (float)DISPLAYWIDTH / 200f * FULLSCREENZOOMX,
                                                                                (float)DISPLAYWIDTH / 2 - (float)DISPLAYWIDTH / 200f * FULLSCREENZOOMX,
                                                                                (float)-DISPLAYHEIGHT / 2 + (float)DISPLAYHEIGHT / 200f * FULLSCREENZOOMY,
                                                                                (float)DISPLAYHEIGHT / 2 - (float)DISPLAYHEIGHT / 200f * FULLSCREENZOOMY,
                                                                                1f, 200f
                                                                                );
            }
            else
            {
                Projection = Matrix.CreateOrthographicOffCenter((float)-DISPLAYWIDTH / 2 + (float)DISPLAYWIDTH / 200f,
                                                                                (float)DISPLAYWIDTH / 2 - (float)DISPLAYWIDTH / 200f,
                                                                                (float)-DISPLAYHEIGHT / 2 + (float)DISPLAYHEIGHT / 200f,
                                                                                (float)DISPLAYHEIGHT / 2 - (float)DISPLAYHEIGHT / 200f,
                                                                                1f, 200f
                                                                                );
            }
            basicEffect.Projection = Projection;
        }

        private void CloseAllDialogs()
        {
            currentMainMenuSelection = 0;
            dialogOpen = false;

            totalStatsOpen = false;
            helpOpen = false;
            creditsOpen = false;
            featSummaryOpen = false;
            currentStatScreenOffset = 0;
            contextDialogOpen = false;
            currentContextMenuSelection = 0;

            contextDialogSubMenuOpen = false;
            subMenuAction = "";
            currentContextSubMenuSelection = 0;

            startMenuOpen = false;
            currentStartMenuSelection = 0;

            optionsMenuOpen = false;
            currentOptionMenuSelection = 0;

            inventoryOpen = false;
            currentInventorySelection = 0;
            inventoryOffset = 0;

            characterSheetOpen = false;

            spellListOpen = false;
            currentSpellListSelection = 0;

            runStatsOpen = false;
            currentStatOffset = 0;

            shopOpen = false;
            shopConfirmOpen = false;
            currentShopSelection = 0;

            tutorialMessageOpen = false;

            gameQuitConfirmOpen = false;
            gameSuspendConfirm = false;

            controllerVibrate = 0f;
        }

        private void SetVibration(double damage)
        {
            controllerVibrate = (float)Math.Max((2 * damage / player1.MAX_HEALTH), 0.1);
        }

        private void UpdateVibration()
        {
            controllerVibrate = controllerVibrate * controllerVibrateDropOff;
            if (controllerVibrate < 0.05f) controllerVibrate = 0f;
        }

        private List<String> GetTileContextMenuOptions(int aX, int aY)
        {
            List<String> aList = currentLevel.GetTileContextMenuOptions(aX, aY);
            if (inVillage && currentLevel.PlayerCanBuild(aX, aY) && (aX != player1.x || aY != player1.y))
            {
                if (currentLevel.GetTile(aX, aY) == TILE_VILLAGE_FLOOR)
                {
                    if (currentLevel.CanMove(aX, aY) && currentLevel.GetItem(0, aX, aY) == null)
                    {
                        aList.Add("Build Wall (" + VILLAGE_BUILD_WALL_COST + ")");
                        aList.Add("Build Door (" + VILLAGE_BUILD_DOOR_COST + ")");
                    }
                }
                else
                {
                    if (currentLevel.GetTile(aX, aY) == TILE_VILLAGE_DOOR_CLOSED || currentLevel.GetTile(aX, aY) == TILE_VILLAGE_DOOR_OPEN)
                    {
                        aList.Add("Remove Door (" + VILLAGE_DEMOLISH_DOOR_COST + ")");
                    }
                    if (currentLevel.GetTile(aX, aY) == TILE_VILLAGE_WALL)
                    {
                        aList.Add("Demolish Wall (" + VILLAGE_DEMOLISH_WALL_COST + ")");
                    }
                }
            }
            if (currentLevel.GetTrap(aX, aY) != null)
            {
                if (currentLevel.GetTrap(aX, aY).discovered)
                {
                    if (!currentLevel.GetTrap(aX, aY).disarmed)
                    {
                        aList.Insert(0, "Disarm Trap");
                    }
                }
            }
            if (currentLevel.GetFurniture(aX, aY) != null)
            {
                if (currentLevel.GetFurniture(aX, aY).inscribed) aList.Add("Read");
                if (currentLevel.GetFurniture(aX, aY).furnType == (int)SC.FurnTypes.TELEPORTER)
                {
                    if (inVillage)
                    {
                        aList.Add("Warp To Dungeon");
                    }
                    else
                    {
                        aList.Add("Return To Village");
                    }
                }
                if (currentLevel.GetFurniture(aX, aY).furnType == (int)SC.FurnTypes.ALTAR)
                {
                    aList.Add("Pray");
                }
            }
            aList.Add("Look");
            if (aX == player1.x && aY == player1.y)
            {
                aList.Add("Rest");
                if (currentLevel.GetItems(player1.x, player1.y).Count > 0)
                {
                    aList.Add("Pick Up...");
                }
                if (currentLevel.GetTile(player1.x, player1.y) == TILE_STAIRS_DOWN)
                {
                    aList.Add("Venture Down");
                }
            }
            else
            {
                if (player1.profession == (int)SC.Professions.EXPLORER && !inVillage)
                {
                    if (currentLevel.GetTile(aX, aY) == TILE_WALL) aList.Add("Dig");
                }
                if (currentLevel.GetCreature(aX, aY) != null)
                {

                    if (!currentLevel.GetCreature(aX, aY).hostile)
                    {
                        aList.Add("Chat");
                        if (!currentLevel.GetCreature(aX, aY).allied)
                        {
                            if (currentLevel.GetCreature(aX, aY).description.creatureType == (int)SC.CreatureTypes.QUESTRESCUEE)
                            {
                                aList.Add("Rescue");
                            }
                            else
                            {
                                aList.Add("Recruit");
                            }
                        }
                        if (!inVillage) aList.Add("Switch Places");
                    }
                    if (currentLevel.GetCreature(aX, aY).description.creatureType == (int)SC.CreatureTypes.MERCHANT)
                    {
                        aList.Insert(0, "Trade...");
                        if (inVillage && villageMerchantLevel < MAX_VILLAGE_MERCHANT_LEVEL)
                        {
                            aList.Insert(1, "Invest " + (villageMerchantLevel * GOLD_PER_VILLAGE_MERCHANT_LEVEL).ToString() + " Gold");
                        }
                    }
                    if (currentLevel.GetCreature(aX, aY).allied)
                    {
                        if (currentLevel.GetCreature(aX, aY).waiting)
                        {
                            aList.Add("Follow");
                        }
                        else
                        {
                            aList.Add("Wait There");
                        }
                    }
                    if (player1.profession == (int)SC.Professions.FIGHTER && !inVillage)
                    {
                        aList.Add("Kick");
                    }
                    if (!inVillage) aList.Add("Attack!");
                }
            }
            return aList;
        }

        //A method to handle player actions on a given tile aX,aY
        //Returns true or false to reflect whether the user's turn has been used up
        public bool PlayerAction(String aAction, int aX, int aY)
        {
            if (aAction != null)
            {
                if (aAction.ToString() == "Rest")
                {
                    return PlayerActionRest(inVillage);
                }
                if (aAction.ToString() == "Read")
                {
                    if (Math.Abs(aX - player1.x) > 1 || Math.Abs(aY - player1.y) > 1)
                    {
                        AddMessage("You Need To Stand Beside It To Read It!");
                        return false;
                    }
                    Furniture aFurn = currentLevel.GetFurniture(aX, aY);
                    if (aFurn == null)
                    {
                        AddMessage("There's Nothing To Read There!");
                        return false;
                    }
                    AddMessage("\"" + aFurn.inscription + "\"");
                    AddMessage("The inscription says...");
                    return true;
                }
                if (aAction.ToString() == "Look")
                {
                    String tileString = "";
                    bool seenSomething = true;
                    if (currentLevel.GetTileVision(aX, aY) > 0)
                    {
                        switch (currentLevel.GetTile(aX, aY))
                        {
                            case TILE_DOOR_CLOSED:
                                tileString = "A Closed door. ";
                                break;
                            case TILE_DOOR_OPEN:
                                tileString = "An open door. ";
                                break;
                            case TILE_STAIRS_DOWN:
                                tileString = "Stairs leading further down. ";
                                break;
                            case TILE_WALL:
                                tileString = "Solid wall. ";
                                break;
                            default:
                                tileString = "";
                                seenSomething = false;
                                break;
                        }
                    }
                    else
                    {
                        seenSomething = false;
                    }

                    List<Item> tileItems = currentLevel.GetKnownItems(aX, aY);
                    if (tileItems.Count > 0)
                    {
                        seenSomething = true;
                        AddMessage(tileString + "You See: " + tileItems[0].itemName);
                        if (tileItems.Count > 1)
                        {
                            for (int i = 1; i < tileItems.Count; i++)
                            {
                                AddMessage("You Also See: " + tileItems[i].itemName);
                            }
                        }
                    }
                    else
                    {
                        Furniture aFurn = currentLevel.GetFurniture(aX, aY);
                        if (aFurn != null)
                        {
                            seenSomething = true;
                            AddMessage(tileString + "You See: " + aFurn.name);
                        }
                        else
                        {
                            if (tileString != "")
                            {
                                AddMessage(tileString);
                            }
                        }
                    }
                    Creature possibleCreature = currentLevel.GetKnownCreature(aX, aY);
                    if (possibleCreature != null)
                    {
                        seenSomething = true;
                        String relativeStrength = possibleCreature.description.prefix + " ";
                        if (possibleCreature.allied)
                        {
                            if (possibleCreature.description.creatureType != (int)SC.CreatureTypes.QUESTRESCUEE)
                            {
                                relativeStrength = "an Allied ";
                            }
                        }
                        else
                        {
                            if (CreatureIsDangerous(possibleCreature))
                            {
                                if (possibleCreature.description.description == "")
                                {
                                    relativeStrength = "The Dangerous ";
                                }
                                else
                                {
                                    relativeStrength = "A Dangerous ";
                                }
                            }
                            else
                            {
                                if (CreatureIsPuny(possibleCreature))
                                {
                                    if (possibleCreature.description.description == "")
                                    {
                                        relativeStrength = "The Puny ";
                                    }
                                    else
                                    {
                                        relativeStrength = "A Puny ";
                                    }
                                }
                            }
                        }
                        if (possibleCreature.allied)
                        {
                            AddMessage("Lvl: " + possibleCreature.currentLevel + " Health: " + possibleCreature.currentHealth + "/" + possibleCreature.description.MAX_HEALTH);
                        }
                        AddMessage("There is " + relativeStrength + possibleCreature.description.description);
                    }
                    else
                    {
                        Trap possibleTrap = currentLevel.GetTrap(aX, aY);
                        if (possibleTrap != null)
                        {
                            if (possibleTrap.discovered && !possibleTrap.disarmed)
                            {
                                seenSomething = true;
                                switch (possibleTrap.trapType)
                                {
                                    case (int)SC.TrapType.ARROW:
                                        AddMessage("You See An Arrow Trap Here.", true);
                                        break;
                                    case (int)SC.TrapType.DESCENT:
                                        AddMessage("You See A Trap Of Wild Descent Here.", true);
                                        break;
                                    case (int)SC.TrapType.POISON:
                                        AddMessage("You See A Poison Gas Trap Here.", true);
                                        break;
                                    case (int)SC.TrapType.TELEPORT:
                                        AddMessage("You See A Teleporter Trap Here.", true);
                                        break;
                                    default:
                                        AddMessage("You See A Strange Trap Here...", true);
                                        break;
                                }
                            }
                            if (!possibleTrap.discovered)
                            {
                                AddMessage("Something Looks Amiss Here...");
                            }
                        }
                        if (!seenSomething)
                        {
                            if (currentLevel.GetTileVision(aX, aY) < 64)
                            {
                                if (currentLevel.GetTileVision(aX, aY) == 0)
                                {
                                    AddMessage("You Haven't Seen That Spot.");
                                }
                                else
                                {
                                    AddMessage("You Can't Be Sure What's There.");
                                }
                            }
                            else
                            {
                                AddMessage("You Don't See Anything!");
                            }
                        }
                    }

                    return false;   //Looking is free :)
                }
                if (aAction.ToString().Length > 9)
                {
                    if (aAction.ToString().Substring(0, 10) == "Build Wall")
                    {
                        if (player1.gold < VILLAGE_BUILD_WALL_COST)
                        {
                            AddMessage("You Can't Afford To Build That!", true);
                            sfxSelectBad.Play();
                            return false;
                        }
                        currentLevel.tiles[aX][aY] = TILE_VILLAGE_WALL;
                        sfxClub.Play();
                        player1.gold -= VILLAGE_BUILD_WALL_COST;
                    }
                }
                if (aAction.ToString().Length > 12)
                {
                    if (aAction.ToString().Substring(0, 13) == "Demolish Wall")
                    {
                        if (player1.gold < VILLAGE_DEMOLISH_WALL_COST)
                        {
                            AddMessage("You Can't Afford To Demolish That!", true);
                            sfxSelectBad.Play();
                            return false;
                        }
                        currentLevel.tiles[aX][aY] = TILE_VILLAGE_FLOOR;
                        sfxClub.Play();
                        player1.gold -= VILLAGE_DEMOLISH_WALL_COST;
                    }
                }
                if (aAction.ToString().Length > 9)
                {
                    if (aAction.ToString().Substring(0, 10) == "Build Door")
                    {
                        if (player1.gold < VILLAGE_BUILD_DOOR_COST)
                        {
                            AddMessage("You Can't Afford To Build A Door!", true);
                            sfxSelectBad.Play();
                            return false;
                        }
                        currentLevel.tiles[aX][aY] = TILE_VILLAGE_DOOR_CLOSED;
                        sfxClub.Play();
                        player1.gold -= VILLAGE_BUILD_DOOR_COST;
                    }
                }
                if (aAction.ToString().Length > 10)
                {
                    if (aAction.ToString().Substring(0, 11) == "Remove Door")
                    {
                        if (player1.gold < VILLAGE_DEMOLISH_DOOR_COST)
                        {
                            AddMessage("You Can't Afford To Remove The Door!", true);
                            sfxSelectBad.Play();
                            return false;
                        }
                        currentLevel.tiles[aX][aY] = TILE_VILLAGE_FLOOR;
                        sfxClub.Play();
                        player1.gold -= VILLAGE_DEMOLISH_DOOR_COST;
                    }
                }
                if (aAction.ToString() == "Dig")
                {
                    if (Math.Abs(aX - player1.x) > 1 || Math.Abs(aY - player1.y) > 1)
                    {
                        AddMessage("You Have To Stand Beside This Spot To Dig");
                        return false;
                    }
                    return PlayerActionDig(aX, aY);
                }
                if (aAction.ToString() == "Warp To Dungeon")
                {
                    if (currentQuest == null)
                    {
                        AddMessage("Speak To The Questmaster In The Adventurer's Guild.", true);
                        AddMessage("You Need To Choose A Quest First!", true);
                        sfxSelectBad.Play();
                        return false;
                    }
                    else
                    {
                        WarpToDungeon();
                        return false;
                    }
                }
                if (aAction.ToString() == "Return To Village")
                {
                    if (currentQuest.IsFinalQuest())
                    {
                        PlayingGameOutroTransition();
                    }
                    else
                    {
                        ReturnToVillage();
                    }
                }
                if (aAction.ToString() == "Open")
                {
                    return PlayerActionOpen(aX, aY);
                }
                if (aAction.ToString() == "Close")
                {
                    return PlayerActionClose(aX, aY);
                }
                if (aAction.ToString() == "Venture Down")
                {
                    isLevelComplete = true;
                    return true;
                }
                if (aAction.ToString() == "Pick Up...")
                {
                    if (Math.Abs(aX - player1.x) > 1 || Math.Abs(aY - player1.y) > 1)
                    {
                        AddMessage("You're Too Far Away!", true);
                        return false;
                    }
                    else
                    {
                        if (!currentLevel.CanMove(aX, aY))
                        {
                            AddMessage("Something is standing in the way...", true);
                            return false;
                        }
                        List<Item> tileItems = currentLevel.GetItems(aX, aY);
                        contextSubMenuStringList = new List<string>();
                        if (tileItems.Count > 0)
                        {
                            for (int i = 0; i < tileItems.Count; i++)
                            {
                                contextSubMenuStringList.Add(tileItems[i].itemName);
                            }
                        }
                        //Open the sub-menu to choose an item to pick-up
                        contextDialogSubMenuOpen = true;
                        //But this action closes the context menu!
                        //So we need to set it to open again now...
                        contextDialogOpen = true;
                        dialogOpen = true;
                        subMenuAction = "Pick Up...";
                        return false;
                    }
                }
                if (aAction.ToString() == "Attack!")
                {
                    if (Math.Abs(aX - player1.x) > player1.equippedSword.range
                        || Math.Abs(aY - player1.y) > player1.equippedSword.range)
                    {
                        AddMessage("You're Too Far Away!", true);
                        return false;
                    }
                    else
                    {
                        if (!currentLevel.CanSee(player1.x, player1.y, aX, aY))
                        {
                            AddMessage("You Can't Attack What You Can't See!", true);
                            return false;
                        }
                        Attack(aX, aY);
                        return true;
                    }
                }
                if (aAction.ToString() == "Kick")
                {
                    if (Math.Abs(aX - player1.x) > 1 || Math.Abs(aY - player1.y) > 1)
                    {
                        AddMessage("You're Too Far Away!", true);
                        return false;
                    }
                    else
                    {
                        Kick(aX, aY);
                        return true;
                    }
                }
                if (aAction.ToString() == "Trade...")
                {
                    if (Math.Abs(aX - player1.x) > 1 || Math.Abs(aY - player1.y) > 1)
                    {
                        AddMessage("You Need To Stand Beside The Merchant To Trade.", true);
                        return false;
                    }
                    Creature creature = currentLevel.GetCreature(aX, aY);
                    if (creature == null)
                    {
                        AddMessage("There's Nobody There To Trade With...", true);
                        return false;
                    }
                    if (creature.description.creatureType != (int)SC.CreatureTypes.MERCHANT)
                    {
                        AddMessage("It Doesn't Want To Trade!", true);
                        return true;
                    }
                    if (currentLevel.GetCreature(aX, aY).frozen)
                    {
                        AddMessage("The Merchant Is In No Condition To Trade Right Now!", true);
                        return false;
                    }
                    shopOpen = true;
                    lastShopMessage = "";
                    controller.Update(); //reset the last button state, else the game will read input for the shop
                    dialogOpen = true;
                    currentShopSelection = 0;
                    double sold = SellItems();
                    if (sold > 0)
                    {
                        lastShopMessage = "The Merchant Gave You " + sold + " Gold For Your Unwanted Items";
                        AddMessage(lastShopMessage);
                        sfxCoins.Play();
                    }
                }
                if (aAction.ToString().Length >= 6 && aAction.ToString().Substring(0, 6) == "Invest")
                {
                    if (Math.Abs(aX - player1.x) > 1 || Math.Abs(aY - player1.y) > 1)
                    {
                        AddMessage("You Need To Stand Closer To Do That!", true);
                        return false;
                    }
                    Creature creature = currentLevel.GetCreature(aX, aY);
                    if (creature == null)
                    {
                        AddMessage("There's Nobody There...", true);
                        return false;
                    }
                    if (creature.description.creatureType != (int)SC.CreatureTypes.MERCHANT)
                    {
                        AddMessage("It's Not Interested.", true);
                        return true;
                    }
                    if (villageMerchantLevel < MAX_VILLAGE_MERCHANT_LEVEL)
                    {
                        if (player1.gold >= villageMerchantLevel * GOLD_PER_VILLAGE_MERCHANT_LEVEL)
                        {
                            player1.gold -= villageMerchantLevel * GOLD_PER_VILLAGE_MERCHANT_LEVEL;
                            runStats.IncreaseStat("Gold Invested", villageMerchantLevel * GOLD_PER_VILLAGE_MERCHANT_LEVEL);
                            villageMerchantLevel++;
                            sfxRecruit.Play();
                            villageMerchantItems = itemGenerator.VillageMerchantInventory(villageMerchantLevel, 7);
                            return true;
                        }
                        else
                        {
                            AddMessage("You Can't Afford The Investment.", true);
                            sfxSelectBad.Play();
                            return false;
                        }
                    }
                }
                if (aAction.ToString() == "Chat")
                {
                    Creature creature = currentLevel.GetCreature(aX, aY);
                    if (creature == null)
                    {
                        AddMessage("There's Nothing There To Chat With...", true);
                        return false;
                    }
                    if (Math.Abs(aX - player1.x) > 2 || Math.Abs(aY - player1.y) > 2
                        || !currentLevel.CanSee(player1.x, player1.y, aX, aY))
                    {
                        AddMessage("You Need To Stand Closer To Chat...", true);
                        return false;
                    }
                    if (creature.frozen)
                    {
                        if (creature.description.prefix == "")
                        {
                            AddMessage(creature.description.description + " Is In No Condition To Chat!", true);
                        }
                        else
                        {
                            AddMessage("The " + creature.description.description + " Is In No Condition To Chat!", true);
                        }
                        return false;
                    }
                    Creature sighting = new Creature(); //Need to initialize; C# not happy with testing if null
                    if (currentLevel.creatureList.Count > 0)
                    {
                        for (int i = 0; i < currentLevel.creatureList.Count; i++)
                        {
                            if (!currentLevel.creatureList[i].seenByPlayer && currentLevel.creatureList[i].hostile) sighting = currentLevel.creatureList[i];
                        }
                    }
                    if (creature.description.creatureType == (int)SC.CreatureTypes.MERCHANT)
                    {
                        if (random.NextDouble() < 0.3 && sighting.description.name != "UNINITIALIZED NAME")
                        {
                            AddMessage("Be Careful " + player1.name + ", I Saw " + sighting.description.prefix + " " + sighting.description.description + "!");
                            return true;
                        }
                        else
                        {
                            AddMessage("\"Greetings! Here To Trade?\"");
                            return true;
                        }
                    }
                    if (creature.description.creatureType == (int)SC.CreatureTypes.HUMAN)
                    {
                        if (random.NextDouble() < 0.3 && sighting.description.name != "UNINITIALIZED NAME")
                        {
                            AddMessage("Be Careful " + player1.name + ", I Saw " + sighting.description.prefix + " " + sighting.description.description + "!");
                            return true;
                        }
                        else
                        {
                            if (!creature.allied)
                            {
                                AddMessage("\"I'll Join You For " + AdjustedRecruitCost(creature).ToString() + " Gold\"");
                            }
                            else
                            {
                                AddMessage("\"Lead The Way, Boss!\"");
                            }
                        }
                    }
                    if (creature.description.creatureType == (int)SC.CreatureTypes.GHOST)
                    {
                        if (random.NextDouble() < 0.3)
                        {
                            AddMessage("The " + creature.description.description + " Moans \"Rooooooooosebud\"");
                            return true;
                        }
                        if (random.NextDouble() < 0.3)
                        {
                            AddMessage("The " + creature.description.description + " Wails \"RAWWR U BOO\"!");
                            return true;
                        }
                        AddMessage("The " + creature.description.description + " Ignores You...");
                        return true;
                    }
                    if (creature.description.creatureType == (int)SC.CreatureTypes.CHEF)
                    {
                        if (random.NextDouble() < 0.3 && sighting.description.name != "UNINITIALIZED NAME")
                        {
                            AddMessage("Be Careful " + player1.name + ", I Saw " + sighting.description.prefix + " " + sighting.description.description + "!");
                            return true;
                        }
                        else
                        {
                            if (!creature.allied)
                            {
                                AddMessage("\"I'll Join You For " + AdjustedRecruitCost(creature).ToString() + " Gold\"");
                            }
                            else
                            {
                                AddMessage("\"Where Now? Find Me Something To Cook!\"");
                            }
                            return true;
                        }
                    }
                    if (creature.description.creatureType == (int)SC.CreatureTypes.QUESTRESCUEE)
                    {
                        if (!creature.allied)
                        {
                            AddMessage("\"Please, Get Me Out Of Here!!\"");
                        }
                        else
                        {
                            if (random.NextDouble() < 0.3)
                            {
                                AddMessage("\"Let's Go! Hurry!\"");
                                return true;
                            }
                            if (random.NextDouble() < 0.3 && sighting.description.name != "UNINITIALIZED NAME")
                            {
                                AddMessage("Be Careful " + player1.name + ", I Saw " + sighting.description.prefix + " " + sighting.description.description + "!");
                                return true;
                            }
                            AddMessage("\"Get Me To The Exit Teleporter, Please!\"");
                            return true;
                        }
                    }
                    if (creature.description.creatureType == (int)SC.CreatureTypes.VILLAGER)
                    {
                        AddMessage(RandomVillagerResponse());
                        return true;
                    }
                    if (creature.description.creatureType == (int)SC.CreatureTypes.QUESTMASTER)
                    {
                        if (currentQuest == null)
                        {
                            questListOpen = true;
                            questForfeitOpen = false;
                            questTurnInOpen = false;
                            controller.Update(); //reset the last button state, else the game will read input for the quest dialog
                            dialogOpen = true;
                            currentQuestListSelection = 0;
                        }
                        else
                        {
                            if (currentQuest.completed)
                            {
                                questListOpen = false;
                                questForfeitOpen = false;
                                questTurnInOpen = true;
                                controller.Update(); //reset the last button state, else the game will read input for the quest dialog
                                dialogOpen = true;
                            }
                            else
                            {
                                if (currentQuest.failed)
                                {
                                    questListOpen = false;
                                    questForfeitOpen = false;
                                    questTurnInOpen = false;
                                    questFailedOpen = true;
                                    controller.Update(); //reset the last button state, else the game will read input for the quest dialog
                                    dialogOpen = true;
                                }
                                else
                                {
                                    questListOpen = false;
                                    questForfeitOpen = true;
                                    questTurnInOpen = false;
                                    controller.Update(); //reset the last button state, else the game will read input for the quest dialog
                                    dialogOpen = true;
                                }
                            }
                        }
                    }
                }

                if (aAction.ToString() == "Recruit")
                {

                    Creature recruit = currentLevel.GetCreature(aX, aY);
                    if (recruit == null || recruit.description.creatureType == (int)SC.CreatureTypes.GHOST)
                    {
                        AddMessage("Nobody To Recruit There!", true);
                        return false;
                    }
                    else
                    {
                        if (recruit.hostile)
                        {
                            AddMessage("The " + recruit.description.description + " Is Hostile!", true);
                            return false;
                        }
                        if (recruit.description.creatureType == (int)SC.CreatureTypes.VILLAGER)
                        {
                            AddMessage("\"No, Not Me! I Can't Fight!\"");
                            return false;
                        }
                        if (recruit.description.creatureType == (int)SC.CreatureTypes.QUESTMASTER)
                        {
                            AddMessage("\"I'm Too Getting A Little Too Old For Dungeons, Sorry.\"");
                            return false;
                        }
                        if (recruit.description.creatureType == (int)SC.CreatureTypes.QUESTRESCUEE)
                        {
                            AddMessage("\"Are You Kidding? GET ME OUT OF HERE!\"", true);
                            return false;
                        }
                        if (Math.Abs(recruit.x - player1.x) > 1 || Math.Abs(recruit.x - player1.x) > 1)
                        {
                            AddMessage("You Need To Stand Next To The " + recruit.description.description + " To Recruit", true);
                            return false;
                        }

                        if (recruit.description.creatureType == (int)SC.CreatureTypes.MERCHANT)
                        {
                            AddMessage("\"Sorry, I Need To Remain Impartial!\"");
                            return false;
                        }
                        else
                        {

                            if (player1.gold < AdjustedRecruitCost(recruit))
                            {
                                AddMessage("You Can't Afford " + AdjustedRecruitCost(recruit).ToString() + " Gold", true);
                                sfxSelectBad.Play();
                                return false;
                            }
                            else
                            {
                                player1.RemoveGold(AdjustedRecruitCost(recruit));
                                recruit.allied = true;
                                sfxRecruit.Play();
                                if (recruit.description.attacksWhenAllied)
                                {
                                    AddMessage("The " + recruit.description.description + " Is Now Fighting By Your Side!");
                                }
                                else
                                {
                                    AddMessage("The " + recruit.description.description + " Is Now Following You.");
                                }
                                //Need to move the new recruit to the back of the creature list
                                //This is so that the game always recognises when another creature has hit it
                                Creature temp = recruit;
                                currentLevel.creatureList.Remove(recruit);
                                currentLevel.creatureList.Add(recruit);
                                runStats.IncrementStat("Creatures Recruited");
                                IncreaseFeat(SC.FEAT_RECRUIT_ALLIES, 1);
                                return true;
                            }
                        }
                    }
                }

                if (aAction.ToString() == "Rescue")
                {
                    return PlayerCommandRescue(aX, aY);
                }

                if (aAction.ToString() == "Switch Places")
                {
                    if (Math.Abs(aX - player1.x) > 1 || Math.Abs(aY - player1.y) > 1)
                    {
                        AddMessage("You Can Only Swap Places With A Creature Standing Beside You", true);
                        return false;
                    }
                    Creature possibleCreature = currentLevel.GetCreature(aX, aY);
                    if (possibleCreature == null)
                    {
                        AddMessage("No Creature There!", true);
                        return false;
                    }
                    return SwitchPlaces(possibleCreature);
                }

                if (aAction.ToString() == "Wait There")
                {
                    return PlayerCommandWait(aX, aY);
                }

                if (aAction.ToString() == "Follow")
                {
                    return PlayerCommandFollow(aX, aY);
                }

                if (aAction.ToString() == "Disarm Trap")
                {
                    return PlayerActionDisarmTrap(aX, aY);
                }

                if (aAction.ToString() == "Pray")
                {
                    PrayAtAltar();
                    return true;
                }
            }
            return false;
        }

        //A method to handle player actions when Left Trigger is pulled
        public bool PlayerAction(int aX, int aY)
        {
            Creature possibleCreature = currentLevel.GetCreature(aX, aY);
            
            //If Fighter
            if (player1.profession == (int)SC.Professions.FIGHTER)
            {
                //If pointing at enemy and hostile, attack
                if (possibleCreature != null && possibleCreature.hostile)
                {
                    if (Math.Abs(aX - player1.x) <= player1.equippedSword.range
                        && Math.Abs(aY - player1.y) <= player1.equippedSword.range
                        && currentLevel.CanSee(player1.x, player1.y, aX, aY))
                    {
                        Attack(aX, aY);
                        return true;
                    }
                }

                //If standing adjacent and hostile, attack!
                if (currentLevel.GetHostileCreaturePointNextTo(player1.x, player1.y).X != -1)
                {
                    Point hostile = currentLevel.GetHostileCreaturePointNextTo(player1.x, player1.y);
                    Attack(hostile.X, hostile.Y);
                    return true;
                }

                //If not adjacent but in line of fire, attack if in range or else try to cast Freeze

                if (PlayerLookingInSomeDirection())
                {
                    int possibleCreatureIndex = CreaturePlayerIsPointingAt(playerLookAtX - player1.x, playerLookAtY - player1.y);
                    if (possibleCreatureIndex != -1)
                    {
                        if (currentLevel.creatureList[possibleCreatureIndex].hostile)
                        {
                            if (PlayerWillBeAbleToHit(currentLevel.creatureList[possibleCreatureIndex]))
                            {
                                Attack(currentLevel.creatureList[possibleCreatureIndex].x, currentLevel.creatureList[possibleCreatureIndex].y);
                                return true;
                            }
                            if (player1.spells.IsSpellKnown("Freeze") && player1.WillBeAbleToCast("Freeze"))
                            {
                                CastSpell("Freeze", aX, aY);
                                return true;
                            }
                            else
                            {
                                if (player1.spells.IsSpellKnown("Freeze"))
                                {
                                    AddMessage("You Aren't Able To Cast Freeze!", true);
                                    sfxSelectBad.Play();
                                    return false;
                                }
                            }
                        }
                    }
                }

                //If aiming at self and Magic > 90% and Health < 60%, Heal
                if (!PlayerLookingInSomeDirection() && player1.WillBeAbleToCast("Heal") && !inVillage)
                {
                    if (player1.currentHealth < player1.MAX_HEALTH * 0.6)
                    {
                        if (player1.currentMagik >= player1.MAX_MAGIK * 0.95)
                        {
                            CastSpell("Heal", aX, aY);
                            return true;
                        }
                    }
                }
            }

            //If Mage
            if (player1.profession == (int)SC.Professions.MAGE)
            {
                //If looking distantly and hostile on the tile, Magic Missile
                if (controller.LeftShoulderIsHeld())
                {
                    if (possibleCreature != null && possibleCreature.hostile
                        && currentLevel.CanSee(player1.x, player1.y, aX, aY))
                    {
                        if (player1.spells.IsSpellKnown("Magic Missile") && player1.WillBeAbleToCast("Magic Missile"))
                        {
                            CastSpell("Magic Missile", aX, aY);
                            return true;
                        }
                        else
                        {
                            if (player1.spells.IsSpellKnown("Magic Missile"))
                            {
                                AddMessage("You Haven't Enough " + magicName + "To Cast Magic Missile!", true);
                                sfxSelectBad.Play();
                                return false;
                            }
                        }
                    }
                }
                //If hostile adjacent or in line of fire, cast Fire ball
                if (player1.WillBeAbleToCast("Fireball"))
                {
                    if (PlayerLookingInSomeDirection())
                    {
                        int possibleCreatureIndex = CreaturePlayerIsPointingAt(playerLookAtX - player1.x, playerLookAtY - player1.y);
                        if (possibleCreatureIndex != -1)
                        {
                            if (currentLevel.creatureList[possibleCreatureIndex].hostile)
                            {
                                CastSpell("Fireball", aX, aY);
                                return true;
                            }
                        }
                    }
                }

                //If hostile adjacent, attack
                if (possibleCreature != null && possibleCreature.hostile)
                {
                    if (Math.Abs(aX - player1.x) <= 1 && Math.Abs(aY - player1.y) <= 1)
                    {
                        Attack(aX, aY);
                        return true;
                    }
                }

                //If aiming at self and Magic > 90% and Health < 60%, Heal
                if (!PlayerLookingInSomeDirection() && player1.WillBeAbleToCast("Heal") && !inVillage)
                {
                    if (player1.currentHealth < player1.MAX_HEALTH * 0.6)
                    {
                        if (player1.currentMagik >= player1.MAX_MAGIK * 0.95)
                        {
                            CastSpell("Heal", aX, aY);
                            return true;
                        }
                    }
                }
            }

            //If Explorer
            if (player1.profession == (int)SC.Professions.EXPLORER)
            {

                //If pointing at enemy and hostile, attack
                if (possibleCreature != null && possibleCreature.hostile)
                {
                    if (Math.Abs(aX - player1.x) <= player1.equippedSword.range
                        && Math.Abs(aY - player1.y) <= player1.equippedSword.range
                        && currentLevel.CanSee(player1.x, player1.y, aX, aY))
                    {
                        Attack(aX, aY);
                        return true;
                    }
                }

                //If not adjacent but in line of fire, attack if in range
                if (PlayerLookingInSomeDirection())
                {
                    int possibleCreatureIndex = CreaturePlayerIsPointingAt(playerLookAtX - player1.x, playerLookAtY - player1.y);
                    if (possibleCreatureIndex != -1)
                    {
                        if (currentLevel.creatureList[possibleCreatureIndex].hostile)
                        {
                            if (Math.Abs(player1.x - currentLevel.creatureList[possibleCreatureIndex].x) <= player1.equippedSword.range
                                && Math.Abs(player1.y - currentLevel.creatureList[possibleCreatureIndex].y) <= player1.equippedSword.range
                                && currentLevel.CanSee(player1.x, player1.y, currentLevel.creatureList[possibleCreatureIndex].x, currentLevel.creatureList[possibleCreatureIndex].y))
                            {
                                Attack(currentLevel.creatureList[possibleCreatureIndex].x, currentLevel.creatureList[possibleCreatureIndex].y);
                                return true;
                            }
                        }
                    }
                }

                //If wall, dig!
                if (!inVillage && currentLevel.GetTile(aX, aY) == TILE_WALL)
                {
                    if (currentLevel.CanDig(aX, aY))
                    {
                        return PlayerActionDig(aX, aY);
                    }
                    else
                    {
                        AddMessage("This Wall Can't Be Dug Out.", true);
                        sfxSelectBad.Play();
                        return false;
                    }
                }

                //If standing adjacent and hostile, attack!
                if (currentLevel.GetHostileCreaturePointNextTo(player1.x, player1.y).X != -1)
                {
                    Point hostile = currentLevel.GetHostileCreaturePointNextTo(player1.x, player1.y);
                    Attack(hostile.X, hostile.Y);
                    return true;
                }

            } // End Explorer custom actions

            //If trap present, try and disarm
            if (currentLevel.GetTrap(aX, aY) != null)
            {
                Trap trap = currentLevel.GetTrap(aX, aY);
                if (trap.discovered && !trap.disarmed)
                {
                    return PlayerActionDisarmTrap(aX, aY);
                }
            }

            //Non-hostile creature
            if (possibleCreature != null)
            {
                //If allied creature, toggle Follow/Wait
                if (possibleCreature.allied)
                {
                    if (possibleCreature.waiting)
                    {
                        return PlayerCommandFollow(aX, aY);
                    }
                    else
                    {
                        return PlayerCommandWait(aX, aY);
                    }
                }

                //If quest rescuee and not yet allied, Rescue
                if (possibleCreature.description.creatureType == (int)SC.CreatureTypes.QUESTRESCUEE && !possibleCreature.allied)
                {
                    return PlayerCommandRescue(aX, aY);
                }

                //If non-hostile but not allied, Switch Places
                if (!possibleCreature.hostile && !possibleCreature.allied)
                {
                    if (possibleCreature.description.creatureType == (int)SC.CreatureTypes.QUESTMASTER
                        || (inVillage && possibleCreature.description.creatureType == (int)SC.CreatureTypes.MERCHANT))
                    {
                        return PlayerAction("Chat", aX, aY);
                    }
                    else
                    {
                        return SwitchPlaces(possibleCreature);
                    }
                }
            }

            //If closed door, open
            if ((inVillage && currentLevel.GetTile(aX, aY) == TILE_VILLAGE_DOOR_CLOSED)
                || (!inVillage && currentLevel.GetTile(aX, aY) == TILE_DOOR_CLOSED))
            {
                return PlayerActionOpen(aX, aY);
            }

            //If open door and can close, close.
            //NOTE - If can't close, continue with this method - action for entity blocking the door should be considered
            if ((inVillage && currentLevel.GetTile(aX, aY) == TILE_VILLAGE_DOOR_OPEN)
                || (!inVillage && currentLevel.GetTile(aX, aY) == TILE_DOOR_OPEN))
            {
                if (PlayerActionClose(aX, aY)) return true;
            }

            //If item underfoot and "good", pick it up
            if (currentLevel.GetItems(aX, aY).Count > 0
                && aX == player1.x && aY == player1.y)
            {
                Item possibleItem = currentLevel.GetItem(0, aX, aY);
                //Ignore rotten food
                if (possibleItem.itemType != (int)SC.ItemTypes.FOOD || !possibleItem.rotten)
                {
                    if (player1.WillBeAbleToCarry(possibleItem))
                    {
                        return PickUp(0, aX, aY);
                    }
                }
            }

            Furniture possibleFurn = currentLevel.GetFurniture(aX, aY);
            if (possibleFurn != null)
            {
                //If teleporter underfoot, use it
                if (aX == player1.x && aY == player1.y)
                {
                    if (possibleFurn.furnType == (int)SC.FurnTypes.TELEPORTER)
                    {
                        if (inVillage)
                        {
                            if (currentQuest == null)
                            {
                                AddMessage("Speak To The Questmaster In The Adventurer's Guild.", true);
                                AddMessage("You Need To Choose A Quest First!", true);
                                sfxSelectBad.Play();
                                return false;
                            }
                            else
                            {
                                WarpToDungeon();
                                return false;
                            }
                        }
                        else
                        {
                            if (currentQuest.IsFinalQuest())
                            {
                                PlayingGameOutroTransition();
                            }
                            else
                            {
                                ReturnToVillage();
                            }
                            return false;
                        }
                    }
                }
                if (possibleFurn.visibility > 0
                    && Math.Abs(player1.x - possibleFurn.x) <= 1
                    && Math.Abs(player1.y - possibleFurn.y) <= 1)
                {
                    if (possibleFurn.inscribed)
                    {
                        AddMessage("\"" + possibleFurn.inscription + "\"");
                        AddMessage("You read the inscription on the " + possibleFurn.name + ":");
                        return true;
                    }
                    //Note: Could insert auto-use of altar here? Not going to because it's not a "safe" action
                }
            }

            //Default, rest.
            return PlayerActionRest(inVillage);
        }

        private bool PlayerActionRest(bool isInVillage)
        {
            player1.Rest(isInVillage);
            return true;
        }

        private bool PlayerActionDisarmTrap(int aX, int aY)
        {
            if (currentLevel.GetTrap(aX, aY) == null)
            {
                AddMessage("There Is No Trap To Disarm!", true);
                return false;
            }
            if (Math.Abs(aX - player1.x) > 1 || Math.Abs(aY - player1.y) > 1)
            {
                AddMessage("You Need To Stand Beside The Trap To Disarm It.", true);
                return false;
            }
            double disarmRoll = random.NextDouble();
            if (player1.profession == (int)SC.Professions.EXPLORER)
            {
                disarmRoll = disarmRoll / 4;
            }
            if (disarmRoll < CHANCE_TO_DISARM_TRAP)
            {
                currentLevel.GetTrap(aX, aY).disarmed = true;
                AddMessage("You Deftly Disarm The Trap! (Bonus Experience Awarded)");
                runStats.IncrementStat("Traps Disarmed");
                sfxRecruit.Play();
                AddExperience(20);
                return true;
            }
            else
            {
                AddMessage("You Poke Around At The Trap's Mechanisms In Vain", true);
                sfxFail.Play();
                //TODO - Chance to trigger trap at spot
                return true;
            }
        }

        private bool PlayerActionOpen(int aX, int aY)
        {
            if (Math.Abs(aX - player1.x) > 1 || Math.Abs(aY - player1.y) > 1)
            {
                AddMessage("You're Too Far Away!", true);
                return false;
            }
            else
            {
                if (currentLevel.Open(aX, aY))
                {
                    //AddMessage("The door creaks open...");
                    if (tutorialMessages[8].shown) ShowTutorialMessage(15);
                    return true;
                }
                else
                {
                    AddMessage("It won't open!", true);
                    return false;
                }
            }
        }

        private bool PlayerActionClose(int aX, int aY)
        {
            if (Math.Abs(aX - player1.x) > 1 || Math.Abs(aY - player1.y) > 1)
            {
                AddMessage("You're Too Far Away!", true);
                return false;
            }
            else
            {
                if (aX == player1.x && aY == player1.y)
                {
                    AddMessage("You Can't Close It While You're In The Doorway", true);
                    return false;
                }
                else
                {
                    if (currentLevel.Close(aX, aY))
                    {
                        AddMessage("You Close The Door.");
                        return true;
                    }
                    else
                    {
                        AddMessage("You Can't Close It!");
                        return false;
                    }
                }
            }
        }

        private bool PlayerActionDig(int aX, int aY)
        {
            if (!currentLevel.CanDig(aX, aY))
            {
                AddMessage("You Find You Can't Dig Through Here", true);
                player1.Digest();
                player1.Digest();
                player1.Digest();
                player1.Digest();
                player1.Digest();
                player1.Digest();
                return true;
            }
            else
            {
                currentLevel.Dig(aX, aY);
                AddMessage("You Smash Through The Wall");
                player1.Digest();
                player1.Digest();
                runStats.IncrementStat("Spaces Dug");
                return true;
            }
        }

        private bool PlayerCommandWait(int aX, int aY)
        {
            Creature possibleCreature = currentLevel.GetCreature(aX, aY);
            if (possibleCreature == null)
            {
                AddMessage("No Creature There!");
                return false;
            }
            if (!possibleCreature.allied)
            {
                AddMessage("The " + possibleCreature.description.description + " Doesn't Take Orders From You");
            }
            else
            {
                possibleCreature.waiting = true;
                AddMessage("The " + possibleCreature.description.description + " Will Hold Its Position");
            }
            return true;
        }

        private bool PlayerCommandFollow(int aX, int aY)
        {
            Creature possibleCreature = currentLevel.GetCreature(aX, aY);
            if (possibleCreature == null)
            {
                AddMessage("No Creature There!");
                return false;
            }
            if (!possibleCreature.allied)
            {
                AddMessage("The " + possibleCreature.description.description + " Doesn't Take Orders From You");
            }
            else
            {
                possibleCreature.waiting = false;
                AddMessage("The " + possibleCreature.description.description + " Is Now Following You Again");
            }
            return true;
        }

        private bool PlayerCommandRescue(int aX, int aY)
        {
            Creature recruit = currentLevel.GetCreature(aX, aY);
            if (recruit == null || recruit.description.creatureType != (int)SC.CreatureTypes.QUESTRESCUEE)
            {
                AddMessage("Nobody To Rescue There!", true);
                return false;
            }
            else
            {
                if (recruit.hostile)
                {
                    AddMessage("The " + recruit.description.description + " Is Hostile!", true);
                    return false;
                }
                if (Math.Abs(recruit.x - player1.x) > 1 || Math.Abs(recruit.x - player1.x) > 1)
                {
                    AddMessage("You Need To Stand Next To The " + recruit.description.description + " To Rescue", true);
                    return false;
                }
                recruit.allied = true;
                sfxRecruit.Play();
                AddMessage("The " + recruit.description.description + " Will Now Follow You To Safety");
                return true;
            }
        }

        private bool SwitchPlaces(Creature creature)
        {
            if (creature.hostile)
            {
                AddMessage("You Can't Switch Places With A Hostile Creature!", true);
                return false;
            }
            if (creature.description.width > 1 || creature.description.height > 1)
            {
                AddMessage("The " + creature.description.description + " Is Too Big To Move!", true);
                return false;
            }
            int tmpX = player1.x;
            int tmpY = player1.y;
            player1.x = creature.x;
            player1.y = creature.y;
            creature.x = tmpX;
            creature.y = tmpY;
            AddMessage("You Switch Places With The " + creature.description.description);
            sfxStep.Play();
            return true;
        }

        private void PrayAtAltar()
        {
            if (!currentLevel.playerHasPrayed)
            {
                runStats.IncrementStat("Altars Prayed At");
                if (player1.profession == (int)SC.Professions.MAGE)
                {
                    player1.currentMagik = player1.MAX_MAGIK;
                    sfxMagicHeal.Play();
                    AddMessage("Your " + magicName + " Has Been Completely Refilled!");
                }
                if (player1.profession == (int)SC.Professions.FIGHTER)
                {
                    player1.attackBuff = player1.UnbuffedAttackPower();
                    player1.attackBuffTurnsLeft = 400;
                    sfxMagicHeal.Play();
                    AddMessage("You Feel Mighty!");
                }
                ExecuteGoodKarmaEvent();
            }
            else
            {
                ExecuteBadKarmaEvent();
            }
            currentLevel.playerHasPrayed = true;
        }

        //Execute a random good karma event
        //Return if something good happened or not
        private bool ExecuteGoodKarmaEvent()
        {
            bool result = false;
            bool sfxPlayed = false;
            if (random.NextDouble() < 0.3)
            {
                player1.hunger = player1.full_stomach;
                if (!sfxPlayed) sfxMagicHeal.Play();
                sfxPlayed = true;
                AddMessage("Your Belly Is Filled!");
                result = true;
            }
            if (random.NextDouble() < 0.3)
            {
                player1.currentHealth = player1.MAX_HEALTH;
                if (!sfxPlayed) sfxMagicHeal.Play();
                sfxPlayed = true;
                AddMessage("You Feel Renewed.");
                return true;
            }
            if (random.NextDouble() < 0.3)
            {
                currentLevel.KnowCreatures();
                currentLevel.KnowDoors();
                currentLevel.KnowExit();
                currentLevel.KnowFood();
                currentLevel.KnowItems();
                currentLevel.KnowTraps();
                AddMessage("Your Mind Warms With The Blessing Of Cogniscience");
                if (!sfxPlayed) sfxMagicHeal.Play();
                sfxPlayed = true;
                return true;
            }
            if (random.NextDouble() < 0.4)
            {
                double exp = currentLevel.KillRandomHostile();
                if (exp > 0)
                {
                    AddExperience(exp);
                    if (!sfxPlayed) sfxMagicHeal.Play();
                    sfxPlayed = true;
                    AddMessage("One Of Your Foes Has Been Smited...");
                    return true;
                }
            }
            if (random.NextDouble() < 0.4)
            {
                double exp = currentLevel.KillAllHostiles();
                if (exp > 0)
                {
                    AddExperience(exp);
                    if (!sfxPlayed) sfxMagicHeal.Play();
                    sfxPlayed = true;
                    AddMessage("ANNIHILATION!!!");
                    return true;
                }
            }
            return result;
        }

        private void ExecuteBadKarmaEvent()
        {
            bool sfxPlayed = false;

            if (random.NextDouble() < 0.1)
            {
                AddMessage("Nothing Happens...");
                return;
            }
            if (random.NextDouble() < 0.2)
            {
                PoisonPlayerScaledToLevel();
                AddMessage("You Feel Suddenly Sick...", true);
                if (!sfxPlayed) sfxFail.Play();
                sfxPlayed = true;
                return;
            }
            //Starve the player
            if (random.NextDouble() < 0.2)
            {
                player1.ReduceStomachTo10Percent();
                AddMessage("Your Stomach Feels Empty...", true);
                if (!sfxPlayed) sfxFail.Play();
                sfxPlayed = true;
                return;
            }
            //Cut the player's HP
            if (random.NextDouble() < 0.2)
            {
                player1.LimitedDamage(Math.Floor(player1.MAX_HEALTH / 3));
                AddMessage("You Feel Spasms Of Pain!", true);
                if (!sfxPlayed) sfxFail.Play();
                sfxPlayed = true;
                return;
            }

            //Summon a daemon
            if (random.NextDouble() < 0.3)
            {
                if (currentLevel.SummonAltarDaemonNearPlayer(player1.x, player1.y))
                {
                    currentLevel.UpdateVision(player1.x, player1.y, player1.perception);
                    AddMessage("Uh Oh.", true);
                    if (!sfxPlayed) sfxFail.Play();
                    sfxPlayed = true;
                }
            }

            //TRAP MANIA!
            if (random.NextDouble() < 0.3)
            {
                currentLevel.TrapMania();
                AddMessage("You Have A Bad Feeling About This...", true);
                sfxDanger.Play();
                return;
            }

            AddMessage("Nothing Happens...");
        }

        private String RandomVillagerResponse()
        {
            return "\"" + villagerResponses[random.Next(villagerResponses.Count)] + "\"";
        }

        private void WarpToDungeon()
        {
            sfxTeleport.Play();
            inVillage = false;
            currentLevel = dungeonLevel;
            player1.x = currentLevel.startingX;
            player1.y = currentLevel.startingY;
            UpdateVision();
            PlayBGM(dungeonMusic);
        }

        private void ReturnToVillage()
        {
            sfxTeleport.Play();
            inVillage = true;
            if (!currentQuest.failed && currentQuest.questType == (int)SC.QuestType.RESCUE)
            {
                if (currentQuest.questCreature.allied && currentLevel.CanSee(player1.x, player1.y, currentQuest.questCreature.x, currentQuest.questCreature.y))
                {
                    currentQuest.completed = true;
                    ShowTutorialMessage(6);
                    AddMessage("You Have Led The " + currentQuest.questCreature.description.description + " Safely Back To The Village.");
                    if (dungeonLevel.creatureList.Contains(currentQuest.questCreature))
                        dungeonLevel.creatureList.Remove(currentQuest.questCreature);
                }
            }
            currentLevel = villageLevel;
            player1.x = currentLevel.startingX;
            player1.y = currentLevel.startingY;
            player1.ClearPoison();
            UpdateVision();
            PlayBGM(villageMusic);
        }

        private void AcceptQuest(Quest aQuest)
        {
            currentQuest = aQuest;
            dungeonLevel = new Level(aQuest.level, aQuest, itemGenerator, bestiary);
            dungeonLevel.PlaceItems(LootItemsStillUncollected());
            if (player1.profession == (int)SC.Professions.EXPLORER) dungeonLevel.KnowExit();
            //lootItemsFromLastPlayer = null;
            AddMessage("Good Luck, Adventurer!");
            ShuffleDungeonBGM();
            ShowTutorialMessage(4);
        }

        private void TurnInQuest()
        {
            runStats.IncrementStat("Quests Completed");
            if (runStats.GetStatValue("Quests Completed") > 1) ShowTutorialMessage(7);
            IncreaseFeat(SC.FEAT_COMPLETE_QUESTS, 1);
            if (player1.profession == (int)SC.Professions.EXPLORER) IncreaseFeat(SC.FEAT_COMPLETE_EXPLORER_QUESTS, 1);
            if (player1.profession == (int)SC.Professions.FIGHTER) IncreaseFeat(SC.FEAT_COMPLETE_FIGHTER_QUESTS, 1);
            if (player1.profession == (int)SC.Professions.MAGE) IncreaseFeat(SC.FEAT_COMPLETE_MAGE_QUESTS, 1);
            villageMerchantItems = itemGenerator.VillageMerchantInventory(villageMerchantLevel, 7);
            player1.currentHealth = player1.MAX_HEALTH;
            player1.currentMagik = player1.MAX_MAGIK;
            player1.hunger = player1.full_stomach;
            if (currentQuest.isBossQuest)
            {
                for (int i = 0; i < bossList.Count; i++)
                {
                    if (bossList[i].level == currentQuest.level)
                    {
                        if (bossList[i].beaten == false)
                        {
                            totalStats.IncrementStat("Bosses Killed");
                            IncreaseFeat(SC.FEAT_KILL_BOSSES, 1);
                            bossList[i].beaten = true;
                        }
                    }
                }
            }
            player1.RemoveQuestItemsFromInventory();
            if (currentQuest.rewardExp > 0)
            {
                AddExperience(currentQuest.rewardExp);
            }
            if (currentQuest.rewardGold > 0)
            {
                player1.AddGold(currentQuest.rewardGold);
            }
            if (currentQuest.rewardItem != null)
            {
                if (!player1.WillBeAbleToCarry(currentQuest.rewardItem))
                {
                    if (currentLevel.DropItem(currentQuest.rewardItem, player1.x, player1.y))
                    {
                        AddMessage("You Can't Carry Your Reward! It's On The Ground", true);
                    }
                    else
                    {
                        AddMessage("You Can't Carry Your Reward And There Was No Room On The Ground!!!", true);
                    }
                }
                else
                {
                    player1.AddItem(currentQuest.rewardItem);
                }
            }
            //Increase player's highest available quest level. Note that if boss level, we move on to
            //a new boss progress phase; set progress at a little over the defeated boss' level
            if (currentQuest.isBossQuest)
            {
                MAX_STARTING_QUEST_LEVEL = Math.Max(MAX_STARTING_QUEST_LEVEL + 1, currentQuest.level + 1);
            }
            else
            {
                if (!QuestListContainsBoss())
                {
                    MAX_STARTING_QUEST_LEVEL = Math.Max(MAX_STARTING_QUEST_LEVEL + 1, currentQuest.level + currentQuest.numFloors - 4);
                }
            }
            RemoveQuestFromQuestList(currentQuest);
            if (dungeonLevel.AllyExists())
            {
                AddMessage("Your Allies In The Dungeon Must Be Left Behind", true);
            }
            currentQuest = null;
            AddNewQuestToQuestList();
            //Remove any quest items
            if (player1.carriedItems.Count > 0)
            {
                for (int i = player1.carriedItems.Count - 1; i >= 0; i--)
                {
                    if (player1.carriedItems[i].itemType == (int)SC.ItemTypes.QUEST) player1.DropItem(player1.carriedItems[i]);
                }
            }
            if (!cheating && notCurrentlySaving) SaveSettings();
        }

        private void ForfeitQuest()
        {
            if (currentQuest != null)
            {
                player1.gold = Math.Max(0, player1.gold - currentQuest.forfeitGold);
                if (!currentQuest.isBossQuest)
                {
                    RemoveQuestFromQuestList(currentQuest);
                    AddNewQuestToQuestList();
                }
                currentQuest = null;
                if (dungeonLevel.AllyExists())
                {
                    AddMessage("Your Allies In The Dungeon Must Be Left Behind", true);
                }
            }
        }

        //Check if quest completed
        private bool UpdateQuestCompletion()
        {
            if (currentQuest == null) return false;
            bool result = false;
            switch (currentQuest.questType)
            {
                case (int)SC.QuestType.ASSASSINATE:
                    if (!currentQuest.questCreature.alive) result = true;
                    break;
                case (int)SC.QuestType.ITEM:
                    if (player1.carriedItems != null && player1.carriedItems.Count > 0)
                    {
                        for (int i = 0; i < player1.carriedItems.Count; i++)
                        {
                            if (player1.carriedItems[i].Equals(currentQuest.questItem)) result = true;
                        }
                    }
                    break;
                case (int)SC.QuestType.EXPLORE:
                    if (dungeonLevel.levelNumber >= currentQuest.level + currentQuest.numFloors - 1)
                    {
                        result = true;
                    }
                    break;
                default:
                    break;
            }
            if (result)
            {
                currentQuest.completed = true;
                ShowTutorialMessage(6);
            }
            return result;
        }

        //Check if current quest has become uncompletable
        private bool UpdateQuestFailed()
        {
            if (currentQuest == null) return false;
            bool result = false;
            switch (currentQuest.questType)
            {
                case (int)SC.QuestType.RESCUE:
                    if (!currentQuest.questCreature.alive) result = true;
                    break;
                default:
                    break;
            }
            return result;
        }

        private List<Item> LootItemsStillUncollected()
        {
            List<Item> result = new List<Item>();
            if (lootItemsFromLastPlayer == null || lootItemsFromLastPlayer.Count == 0) return result;
            for (int i = 0; i < lootItemsFromLastPlayer.Count; i++)
            {
                bool foundInPlayersInventory = false;
                if (player1.carriedItems.Count > 0)
                {
                    for (int k = 0; k < player1.carriedItems.Count; k++)
                    {
                        if (lootItemsFromLastPlayer[i].Equals(player1.carriedItems[k])
                            || lootItemsFromLastPlayer[i].Equals(player1.equippedArmour)
                            || lootItemsFromLastPlayer[i].Equals(player1.equippedShield)
                            || lootItemsFromLastPlayer[i].Equals(player1.equippedSword)) foundInPlayersInventory = true;
                    }
                }
                if (!foundInPlayersInventory)
                {
                    result.Add(lootItemsFromLastPlayer[i]);
                }
            }
            return result;
        }

        private double AdjustedRecruitCost(Creature creature)
        {
            double result = 0;
            result = creature.recruitCost;
            if (player1.profession == (int)SC.Professions.EXPLORER) result = Math.Ceiling(result / 2);
            return result;
        }

        private bool CreatureIsDangerous(Creature theCreature)
        {
            if (!theCreature.hostile) return false;
            return theCreature.Attack(player1.FireDefence(), player1.IceDefence()) - player1.DefencePower() > player1.MAX_HEALTH / 3;
        }
        private bool CreatureIsPuny(Creature theCreature)
        {
            if (!theCreature.hostile) return false;
            return theCreature.Attack(player1.FireDefence(), player1.IceDefence()) - player1.DefencePower() <= 1;
        }

        private void Attack(int aX, int aY)
        {
            Creature enemy = currentLevel.GetCreature(aX, aY);
            if (enemy == null)
            {
                AddMessage("You Attack Thin Air!", true);
            }
            else
            {
                if (!enemy.hostile)
                {
                    enemy.hostile = true;
                    enemy.allied = false;
                    if (enemy.description.prefix == "")
                    {
                        AddMessage(enemy.description.description + " Is Enraged!", true);
                    }
                    else
                    {
                        AddMessage("The " + enemy.description.description + " Is Enraged!", true);
                    }
                }
                double damage = player1.AttackPower(enemy.description.elementDefenceFire, enemy.description.elementDefenceIce, enemy.defencePower);
                particleClusters.Add(new ParticleCluster(3,
                                                        aX * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        aY * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 2, 2, 5, txEffectImpact, 2));
                if (player1.equippedSword.elementalOffenceFire > 0)
                {
                    particleClusters.Add(new ParticleCluster(3,
                                                           aX * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                           aY * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                           TILE_SIZE / 2, 2, 4, txEffectFire, 2));
                }
                if (player1.equippedSword.elementalOffenceIce > 0)
                {
                    particleClusters.Add(new ParticleCluster(3,
                                                           aX * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                           aY * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                           TILE_SIZE / 2, 2, 4, txEffectIce, 2));
                }

                if (player1.equippedSword.weaponType == (int)SC.WeaponTypes.MACE)
                {
                    sfxClub.Play();
                }
                if (player1.equippedSword.weaponType == (int)SC.WeaponTypes.SWORD)
                {
                    sfxBlade.Play();
                }
                if (player1.equippedSword.weaponType == (int)SC.WeaponTypes.SPEAR)
                {
                    sfxBlade.Play();
                }
                if (player1.equippedSword.weaponType == (int)SC.WeaponTypes.DAGGER)
                {
                    sfxBlade.Play();
                }
                if (player1.equippedSword.weaponType == (int)SC.WeaponTypes.BOW)
                {
                    sfxArrow.Play();
                }

                if (enemy.Damage(damage))
                {
                    if (enemy.description.prefix == "")
                    {
                        AddMessage("You Kill " + enemy.description.name + "! " + damage.ToString() + " Damage!");
                    }
                    else
                    {
                        AddMessage("You Kill The " + enemy.description.name + "! " + damage.ToString() + " Damage!");
                    }
                    AddExperience(enemy);
                    StopFastMovement(); //Stop fast moving; player might walk into the next enemy and be killed instantly
                }
                else
                {
                    if (enemy.description.prefix == "")
                    {
                        AddMessage("You Strike " + enemy.description.name + " For " + damage.ToString() + " Damage");
                    }
                    else
                    {
                        AddMessage("You Strike The " + enemy.description.name + " For " + damage.ToString() + " Damage");
                    }
                }
            }
        }

        private void Kick(int aX, int aY)
        {
            Creature enemy = currentLevel.GetCreature(aX, aY);
            if (enemy == null)
            {
                AddMessage("You Kick Thin Air!", true);
                return;
            }
            if (enemy.kicked)
            {
                if (enemy.Damage(Math.Max(Math.Log(player1.level), 1)))
                {
                    AddMessage("Boof!");
                    AddExperience(enemy);
                    runStats.IncrementStat("Creatures Stomped");
                }
                else
                {
                    enemy.hostile = true;
                    enemy.allied = false;
                    if (enemy.description.prefix == "")
                    {
                        AddMessage(enemy.description.description + " Is Fed Up Being Kicked Around!", true);
                    }
                    else
                    {
                        AddMessage("The " + enemy.description.description + " Is Fed Up Being Kicked Around!", true);
                    }
                }
                return;
            }
            int kickX = 0;
            int kickY = 0;
            if (player1.x < enemy.x)
            {
                kickX = 1;
            }
            else
            {
                if (player1.x >= enemy.x + enemy.description.width) kickX = -1;
            }
            if (player1.y < enemy.y)
            {
                kickY = 1;
            }
            else
            {
                if (player1.y >= enemy.y + enemy.description.height) kickY = -1;
            }
            if (!currentLevel.CanMove(enemy.x + kickX, enemy.y + kickY, enemy.description.width, enemy.description.height, currentLevel.GetCreatureIndex(aX, aY)))
            {
                if (enemy.description.prefix == "")
                {
                    AddMessage("You Kick " + enemy.description.description + " But It Doesn't Budge", true);
                }
                else
                {
                    AddMessage("You Kick The " + enemy.description.description + " But It Doesn't Budge", true);
                }
            }
            else
            {
                enemy.x += kickX;
                enemy.y += kickY;
                if (currentLevel.CanMove(enemy.x + kickX, enemy.y + kickY, enemy.description.width, enemy.description.height, currentLevel.GetCreatureIndex(aX, aY)))
                {
                    enemy.x += kickX;
                    enemy.y += kickY;
                }
                if (currentLevel.CanMove(enemy.x + kickX, enemy.y + kickY, enemy.description.width, enemy.description.height, currentLevel.GetCreatureIndex(aX, aY)))
                {
                    enemy.x += kickX;
                    enemy.y += kickY;
                }
                if (enemy.description.prefix == "")
                {
                    AddMessage("You Boot " + enemy.description.description + "!");
                }
                else
                {
                    AddMessage("You Boot The " + enemy.description.description + "!");
                }
            }
            enemy.kicked = true;
            enemy.hostile = true;
            enemy.allied = false;
            if (enemy.Damage(Math.Max(Math.Log(player1.level), 1)))
            {
                AddMessage("Boof!");
                AddExperience(enemy);
                runStats.IncrementStat("Creatures Stomped");
            }
        }

        private void AddExperience(Creature creatureKilled)
        {
            double expGained = Math.Ceiling((double)((float)creatureKilled.description.experience * ((float)creatureKilled.currentLevel / player1.level)));
            double debug = Math.Ceiling(Math.Log(5 * (creatureKilled.currentLevel - player1.level / 2)));
            if (creatureKilled.currentLevel > player1.level) expGained = expGained * Math.Ceiling(Math.Log(6 * (creatureKilled.currentLevel - player1.level / 2)));
            AddExperience(expGained);
            runStats.IncrementStat("Creatures Killed");
            IncreaseFeat(SC.FEAT_KILL_ENEMIES, 1);
            if (creatureKilled.frozen) IncreaseFeat(SC.FEAT_KILL_FROZEN_ENEMIES, 1);
            if (CreatureIsDangerous(creatureKilled)) IncreaseFeat(SC.FEAT_KILL_DANGEROUS_ENEMIES, 1);
        }

        private void AddExperience(double aExp)
        {
            double remainder = aExp;
            if (player1.profession == (int)SC.Professions.EXPLORER) remainder = Math.Ceiling(remainder * 1.5);
            bool levelGained = false;
            while (remainder > LARGE_EXP_CHUNK)
            {
                levelGained = player1.AddExperience(LARGE_EXP_CHUNK) || levelGained;
                remainder -= LARGE_EXP_CHUNK;
            }
            levelGained = player1.AddExperience(remainder) || levelGained;
        }

        //Add experience without any bonuses
        private void AddRawExperience(double aExp)
        {
            double remainder = aExp;
            bool levelGained = false;
            while (remainder > LARGE_EXP_CHUNK)
            {
                levelGained = player1.AddExperience(LARGE_EXP_CHUNK) || levelGained;
                remainder -= LARGE_EXP_CHUNK;
            }
            levelGained = player1.AddExperience(remainder) || levelGained;
        }

        private void UpdateGame()
        {
            if (moveCount < MAX_MOVES && !inVillage) moveCount++;

            if (!isLevelComplete)
            {
                UpdateCreatures();

                UpdateVision();

                CreatureVisionCheck();

                PlayerTrapCheck();
            }

            UpdateQuestCompletion();

            //Age the food in player's inventory
            if (player1.carriedItems != null && player1.carriedItems.Count > 0)
            {
                for (int i = 0; i < player1.carriedItems.Count; i++)
                {
                    if (player1.carriedItems[i].itemType == (int)SC.ItemTypes.FOOD && !player1.carriedItems[i].rotten && !player1.carriedItems[i].canned)
                    {
                        player1.carriedItems[i].quality -= player1.carriedItems[i].decay;
                        if (player1.carriedItems[i].quality <= 0)
                        {
                            player1.carriedItems[i].rotten = true;
                            player1.carriedItems[i].itemName = "Rotten " + player1.carriedItems[i].itemName;
                            player1.carriedItems[i].itemDescription = "Yuck!";
                            player1.carriedItems[i].value = 1;
                        }
                    }
                }
            }
            //Age the food in the level
            if (currentLevel.itemList != null && currentLevel.itemList.Count > 0)
            {
                for (int i = 0; i < currentLevel.itemList.Count; i++)
                {
                    if (currentLevel.itemList[i].itemType == (int)SC.ItemTypes.FOOD && !currentLevel.itemList[i].rotten && !currentLevel.itemList[i].canned)
                    {
                        currentLevel.itemList[i].quality -= currentLevel.itemList[i].decay;
                        if (currentLevel.itemList[i].quality <= 0)
                        {
                            currentLevel.itemList[i].rotten = true;
                            currentLevel.itemList[i].itemName = "Rotten " + currentLevel.itemList[i].itemName;
                            currentLevel.itemList[i].value = 1;
                        }
                    }
                }
            }

            //Increment log message ages
            if (messages.Count > 0)
            {
                for (int i = 0; i < messages.Count; i++)
                {
                    messages[i].age += 1;
                }
            }

            hasMadeMove = false;
        }

        //Return a bool to show if the player got a high score.
        private bool GameOver()
        {
            //Need to update feats in case player quits when a feat == its max
            UpdateFeats();
            currentGameState = GAMESTATE_GAMEOVERSCREEN;
            runBonusXP = Math.Max(Math.Ceiling((player1.experience - totalBonusXP) * 0.75), 0); //2% bonus
            runBonusGold = Math.Max(Math.Ceiling(player1.gold / 500), 2);
            runStats.IncreaseStat("Accumulated Exp", player1.experience - totalBonusXP);
            runStats.IncreaseStat("Accumulated Gold", player1.gold);

            selectedRandomTip = random.Next(0, randomTips.Count);
            death = "Died ";
            if (playerQuit)
            {
                death = "Quit ";
            }
            else
            {
                if (player1.deadFromStarvation)
                {
                    death = "Died From Starvation ";
                }
                else
                {
                    if (player1.deadFromPoison)
                    {
                        death = "Died From Poisoning ";
                    }
                    else
                    {
                        if (player1.killedBySelf)
                        {
                            death = "Killed Himself ";
                        }
                        else
                        {
                            if (fatalEnemy != null)
                            {
                                if (fatalEnemy.description.prefix == "")
                                {
                                    death = "Killed By " + fatalEnemy.description.name + " ";
                                }
                                else
                                {
                                    death = "Killed By " + fatalEnemy.description.prefix + " " + fatalEnemy.description.name + " ";
                                }
                            }
                        }
                    }
                }
            }
            double experienceRate = Math.Max(player1.experience - totalBonusXP, 0) / Math.Max(moveCount, 1);
            if (cheating) experienceRate = 0;
            death += "\n\nExperience Rate: " + experienceRate.ToString("0.00");
            if (!cheating)
            {
                AddPlayersItemsToLootList();
                return false;
            }
            else
            {
                startingLevel = 1; //If cheating, don't leave starting level at current level
                return false;
            }
        }

        private void AddPlayersItemsToLootList()
        {
            //lootItemsFromLastPlayer = new List<Item>();
            if (lootItemsFromLastPlayer.Count == 0)
            {
                lootItemsFromLastPlayer.Add(player1.equippedArmour);
                lootItemsFromLastPlayer.Add(player1.equippedShield);
                lootItemsFromLastPlayer.Add(player1.equippedSword);
            }
            else
            {
                //Need to catch case that current loot list doesn't have any item of a certain type.
                //bools below will be used to track if no item (let alone a better one) of a give type was found or not
                bool foundWeapon = false;
                bool foundArmour = false;
                bool foundShield = false;
                for (int i = 0; i < lootItemsFromLastPlayer.Count; i++)
                {
                    if (lootItemsFromLastPlayer[i].itemType == (int)SC.ItemTypes.ARMOUR)
                    {
                        foundArmour = true;
                        if (player1.equippedArmour.defence > lootItemsFromLastPlayer[i].defence)
                        {
                            lootItemsFromLastPlayer.RemoveAt(i);
                            lootItemsFromLastPlayer.Add(player1.equippedArmour);
                            break;
                        }
                    }
                }
                if (!foundArmour) lootItemsFromLastPlayer.Add(player1.equippedArmour);
                for (int i = 0; i < lootItemsFromLastPlayer.Count; i++)
                {
                    if (lootItemsFromLastPlayer[i].itemType == (int)SC.ItemTypes.SHIELD)
                    {
                        foundShield = true;
                        if (player1.equippedShield.defence > lootItemsFromLastPlayer[i].defence)
                        {
                            lootItemsFromLastPlayer.RemoveAt(i);
                            lootItemsFromLastPlayer.Add(player1.equippedShield);
                            break;
                        }
                    }
                }
                if (!foundShield) lootItemsFromLastPlayer.Add(player1.equippedShield);

                for (int i = 0; i < lootItemsFromLastPlayer.Count; i++)
                {
                    if (lootItemsFromLastPlayer[i].itemType == (int)SC.ItemTypes.WEAPON && lootItemsFromLastPlayer[i].weaponType == player1.equippedSword.weaponType)
                    {
                        foundWeapon = true;
                        if (player1.equippedSword.damage + player1.equippedSword.elementalOffenceFire + player1.equippedSword.elementalOffenceIce
                            > lootItemsFromLastPlayer[i].damage + lootItemsFromLastPlayer[i].elementalOffenceFire + lootItemsFromLastPlayer[i].elementalOffenceIce)
                        {
                            lootItemsFromLastPlayer.RemoveAt(i);
                            lootItemsFromLastPlayer.Add(player1.equippedSword);
                            break;
                        }
                    }   
                }
                //If "found" is still false then no weapon of this type existed in the list yet; so add this one.
                if (!foundWeapon)
                {
                    lootItemsFromLastPlayer.Add(player1.equippedSword);
                }

            }
            //Mark all items as not-for-sale, else they might appear marked in next game
            for (int i = 0; i < lootItemsFromLastPlayer.Count; i++)
            {
                lootItemsFromLastPlayer[i].forSale = false;
            }
        }

        private void RemoveItemFromLootList(Item aItem)
        {
            if (!(lootItemsFromLastPlayer == null) && !(lootItemsFromLastPlayer.Count == 0))
            {
                for (int i = 0; i < lootItemsFromLastPlayer.Count; i++)
                {
                    if (lootItemsFromLastPlayer[i].Equals(aItem))
                    {
                        lootItemsFromLastPlayer.RemoveAt(i);
                        break;
                    }
                }
            }
            //Found bug where item could be cloned if it were dropped in village after a new quest was accepted
            //So need to look for this item and remove from current dungeon level item list if present
            if (inVillage && dungeonLevel != null)
            {
                if (!(dungeonLevel.itemList == null) && !(dungeonLevel.itemList.Count == 0))
                {
                    for (int i = 0; i < dungeonLevel.itemList.Count; i++)
                    {
                        if (dungeonLevel.itemList[i].Equals(aItem))
                        {
                            dungeonLevel.itemList.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        private String PlayerProfileString()
        {
            if (SignedInGamer.SignedInGamers.Count > 0)
            {
                for (int i = 0; i < SignedInGamer.SignedInGamers.Count; i++)
                {
                    if (SignedInGamer.SignedInGamers[i].PlayerIndex == playerIndex)
                    {
                        return SignedInGamer.SignedInGamers[i].Gamertag;
                    }
                }
            }
            return "---";
        }

        private bool IsSignedIn(PlayerIndex aPI)
        {
            if (SignedInGamer.SignedInGamers.Count > 0)
            {
                for (int i = 0; i < SignedInGamer.SignedInGamers.Count; i++)
                {
                    if (SignedInGamer.SignedInGamers[i].PlayerIndex == aPI)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool AnyInputDetected(PlayerIndex aPI)
        {
            GamePadState theGamePad = GamePad.GetState(aPI);
            return (theGamePad.Buttons.A == ButtonState.Pressed
                || theGamePad.Buttons.B == ButtonState.Pressed
                || theGamePad.Buttons.Y == ButtonState.Pressed
                || theGamePad.Buttons.X == ButtonState.Pressed
                || theGamePad.Buttons.Back == ButtonState.Pressed
                || theGamePad.Buttons.LeftShoulder == ButtonState.Pressed
                || theGamePad.Buttons.LeftStick == ButtonState.Pressed
                || theGamePad.Buttons.RightShoulder == ButtonState.Pressed
                || theGamePad.Buttons.RightStick == ButtonState.Pressed
                || theGamePad.Buttons.Start == ButtonState.Pressed
                || theGamePad.DPad.Down == ButtonState.Pressed
                || theGamePad.DPad.Up == ButtonState.Pressed
                || theGamePad.DPad.Left == ButtonState.Pressed
                || theGamePad.DPad.Right == ButtonState.Pressed
                || theGamePad.ThumbSticks.Left.X != 0f
                || theGamePad.ThumbSticks.Left.Y != 0f
                || theGamePad.ThumbSticks.Right.X != 0f
                || theGamePad.ThumbSticks.Right.Y != 0f
                || theGamePad.Triggers.Left != 0f
                || theGamePad.Triggers.Right != 0f
                );
        }

        //End the game without game over e.g. on a Suspended Game
        private void EndGame()
        {
            currentGameState = GAMESTATE_TITLESCREEN;
            totalSecondsPlayed += (long)runTime.Elapsed.TotalSeconds;
            MediaPlayer.Stop();
            MediaPlayer.Play(songTitle);
            settingsLoadComplete = false;
            dialogOpen = false;
            startMenuOpen = false;
        }

        private void CheckIfResumableExists()
        {
            if (gamerStorageDevice == null)
            {
                StorageDevice.BeginShowSelector(playerIndex, CheckResumableCallback, null);
            }
            else
            {
                if (!gamerStorageDevice.IsConnected)
                {
                    StorageDevice.BeginShowSelector(playerIndex, CheckResumableCallback, null);
                }
                else
                {
                    CheckResumableBody();
                }
            }
        }
        private void CheckResumableCallback(IAsyncResult aIASR)
        {
            gamerStorageDevice = StorageDevice.EndShowSelector(aIASR);
            if (gamerStorageDevice == null)
            {
                resumableGameExists = false;
            }
            else
            {
                if (!gamerStorageDevice.IsConnected)
                {
                    resumableGameExists = false;
                }
                else
                {
                    CheckResumableBody();
                }
            }
        }
        private void CheckResumableBody()
        {
            try
            {
                gamerStorageDevice.BeginOpenContainer(TITLE, CheckResumableInnards, null);
                /*StorageContainer container = gamerStorageDevice.OpenContainer(TITLE);
                String filePath = container.Path + "SuspendedGame";
                resumableGameExists = File.Exists(filePath);
                container.Dispose();*/
            }
            catch
            {
                resumableGameExists = false;
            }
        }

        private void CheckResumableInnards(IAsyncResult aIASR)
        {
            try
            {
                StorageContainer container = gamerStorageDevice.EndOpenContainer(aIASR);
                String filePath = "SuspendedGame";
                resumableGameExists = container.FileExists(filePath);
                container.Dispose();
            }
            catch
            {
                resumableGameExists = false;
            }
        }

        //Save a resumable game. 
        //TODO : Have a global boolean for the success of the last save attempt
        //This function cannot return a bool since it fires callbacks
        private void SaveResumableGame()
        {
            resumableGameSaving = true;
            if (gamerStorageDevice == null)
            {
                StorageDevice.BeginShowSelector(playerIndex, SaveResumableCallBack, null);
            }
            else
            {
                if (!gamerStorageDevice.IsConnected)
                {
                    StorageDevice.BeginShowSelector(playerIndex, SaveResumableCallBack, null);
                }
                else
                {
                    new Thread(SaveResumableBody).Start();
                }
            }
        }
        private void SaveResumableCallBack(IAsyncResult aIASR)
        {
            gamerStorageDevice = StorageDevice.EndShowSelector(aIASR);
            if (gamerStorageDevice != null && gamerStorageDevice.IsConnected)
            {
                new Thread(SaveResumableBody).Start();
            }
            else
            {
                resumableGameSaving = false;
            }
        }
        private void SaveResumableBody()
        {
            try
            {
                gamerStorageDevice.BeginOpenContainer(TITLE, SaveResumableInnards, null);
                /*StorageContainer container = gamerStorageDevice.OpenContainer(TITLE);
                String filePath = container.Path + "SuspendedGame";
                FileStream fileStream = File.Open(filePath, FileMode.Create);
                XmlSerializer xmls = new XmlSerializer(typeof(ResumableGame));
                xmls.Serialize(fileStream, new ResumableGame(VERSION, player1, currentLevel, runStats, merchantItems, moveCount));
                fileStream.Close();
                container.Dispose();
                resumableGameSaving = false;
                SaveSettings();*/
            }
            catch
            {
                resumableGameSaving = false;
            }
        }

        private void SaveResumableInnards(IAsyncResult aIASR)
        {
            try
            {
                StorageContainer container = gamerStorageDevice.EndOpenContainer(aIASR);
                String filePath = "SuspendedGame"; //container.Path + "SuspendedGame";
                Stream fileStream = container.OpenFile(filePath, FileMode.Create);
                XmlSerializer xmls = new XmlSerializer(typeof(ResumableGame));
                xmls.Serialize(fileStream, new ResumableGame(VERSION, player1, dungeonLevel, villageLevel, inVillage,
                                                            runStats, dungeonMerchantItems, villageMerchantItems, villageMerchantLevel,
                                                            moveCount, currentQuest, currentQuestList, dungeonMusic.title));
                fileStream.Close();
                container.Dispose();
                resumableGameSaving = false;
                SaveSettings();
            }
            catch (Exception e)
            {
                resumableGameSaving = false;
            }
        }

        private void LoadResumableGame()
        {
            resumableGameLoading = true;
            if (gamerStorageDevice == null)
            {
                StorageDevice.BeginShowSelector(playerIndex, LoadResumableCallBack, null);
            }
            else
            {
                if (!gamerStorageDevice.IsConnected)
                {
                    StorageDevice.BeginShowSelector(playerIndex, LoadResumableCallBack, null);
                }
                else
                {
                    LoadResumableBody();
                }
            }
        }
        private void LoadResumableCallBack(IAsyncResult aIASR)
        {
            gamerStorageDevice = StorageDevice.EndShowSelector(aIASR);
            if (gamerStorageDevice != null && gamerStorageDevice.IsConnected)
            {
                LoadResumableBody();
            }
            else
            {
                resumableGameLoading = false;
            }
        }
        private void LoadResumableBody()
        {

            /*StorageContainer container = gamerStorageDevice.OpenContainer(TITLE);
            String filePath = container.Path + "SuspendedGame";
            FileStream fileStream;
            XmlSerializer xmls = new XmlSerializer(typeof(ResumableGame));
            ResumableGame loadedGame;*/
            try
            {
                gamerStorageDevice.BeginOpenContainer(TITLE, LoadResumableInnards, null);
            }
            catch (Exception e)
            {
                //container.Dispose();
                resumableGameLoading = false;
                currentGameState = GAMESTATE_TITLESCREEN;
                MediaPlayer.Stop();
                MediaPlayer.Play(songTitle);
                currentMainMenuSelection = 0;
                settingsLoadComplete = false;
            }
        }

        private void LoadResumableInnards(IAsyncResult aIASR)
        {
            StorageContainer container = gamerStorageDevice.EndOpenContainer(aIASR);
            String filePath = "SuspendedGame"; // container.Path + "SuspendedGame";
            Stream fileStream;
            XmlSerializer xmls = new XmlSerializer(typeof(ResumableGame));
            ResumableGame loadedGame;
            try
            {
                fileStream = container.OpenFile(filePath, FileMode.Open);
                loadedGame = (ResumableGame)xmls.Deserialize(fileStream);
                player1 = loadedGame.player;
                lastLevel = player1.level;
                lastHunger = player1.hunger;
                dungeonLevel = loadedGame.dungeonLevel;
                villageLevel = loadedGame.villageLevel;
                inVillage = loadedGame.inVillage;
                if (inVillage)
                {
                    currentLevel = villageLevel;
                }
                else
                {
                    currentLevel = dungeonLevel;
                }
                runStats = loadedGame.stats;
                dungeonMerchantItems = loadedGame.dungeonMerchantItems;
                villageMerchantItems = loadedGame.villageMerchantItems;
                //It's possible for the player to have a higher merchant level in settings than in this saved game
                //Make sure it doesn't revert to a lower number!
                villageMerchantLevel = Math.Max(villageMerchantLevel, loadedGame.villageMerchantLevel);
                moveCount = loadedGame.moveCount;
                currentQuest = loadedGame.currentQuest;
                currentQuestList = loadedGame.questList;
                messages = new List<LogMessage>();
                cheating = false;
                runTime.Reset();
                runTime.Start();
                SetTileSize(TILE_SIZE);
                SetScreenProjection();
                if (inVillage)
                {
                    PlayBGM(villageMusic);
                }
                else
                {
                    PlayDungeonBGM(loadedGame.dungeonMusicTitle);
                }
                AddMessage("Welcome Back " + player1.name);
                if (currentLevel != null) currentLevelNumber = currentLevel.levelNumber;
                //Bug where quest creature might not equal creature in level
                if (dungeonLevel != null && currentQuest != null 
                    && dungeonLevel.levelNumber == currentQuest.level + currentQuest.numFloors - 1)
                {
                    if (currentQuest.questCreature != null)
                    {
                        if (dungeonLevel.creatureList.Count > 0)
                        {
                            for (int i = 0; i < dungeonLevel.creatureList.Count; i++)
                            {
                                if (dungeonLevel.creatureList[i].Equals(currentQuest.questCreature))
                                {
                                    //Set the level creature as the quest creature, and not the other way round.
                                    //The one in the level is the one that gets updated!
                                    currentQuest.questCreature = dungeonLevel.creatureList[i];
                                }
                            }
                        }
                    }
                }
                fileStream.Close();
                container.DeleteFile(filePath);
                container.Dispose();
                resumableGameLoading = false;
            }
            catch (Exception e)
            {
                container.Dispose();
                resumableGameLoading = false;
                currentGameState = GAMESTATE_TITLESCREEN;
                MediaPlayer.Stop();
                MediaPlayer.Play(songTitle);
                currentMainMenuSelection = 0;
                settingsLoadComplete = false;
            }
        }

        private void DeleteResumableGame()
        {
            if (gamerStorageDevice != null && gamerStorageDevice.IsConnected)
            {
                gamerStorageDevice.BeginOpenContainer(TITLE, DeleteResumableGameBody, null);
            }
        }
        private void DeleteResumableGameBody(IAsyncResult aIASR)
        {
            StorageContainer container = gamerStorageDevice.EndOpenContainer(aIASR);
            String filePath = "SuspendedGame"; // container.Path + "SuspendedGame";
            try
            {
                container.DeleteFile(filePath);
                container.Dispose();
            }
            catch (Exception e)
            {
                container.Dispose();
            }
        }

        private void SaveSettings()
        {
            if (gamerStorageDevice == null)
            {
                StorageDevice.BeginShowSelector(playerIndex, SaveSettingsCallBack, null);
            }
            else
            {
                if (!gamerStorageDevice.IsConnected)
                {
                    StorageDevice.BeginShowSelector(playerIndex, SaveSettingsCallBack, null);
                }
                else
                {
                    SaveSettingsBody();
                }
            }
        }
        private void SaveSettingsCallBack(IAsyncResult aIASR)
        {
            gamerStorageDevice = StorageDevice.EndShowSelector(aIASR);
            if (gamerStorageDevice != null && gamerStorageDevice.IsConnected) SaveSettingsBody();
        }
        private void SaveSettingsBody()
        {
            gamerStorageDevice.BeginOpenContainer(TITLE, SaveSettingsInnards, null);
        }
        private void SaveSettingsInnards(IAsyncResult aIASR)
        {
            notCurrentlySaving = false;
            guiStorageNotifyFade = 255;
            try
            {
                StorageContainer container = gamerStorageDevice.EndOpenContainer(aIASR);
                String filePath = "Settings";
                Stream fileStream = container.OpenFile(filePath, FileMode.Create);
                XmlSerializer xmls = new XmlSerializer(typeof(GameSavedData));
                GameSavedData gsd = new GameSavedData(VERSION, totalBonusXP, totalBonusGold, totalStats, displayFullScreen, TILE_SIZE, MAX_STARTING_QUEST_LEVEL, totalSecondsPlayed, bossList, villageMerchantLevel, villageLevel.GetVillageBuildArea(), villageLevel.GetSavableItemsInPlayerBuildArea(), tutorialMessagesNeverToSeeAgain);
                gsd.movingWithDpad = controller.MovingWithDPad();
                gsd.bgmPlayOption = bgmPlayOption;
                gsd.bgmShuffleOption = bgmShuffleOption;
                gsd.featList = featList;
                gsd.lootItemsFromLastPlayer = this.lootItemsFromLastPlayer;
                xmls.Serialize(fileStream, gsd);
                fileStream.Close();
                container.Dispose();
                notCurrentlySaving = true;
            }
            catch
            {
                notCurrentlySaving = true;
            }
        }

        private void LoadSettings()
        {
            if (gamerStorageDevice == null)
            {
                StorageDevice.BeginShowSelector(playerIndex, LoadSettingsCallBack, null);
            }
            else
            {
                if (!gamerStorageDevice.IsConnected)
                {
                    StorageDevice.BeginShowSelector(playerIndex, LoadSettingsCallBack, null);
                }
                else
                {
                    LoadSettingsBody();
                }
            }
        }
        private void LoadSettingsCallBack(IAsyncResult aIASR)
        {
            gamerStorageDevice = StorageDevice.EndShowSelector(aIASR);
            if (gamerStorageDevice != null && gamerStorageDevice.IsConnected)
            {
                LoadSettingsBody();
            }
            else
            {
                settingsLoadComplete = true; //The attempt to load settings has ended
            }
        }
        private void LoadSettingsBody()
        {
            gamerStorageDevice.BeginOpenContainer(TITLE, LoadSettingsInnards, null);

        }
        private void LoadSettingsInnards(IAsyncResult aIASR)
        {
            notCurrentlyLoading = false;
            guiStorageNotifyFade = 255;
            try
            {
                StorageContainer container = gamerStorageDevice.EndOpenContainer(aIASR);
                String filePath = "Settings";
                if (container.FileExists(filePath))
                {
                    Stream fileStream = container.OpenFile(filePath, FileMode.Open);
                    XmlSerializer xmls = new XmlSerializer(typeof(GameSavedData));
                    GameSavedData loadedSettings = (GameSavedData)xmls.Deserialize(fileStream);
                    totalBonusXP = loadedSettings.totalBonusXP;
                    totalBonusGold = loadedSettings.totalBonusGold;
                    totalStats = loadedSettings.totalStats;
                    displayFullScreen = loadedSettings.fullDisplay;
                    controller.SetMovingWithDPad(loadedSettings.movingWithDpad);
                    TILE_SIZE = loadedSettings.tileSize;
                    MAX_STARTING_QUEST_LEVEL = loadedSettings.maxStartingLevel;
                    totalSecondsPlayed = loadedSettings.totalTimeSeconds;
                    if (loadedSettings.bossList != null)
                    {
                        bossList = MergedBossList(bossList, loadedSettings.bossList);
                    }
                    villageMerchantLevel = loadedSettings.villageMerchantLevel;
                    loadedVillageBuildArea = loadedSettings.villageBuildArea;
                    loadedVillageItems = loadedSettings.villageItems;
                    if (loadedSettings.tutorialMessagesNeverToSeeAgain != null)
                    {
                        tutorialMessagesNeverToSeeAgain = loadedSettings.tutorialMessagesNeverToSeeAgain;
                    }
                    bgmPlayOption = loadedSettings.bgmPlayOption;
                    bgmShuffleOption = loadedSettings.bgmShuffleOption;
                    if (loadedSettings.featList != null)
                    {
                        featList = loadedSettings.featList;
                    }
                    if (loadedSettings.lootItemsFromLastPlayer != null)
                    {
                        lootItemsFromLastPlayer = loadedSettings.lootItemsFromLastPlayer;
                    }
                    fileStream.Close();
                }
                container.Dispose();
                CheckIfResumableExists();
                settingsLoadComplete = true;
            }
            catch
            {
                settingsLoadComplete = true;
            }

            notCurrentlyLoading = true;
        }

        //Merge 2 boss lists. Used to merge the saved boss list with the list in the binary.
        private List<Boss> MergedBossList(List<Boss> source, List<Boss> dest)
        {
            List<Boss> result = new List<Boss>();

            foreach (Boss boss in dest)
            {
                result.Add(boss);
            }

            foreach (Boss sourceBoss in source)
            {
                bool found = false;
                foreach (Boss destBoss in dest)
                {
                    if (destBoss.name == sourceBoss.name) found = true;
                }
                if (!found) result.Add(sourceBoss);
            }

            return result;
        }

        private void AddMessage(String aString)
        {
            //Add this string
            messages.Add(new LogMessage(aString));
            //Delete the last one
            if (messages.Count > 50)
            {
                messages.RemoveAt(0);
            }
            //We want the player to see this, snap the message display back
            //to the top
            messageDisplayOffset = 0;
        }
        private void AddMessage(String aString, bool bad)
        {
            //Add this string
            messages.Add(new LogMessage(aString, bad));
            //Delete the last one
            if (messages.Count > 50)
            {
                messages.RemoveAt(0);
            }
            //We want the player to see this, snap the message display back
            //to the top
            messageDisplayOffset = 0;
        }

        private void KillCreature(int theCreatureIndex)
        {
            Creature theCreature = currentLevel.creatureList[theCreatureIndex];
            if (theCreature.description.creatureType != (int)SC.CreatureTypes.SKELETON
                && theCreature.description.creatureType != (int)SC.CreatureTypes.ZOMBIE
                && theCreature.description.creatureType != (int)SC.CreatureTypes.DEMON
                && theCreature.description.creatureType != (int)SC.CreatureTypes.MERCHANT
                && theCreature.description.creatureType != (int)SC.CreatureTypes.GOLDRICK)
            {
                for (int i = 0; i < theCreature.description.width; i++)
                {
                    for (int j = 0; j < theCreature.description.height; j++)
                    {
                        if (random.NextDouble() <= CHANCE_OF_CORPSE)
                        {
                            Item corpse = itemGenerator.GenerateCorpse(theCreature, i, j);
                            currentLevel.DropItem(corpse, corpse.x, corpse.y);
                        }
                    }
                }
            }
            currentLevel.creatureList.RemoveAt(theCreatureIndex);
        }

        private void UpdateCreatures()
        {
            bool NeedToPlayCreatureAttackSFX = false;
            if (currentLevel.creatureList != null && currentLevel.creatureList.Count > 0)
            {
                //find and remove any dead creatures
                for (int i = currentLevel.creatureList.Count - 1; i >= 0; i--)
                {
                    currentLevel.creatureList[i].Update();
                    if (!currentLevel.creatureList[i].alive)
                    {
                        KillCreature(i);
                    }
                }

                for (int i = 0; i < currentLevel.creatureList.Count; i++)
                {
                    bool moved = false;
                    Creature theCreature = currentLevel.creatureList[i]; //This pointer makes the code below more readable.

                    if (!theCreature.frozen)
                    {
                        //TODO: Sloppy code here to check ALL tiles of an enemy against player's
                        //position. Only need to check the outer tiles.
                        bool standingBesidePlayer = false;
                        for (int j = theCreature.x; j < theCreature.x + theCreature.description.width; j++)
                        {
                            for (int k = theCreature.y; k < theCreature.y + theCreature.description.height; k++)
                            {
                                if (Math.Abs(j - player1.x) <= 1 && Math.Abs(k - player1.y) <= 1)
                                {
                                    standingBesidePlayer = true;
                                }
                            }
                        }
                        //Also check whether standing immediately beside (not diagonally to) player
                        bool standingImmediatelyBesidePlayer = false;
                        for (int j = theCreature.x; j < theCreature.x + theCreature.description.width; j++)
                        {
                            for (int k = theCreature.y; k < theCreature.y + theCreature.description.height; k++)
                            {
                                if ((Math.Abs(j - player1.x) <= 1 && Math.Abs(k - player1.y) <= 0)
                                    || (Math.Abs(j - player1.x) <= 0 && Math.Abs(k - player1.y) <= 1))
                                {
                                    standingImmediatelyBesidePlayer = true;
                                }
                            }
                        }
                        //if hostile and hasn't used its turn moving, try and attack
                        if (!moved && theCreature.hostile)
                        {
                            if (standingBesidePlayer)
                            {
                                moved = true;
                                double damage = Math.Max(theCreature.Attack(player1.FireDefence(), player1.IceDefence()) - player1.DefencePower(), 1);

                                if (theCreature.description.elementOffenceFire > 0)
                                {
                                    particleClusters.Add(new ParticleCluster(3,
                                                            halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                            halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                            TILE_SIZE / 2, 2, 5, txEffectFire, 2));
                                }
                                else
                                {
                                    if (theCreature.description.elementOffenceIce > 0)
                                    {
                                        particleClusters.Add(new ParticleCluster(3,
                                                         halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                         halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                         TILE_SIZE / 2, 2, 5, txEffectIce, 2));
                                    }
                                    else
                                    {
                                        particleClusters.Add(new ParticleCluster(3,
                                                        halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                        halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                        TILE_SIZE / 2, 2, 5, txEffectImpact, 2));
                                    }
                                }

                                if (player1.Damage(damage))
                                {
                                    AddMessage(damage.ToString() + " Damage!", true);
                                    fatalEnemy = theCreature;
                                }
                                else
                                {
                                    NeedToPlayCreatureAttackSFX = true;
                                    if (theCreature.description.prefix == "")
                                    {
                                        AddMessage(theCreature.description.name + " Hit You For " + damage.ToString() + " Damage!", true);
                                    }
                                    else
                                    {
                                        AddMessage("The " + theCreature.description.name + " Hit You For " + damage.ToString() + " Damage!", true);
                                    }
                                    SetVibration(damage);
                                    if (theCreature.description.poisonous && random.NextDouble() < CHANCE_ENEMY_WILL_POISON)
                                    {
                                        AddMessage("You Have Been Poisoned!", true);
                                        sfxFail.Play();
                                        player1.poisoned = true;
                                        player1.poisonDuration = 20;
                                        player1.poisonDamage = Math.Ceiling(damage / 3);
                                    }
                                }
                            }

                            //If turn wasn't used attacking player, attack allied creature instead
                            int possibleAllied = NextToAlliedCreature(theCreature.x, theCreature.y, i);
                            if (!moved && possibleAllied >= 0)
                            {
                                moved = true;
                                double damage = Math.Max(theCreature.Attack(currentLevel.creatureList[possibleAllied].description.elementDefenceFire,
                                                        currentLevel.creatureList[possibleAllied].description.elementDefenceIce)
                                                        - currentLevel.creatureList[possibleAllied].defencePower,
                                                        1);
                                currentLevel.creatureList[possibleAllied].Damage(damage);
                                if (currentLevel.CanSee(player1.x, player1.y, currentLevel.creatureList[possibleAllied].x, currentLevel.creatureList[possibleAllied].y))
                                {
                                    if (theCreature.description.prefix == "")
                                    {
                                        AddMessage(theCreature.description.description +
                                            " Hit The Allied " + currentLevel.creatureList[possibleAllied].description.description +
                                            " For " + damage.ToString() + " Damage", true);
                                    }
                                    else
                                    {
                                        AddMessage("The " + theCreature.description.description +
                                            " Hit The Allied " + currentLevel.creatureList[possibleAllied].description.description +
                                            " For " + damage.ToString() + " Damage", true);
                                    }
                                    if (!currentLevel.creatureList[possibleAllied].alive)
                                    {
                                        StopFastMovement(); //Need to stop fast moving in case ally's death exposes player to attack
                                        AddMessage("The " + currentLevel.creatureList[possibleAllied].description.description + " Is Dead!");
                                    }
                                }
                            }
                        }

                        //Hasn't moved
                        //Allied and next to a hostile and isn't passive
                        if (!moved && theCreature.allied && theCreature.description.attacksWhenAllied)
                        {
                            int possibleHostile = NextToHostileCreature(theCreature.x, theCreature.y, i);
                            if (possibleHostile >= 0)
                            {
                                moved = true;
                                double damage = Math.Max(theCreature.Attack(currentLevel.creatureList[possibleHostile].description.elementDefenceFire,
                                                                            currentLevel.creatureList[possibleHostile].description.elementDefenceIce)
                                                                            - currentLevel.creatureList[possibleHostile].defencePower,
                                                                            1);
                                currentLevel.creatureList[possibleHostile].Damage(damage);
                                if (currentLevel.CanSee(player1.x, player1.y, currentLevel.creatureList[possibleHostile].x, currentLevel.creatureList[possibleHostile].y)
                                    || currentLevel.CanSee(player1.x, player1.y, theCreature.x, theCreature.y))
                                {
                                    if (currentLevel.creatureList[possibleHostile].description.prefix == "")
                                    {
                                        AddMessage("The Allied " + theCreature.description.name +
                                                " Hits " + currentLevel.creatureList[possibleHostile].description.name +
                                                " For " + damage.ToString() + " Damage");
                                    }
                                    else
                                    {
                                        AddMessage("The Allied " + theCreature.description.name +
                                                " Hits The " + currentLevel.creatureList[possibleHostile].description.name +
                                                " For " + damage.ToString() + " Damage");
                                    }
                                }
                                if (!currentLevel.creatureList[possibleHostile].alive)
                                {
                                    StopFastMovement(); //If ally has killed a creature, stop fast turns in case player now exposed to attack
                                    player1.AddExperience(10);
                                    if (currentLevel.CanSee(player1.x, player1.y, currentLevel.creatureList[possibleHostile].x, currentLevel.creatureList[possibleHostile].y))
                                    {
                                        if (currentLevel.creatureList[possibleHostile].description.prefix == "")
                                        {
                                            AddMessage(currentLevel.creatureList[possibleHostile].description.name + " Is Dead!");
                                        }
                                        else
                                        {
                                            AddMessage("The " + currentLevel.creatureList[possibleHostile].description.name + " Is Dead!");
                                        }
                                    }
                                }
                            }
                        }

                        if (!moved)
                        {
                            //if the creature is hostile and can see the player, home in
                            if (theCreature.hostile && currentLevel.IsLineOfSight(player1.x, player1.y, theCreature.x, theCreature.y, theCreature.description.width, theCreature.description.height))
                            {
                                moved = MoveTowards(theCreature, i, player1.x, player1.y);
                            }
                            //else not hostile and line of sight...
                            else
                            {

                                if (theCreature.allied && !theCreature.waiting)
                                {
                                    //Allied and aggresive and hostile in line of sight
                                    if (theCreature.description.attacksWhenAllied)
                                    {
                                        for (int j = 0; j < currentLevel.creatureList.Count; j++)
                                        {
                                            if (currentLevel.creatureList[j].hostile)
                                            {
                                                for (int kX = 0; kX < currentLevel.creatureList[j].description.width; kX++)
                                                {
                                                    for (int kY = 0; kY < currentLevel.creatureList[j].description.height; kY++)
                                                    {
                                                        if (!moved
                                                            && currentLevel.IsLineOfSight(theCreature.x, theCreature.y,
                                                                                            currentLevel.creatureList[j].x + kX,
                                                                                            currentLevel.creatureList[j].y + kY))
                                                        {
                                                            moved = MoveTowards(theCreature, i,
                                                                                currentLevel.creatureList[j].x + kX,
                                                                                currentLevel.creatureList[j].y + kY);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    //Allied and cook...
                                    if (!moved && theCreature.allied
                                        && theCreature.description.creatureType == (int)SC.CreatureTypes.CHEF)
                                    {
                                        //...and standing on corpse
                                        int possibleFood = currentLevel.GetEthicalCorpseIndexAtXY(theCreature.x, theCreature.y);
                                        if (possibleFood != -1)
                                        {
                                            moved = true;
                                            CookPrepareCorpse(theCreature, possibleFood);
                                        }
                                        //Allied and cook and corpse in line of sight
                                        possibleFood = currentLevel.GetIndexOfVisibleEthicalCorpse(theCreature.x, theCreature.y);
                                        if (!moved && possibleFood != -1)
                                        {
                                            moved = MoveTowards(theCreature, i, currentLevel.itemList[possibleFood].x, currentLevel.itemList[possibleFood].y);
                                        }
                                    }

                                    //Allied and player in line of sight
                                    if (!moved && theCreature.allied
                                        && !standingImmediatelyBesidePlayer
                                        && currentLevel.IsLineOfSight(player1.x, player1.y, theCreature.x, theCreature.y, theCreature.description.width, theCreature.description.height))
                                    {
                                        moved = MoveTowards(theCreature, i, player1.x, player1.y);
                                    }

                                    //Allied and other allied in line of sight
                                    if (!moved && theCreature.allied)
                                    {
                                        for (int j = 0; j < currentLevel.creatureList.Count; j++)
                                        {
                                            if (j != i
                                                && currentLevel.creatureList[j].allied
                                                && currentLevel.IsLineOfSight(theCreature.x, theCreature.y,
                                                                            currentLevel.creatureList[j].x, currentLevel.creatureList[j].y))
                                            {
                                                moved = MoveTowards(theCreature, i, currentLevel.creatureList[j].x, currentLevel.creatureList[j].y);
                                                if (moved) break;
                                            }
                                        }
                                    }

                                }

                                //Wander around
                                if (!moved
                                    && !theCreature.waiting
                                    && !(theCreature.description.creatureType == (int)SC.CreatureTypes.QUESTMASTER)
                                    && (!theCreature.allied || !standingBesidePlayer)
                                    && (!theCreature.allied || NextToAlliedCreature(theCreature.x, theCreature.y, i) == -1)
                                    )
                                {
                                    bool[] possibleMoves = new bool[8];
                                    possibleMoves[0] = CanMove(theCreature.x, theCreature.y - 1,
                                                                theCreature.description.width, theCreature.description.height, i);
                                    possibleMoves[1] = CanMove(theCreature.x + 1, theCreature.y - 1,
                                                                theCreature.description.width, theCreature.description.height, i);
                                    possibleMoves[2] = CanMove(theCreature.x + 1, theCreature.y,
                                                                theCreature.description.width, theCreature.description.height, i);
                                    possibleMoves[3] = CanMove(theCreature.x + 1, theCreature.y + 1,
                                                                theCreature.description.width, theCreature.description.height, i);
                                    possibleMoves[4] = CanMove(theCreature.x, theCreature.y + 1,
                                                                theCreature.description.width, theCreature.description.height, i);
                                    possibleMoves[5] = CanMove(theCreature.x - 1, theCreature.y + 1,
                                                                theCreature.description.width, theCreature.description.height, i);
                                    possibleMoves[6] = CanMove(theCreature.x - 1, theCreature.y,
                                                                theCreature.description.width, theCreature.description.height, i);
                                    possibleMoves[7] = CanMove(theCreature.x - 1, theCreature.y - 1,
                                                                theCreature.description.width, theCreature.description.height, i);

                                    int numPossibleMoves = 0;
                                    for (int j = 0; j < 8; j++)
                                    {
                                        if (possibleMoves[j]) numPossibleMoves++;
                                    }
                                    if (numPossibleMoves > 0)
                                    {
                                        int moveToMake = random.Next(1, numPossibleMoves + 3); //Add 2 extra to this - add a chance that creature will stay in current square
                                        int movesFound = 0;
                                        for (int j = 0; j < 8; j++)
                                        {
                                            if (possibleMoves[j])
                                            {
                                                movesFound++;
                                            }
                                            if (movesFound == moveToMake)
                                            {
                                                switch (j)
                                                {
                                                    case 0:
                                                        theCreature.y -= 1;
                                                        moved = true;
                                                        break;
                                                    case 1:
                                                        theCreature.x += 1;
                                                        theCreature.y -= 1;
                                                        moved = true;
                                                        break;
                                                    case 2:
                                                        theCreature.x += 1;
                                                        moved = true;
                                                        break;
                                                    case 3:
                                                        theCreature.x += 1;
                                                        theCreature.y += 1;
                                                        moved = true;
                                                        break;
                                                    case 4:
                                                        theCreature.y += 1;
                                                        moved = true;
                                                        break;
                                                    case 5:
                                                        theCreature.x -= 1;
                                                        theCreature.y += 1;
                                                        moved = true;
                                                        break;
                                                    case 6:
                                                        theCreature.x -= 1;
                                                        moved = true;
                                                        break;
                                                    case 7:
                                                        theCreature.x -= 1;
                                                        theCreature.y -= 1;
                                                        moved = true;
                                                        break;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //If allied and not frozen and not currently under attack, heal a little each turn
                        if (theCreature.allied && player1.profession == (int)SC.Professions.FIGHTER
                            && !theCreature.attackedLastRound)
                        {
                            if (theCreature.Heal(1)) AddMessage("The " + theCreature.description.name + "'s Wounds Have Healed.");
                        }

                    } //End "Not Frozen"
                    else
                    {
                        if (theCreature.FreezeTimeRemaining() > 3)
                        {
                            for (int j = 0; j < theCreature.description.width; j++)
                            {
                                for (int k = 0; k < theCreature.description.height; k++)
                                {
                                    particleClusters.Add(new ParticleCluster(40,
                                                                    (theCreature.x + j) * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + TILE_SIZE / 2,
                                                                    (theCreature.y + k) * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE + TILE_SIZE / 2,
                                                                    TILE_SIZE / 8, 3, 6, txEffectIce, 3));
                                }
                            }
                        }
                        else
                        {
                            if (currentLevel.IsLineOfSight(player1.x, player1.y,
                                                            theCreature.x, theCreature.y,
                                                            theCreature.description.width, theCreature.description.height))
                            {
                                if (theCreature.description.prefix == "")
                                {
                                    AddMessage(theCreature.description.description + " Has Almost Thawed Out!", true);
                                }
                                else
                                {
                                    AddMessage("The " + theCreature.description.description + " Has Almost Thawed Out!", true);
                                }
                                StopFastMovement();
                            }

                        }
                    }

                    if (moved && !theCreature.description.floats) CreatureTrapCheck(theCreature);
                }
            }

            if (NeedToPlayCreatureAttackSFX) sfxEnemyAttack.Play();
        }

        private void CreatureVisionCheck()
        {
            bool NeedToPlayDangerSFX = false; //Only want to play danger SFX once per turn!
            if (currentLevel.creatureList.Count > 0)
            {
                for (int i = 0; i < currentLevel.creatureList.Count; i++)
                {
                    Creature theCreature = currentLevel.creatureList[i];
                    if (!theCreature.seenByPlayer)
                    {
                        if (currentLevel.IsLineOfSight(player1.x, player1.y, theCreature.x, theCreature.y))
                        {
                            if (theCreature.hostile)
                            {
                                ShowTutorialMessage(10);
                            }
                            else
                            {
                                if (theCreature.description.creatureType == (int)SC.CreatureTypes.HUMAN)
                                {
                                    ShowTutorialMessage(13);
                                }
                            }
                            theCreature.seenByPlayer = true;
                            StopFastMovement();
                            if (CreatureIsDangerous(theCreature) && theCreature.hostile)
                            {
                                if (theCreature.description.prefix != "")
                                {
                                    AddMessage("You See A Dangerous " + theCreature.description.description + "!", true);
                                }
                                else
                                {
                                    AddMessage("You See " + theCreature.description.description + "!", true);
                                }
                                NeedToPlayDangerSFX = true;
                            }
                            else
                            {
                                if (theCreature.description.prefix != "")
                                {
                                    AddMessage("You See " + theCreature.description.prefix + " " + theCreature.description.description + "!", theCreature.hostile);
                                }
                                else
                                {
                                    AddMessage("You See " + theCreature.description.description + "!", theCreature.hostile);
                                }
                            }
                        }
                    }
                }
            }
            if (NeedToPlayDangerSFX) sfxDanger.Play();
        }

        //Level up allied creatures once.
        //Return true if at least one combatant ally was levelled up
        private bool LevelUpAlliedCreatures()
        {
            bool result = false;
            List<Creature> creatures = currentLevel.creatureList;
            if (creatures.Count > 0)
            {
                for (int i = 0; i < creatures.Count; i++)
                {
                    if (creatures[i].allied)
                    {
                        creatures[i].LevelUp();
                        if (creatures[i].description.attacksWhenAllied) result = true;
                    }
                }
            }
            return result;
        }

        private void CookPrepareCorpse(Creature theCook, int corpseIndex)
        {
            Item theCorpse = currentLevel.itemList[corpseIndex];
            currentLevel.itemList.RemoveAt(corpseIndex);
            List<Item> preparedFood = itemGenerator.CorpseToFoodItems(theCorpse);
            if (preparedFood != null && preparedFood.Count > 0)
            {
                for (int i = 0; i < preparedFood.Count; i++)
                {
                    currentLevel.itemList.Add(preparedFood[i]);
                }
                if (preparedFood.Count > 1)
                {
                    AddMessage("The " + theCook.description.name + " Turned The " +
                            theCorpse.itemName + " Into " +
                            preparedFood.Count + " Items!");
                }
                else
                {
                    AddMessage("The " + theCook.description.name + " Turned The " +
                            theCorpse.itemName + " Into " +
                            preparedFood[0].itemName + "!");
                }

            }
        }

        //private method to check if a space is free to move into, calls upon
        //the same method for the level
        private bool CanMove(int aX, int aY)
        {
            if (aX == player1.x && aY == player1.y) return false;
            return currentLevel.CanMove(aX, aY);
        }
        //Method for creatures bigger than 1 square
        private bool CanMove(int aX, int aY, int width, int height, int enemyIndex)
        {
            if ((aX == player1.x || (aX < player1.x && aX + width - 1 >= player1.x))
                && (aY == player1.y || (aY < player1.y && aY + height - 1 >= player1.y))) return false;
            return currentLevel.CanMove(aX, aY, width, height, enemyIndex);
        }

        private bool CanRemoveBossDoor()
        {
            bool result = true;

            if (player1.y >= currentLevel.DIMENSION - 4) result = false;

            if (currentLevel.creatureList.Count > 0)
            {
                for (int i = 0; i < currentLevel.creatureList.Count; i++)
                {
                    if (currentLevel.creatureList[i].y >= currentLevel.DIMENSION - 4) result = false;
                }
            }

            return result;
        }

        private bool MoveTowards(Creature theCreature, int creatureIndex, int tX, int tY)
        {
            bool result = false;
            //Case of player being above and to the left of enemy
            if (!result && tX < theCreature.x && tY < theCreature.y)
            {
                if (CanMove(theCreature.x - 1, theCreature.y - 1,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.x -= 1;
                    theCreature.y -= 1;
                    result = true;
                }
                if (!result && CanMove(theCreature.x - 1, theCreature.y,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.x -= 1;
                    result = true;
                }
                if (!result && CanMove(theCreature.x, theCreature.y - 1,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.y -= 1;
                    result = true;
                }
            }
            //Case of player being above the enemy
            if (!result && tX >= theCreature.x
                && tX <= theCreature.x + theCreature.description.width - 1
                && tY < theCreature.y)
            {
                if (!result && CanMove(theCreature.x, theCreature.y - 1,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.y -= 1;
                    result = true;
                }
                if (!result && CanMove(theCreature.x - 1, theCreature.y - 1,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.x -= 1;
                    theCreature.y -= 1;
                    result = true;
                }
                if (!result && CanMove(theCreature.x + 1, theCreature.y - 1,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.x += 1;
                    theCreature.y -= 1;
                    result = true;
                }
            }
            //Case of player being above and to the right
            if (!result && tX > theCreature.x + theCreature.description.width - 1
                && tY < theCreature.y)
            {
                if (CanMove(theCreature.x + 1, theCreature.y - 1,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.x += 1;
                    theCreature.y -= 1;
                    result = true;
                }
                if (!result && CanMove(theCreature.x + 1, theCreature.y,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.x += 1;
                    result = true;
                }
                if (!result && CanMove(theCreature.x, theCreature.y - 1,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.y -= 1;
                    result = true;
                }
            }
            //Case of player being to the right of the creature
            if (!result && tX > theCreature.x + theCreature.description.width - 1
                && tY >= theCreature.y && tY <= theCreature.y + theCreature.description.height - 1)
            {
                if (CanMove(theCreature.x + 1, theCreature.y,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.x += 1;
                    result = true;
                }
                if (!result && CanMove(theCreature.x + 1, theCreature.y + 1,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.x += 1;
                    theCreature.y += 1;
                    result = true;
                }
                if (!result && CanMove(theCreature.x + 1, theCreature.y - 1,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.x += 1;
                    theCreature.y -= 1;
                    result = true;
                }
            }
            //Case of player being below and to the right
            if (!result && tX > theCreature.x + theCreature.description.width - 1
                && tY > theCreature.y + theCreature.description.height - 1)
            {
                if (CanMove(theCreature.x + 1, theCreature.y + 1,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.x += 1;
                    theCreature.y += 1;
                    result = true;
                }
                if (!result && CanMove(theCreature.x + 1, theCreature.y,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.x += 1;
                    result = true;
                }
                if (!result && CanMove(theCreature.x, theCreature.y + 1,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.y += 1;
                    result = true;
                }
            }
            //Case of player being below the creature
            if (!result && tX >= theCreature.x
                && tX <= theCreature.x + theCreature.description.width - 1
                && tY > theCreature.y)
            {
                if (CanMove(theCreature.x, theCreature.y + 1,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.y += 1;
                    result = true;
                }
                if (!result && CanMove(theCreature.x + 1, theCreature.y + 1,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.x += 1;
                    theCreature.y += 1;
                    result = true;
                }
                if (!result && CanMove(theCreature.x - 1, theCreature.y + 1,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.x -= 1;
                    theCreature.y += 1;
                    result = true;
                }
            }
            //Case of player being below and to the left of the creature
            if (!result && tX < theCreature.x
                && tY > theCreature.y + theCreature.description.height - 1)
            {
                if (CanMove(theCreature.x - 1, theCreature.y + 1,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.x -= 1;
                    theCreature.y += 1;
                    result = true;
                }
                if (!result && CanMove(theCreature.x - 1, theCreature.y,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.x -= 1;
                    result = true;
                }
                if (!result && CanMove(theCreature.x, theCreature.y + 1,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.y += 1;
                    result = true;
                }
            }
            //Case of player being to the left of the creature
            if (!result && tX < theCreature.x
                && tY >= theCreature.y
                && tY <= theCreature.y + theCreature.description.height - 1)
            {
                if (CanMove(theCreature.x - 1, theCreature.y,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.x -= 1;
                    result = true;
                }
                if (!result && CanMove(theCreature.x - 1, theCreature.y + 1,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.x -= 1;
                    theCreature.y += 1;
                    result = true;
                }
                if (!result && CanMove(theCreature.x - 1, theCreature.y - 1,
                    theCreature.description.width, theCreature.description.height, creatureIndex))
                {
                    theCreature.x -= 1;
                    theCreature.y -= 1;
                    result = true;
                }
            }
            return result;
        }

        //Return index of the hostile creature next to x,y
        //-1 if no creature
        //index is the index of the creature to ensure a creature doesn't return itself. -1 if not in creatureList
        private int NextToHostileCreature(int x, int y, int index)
        {
            if (currentLevel.creatureList.Count > 0)
            {
                for (int i = 0; i < currentLevel.creatureList.Count; i++)
                {
                    if (i != index && currentLevel.creatureList[i].hostile)
                    {
                        for (int j = currentLevel.creatureList[i].x;
                            j < currentLevel.creatureList[i].x + currentLevel.creatureList[i].description.width; j++)
                        {
                            for (int k = currentLevel.creatureList[i].y;
                                k < currentLevel.creatureList[i].y + currentLevel.creatureList[i].description.height; k++)
                            {
                                //int debug1 = Math.Abs(j - x);
                                //int debug2 = Math.Abs(k - y);
                                if (Math.Abs(j - x) <= 1 && Math.Abs(k - y) <= 1) return i;
                            }
                        }
                    }
                }
            }
            return -1;
        }

        //Return index of the allied creature next to x,y
        private int NextToAlliedCreature(int x, int y, int index)
        {

            if (currentLevel.creatureList.Count > 0)
            {
                for (int i = 0; i < currentLevel.creatureList.Count; i++)
                {
                    if (i != index && currentLevel.creatureList[i].allied)
                    {
                        for (int j = currentLevel.creatureList[i].x;
                            j < currentLevel.creatureList[i].x + currentLevel.creatureList[i].description.width; j++)
                        {
                            for (int k = currentLevel.creatureList[i].y;
                                k < currentLevel.creatureList[i].y + currentLevel.creatureList[i].description.height; k++)
                            {
                                if (Math.Abs(j - x) <= 1 && Math.Abs(k - y) <= 1) return i;
                            }
                        }
                    }
                }
            }

            return -1;
        }

        //Method to return if player is looking in some direction
        // i.e. is looking at a tile around them, and not at themselves and not at a distant tile
        private bool PlayerLookingInSomeDirection()
        {
            //If player is looking...
            if (!playerLooking) return false;
            //If player is looking within one tile of himself...
            if (Math.Abs(playerLookAtX - player1.x) <= 1 && Math.Abs(playerLookAtY - player1.y) <= 1)
            {
                //...and not the tile 
                if (playerLookAtX != player1.x || playerLookAtY != player1.y)
                {
                    return true;
                }
            }
            return false;
        }

        //Method to return which enemy is in the direction the player is pointing
        //return -1 if no enemy
        private int CreaturePlayerIsPointingAt(int dX, int dY)
        {
            int result = -1;
            int targetX = player1.x + dX;
            int targetY = player1.y + dY;
            bool doneSearching = false;
            while (!doneSearching)
            {
                if (currentLevel.GetCreatureIndex(targetX, targetY) != -1)
                {
                    result = currentLevel.GetCreatureIndex(targetX, targetY);
                    doneSearching = true;
                }
                targetX += dX;
                targetY += dY;
                //if new spot is outside level area, stop looking
                if (targetX < 0 || targetX >= currentLevel.DIMENSION || targetY < 0 || targetY >= currentLevel.DIMENSION) doneSearching = true;
                //if new spot is not visible to player, stop looking
                if (!doneSearching && !currentLevel.CanSee(player1.x, player1.y, targetX, targetY)) doneSearching = true;
            }

            return result;
        }

        private bool PlayerWillBeAbleToHit(Creature creature)
        {
            return (Math.Abs(player1.x - creature.x) <= player1.equippedSword.range
                        || Math.Abs(player1.x - (creature.x + creature.description.width - 1)) <= player1.equippedSword.range)
                   && (Math.Abs(player1.y - creature.y) <= player1.equippedSword.range
                        || Math.Abs(player1.y - (creature.y + creature.description.height - 1)) <= player1.equippedSword.range)
                                && currentLevel.CanSee(player1.x, player1.y, creature.x, creature.y);
        }

        private void GenerateStartingGear()
        {
            //basic starting gear
            switch (player1.profession)
            {
                case (int)SC.Professions.MAGE:
                    for (int i = 0; i < 2; i++)
                    {
                        player1.AddItem(itemGenerator.ItemHealingPotion());
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        player1.AddItem(itemGenerator.ItemMagikPotion());
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        player1.AddItem(itemGenerator.GenerateFood());
                    }
                    break;
                case (int)SC.Professions.FIGHTER:
                    for (int i = 0; i < 4; i++)
                    {
                        player1.AddItem(itemGenerator.ItemHealingPotion());
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        player1.AddItem(itemGenerator.ItemMagikPotion());
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        player1.AddItem(itemGenerator.GenerateFood());
                    }
                    int numAttackPotions = (int)Math.Max(1, Math.Log((double)player1.level) / 2);
                    for (int i = 0; i < numAttackPotions; i++)
                    {
                        player1.AddItem(itemGenerator.ItemAttackPotion());
                    }
                    break;
                case (int)SC.Professions.EXPLORER:
                    for (int i = 0; i < 4; i++)
                    {
                        player1.AddItem(itemGenerator.ItemHealingPotion());
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        player1.AddItem(itemGenerator.GenerateFood());
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        player1.AddItem(itemGenerator.GenerateTinnedFood());
                    }
                    for (int i = 0; i < 7; i++)
                    {
                        player1.AddItem(itemGenerator.GenerateScroll(100));
                    }
                    break;
            }

            //bonus gear
            if (totalBonusGold > 0)
            {
                double bonusRemaining = totalBonusGold;

                int armourBonus = random.Next(10, Math.Min((int)bonusRemaining, player1.level) + 11);
                if (player1.profession == (int)SC.Professions.FIGHTER)
                {
                    if (cheating)
                    {
                        player1.equippedArmour = itemGenerator.GenerateArmour(player1.level);
                    }
                    else
                    {
                        player1.equippedArmour = itemGenerator.GenerateArmour(Math.Max(10, random.Next((int)Math.Floor(player1.level / 3 * 2), player1.level)));
                    }
                }
                else
                {
                    player1.equippedArmour = itemGenerator.GenerateArmour(armourBonus);
                }
                bonusRemaining -= armourBonus;
                int swordBonus = 10;
                if (bonusRemaining > 1)
                    swordBonus = random.Next(10, Math.Min((int)bonusRemaining, player1.level) + 11);

                if (player1.profession == (int)SC.Professions.EXPLORER)
                    player1.equippedSword = itemGenerator.GenerateBow(swordBonus);
                if (player1.profession == (int)SC.Professions.MAGE)
                    player1.equippedSword = itemGenerator.GenerateDagger(swordBonus);
                if (player1.profession == (int)SC.Professions.FIGHTER)
                {
                    if (cheating)
                    {
                        player1.equippedSword = itemGenerator.GenerateMace((int)Math.Ceiling((double)player1.level / 10) * 8);
                    }
                    else
                    {
                        player1.equippedSword = itemGenerator.GenerateMace(Math.Max(20, random.Next((int)Math.Floor((double)player1.level / 2), (int)Math.Ceiling((double)player1.level / 10) * 8)));
                    }
                }
                bonusRemaining -= swordBonus;
                if (bonusRemaining > 1)
                {
                    int shieldBonus = random.Next(5, Math.Min((int)bonusRemaining, player1.level) + 10);
                    if (player1.profession == (int)SC.Professions.FIGHTER)
                    {
                        if (cheating)
                        {
                            player1.equippedShield = itemGenerator.GenerateShield(player1.level);
                        }
                        else
                        {
                            player1.equippedShield = itemGenerator.GenerateShield(Math.Max(10, random.Next((int)Math.Floor(player1.level / 3 * 2), player1.level)));
                        }
                    }
                    else
                    {
                        player1.equippedShield = itemGenerator.GenerateShield(shieldBonus);
                    }
                    bonusRemaining -= shieldBonus;
                    if (bonusRemaining > 1)
                    {
                        int cloakBonus = random.Next(1, Math.Min((int)bonusRemaining, player1.level) + 10);
                        player1.equippedCloak = itemGenerator.GenerateCloak(cloakBonus);
                        bonusRemaining -= shieldBonus;
                        int extraItems = 0;
                        while (bonusRemaining > 1 && extraItems < 7)
                        {
                            int itemBonus = random.Next(Math.Max((int)Math.Ceiling(bonusRemaining / 2), 1), (int)bonusRemaining);
                            player1.AddItem(itemGenerator.GenerateStartingGearItem(itemBonus));
                            bonusRemaining -= itemBonus;
                            extraItems++;
                        }
                    }
                    else
                    {
                        player1.equippedCloak = itemGenerator.GenerateCloak(10);
                    }
                }
                else
                {
                    player1.equippedShield = itemGenerator.GenerateShield(10);
                    player1.equippedCloak = itemGenerator.GenerateCloak(10);
                }
            }
            else
            {
                player1.equippedArmour = itemGenerator.GenerateArmour(5);
                player1.equippedShield = itemGenerator.GenerateShield(5);
                player1.equippedCloak = itemGenerator.GenerateCloak(5);
                if (player1.profession == (int)SC.Professions.EXPLORER)
                    player1.equippedSword = itemGenerator.GenerateBow(5);
                if (player1.profession == (int)SC.Professions.MAGE)
                    player1.equippedSword = itemGenerator.GenerateDagger(5);
                if (player1.profession == (int)SC.Professions.FIGHTER)
                    player1.equippedSword = itemGenerator.GenerateMace(5);
            }

            //Mark all items as "load out"
            if (player1.carriedItems.Count > 0)
            {
                for (int i = 0; i < player1.carriedItems.Count; i++)
                {
                    player1.carriedItems[i].loadOut = true;
                }
            }
        }

        private void GenerateQuestList()
        {
            currentQuestList = new List<Quest>();
            for (int i = 0; i < QUEST_LIST_SIZE; i++)
            {
                AddNewQuestToQuestList();
            }
        }

        private void AddNewQuestToQuestList()
        {
            bool addedBossLevel = false;
            if (!QuestListContainsBoss())
            {
                for (int i = 0; i < bossList.Count; i++)
                {
                    if (bossList[i].level <= MAX_STARTING_QUEST_LEVEL && !bossList[i].beaten)
                    {
                        currentQuestList.Add(questGenerator.GenerateBossQuest(bossList[i]));
                        addedBossLevel = true;
                        break;
                    }
                }
            }
            if (!addedBossLevel)
            {
                //Quest difficulty should +/- 4 levels of player level, but not less than 1
                int min = Math.Min(MAX_STARTING_QUEST_LEVEL, player1.level);
                int max = Math.Max(MAX_STARTING_QUEST_LEVEL, player1.level) + 1;
                int questLevel = random.Next(min, max);
                currentQuestList.Add(questGenerator.GenerateQuest(questLevel));
            }
        }
        private void RemoveQuestFromQuestList(Quest aQuest)
        {
            for (int i = currentQuestList.Count - 1; i >= 0; i--)
            {
                if (aQuest.Equals(currentQuestList[i]))
                {
                    currentQuestList.RemoveAt(i);
                    break;
                }
            }
        }

        //Method to replace one quest in list with a guaranteed easy quest
        private void PlaceEasyQuestInQuestList()
        {
            int index = 0;
            if (currentQuestList.Count == 0) return;
            if (currentQuestList[index].isBossQuest)
            {
                index += 1;
                if (currentQuestList.Count == 1) return;
            }
            int handicap = (int)Math.Max(10, Math.Floor(player1.level / 10));
            currentQuestList[index] = questGenerator.GenerateQuest(Math.Max(1, player1.level - handicap));
        }

        public void InitOutro()
        {
            outroTiles = new byte[outroWidth][];
            for (int i = 0; i < outroWidth; i++)
            {
                outroTiles[i] = new byte[outroHeight];
                for (int j = 0; j < outroHeight; j++)
                {
                    if (random.NextDouble() < 0.05)
                    {
                        outroTiles[i][j] = TILE_WALL;
                    }
                    else
                    {
                        outroTiles[i][j] = TILE_VOID;
                    }
                }
            }
            outroTileSet = random.Next(0, NUM_TILE_SETS);
        }

        public void UpdateOutro()
        {

            currentFrameTextShiftDelay++;
            if (GamePad.GetState(playerIndex).Buttons.A == ButtonState.Pressed) currentFrameTextShiftDelay++;
            if (currentFrameTextShiftDelay >= FRAMES_PER_TEXT_SHIFT)
            {
                currentFrameTextShiftDelay = 0;
                if (outroTextOffset < MAX_INT) outroTextOffset += 2;

                if (outroTextOffset % 4 == 0)
                {
                    int cellChanges = 0;
                    for (int i = 0; i < outroWidth; i++)
                    {
                        for (int j = 0; j < outroHeight; j++)
                        {
                            if (outroTiles[i][j] == TILE_VOID)
                            {
                                if (OutroCellNumNeighbors(i, j) > 2)
                                {
                                    outroTiles[i][j] = TILE_WALL;
                                    cellChanges++;
                                }
                            }
                            else
                            {
                                if (OutroCellNumNeighbors(i, j) > 3)
                                {
                                    outroTiles[i][j] = TILE_VOID;
                                    cellChanges++;
                                }
                            }
                        }
                    }

                    if (cellChanges < 2 || outroTextOffset % 300 == 0) InitOutro();
                }
            }
            if (GamePad.GetState(playerIndex).Buttons.Start == ButtonState.Pressed
                && lastButtons.Buttons.Start == ButtonState.Released)
            {
                currentGameState = GAMESTATE_TITLESCREEN;
                MediaPlayer.Stop();
                MediaPlayer.Play(songTitle);
                settingsLoadComplete = false;
            }
        }

        private int OutroCellNumNeighbors(int x, int y)
        {
            int result = 0;

            if (x > 0)
            {
                if (y > 0)
                {
                    if (outroTiles[x - 1][y - 1] == TILE_WALL) result++;
                }
                if (outroTiles[x - 1][y] == TILE_WALL) result++;
                if (y < outroHeight - 1)
                {
                    if (outroTiles[x - 1][y + 1] == TILE_WALL) result++;
                }
            }
            if (y > 0)
            {
                if (outroTiles[x][y - 1] == TILE_WALL) result++;
            }
            if (y < outroHeight - 1)
            {
                if (outroTiles[x][y + 1] == TILE_WALL) result++;
            }
            if (x < outroWidth - 1)
            {
                if (y > 0)
                {
                    if (outroTiles[x + 1][y - 1] == TILE_WALL) result++;
                }
                if (outroTiles[x + 1][y] == TILE_WALL) result++;
                if (y < outroHeight - 1)
                {
                    if (outroTiles[x + 1][y + 1] == TILE_WALL) result++;
                }
            }

            return result;
        }

        private int TotalHoursPlayed()
        {
            return (int)Math.Floor(totalSecondsPlayed / 3600);
        }
        private int TotalMinutesPlayed()
        {
            return (int)Math.Floor((totalSecondsPlayed % 3600) / 60);
        }
        private int TotalSecondsPlayed()
        {
            return (int)(totalSecondsPlayed % 60);
        }

        //return the highest level of defeated boss levels
        //return 0 if no bosses beaten yet
        private int LevelOfLastDefeatedBoss()
        {
            int result = 0;
            for (int i = 0; i < bossList.Count; i++)
            {
                if (bossList[i].beaten && bossList[i].level > result)
                    result = bossList[i].level;
            }
            return result;
        }

        //return lowest level of undefeated bosses
        //return 0 if all bosses beated
        private int LevelOfNextUndefeatedBoss()
        {
            int result = MAX_LEVELS + 1;
            for (int i = 0; i < bossList.Count; i++)
            {
                if (!bossList[i].beaten && bossList[i].level < result)
                    result = bossList[i].level;
            }
            if (result == MAX_LEVELS + 1) return 0;
            return result;
        }

        private void DrawLogoScreen()
        {
            spriteBatch.Begin();
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.Clear(Color.White);

            if (logoScreenTimer < 120)
            {
                int delta = (120 - logoScreenTimer) * 8;
                spriteBatch.Draw(tx68KLogo, new Rectangle(240 - delta, 300 - delta, 792 + delta * 2, 103 + delta * 2), Color.White);
            }
            else
            {
                spriteBatch.Draw(tx68KLogo, new Rectangle(240, 300, 792, 103), Color.White);
            }

            if (logoScreenTimer < 300)
            {
                for (int i = 0; i < DISPLAYWIDTH; i = i + 8)
                {
                    for (int j = 0; j < DISPLAYHEIGHT; j = j + 8)
                    {
                        if (random.Next(0, 300) > logoScreenTimer)
                        {
                            byte shade = (byte)random.Next(256);
                            byte tint = (byte)(255 - Math.Min(255, random.Next(logoScreenTimer)));
                            spriteBatch.Draw(txWhite, new Rectangle(i, j, 8, 8), new Color(shade, shade, shade, tint));
                        }
                    }
                }
            }

            if (logoScreenTimer > logoScreenTimeMax - 120)
            {
                byte alpha = (byte)(255 - ((logoScreenTimeMax - logoScreenTimer) * 2));
                spriteBatch.Draw(txBlack, new Rectangle(0, 0, DISPLAYWIDTH, DISPLAYHEIGHT), new Color(0, 0, 0, alpha));
            }


            spriteBatch.End();
        }

        private void DrawOrawartLogo()
        {
            spriteBatch.Begin();
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.Clear(Color.Black);

            if (logoScreenTimer < 120)
            {
                int delta = logoScreenTimer * 2;
                spriteBatch.Draw(txOrawartLogo, new Rectangle(350, 0, 509, 720), new Color(delta, delta, delta, delta));
            }
            else
            {
                spriteBatch.Draw(txOrawartLogo, new Rectangle(350, 0, 509, 720), Color.White);
            }
            spriteBatch.End();
        }

        private void DrawMusicLogo()
        {
            spriteBatch.Begin();
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.Clear(Color.Black);

            if (logoScreenTimer < 120)
            {
                int delta = logoScreenTimer * 2;
                spriteBatch.Draw(txMusicLogo, new Rectangle(200, 100, 900, 494), new Color(delta, delta, delta, delta));
            }
            else
            {
                spriteBatch.Draw(txMusicLogo, new Rectangle(200, 100, 900, 494), Color.White);
            }
            spriteBatch.End();
        }

        private void DrawStorageWarningScreen()
        {
            spriteBatch.Begin();
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.Clear(Color.DarkGray);

            spriteBatch.DrawString(sfFranklinGothic, "This game saves and loads data automatically.\n"
                                                    + "When you see the symbol below, do not turn off\n"
                                                    + "your console or unplug any devices.", new Vector2(DISPLAYWIDTH / 2 - 200, DISPLAYHEIGHT / 2 - 150), Color.Yellow);
            DrawStorageAccessNotification(DISPLAYWIDTH / 2 - 60, DISPLAYHEIGHT / 2 - 40, 255);
            spriteBatch.End();
        }

        private void DrawTitleScreen()
        {
            spriteBatch.Begin();
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            /*GraphicsDevice.RenderState.AlphaBlendEnable = true;
            GraphicsDevice.RenderState.DestinationBlend = Blend.One;
            GraphicsDevice.RenderState.SourceBlend = Blend.One;*/

            GraphicsDevice.Clear(new Color(220, 220, 220));
            spriteBatch.Draw(txTitleScreenLogo, new Rectangle(0, 0, DISPLAYWIDTH, DISPLAYHEIGHT), Color.White);

            if (!resumableGameSaving)
            {
                if (titleScreenStatus.Equals("Press Start"))
                {
                    spriteBatch.Draw(txBlack, new Rectangle(DISPLAYWIDTH / 2 - 122, DISPLAYHEIGHT - 160, 210, 50), new Color(180, 180, 180, 180));
                    spriteBatch.DrawString(sfSylfaen14, "PRESS START", new Vector2(DISPLAYWIDTH / 2 - 116, DISPLAYHEIGHT - 150), Color.White);
                    spriteBatch.Draw(btnTxStart, new Rectangle(DISPLAYWIDTH / 2 + 44, DISPLAYHEIGHT - 155, 40, 40), Color.White);
                }
                if (titleScreenStatus.Equals("Please Press       And Sign In"))
                {
                    spriteBatch.Draw(txBlack, new Rectangle(DISPLAYWIDTH / 2 - 190, DISPLAYHEIGHT - 182, 380, 82), new Color(180, 180, 180, 180));
                    spriteBatch.DrawString(sfSylfaen14, "PLEASE PRESS       AND SIGN IN\nTHEN PRESS START", new Vector2(DISPLAYWIDTH / 2 - 180, DISPLAYHEIGHT - 170), Color.White);
                    spriteBatch.Draw(btnTxGuide, new Rectangle(DISPLAYWIDTH / 2 - 10, DISPLAYHEIGHT - 175, 40, 40), Color.White);
                }
                if (titleScreenStatus.Equals("PLAYER HAS SIGNED OUT\n\nPLEASE PRESS START"))
                {
                    spriteBatch.Draw(txBlack, new Rectangle(DISPLAYWIDTH / 2 - 170, DISPLAYHEIGHT - 180, 340, 90), new Color(180, 180, 180, 180));
                    spriteBatch.DrawString(sfSylfaen14, "PLAYER HAS SIGNED OUT\nPLEASE PRESS START", new Vector2(DISPLAYWIDTH / 2 - 150, DISPLAYHEIGHT - 170), Color.White);
                    spriteBatch.Draw(btnTxStart, new Rectangle(DISPLAYWIDTH / 2 + 110, DISPLAYHEIGHT - 140, 40, 40), Color.White);
                }
            }

            GraphicsDevice.BlendState = BlendState.Opaque;
            spriteBatch.End();
        }

        private void DrawMainMenu()
        {
            spriteBatch.Begin();
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            /*GraphicsDevice.RenderState.AlphaBlendEnable = true;
            GraphicsDevice.RenderState.DestinationBlend = Blend.One;
            GraphicsDevice.RenderState.SourceBlend = Blend.One;*/

            GraphicsDevice.Clear(new Color(0, 0, 50, 255));
            for (int i = 0; i < 42; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    if (j % 2 == 0)
                    {
                        spriteBatch.Draw(txTileWall, new Rectangle(-BACKGROUND_TILE_SIZE + i * BACKGROUND_TILE_SIZE + creationScreenBackgroundOffset,
                                                                    -BACKGROUND_TILE_SIZE + j * BACKGROUND_TILE_SIZE + creationScreenBackgroundOffset,
                                                                    BACKGROUND_TILE_SIZE, BACKGROUND_TILE_SIZE), new Color(255, 255, 255, 120));
                    }
                    else
                    {
                        spriteBatch.Draw(txTileWall, new Rectangle(-BACKGROUND_TILE_SIZE + i * BACKGROUND_TILE_SIZE + creationScreenBackgroundOffset,
                                                                    -BACKGROUND_TILE_SIZE + j * BACKGROUND_TILE_SIZE + creationScreenBackgroundOffset,
                                                                    BACKGROUND_TILE_SIZE, BACKGROUND_TILE_SIZE), new Color(255, 255, 255, 120));
                    }
                }
            }

            if (!dialogOpen)
            {
                for (int i = 0; i < mainMenuStringList.Count; i++)
                {
                    Color displayColor = Color.White;
                    if (mainMenuStringList[i] == "Resume" && !resumableGameExists) displayColor = Color.Gray;
                    spriteBatch.DrawString(sfSylfaen14, mainMenuStringList[i], new Vector2(DISPLAYWIDTH / 2 - 100, DISPLAYHEIGHT / 2 - 100 + 30 * i), displayColor);
                }
                spriteBatch.Draw(txWhite, new Rectangle(DISPLAYWIDTH / 2 - 102, DISPLAYHEIGHT / 2 - 100 + 30 * currentMainMenuSelection, 210, 30), new Color(100, 100, 100, 80));
                if (showingCannotBuyWarning)
                {
                    spriteBatch.DrawString(sfSylfaen14, "SORRY - You need an XBox Live! profile that can purchase Marketplace content", new Vector2(150, DISPLAYHEIGHT - 200), Color.LightSalmon);
                }
            }

            if (dialogOpen)
            {
                spriteBatch.Draw(txBlack, new Rectangle(0, 0, DISPLAYWIDTH, DISPLAYHEIGHT), new Color(255, 255, 255, 80));

                if (totalStatsOpen)
                {
                    if (totalStats == null || totalStats.statList.Count == 0)
                    {
                        spriteBatch.DrawString(sfSylfaen14, "No Stats! Get Playing!", new Vector2(DISPLAYWIDTH / 2 - 90, DISPLAYHEIGHT / 2), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(txBlack, new Rectangle(200, 100, DISPLAYWIDTH - 400, DISPLAYHEIGHT - 200), new Color(200, 200, 200, 120));
                        //TODO - Querying profile name every frame, don't do this!!!
                        spriteBatch.DrawString(sfSylfaen14, "Stats - " + PlayerProfileString(), new Vector2(500, 120), Color.White);
                        spriteBatch.DrawString(sfSylfaen14, "Total Play Time: " + TotalHoursPlayed() + ":" + TotalMinutesPlayed().ToString().PadLeft(2, '0') + ":" + TotalSecondsPlayed().ToString().PadLeft(2, '0'), new Vector2(500, 150), Color.White);
                        for (int i = currentStatOffset; i < Math.Min(currentStatOffset + STATS_PER_PAGE, totalStats.statList.Count); i++)
                        {
                            spriteBatch.DrawString(sfFranklinGothic, totalStats.statList[i].name + " - " + totalStats.statList[i].value.ToString(), new Vector2(210, 190 + (i - currentStatOffset) * 21), Color.White);
                        }
                        if (totalStats.statList.Count > STATS_PER_PAGE)
                        {
                            for (int i = STATS_PER_PAGE; i < Math.Min(STATS_PER_PAGE * 2, totalStats.statList.Count); i++)
                            {
                                spriteBatch.DrawString(sfFranklinGothic, totalStats.statList[i].name + " - " + totalStats.statList[i].value.ToString(), new Vector2(600, 190 + (i - STATS_PER_PAGE - currentStatOffset) * 21), Color.White);
                            }
                        }
                    }
                    spriteBatch.Draw(btnTxButtonB, new Rectangle(DISPLAYWIDTH - 300, DISPLAYHEIGHT - 150, 24, 24), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Close", new Vector2(DISPLAYWIDTH - 270, DISPLAYHEIGHT - 150), Color.White);
                }

                if (creditsOpen)
                {
                    spriteBatch.DrawString(sfSylfaen14, TITLE_STRING + " " + VERSION, new Vector2(DISPLAYWIDTH / 2 - 50, 120), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Created by James \"68ST0X20\" Stocks." +
                                                        "\nwww.68ST0X20.com\nwww.twitter.com/68ST0X20" +
                                                        "\n\nCharacter artwork by Garath O'Rawe ( www.orawart.com )" +
                                                        "\n\nMusic by Scheme Boy ( www.schemeboy.com ) and Citezen" +
                                                        "\n\nThanks To - Helpful People At create.msdn.com and LL for advice and playtesting.",
                                                        new Vector2(120, 180), Color.White);
                    spriteBatch.Draw(btnTxButtonB, new Rectangle(DISPLAYWIDTH - 300, DISPLAYHEIGHT - 150, 24, 24), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Close", new Vector2(DISPLAYWIDTH - 270, DISPLAYHEIGHT - 150), Color.White);
                }

                if (helpOpen)
                {
                    DrawHelp(currentHelpPage);
                }

                if (featSummaryOpen)
                {
                    DrawFeatSummaryPage();
                }

                if (showingExitConfirmation)
                {
                    spriteBatch.Draw(txBlack, new Rectangle(350, DISPLAYHEIGHT / 2 - 100, DISPLAYWIDTH - 700, 200), new Color(200, 200, 200, 120));
                    spriteBatch.DrawString(sfSylfaen14, "Really Exit?", new Vector2(550, DISPLAYHEIGHT / 2 - 94), Color.White);
                    spriteBatch.Draw(btnTxButtonY, new Rectangle(450, DISPLAYHEIGHT / 2 - 40, 32, 32), Color.White);
                    spriteBatch.Draw(btnTxButtonB, new Rectangle(450, DISPLAYHEIGHT / 2 + 40, 32, 32), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Exit To Dashboard", new Vector2(490, DISPLAYHEIGHT / 2 - 38), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Return To Main Menu", new Vector2(490, DISPLAYHEIGHT / 2 + 42), Color.White);
                }

                if (showingResumableExistsWarning)
                {
                    spriteBatch.Draw(txBlack, new Rectangle(350, DISPLAYHEIGHT / 2 - 200, DISPLAYWIDTH - 700, 400), new Color(200, 200, 200, 120));
                    spriteBatch.DrawString(sfSylfaen14, "WARNING", new Vector2(550, DISPLAYHEIGHT / 2 - 194), Color.Red);
                    spriteBatch.DrawString(sfSylfaen14, "A previous game has been suspended. If you \nbegin a New Game then the suspended \ncharacter and their current quest will be lost.\n\nAre You Sure?", new Vector2(374, DISPLAYHEIGHT / 2 - 150), Color.White);
                    spriteBatch.Draw(btnTxButtonY, new Rectangle(450, DISPLAYHEIGHT / 2 + 30, 32, 32), Color.White);
                    spriteBatch.Draw(btnTxButtonB, new Rectangle(450, DISPLAYHEIGHT / 2 + 99, 32, 32), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Start New Game Anyway", new Vector2(490, DISPLAYHEIGHT / 2 + 30), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Return To Main Menu", new Vector2(490, DISPLAYHEIGHT / 2 + 102), Color.White);
                }
            }

            spriteBatch.End();
        }

        private void DrawGameOverScreen()
        {
            spriteBatch.Begin();
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            GraphicsDevice.Clear(Color.DarkBlue);
            for (int i = 0; i < 42; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    spriteBatch.Draw(txTileEarth, new Rectangle(-32 + i * 32 + creationScreenBackgroundOffset,
                                                                -32 + j * 32 + creationScreenBackgroundOffset,
                                                                32, 32), new Color(255, 255, 255, 200));
                }
            }

            spriteBatch.Draw(txGameOverLogo, new Rectangle(DISPLAYWIDTH / 2 - 303, 50, 605, 188), new Color(255, 255, 255, 200));

            spriteBatch.DrawString(sfSylfaen14, "R.I.P. " + player1.name, new Vector2(DISPLAYWIDTH / 2 - 100, 250), Color.Yellow);
            spriteBatch.DrawString(sfSylfaen14, death, new Vector2(DISPLAYWIDTH / 2 - 260, 320), Color.Yellow);

            spriteBatch.DrawString(sfSylfaen14, "Tip: ", new Vector2(130, DISPLAYHEIGHT - 150), Color.White);
            spriteBatch.DrawString(sfSylfaen14, randomTips[selectedRandomTip], new Vector2(180, DISPLAYHEIGHT - 150), Color.White);

            GraphicsDevice.BlendState = BlendState.Opaque;
            spriteBatch.End();

        }

        private void DrawCharacterCreationScreen()
        {

            GraphicsDevice.BlendState = BlendState.NonPremultiplied;

            spriteBatch.Begin();

            GraphicsDevice.Clear(Color.DarkBlue);
            for (int i = 0; i < 42; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    spriteBatch.Draw(txTileWall, new Rectangle(-BACKGROUND_TILE_SIZE + i * BACKGROUND_TILE_SIZE + creationScreenBackgroundOffset,
                                                                -BACKGROUND_TILE_SIZE + j * BACKGROUND_TILE_SIZE + creationScreenBackgroundOffset,
                                                                BACKGROUND_TILE_SIZE, BACKGROUND_TILE_SIZE), new Color(128, 128, 128, 128));
                }
            }
            spriteBatch.Draw(txWhite, new Rectangle(100, 100, DISPLAYWIDTH - 200, DISPLAYHEIGHT - 200), Color.White);
            spriteBatch.Draw(txBlue, new Rectangle(102, 102, DISPLAYWIDTH - 204, DISPLAYHEIGHT - 204), Color.White);

            spriteBatch.DrawString(sfSylfaen14, "CHARACTER CREATION", new Vector2(DISPLAYWIDTH / 2 - 150, 120), Color.White);
            spriteBatch.DrawString(sfSylfaen14, "Name: \n" + player1.name, new Vector2(250, 180), Color.Gray);
            spriteBatch.DrawString(sfSylfaen14, "Profession: \nFighter        Mage          Explorer", new Vector2(250, 260), Color.Gray);
            spriteBatch.DrawString(sfSylfaen14, "Race: \nOrc             Human        Elf", new Vector2(250, 340), Color.Gray);
            spriteBatch.DrawString(sfSylfaen14, "Show Tutorial Messages?\nYes    No", new Vector2(250, 426), Color.Gray);

            switch (currentCharacterCreationOption)
            {
                case CHAR_CREATE_OPTION_NAME:
                    spriteBatch.DrawString(sfSylfaen14, "Name: \n" + player1.name, new Vector2(250, 180), Color.White);
                    spriteBatch.Draw(txWhite, new Rectangle(246, 210, 300, 30), new Color(100, 100, 100, 80));
                    break;
                case CHAR_CREATE_OPTION_PROFESSION:
                    spriteBatch.DrawString(sfSylfaen14, "Profession: \nFighter        Mage          Explorer", new Vector2(250, 260), Color.White);
                    break;
                case CHAR_CREATE_OPTION_RACE:
                    spriteBatch.DrawString(sfSylfaen14, "Race: \nOrc             Human        Elf", new Vector2(250, 340), Color.White);
                    break;
                case CHAR_CREATE_OPTION_TUTORIALMESSAGES:
                    spriteBatch.DrawString(sfSylfaen14, "Show Tutorial Messages?\nYes    No", new Vector2(250, 426), Color.White);
                    break;
            }

            spriteBatch.DrawString(sfSylfaen14, "Press         To Begin", new Vector2(DISPLAYWIDTH / 2 - 75, DISPLAYHEIGHT - 80), Color.White);
            spriteBatch.Draw(btnTxStart, new Rectangle(DISPLAYWIDTH / 2 - 10, DISPLAYHEIGHT - 90, 40, 40), Color.White);

            spriteBatch.Draw(btnTxButtonB, new Rectangle(290, DISPLAYHEIGHT - 80, 26, 26), Color.White);
            if (changingStartingLevel)
            {
                spriteBatch.DrawString(sfSylfaen14, "Done.", new Vector2(210, DISPLAYHEIGHT - 80), Color.White);
            }
            else
            {
                spriteBatch.DrawString(sfSylfaen14, "Main Menu", new Vector2(150, DISPLAYHEIGHT - 80), Color.White);
            }

            if (player1.profession == (int)SC.Professions.FIGHTER)
            {
                spriteBatch.Draw(txWhite, new Rectangle(246, 294, 95, 30), new Color(100, 100, 100, 80));
            }
            if (player1.profession == (int)SC.Professions.MAGE)
            {
                spriteBatch.Draw(txWhite, new Rectangle(376, 294, 75, 30), new Color(100, 100, 100, 80));
            }
            if (player1.profession == (int)SC.Professions.EXPLORER)
            {
                spriteBatch.Draw(txWhite, new Rectangle(506, 294, 105, 30), new Color(100, 100, 100, 80));
            }
            if (player1.race == (int)SC.Races.ORC)
            {
                spriteBatch.Draw(txWhite, new Rectangle(246, 372, 65, 30), new Color(100, 100, 100, 80));
            }
            if (player1.race == (int)SC.Races.HUMAN)
            {
                spriteBatch.Draw(txWhite, new Rectangle(376, 372, 100, 30), new Color(100, 100, 100, 80));
            }
            if (player1.race == (int)SC.Races.ELF)
            {
                spriteBatch.Draw(txWhite, new Rectangle(506, 372, 60, 30), new Color(100, 100, 100, 80));
            }
            if (!neverSeeTutorialMessages)
            {
                spriteBatch.Draw(txWhite, new Rectangle(248, 460, 44, 22), new Color(100, 100, 100, 80));
            }
            else
            {
                spriteBatch.Draw(txWhite, new Rectangle(316, 460, 36, 22), new Color(100, 100, 100, 80));
            }

            //Draw large version of player sprite
            Texture2D largePlayer = txLargePlayerHuman;
            switch (player1.race)
            {
                case (int)SC.Races.ELF:
                    largePlayer = txLargePlayerElf;
                    break;
                case (int)SC.Races.HUMAN:
                    largePlayer = txLargePlayerHuman;
                    break;
                case (int)SC.Races.ORC:
                    largePlayer = txLargePlayerOrc;
                    break;
            }
            spriteBatch.Draw(largePlayer, new Rectangle(DISPLAYWIDTH - largePlayer.Width - 100, 25, largePlayer.Width, largePlayer.Height), Color.White);

            spriteBatch.End();
        }

        private void DrawGamePlaying()
        {
            GraphicsDevice.SetRenderTarget(playFieldRenderTarget);
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            GraphicsDevice.Clear(clearColor);

            if (!resumableGameLoading)
            {
                //spriteBatch.Begin();
                //The Begin(...) below will render the sprites blocky
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
                //Draw the map
                byte[,] tilesToDraw = currentLevel.GetTiles(player1.x - halfTilesInViewX,
                                                            player1.y - halfTilesInViewY,
                                                            tilesInViewX, tilesInViewY);
                int[,] visibility = currentLevel.GetTileVision(player1.x - halfTilesInViewX,
                                                            player1.y - halfTilesInViewY,
                                                            tilesInViewX, tilesInViewY);
                for (int i = 0; i < tilesInViewX; i++)
                {
                    for (int j = 0; j < tilesInViewY; j++)
                    {
                        Texture2D tileToDraw = txTileWall;
                        if (!inVillage)
                        {
                            if (tilesToDraw[i, j] == TILE_EARTH)
                            {
                                tileToDraw = txTileEarth;
                            }
                            if (tilesToDraw[i, j] == TILE_WALL)
                            {
                                switch (currentLevel.currentTileSet)
                                {
                                    case 0:
                                        tileToDraw = txTileWall;
                                        break;
                                    case 1:
                                        tileToDraw = txTileSet1Wall;
                                        break;
                                    case 2:
                                        tileToDraw = txTileSet2Wall;
                                        break;
                                    case 3:
                                        tileToDraw = txTileSet3Wall;
                                        break;
                                    case 4:
                                        tileToDraw = txTileSet4Wall;
                                        break;
                                    case 5:
                                        tileToDraw = txTileSet5Wall;
                                        break;
                                    default:
                                        tileToDraw = txTileWall;
                                        break;
                                }
                            }
                            if (tilesToDraw[i, j] == TILE_VOID)
                            {
                                tileToDraw = txBlack;
                            }
                            if (tilesToDraw[i, j] == TILE_DOOR_CLOSED)
                            {
                                switch (currentLevel.currentTileSet)
                                {
                                    case 0:
                                        tileToDraw = txTileDoorClosed;
                                        break;
                                    case 1:
                                        tileToDraw = txTileSet1DoorClosed;
                                        break;
                                    case 2:
                                        tileToDraw = txTileSet2DoorClosed;
                                        break;
                                    case 3:
                                        tileToDraw = txTileSet3DoorClosed;
                                        break;
                                    case 4:
                                        tileToDraw = txTileSet4DoorClosed;
                                        break;
                                    case 5:
                                        tileToDraw = txTileSet5DoorClosed;
                                        break;
                                    default:
                                        tileToDraw = txTileDoorClosed;
                                        break;
                                }
                            }
                            if (tilesToDraw[i, j] == TILE_DOOR_OPEN)
                            {
                                switch (currentLevel.currentTileSet)
                                {
                                    case 0:
                                        tileToDraw = txTileDoorOpen;
                                        break;
                                    case 1:
                                        tileToDraw = txTileSet1DoorOpen;
                                        break;
                                    case 2:
                                        tileToDraw = txTileSet2DoorOpen;
                                        break;
                                    case 3:
                                        tileToDraw = txTileSet3DoorOpen;
                                        break;
                                    case 4:
                                        tileToDraw = txTileSet4DoorOpen;
                                        break;
                                    case 5:
                                        tileToDraw = txTileSet5DoorOpen;
                                        break;
                                    default:
                                        tileToDraw = txTileDoorOpen;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (tilesToDraw[i, j] == TILE_VILLAGE_DOOR_CLOSED) tileToDraw = txVillageDoorClosed;
                            if (tilesToDraw[i, j] == TILE_VILLAGE_DOOR_OPEN) tileToDraw = txVillageDoorOpen;
                            if (tilesToDraw[i, j] == TILE_VILLAGE_GRASS) tileToDraw = txVillageGrass;
                            if (tilesToDraw[i, j] == TILE_VILLAGE_HEDGE) tileToDraw = txVillageHedge;
                            if (tilesToDraw[i, j] == TILE_VILLAGE_PATH) tileToDraw = txVillagePath;
                            if (tilesToDraw[i, j] == TILE_VILLAGE_TREE) tileToDraw = txVillageTree;
                            if (tilesToDraw[i, j] == TILE_VILLAGE_WALL) tileToDraw = txVillageWall;
                            if (tilesToDraw[i, j] == TILE_VILLAGE_WATER) tileToDraw = txVillageWater;
                            if (tilesToDraw[i, j] == TILE_VILLAGE_FLOOR) tileToDraw = txVillageFloor;
                        }
                        if (tilesToDraw[i, j] == TILE_STAIRS_DOWN)
                        {
                            tileToDraw = txTileStairsDown;
                        }
                        if (tilesToDraw[i, j] == TILE_STAIRS_UP)
                        {
                            tileToDraw = txTileStairsUp;
                        }
                        spriteBatch.Draw(tileToDraw, new Rectangle(i * TILE_SIZE, j * TILE_SIZE, TILE_SIZE, TILE_SIZE), Color.White);
                    }
                }

                //Draw tile visibility
                //int darknessSize = (int)(TILE_SIZE * 1.5);
                //int darknessMargin = (int)(TILE_SIZE / 4);
                for (int i = 0; i < tilesInViewX; i++)
                {
                    for (int j = 0; j < tilesInViewY; j++)
                    {
                        spriteBatch.Draw(txBlack, new Rectangle(i * TILE_SIZE, j * TILE_SIZE, TILE_SIZE, TILE_SIZE), new Color(255, 255, 255, (byte)(255 - visibility[i, j])));
                        //spriteBatch.Draw(txEffectDarkness, new Rectangle(i * TILE_SIZE - darknessMargin, j * TILE_SIZE - darknessMargin, darknessSize, darknessSize), new Color(255 - visibility[i, j], 255 - visibility[i, j], 255 - visibility[i, j], 255 - visibility[i, j]));
                    }
                }
                //spriteBatch.Draw(shadowTexture, new Rectangle(0, 0, tilesInViewX * TILE_SIZE, tilesInViewY * TILE_SIZE), Color.White);

                //Traps
                List<Trap> trapsInView = currentLevel.GetTraps(player1.x - halfTilesInViewX,
                                                                player1.y - halfTilesInViewY,
                                                                tilesInViewX, tilesInViewY);
                if (trapsInView.Count > 0)
                {
                    Texture2D trapToDraw;
                    for (int i = 0; i < trapsInView.Count; i++)
                    {
                        if (trapsInView[i].discovered && !trapsInView[i].disarmed)
                        {
                            switch (trapsInView[i].trapType)
                            {
                                case (int)SC.TrapType.ARROW:
                                    trapToDraw = txTrapArrow;
                                    break;
                                case (int)SC.TrapType.DESCENT:
                                    trapToDraw = txTrapDescent;
                                    break;
                                case (int)SC.TrapType.POISON:
                                    trapToDraw = txTrapPoison;
                                    break;
                                case (int)SC.TrapType.TELEPORT:
                                    trapToDraw = txTrapTeleport;
                                    break;
                                default:
                                    trapToDraw = txTrapArrow;
                                    break;
                            }
                            spriteBatch.Draw(trapToDraw, new Rectangle(trapsInView[i].x * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE,
                                                                        trapsInView[i].y * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE,
                                                                        TILE_SIZE, TILE_SIZE),
                                                                        Color.White);
                        }
                    }
                }

                //Passible furniture that can be walked on should be drawn BEFORE items
                DrawListOfFurniture(currentLevel.GetPassibleFurniture(player1.x - halfTilesInViewX,
                                                            player1.y - halfTilesInViewY,
                                                            tilesInViewX, tilesInViewY));

                //Items
                List<Item> itemsInView = currentLevel.GetItems(player1.x - halfTilesInViewX,
                                                            player1.y - halfTilesInViewY,
                                                            tilesInViewX, tilesInViewY);
                if (itemsInView.Count > 0)
                {
                    Texture2D itemToDraw = txItemCoins;
                    Color colorToDraw;
                    for (int i = 0; i < itemsInView.Count; i++)
                    {
                        colorToDraw = new Color(1, 1, 1) * itemsInView[i].visibility;
                        switch (itemsInView[i].itemType)
                        {
                            case (int)SC.ItemTypes.ARMOUR:
                                itemToDraw = txItemArmour;
                                break;
                            case (int)SC.ItemTypes.GOLD:
                                itemToDraw = txItemCoins;
                                break;
                            case (int)SC.ItemTypes.SHIELD:
                                itemToDraw = txItemShield;
                                break;
                            case (int)SC.ItemTypes.WEAPON:
                                if (itemsInView[i].weaponType == (int)SC.WeaponTypes.SWORD) itemToDraw = txItemSword;
                                if (itemsInView[i].weaponType == (int)SC.WeaponTypes.DAGGER) itemToDraw = txItemDagger;
                                if (itemsInView[i].weaponType == (int)SC.WeaponTypes.MACE) itemToDraw = txItemMace;
                                if (itemsInView[i].weaponType == (int)SC.WeaponTypes.SPEAR) itemToDraw = txItemSpear;
                                if (itemsInView[i].weaponType == (int)SC.WeaponTypes.BOW) itemToDraw = txItemBow;
                                break;
                            case (int)SC.ItemTypes.CLOAK:
                                itemToDraw = txItemCloak;
                                break;
                            case (int)SC.ItemTypes.POTION:
                                itemToDraw = txItemPotion;
                                if (itemsInView[i].itemName == "Potion of Healing")
                                {
                                    colorToDraw = new Color(itemsInView[i].visibility, 0, 0);
                                }
                                if (itemsInView[i].itemName == "Magic Potion")
                                {
                                    colorToDraw = new Color(0, 0, itemsInView[i].visibility);
                                }
                                if (itemsInView[i].itemName == "Antidote")
                                {
                                    colorToDraw = new Color(itemsInView[i].visibility, 0, itemsInView[i].visibility);
                                }
                                if (itemsInView[i].itemName == "Attack Potion")
                                {
                                    colorToDraw = new Color(itemsInView[i].visibility, itemsInView[i].visibility, 0);
                                }
                                if (itemsInView[i].itemName == "Vitality Elixir")
                                {
                                    colorToDraw = new Color(itemsInView[i].visibility, 0, 0);
                                    itemToDraw = txItemSuperPotion;
                                }
                                if (itemsInView[i].itemName == "Will Elixir")
                                {
                                    colorToDraw = new Color(0, 0, itemsInView[i].visibility);
                                    itemToDraw = txItemSuperPotion;
                                }
                                break;
                            case (int)SC.ItemTypes.SCROLL:
                                itemToDraw = txItemScroll;
                                break;
                            case (int)SC.ItemTypes.FOOD:
                                if (itemsInView[i].canned)
                                {
                                    itemToDraw = txItemFoodCanned;
                                }
                                else
                                {
                                    switch (itemsInView[i].foodType)
                                    {
                                        case (int)SC.FoodTypes.BREAD:
                                            itemToDraw = txItemFoodBread;
                                            break;
                                        case (int)SC.FoodTypes.MEAT:
                                            itemToDraw = txItemFoodMeat;
                                            break;
                                        case (int)SC.FoodTypes.VEG:
                                            itemToDraw = txItemFoodVeg;
                                            break;
                                        case (int)SC.FoodTypes.CORPSE:
                                            itemToDraw = txItemFoodCorpse;
                                            break;
                                        default:
                                            itemToDraw = txItemFoodBread;
                                            break;
                                    }
                                }
                                break;
                            case (int)SC.ItemTypes.ROCK:
                                switch (itemsInView[i].rockType)
                                {
                                    case (int)SC.RockTypes.GEM:
                                        itemToDraw = txRockGem;
                                        break;
                                    case (int)SC.RockTypes.METAL:
                                        itemToDraw = txRockMetal;
                                        break;
                                    case (int)SC.RockTypes.ROCK:
                                        itemToDraw = txRockRock;
                                        break;
                                    case (int)SC.RockTypes.GRINDSTONE:
                                        itemToDraw = txRockGrindStone;
                                        break;
                                    default:
                                        itemToDraw = txRockRock;
                                        break;
                                }
                                break;
                            case (int)SC.ItemTypes.QUEST:
                                itemToDraw = txItemQuest;
                                break;
                            default:
                                itemToDraw = txItemQuest;
                                break;
                        }
                        spriteBatch.Draw(itemToDraw, new Rectangle(itemsInView[i].x * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE,
                                                                    itemsInView[i].y * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE,
                                                                    TILE_SIZE, TILE_SIZE), colorToDraw);
                    }
                }

                //Impassible furniture should be drawn AFTER items
                DrawListOfFurniture(currentLevel.GetImpassibleFurniture(player1.x - halfTilesInViewX,
                                                            player1.y - halfTilesInViewY,
                                                            tilesInViewX, tilesInViewY));

                //Creatures
                List<Creature> creaturesInView = currentLevel.GetCreatures(player1.x - halfTilesInViewX,
                                                            player1.y - halfTilesInViewY,
                                                            tilesInViewX, tilesInViewY);
                if (creaturesInView.Count > 0)
                {
                    Texture2D creatureToDraw = txCreatureRodent;
                    Color tint = Color.White;
                    for (int i = 0; i < creaturesInView.Count; i++)
                    {
                        if (currentCreatureTint == CREATURE_TINT_PERIOD)
                        {
                            if (creaturesInView[i].frozen)
                            {
                                tint = new Color(20, 20, 250, creaturesInView[i].visibility);
                            }
                            else
                            {
                                //tint = new Color(255, 255, 255, creaturesInView[i].visibility);
                                tint = new Color(1, 1, 1) * creaturesInView[i].visibility;
                            }
                        }
                        else
                        {
                            //tint = new Color(255, 255, 255, creaturesInView[i].visibility);
                            tint = new Color(1, 1, 1) * creaturesInView[i].visibility;
                        }
                        switch (creaturesInView[i].description.creatureType)
                        {
                            case (int)SC.CreatureTypes.BAT:
                                creatureToDraw = txCreatureBat;
                                break;
                            case (int)SC.CreatureTypes.BUG:
                                creatureToDraw = txCreatureBug;
                                break;
                            case (int)SC.CreatureTypes.LIZARD:
                                creatureToDraw = txCreatureLizard;
                                break;
                            case (int)SC.CreatureTypes.RODENT:
                                creatureToDraw = txCreatureRodent;
                                break;
                            case (int)SC.CreatureTypes.CANINE:
                                creatureToDraw = txCreatureCanine;
                                break;
                            case (int)SC.CreatureTypes.MERCHANT:
                                creatureToDraw = txCreatureMerchant;
                                break;
                            case (int)SC.CreatureTypes.DRAGON:
                                creatureToDraw = txCreatureDragon;
                                break;
                            case (int)SC.CreatureTypes.GOBLIN:
                                creatureToDraw = txCreatureGoblin;
                                break;
                            case (int)SC.CreatureTypes.SNAKE:
                                creatureToDraw = txCreatureSnake;
                                break;
                            case (int)SC.CreatureTypes.SUCCUBUS:
                                creatureToDraw = txCreatureSuccubus;
                                break;
                            case (int)SC.CreatureTypes.SPIDER:
                                creatureToDraw = txCreatureSpider;
                                break;
                            case (int)SC.CreatureTypes.HUMAN:
                                creatureToDraw = txCreatureHuman;
                                break;
                            case (int)SC.CreatureTypes.DEMON:
                                creatureToDraw = txCreatureDemon;
                                break;
                            case (int)SC.CreatureTypes.SKELETON:
                                creatureToDraw = txCreatureSkeleton;
                                break;
                            case (int)SC.CreatureTypes.GHOST:
                                creatureToDraw = txCreatureGhost;
                                break;
                            case (int)SC.CreatureTypes.SLIME:
                                creatureToDraw = txCreatureSlime;
                                break;
                            case (int)SC.CreatureTypes.TROLL:
                                creatureToDraw = txCreatureTroll;
                                break;
                            case (int)SC.CreatureTypes.GOLDRICK:
                                creatureToDraw = txCreatureGoldrick;
                                break;
                            case (int)SC.CreatureTypes.ZOMBIE:
                                creatureToDraw = txCreatureZombie;
                                break;
                            case (int)SC.CreatureTypes.CHEF:
                                creatureToDraw = txCreatureCook;
                                break;
                            case (int)SC.CreatureTypes.VILLAGER:
                                creatureToDraw = txCreatureVillager;
                                break;
                            case (int)SC.CreatureTypes.QUESTMASTER:
                                creatureToDraw = txCreatureQuestMaster;
                                break;
                            case (int)SC.CreatureTypes.QUESTRESCUEE:
                                creatureToDraw = txCreatureVillager;
                                break;
                            default:
                                creatureToDraw = txItemFoodCanned;
                                break;
                        }
                        if (creaturesInView[i].allied)
                        {
                            spriteBatch.Draw(txAlliedIndicator, new Rectangle(creaturesInView[i].x * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE,
                                                                    creaturesInView[i].y * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE,
                                                                    TILE_SIZE * creaturesInView[i].description.width, TILE_SIZE * creaturesInView[i].description.height),
                                                                    new Color(255, 255, 255, guiArrowTint));
                        }
                        if (currentQuest != null
                            && currentQuest.questType == (int)SC.QuestType.ASSASSINATE
                            && !currentQuest.isBossQuest)
                        {
                            if (creaturesInView[i] == currentQuest.questCreature)
                            {
                                spriteBatch.Draw(txHostileIndicator, new Rectangle(creaturesInView[i].x * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE,
                                                                    creaturesInView[i].y * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE,
                                                                    TILE_SIZE * creaturesInView[i].description.width, TILE_SIZE * creaturesInView[i].description.height),
                                                                    new Color(255, 255, 255, guiArrowTint));
                            }
                        }
                        int extraPixels = (int)((1f - 1f / creaturesInView[i].description.displaySize) * TILE_SIZE);
                        spriteBatch.Draw(creatureToDraw, new Rectangle(creaturesInView[i].x * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE - extraPixels / 2,
                                                                    creaturesInView[i].y * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE - extraPixels / 2,
                                                                    TILE_SIZE * creaturesInView[i].description.width + extraPixels,
                                                                    TILE_SIZE * creaturesInView[i].description.height + extraPixels),
                                                                    tint);
                    }

                }

                if (player1.alive)
                {
                    if (player1.race == (int)SC.Races.ELF)
                    {
                        switch (player1.directionFacing)
                        {
                            case SC.DIRECTION_DOWN:
                                txPlayer = txPlayerElf;
                                break;
                            case SC.DIRECTION_DOWNLEFT:
                                txPlayer = txPlayerElf_Left;
                                break;
                            case SC.DIRECTION_DOWNRIGHT:
                                txPlayer = txPlayerElf_Right;
                                break;
                            case SC.DIRECTION_LEFT:
                                txPlayer = txPlayerElf_Left;
                                break;
                            case SC.DIRECTION_NONE:
                                txPlayer = txPlayerElf;
                                break;
                            case SC.DIRECTION_RIGHT:
                                txPlayer = txPlayerElf_Right;
                                break;
                            case SC.DIRECTION_UP:
                                txPlayer = txPlayerElf_Back;
                                break;
                            case SC.DIRECTION_UPLEFT:
                                txPlayer = txPlayerElf_Left;
                                break;
                            case SC.DIRECTION_UPRIGHT:
                                txPlayer = txPlayerElf_Right;
                                break;
                            default:
                                txPlayer = txPlayerElf;
                                break;
                        }
                    }
                    if (player1.race == (int)SC.Races.ORC)
                    {
                        switch (player1.directionFacing)
                        {
                            case SC.DIRECTION_DOWN:
                                txPlayer = txPlayerOrc;
                                break;
                            case SC.DIRECTION_DOWNLEFT:
                                txPlayer = txPlayerOrc_Left;
                                break;
                            case SC.DIRECTION_DOWNRIGHT:
                                txPlayer = txPlayerOrc_Right;
                                break;
                            case SC.DIRECTION_LEFT:
                                txPlayer = txPlayerOrc_Left;
                                break;
                            case SC.DIRECTION_NONE:
                                txPlayer = txPlayerOrc;
                                break;
                            case SC.DIRECTION_RIGHT:
                                txPlayer = txPlayerOrc_Right;
                                break;
                            case SC.DIRECTION_UP:
                                txPlayer = txPlayerOrc_Back;
                                break;
                            case SC.DIRECTION_UPLEFT:
                                txPlayer = txPlayerOrc_Left;
                                break;
                            case SC.DIRECTION_UPRIGHT:
                                txPlayer = txPlayerOrc_Right;
                                break;
                            default:
                                txPlayer = txPlayerOrc;
                                break;
                        }
                    }
                    if (player1.race == (int)SC.Races.HUMAN)
                    {
                        switch (player1.directionFacing)
                        {
                            case SC.DIRECTION_DOWN:
                                txPlayer = txPlayerHuman;
                                break;
                            case SC.DIRECTION_DOWNLEFT:
                                txPlayer = txPlayerHuman_Left;
                                break;
                            case SC.DIRECTION_DOWNRIGHT:
                                txPlayer = txPlayerHuman_Right;
                                break;
                            case SC.DIRECTION_LEFT:
                                txPlayer = txPlayerHuman_Left;
                                break;
                            case SC.DIRECTION_NONE:
                                txPlayer = txPlayerHuman;
                                break;
                            case SC.DIRECTION_RIGHT:
                                txPlayer = txPlayerHuman_Right;
                                break;
                            case SC.DIRECTION_UP:
                                txPlayer = txPlayerHuman_Back;
                                break;
                            case SC.DIRECTION_UPLEFT:
                                txPlayer = txPlayerHuman_Left;
                                break;
                            case SC.DIRECTION_UPRIGHT:
                                txPlayer = txPlayerHuman_Right;
                                break;
                            default:
                                txPlayer = txPlayerHuman;
                                break;
                        }
                    }
                    spriteBatch.Draw(txPlayer, new Rectangle(TILE_SIZE * halfTilesInViewX, TILE_SIZE * halfTilesInViewY, TILE_SIZE, TILE_SIZE), Color.White);
                    if (player1.attackBuffTurnsLeft > 10)
                    {
                        spriteBatch.Draw(txGUIArrowUp, new Rectangle(TILE_SIZE * halfTilesInViewX, TILE_SIZE * halfTilesInViewY, TILE_SIZE / 4, TILE_SIZE / 4), new Color(255, 0, 0, guiArrowTint));
                    }
                }

                //Draw particles
                if (particleClusters.Count > 0)
                {
                    for (int i = 0; i < particleClusters.Count; i++)
                    {
                        if (particleClusters[i].explosions.Count > 0)
                        {
                            Color aColor = new Color(255, 255, 255, particleClusters[i].Transparency());
                            for (int j = 0; j < particleClusters[i].explosions.Count; j++)
                            {
                                spriteBatch.Draw(particleClusters[i].texture,
                                    new Rectangle((int)particleClusters[i].explosions[j].x, (int)particleClusters[i].explosions[j].y, (int)particleClusters[i].explosions[j].scale, (int)particleClusters[i].explosions[j].scale), null, aColor, (float)particleClusters[i].explosions[j].rotation, new Vector2(64, 64), SpriteEffects.None, 0);
                            }
                        }
                    }
                }

                if (playerLooking)
                {
                    if (currentLevel.GetTileVision(playerLookAtX, playerLookAtY) > 64
                        || currentLevel.GetItemVisibilityAtTile(playerLookAtX, playerLookAtY) > 0
                        || currentLevel.GetCreatureVisibilityAtTile(playerLookAtX, playerLookAtY) > 0)
                    {
                        spriteBatch.Draw(txRed, new Rectangle(TILE_SIZE * halfTilesInViewX + TILE_SIZE * (playerLookAtX - player1.x), TILE_SIZE * halfTilesInViewY + TILE_SIZE * (playerLookAtY - player1.y), TILE_SIZE, TILE_SIZE), new Color(255, 255, 255, 80));
                    }
                    else
                    {
                        spriteBatch.Draw(txRed, new Rectangle(TILE_SIZE * halfTilesInViewX + TILE_SIZE * (playerLookAtX - player1.x), TILE_SIZE * halfTilesInViewY + TILE_SIZE * (playerLookAtY - player1.y), TILE_SIZE, TILE_SIZE), Color.White);
                    }
                }

                //Draw a black box to cover anything that might slip off the bottom
                spriteBatch.Draw(txBlack, new Rectangle(0, TILE_SIZE * tilesInViewY, GAMEFIELDWIDTH, GAMEFIELDHEIGHT - (TILE_SIZE * tilesInViewY - TILE_SIZE)), Color.White);

                //Draw creature health bars
                if (creaturesInView.Count > 0)
                {
                    for (int i = 0; i < creaturesInView.Count; i++)
                    {
                        if (creaturesInView[i].seenByPlayer && creaturesInView[i].currentHealth < creaturesInView[i].description.MAX_HEALTH)
                        {
                            bool drawBar = false;
                            for (int j = 0; j < creaturesInView[i].description.width; j++)
                            {
                                for (int k = 0; k < creaturesInView[i].description.height; k++)
                                {
                                    if (currentLevel.CanSee(player1.x, player1.y, creaturesInView[i].x + j, creaturesInView[i].y + k))
                                    {
                                        if ((creaturesInView[i].x + j - player1.x) * (creaturesInView[i].x + j - player1.x)
                                            + (creaturesInView[i].y + k - player1.y) * (creaturesInView[i].y + k - player1.y)
                                            <= player1.perception * player1.perception)
                                        {
                                            drawBar = true;
                                        }
                                    }
                                    if (drawBar) break;
                                }
                                if (drawBar) break;
                            }

                            if (drawBar)
                            {
                                spriteBatch.Draw(txRed, new Rectangle(creaturesInView[i].x * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + 1,
                                                                    creaturesInView[i].y * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE - 5,
                                                                    TILE_SIZE * creaturesInView[i].description.width - 2, 4),
                                                                    new Color(255, 255, 255, 100));
                                spriteBatch.Draw(txGreen, new Rectangle(creaturesInView[i].x * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE + 1,
                                                                    creaturesInView[i].y * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE - 5,
                                                                    (int)((double)(TILE_SIZE * creaturesInView[i].description.width - 2) * (creaturesInView[i].currentHealth / creaturesInView[i].description.MAX_HEALTH))
                                                                    , 4),
                                                                    Color.White);
                            }
                        }
                    }
                }
                //Draw HUD
                //New spriteBatch pass, to draw smooth instead of pixellated
                spriteBatch.End();
                spriteBatch.Begin();
                spriteBatch.Draw(txDarkBlue, new Rectangle(0, 0, GAMEFIELDWIDTH, 80), new Color(255, 255, 255, 100));
                spriteBatch.DrawString(sfFranklinGothic, "MOVES: " + moveCount.ToString().PadLeft(5, '0'), new Vector2(GAMEFIELDWIDTH - 200, 2), Color.White);
                if (currentQuest != null && !inVillage) spriteBatch.DrawString(sfFranklinGothic, "FLOOR: " + ((currentLevel.levelNumber - currentQuest.level) + 1).ToString() + "/" + currentQuest.numFloors.ToString(), new Vector2(GAMEFIELDWIDTH - 200, 20), Color.White);
                spriteBatch.DrawString(sfFranklinGothic, "HEALTH", new Vector2(10, 4), Color.White);
                spriteBatch.DrawString(sfFranklinGothic, "HUNGER", new Vector2(10, 28), Color.White);
                spriteBatch.DrawString(sfFranklinGothic, "MAGIC", new Vector2(10, 54), Color.White);
                spriteBatch.DrawString(sfFranklinGothic, "LVL: " + player1.level + "\n\nGOLD: " + player1.gold.ToString(), new Vector2(350, 4), Color.White);

                int statusMeterLeft = 95;
                int StatusMetersLength = 200;
                int StatusMetersThickness = 20;
                spriteBatch.Draw(txWhite, new Rectangle(statusMeterLeft - 1, 7, StatusMetersLength + 2, 70), new Color(200, 200, 200, 255));

                int statusMeterProgress = (int)(((float)player1.currentHealth / (float)player1.MAX_HEALTH) * StatusMetersLength);
                DrawStatusBar(8, statusMeterLeft, StatusMetersLength, statusMeterProgress, Color.Green);

                statusMeterProgress = (int)(((float)(player1.hunger) / (float)(player1.full_stomach)) * StatusMetersLength);
                DrawStatusBar(32, statusMeterLeft, StatusMetersLength, statusMeterProgress, Color.Orange);

                statusMeterProgress = (int)(((float)(player1.currentMagik) / (float)(player1.MAX_MAGIK)) * StatusMetersLength);
                DrawStatusBar(56, statusMeterLeft, StatusMetersLength, statusMeterProgress, Color.Blue);

                int xpBarLeft = 360 + (int)sfFranklinGothic.MeasureString("LVL: " + player1.level).X;
                spriteBatch.Draw(txWhite, new Rectangle(xpBarLeft - 1, 7, StatusMetersLength + 2, StatusMetersThickness + 2), new Color(200, 200, 200, 255));
                statusMeterProgress = (int)(((float)(player1.experience - player1.totalExpToLastLevel) / (float)(player1.totalExpToNextLevel - player1.totalExpToLastLevel)) * StatusMetersLength);
                DrawStatusBar(8, xpBarLeft, StatusMetersLength, statusMeterProgress, Color.HotPink);

                spriteBatch.Draw(txDarkBlue, new Rectangle(0, GAMEFIELDHEIGHT - 100, GAMEFIELDWIDTH, 100), new Color(255, 255, 255, 100));
                if (messages != null)
                {
                    if (messages.Count > 0)
                    {
                        int messagesToDraw = Math.Min(NUM_MESSAGES_SHOWN, messages.Count);
                        Color colorToDraw = Color.White;
                        for (int i = 0; i < messagesToDraw; i++)
                        {
                            if (messages[messages.Count - 1 - i - messageDisplayOffset].age < 3)
                            {
                                if (messages[messages.Count - 1 - i - messageDisplayOffset].isBad)
                                {
                                    colorToDraw = Color.Red;
                                }
                                else
                                {
                                    colorToDraw = Color.YellowGreen;
                                }
                            }
                            else
                            {
                                byte shade = (byte)Math.Max(100, 255 - 4 * messages[messages.Count - 1 - i - messageDisplayOffset].age);
                                if (messages[messages.Count - 1 - i - messageDisplayOffset].isBad)
                                {
                                    colorToDraw = new Color(shade, 0, 0);
                                }
                                else
                                {
                                    colorToDraw = new Color(shade, shade, shade);
                                }
                            }
                            spriteBatch.DrawString(sfSylfaen14, messages[messages.Count - 1 - i - messageDisplayOffset].message, new Vector2(2, GAMEFIELDHEIGHT - 104 + (20 * i)), colorToDraw);
                        }
                    }

                    guiArrowTint += 4;
                    if (guiArrowTint < 100) guiArrowTint = 100;
                    if (messages.Count > NUM_MESSAGES_SHOWN)
                    {
                        if (messageDisplayOffset > 0)
                        {
                            spriteBatch.Draw(txGUIArrowUp, new Rectangle(GAMEFIELDWIDTH - 68, GAMEFIELDHEIGHT - 85, 16, 16), new Color(guiArrowTint, guiArrowTint, 0, guiArrowTint));
                        }
                        if (messageDisplayOffset < messages.Count - NUM_MESSAGES_SHOWN)
                        {
                            spriteBatch.Draw(txGUIArrowDown, new Rectangle(GAMEFIELDWIDTH - 68, GAMEFIELDHEIGHT - 30, 16, 16), new Color(guiArrowTint, guiArrowTint, 0, guiArrowTint));
                        }
                    }
                }
                if (!player1.alive)
                {
                    spriteBatch.DrawString(sfSylfaen14, "You Are Dead.\n\n      End Game", new Vector2(8, 100), Color.White);
                    spriteBatch.Draw(btnTxButtonY, new Rectangle(12, 150, 28, 28), Color.White);
                }

                if (isCastingSpell)
                {

                    if (spellTypeBeingCast == (int)SC.SpellCasting.DIRECTION)
                    {
                        spriteBatch.Draw(txBlue, new Rectangle(TILE_SIZE * halfTilesInViewX + TILE_SIZE * (spellTargetX - player1.x), TILE_SIZE * halfTilesInViewY + TILE_SIZE * (spellTargetY - player1.y), TILE_SIZE, TILE_SIZE), new Color(255, 255, 255, 80));
                        spriteBatch.DrawString(sfSylfaen14, "Spell Casting: Choose a Direction To Cast\n      Cast\n      Cancel", new Vector2(8, 100), Color.White);
                    }
                    if (spellTypeBeingCast == (int)SC.SpellCasting.TARGET)
                    {
                        spriteBatch.Draw(txBlue, new Rectangle(TILE_SIZE * halfTilesInViewX + TILE_SIZE * (spellTargetX - player1.x), TILE_SIZE * halfTilesInViewY + TILE_SIZE * (spellTargetY - player1.y), TILE_SIZE, TILE_SIZE), new Color(255, 255, 255, 80));
                        spriteBatch.DrawString(sfSylfaen14, "Spell Casting: Choose a Target\n      Cast\n      Cancel", new Vector2(8, 100), Color.White);
                    }
                    if (spellTypeBeingCast == (int)SC.SpellCasting.RADIUS)
                    {
                        for (int i = 0; i < tilesInViewX; i++)
                        {
                            for (int j = 0; j < tilesInViewY; j++)
                            {
                                if ((i - halfTilesInViewX) * (i - halfTilesInViewX)
                                    + (j - halfTilesInViewY) * (j - halfTilesInViewY) <= spellRadiusSquared)
                                {
                                    spriteBatch.Draw(txBlue, new Rectangle(TILE_SIZE * i, TILE_SIZE * j, TILE_SIZE, TILE_SIZE), new Color(255, 255, 255, 80));
                                }
                            }
                        }
                        spriteBatch.DrawString(sfSylfaen14, "Spell Casting: Radius Of Effect Is Highlighted\n      Cast\n      Cancel", new Vector2(8, 100), Color.White);

                    }
                    spriteBatch.Draw(btnTxButtonA, new Rectangle(8, 130, 28, 28), Color.White);
                    spriteBatch.Draw(btnTxButtonB, new Rectangle(8, 160, 28, 28), Color.White);
                }

                if (contextDialogOpen)
                {
                    Color dialogWhite = Color.White;
                    if (contextDialogSubMenuOpen)
                    {
                        dialogWhite = new Color(120, 120, 120, 120);
                    }
                    spriteBatch.Draw(txWhite, new Rectangle(TILE_SIZE * halfTilesInViewX + TILE_SIZE * 2, TILE_SIZE * halfTilesInViewY - TILE_SIZE * 2, 250, 300), dialogWhite);
                    spriteBatch.Draw(txBlue, new Rectangle(TILE_SIZE * halfTilesInViewX + TILE_SIZE * 2 + 2, TILE_SIZE * halfTilesInViewY - TILE_SIZE * 2 + 2, 246, 296), dialogWhite);
                    if (contextMenuStringList != null)
                    {
                        if (contextMenuStringList.Count > 0)
                        {
                            for (int i = 0; i < contextMenuStringList.Count; i++)
                            {
                                spriteBatch.DrawString(sfSylfaen14, contextMenuStringList[i], new Vector2(TILE_SIZE * halfTilesInViewX + TILE_SIZE * 2 + 4, TILE_SIZE * halfTilesInViewY - TILE_SIZE * 2 + 4 + i * 24), dialogWhite);
                            }
                        }
                    }
                    //spriteBatch.DrawString(sfSylfaen14, "(B)", new Vector2(TILE_SIZE * halfTilesInViewX + TILE_SIZE * 2 + 4, TILE_SIZE * halfTilesInViewY - TILE_SIZE * 2 + 254), Color.OrangeRed);
                    spriteBatch.Draw(btnTxButtonB, new Rectangle(TILE_SIZE * halfTilesInViewX + TILE_SIZE * 2 + 4, TILE_SIZE * halfTilesInViewY - TILE_SIZE * 2 + 254, 32, 32), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Cancel", new Vector2(TILE_SIZE * halfTilesInViewX + TILE_SIZE * 2 + 40, TILE_SIZE * halfTilesInViewY - TILE_SIZE * 2 + 254), dialogWhite);
                    if (!contextDialogSubMenuOpen)
                    {
                        spriteBatch.Draw(txWhite, new Rectangle(TILE_SIZE * halfTilesInViewX + TILE_SIZE * 2 + 4, TILE_SIZE * halfTilesInViewY - TILE_SIZE * 2 + 6 + 24 * currentContextMenuSelection, 242, 24), new Color(100, 100, 100, 80));
                    }
                }

                if (featPanelDisplayTimer < featPanelDisplayDuration && featToDisplay != null)
                {
                    byte panelAlpha = 255;
                    if ((featPanelDisplayDuration - featPanelDisplayTimer) < featPanelDisplayDuration / 3)
                    {
                        panelAlpha = (byte)(featPanelDisplayDuration - featPanelDisplayTimer);
                    }
                    Color tintColor = new Color(panelAlpha, panelAlpha, panelAlpha, panelAlpha);
                    spriteBatch.Draw(txOrange, new Rectangle(50, 20, GAMEFIELDWIDTH - 100, 80), tintColor);
                    spriteBatch.Draw(txBlack, new Rectangle(65, 35, GAMEFIELDWIDTH - 130, 50), tintColor);
                    if (featToDisplay.rank == 1)
                    {
                        spriteBatch.DrawString(sfFranklinGothic, "FEAT ACHIEVED", new Vector2(350, 36), tintColor);
                    }
                    else
                    {
                        spriteBatch.DrawString(sfFranklinGothic, "FEAT RANKED UP (RANK " + featToDisplay.rank.ToString() + ")", new Vector2(350, 36), tintColor);
                    }
                    spriteBatch.DrawString(sfSylfaen14, featToDisplay.title, new Vector2(350, 56), tintColor);
                }

                if (contextDialogSubMenuOpen)
                {
                    spriteBatch.Draw(txWhite, new Rectangle(98, TILE_SIZE * halfTilesInViewY - TILE_SIZE * 2 - 10, GAMEFIELDWIDTH - 196, 300), Color.White);
                    spriteBatch.Draw(txBlue, new Rectangle(100, TILE_SIZE * halfTilesInViewY - TILE_SIZE * 2 - 8, GAMEFIELDWIDTH - 200, 296), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, subMenuAction, new Vector2(TILE_SIZE * halfTilesInViewX + TILE_SIZE * 2 + 4 - 140, TILE_SIZE * halfTilesInViewY - TILE_SIZE * 2 + 2 - 10), Color.White);
                    //spriteBatch.DrawString(sfSylfaen14, "----------", new Vector2(TILE_SIZE * halfTilesInViewX + TILE_SIZE * 2 + 4 - 230, TILE_SIZE * halfTilesInViewY - TILE_SIZE * 2 + 2 + 45), Color.White);
                    if (contextSubMenuStringList != null)
                    {
                        if (contextSubMenuStringList.Count > 0)
                        {
                            for (int i = 0; i < contextSubMenuStringList.Count; i++)
                            {
                                spriteBatch.DrawString(sfSylfaen14, contextSubMenuStringList[i], new Vector2(104, TILE_SIZE * halfTilesInViewY - TILE_SIZE * 2 + 2 + i * 24 + 44), Color.White);
                                if (subMenuAction == "Pick Up...")
                                {
                                    String comparison = "";
                                    Color compColor = Color.White;
                                    double diff = 0;
                                    switch (currentLevel.GetItems(player1.x, player1.y)[i].itemType)
                                    {
                                        case (int)SC.ItemTypes.ARMOUR:
                                            diff = player1.CompareArmour(currentLevel.GetItems(player1.x, player1.y)[i]);
                                            if (diff == 0) comparison = "= Defence";
                                            if (diff < 0)
                                            {
                                                comparison = diff.ToString() + " Defence";
                                                compColor = Color.OrangeRed;
                                            }
                                            if (diff > 0)
                                            {
                                                comparison = "+" + diff.ToString() + " Defence";
                                                compColor = Color.LightGreen;
                                            }
                                            break;
                                        case (int)SC.ItemTypes.SHIELD:
                                            diff = player1.CompareShield(currentLevel.GetItems(player1.x, player1.y)[i]);
                                            if (diff == 0) comparison = "= Defence";
                                            if (diff < 0)
                                            {
                                                comparison = diff.ToString() + " Defence";
                                                compColor = Color.OrangeRed;
                                            }
                                            if (diff > 0)
                                            {
                                                comparison = "+" + diff.ToString() + " Defence";
                                                compColor = Color.LightGreen;
                                            }
                                            break;
                                        case (int)SC.ItemTypes.WEAPON:
                                            diff = player1.CompareSword(currentLevel.GetItems(player1.x, player1.y)[i]);
                                            if (diff == 0) comparison = "= Attack";
                                            if (diff < 0)
                                            {
                                                comparison = diff.ToString() + " Attack";
                                                compColor = Color.OrangeRed;
                                            }
                                            if (diff > 0)
                                            {
                                                comparison = "+" + diff.ToString() + " Attack";
                                                compColor = Color.LightGreen;
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    if (!comparison.Equals(""))
                                    {
                                        spriteBatch.DrawString(sfSylfaen14, comparison, new Vector2(700, TILE_SIZE * halfTilesInViewY - TILE_SIZE * 2 + 2 + i * 24 + 44), compColor);
                                    }
                                    DrawItemComparisonPanel(100, TILE_SIZE * halfTilesInViewY - TILE_SIZE * 2 + 290, GAMEFIELDWIDTH - 200, currentLevel.GetItems(player1.x, player1.y)[currentContextSubMenuSelection]);
                                }
                            }
                        }
                    }
                    spriteBatch.Draw(txWhite, new Rectangle(102, TILE_SIZE * halfTilesInViewY - TILE_SIZE * 2 + 44 + 24 * currentContextSubMenuSelection + 6, GAMEFIELDWIDTH - 204, 24), new Color(100, 100, 100, 80));

                }

                if (startMenuOpen)
                {
                    spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH - 406, 2, 404, GAMEFIELDHEIGHT / 2 - 4), Color.White);
                    spriteBatch.Draw(txBlue, new Rectangle(GAMEFIELDWIDTH - 404, 4, 400, GAMEFIELDHEIGHT / 2 - 8), Color.White);
                    spriteBatch.Draw(btnTxButtonB, new Rectangle(GAMEFIELDWIDTH - 404, 4 + GAMEFIELDHEIGHT / 2, 32, 32), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "/", new Vector2(GAMEFIELDWIDTH - 368, 4 + GAMEFIELDHEIGHT / 2), Color.White);
                    spriteBatch.Draw(btnTxStart, new Rectangle(GAMEFIELDWIDTH - 356, GAMEFIELDHEIGHT / 2, 36, 36), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Close", new Vector2(GAMEFIELDWIDTH - 318, 2 + GAMEFIELDHEIGHT / 2), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "MENU", new Vector2(GAMEFIELDWIDTH - 264, 6), Color.White);
                    if (startMenuStringList != null)
                    {
                        if (startMenuStringList.Count > 0)
                        {
                            for (int i = 0; i < startMenuStringList.Count; i++)
                            {
                                if ((cheating || isTrialMode) && startMenuStringList[i] == "Suspend")
                                {
                                    spriteBatch.DrawString(sfSylfaen14, startMenuStringList[i], new Vector2(GAMEFIELDWIDTH - 400, 66 + i * 24), Color.DarkGray);
                                }
                                else
                                {
                                    spriteBatch.DrawString(sfSylfaen14, startMenuStringList[i], new Vector2(GAMEFIELDWIDTH - 400, 66 + i * 24), Color.White);
                                }
                            }
                        }
                    }
                    spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH - 400, 66 + currentStartMenuSelection * 24, 350, 24), new Color(100, 100, 100, 80));
                    //Draw current quest
                    if (currentQuest != null)
                    {
                        spriteBatch.Draw(txBlack, new Rectangle(40, 400, GAMEFIELDWIDTH - 80, 100), new Color(150, 150, 150, 150));
                        spriteBatch.DrawString(sfSylfaen14, "Current Quest:", new Vector2(60, 405), Color.White);
                        Color questStatusColor = Color.White;
                        String questStatus = currentQuest.title;
                        if (currentQuest.completed)
                        {
                            questStatusColor = Color.Green;
                            questStatus += " (COMPLETED)";
                        }
                        if (currentQuest.failed)
                        {
                            questStatusColor = Color.Red;
                            questStatus += " (FAILED)";
                        }
                        spriteBatch.DrawString(sfFranklinGothic, questStatus, new Vector2(60, 430), questStatusColor);
                    }

                    if (bgmPlayOption != SC.BGMPlayOption.OFF)
                    {
                        spriteBatch.Draw(txBlack, new Rectangle(3, 84, 550, 30), new Color(150,150,150,150));
                        if (inVillage)
                        {
                            spriteBatch.DrawString(sfFranklinGothic, "Current BGM: " + villageMusic.artist + " - " + villageMusic.title, new Vector2(5, 85), Color.White);
                        }
                        else
                        {
                            spriteBatch.DrawString(sfFranklinGothic, "Current BGM: " + dungeonMusic.artist + " - " + dungeonMusic.title, new Vector2(5, 85), Color.White);
                        }
                    }
                }

                if (optionsMenuOpen)
                {
                    spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH - 406, 2, 404, 464), Color.White);
                    spriteBatch.Draw(txBlue, new Rectangle(GAMEFIELDWIDTH - 404, 4, 400, 460), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "OPTIONS MENU", new Vector2(GAMEFIELDWIDTH - 300, 6), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Tile Size:", new Vector2(GAMEFIELDWIDTH - 396, 66), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Large - Normal - Small", new Vector2(GAMEFIELDWIDTH - 396, 86), Color.Gray);
                    spriteBatch.DrawString(sfSylfaen14, "Display Size:", new Vector2(GAMEFIELDWIDTH - 396, 126), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Normal - Maxed", new Vector2(GAMEFIELDWIDTH - 396, 146), Color.Gray);
                    spriteBatch.DrawString(sfSylfaen14, "Movement:", new Vector2(GAMEFIELDWIDTH - 396, 186), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Left Stick - D-Pad", new Vector2(GAMEFIELDWIDTH - 396, 206), Color.Gray);
                    spriteBatch.DrawString(sfSylfaen14, "Background Music", new Vector2(GAMEFIELDWIDTH - 396, 246), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "On      Off", new Vector2(GAMEFIELDWIDTH - 396, 266), Color.Gray);
                    spriteBatch.DrawString(sfSylfaen14, "BGM Shuffle Mode:", new Vector2(GAMEFIELDWIDTH - 396, 306), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Quest    Floor", new Vector2(GAMEFIELDWIDTH - 396, 326), Color.Gray);
                    spriteBatch.DrawString(sfSylfaen14, "Show Tutorial Messages", new Vector2(GAMEFIELDWIDTH - 396, 366), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Yes - No", new Vector2(GAMEFIELDWIDTH - 396, 386), Color.Gray);
                    spriteBatch.DrawString(sfSylfaen14, "Reset Tutorial Messages", new Vector2(GAMEFIELDWIDTH - 396, 426), Color.Gray);

                    if (currentOptionMenuSelection == 0)
                    {
                        if (TILE_SIZE == 16)
                        {
                            spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH - 208, 90, 65, 24), new Color(100, 100, 100, 80));
                        }
                        if (TILE_SIZE == 32)
                        {
                            spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH - 312, 90, 82, 24), new Color(100, 100, 100, 80));
                        }
                        if (TILE_SIZE == 64)
                        {
                            spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH - 398, 90, 66, 24), new Color(100, 100, 100, 80));
                        }
                    }
                    if (currentOptionMenuSelection == 1)
                    {
                        spriteBatch.Draw(txRed, new Rectangle(0, 0, GAMEFIELDWIDTH, 16), new Color(255, 255, 255, guiArrowTint));
                        spriteBatch.Draw(txRed, new Rectangle(0, GAMEFIELDHEIGHT - 16, GAMEFIELDWIDTH, 16), new Color(255, 255, 255, guiArrowTint));
                        spriteBatch.Draw(txRed, new Rectangle(0, 0, 16, GAMEFIELDHEIGHT), new Color(255, 255, 255, guiArrowTint));
                        spriteBatch.Draw(txRed, new Rectangle(GAMEFIELDWIDTH - 16, 0, 16, GAMEFIELDHEIGHT), new Color(255, 255, 255, guiArrowTint));
                        if (displayFullScreen)
                        {
                            spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH - 292, 151, 73, 20), new Color(100, 100, 100, 80));
                        }
                        else
                        {
                            spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH - 396, 151, 81, 20), new Color(100, 100, 100, 80));
                        }
                    }
                    if (currentOptionMenuSelection == 2)
                    {
                        if (controller.MovingWithDPad())
                        {
                            spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH - 272, 211, 72, 20), new Color(100, 100, 100, 80));
                        }
                        else
                        {
                            spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH - 396, 211, 105, 20), new Color(100, 100, 100, 80));
                        }

                    }
                    if (currentOptionMenuSelection == 3)
                    {
                        if (bgmPlayOption == SC.BGMPlayOption.ON)
                            spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH - 398, 272, 36, 20), new Color(100, 100, 100, 80));
                        if (bgmPlayOption == SC.BGMPlayOption.OFF)
                            spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH - 324, 272, 40, 20), new Color(100, 100, 100, 80));
                    }
                    if (currentOptionMenuSelection == 4)
                    {
                        if (bgmShuffleOption == SC.BGMShuffleOption.ONCE_PER_QUEST)
                            spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH - 398, 332, 68, 20), new Color(100, 100, 100, 80));
                        if (bgmShuffleOption == SC.BGMShuffleOption.ONCE_PER_FLOOR)
                            spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH - 308, 332, 64, 20), new Color(100, 100, 100, 80));

                    }
                    if (currentOptionMenuSelection == 5)
                    {
                        if (neverSeeTutorialMessages)
                        {
                            spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH - 334, 391, 34, 22), new Color(100, 100, 100, 80));
                        }
                        else
                        {
                            spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH - 396, 391, 40, 22), new Color(100, 100, 100, 80));
                        }
                    }
                    if (currentOptionMenuSelection == 6)
                    {
                        spriteBatch.DrawString(sfSylfaen14, "Reset Tutorial Messages", new Vector2(GAMEFIELDWIDTH - 396, 426), Color.White);
                        spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH - 400, 430, 272, 22), new Color(100, 100, 100, 80));
                    }

                }

                if (questListOpen)
                {
                    int windowLeft = 8;
                    spriteBatch.Draw(txBlack, new Rectangle(0, 0, GAMEFIELDWIDTH, GAMEFIELDHEIGHT), new Color(200, 200, 200, 100));
                    spriteBatch.Draw(txWhite, new Rectangle(windowLeft - 2, 78, GAMEFIELDWIDTH - 96, GAMEFIELDHEIGHT - 156), Color.White);
                    spriteBatch.Draw(txBlue, new Rectangle(windowLeft, 80, GAMEFIELDWIDTH - 100, GAMEFIELDHEIGHT - 160), Color.White);

                    spriteBatch.Draw(txWhite, new Rectangle(windowLeft + 200, 28, 500, 50), Color.White);
                    spriteBatch.Draw(txBlue, new Rectangle(windowLeft + 202, 30, 496, 48), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Quest List", new Vector2(windowLeft + 385, 34), Color.White);
                    if (currentQuestList == null || currentQuestList.Count == 0)
                    {
                        spriteBatch.DrawString(sfSylfaen14, "OOPS NO QUESTS. THIS IS BUG :(", new Vector2(GAMEFIELDWIDTH / 2 - 200, GAMEFIELDHEIGHT / 2), Color.White);
                    }
                    else
                    {
                        for (int i = 0; i < currentQuestList.Count; i++)
                        {

                            Color questStatusColor = new Color(255, 255, 255, 255);
                            if (currentQuestList[i].level < player1.level - 5) questStatusColor = new Color(100, 255, 100, 255);
                            if (currentQuestList[i].level > player1.level + 5) questStatusColor = new Color(255, 100, 100, 255);
                            if (currentQuestList[i].level >= player1.level && currentQuestList[i].level <= player1.level + 5) questStatusColor = new Color(255, 255, 100, 255);

                            spriteBatch.DrawString(sfFranklinGothic, "EXP: " + currentQuestList[i].rewardExp, new Vector2(windowLeft + 4, 110 + i * 60), questStatusColor);
                            spriteBatch.Draw(txItemCoins, new Rectangle(windowLeft + 140, 114 + i * 60, 20, 20), Color.White);
                            spriteBatch.DrawString(sfFranklinGothic, currentQuestList[i].rewardGold.ToString(), new Vector2(windowLeft + 165, 110 + i * 60), questStatusColor);
                            if (currentQuestList[i].rewardItem != null)
                            {
                                spriteBatch.Draw(txItemQuest, new Rectangle(windowLeft + 264, 114 + i * 60, 20, 20), Color.White);
                                spriteBatch.DrawString(sfFranklinGothic, currentQuestList[i].rewardItem.itemName, new Vector2(windowLeft + 285, 110 + i * 60), questStatusColor);
                            }
                            if (currentQuestList[i].isBossQuest)
                            {
                                spriteBatch.Draw(txIconBoss, new Rectangle(windowLeft + 669, 104 + i * 60, 32, 32), Color.White);
                            }
                            else
                            {
                                spriteBatch.Draw(txTileStairsDown, new Rectangle(windowLeft + 675, 110 + i * 60, 20, 20), Color.White);
                                spriteBatch.DrawString(sfFranklinGothic, currentQuestList[i].numFloors.ToString(), new Vector2(windowLeft + 700, 110 + i * 60), questStatusColor);
                            }
                            //Draw the quest title itself last, so it overlaps anything else
                            spriteBatch.DrawString(sfSylfaen14, currentQuestList[i].title, new Vector2(windowLeft + 4, 84 + i * 60), questStatusColor);
                        }
                        spriteBatch.Draw(txWhite, new Rectangle(windowLeft + 2, 82 + currentQuestListSelection * 60, GAMEFIELDWIDTH - 104, 54), new Color(100, 100, 100, 80));
                    }
                    spriteBatch.Draw(txLargeCreatureQuestMaster, new Rectangle(GAMEFIELDWIDTH - 360, -50, 510, 923), Color.White);
                    DrawBossProgress(windowLeft - 2, GAMEFIELDHEIGHT - 80, GAMEFIELDWIDTH - 296);
                }

                if (questTurnInOpen)
                {
                    spriteBatch.Draw(txWhite, new Rectangle(48, 108, GAMEFIELDWIDTH - 96, GAMEFIELDHEIGHT - 216), Color.White);
                    spriteBatch.Draw(txBlue, new Rectangle(50, 110, GAMEFIELDWIDTH - 100, GAMEFIELDHEIGHT - 220), Color.White);
                    DrawBossProgress(48, GAMEFIELDHEIGHT - 108, GAMEFIELDWIDTH - 296);
                    spriteBatch.DrawString(sfSylfaen14, "Current Quest Complete!", new Vector2(GAMEFIELDWIDTH / 2 - 150, 114), Color.White);
                    spriteBatch.DrawString(sfFranklinGothic, currentQuest.title, new Vector2(54, 170), Color.LightGray);
                    if (currentQuest.rewardExp > 0)
                    {
                        spriteBatch.DrawString(sfFranklinGothic, "EXP: " + currentQuest.rewardExp, new Vector2(200, 200), Color.Yellow);
                    }
                    if (currentQuest.rewardGold > 0)
                    {
                        spriteBatch.Draw(txItemCoins, new Rectangle(200, 230, 24, 24), Color.White);
                        spriteBatch.DrawString(sfFranklinGothic, currentQuest.rewardGold.ToString(), new Vector2(230, 230), Color.Yellow);
                    }
                    if (currentQuest.rewardItem != null)
                    {
                        spriteBatch.Draw(txItemQuest, new Rectangle(200, 260, 24, 24), Color.White);
                        spriteBatch.DrawString(sfFranklinGothic, currentQuest.rewardItem.itemName, new Vector2(230, 260), Color.Yellow);
                    }
                    spriteBatch.Draw(btnTxButtonA, new Rectangle(GAMEFIELDWIDTH / 2 - 64, GAMEFIELDHEIGHT - 150, 30, 30), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Turn In Quest", new Vector2(GAMEFIELDWIDTH / 2 - 32, GAMEFIELDHEIGHT - 150), Color.White);
                }

                if (questForfeitOpen)
                {
                    spriteBatch.Draw(txWhite, new Rectangle(48, 108, GAMEFIELDWIDTH - 96, GAMEFIELDHEIGHT - 216), Color.White);
                    spriteBatch.Draw(txBlue, new Rectangle(50, 110, GAMEFIELDWIDTH - 100, GAMEFIELDHEIGHT - 220), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Current Quest:", new Vector2(GAMEFIELDWIDTH / 2 - 70, 114), Color.White);
                    spriteBatch.DrawString(sfFranklinGothic, currentQuest.title, new Vector2(54, 160), Color.LightGray);
                    spriteBatch.DrawString(sfFranklinGothic, "Quest Forfeit Penalty: " + currentQuest.forfeitGold + " Gold", new Vector2(230, 220), Color.Red);
                    spriteBatch.Draw(btnTxButtonY, new Rectangle(GAMEFIELDWIDTH / 2 - 170, GAMEFIELDHEIGHT - 180, 30, 30), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Forfeit Quest", new Vector2(GAMEFIELDWIDTH / 2 - 140, GAMEFIELDHEIGHT - 180), Color.White);
                    spriteBatch.Draw(btnTxButtonB, new Rectangle(GAMEFIELDWIDTH / 2 + 20, GAMEFIELDHEIGHT - 180, 30, 30), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Close", new Vector2(GAMEFIELDWIDTH / 2 + 50, GAMEFIELDHEIGHT - 180), Color.White);
                    DrawBossProgress(48, GAMEFIELDHEIGHT - 110, GAMEFIELDWIDTH - 296);
                }

                if (questFailedOpen)
                {
                    spriteBatch.Draw(txWhite, new Rectangle(48, 108, GAMEFIELDWIDTH - 96, GAMEFIELDHEIGHT - 216), Color.White);
                    spriteBatch.Draw(txBlue, new Rectangle(50, 110, GAMEFIELDWIDTH - 100, GAMEFIELDHEIGHT - 220), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "YOU HAVE FAILED YOUR QUEST:", new Vector2(GAMEFIELDWIDTH / 2 - 200, 114), Color.Red);
                    spriteBatch.DrawString(sfFranklinGothic, currentQuest.title, new Vector2(54, 160), Color.LightGray);
                    spriteBatch.DrawString(sfFranklinGothic, "Quest Failure Penalty: " + currentQuest.forfeitGold + " Gold", new Vector2(230, 220), Color.Red);
                    spriteBatch.Draw(btnTxButtonY, new Rectangle(GAMEFIELDWIDTH / 2 - 170, GAMEFIELDHEIGHT - 180, 30, 30), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Forfeit Quest", new Vector2(GAMEFIELDWIDTH / 2 - 140, GAMEFIELDHEIGHT - 180), Color.White);
                    spriteBatch.Draw(btnTxButtonB, new Rectangle(GAMEFIELDWIDTH / 2 + 20, GAMEFIELDHEIGHT - 180, 30, 30), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Close", new Vector2(GAMEFIELDWIDTH / 2 + 50, GAMEFIELDHEIGHT - 180), Color.White);
                    DrawBossProgress(48, GAMEFIELDHEIGHT - 108, GAMEFIELDWIDTH - 296);
                }

                if (inventoryOpen)
                {
                    spriteBatch.Draw(txBlack, new Rectangle(0, 0, GAMEFIELDWIDTH, GAMEFIELDWIDTH), new Color(200, 200, 200, 100));
                    spriteBatch.Draw(txWhite, new Rectangle(48, 78, GAMEFIELDWIDTH - 96, GAMEFIELDHEIGHT - 156), Color.White);
                    spriteBatch.Draw(txBlue, new Rectangle(50, 80, GAMEFIELDWIDTH - 100, GAMEFIELDHEIGHT - 160), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Inventory - " + inventoryTabs[currentInventoryTab].title, new Vector2(GAMEFIELDWIDTH / 2 - 200, 86), Color.White);
                    for (int i = 0; i < inventoryTabs.Count; i++)
                    {
                        spriteBatch.Draw(txWhite, new Rectangle(48 + 120 * i, 38, 120, 40), Color.Gray);
                        spriteBatch.Draw(txDarkBlue, new Rectangle(50 + 120 * i, 40, 116, 36), Color.White);
                        spriteBatch.DrawString(sfSylfaen14, inventoryTabs[i].title, new Vector2(52 + 120 * i, 42), Color.Gray);
                    }
                    spriteBatch.Draw(txWhite, new Rectangle(48 + 120 * currentInventoryTab, 38, 120, 40), Color.White);
                    spriteBatch.Draw(txBlue, new Rectangle(50 + 120 * currentInventoryTab, 40, 116, 36), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, inventoryTabs[currentInventoryTab].title, new Vector2(52 + 120 * currentInventoryTab, 42), Color.White);
                    List<Item> items = inventoryTabs[currentInventoryTab].items;
                    if (items.Count == 0)
                    {
                        spriteBatch.DrawString(sfSylfaen14, "No Items Of This Type!", new Vector2(GAMEFIELDWIDTH / 2 - 200, GAMEFIELDHEIGHT / 2), Color.White);
                    }
                    else
                    {
                        int firstItemToList = inventoryOffset;
                        int lastItemToList = Math.Min(inventoryOffset + (inventoryItemsPerPage - 1), items.Count - 1);
                        spriteBatch.DrawString(sfSylfaen14, "Wgt", new Vector2(666, 116), Color.White);
                        if (items.Count > inventoryItemsPerPage)
                        {
                            spriteBatch.Draw(txBlack, new Rectangle(933, 148, 32, 312), Color.White);
                            int scrollNotchYOffset = (int)(312f * ((float)inventoryOffset / (float)items.Count));
                            int scrollNotchHeight = (int)(312f * ((float)inventoryItemsPerPage / (float)items.Count));
                            spriteBatch.Draw(txGUIScrollTab, new Rectangle(933, 148 + scrollNotchYOffset, 32, scrollNotchHeight), Color.White);
                        }
                        for (int i = firstItemToList; i <= lastItemToList; i++)
                        {
                            spriteBatch.DrawString(sfSylfaen14, items[i].itemName, new Vector2(104, 146 + 40 * (i - firstItemToList)), Color.White);
                            spriteBatch.DrawString(sfSylfaen14, items[i].weight.ToString(), new Vector2(670, 146 + 40 * (i - firstItemToList)), Color.White);
                            String comparison = "";
                            Color compColor = Color.White;
                            double diff = 0;
                            switch (items[i].itemType)
                            {
                                case (int)SC.ItemTypes.ARMOUR:
                                    diff = player1.CompareArmour(items[i]);
                                    if (diff == 0) comparison = "= Defence";
                                    if (diff < 0)
                                    {
                                        comparison = diff.ToString() + " Defence";
                                        compColor = Color.OrangeRed;
                                    }
                                    if (diff > 0)
                                    {
                                        comparison = "+" + diff.ToString() + " Defence";
                                        compColor = Color.LightGreen;
                                    }
                                    break;
                                case (int)SC.ItemTypes.SHIELD:
                                    diff = player1.CompareShield(items[i]);
                                    if (diff == 0) comparison = "= Defence";
                                    if (diff < 0)
                                    {
                                        comparison = diff.ToString() + " Defence";
                                        compColor = Color.OrangeRed;
                                    }
                                    if (diff > 0)
                                    {
                                        comparison = "+" + diff.ToString() + " Defence";
                                        compColor = Color.LightGreen;
                                    }
                                    break;
                                case (int)SC.ItemTypes.WEAPON:
                                    diff = player1.CompareSword(items[i]);
                                    if (diff == 0) comparison = "= Attack";
                                    if (diff < 0)
                                    {
                                        comparison = diff.ToString() + " Attack";
                                        compColor = Color.OrangeRed;
                                    }
                                    if (diff > 0)
                                    {
                                        comparison = "+" + diff.ToString() + " Attack";
                                        compColor = Color.LightGreen;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            if (!comparison.Equals(""))
                            {
                                spriteBatch.DrawString(sfSylfaen14, comparison, new Vector2(750, 146 + 40 * (i - firstItemToList)), compColor);
                            }
                            if (items[i].forSale)
                            {
                                spriteBatch.Draw(txItemCoins, new Rectangle(74, 146 + 40 * (i - firstItemToList), 30, 30), Color.White);
                            }
                        }
                        spriteBatch.Draw(txWhite, new Rectangle(104, 146 + 40 * (currentInventorySelection - inventoryOffset), 556, 40), new Color(100, 100, 100, 80));
                        spriteBatch.Draw(txWhite, new Rectangle(48, 470, GAMEFIELDWIDTH - 96, 2), Color.White);
                        spriteBatch.DrawString(sfSylfaen14, "Carrying: " + ((int)player1.currentCarriedWeight).ToString()
                                                            + "/" + ((int)player1.carryingCapacity).ToString() + " Kgs",
                                                            new Vector2(84, 478), Color.White);
                        if (items[currentInventorySelection].itemType != (int)SC.ItemTypes.QUEST)
                        {
                            spriteBatch.Draw(btnTxButtonY, new Rectangle(352, 480, 24, 24), Color.White);
                            if (items[currentInventorySelection].forSale)
                            {
                                spriteBatch.DrawString(sfSylfaen14, "Don't Sell", new Vector2(380, 478), Color.White);
                            }
                            else
                            {
                                spriteBatch.DrawString(sfSylfaen14, "Mark as \"For Sale\"", new Vector2(380, 478), Color.White);
                            }
                            spriteBatch.Draw(btnTxButtonX, new Rectangle(592, 480, 24, 24), Color.White);
                            spriteBatch.DrawString(sfSylfaen14, "Drop", new Vector2(620, 478), Color.White);
                            spriteBatch.Draw(btnTxButtonA, new Rectangle(700, 480, 24, 24), Color.White);
                            String usageString = "Equip";
                            if (items[currentInventorySelection].itemType == (int)SC.ItemTypes.SCROLL) usageString = "Use";
                            if (items[currentInventorySelection].itemType == (int)SC.ItemTypes.FOOD) usageString = "Eat";
                            if (items[currentInventorySelection].itemType == (int)SC.ItemTypes.POTION) usageString = "Drink";
                            if (usageString.Equals("Equip") && !player1.CanEquip(items[currentInventorySelection])) usageString = "---";
                            spriteBatch.DrawString(sfSylfaen14, usageString, new Vector2(728, 478), Color.White);
                        }
                        DrawItemComparisonPanel(100, GAMEFIELDHEIGHT - 78, GAMEFIELDWIDTH - 200, items[currentInventorySelection]);
                    }
                }

                if (characterSheetOpen)
                {
                    spriteBatch.Draw(txWhite, new Rectangle(198, 18, GAMEFIELDWIDTH - 396, GAMEFIELDHEIGHT - 36), Color.White);
                    spriteBatch.Draw(txBlue, new Rectangle(200, 20, GAMEFIELDWIDTH - 400, GAMEFIELDHEIGHT - 40), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Character", new Vector2(GAMEFIELDWIDTH / 2 - 80, 26), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Character Name:   " + player1.name, new Vector2(208, 70), Color.White);
                    String raceString = "";
                    switch (player1.race)
                    {
                        case (int)SC.Races.ELF:
                            raceString = "Elf";
                            break;
                        case (int)SC.Races.HUMAN:
                            raceString = "Human";
                            break;
                        case (int)SC.Races.ORC:
                            raceString = "Orc";
                            break;
                    }
                    switch (player1.profession)
                    {
                        case (int)SC.Professions.EXPLORER:
                            raceString += " Explorer";
                            break;
                        case (int)SC.Professions.FIGHTER:
                            raceString += " Fighter";
                            break;
                        case (int)SC.Professions.MAGE:
                            raceString += " Mage";
                            break;
                    }
                    spriteBatch.DrawString(sfSylfaen14, "Level " + player1.level.ToString() + " " + raceString, new Vector2(412, 92), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Health:   " + player1.currentHealth + "/" + player1.MAX_HEALTH + "     " + magicName + ":   " + player1.currentMagik + "/" + player1.MAX_MAGIK, new Vector2(208, 130), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Experience:   " + player1.experience + "/" + (player1.experience + player1.next).ToString(), new Vector2(208, 165), Color.White);
                    String tempString = "Attack:   " + player1.AttackPower().ToString();
                    if (player1.equippedSword.elementalOffenceFire > 0) tempString = tempString + " +" + player1.equippedSword.elementalOffenceFire.ToString() + " Flame Damage";
                    if (player1.equippedSword.elementalOffenceIce > 0) tempString = tempString + " +" + player1.equippedSword.elementalOffenceIce.ToString() + " Ice Damage";
                    spriteBatch.DrawString(sfSylfaen14, tempString, new Vector2(208, 205), Color.White);
                    tempString = "Defence:   " + player1.DefencePower().ToString();
                    if (player1.FireDefence() < 1) tempString = tempString + "   " + (100 - player1.FireDefence() * 100).ToString() + "% Fire Resist";
                    if (player1.IceDefence() < 1) tempString = tempString + "   " + (100 - player1.IceDefence() * 100).ToString() + "% Ice Resist";
                    spriteBatch.DrawString(sfSylfaen14, tempString, new Vector2(208, 240), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Equipped Weapon: \n   " + player1.equippedSword.itemName, new Vector2(208, 300), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Armour: \n   " + player1.equippedArmour.itemName, new Vector2(208, 360), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Shield: \n   " + player1.equippedShield.itemName, new Vector2(208, 420), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Cloak: \n   " + player1.equippedCloak.itemName, new Vector2(208, 480), Color.White);
                    spriteBatch.Draw(btnTxButtonB, new Rectangle(GAMEFIELDWIDTH / 2 + 200, GAMEFIELDHEIGHT - 50, 28, 28), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, " Close", new Vector2(GAMEFIELDWIDTH / 2 + 230, GAMEFIELDHEIGHT - 50), Color.White);
                }

                if (spellListOpen)
                {
                    spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH / 2 - 200, 40, 400, GAMEFIELDHEIGHT - 240), Color.White);
                    spriteBatch.Draw(txBlue, new Rectangle(GAMEFIELDWIDTH / 2 - 198, 42, 396, GAMEFIELDHEIGHT - 244), Color.White);
                    List<Spell> spells = player1.spells.GetKnownSpells();
                    if (spells.Count > 0)
                    {
                        spriteBatch.DrawString(sfSylfaen14, "Spell\n------", new Vector2(GAMEFIELDWIDTH / 2 - 194, 38), Color.White);
                        spriteBatch.DrawString(sfSylfaen14, "Lvl\n----", new Vector2(GAMEFIELDWIDTH / 2 + 40, 38), Color.White);
                        spriteBatch.DrawString(sfSylfaen14, "Cost\n-----", new Vector2(GAMEFIELDWIDTH / 2 + 110, 38), Color.White);
                        for (int i = 0; i < spells.Count; i++)
                        {
                            spriteBatch.DrawString(sfSylfaen14, spells[i].spellName, new Vector2(GAMEFIELDWIDTH / 2 - 194, 90 + i * 20), Color.White);
                            spriteBatch.DrawString(sfSylfaen14, spells[i].level.ToString(), new Vector2(GAMEFIELDWIDTH / 2 + 40, 90 + i * 20), Color.White);
                            spriteBatch.DrawString(sfSylfaen14, spells[i].castingCost.ToString(), new Vector2(GAMEFIELDWIDTH / 2 + 110, 90 + i * 20), Color.White);
                        }
                        spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH / 2 - 196, 94 + currentSpellListSelection * 20, 200, 20), new Color(100, 100, 100, 80));
                    }
                }

                if (runStatsOpen)
                {
                    spriteBatch.Draw(txWhite, new Rectangle(GAMEFIELDWIDTH / 2 - 200, 40, 400, GAMEFIELDHEIGHT - 40), Color.White);
                    spriteBatch.Draw(txBlue, new Rectangle(GAMEFIELDWIDTH / 2 - 198, 42, 396, GAMEFIELDHEIGHT - 44), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Stats For This Game", new Vector2(GAMEFIELDWIDTH / 2 - 150, 46), Color.White);
                    if (runStats.statList.Count > 0)
                    {
                        for (int i = 0; i < runStats.statList.Count; i++)
                        {
                            spriteBatch.DrawString(sfFranklinGothic, runStats.statList[i].name, new Vector2(GAMEFIELDWIDTH / 2 - 194, 86 + i * 18), Color.White);
                            spriteBatch.DrawString(sfFranklinGothic, runStats.statList[i].value.ToString(), new Vector2(GAMEFIELDWIDTH / 2 + 194 - sfFranklinGothic.MeasureString(runStats.statList[i].value.ToString()).X, 86 + i * 18), Color.White);
                        }
                    }
                    else
                    {
                        spriteBatch.DrawString(sfFranklinGothic, "No Stats Yet!", new Vector2(GAMEFIELDWIDTH / 2 - 194, 86), Color.White);
                    }
                    spriteBatch.Draw(btnTxButtonB, new Rectangle(GAMEFIELDWIDTH / 2 + 50, GAMEFIELDHEIGHT - 50, 28, 28), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, " Close", new Vector2(GAMEFIELDWIDTH / 2 + 80, GAMEFIELDHEIGHT - 50), Color.White);
                }

                if (shopOpen)
                {
                    List<Item> theItems;
                    if (inVillage)
                    {
                        theItems = villageMerchantItems;
                    }
                    else
                    {
                        theItems = dungeonMerchantItems;
                    }
                    int left = 100;
                    int top = 80;
                    int width = GAMEFIELDWIDTH - 200;
                    int height = 56 + 30 * itemsPerMerchant;
                    spriteBatch.Draw(txBlack, new Rectangle(0, 0, GAMEFIELDWIDTH, GAMEFIELDHEIGHT), new Color(150, 150, 150, 150));
                    spriteBatch.Draw(txLargeCreatureMerchant, new Rectangle(-400, -200, 913, 1378), Color.White);
                    spriteBatch.Draw(txWhite, new Rectangle(left - 2, top - 2, width + 4, 2), Color.White);
                    spriteBatch.Draw(txWhite, new Rectangle(left - 2, top - 2, 2, height + 4), Color.White);
                    spriteBatch.Draw(txWhite, new Rectangle(left + width, top - 2, 2, height + 4), Color.White);
                    spriteBatch.Draw(txWhite, new Rectangle(left - 2, top + height, width + 4, 2), Color.White);
                    spriteBatch.Draw(txBlue, new Rectangle(left, top, width, height), new Color(150, 150, 150, 150));
                    spriteBatch.DrawString(sfSylfaen14, "Shop", new Vector2(GAMEFIELDWIDTH / 2 - 80, 86), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "-----", new Vector2(GAMEFIELDWIDTH / 2 - 80, 106), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Cost", new Vector2(780, 86), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "-----", new Vector2(780, 106), Color.White);
                    if (theItems == null || theItems.Count == 0)
                    {
                        spriteBatch.DrawString(sfSylfaen14, "This Merchant Has Nothing\nTo Trade", new Vector2(GAMEFIELDWIDTH / 2 - 150, 200), Color.White);
                    }
                    else
                    {
                        for (int i = 0; i < theItems.Count; i++)
                        {
                            Color colorToDraw = Color.White;
                            if (!CanBuy(theItems[i])) colorToDraw = Color.DarkGray;
                            spriteBatch.DrawString(sfSylfaen14, theItems[i].itemName, new Vector2(102, 126 + 30 * i), colorToDraw);
                            spriteBatch.DrawString(sfSylfaen14, MerchantPrice(theItems[i]).ToString(), new Vector2(780, 126 + 30 * i), colorToDraw);
                            String comparison = "";
                            Color compColor = Color.White;
                            double diff = 0;
                            switch (theItems[i].itemType)
                            {
                                case (int)SC.ItemTypes.ARMOUR:
                                    diff = player1.CompareArmour(theItems[i]);
                                    if (diff == 0) comparison = "= Defence";
                                    if (diff < 0)
                                    {
                                        comparison = diff.ToString() + " Defence";
                                        compColor = Color.OrangeRed;
                                    }
                                    if (diff > 0)
                                    {
                                        comparison = "+" + diff.ToString() + " Defence";
                                        compColor = Color.LightGreen;
                                    }
                                    break;
                                case (int)SC.ItemTypes.SHIELD:
                                    diff = player1.CompareShield(theItems[i]);
                                    if (diff == 0) comparison = "= Defence";
                                    if (diff < 0)
                                    {
                                        comparison = diff.ToString() + " Defence";
                                        compColor = Color.OrangeRed;
                                    }
                                    if (diff > 0)
                                    {
                                        comparison = "+" + diff.ToString() + " Defence";
                                        compColor = Color.LightGreen;
                                    }
                                    break;
                                case (int)SC.ItemTypes.WEAPON:
                                    diff = player1.CompareSword(theItems[i]);
                                    if (diff == 0) comparison = "= Attack";
                                    if (diff < 0)
                                    {
                                        comparison = diff.ToString() + " Attack";
                                        compColor = Color.OrangeRed;
                                    }
                                    if (diff > 0)
                                    {
                                        comparison = "+" + diff.ToString() + " Attack";
                                        compColor = Color.LightGreen;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            if (!comparison.Equals(""))
                            {
                                spriteBatch.DrawString(sfSylfaen14, comparison, new Vector2(520, 126 + 30 * i), compColor);
                            }
                        }
                        spriteBatch.Draw(txWhite, new Rectangle(102, 130 + 30 * currentShopSelection, GAMEFIELDWIDTH - 204, 20), new Color(100, 100, 100, 80));
                        DrawItemComparisonPanel(100, 136 + 30 * itemsPerMerchant, GAMEFIELDWIDTH - 200, theItems[currentShopSelection]);
                        if (lastShopMessage != "") spriteBatch.DrawString(sfSylfaen14, lastShopMessage, new Vector2(left, 190 + 30 * itemsPerMerchant), Color.White);
                    }
                    if (shopConfirmOpen)
                    {
                        spriteBatch.Draw(txBlack, new Rectangle(98, 78, GAMEFIELDWIDTH - 196, 60 + 30 * itemsPerMerchant), new Color(128, 128, 128, 128));
                        spriteBatch.Draw(txWhite, new Rectangle(198, GAMEFIELDHEIGHT / 2 - 162, GAMEFIELDWIDTH - 396, 204), Color.White);
                        spriteBatch.Draw(txBlue, new Rectangle(200, GAMEFIELDHEIGHT / 2 - 160, GAMEFIELDWIDTH - 400, 200), Color.White);
                        spriteBatch.DrawString(sfSylfaen14, "Buy \n" + theItems[currentShopSelection].itemName + "\nFor " + MerchantPrice(theItems[currentShopSelection]).ToString() + " Gold?", new Vector2(GAMEFIELDWIDTH / 2 - 148, GAMEFIELDHEIGHT / 2 - 158), Color.White);
                        spriteBatch.DrawString(sfSylfaen14, "Yes           No", new Vector2(GAMEFIELDWIDTH / 2 - 116, GAMEFIELDHEIGHT / 2), Color.White);
                        spriteBatch.Draw(btnTxButtonA, new Rectangle(GAMEFIELDWIDTH / 2 - 148, GAMEFIELDHEIGHT / 2, 30, 30), Color.White);
                        spriteBatch.Draw(btnTxButtonB, new Rectangle(GAMEFIELDWIDTH / 2 - 38, GAMEFIELDHEIGHT / 2, 30, 30), Color.White);
                    }
                }

                if (mapOpen)
                {
                    int mapDisplayWidth = GAMEFIELDWIDTH - 200;
                    int mapDisplayHeight = GAMEFIELDHEIGHT - 200;
                    int mapWidth = currentLevel.KnownMapWidth();
                    int mapHeight = currentLevel.KnownMapHeight();
                    int tileScale;
                    if ((mapDisplayWidth / (mapWidth + 2)) < (mapDisplayHeight / (mapHeight + 2)))
                    {
                        tileScale = mapDisplayWidth / (mapWidth + 2);
                    }
                    else
                    {
                        tileScale = mapDisplayHeight / (mapHeight + 2);
                    }

                    int horizontalOffset = (mapDisplayWidth - mapWidth * tileScale) / 2;

                    spriteBatch.Draw(txMapPaper, new Rectangle(98, 98, GAMEFIELDWIDTH - 196, GAMEFIELDHEIGHT - 196), Color.White);

                    for (int i = 0; i < mapWidth; i++)
                    {
                        for (int j = 0; j < mapHeight; j++)
                        {
                            Texture2D tileToDraw = txTileWall;
                            if (mapTiles[i, j] == TILE_EARTH)
                            {
                                tileToDraw = txMapGround;
                            }
                            if (mapTiles[i, j] == TILE_WALL)
                            {
                                tileToDraw = txMapWall;
                            }
                            if (mapTiles[i, j] == TILE_VOID)
                            {
                                tileToDraw = null;
                            }
                            if (mapTiles[i, j] == TILE_DOOR_CLOSED)
                            {
                                tileToDraw = txMapDoorClosed;
                            }
                            if (mapTiles[i, j] == TILE_DOOR_OPEN)
                            {
                                tileToDraw = txMapDoorOpen;
                            }
                            if (mapTiles[i, j] == TILE_STAIRS_DOWN)
                            {
                                tileToDraw = txMapStairs;
                            }
                            if (mapTiles[i, j] == TILE_STAIRS_UP)
                            {
                                tileToDraw = txMapStairs;
                            }

                            if (tileToDraw != null)
                            {
                                spriteBatch.Draw(tileToDraw, new Rectangle(horizontalOffset + 100 + tileScale + tileScale * i, 100 + tileScale + tileScale * j, tileScale, tileScale), Color.White);
                            }
                        }
                    }
                    spriteBatch.Draw(txMapPlayer, new Rectangle(horizontalOffset + 100 + tileScale + tileScale * (player1.x - currentLevel.LeftMostKnown()), 100 + tileScale + tileScale * (player1.y - currentLevel.TopMostKnown()), tileScale, tileScale), Color.White);

                    if (!inVillage)
                    {
                        if (currentLevel.MerchantExists())
                        {
                            if (currentLevel.MerchantKnown())
                            {
                                spriteBatch.Draw(txMapMerchant, new Rectangle(horizontalOffset + 100 + tileScale + tileScale * (currentLevel.MerchantX() - currentLevel.LeftMostKnown()), 100 + tileScale + tileScale * (currentLevel.MerchantY() - currentLevel.TopMostKnown()), tileScale, tileScale), Color.White);
                            }
                            else
                            {
                                spriteBatch.Draw(txMapMerchant, new Rectangle(110, 110, 32, 32), Color.White);
                            }
                        }
                        if (currentLevel.TeleporterKnown())
                        {
                            Point teleporterLocation = currentLevel.KnownTeleporterLocation();
                            spriteBatch.Draw(txMapTeleporter, new Rectangle(horizontalOffset + 100 + tileScale + tileScale * (teleporterLocation.X - currentLevel.LeftMostKnown()), 100 + tileScale + tileScale * (teleporterLocation.Y - currentLevel.TopMostKnown()), tileScale, tileScale), Color.White);
                        }
                    }

                    spriteBatch.Draw(txBlack, new Rectangle(GAMEFIELDWIDTH - 261, GAMEFIELDHEIGHT - 155, 143, 40), new Color(0, 0, 0, 100));
                    spriteBatch.DrawString(sfSylfaen14, "Close Map", new Vector2(GAMEFIELDWIDTH - 230, GAMEFIELDHEIGHT - 146), Color.White);
                    spriteBatch.Draw(btnTxButtonB, new Rectangle(GAMEFIELDWIDTH - 260, GAMEFIELDHEIGHT - 146, 28, 28), Color.White);

                }

                if (gameQuitConfirmOpen)
                {
                    spriteBatch.Draw(txWhite, new Rectangle(198, 78, GAMEFIELDWIDTH - 396, 160), Color.White);
                    spriteBatch.Draw(txBlue, new Rectangle(200, 80, GAMEFIELDWIDTH - 400, 156), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "End Game?", new Vector2(204, 84), Color.Yellow);
                    spriteBatch.DrawString(sfFranklinGothic, "The game will end as if you had died at this point.", new Vector2(204, 150), Color.White);
                    spriteBatch.Draw(btnTxButtonY, new Rectangle(206, 206, 26, 26), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Confirm", new Vector2(238, 206), Color.White);
                    spriteBatch.Draw(btnTxButtonB, new Rectangle(346, 206, 26, 26), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Cancel", new Vector2(378, 206), Color.White);
                }

                if (gameSuspendConfirm)
                {
                    spriteBatch.Draw(txWhite, new Rectangle(198, 78, GAMEFIELDWIDTH - 396, 160), Color.White);
                    spriteBatch.Draw(txBlue, new Rectangle(200, 80, GAMEFIELDWIDTH - 400, 156), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Suspend Game?", new Vector2(204, 84), Color.Yellow);
                    spriteBatch.DrawString(sfFranklinGothic, "The game will be suspended and will exit to the title screen.\nYou can resume playing at a later time.", new Vector2(204, 124), Color.White);
                    spriteBatch.Draw(btnTxButtonY, new Rectangle(206, 206, 26, 26), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Confirm", new Vector2(238, 206), Color.White);
                    spriteBatch.Draw(btnTxButtonB, new Rectangle(346, 206, 26, 26), Color.White);
                    spriteBatch.DrawString(sfSylfaen14, "Cancel", new Vector2(378, 206), Color.White);
                }

                if (tutorialMessageOpen)
                {
                    DrawTutorialMessage(currentTutorialMessageOpen);
                }

                //Draw a black rectangle to cover anything that might draw past the bottom of intended display area
                spriteBatch.Draw(txBlack, new Rectangle(0, GAMEFIELDHEIGHT, GAMEFIELDWIDTH, 400), Color.White);

                spriteBatch.End();
            }

            GraphicsDevice.SetRenderTarget(null);
            basicEffect.Texture = playFieldRenderTarget;
            GraphicsDevice.Clear(clearColor);

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList, playfieldQuad.Vertices, 0, 4, playfieldQuad.Indexes, 0, 2);
            }

            if (helpOpen)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(txBlack, new Rectangle(0, 0, DISPLAYWIDTH, DISPLAYHEIGHT), new Color(255, 255, 255, 160));
                DrawHelp(currentHelpPage);
                spriteBatch.End();
            }

        }

        private void DrawTutorialMessage(int aID)
        {
            TutorialMessage theTM = tutorialMessages[0];
            for (int i = 0; i < tutorialMessages.Count; i++)
            {
                if (tutorialMessages[i].ID == aID)
                {
                    theTM = tutorialMessages[i];
                }
            }
            int horizMargin = (GAMEFIELDWIDTH - TUTORIAL_WINDOW_WIDTH) / 2;
            int tutorialWindowHeight = 86 + 20 * currentTutorialMessage.Count;
            spriteBatch.Draw(txWhite, new Rectangle(horizMargin - 2, 98, TUTORIAL_WINDOW_WIDTH + 4, 2), new Color(255, 255, 255, (byte)64));
            spriteBatch.Draw(txWhite, new Rectangle(horizMargin - 2, 98 + tutorialWindowHeight + 4, TUTORIAL_WINDOW_WIDTH + 4, 2), new Color(255, 255, 255, (byte)64));
            spriteBatch.Draw(txWhite, new Rectangle(horizMargin - 2, 98, 2, tutorialWindowHeight + 4), new Color(255, 255, 255, (byte)64));
            spriteBatch.Draw(txWhite, new Rectangle(horizMargin + TUTORIAL_WINDOW_WIDTH, 98, 2, tutorialWindowHeight + 4), new Color(255, 255, 255, (byte)64));
            spriteBatch.Draw(txBlack, new Rectangle(horizMargin, 100, TUTORIAL_WINDOW_WIDTH, tutorialWindowHeight), new Color(255, 255, 255, (byte)128));
            spriteBatch.DrawString(sfSylfaen14, "Tutorial: " + theTM.title, new Vector2(horizMargin + 4, 102), Color.White);
            spriteBatch.Draw(txWhite, new Rectangle(horizMargin, 134, TUTORIAL_WINDOW_WIDTH, 2), Color.White);
            if (currentTutorialMessage.Count > 0)
            {
                for (int i = 0; i < currentTutorialMessage.Count; i++)
                {
                    spriteBatch.DrawString(sfFranklinGothic, currentTutorialMessage[i], new Vector2(horizMargin + 2, 140 + 20 * i), Color.White);
                }
            }
            spriteBatch.Draw(txWhite, new Rectangle(horizMargin, 152 + currentTutorialMessage.Count * 20, TUTORIAL_WINDOW_WIDTH, 2), Color.White);
            spriteBatch.Draw(btnTxButtonB, new Rectangle(horizMargin + 4, 158 + currentTutorialMessage.Count * 20, 24, 24), Color.White);
            spriteBatch.DrawString(sfSylfaen14, "Close", new Vector2(horizMargin + 32, 156 + currentTutorialMessage.Count * 20), Color.White);
            spriteBatch.Draw(btnTxButtonX, new Rectangle(horizMargin + 150, 158 + currentTutorialMessage.Count * 20, 24, 24), Color.White);
            spriteBatch.DrawString(sfSylfaen14, "Never Show Again", new Vector2(horizMargin + 178, 156 + currentTutorialMessage.Count * 20), Color.White);
        }

        private List<String> SplitText(int width, String aString, SpriteFont aFont)
        {
            List<String> result = new List<String>();
            if (aFont.MeasureString(aString).X <= width)
            {
                result.Add(aString);
                return result;
            }
            bool done = false;
            int charCount = 0;
            int lastSpaceOccurence = 0;
            while (!done)
            {
                charCount++;
                if (charCount >= aString.Length || aString[charCount] == ' ')
                {
                    if (aFont.MeasureString(aString.Substring(0, charCount)).X > width)
                    {
                        result.Add(aString.Substring(0, lastSpaceOccurence));
                        result = (result.Concat<String>(SplitText(width, aString.Substring(lastSpaceOccurence + 1), aFont))).ToList<String>();
                        done = true;
                    }
                    lastSpaceOccurence = charCount;
                }
            }
            return result;
        }

        private void DrawOutro()
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            for (int i = 0; i < outroWidth; i++)
            {
                for (int j = 0; j < outroHeight; j++)
                {
                    if (outroTiles[i][j] == TILE_WALL)
                    {
                        Texture2D tileToDraw = txTileWall;
                        switch (outroTileSet)
                        {
                            case 0:
                                tileToDraw = txTileWall;
                                break;
                            case 1:
                                tileToDraw = txTileSet1Wall;
                                break;
                            case 2:
                                tileToDraw = txTileSet2Wall;
                                break;
                            case 3:
                                tileToDraw = txTileSet3Wall;
                                break;
                            case 4:
                                tileToDraw = txTileSet4Wall;
                                break;
                            case 5:
                                tileToDraw = txTileSet5Wall;
                                break;
                            default:
                                tileToDraw = txTileWall;
                                break;
                        }
                        spriteBatch.Draw(tileToDraw, new Rectangle(16 * i, 16 * j, 16, 16), new Color(128, 128, 128, 128));
                    }

                }
            }

            if (outroText != null && outroText.Count > 0)
            {
                for (int i = 0; i < outroText.Count; i++)
                {
                    spriteBatch.DrawString(sfSylfaen14, outroText[i], new Vector2(250, DISPLAYHEIGHT + i * 24 - Math.Min(outroTextOffset, DISPLAYHEIGHT - 100)), Color.White);
                }
            }

            spriteBatch.End();
        }

        private void DrawHelp(int page)
        {

            String title = "Game Intro";
            switch (page)
            {
                case 1:
                    title = "Game Intro";
                    break;
                case 2:
                    title = "Hero Professions";
                    break;
                case 3:
                    title = "Controls (1)";
                    break;
                case 4:
                    title = "Controls (2)";
                    break;
                default:
                    title = "Basic Info";
                    break;
            }
            spriteBatch.DrawString(sfSylfaen14, title, new Vector2(DISPLAYWIDTH / 2 - 85, TITLE_SAFE_RECT.Top + 10), Color.White);

            if (page < NUM_HELP_PAGES)
            {
                spriteBatch.Draw(btxTxRightShoulder, new Rectangle(DISPLAYWIDTH - 250, 100, 64, 32), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "Next\nPage", new Vector2(DISPLAYWIDTH - 320, 85), Color.White);
            }
            if (page > 1)
            {
                spriteBatch.Draw(btnTxLeftShoulder, new Rectangle(180, 100, 64, 32), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "Prev\nPage", new Vector2(255, 85), Color.White);
            }
            spriteBatch.DrawString(sfSylfaen14, "Page " + page.ToString() + "/" + NUM_HELP_PAGES.ToString(),
                                    new Vector2(DISPLAYWIDTH - 250, DISPLAYHEIGHT - 150), Color.White);
            spriteBatch.Draw(btnTxButtonB, new Rectangle(DISPLAYWIDTH - 250, DISPLAYHEIGHT - 120, 24, 24), Color.White);
            spriteBatch.DrawString(sfSylfaen14, "Close", new Vector2(DISPLAYWIDTH - 220, DISPLAYHEIGHT - 120), Color.White);

            if (page == 1)
            {
                spriteBatch.DrawString(sfSylfaen14, "Your once-peaceful land is now threatened by the dark influence of the " +
                                                    "\nEmpire of Jem. Your village is cut off - surrounded by monsters" +
                                                    "\nsummoned to do the Empire's bidding.",
                                                    new Vector2(160, 180), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "It's time to fight back! The village folk form an Adventurers Guild" +
                                                    "\nto organise and reward any adventurer brave enough to help." +
                                                    "\nEvery task completed and enemy defeated brings the village one" +
                                                    "\nstep closer to stopping the Jem Empire's dark lord - GOLD RICK",
                                                    new Vector2(160, 300), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "Welcome to " + TITLE_STRING + ". A long war is ahead; and you will step into " +
                                                    "\nthe shoes of many heroes before Gold Rick is dead. But take heart - every" +
                                                    "\ngame you play can level up subsequent heroes; and brings you one \nstep closer to victory!",
                                                    new Vector2(160, 448), Color.White);
            }

            if (page == 2)
            {
                spriteBatch.DrawString(sfSylfaen14, "Fighter - Able to wield most weapon types, and naturally resilient. Even " +
                                                    "\nwhen things get too tough, Fighters can still Kick enemies out of their way!" +
                                                    "\nFighters are also natural leaders, and their allies can grow powerful.",
                                                    new Vector2(160, 200), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "Fighter - ", new Vector2(160, 200), Color.Red);
                spriteBatch.DrawString(sfSylfaen14, "Mage - Mages can cast powerful offensive spells, making them the most deadly" +
                                                    "\nheroes in the land. But their " + magicName.ToLower() + " power is limited, and they are vulnerable" +
                                                    "\nif it runs low. Mages can only wield daggers.", new Vector2(160, 320), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "Mage - ", new Vector2(160, 320), Color.Red);
                spriteBatch.DrawString(sfSylfaen14, "Explorer - Lacking the brawn of Fighters or the spellcasting of Mages, Explorers " +
                                                    "\nare at a disadvantage and must choose their fights carefully. On the plus side," +
                                                    "\nExplorers have wit and luck, and can navigate dungeons quicker than other heroes.",
                                                    new Vector2(160, 450), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "Explorer - ", new Vector2(160, 450), Color.Red);
            }

            if (page == 3)
            {
                spriteBatch.Draw(btnTxLeftStick, new Rectangle(120, 160, 64, 64), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "LEFT\nSTICK", new Vector2(130, 160), new Color(255, 255, 0, 100));
                spriteBatch.DrawString(sfSylfaen14, "Move Character / Change Menu Selection / Aim Spell", new Vector2(250, 180), Color.White);

                spriteBatch.Draw(btnTxRightStick, new Rectangle(120, 250, 64, 64), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "RIGHT\nSTICK", new Vector2(130, 250), new Color(255, 255, 0, 100));
                spriteBatch.DrawString(sfSylfaen14, "Look Around / Aim Spell", new Vector2(250, 270), Color.White);

                spriteBatch.Draw(btnTxButtonA, new Rectangle(130, 340, 32, 32), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "Confirm Menu Selection And Commands", new Vector2(250, 340), Color.White);

                spriteBatch.Draw(btnTxButtonB, new Rectangle(130, 380, 32, 32), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "Rest A Turn / Cancel Menu Selection", new Vector2(250, 380), Color.White);

                spriteBatch.Draw(btnTxRightTrigger, new Rectangle(130, 425, 32, 64), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "RT", new Vector2(160, 425), new Color(255, 255, 0, 100));
                spriteBatch.DrawString(sfSylfaen14, "View Commands Possible For Space Being Aimed At With Right Stick." +
                                                    "\nIf Not Aiming With Right Stick, Shows Commands For Space You're Standing On", new Vector2(250, 425), Color.White);

                spriteBatch.Draw(btnTxLeftTrigger, new Rectangle(130, 510, 32, 64), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "LT", new Vector2(160, 510), new Color(255, 255, 0, 100));
                spriteBatch.DrawString(sfSylfaen14, "Execute Commands Automatically Without Opening The Command List" +
                                                    "\nQuicker And Easier Than Using Right Trigger, Give It A Try!", new Vector2(250, 510), Color.White);

            }

            if (page == 4)
            {
                spriteBatch.Draw(btnTxLeftStick, new Rectangle(120, 180, 64, 64), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "LEFT\nSTICK", new Vector2(130, 180), new Color(255, 255, 0, 100));
                spriteBatch.DrawString(sfSylfaen14, "+", new Vector2(208, 200), Color.White);
                spriteBatch.Draw(btnTxLeftShoulder, new Rectangle(230, 200, 64, 32), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "Move In Direction Continuously (Stops If Danger Spotted)", new Vector2(320, 192), Color.White);

                spriteBatch.Draw(btnTxRightStick, new Rectangle(120, 270, 64, 64), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "RIGHT\nSTICK", new Vector2(130, 270), new Color(255, 255, 0, 100));
                spriteBatch.DrawString(sfSylfaen14, "+", new Vector2(208, 290), Color.White);
                spriteBatch.Draw(btnTxLeftShoulder, new Rectangle(230, 290, 64, 32), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "Look At Distant Spaces", new Vector2(320, 290), Color.White);

                spriteBatch.Draw(btnTxButtonB, new Rectangle(120, 352, 32, 32), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "+", new Vector2(164, 356), Color.White);
                spriteBatch.Draw(btnTxLeftShoulder, new Rectangle(200, 352, 64, 32), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "Rest Continuously While Both Buttons Are Held", new Vector2(320, 356), Color.White);

                spriteBatch.Draw(btxTxRightShoulder, new Rectangle(120, 404, 64, 32), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "Open Spell List", new Vector2(320, 408), Color.White);

                spriteBatch.Draw(btnTxButtonY, new Rectangle(120, 458, 32, 32), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "Open Inventory", new Vector2(320, 460), Color.White);

                spriteBatch.Draw(btnTxBack, new Rectangle(120, 510, 40, 40), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "View Map Of Known Dungeon", new Vector2(320, 514), Color.White);

                spriteBatch.Draw(btnTxStart, new Rectangle(120, 550, 40, 40), Color.White);
                spriteBatch.DrawString(sfSylfaen14, "Open In-Game Menu", new Vector2(320, 554), Color.White);

            }

        }

        private void DrawBossProgress(int left, int top, int width)
        {
            spriteBatch.Draw(txWhite, new Rectangle(left, top, width, 50), Color.White);
            spriteBatch.Draw(txBlue, new Rectangle(left + 2, top + 2, width - 4, 46), Color.White);
            if (!QuestListContainsBoss())
            {
                int lastBoss = LevelOfLastDefeatedBoss();
                int nextBoss = LevelOfNextUndefeatedBoss();
                if (nextBoss > 0)
                {
                    int progressToNextBoss = Math.Max(1, (nextBoss - MAX_STARTING_QUEST_LEVEL));
                    int barLength = (width - 42) - (int)((float)(width - 42) * ((float)(progressToNextBoss) / (float)(nextBoss - lastBoss)));
                    spriteBatch.DrawString(sfFranklinGothic, "Next Boss:", new Vector2(left + 6, top + 4), Color.White);
                    spriteBatch.Draw(txBlack, new Rectangle(left + 10, top + 30, width - 42, 12), Color.White);
                    spriteBatch.Draw(txWhite, new Rectangle(left + 10, top + 30, barLength, 12), new Color(255, 255, 100));
                    spriteBatch.Draw(txIconBoss, new Rectangle(left + width - 38, top + 12, 32, 32), Color.White);
                }
            }
            else
            {
                spriteBatch.DrawString(sfFranklinGothic, "DEFEAT THE BOSS TO PROGRESS!", new Vector2(left + 30, top + 15), Color.White);
            }
        }

        private void DrawStatusBar(int top, int left, int length, int progress, Color color)
        {
            int thickness = 20;
            spriteBatch.Draw(txBlack, new Rectangle(left, top, length, thickness), Color.White);
            spriteBatch.Draw(txWhite, new Rectangle(left, top, progress, thickness), color);
            spriteBatch.Draw(txBlack, new Rectangle(left, top, progress, 2), new Color(100, 100, 100, 100));
            spriteBatch.Draw(txBlack, new Rectangle(left, top + 2, progress, 2), new Color(40, 40, 40, 40));
            spriteBatch.Draw(txWhite, new Rectangle(left, top + 8, progress, 2), new Color(40, 40, 40, 40));
            spriteBatch.Draw(txWhite, new Rectangle(left, top + 10, progress, 2), new Color(100, 100, 100, 100));
            spriteBatch.Draw(txWhite, new Rectangle(left, top + 12, progress, 2), new Color(40, 40, 40, 40));
            spriteBatch.Draw(txBlack, new Rectangle(left, top + thickness - 2, progress, 2), new Color(100, 100, 100, 100));
            spriteBatch.Draw(txBlack, new Rectangle(left, top + thickness - 4, progress, 2), new Color(40, 40, 40, 40));
        }

        private void DrawFeatPanel(int left, int top, Feat feat)
        {
            int width = 300;
            int height = 80;
            spriteBatch.Draw(txWhite, new Rectangle(left, top, width, height), Color.LightBlue);
            spriteBatch.Draw(txBlack, new Rectangle(left + 2, top + 2, width - 4, height - 4), Color.White);
            spriteBatch.Draw(txWhite, new Rectangle(left + 2, top + 20, width - 4, 2), Color.LightBlue);
            if (feat.hidden)
            {
                spriteBatch.DrawString(sfFranklinGothic, "??????????", new Vector2(left + 6, top - 2), Color.White);
            }
            else
            {
                spriteBatch.DrawString(sfFranklinGothic, feat.title, new Vector2(left + 6, top - 2), Color.Yellow);
            }
            //If unlocked, display details 
            if (!feat.hidden)
            {
                //If not fully ranked up, display progress
                if (feat.rank < feat.MAX_RANK)
                {
                    spriteBatch.DrawString(sfFranklinGothic, "RANK: " + feat.rank, new Vector2(left + 6, top + 23), Color.White);
                    spriteBatch.DrawString(sfFranklinGothic, "Next: " + feat.progress + "/" + feat.goal, new Vector2(left + 6, top + 43), Color.White);
                    spriteBatch.Draw(txBlack, new Rectangle(left + 6, top + 70, width - 12, 4), Color.White);
                    int progressWidth = (int)((float)(width - 12) * ((float)(feat.progress) / (float)(feat.goal)));
                    spriteBatch.Draw(txWhite, new Rectangle(left + 6, top + 70, progressWidth, 4), new Color(255, 255, 0, 255));
                }
                else
                {
                    //If fully ranked up
                    if (feat.MAX_RANK > 1)
                    {
                        spriteBatch.DrawString(sfFranklinGothic, "MAX RANK " + feat.rank, new Vector2(left + 6, top + 24), Color.Yellow);
                    }
                    else
                    //If only one rank, just display as UNLOCKED
                    {
                        spriteBatch.DrawString(sfFranklinGothic, "FEAT ACHIEVED", new Vector2(left + 6, top + 24), Color.Yellow);
                    }
                }
                spriteBatch.DrawString(sfFranklinGothic, feat.description, new Vector2(left, top + height - 1), Color.White);
            }
            else
            {
                spriteBatch.Draw(txBlack, new Rectangle(left + 6, top + 70, width - 12, 4), Color.White);
                int progressWidth = (int)((float)(width - 12) * ((float)(feat.progress) / (float)(feat.goal)));
                spriteBatch.Draw(txWhite, new Rectangle(left + 6, top + 70, progressWidth, 4), new Color(255, 255, 0, 255));
                spriteBatch.DrawString(sfFranklinGothic, feat.descriptionWhenHidden, new Vector2(left, top + height - 1), Color.White);
            }
        }

        private void DrawFeatSummaryPage()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    DrawFeatPanel(200 + (j * 500), 100 + (i * 110), featList[j * 5 + i]);
                }
            }
        }

        private void DrawItemComparisonPanel(int left, int top, int width, Item theItem)
        {
            spriteBatch.Draw(txWhite, new Rectangle(left - 2, top - 2, width + 4, 50), Color.White);
            spriteBatch.Draw(txBlue, new Rectangle(left, top, width, 46), Color.White);
            if (theItem.itemType == (int)SC.ItemTypes.ARMOUR
                || theItem.itemType == (int)SC.ItemTypes.WEAPON
                || theItem.itemType == (int)SC.ItemTypes.SHIELD
                || theItem.itemType == (int)SC.ItemTypes.CLOAK)
            {
                String comparison = "";
                Color compColor = Color.White;
                double diff = 0;
                switch (theItem.itemType)
                {
                    case (int)SC.ItemTypes.ARMOUR:
                        diff = player1.CompareArmour(theItem);
                        comparison = theItem.defence.ToString() + " Defence";
                        if (diff < 0)
                        {
                            compColor = Color.OrangeRed;
                        }
                        if (diff > 0)
                        {
                            compColor = Color.LightGreen;
                        }
                        break;
                    case (int)SC.ItemTypes.SHIELD:
                        diff = player1.CompareShield(theItem);
                        comparison = theItem.defence.ToString() + " Defence";
                        if (diff < 0)
                        {
                            compColor = Color.OrangeRed;
                        }
                        if (diff > 0)
                        {
                            compColor = Color.LightGreen;
                        }
                        break;
                    case (int)SC.ItemTypes.WEAPON:
                        diff = player1.CompareSword(theItem);
                        comparison = theItem.damage.ToString() + " Attack";
                        if (diff < 0)
                        {
                            compColor = Color.OrangeRed;
                        }
                        if (diff > 0)
                        {
                            compColor = Color.LightGreen;
                        }
                        break;
                    case (int)SC.ItemTypes.CLOAK:
                        diff = player1.CompareCloak(theItem);
                        comparison = theItem.defence.ToString() + " Defence";
                        if (diff < 0)
                        {
                            compColor = Color.OrangeRed;
                        }
                        if (diff > 0)
                        {
                            compColor = Color.LightGreen;
                        }
                        break;
                    default:
                        break;
                }
                if (!comparison.Equals(""))
                {
                    spriteBatch.DrawString(sfSylfaen14, comparison, new Vector2(left + 6, top + 4), compColor);
                }
                if (theItem.itemType == (int)SC.ItemTypes.WEAPON)
                {
                    double fireDiff = player1.CompareSwordFire(theItem);
                    double iceDiff = player1.CompareSwordIce(theItem);
                    if (fireDiff != 0 || iceDiff != 0)
                    {
                        spriteBatch.Draw(txIconFlame, new Rectangle(left + 200, top + 4, 32, 32), Color.White);
                        spriteBatch.Draw(txIconIce, new Rectangle(left + 500, top + 4, 32, 32), Color.White);

                        Color colorToDraw = Color.White;
                        String elementCompare = theItem.elementalOffenceFire.ToString() + " Flame Damage";
                        if (fireDiff > 0)
                        {
                            colorToDraw = Color.LightGreen;
                            elementCompare = "+" + elementCompare;
                        }
                        if (fireDiff < 0)
                        {
                            colorToDraw = Color.OrangeRed;
                        }
                        spriteBatch.DrawString(sfSylfaen14, elementCompare, new Vector2(left + 228, top + 4), colorToDraw);

                        colorToDraw = Color.White;
                        elementCompare = theItem.elementalOffenceIce.ToString() + " Ice Damage";
                        if (iceDiff > 0)
                        {
                            colorToDraw = Color.LightGreen;
                            elementCompare = "+" + elementCompare;
                        }
                        if (iceDiff < 0)
                        {
                            colorToDraw = Color.OrangeRed;
                        }
                        spriteBatch.DrawString(sfSylfaen14, elementCompare, new Vector2(left + 528, top + 4), colorToDraw);

                    }
                }
                if (theItem.itemType == (int)SC.ItemTypes.CLOAK)
                {
                    double fireDiff = player1.CompareCloakFire(theItem);
                    double iceDiff = player1.CompareCloakIce(theItem);
                    Color colorToDraw = Color.White;
                    String elementCompare = (100 - theItem.elementalDefenceFire * 100).ToString() + "% Flame Defence";
                    if (fireDiff < 0)
                    {
                        colorToDraw = Color.LightGreen;
                        //elementCompare = "+" + elementCompare;
                    }
                    if (fireDiff > 0)
                    {
                        colorToDraw = Color.OrangeRed;
                    }
                    spriteBatch.DrawString(sfSylfaen14, elementCompare, new Vector2(left + 228, top + 4), colorToDraw);

                    colorToDraw = Color.White;
                    elementCompare = (100 - theItem.elementalDefenceIce * 100).ToString() + "% Ice Defence";
                    if (iceDiff < 0)
                    {
                        colorToDraw = Color.LightGreen;
                    }
                    if (iceDiff > 0)
                    {
                        colorToDraw = Color.OrangeRed;
                    }
                    spriteBatch.DrawString(sfSylfaen14, elementCompare, new Vector2(left + 528, top + 4), colorToDraw);
                }
            }
            else
            {
                spriteBatch.DrawString(sfSylfaen14, theItem.itemDescription, new Vector2(left + 6, top + 4), Color.White);
            }
        }

        private void DrawListOfFurniture(List<Furniture> furnitureInView)
        {
            if (furnitureInView.Count > 0)
            {
                Texture2D furnToDraw = txFurnStatue;
                Color colorToDraw;
                for (int i = 0; i < furnitureInView.Count; i++)
                {
                    colorToDraw = new Color(1, 1, 1) * furnitureInView[i].visibility;
                    switch (furnitureInView[i].furnType)
                    {
                        case (int)SC.FurnTypes.ALTAR:
                            furnToDraw = txFurnAltar;
                            break;
                        case (int)SC.FurnTypes.BARREL:
                            furnToDraw = txFurnBarrel;
                            break;
                        case (int)SC.FurnTypes.CANDELABRA:
                            furnToDraw = txFurnCandelabra;
                            break;
                        case (int)SC.FurnTypes.CRATE:
                            furnToDraw = txFurnCrate;
                            break;
                        case (int)SC.FurnTypes.GRAVESTONE:
                            furnToDraw = txFurnGravestone;
                            break;
                        case (int)SC.FurnTypes.SIGNPOST:
                            furnToDraw = txFurnSignpost;
                            break;
                        case (int)SC.FurnTypes.STATUE:
                            furnToDraw = txFurnStatue;
                            break;
                        case (int)SC.FurnTypes.TABLE:
                            furnToDraw = txFurnTable;
                            break;
                        case (int)SC.FurnTypes.CHAIR_LEFT:
                            furnToDraw = txFurnChairLeft;
                            break;
                        case (int)SC.FurnTypes.CHAIR_RIGHT:
                            furnToDraw = txFurnChairRight;
                            break;
                        case (int)SC.FurnTypes.TELEPORTER:
                            furnToDraw = txFurnTeleporter;
                            break;
                        default:
                            furnToDraw = txFurnStatue;
                            break;
                    }
                    spriteBatch.Draw(furnToDraw, new Rectangle(furnitureInView[i].x * TILE_SIZE - player1.x * TILE_SIZE + halfTilesInViewX * TILE_SIZE,
                                                                furnitureInView[i].y * TILE_SIZE - player1.y * TILE_SIZE + halfTilesInViewY * TILE_SIZE,
                                                                TILE_SIZE, TILE_SIZE), colorToDraw);
                }
            }
        }

        private void DrawStorageAccessNotification(int left, int top, byte alpha)
        {
            Color color = new Color(alpha, alpha, alpha, alpha);
            spriteBatch.Draw(txBlack, new Rectangle(left, top, 100, 100), color);
            spriteBatch.Draw(RandomTexture(), new Rectangle(left + 10, top + 10, 80, 80), color);
            spriteBatch.DrawString(sfFranklinGothic, "WAIT!", new Vector2(left + 25, top + 65), new Color(alpha,alpha,0, alpha));
        }

        private Texture2D RandomTexture()
        {
            int MAX_TEXTURES = 20;
            Texture2D result = txCreatureBat;
            int choice = random.Next(MAX_TEXTURES);
            switch (choice)
            {
                case 0:
                    result = txCreatureBat;
                    break;
                case 1:
                    result = txCreatureBug;
                    break;
                case 2:
                    result = txCreatureCanine;
                    break;
                case 3:
                    result = txCreatureDragon;
                    break;
                case 4:
                    result = txCreatureGhost;
                    break;
                case 5:
                    result = txCreatureGoblin;
                    break;
                case 6:
                    result = txCreatureHuman;
                    break;
                case 7:
                    result = txCreatureLizard;
                    break;
                case 8:
                    result = txCreatureMerchant;
                    break;
                case 9:
                    result = txCreatureRodent;
                    break;
                case 10:
                    result = txCreatureSkeleton;
                    break;
                case 11:
                    result = txCreatureSlime;
                    break;
                case 12:
                    result = txCreatureSnake;
                    break;
                case 13:
                    result = txCreatureSuccubus;
                    break;
                case 14:
                    result = txCreatureTroll;
                    break;
                case 15:
                    result = txItemCoins;
                    break;
                case 16:
                    result = txItemSword;
                    break;
                case 17:
                    result = txPlayerElf;
                    break;
                case 18:
                    result = txPlayerHuman;
                    break;
                case 19:
                    result = txPlayerOrc;
                    break;
                default:
                    result = txCreatureCook;
                    break;
            }
            return result;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (currentGameState == GAMESTATE_TITLESCREEN) DrawTitleScreen();

            if (currentGameState == GAMESTATE_MAINMENU) DrawMainMenu();

            if (currentGameState == GAMESTATE_GAMEOVERSCREEN) DrawGameOverScreen();

            if (currentGameState == GAMESTATE_CHARACTERCREATION) DrawCharacterCreationScreen();

            if (currentGameState == GAMESTATE_PLAYING) DrawGamePlaying();

            if (currentGameState == GAMESTATE_OUTRO) DrawOutro();

            if (currentGameState == GAMESTATE_LOGOSCREEN) DrawLogoScreen();

            if (currentGameState == GAMESTATE_MUSIC_LOGO) DrawMusicLogo();

            if (currentGameState == GAMESTATE_ORAWART_LOGO) DrawOrawartLogo();

            if (currentGameState == GAMESTATE_STORAGE_WARNING) DrawStorageWarningScreen();

            if (resumableGameLoading || resumableGameSaving)
            {
                spriteBatch.Begin();
                GraphicsDevice.BlendState = BlendState.AlphaBlend;
                int left = 270;
                int top = 400;

                spriteBatch.Draw(txBlack, new Rectangle(0, 0, DISPLAYWIDTH, DISPLAYHEIGHT), new Color(255, 255, 255, 180));
                spriteBatch.Draw(txBlack, new Rectangle(left, top, 750, 60), new Color(255, 255, 255, 180));
                spriteBatch.DrawString(sfSylfaen14, "ACCESSING STORAGE DEVICE. PLEASE DO NOT REMOVE\n"
                                                + "STORAGE DEVICES OR TURN OFF YOUR CONSOLE", new Vector2(left + 50, top), Color.Yellow);
                Texture2D randomTexture = RandomTexture();
                spriteBatch.Draw(randomTexture, new Rectangle(left, top + 8, 40, 40), Color.White);
                spriteBatch.Draw(randomTexture, new Rectangle(left + 710, top + 8, 40, 40), Color.White);
                spriteBatch.End();
            }

            if (!notCurrentlySaving || !notCurrentlyLoading || guiStorageNotifyFade > 0)
            {
                spriteBatch.Begin();
                if (!notCurrentlySaving || !notCurrentlyLoading)
                {
                    DrawStorageAccessNotification(DISPLAYWIDTH - 240, 100, 255);
                }
                else
                {
                    DrawStorageAccessNotification(DISPLAYWIDTH - 240, 100, guiStorageNotifyFade);
                    guiStorageNotifyFade = (byte)Math.Max(0, ((int)guiStorageNotifyFade) - 8);
                }
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}