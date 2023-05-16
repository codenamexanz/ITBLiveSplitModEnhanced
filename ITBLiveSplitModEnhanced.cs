using System.IO;
using MelonLoader;
using UnityEngine;
using static UnityEngine.Object;
using Il2Cpp;
using HarmonyLib;
using System.Reflection;
using static MelonLoader.MelonLogger;
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
        private Rect runSelectorRect = new Rect(10f, 10f, 200f, 250f);
        private static bool showCategoriesMenu = false;
        private static bool showEndingsMenu = false;
        private static bool showILsMenu = false;
        private static bool showCategoryExtensionsMenu = false;
        //Setup GameRule
        private enum GameRule
        {
            Any,
            Glitchless,
            Inbounds
        }
        private static GameRule selectedGameRule;
        //Setup Run Categories
        private enum RunCategory
        {
            Escaped,
            Fun,
            Chase,
            Sewer,
            Hotel,
            AllEndings,
            Darkrooms,
            Garage,
            Office,
            FunRoom,
            ChaseLevel,
            Sewers,
            TerrorHotel
        }
        private static RunCategory selectedRunCategory;

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
            if (sceneName == "MainLevel")
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

        public void DrawRunSelector()
        {
            GUI.Box(runSelectorRect, "Run Selector");
            runSelectorRect.width = 270;
            runSelectorRect.height = 110;
            float buttonWidth = runSelectorRect.width - 10f;
            float buttonHeight = 30f;
            float smallButtonWidth = 80f;

            // Run Types
            if (GUI.Button(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 40f, smallButtonWidth, buttonHeight), "Any"))
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Any Button Down");
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
                GUI.Box(new Rect(runSelectorRect.x + 5f, runSelectorRect.y + 105f, buttonWidth, 110f), "");

                // Endings submenu
                if (GUI.Button(new Rect(runSelectorRect.x + 15f, runSelectorRect.y + 110f, buttonWidth - 20f, buttonHeight), "Endings"))
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "Endings Button Down");
                    showEndingsMenu = !showEndingsMenu;
                }

                // ILs submenu
                if (GUI.Button(new Rect(runSelectorRect.x + 15f, runSelectorRect.y + 145f, buttonWidth - 20f, buttonHeight), "ILs"))
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "ILs Button Down");
                    showILsMenu = !showILsMenu;
                }

                // Extensions submenu
                if (GUI.Button(new Rect(runSelectorRect.x + 15f, runSelectorRect.y + 180f, buttonWidth - 20f, buttonHeight), "Extensions"))
                {
                    MelonLogger.Msg(System.ConsoleColor.Green, "Extensions Button Down");
                    showCategoryExtensionsMenu = !showCategoryExtensionsMenu;
                }
            }
        }
    }
}