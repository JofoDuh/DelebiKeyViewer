using UnityEngine;
using Object = UnityEngine.Object;

namespace DelebiKeyViewer
{
    public class DelebiBundleManager
    {
        public static DelebiBundleManager Instance;
        public readonly AssetBundle Bundle;
        public Sprite[] PressedKeySprites;
        public Sprite[] UnpressedKeySprites;
        public Sprite DelebiIdle;
        public Sprite DelebiIdleWink;
        public Sprite DelebiDie;
        public Sprite DelebiClear;
        public Sprite DelebiSmash;
        public GameObject KeyViewerObject;

        public DelebiBundleManager()
        {
            Bundle = AssetBundle.LoadFromFile(FilesUtility.GetAssetBundlePath("dkv_assets.bundle"));
            PressedKeySprites = new Sprite[10];
            UnpressedKeySprites = new Sprite[2];
            foreach (Object asset in Bundle.LoadAllAssets())
            {
                if (asset is GameObject gameObject && ((GameObject)asset).name == "DelebiKeyViewer")
                {
                    KeyViewerObject = gameObject;
                    continue;
                }
                if (asset is not Sprite sprite) continue;
                else if (asset.name.StartsWith("pressed_key")) PressedKeySprites[int.Parse(asset.name.Substring(11)) - 1] = sprite;
                else switch (asset.name)
                    {
                        case "revi1":
                            DelebiIdle = sprite;
                            break;
                        case "revi_wink":
                            DelebiIdleWink = sprite;
                            break;
                        case "revi_8K10K_attack":
                            DelebiSmash = sprite;
                            break;
                        case "unpressed_left":
                            UnpressedKeySprites[0] = sprite;
                            break;
                        case "unpressed_right":
                            UnpressedKeySprites[1] = sprite;
                            break;
                        case "revi_clear1":
                            DelebiClear = sprite;
                            break;
                        case "revi_Die":
                            DelebiDie = sprite;
                            break;
                    }
            }
        }

        public void Dispose()
        {
            Bundle.Unload(true);
            Instance = null;
        }
    }
}