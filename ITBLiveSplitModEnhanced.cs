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
        private static bool lightray = false;

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
        //Reset Split
        private static bool mainMenuSplitsReset = true;

        #region Start Splits
        //Start Splits
        private static bool ladderStart = true; // true by default since its normal
        private static bool hotelStart = true;
        #endregion

        #region Pause Splits
        //Pause Splits
        private static bool pauseSplitsEscapeChase = false;
        private static bool pauseSplitsFunSewersHotel = false;
        #endregion

        #region End Splits
        //End Splits
        private static bool endSplitsEscapeChase = false;
        private static bool endSplitsFunSewersHotel = false;
        #endregion

        #region Death Splits
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
        #endregion

        #region Darkrooms Splits
        //Darkrooms Splits
        private static bool darkroomsSplitsLobbyElevator = false;
        private static bool darkroomsSplitsVHSPutIn = false;
        private static bool darkroomsSplitsHammer = false;
        private static bool darkroomsHorsePuzzle = false;
        private static bool darkroomsSplitsClock = false;
        private static bool darkroomsSplitsPliers = false;
        private static bool darkroomsSplitsMirror = false;
        private static bool darkroomsSplitsLevers = false;
        private static bool darkroomsSplitsRadiation = false;
        private static bool darkroomsSplitsChainCut = false;
        #endregion

        #region Garage Splits
        //Garage Splits
        private static bool garageSplitsValvesDone = false;
        private static bool garageSplitsGarageElevator = false;
        #endregion

        #region Office Splits
        //Office Splits
        private static bool officeSplitsFusesDone = false;
        private static bool officeSplitsPartyStart = false;
        private static bool officeSplitsBalloons = false;
        private static bool officeSplitsPresents = false;
        private static bool officeSplitsCakeExplode = false;
        private static bool officeSplitsBlueDoor = false;
        private static bool officeSplitsRedDoor = false;
        private static bool officeSplitsSecurityGrid = false;
        #endregion

        #region Sewer Splits
        //Sewers Splits
        private static bool sewersSplitsMetalDetector = false;
        private static bool sewersSplitsSpikesOff = false;
        private static bool sewersSplitsLabyrinthChainCut = false;
        private static bool sewersSplitsCrusherPuzzle = false;
        private static bool sewersSplitsGearSpawn = false;
        private static bool sewersSplitsCounterweights = false;
        #endregion
        
        #region Hotel Splits
        //Hotel Splits
        private static bool hotelSplitsPaintings = false; // Test OnSolve
        private static bool hotelSplitsStatue = false; // Works Multi
        private static bool hotelSplitsVinyl = false; // Works Multi
        private static bool hotelSplitsPiano = false; // Works Multi
        private static bool hotelSplitsPhone = false; // Solo Only
        private static bool hotelSplitsGemIn = false; // Placement Player
        private static bool hotelSplitsBoilerKeys = false; // Closest Player
        private static bool hotelSplitsBoilersOn = false; // Test SmokeOn
        private static bool hotelSplitsBathroomLock = false; // Works Multi
        private static bool hotelSplitsPlaceCocoon = false; // Works Multi
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

        public static void SplitTimer(string splitName, ref bool boolVariable)
        {
            if (boolVariable)
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

        public static void StartTimer(string splitName, ref bool boolVariable)
        {
            if (boolVariable)
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
            if (ladderStart)
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
            GUI.Box(runSelectorRect, "Autosplit Customizer");

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
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 350f, 270f, buttonHeight * 2 + 5 * 3), "");                
                CreateButton("Ladder Start", ref ladderStart, 130f);
                CreateButton("Hotel Start", ref hotelStart, 165f);
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
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 350f, 270f, buttonHeight * 2 + 5 * 3), "");                
                CreateButton("Escape Chase Pause", ref pauseSplitsEscapeChase, 130f);
                CreateButton("Fun Sewers Hotel Pause", ref pauseSplitsFunSewersHotel, 165f);

            }
            #endregion

            #region End Splits

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 90f, buttonWidth, buttonHeight), "Ending Splits"))
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
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 350f, 270f, buttonHeight * 2 + 5 * 3), "");                
                CreateButton("Escape Chase End", ref endSplitsEscapeChase, 130f);
                CreateButton("Fun Sewers Hotel End", ref endSplitsFunSewersHotel, 165f);
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
                CreateButton("Spikes Death", ref deathSplitsSpikes, 515f);
                CreateButton("Crusher Death", ref deathSplitsCrusher, 550f);
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
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 350f, 270f, buttonHeight * 10 + 5 * 11), "");                
                CreateButton("Lobby Elevator", ref darkroomsSplitsLobbyElevator, 130f);
                CreateButton("VHS", ref darkroomsSplitsVHSPutIn, 165f);
                CreateButton("Hammer Pickup", ref darkroomsSplitsHammer, 200f);
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
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 350f, 270f, buttonHeight * 2 + 5 * 3), "");                
                CreateButton("Valves", ref garageSplitsValvesDone, 130f);
                CreateButton("Office Elevator", ref garageSplitsGarageElevator, 165f);
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
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 350f, 270f, buttonHeight * 8 + 5 * 9), "");                
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
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 350f, 270f, buttonHeight * 6 + 5 * 7), "");               
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
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 350f, 270f, buttonHeight * 10 + 5 * 11), "");                
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
        }


        //Start of Patches
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

        #region Darkrooms Patches
        //Split Timer when using the elevator
        [HarmonyPatch(typeof(Elevator), "RpcDoorElevatorPlay")]
        class ElevatorPatch
        {
            [HarmonyPrefix]
            internal static void RpcDoorElevatorPlayPrefix()
            {
                if (!elevator)
                {
                    SplitTimer("Elevator", ref darkroomsSplitsLobbyElevator);
                    elevator = true;
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

        [HarmonyPatch(typeof(PlayerInventory), "TargetOnItemPickup")]
        class TargetOnItemPickupPatch
        {
            [HarmonyPrefix]
            internal static void TargetOnItemPickupPrefix(string item)
            {
                if (item == "Hammer" && !hammer)
                {
                    SplitTimer("Hammer", ref darkroomsSplitsHammer);
                    hammer = true;
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
        #endregion

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
                    SplitTimer("Escape/Chase Ending", ref endSplitsEscapeChase);
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
                SplitTimer("Fun/Sewer/Hotel Ending", ref endSplitsFunSewersHotel);
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

        //Split Timer when smoke turns on in moth room
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

        //PlayerController.PlayMonsterDeathAnimation(MonsterObject)
    }
}
/*
[09:08:36.888] Players in Lobby
[09:08:36.888] Player[ConnID:0] - Xanz
[09:08:58.280] [ITB_LSM_Enhanced] Testing Start Ladder
[09:08:58.280] [ITB_LSM_Enhanced] Attempting to send 'starttimer' command to LiveSplit Server
[09:08:58.281] [ITB_LSM_Enhanced] Succesfully sent 'starttimer' command to LiveSplit Server
[09:09:12.524] [ITB_LSM_Enhanced] Testing Elevator
[09:09:12.525] [ITB_LSM_Enhanced] Attempting to send 'split' command to LiveSplit Server
[09:09:12.526] [ITB_LSM_Enhanced] Succesfully sent 'split' command to LiveSplit Server
[09:10:02.098] [ITB_LSM_Enhanced] Testing VHS
[09:10:02.099] [ITB_LSM_Enhanced] Attempting to send 'split' command to LiveSplit Server
[09:10:02.100] [ITB_LSM_Enhanced] Succesfully sent 'split' command to LiveSplit Server
[09:10:25.661] [ITB_LSM_Enhanced] Testing Hammer
[09:10:25.661] [ITB_LSM_Enhanced] Attempting to send 'split' command to LiveSplit Server
[09:10:25.662] [ITB_LSM_Enhanced] Succesfully sent 'split' command to LiveSplit Server
Horse Puzzle is not here but I fixed it
[09:11:04.281] [ITB_LSM_Enhanced] Testing Clock
[09:11:04.281] [ITB_LSM_Enhanced] Attempting to send 'split' command to LiveSplit Server
[09:11:04.282] [ITB_LSM_Enhanced] Succesfully sent 'split' command to LiveSplit Server
[09:13:38.233] [ITB_LSM_Enhanced] Testing Mirror Break
[09:13:38.233] [ITB_LSM_Enhanced] Attempting to send 'split' command to LiveSplit Server
[09:13:38.234] [ITB_LSM_Enhanced] Succesfully sent 'split' command to LiveSplit Server
[09:15:10.056] [ITB_LSM_Enhanced] Testing Levers
[09:15:10.056] [ITB_LSM_Enhanced] Attempting to send 'split' command to LiveSplit Server
[09:15:10.058] [ITB_LSM_Enhanced] Succesfully sent 'split' command to LiveSplit Server
[09:17:09.193] [ITB_LSM_Enhanced] Testing Radiation Unlock
[09:17:09.193] [ITB_LSM_Enhanced] Attempting to send 'split' command to LiveSplit Server
[09:17:09.194] [ITB_LSM_Enhanced] Succesfully sent 'split' command to LiveSplit Server
Padlock and Chain not done because not active bools
*/