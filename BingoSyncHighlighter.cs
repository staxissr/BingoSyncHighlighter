using HutongGames.PlayMaker.Actions;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace BingoSyncHighlighter
{
    public class BingoSyncHighlighter : Mod, IGlobalSettings<GlobalSettingsClass>, IMenuMod
    {
        internal static BingoSyncHighlighter Instance;
        public static GlobalSettingsClass GS { get; set; } = new GlobalSettingsClass();
        KeyListener kl;

        public override string GetVersion() => "1.0.0";
        public void OnLoadGlobal(GlobalSettingsClass s)
        {
            GS = s;
        }
        public GlobalSettingsClass OnSaveGlobal()
        {
            return GS;
        }

        GameObject keyListener;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            Instance = this;
            keyListener = new GameObject("KeyListener");
            GameObject.DontDestroyOnLoad(keyListener);
            kl = keyListener.AddComponent<KeyListener>();
            SetKeys();

            Log("Initialized");
        }

        void SetKeys()
        {
            kl.SetKeys(GS.useKeyboardNumbers, GS.useNumpad, GS.customActivationKey);
        }

        public void HighlightGoal(int row, int column)
        {
            var session = BingoSync.Interfaces.SessionManager.GetActiveSession();
            session.Board.GetSlot(row * 5 + column).Highlighted ^= true;
            session.LocalUpdate();
        }

        void UnhighlightAllGoals()
        {
            var session = BingoSync.Interfaces.SessionManager.GetActiveSession();
            for (int i=0; i < 25; i++) session.Board.GetSlot(i).Highlighted = false;
            session.LocalUpdate();
        }

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new List<IMenuMod.MenuEntry>
        {
            new IMenuMod.MenuEntry {
                Name = "Enabled",
                Description = null,
                Values = new string[] {
                    "Yes",
                    "No"
                },
                // opt will be the index of the option that has been chosen
                Saver = opt => {
                    if (opt == 0) {
                        kl.gameObject.SetActive(true);
                    } else {
                        kl.gameObject.SetActive(false);
                        UnhighlightAllGoals();
                    }
                },
                Loader = () => 0
            },
            new IMenuMod.MenuEntry {
                Name = "Input method",
                Description = null,
                Values = new string[] {
                    "Numpad numbers",
                    "Keyboard numbers",
                    "Numpad or keyboard"
                },
                // opt will be the index of the option that has been chosen
                Saver = opt => {
                    GS.useKeyboardNumbers = opt == 1 || opt == 2;
                    GS.useNumpad = opt == 0 || opt == 2;
                    SetKeys();
                },
                Loader = () => {
                    if (GS.useKeyboardNumbers && GS.useNumpad) return 2;
                    else if (GS.useNumpad) return 0;
                    return 1;
                }
            }
        };
        }

        bool IMenuMod.ToggleButtonInsideMenu => true;

    }
}