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
        private Rect runSelectorRect = new Rect(10f, 10f, 270f, 125f);
        private float buttonWidth = 260f;
        private float buttonHeight = 30f;
        private static bool showStartSplits = false;
        private static bool showPauseSplits = false;
        private static bool showEndSplits = false;

        //Setup Splits
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

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + offsety, buttonWidth, buttonHeight), label, buttonStyle))
            {
                boolVariable = !boolVariable;
                MelonLogger.Msg(System.ConsoleColor.Green, label + " Button Down");
            }
        }

        public void DrawRunSelector()
        {
            GUI.Box(runSelectorRect, "Splits Customizer");
           
            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 20f, buttonWidth, buttonHeight), "Splits that Start"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Start Splits Button Down");
                showStartSplits = !showStartSplits;
            }
            if (showStartSplits)
            {
                GUI.Box(new Rect(runSelectorRect.x, runSelectorRect.y + 125f, 270f, buttonHeight * 8 + 5 * 9), "");
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

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 55f, buttonWidth, buttonHeight), "Splits that Pause"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Pause Splits Button Down");
            }

            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 90f, buttonWidth, buttonHeight), "Splits that End"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "End Splits Button Down");
            }

            /*// Categories Menu
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
            }*/
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
