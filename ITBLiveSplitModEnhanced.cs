﻿using System.IO;
using MelonLoader;
using UnityEngine;
using static UnityEngine.Object;
using Il2Cpp;
using HarmonyLib;
using System.Reflection;
using Il2CppMirror;
using static System.Net.Mime.MediaTypeNames;

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
        private string version = "";

        private PlayerController[]? players;
        private static int numPlayers = 0;
        private static int numEscaped = 0;

        private static LiveSplitClient? lsm;

        private static bool inGame = false; // Maybe deprecated
        private static bool elevator = false;
        private static bool garageElevator = false;
        private static bool lightray = false;

        //Setup GUI
        #region GUI Fields
        private Rect runSelectorRect = new Rect(10f, 10f, 270f, 370f);
        private float buttonWidth = 260f;
        private float buttonHeight = 30f;
        #endregion

        //Setup Splits
        #region Splits Fields

        #region Show Splits
        private static MelonPreferences_Category category = MelonPreferences.CreateCategory("ITB LSME");
        private static MelonPreferences_Entry<bool> showStartSplits = category.CreateEntry<bool>("Start Splits", false);
        private static MelonPreferences_Entry<bool> showPauseSplits = category.CreateEntry<bool>("Pause Splits", false);
        private static MelonPreferences_Entry<bool> showEndSplits = category.CreateEntry<bool>("End Splits", false);
        private static MelonPreferences_Entry<bool> showCommonSplits = category.CreateEntry<bool>("Common Splits", false);
        private static MelonPreferences_Entry<bool> showDarkroomsSplits = category.CreateEntry<bool>("Darkrooms Splits", false);
        private static MelonPreferences_Entry<bool> showGarageSplits = category.CreateEntry<bool>("Garege Splits", false);
        private static MelonPreferences_Entry<bool> showOfficeSplits = category.CreateEntry<bool>("Office Splits", false);
        private static MelonPreferences_Entry<bool> showSewersSplits = category.CreateEntry<bool>("Sewers Splits", false);
        private static MelonPreferences_Entry<bool> showHotelSplits = category.CreateEntry<bool>("Hotel Splits", false);
        private static MelonPreferences_Entry<bool> showGrassroomsSplits = category.CreateEntry<bool>("Grassrooms Splits", false);
        #endregion

        #region Control Splits
        //Control Splits
        private static MelonPreferences_Entry<bool> ladderStart = category.CreateEntry<bool>("ladderStart", false);
        private static MelonPreferences_Entry<bool> hotelStart = category.CreateEntry<bool>("hotelStart", false);
        private static MelonPreferences_Entry<bool> mainMenuSplitsReset = category.CreateEntry<bool>("mainMenuResetSplits", false);
        #endregion

        #region Pause Splits
        //Pause Splits
        private static MelonPreferences_Entry<bool> pauseSplitsExitZone = category.CreateEntry<bool>("pauseExitZone", false);
        private static MelonPreferences_Entry<bool> pauseSplitsInteractable = category.CreateEntry<bool>("pauseInteractableEnding", false);
        #endregion

        #region End Splits
        //End Splits
        private static MelonPreferences_Entry<bool> endSplitsExitZone = category.CreateEntry<bool>("exitZone", false);
        private static MelonPreferences_Entry<bool> endSplitsInteractable = category.CreateEntry<bool>("interactableEnding", false);
        #endregion

        #region Common Splits
        private static MelonPreferences_Entry<bool> commonSplitsEscaped = category.CreateEntry<bool>("commonEscaped", false);
        private static MelonPreferences_Entry<bool> commonSplitsChase = category.CreateEntry<bool>("commonChase", false);
        private static MelonPreferences_Entry<bool> commonSplitsFun = category.CreateEntry<bool>("commonFun", false);
        private static MelonPreferences_Entry<bool> commonSplitsSewers = category.CreateEntry<bool>("commonSewers", false);
        private static MelonPreferences_Entry<bool> commonSplitsHotel = category.CreateEntry<bool>("commonHotel", false);
        private static MelonPreferences_Entry<bool> commonSplitsHotelLevel = category.CreateEntry<bool>("commonHotelLevel", false);
        private static MelonPreferences_Entry<bool> commonSplitsGrassrooms = category.CreateEntry<bool>("commonGrassrooms", false);
        private static MelonPreferences_Entry<bool> commonSplitsAllEndings = category.CreateEntry<bool>("commonAllEndings", false);
        #endregion

        #region Darkrooms Splits
        //Darkrooms Splits
        private static MelonPreferences_Entry<bool> darkroomsSplitsLobbyElevator = category.CreateEntry<bool>("LobbyElevator", false);
        private static MelonPreferences_Entry<bool> darkroomsSplitsVHSPutIn = category.CreateEntry<bool>("VHS", false);
        private static MelonPreferences_Entry<bool> darkroomsSplitsBodyLocker = category.CreateEntry<bool>("BodyLocker", false);
        private static MelonPreferences_Entry<bool> darkroomsHorsePuzzle = category.CreateEntry<bool>("Horse", false);
        private static MelonPreferences_Entry<bool> darkroomsSplitsClock = category.CreateEntry<bool>("Clock", false);
        private static MelonPreferences_Entry<bool> darkroomsSplitsPliers = category.CreateEntry<bool>("Pliers", false);
        private static MelonPreferences_Entry<bool> darkroomsSplitsMirror = category.CreateEntry<bool>("Mirror", false);
        private static MelonPreferences_Entry<bool> darkroomsSplitsLevers = category.CreateEntry<bool>("Levers", false);
        private static MelonPreferences_Entry<bool> darkroomsSplitsRadiation = category.CreateEntry<bool>("RadiationDoor", false);
        private static MelonPreferences_Entry<bool> darkroomsSplitsChainCut = category.CreateEntry<bool>("ChainCut", false);
        #endregion

        #region Garage Splits
        //Garage Splits
        private static MelonPreferences_Entry<bool> garageSplitsValvesDone = category.CreateEntry<bool>("ValvesDone", false);
        private static MelonPreferences_Entry<bool> garageSplitsGarageElevator = category.CreateEntry<bool>("GarageElevator", false);
        #endregion

        #region Office Splits
        //Office Splits
        private static MelonPreferences_Entry<bool> officeSplitsFusesDone = category.CreateEntry<bool>("FusesDone", false);
        private static MelonPreferences_Entry<bool> officeSplitsPartyStart = category.CreateEntry<bool>("PartyStart", false);
        private static MelonPreferences_Entry<bool> officeSplitsBalloons = category.CreateEntry<bool>("Balloons", false);
        private static MelonPreferences_Entry<bool> officeSplitsPresents = category.CreateEntry<bool>("Presents", false);
        private static MelonPreferences_Entry<bool> officeSplitsCakeExplode = category.CreateEntry<bool>("CakeExplode", false);
        private static MelonPreferences_Entry<bool> officeSplitsBlueDoor = category.CreateEntry<bool>("BlueDoor", false);
        private static MelonPreferences_Entry<bool> officeSplitsRedDoor = category.CreateEntry<bool>("RedDoor", false);
        private static MelonPreferences_Entry<bool> officeSplitsSecurityGrid = category.CreateEntry<bool>("SecurityGrid", false);
        #endregion

        #region Sewer Splits
        //Sewers Splits
        private static MelonPreferences_Entry<bool> sewersSplitsMetalDetector = category.CreateEntry<bool>("MetalDetector", false);
        private static MelonPreferences_Entry<bool> sewersSplitsSpikesOff = category.CreateEntry<bool>("Spikes Off", false);
        private static MelonPreferences_Entry<bool> sewersSplitsLabyrinthChainCut = category.CreateEntry<bool>("Labyrinth Chain Cut", false);
        private static MelonPreferences_Entry<bool> sewersSplitsCrusherPuzzle = category.CreateEntry<bool>("Crusher Puzzle", false);
        private static MelonPreferences_Entry<bool> sewersSplitsGearSpawn = category.CreateEntry<bool>("Gear Spawn", false);
        private static MelonPreferences_Entry<bool> sewersSplitsCounterweights = category.CreateEntry<bool>("Counterweights", false);
        #endregion

        #region Hotel Splits
        //Hotel Splits
        private static MelonPreferences_Entry<bool> hotelSplitsPaintings = category.CreateEntry<bool>("Paintings", false); // Test OnSolve
        private static MelonPreferences_Entry<bool> hotelSplitsStatue = category.CreateEntry<bool>("Statue", false); // Works Multi
        private static MelonPreferences_Entry<bool> hotelSplitsVinyl = category.CreateEntry<bool>("Vinyl", false); // Works Multi
        private static MelonPreferences_Entry<bool> hotelSplitsPiano = category.CreateEntry<bool>("Piano", false); // Works Multi
        private static MelonPreferences_Entry<bool> hotelSplitsPhone = category.CreateEntry<bool>("Phone", false); // Solo Only
        private static MelonPreferences_Entry<bool> hotelSplitsGemIn = category.CreateEntry<bool>("GemIn", false); // Placement Player
        private static MelonPreferences_Entry<bool> hotelSplitsBoilerKeys = category.CreateEntry<bool>("BoilerKeys", false); // Closest Player
        private static MelonPreferences_Entry<bool> hotelSplitsBoilersOn = category.CreateEntry<bool>("BoilersOn", false); // Test SmokeOn
        private static MelonPreferences_Entry<bool> hotelSplitsBathroomLock = category.CreateEntry<bool>("BathroomLock", false); // Works Multi
        private static MelonPreferences_Entry<bool> hotelSplitsPlaceCocoon = category.CreateEntry<bool>("PlaceCocoon", false); // Works Multi
        #endregion

        #region Grassrooms Splits
        private static MelonPreferences_Entry<bool> grassroomsStartDoor = category.CreateEntry<bool>("Start Door", false);
        private static MelonPreferences_Entry<bool> grassroomsStorage = category.CreateEntry<bool>("Storage", false);
        private static MelonPreferences_Entry<bool> grassroomsCase = category.CreateEntry<bool>("Industrial Scissors", false);
        private static MelonPreferences_Entry<bool> grassroomsLaptop = category.CreateEntry<bool>("Laptop", false);
        private static MelonPreferences_Entry<bool> grassroomsEnergyPuzzle = category.CreateEntry<bool>("Energy Puzzle", false);
        private static MelonPreferences_Entry<bool> grassroomsCathedral = category.CreateEntry<bool>("Cathedral", false);
        private static MelonPreferences_Entry<bool> grassroomsLibrary = category.CreateEntry<bool>("Library", false);
        
        #endregion

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

            #region MelonPref Setup
            
            #endregion
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName == "MainMenu")
            {
                inGame = false;
                elevator = false;
                garageElevator = false;
                MelonEvents.OnGUI.Subscribe(DrawRegisteredMods, 100);
                MelonEvents.OnGUI.Subscribe(DrawRunSelector, 100);
                MelonEvents.OnGUI.Subscribe(DrawVersion, 100);

                if (mainMenuSplitsReset.Value)
                {
                    if (lsm == null)
                    {
                        MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                        return;
                    }
                    lsm.ResetTimer();
                }
            }
            if (sceneName == "MainLevel" || sceneName == "HOTEL_SCENE" || sceneName == "GRASS_ROOMS_SCENE")
            {
                inGame = true;
                MelonEvents.OnGUI.Unsubscribe(DrawRegisteredMods);
                MelonEvents.OnGUI.Unsubscribe(DrawRunSelector);
                MelonEvents.OnGUI.Unsubscribe(DrawVersion);
            }
            if (sceneName == "HOTEL_SCENE")
            {
                StartTimer("Hotel Start", ref hotelStart);
                if (lsm == null)
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                    return;
                }
                lsm.ResumeTimer();
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

        private void DrawVersion()
        {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.UpperRight;
            style.normal.textColor = Color.white;

            if (version == "")
            {
                version = GameObject.Find("VersionText").GetComponent<UnityEngine.UI.Text>().m_Text;
            }

            GUI.Label(new Rect(Screen.width - 500 - 10, 85, 500, 15), version, style);
        }

        public void CreateButton(string label, ref MelonPreferences_Entry<bool> boolVariable, float offsety)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

            if (boolVariable.Value)
            {
                buttonStyle.normal.background = Texture2D.whiteTexture;
                buttonStyle.normal.textColor = Color.black;
            }
            else
            {
                buttonStyle.normal.background = GUI.skin.button.normal.background;
                buttonStyle.normal.textColor = GUI.skin.button.normal.textColor;
            }

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + offsety + 250f, buttonWidth, buttonHeight), label, buttonStyle))
            {
                boolVariable.Value = !boolVariable.Value;
                MelonLogger.Msg(System.ConsoleColor.Green, label + " Button Down");
                MelonLogger.Msg(System.ConsoleColor.Green, label + " bool changed to " + boolVariable.Value);
            }
        }

        public static void SplitTimer(string splitName, ref MelonPreferences_Entry<bool> boolVariable)
        {
            if (boolVariable.Value)
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Testing " + splitName);
                if (lsm == null)
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                    return;
                }
                lsm.SplitTimer();
            }
        }

        public static void StartTimer(string splitName, ref MelonPreferences_Entry<bool> boolVariable)
        {
            if (boolVariable.Value)
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Testing Start " + splitName);
                if (lsm == null)
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                    return;
                }
                lsm.StartTimer();
            }
        }

        public static async Task LadderResume(string splitName)
        {
            if (ladderStart.Value)
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Testing Resume " + splitName);
                if (lsm == null)
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "LiveSplitClient not created! How did you get here?");
                    return;
                }
                await Task.Delay(TimeSpan.FromSeconds(3.91));
                lsm.ResumeTimer();
            }
        }

        public void SetAllButtonsFalse()
        {
            showStartSplits.Value = false;
            showPauseSplits.Value = false;
            showEndSplits.Value = false;
            showCommonSplits.Value = false;
            showDarkroomsSplits.Value = false;
            showGarageSplits.Value = false;
            showOfficeSplits.Value = false;
            showSewersSplits.Value = false;
            showHotelSplits.Value = false;
            showGrassroomsSplits.Value = false;
        }

        public void SetAllSplitValuesFalse()
        {

        }

        public void DrawRunSelector()
        {
            GUI.Box(runSelectorRect, "Autosplit Customizer");

            #region Control Splits

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 20f, buttonWidth, buttonHeight), "Splits that Start/Reset"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Start Splits Button Down");
                if (showStartSplits.Value)
                {
                    SetAllButtonsFalse();
                }
                else
                {
                    SetAllButtonsFalse();
                    showStartSplits.Value = true;
                }
            }

            if (showStartSplits.Value)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 375f, 270f, buttonHeight * 3 + 5 * 4), "");
                CreateButton("Ladder Start", ref ladderStart, 130f);
                CreateButton("Hotel Start", ref hotelStart, 165f);
                CreateButton("Main Menu Reset Timer", ref mainMenuSplitsReset, 200f);
            }
            #endregion

            #region Pause Splits

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 55f, buttonWidth, buttonHeight), "Splits that Pause"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Pause Splits Button Down");
                if (showPauseSplits.Value)
                {
                    SetAllButtonsFalse();
                }
                else
                {
                    SetAllButtonsFalse();
                    showPauseSplits.Value = true;
                }
            }

            if (showPauseSplits.Value)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 375f, 270f, buttonHeight * 2 + 5 * 3), "");
                CreateButton("Escape Chase Pause", ref pauseSplitsExitZone, 130f);
                CreateButton("Fun Sewers Hotel Pause", ref pauseSplitsInteractable, 165f);

            }
            #endregion

            #region End Splits

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 90f, buttonWidth, buttonHeight), "Ending Splits"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "End Splits Button Down");
                if (showEndSplits.Value)
                {
                    SetAllButtonsFalse();
                }
                else
                {
                    SetAllButtonsFalse();
                    showEndSplits.Value = true;
                }
            }

            if (showEndSplits.Value)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 375f, 270f, buttonHeight * 2 + 5 * 3), "");
                CreateButton("Escape Chase End", ref endSplitsExitZone, 130f);
                CreateButton("Fun Sewers Hotel End", ref endSplitsInteractable, 165f);
            }
            #endregion

            #region Common Splits

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 125f, buttonWidth, buttonHeight), "Common Split Setups"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Common Splits Button Down");
                if (showCommonSplits.Value)
                {
                    SetAllButtonsFalse();
                }
                else
                {
                    SetAllButtonsFalse();
                    showCommonSplits.Value = true;
                }
            }

            if (showCommonSplits.Value)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 375f, 270f, buttonHeight * 8 + 5 * 9), "");
                CreateButton("Escaped Ending", ref commonSplitsEscaped, 130f);
                CreateButton("Chase Ending", ref commonSplitsChase, 165f);
                CreateButton("Fun Ending", ref commonSplitsFun, 200f);
                CreateButton("Sewers Ending", ref commonSplitsSewers, 235f);
                CreateButton("Hotel Ending", ref commonSplitsHotel, 270f);
                CreateButton("Grassrooms Level", ref commonSplitsGrassrooms, 305f);
                CreateButton("Hotel Level", ref commonSplitsHotelLevel, 340f);
                CreateButton("All Endings", ref commonSplitsAllEndings, 375f);
                /*
                CreateButton("Balloons Death", ref deathSplitsBalloons, 375f);
                CreateButton("Electricity Death", ref deathSplitsElectricity, 410f);
                CreateButton("Spider Death", ref deathSplitsSpider, 445f);
                CreateButton("Rat Death", ref deathSplitsRat, 480f);
                CreateButton("Spikes Death", ref deathSplitsSpikes, 515f);
                CreateButton("Crusher Death", ref deathSplitsCrusher, 550f);
                CreateButton("Moth Death", ref deathSplitsMoth, 585f);
                CreateButton("Receptionist Death", ref deathSplitsReceptionist, 620f);
                */
            }

            #endregion

            #region Darkrooms Splits

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 160f, buttonWidth, buttonHeight), "Darkrooms Splits"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Darkrooms Splits Button Down");
                if (showDarkroomsSplits.Value)
                {
                    SetAllButtonsFalse();
                }
                else
                {
                    SetAllButtonsFalse();
                    showDarkroomsSplits.Value = true;
                }
            }

            if (showDarkroomsSplits.Value)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 375f, 270f, buttonHeight * 10 + 5 * 11), "");
                CreateButton("Lobby Elevator", ref darkroomsSplitsLobbyElevator, 130f);
                CreateButton("VHS", ref darkroomsSplitsVHSPutIn, 165f);
                CreateButton("Body Locker", ref darkroomsSplitsBodyLocker, 200f);
                CreateButton("Horse Puzzle", ref darkroomsHorsePuzzle, 235f);
                CreateButton("Clock", ref darkroomsSplitsClock, 270f);
                CreateButton("Pliers", ref darkroomsSplitsPliers, 305f);
                CreateButton("Mirror", ref darkroomsSplitsMirror, 340f);
                CreateButton("Levers", ref darkroomsSplitsLevers, 375f);
                CreateButton("Radiation", ref darkroomsSplitsRadiation, 410f);
                CreateButton("Chain Cut", ref darkroomsSplitsChainCut, 445f);
            }

            #endregion

            #region Garage Splits

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 195f, buttonWidth, buttonHeight), "Garage Splits"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Garage Splits Button Down");
                if (showGarageSplits.Value)
                {
                    SetAllButtonsFalse();
                }
                else
                {
                    SetAllButtonsFalse();
                    showGarageSplits.Value = true;
                }
            }

            if (showGarageSplits.Value)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 375f, 270f, buttonHeight * 2 + 5 * 3), "");
                CreateButton("Valves", ref garageSplitsValvesDone, 130f);
                CreateButton("Office Elevator", ref garageSplitsGarageElevator, 165f);
            }

            #endregion

            #region Office Splits

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 230f, buttonWidth, buttonHeight), "Office Splits"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Office Splits Button Down");
                if (showOfficeSplits.Value)
                {
                    SetAllButtonsFalse();
                }
                else
                {
                    SetAllButtonsFalse();
                    showOfficeSplits.Value = true;
                }
            }

            if (showOfficeSplits.Value)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 375f, 270f, buttonHeight * 8 + 5 * 9), "");
                CreateButton("Fuses", ref officeSplitsFusesDone, 130f);
                CreateButton("Party Start", ref officeSplitsPartyStart, 165f);
                CreateButton("Balloons Finish", ref officeSplitsBalloons, 200f);
                CreateButton("Presents Finish", ref officeSplitsPresents, 235f);
                CreateButton("Cake Explode", ref officeSplitsCakeExplode, 270f);
                CreateButton("Blue Door", ref officeSplitsBlueDoor, 305f);
                CreateButton("Red Door", ref officeSplitsRedDoor, 340f);
                CreateButton("Security Grid", ref officeSplitsSecurityGrid, 375f);
            }

            #endregion

            #region Sewers Splits

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 265f, buttonWidth, buttonHeight), "Sewers Splits"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Sewers Splits Button Down");
                if (showSewersSplits.Value)
                {
                    SetAllButtonsFalse();
                }
                else
                {
                    SetAllButtonsFalse();
                    showSewersSplits.Value = true;
                }
            }

            if (showSewersSplits.Value)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 375f, 270f, buttonHeight * 6 + 5 * 7), "");
                CreateButton("Metal Detector", ref sewersSplitsMetalDetector, 130f);
                CreateButton("Spikes Off", ref sewersSplitsSpikesOff, 165f);
                CreateButton("Labyrinth Chain", ref sewersSplitsLabyrinthChainCut, 200f);
                CreateButton("Crusher Puzzle", ref sewersSplitsCrusherPuzzle, 235f);
                CreateButton("Medallions Puzzle", ref sewersSplitsGearSpawn, 270f);
                CreateButton("Counterweights", ref sewersSplitsCounterweights, 305f);
            }

            #endregion

            #region Hotel Splits

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 300f, buttonWidth, buttonHeight), "Hotel Splits"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Hotel Splits Button Down");
                if (showHotelSplits.Value)
                {
                    SetAllButtonsFalse();
                }
                else
                {
                    SetAllButtonsFalse();
                    showHotelSplits.Value = true;
                }
            }

            if (showHotelSplits.Value)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 375f, 270f, buttonHeight * 10 + 5 * 11), "");
                CreateButton("Paintings", ref hotelSplitsPaintings, 130f);
                CreateButton("Statue", ref hotelSplitsStatue, 165f);
                CreateButton("Vinyl", ref hotelSplitsVinyl, 200f);
                CreateButton("Piano", ref hotelSplitsPiano, 235f);
                CreateButton("Phone", ref hotelSplitsPhone, 270f);
                CreateButton("Gem Placed", ref hotelSplitsGemIn, 305f);
                CreateButton("Mirror Puzzle", ref hotelSplitsBoilerKeys, 340f);
                CreateButton("Boilers On", ref hotelSplitsBoilersOn, 375f);
                CreateButton("Bathroom Lock", ref hotelSplitsBathroomLock, 410f);
                CreateButton("Place Cocoon", ref hotelSplitsPlaceCocoon, 445f);
            }

            #endregion

            #region Grassrooms Splits
            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 335f, buttonWidth, buttonHeight), "Grassrooms Splits"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Grassrooms Splits Button Down");
                if (showGrassroomsSplits.Value)
                {
                    SetAllButtonsFalse();
                }
                else
                {
                    SetAllButtonsFalse();
                    showGrassroomsSplits.Value = true;
                }
            }

            if (showGrassroomsSplits.Value)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 375f, 270f, buttonHeight * 7 + 5 * 8), "");
                CreateButton("Start Door", ref grassroomsStartDoor, 130f);
                CreateButton("Storage", ref grassroomsStorage, 165f);
                CreateButton("Industrial Scissors", ref grassroomsCase, 200f);
                CreateButton("Laptop", ref grassroomsLaptop, 235f);
                CreateButton("Energy Puzzle", ref grassroomsEnergyPuzzle, 270f);
                CreateButton("Cathedral", ref grassroomsCathedral, 305f);
                CreateButton("Library", ref grassroomsLibrary, 340f);
            }
            #endregion
        }


        //TODO Patches:
        //      ValvesDone
        //      GarageElevator
        //      FusesDone
        //      Office Balloons
        //      Office Presents
        //      Office BlueDoor
        //      Office RedDoor

        //Need Testing Patches:
        //
        //Multiplayer:
        //  VHS
        //  BodyLocker
        //  HorsePuzzle
        //  ClockSolve
        //  LeversUnlock
        //  RadiationDone

        //Broken Patches:
        //


        //Start of Patches
        #region All Patches

        //Start Timer when using ladder in lobby
        [HarmonyPatch(typeof(BasePlayerController), "UseStairs")]
        class BasePlayerControllerPatch
        {
            [HarmonyPrefix]
            internal static void UseStairsPrefix()
            {
                StartTimer("Ladder", ref ladderStart);
                LadderResume("Ladder Resume");
            }
        }

        //Split Timer when using the elevator
        [HarmonyPatch(typeof(Elevator), "RpcDoorElevatorPlay")]
        class ElevatorPatch
        {
            [HarmonyPrefix]
            internal static void RpcDoorElevatorPlayPrefix(Elevator __instance)
            {
                if (__instance.name == "Elevator" && elevator == false)
                {
                    SplitTimer("Elevator", ref darkroomsSplitsLobbyElevator);
                    elevator = true;
                }
                if (__instance.name == "Elevator (1)" && garageElevator == false)
                {
                    SplitTimer("Garage Complete", ref garageSplitsGarageElevator);
                    garageElevator = true;
                }
            }
        }

        //Split Timer when adding VHS
        [HarmonyPatch(typeof(CassetePlayer), "CmdInsertCassete")]
        class CmdInsertCassetePatch
        {
            [HarmonyPrefix]
            internal static void CmdInsertCassetePrefix()
            {
                SplitTimer("VHS", ref darkroomsSplitsVHSPutIn);
            }
        }


        [HarmonyPatch(typeof(Door), "UnlockDoor")]
        class CmdUnlockDoor
        {
            [HarmonyPrefix]
            internal static void CmdUnlockDoorPrefix(Door __instance)
            {
                if (__instance.name == "LockerDoor")
                {
                    SplitTimer("Body Locker", ref darkroomsSplitsBodyLocker);
                }
            }
        }


        //Split Timer when solving shadow puzzle
        [HarmonyPatch(typeof(ShadowPuzzle), "OnPuzzleSolve")]
        class SolvePuzzlePatch
        {
            [HarmonyPrefix]
            internal static void SolvePuzzlePrefix()
            {
                SplitTimer("Shadow Puzzle", ref darkroomsHorsePuzzle);
            }
        }


        //Split Timer when solving clock puzzle
        [HarmonyPatch(typeof(ClockPuzzle), "OnResolvePuzzle")]
        class OnResolvePuzzlePatch
        {
            [HarmonyPrefix]
            internal static void OnResolvePuzzlePrefix()
            {
                SplitTimer("Clock", ref darkroomsSplitsClock);
            }
        }

        //To add fuses box here? ambiguous routing really to go bodies or red room
        //Split Timer when the Mirror breaks
        [HarmonyPatch(typeof(LostPersonsPuzzle), "DestroyGlass")]
        class LostPersonsPuzzlePatch
        {
            [HarmonyPrefix]
            internal static void DestroyGlassPrefix()
            {
                SplitTimer("Mirror Break", ref darkroomsSplitsMirror);
            }
        }

        //Split Timer when Levers done
        [HarmonyPatch(typeof(ColorLeverDoorLock), "OnUnlock")]
        class ColorLeverDoorLockPatch
        {
            [HarmonyPrefix]
            internal static void OnUnlockPrefix()
            {
                SplitTimer("Levers", ref darkroomsSplitsLevers);
            }
        }

        //Split Timer when rad door unlocks
        [HarmonyPatch(typeof(RadiationPuzzle), "OnZoneUnlocked")]
        class RadiationPuzzlePatch
        {
            [HarmonyPrefix]
            internal static void OnZoneUnlockedPrefix()
            {
                SplitTimer("Radiation Unlock", ref darkroomsSplitsRadiation);
            }
        }

        //Split Timer when the chain is cut after radiation
        [HarmonyPatch(typeof(DoorChain), "OnChainCut")]
        class DoorChainPatch
        {
            [HarmonyPrefix]
            internal static void OnChainCutPrefix(DoorChain __instance)
            {
                if (__instance.name == "CHAINS_OFFICEDOOR")
                {
                    SplitTimer("Rad Chain Cut", ref darkroomsSplitsChainCut);
                }
                if (__instance.name == "Chains (1)")
                {
                    SplitTimer("Labyrinth Chain Cut", ref sewersSplitsLabyrinthChainCut);
                }
            }
        }

        //Split Timer when the Party Room Starts
        [HarmonyPatch(typeof(PartygoerRoom), "StartPartyGames")]
        class PartygoerRoomPatch
        {
            [HarmonyPrefix]
            internal static void StartPartyGamesPrefix()
            {
                SplitTimer("Party Room Start", ref officeSplitsPartyStart);
            }
        }

        //Split Timer when the Party Room Starts
        [HarmonyPatch(typeof(PartygoerCake), "Explode")]
        class PartygoerCakePatch
        {
            [HarmonyPrefix]
            internal static void ExplodePrefix()
            {
                SplitTimer("Cake Explode", ref officeSplitsCakeExplode);
            }
        }

        //Split Timer when the Security Door
        [HarmonyPatch(typeof(SecretGridComputerPuzzle), "SetGateOpenStatus")]
        class SecretGridComputerPuzzlePatch
        {
            [HarmonyPrefix]
            internal static void SetGateOpenStatusPrefix()
            {
                SplitTimer("Security Grid", ref officeSplitsSecurityGrid);
            }
        }

        //Split Timer when the Spikes off
        [HarmonyPatch(typeof(PikesDisabler), "DisablePikes")]
        class PikesDisablerPatch
        {
            [HarmonyPrefix]
            internal static void DisablePikesPrefix()
            {
                SplitTimer("Disable Spikes", ref sewersSplitsSpikesOff);
            }
        }

        //Split Timer when the Garbage Crusher off
        [HarmonyPatch(typeof(GridPuzzle), "OnGridSolved")]
        class GridPuzzlePatch
        {
            [HarmonyPrefix]
            internal static void OnGridSolvedPrefix()
            {
                SplitTimer("Crusher Puzzle", ref sewersSplitsCrusherPuzzle);
            }
        }

        //Split Timer when the Medallion Puzzle
        [HarmonyPatch(typeof(MedallionsStatue), "OnUnlocked")]
        class MedallionsStatuePatch
        {
            [HarmonyPrefix]
            internal static void OnUnlockedPrefix()
            {
                SplitTimer("Medallion Puzzle", ref sewersSplitsGearSpawn);
            }
        }

        //Split Timer when the Gear Rotation
        [HarmonyPatch(typeof(GearPuzzle), "OnPuzzleSolve")]
        class GearPuzzlePatch
        {
            [HarmonyPrefix]
            internal static void OnPuzzleSolvePrefix()
            {
                SplitTimer("Gear Rotation Puzzle", ref sewersSplitsCounterweights);
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
                    SplitTimer("Escape/Chase/Grassrooms Ending", ref endSplitsExitZone);
                }
            }
        }

        //Split Timer when the Fun/Sewer/Hotel Ending 
        [HarmonyPatch(typeof(EndInteractable), "Interact")]
        class EndInteractablePatch
        {
            [HarmonyPrefix]
            internal static void InteractPrefix()
            {
                SplitTimer("Fun/Sewer/Hotel Ending", ref endSplitsInteractable);
            }
        }

        //TERRORHOTEL
        //Split Timer when Art Room Complete
        [HarmonyPatch(typeof(ArtRoomPuzzle), "OnSolve")] // Check
        public class ArtOnSolvePatch
        {
            [HarmonyPrefix]
            internal static void ArtOnSolvePrefix()
            {
                SplitTimer("Art Room Puzzzle", ref hotelSplitsPaintings);
            }
        }

        //Split Timer when Locked Statue Face Placed
        [HarmonyPatch(typeof(LockedStatue), "OnUnlock")] // Works Multi
        public class LockedStatuePatch
        {
            [HarmonyPrefix]
            internal static void LockedStatuePrefix()
            {
                SplitTimer("Statue", ref hotelSplitsStatue);
            }
        }

        //Split Timer when Vinyl Added
        [HarmonyPatch(typeof(Gramophone), "OnVynilAdd")] // Works Multi
        public class OnVynilAddPatch
        {
            [HarmonyPrefix]
            internal static void OnVynilAddPrefix()
            {
                SplitTimer("Vinyl", ref hotelSplitsVinyl);
            }
        }

        //Split Timer when Piano Solved
        [HarmonyPatch(typeof(Piano), "OnSolve")] // Works Multi
        public class PianoOnSolvePatch
        {
            [HarmonyPrefix]
            internal static void OnSolvePrefix()
            {
                SplitTimer("Piano", ref hotelSplitsPiano);
            }
        }

        //Split Timer when answering Phone
        [HarmonyPatch(typeof(OldPhone), "CmdAnswerPhone")] // Works only for person answering
        public class CmdAnswerPhonePatch
        {
            [HarmonyPrefix]
            internal static void CmdAnswerPhonePrefix()
            {
                SplitTimer("CmdAnswerPhone", ref hotelSplitsPhone);
            }
        }

        //Split Timer when adding gem to ray puzzle
        [HarmonyPatch(typeof(ReflectLightRay), "OnGemAdd")] // Works only for person putting in
        public class OnAddGemPatch
        {
            [HarmonyPrefix]
            internal static void OnAddGemPrefix()
            {
                SplitTimer("OnGemAdd", ref hotelSplitsGemIn);
            }
        }

        //Split Timer when ray puzzle complete
        [HarmonyPatch(typeof(ReflectLightRay), "OnSolve")] // Works only for closest person?
        public class ReflectLightRayOnSolvePatch
        {
            [HarmonyPrefix]
            internal static void ReflectLightRayOnSolvePrefix()
            {
                if (!lightray)
                {
                    SplitTimer("Light Puzzle", ref hotelSplitsBoilerKeys);
                }
            }
        }

        //Split Timer when successful boiler lever pull
        [HarmonyPatch(typeof(BoilersInterruptor), "RpcTurnOn")] // Check multi - works solo
        public class RpcTurnOnPatch
        {
            [HarmonyPrefix]
            internal static void RpcTurnOnPrefix()
            {
                SplitTimer("Boilers Active", ref hotelSplitsBoilersOn);
            }
        }

        //Split Timer when smoke turns on in moth room // Will split when code wrong, not worth using
        [HarmonyPatch(typeof(PitfallsPuzzle), "OnSmokeOn")] // check - works solo
        public class OnSmokeOnPatch
        {
            [HarmonyPrefix]
            internal static void OnSmokeOnPrefix()
            {
                SplitTimer("Boilers Active // Pitfalls - OnSmokeOn", ref hotelSplitsBoilersOn);
            }
        }

        //Pliers, Metal Detector and Bathroom Locks Split
        [HarmonyPatch(typeof(NumericLock), "OnUnlock")] // Works multi
        public class NumericLockPatch
        {
            [HarmonyPrefix]
            internal static void OnUnlockPrefix(NumericLock __instance)
            {
                if (__instance.transform.parent.parent.name == "__BACKROOMS (LEVEL2)")
                {
                    SplitTimer("Pliers Lock", ref darkroomsSplitsPliers);
                }
                if (__instance.transform.parent.parent.name == "__BACKROOMS SEWERAGE (LEVEL 4)")
                {
                    SplitTimer("Metal Detector Lock", ref sewersSplitsMetalDetector);
                }
                if (__instance.transform.parent.parent.name == "--------SCENE------------")
                {
                    SplitTimer("Bathroom Lock", ref hotelSplitsBathroomLock);
                }
            }
        }

        //Split timer when Cocoon added to table
        [HarmonyPatch(typeof(RecepcionistFeedPuzzle), "OnAddCocoon")] // Works multi
        public class OnAddCocoonPatch
        {
            [HarmonyPrefix]
            internal static void OnAddCocoonPrefix()
            {
                SplitTimer("Cocoon Add", ref hotelSplitsPlaceCocoon);
            }
        }
        #endregion
    }
}