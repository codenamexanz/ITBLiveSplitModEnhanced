using System.IO;
using MelonLoader;
using UnityEngine;
using static UnityEngine.Object;
using Il2Cpp;
using HarmonyLib;
using System.Reflection;
using Il2CppMirror;

namespace ITBLiveSplitModEnhanced
{
    /*
     * Enhanced LiveSplit Mod - Class used along side the LiveSplitClient to send commands to the server
     * 
     * Place what ever you need within here to start, split, reset etc. for the game you want to mod 
     */
    public class ITBLiveSplitModEnhanced : MelonMod
    {
        private string listOfMods = "Active Mods \n";

        private PlayerController[]? players;
        private static int numPlayers = 0;
        private static int numEscaped = 0;

        private static bool inGame = false;

        private static LiveSplitClient? lsm;
        //Setup InGame bool - maybe deprecated in the future?
        private static bool elevator = false;
        private static bool hammer = false;
        //Setup GUI
        private Rect runSelectorRect = new Rect(10f, 10f, 270f, 110f);
        private float buttonWidth = 260f;
        private float buttonWidth2 = 250f;
        private float buttonWidth3 = 240f;
        private float buttonHeight = 30f;
        private float smallButtonWidth = 80f;
        private static bool showCategoriesMenu = false;
        private static bool showEndingsMenu = false;
        private static bool showILsMenu = false;

        //Setup Splits
        //Start Splits
        private static bool ladderStart = false;
        private static bool hotelStart = false;
        //Pause Splits
        private static bool pauseSplitsEscapeEnd = false;
        private static bool pauseSplitsChaseEnd = false;
        private static bool pauseSplitsFunEnd = false;
        private static bool pauseSplitsSewersEnd = false;
        private static bool pauseSplitsHotelEnd = false;
        //End Splits
        private static bool endSplitsEscapeEnd = false;
        private static bool endSplitsChaseEnd = false;
        private static bool endSplitsFunEnd = false;
        private static bool endSplitsSewersEnd = false;
        private static bool endSplitsHotelEnd = false;
        //Death Splits
        private static bool deathSplitsBacteria = false;
        private static bool deathSplitsDog = false;
        private static bool deathSplitsSmiler = false;
        private static bool deathSplitsRadiation = false;
        private static bool deathSplitsSkinStealer = false;
        private static bool deathSplitsGas = false;
        private static bool deathSplitsPartyGoer = false;
        private static bool deathSplitsBalloons = false;
        private static bool deathSplitsElectricity = false;
        private static bool deathSplitsSpider = false;
        private static bool deathSplitsRat = false;
        private static bool deathSplitsCrusher = false;
        private static bool deathSplitsSpikes = false;
        private static bool deathSplitsMoth = false;
        private static bool deathSplitsReceptionist = false;
        //Darkrooms Splits
        private static bool darkroomsSplitsLobbyElevator = false;
        private static bool darkroomsSplitsVHSPutIn = false;
        private static bool darkroomsSplitsHammer = false;
        private static bool darkroomsSplitsClock = false;
        private static bool darkroomsSplitsRedKey = false;
        private static bool darkroomsSplitsFuseBox = false;
        private static bool darkroomsSplitsMirror = false;
        private static bool darkroomsSplitsLevers = false;
        private static bool darkroomsSplitsRadiation = false;
        private static bool darkroomsSplitsChainCut = false;
        //Garage Splits
        private static bool garageSplitsEnterGarage = false;
        private static bool garageSplitsValvesDone = false;
        private static bool garageSplitsGarageElevator = false;
        //Office Splits
        private static bool officeSplitsFusesDone = false;
        private static bool officeSplitsBlueDoor = false;
        private static bool officeSplitsPartyStart = false;
        private static bool officeSplitsBalloons = false;
        private static bool officeSplitsPresents = false;
        private static bool officeSplitsCakeExplode = false;
        private static bool officeSplitsRedDoor = false;
        private static bool officeSplitsSecurityGrid = false;
        //Sewers Splits
        private static bool sewersSplitsMetalDetector = false;
        private static bool sewersSplitsSpikesOff = false;
        private static bool sewersSplitsLabyrinthChain = false;
        private static bool sewersSplitsCrusherPuzzle = false;
        private static bool sewersSplitsGearSpawn = false;
        private static bool sewersSplitsCounterweights = false;
        //Hotel Splits
        private static bool hotelSplitsPaintings = false;
        private static bool hotelSplitsStatue = false;
        private static bool hotelSplitsVinyl = false;
        private static bool hotelSplitsPiano = false;
        private static bool hotelSplitsPhone = false;
        private static bool hotelSplitsGemIn = false;
        private static bool hotelSplitsBoilerKeys = false;
        private static bool hotelSplitsBoilersOn = false;
        private static bool hotelSplitsRingTable = false;


        public override void OnInitializeMelon()
        {
            foreach (MelonMod mod in RegisteredMelons)
            {
                listOfMods = listOfMods + mod.Info.Name + " by " + mod.Info.Author + "\n";
            }

            if (lsm == null)
            {
                lsm = new LiveSplitClient();
            }
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName == "MainMenu")
            {
                inGame = false;
                elevator = false;
                hammer = false;
                MelonEvents.OnGUI.Subscribe(DrawRegisteredMods, 100);
                //MelonEvents.OnGUI.Subscribe(DrawRunSelector, 100);
                if (lsm == null)
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                    return;
                }
                lsm.ResetTimer(); //Probably bad practice considering Hotel Ending wouldnt want reset? Though I guess you can just add times in post.
            }
            if (sceneName == "MainMenu" || sceneName == "HOTEL_SCENE")
            {
                inGame = true;
                MelonEvents.OnGUI.Unsubscribe(DrawRegisteredMods);
                //MelonEvents.OnGUI.Unsubscribe(DrawRunSelector);
            }
        }

        public override void OnUpdate()
        {
            if (inGame)
            {
                players = FindObjectsOfType<PlayerController>();
                if (numPlayers != players.Length)
                {
                    numPlayers = players.Length;
                    MelonLogger.Msg(System.ConsoleColor.Magenta, "Players in Lobby");
                    for (int i = 0; i < players.Length; i++)
                    {
                        MelonLogger.Msg(System.ConsoleColor.Magenta, players[i].name);
                    }
                }
            }
        }

        private void DrawRegisteredMods()
        {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.UpperRight;
            style.normal.textColor = Color.white;

            GUI.Label(new Rect(Screen.width - 500 - 10, 100, 500, 100), listOfMods, style);
        }

        /*public void CreateCategory(string category, float offsety)
        {
            if (GUI.Button(new Rect(runSelectorRect.x + 15f, runSelectorRect.y + offsety, buttonWidth3, 30f), category))
            {
                if (Enum.TryParse(category, out RunCategory runCategory))
                {
                    selectedRunCategory = runCategory;
                }
                else
                {
                    MelonLogger.Msg(System.ConsoleColor.Red, "Invalid Category: " + category);
                }
                MelonLogger.Msg(System.ConsoleColor.Green, category + " Button Down");
            }
        }

        public void DrawRunSelector()
        {
            GUI.Box(runSelectorRect, "Run Selector\nCurrent Setup: " + selectedGameRule.ToString() + " " + selectedRunCategory.ToString());
           
            // Run Types
            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 40f, smallButtonWidth, buttonHeight), "Any%"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Any% Button Down");
                selectedGameRule = GameRule.Any;
            }

            if (GUI.Button(new Rect(runSelectorRect.x + 95f, runSelectorRect.y + 40f, smallButtonWidth, buttonHeight), "Inbounds"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Inbounds Button Down");
                selectedGameRule = GameRule.Inbounds;
            }

            if (GUI.Button(new Rect(runSelectorRect.x + 185f, runSelectorRect.y + 40f, smallButtonWidth, buttonHeight), "Glitchless"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Glitchless Button Down");
                selectedGameRule = GameRule.Glitchless;
            }

            // Categories Menu
            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 75f, buttonWidth, buttonHeight), "Categories"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Categories Button Down");
                showCategoriesMenu = !showCategoriesMenu;
            }

            if (showCategoriesMenu)
            {
                GUI.Box(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 115f, buttonWidth, buttonHeight * 3 + 5 * (3+1)), "");

                // Endings submenu
                if (GUI.Button(new Rect(runSelectorRect.x + 10f, runSelectorRect.y + 120f, buttonWidth2, buttonHeight), "Endings"))
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "Endings Button Down");
                    showEndingsMenu = !showEndingsMenu;
                    showILsMenu = false;
                    //showCategoryExtensionsMenu = false;
                }
                if (showEndingsMenu)
                {
                    GUI.Box(new Rect(runSelectorRect.x + 10f, runSelectorRect.y + 230f, buttonWidth2, buttonHeight * 6 + 5 * (6+1)), "");
                    //Individual Endings
                    CreateCategory("Escape", 235f);
                    CreateCategory("Fun", 270f);
                    CreateCategory("Chase", 305f);
                    CreateCategory("Sewer", 340f);
                    CreateCategory("Hotel", 375f);
                    CreateCategory("AllEndings", 410f);
                }

                // ILs submenu
                if (GUI.Button(new Rect(runSelectorRect.x + 10f, runSelectorRect.y + 155f, buttonWidth2, buttonHeight), "Individual Levels"))
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "ILs Button Down");
                    showILsMenu = !showILsMenu;
                    showEndingsMenu = false;
                    //showCategoryExtensionsMenu = false;
                }
                if (showILsMenu)
                {
                    GUI.Box(new Rect(runSelectorRect.x + 10f, runSelectorRect.y + 230f, buttonWidth2, buttonHeight * 7 + 5 * (7 + 1)), "");
                    //Individual Levels
                    CreateCategory("Darkrooms", 235f);
                    CreateCategory("Garage", 270f);
                    CreateCategory("Office", 305f);
                    CreateCategory("FunRoom", 340f);
                    CreateCategory("ChaseLevel", 375f);
                    CreateCategory("Sewers", 410f);
                    CreateCategory("TerrorHotel", 445f);
                }
            }
        }

        //Start of Patches
        //Start Timer when using ladder on level 0 - Usable for Escape, Fun, Chase, Sewers, Hotel, All Endings
        [HarmonyPatch(typeof(BasePlayerController), "UseStairs")]
        class BasePlayerControllerPatch
        {
            [HarmonyPrefix]
            internal static void UseStairsPrefix()
            {
                if (new[] { RunCategory.Escape, RunCategory.Fun, RunCategory.Chase, RunCategory.Sewer, RunCategory.Hotel, RunCategory.AllEndings }.Contains(selectedRunCategory))
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "Testing Ladder");
                    if (lsm == null)
                    {
                        MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                        return;
                    }
                    lsm.StartTimer();
                }
            }
        }*/
    }
}