using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using DelebiKeyViewer.Component;
using UnityEngine;

namespace DelebiKeyViewer
{

    public static class KeyInputManager
    {
        public static readonly int[] HandLocation = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 
                                                                        8, 9, };        
        public static bool Reset;
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
        private static bool CheckKey(KeyCode keyCode) => (int)keyCode < 0x1000 ? Input.GetKey(keyCode) : GetAsyncKeyState((int)keyCode - 0x1000) != 0;

        public static void ListenKey()
        {
            try
            {
                Setting setting = Main.setting;
                bool[] keyState = new bool[16];
                List<int> leftPressed = new();
                List<int> rightPressed = new();
                int mainCount = 0;
                scrDelebiKeyViewer keyViewer = Main.KeyViewer;
                while (Main.IsEnabled)
                {
                    if (Reset)
                    {
                        foreach (Key key in keyViewer.keys) key.enable = 0;
                        keyState = new bool[16];
                        leftPressed.Clear();
                        rightPressed.Clear();
                        keyViewer.leftHand.sprite = DelebiBundleManager.Instance.UnpressedKeySprites[0];
                        keyViewer.rightHand.sprite = DelebiBundleManager.Instance.UnpressedKeySprites[1];
                        mainCount = 0;
                        Reset = false;
                    }
                    KeyCode[] keyCodes = setting.KeyCodes;
                    for (int i = 0; i < keyCodes.Length; i++)
                    {
                        bool current = CheckKey(keyCodes[HandLocation[i]]);
                        if (current == keyState[i]) continue;
                        int num;
                        if (setting.FlipHorizontal)
                        {
                            if (i < 8)
                                num = 7 - i; 
                            else    
                                num = 9 - (i - 8);  
                        }
                        else
                        {
                            num = i;
                        }
                        bool left = (num < 4) || (num == 8);
                        Key key = keyViewer.keys[num];
                        keyState[i] = current;
                        key.enable = (sbyte)(current ? 1 : 0);
                        List<int> pressed = left ? leftPressed : rightPressed;
                        if (current)
                        {
                            pressed.Add(num);
                            (left ? keyViewer.leftHand : keyViewer.rightHand).sprite = DelebiBundleManager.Instance.PressedKeySprites[num];
                        }
                        else
                        {
                            pressed.Remove(num);
                            (left ? keyViewer.leftHand : keyViewer.rightHand).sprite =
                                pressed.Count == 0 ? DelebiBundleManager.Instance.UnpressedKeySprites[left ? 0 : 1] : DelebiBundleManager.Instance.PressedKeySprites[pressed[pressed.Count - 1]];
                        }
                        if (i >= 8) continue;
                        if (current) mainCount++;
                        else mainCount--;
                        if (keyViewer.gameResult) continue;
                        if (mainCount < 8)
                        {
                            if (!keyViewer.isSmashing) continue;
                            keyViewer.DelebiSmash.sprite = keyViewer.DelebiSmash.image.sprite = DelebiBundleManager.Instance.DelebiSmash;
                            keyViewer.Delebi.enable = 1;
                            keyViewer.leftHand.enable = 1;
                            keyViewer.rightHand.enable = 1;
                            keyViewer.DelebiSmash.enable = 0;
                            keyViewer.winkOn = false;
                            keyViewer.isSmashing = false;
                        }
                        else if (!keyViewer.isSmashing)
                        {
                            keyViewer.Delebi.enable = 0;
                            keyViewer.leftHand.enable = 0;
                            keyViewer.rightHand.enable = 0;
                            keyViewer.DelebiSmash.enable = 1;
                            keyViewer.isSmashing = true;
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception e)
            {
                Main.Logger.Log($"Issue with KeyInputManager{e.Message}");
            }
        }

        public static void Die(Characters characters)
        {
            switch (characters)
            {
                case Characters.Delebi:
                    scrDelebiKeyViewer keyViewer = Main.KeyViewer;
                    if (keyViewer.gameResult) return;
                    keyViewer.gameResult = true;
                    keyViewer.isSmashing = false;
                    keyViewer.winkOn = false;
                    keyViewer.DelebiClear.sprite = keyViewer.DelebiClear.image.sprite = DelebiBundleManager.Instance.DelebiDie;
                    keyViewer.DelebiClear.enable = 1;
                    keyViewer.Delebi.enable = 0;
                    keyViewer.leftHand.enable = 1;
                    keyViewer.rightHand.enable = 1;
                    keyViewer.DelebiSmash.enable = 0;
                    break;
            }
        }
        public static void Clear(Characters characters)
        {
            switch (characters)
            {
                case Characters.Delebi:
                    scrDelebiKeyViewer keyViewer = Main.KeyViewer;
                    if (keyViewer.gameResult) return;
                    keyViewer.gameResult = true;
                    keyViewer.isSmashing = false;
                    keyViewer.winkOn = false;
                    keyViewer.Delebi.enable = 0;
                    keyViewer.leftHand.enable = 0;
                    keyViewer.rightHand.enable = 0;
                    keyViewer.DelebiSmash.enable = 0;
                    keyViewer.DelebiClear.sprite = keyViewer.DelebiClear.image.sprite = DelebiBundleManager.Instance.DelebiClear;
                    keyViewer.DelebiClear.enable = 1;
                    break;
            }
        }
        public static void ResetPatch(Characters characters)
        {
            switch (characters)
            {
                case Characters.Delebi:
                    scrDelebiKeyViewer keyViewer = Main.KeyViewer;
                    if (!keyViewer.gameResult)
                    {
                        keyViewer.Delebi.sprite = keyViewer.Delebi.image.sprite = DelebiBundleManager.Instance.DelebiIdle;
                    }
                    keyViewer.Delebi.enable = 1;
                    keyViewer.leftHand.enable = 1;
                    keyViewer.rightHand.enable = 1;
                    if (keyViewer.DelebiClear.image.enabled && keyViewer.gameResult)
                    {
                        keyViewer.Delebi.enable = 0;
                        keyViewer.DelebiClear.enable = 1;
                    }
                    else
                    {
                        keyViewer.DelebiClear.enable = 0;
                    }
                    if (keyViewer.DelebiSmash.image.enabled && keyViewer.isSmashing)
                    {
                        keyViewer.Delebi.enable = 0;
                        keyViewer.leftHand.enable = 0;
                        keyViewer.rightHand.enable = 0;
                        keyViewer.DelebiSmash.enable = 1;
                    }
                    else
                    {
                        keyViewer.DelebiSmash.enable = 0;
                        keyViewer.DelebiSmash.image.sprite = DelebiBundleManager.Instance.DelebiSmash;
                    }
                    keyViewer.gameResult = false;
                    break;
            }
        }
    }
}