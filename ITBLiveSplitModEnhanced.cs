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
        #region GUI Fields
        private Rect runSelectorRect = new Rect(10f, 10f, 270f, 340f);
        private float buttonWidth = 260f;
        private float buttonHeight = 30f;
        private static bool showStartSplits = false;
        private static bool showPauseSplits = false;
        private static bool showEndSplits = false;
        private static bool showDeathSplits = false;
        private static bool showDarkroomsSplits = false;
        private static bool showGarageSplits = false;
        private static bool showOfficeSplits = false;
        private static bool showSewersSplits = false;
        private static bool showHotelSplits = false;
        #endregion

        //Setup Splits
        #region Splits Fields
        //Start Splits
        private static bool ladderStart = false;
        private static bool darkroomsStart = false;
        private static bool garageStart = false;
        private static bool officeStart = false;
        private static bool chaseStart = false;
        private static bool funRoomStart = false;
        private static bool sewersStart = false;
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
        private static bool officeSplitsChase = false;
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
        #endregion

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
                MelonEvents.OnGUI.Subscribe(DrawRunSelector, 100);
                if (lsm == null)
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                    return;
                }
                lsm.ResetTimer(); //Probably bad practice considering Hotel Ending wouldnt want reset? Though I guess you can just add times in post.
            }
            if (sceneName == "MainLevel" || sceneName == "HOTEL_SCENE")
            {
                inGame = true;
                MelonEvents.OnGUI.Unsubscribe(DrawRegisteredMods);
                MelonEvents.OnGUI.Unsubscribe(DrawRunSelector);
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

        public void CreateButton(string label, ref bool boolVariable, float offsety)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

            if (boolVariable)
            {
                buttonStyle.normal.background = Texture2D.whiteTexture;
                buttonStyle.normal.textColor = Color.black;
            }
            else
            {
                buttonStyle.normal.background = GUI.skin.button.normal.background;
                buttonStyle.normal.textColor = GUI.skin.button.normal.textColor;
            }

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + offsety + 225f, buttonWidth, buttonHeight), label, buttonStyle))
            {
                boolVariable = !boolVariable;
                MelonLogger.Msg(System.ConsoleColor.Green, label + " Button Down");
                MelonLogger.Msg(System.ConsoleColor.Green, label + " bool changed to " + boolVariable);
            }
        }

        public void SetAllButtonsFalse()
        {
            showStartSplits = false;
            showPauseSplits = false;
            showEndSplits = false;
            showDeathSplits = false;
            showDarkroomsSplits = false;
            showGarageSplits = false;
            showOfficeSplits = false;
            showSewersSplits = false;
            showHotelSplits = false;
        }

        public void DrawRunSelector()
        {
            GUI.Box(runSelectorRect, "Splits Customizer");

            #region Start Splits

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 20f, buttonWidth, buttonHeight), "Splits that Start"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Start Splits Button Down");
                if (showStartSplits)
                {
                    SetAllButtonsFalse();
                }
                else
                {
                    SetAllButtonsFalse();
                    showStartSplits = true;
                }
            }

            if (showStartSplits)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 350f, 270f, buttonHeight * 8 + 5 * 9), "");
                //Note this will need moved down with the introduction of death + level split buttons
                CreateButton("Ladder Start", ref ladderStart, 130f);
                CreateButton("Darkrooms Start", ref darkroomsStart, 165f);
                CreateButton("Garage Start", ref garageStart, 200f);
                CreateButton("Office Start", ref officeStart, 235f);
                CreateButton("Fun Room Start", ref funRoomStart, 270f);
                CreateButton("Chase Start", ref chaseStart, 305f);
                CreateButton("Sewers Start", ref sewersStart, 340f);
                CreateButton("Hotel Start", ref hotelStart, 375f);
            }
            #endregion

            #region Pause Splits

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 55f, buttonWidth, buttonHeight), "Splits that Pause"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Pause Splits Button Down");
                if (showPauseSplits)
                {
                    SetAllButtonsFalse();
                }
                else
                {
                    SetAllButtonsFalse();
                    showPauseSplits = true;
                }
            }

            if (showPauseSplits)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 350f, 270f, buttonHeight * 5 + 5 * 6), "");
                //Note this will need moved down with the introduction of death + level split buttons
                CreateButton("Escape End Pause", ref pauseSplitsEscapeEnd, 130f);
                CreateButton("Chase End Pause", ref pauseSplitsChaseEnd, 165f);
                CreateButton("Fun End Pause", ref pauseSplitsFunEnd, 200f);
                CreateButton("Sewer End Pause", ref pauseSplitsSewersEnd, 235f);
                CreateButton("Hotel End Pause", ref pauseSplitsHotelEnd, 270f);
            }
            #endregion

            #region End Splits

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 90f, buttonWidth, buttonHeight), "Splits that End"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "End Splits Button Down");
                if (showEndSplits)
                {
                    SetAllButtonsFalse();
                }
                else
                {
                    SetAllButtonsFalse();
                    showEndSplits = true;
                }
            }

            if (showEndSplits)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 350f, 270f, buttonHeight * 5 + 5 * 6), "");
                //Note this will need moved down with the introduction of death + level split buttons
                CreateButton("Escape End", ref endSplitsEscapeEnd, 130f);
                CreateButton("Chase End", ref endSplitsChaseEnd, 165f);
                CreateButton("Fun End", ref endSplitsFunEnd, 200f);
                CreateButton("Sewer End", ref endSplitsSewersEnd, 235f);
                CreateButton("Hotel End", ref endSplitsHotelEnd, 270f);
            }
            #endregion

            #region Death Splits

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 125f, buttonWidth, buttonHeight), "Death Splits"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Death Splits Button Down");
                if (showDeathSplits)
                {
                    SetAllButtonsFalse();
                }
                else
                {
                    SetAllButtonsFalse();
                    showDeathSplits = true;
                }
            }

            if (showDeathSplits)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 350f, 270f, buttonHeight * 15 + 5 * 16), "");
                //Note this will need moved down with the introduction of death + level split buttons
                CreateButton("Bacteria Death", ref deathSplitsBacteria, 130f);
                CreateButton("Dog Death", ref deathSplitsDog, 165f);
                CreateButton("Smiler Death", ref deathSplitsSmiler, 200f);
                CreateButton("Radiation Death", ref deathSplitsRadiation, 235f);
                CreateButton("Skin Stealer Death", ref deathSplitsSkinStealer, 270f);
                CreateButton("Gas Death", ref deathSplitsGas, 305f);
                CreateButton("Party Goer Death", ref deathSplitsPartyGoer, 340f);
                CreateButton("Balloons Death", ref deathSplitsBalloons, 375f);
                CreateButton("Electricity Death", ref deathSplitsElectricity, 410f);
                CreateButton("Spider Death", ref deathSplitsSpider, 445f);
                CreateButton("Rat Death", ref deathSplitsRat, 480f);
                CreateButton("Rat Death", ref deathSplitsCrusher, 515f);
                CreateButton("Spikes Death", ref deathSplitsSpikes, 550f);
                CreateButton("Moth Death", ref deathSplitsMoth, 585f);
                CreateButton("Receptionist Death", ref deathSplitsReceptionist, 620f);
            }

            #endregion

            #region Darkrooms Splits

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 160f, buttonWidth, buttonHeight), "Darkrooms Splits"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Darkrooms Splits Button Down");
                if (showDarkroomsSplits)
                {
                    SetAllButtonsFalse();
                }
                else
                {
                    SetAllButtonsFalse();
                    showDarkroomsSplits = true;
                }
            }

            if (showDarkroomsSplits)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 350f, 270f, buttonHeight * 5 + 5 * 6), "");
                //Note this will need moved down with the introduction of death + level split buttons
                CreateButton("Darkrooms Test 1", ref endSplitsEscapeEnd, 130f);
                CreateButton("Darkrooms Test 2", ref endSplitsChaseEnd, 165f);
                CreateButton("Darkrooms Test 3", ref endSplitsFunEnd, 200f);
                CreateButton("Darkrooms Test 4", ref endSplitsSewersEnd, 235f);
                CreateButton("Darkrooms Test 5", ref endSplitsHotelEnd, 270f);
            }

            #endregion

            #region Garage Splits

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 195f, buttonWidth, buttonHeight), "Garage Splits"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Garage Splits Button Down");
                if (showGarageSplits)
                {
                    SetAllButtonsFalse();
                }
                else
                {
                    SetAllButtonsFalse();
                    showGarageSplits = true;
                }
            }

            if (showGarageSplits)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 350f, 270f, buttonHeight * 5 + 5 * 6), "");
                //Note this will need moved down with the introduction of death + level split buttons
                CreateButton("Garage Test 1", ref endSplitsEscapeEnd, 130f);
                CreateButton("Garage Test 2", ref endSplitsChaseEnd, 165f);
                CreateButton("Garage Test 3", ref endSplitsFunEnd, 200f);
                CreateButton("Garage Test 4", ref endSplitsSewersEnd, 235f);
                CreateButton("Garage Test 5", ref endSplitsHotelEnd, 270f);
            }

            #endregion

            #region Office Splits

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 230f, buttonWidth, buttonHeight), "Office Splits"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Office Splits Button Down");
                if (showOfficeSplits)
                {
                    SetAllButtonsFalse();
                }
                else
                {
                    SetAllButtonsFalse();
                    showOfficeSplits = true;
                }
            }

            if (showOfficeSplits)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 350f, 270f, buttonHeight * 5 + 5 * 6), "");
                //Note this will need moved down with the introduction of death + level split buttons
                CreateButton("Office Test 1", ref endSplitsEscapeEnd, 130f);
                CreateButton("Office Test 2", ref endSplitsChaseEnd, 165f);
                CreateButton("Office Test 3", ref endSplitsFunEnd, 200f);
                CreateButton("Office Test 4", ref endSplitsSewersEnd, 235f);
                CreateButton("Office Test 5", ref endSplitsHotelEnd, 270f);
            }

            #endregion

            #region Sewers Splits

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 265f, buttonWidth, buttonHeight), "Sewers Splits"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Sewers Splits Button Down");
                if (showSewersSplits)
                {
                    SetAllButtonsFalse();
                }
                else
                {
                    SetAllButtonsFalse();
                    showSewersSplits = true;
                }
            }

            if (showSewersSplits)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 350f, 270f, buttonHeight * 5 + 5 * 6), "");
                //Note this will need moved down with the introduction of death + level split buttons
                CreateButton("Sewers Test 1", ref endSplitsEscapeEnd, 130f);
                CreateButton("Sewers Test 2", ref endSplitsChaseEnd, 165f);
                CreateButton("Sewers Test 3", ref endSplitsFunEnd, 200f);
                CreateButton("Sewers Test 4", ref endSplitsSewersEnd, 235f);
                CreateButton("Sewers Test 5", ref endSplitsHotelEnd, 270f);
            }

            #endregion

            #region Hotel Splits

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 300f, buttonWidth, buttonHeight), "Hotel Splits"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Hotel Splits Button Down");
                if (showHotelSplits)
                {
                    SetAllButtonsFalse();
                }
                else
                {
                    SetAllButtonsFalse();
                    showHotelSplits = true;
                }
            }

            if (showHotelSplits)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 350f, 270f, buttonHeight * 5 + 5 * 6), "");
                //Note this will need moved down with the introduction of death + level split buttons
                CreateButton("Hotel Test 1", ref endSplitsEscapeEnd, 130f);
                CreateButton("Hotel Test 2", ref endSplitsChaseEnd, 165f);
                CreateButton("Hotel Test 3", ref endSplitsFunEnd, 200f);
                CreateButton("Hotel Test 4", ref endSplitsSewersEnd, 235f);
                CreateButton("Hotel Test 5", ref endSplitsHotelEnd, 270f);
            }

            #endregion


            /*
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

                }
            }
            */
        }

        /*
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
        }
        
        //Split Timer when using the elevator
        [HarmonyPatch(typeof(Elevator), "RpcDoorElevatorPlay")]
        class ElevatorPatch
        {
            [HarmonyPrefix]
            internal static void RpcDoorElevatorPlayPrefix()
            {
                if (!elevator)
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "Testing Elevator");
                    if (lsm == null)
                    {
                        MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                        return;
                    }

                    lsm.SplitTimer();
                    elevator = true;
                }
            }
        }

        //Split Timer when the Mirror breaks
        [HarmonyPatch(typeof(LostPersonsPuzzle), "DestroyGlass")]
        class LostPersonsPuzzlePatch
        {
            [HarmonyPrefix]
            internal static void DestroyGlassPrefix()
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Testing Mirror Break");
                if (lsm == null)
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                    return;
                }

                lsm.SplitTimer();
            }
        }

        //Split Timer when the chain is cut after radiation
        [HarmonyPatch(typeof(DoorChain), "OnChainCut")]
        class DoorChainPatch
        {
            [HarmonyPrefix]
            internal static void OnChainCutPrefix()
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Testing Chain Cut");
                if (lsm == null)
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                    return;
                }

                lsm.SplitTimer();
                elevator = false;
            }
        }

        //Split Timer when the Party Room Starts
        [HarmonyPatch(typeof(PartygoerRoom), "StartPartyGames")]
        class PartygoerRoomPatch
        {
            [HarmonyPrefix]
            internal static void StartPartyGamesPrefix()
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Testing Party Room Start");
                if (lsm == null)
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                    return;
                }

                lsm.SplitTimer();
            }
        }

        //Split Timer when the Party Room Starts
        [HarmonyPatch(typeof(PartygoerCake), "Explode")]
        class PartygoerCakePatch
        {
            [HarmonyPrefix]
            internal static void ExplodePrefix()
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Testing Cake Explode");
                if (lsm == null)
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                    return;
                }

                lsm.SplitTimer();
            }
        }

        //Split Timer when the Security Door
        [HarmonyPatch(typeof(SecretGridComputerPuzzle), "SetGateOpenStatus")]
        class SecretGridComputerPuzzlePatch
        {
            [HarmonyPrefix]
            internal static void SetGateOpenStatusPrefix()
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Testing Security Door");
                if (lsm == null)
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                    return;
                }

                lsm.SplitTimer();
            }
        }

        //Split Timer when the Spikes off
        [HarmonyPatch(typeof(PikesDisabler), "DisablePikes")]
        class PikesDisablerPatch
        {
            [HarmonyPrefix]
            internal static void DisablePikesPrefix()
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Testing Disable Spikes");
                if (lsm == null)
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                    return;
                }

                lsm.SplitTimer();
            }
        }

        //Split Timer when the Garbage Crusher off
        [HarmonyPatch(typeof(GridPuzzle), "OnGridSolved")]
        class GridPuzzlePatch
        {
            [HarmonyPrefix]
            internal static void OnGridSolvedPrefix()
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Testing Garbage Crusher Discable");
                if (lsm == null)
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                    return;
                }

                lsm.SplitTimer();
            }
        }

        //Split Timer when the Medallion Puzzle
        [HarmonyPatch(typeof(MedallionsStatue), "OnUnlocked")]
        class MedallionsStatuePatch
        {
            [HarmonyPrefix]
            internal static void OnUnlockedPrefix()
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Testing Medallion puzzle");
                if (lsm == null)
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                    return;
                }

                lsm.SplitTimer();
            }
        }

        //Split Timer when the Gear Rotation
        [HarmonyPatch(typeof(GearPuzzle), "OnPuzzleSolve")]
        class GearPuzzlePatch
        {
            [HarmonyPrefix]
            internal static void OnPuzzleSolvePrefix()
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Testing Gear Rotation puzzle");
                if (lsm == null)
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                    return;
                }

                lsm.SplitTimer();
            }
        }

        //Split Timer when the Escaped/Chase Ending
        [HarmonyPatch(typeof(BackroomsExitZone), "OnTriggerEnter")]
        class BackroomsExitZonePatch
        {
            [HarmonyPrefix]
            internal static void OnTriggerEnterPrefix()
            {
                numEscaped++;
                if (numEscaped == numPlayers)
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "Testing Escape/Chase Ending");
                    if (lsm == null)
                    {
                        MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                        return;
                    }

                    lsm.SplitTimer();
                }
            }
        }

        //Split Timer when the Fun Ending and Sewer Ending 
        [HarmonyPatch(typeof(EndInteractable), "Interact")]
        class EndInteractablePatch
        {
            [HarmonyPrefix]
            internal static void InteractPrefix()
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Testing Fun/Sewer Ending");
                if (lsm == null)
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                    return;
                }

                lsm.SplitTimer();
            }
        }
        */
    }
}