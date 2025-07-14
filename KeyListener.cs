using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace BingoSyncHighlighter
{
    public class KeyListener : MonoBehaviour
    {
        List<KeyCode> activationKeys = new List<KeyCode>() { KeyCode.Keypad0 };
        int row = -1;
        bool listening = false;
        bool useNumpad = true;
        bool useKeyboardNumbers = false;

        public void Update()
        {
            DetectHotkeys();
        }

        void DetectHotkeys()
        {
            foreach (KeyCode key in activationKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    row = -1;
                    listening = true;
                    return;
                }
            }

            if (!listening) return;

            if (useNumpad)
            {
                for (KeyCode i = KeyCode.Keypad1; i <= KeyCode.Keypad5; i++)
                {
                    if (Input.GetKeyDown(i))
                    {
                        int keyAsInt = (int)(i - KeyCode.Keypad1);
                        if (row == -1)
                        {
                            row = keyAsInt;
                        }
                        else
                        {
                            int column = keyAsInt;
                            HighLightGoal(row, column);
                            row = -1;
                            column = -1;
                            listening = false;
                        }
                        return;
                    }
                }
            }


            if (useKeyboardNumbers)
            {
                for (KeyCode i = KeyCode.Alpha1; i <= KeyCode.Alpha5; i++)
                {
                    if (Input.GetKeyDown(i))
                    {
                        int keyAsInt = (int)(i - KeyCode.Alpha1);
                        if (row == -1)
                        {
                            row = keyAsInt;
                        }
                        else
                        {
                            int column = keyAsInt;
                            HighLightGoal(row, column);
                            row = -1;
                            column = -1;
                            listening = false;
                        }
                        return;
                    }
                }
            }
        }

        void HighLightGoal(int row, int column)
        {
            BingoSyncHighlighter.Instance.HighlightGoal(row, column);
        }

        public void SetKeys(bool keyboard, bool numpad, string triggerKey)
        {
            useKeyboardNumbers = keyboard;
            useNumpad = numpad;
            if (triggerKey == null || triggerKey == "")
            {
                activationKeys = new List<KeyCode>();
                if (useKeyboardNumbers) activationKeys.Add(KeyCode.Alpha0);
                if (useNumpad) activationKeys.Add(KeyCode.Keypad0);
                return;
            }
            try
            {
                activationKeys = new List<KeyCode>() { (KeyCode)System.Enum.Parse(typeof(KeyCode), triggerKey) };
                BingoSyncHighlighter.Instance.Log($"Initialized with custom activation key {activationKeys[0].ToString()}");
            }
            catch
            {
                // Modding.Logger.LogError($"{triggerKey} is not a valid unity keycode. Defaulting to Keypad0");
                BingoSyncHighlighter.Instance.Log($"{triggerKey} is not a valid unity keycode. Defaulting to Keypad0");
                activationKeys = new List<KeyCode>() { KeyCode.Keypad0 };
            }

        }

    }
}