using HarmonyLib;
using MonsterLove.StateMachine;
using System;
using DelebiKeyViewer.Component;

namespace DelebiKeyViewer
{
    public static class Patch
    {
        [HarmonyPatch(typeof(StateBehaviour), nameof(StateBehaviour.ChangeState), new Type[] { typeof(Enum) })]
        static class ChangeStatePatch
        {
            static void Postfix(Enum newState)
            {
                switch ((States)newState)
                {
                    case States.Fail:
                    case States.Fail2:
                        KeyInputManager.Die(Characters.Delebi);
                        break;
                    case States.Won:
                        if (scrController.instance.noFail && scrController.instance.mistakesManager.GetDeaths() != 0) 
                            KeyInputManager.Die(Characters.Delebi);
                        else KeyInputManager.Clear(Characters.Delebi);
                        break;
                    default:
                        KeyInputManager.ResetPatch(Characters.Delebi);
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(scrUIController), "WipeToBlack")]
        public static class WipeToBlackPatch
        {
            public static void Postfix()
            {
                KeyInputManager.ResetPatch(Characters.Delebi);
            }
        }
        [HarmonyPatch(typeof(scnEditor), "ResetScene")]
        public static class ResetScenePatch
        {
            public static void Postfix()
            {
                KeyInputManager.ResetPatch(Characters.Delebi);
            }
        }
        [HarmonyPatch(typeof(scnEditor), "SwitchToEditMode")]
        public static class SwitchToEditModePatch
        {
            public static void Postfix()
            {
                KeyInputManager.ResetPatch(Characters.Delebi);
            }
        }

        [HarmonyPatch(typeof(scrController), "StartLoadingScene")]
        public static class StartLoadingScenePatch
        {
            public static void Postfix()
            {
                KeyInputManager.ResetPatch(Characters.Delebi);
            }
        }
    }
}