using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
#if UNITYMODMANAGER
using UnityModManagerNet; 
#endif

namespace DelebiKeyViewer
{
    public class Setting
#if UNITYMODMANAGER
        : UnityModManager.ModSettings
#endif
    {
        public readonly Dictionary<string, Dictionary<SystemLanguage, string>> Localization = new()
        {
            {
                "dkv.size", new()
                {
                    { SystemLanguage.English, "Size" },
                    { SystemLanguage.German, "Größe" },
                    { SystemLanguage.Korean, "크기" },
                }
            },
            {
                "dkv.posx", new()
                {
                    { SystemLanguage.English, "X Location" },
                    { SystemLanguage.German, "X-Position" },
                    { SystemLanguage.Korean, "X 위치" },
                }
            },
            {
                "dkv.posy", new()
                {
                    { SystemLanguage.English, "Y Location" },
                    { SystemLanguage.German, "Y-Position" },
                    { SystemLanguage.Korean, "Y 위치" },
                }
            },
            {
                "dkv.fliphorizontal", new()
                {
                    { SystemLanguage.English, "Flip Horizontal" },
                    { SystemLanguage.German, "Horizontal spiegeln" },
                    { SystemLanguage.Korean, "수평 반전" },
                }
            },
            {
                "dkv.pressakey", new()
                {
                    { SystemLanguage.English, "Press a key" },
                    { SystemLanguage.German, "Drücken Sie eine Taste" },
                    { SystemLanguage.Korean, "키를 누르세요" },
                }
            },
            {
                "dkv.settingslabel", new()
                {
                    { SystemLanguage.English, "Key Viewer Settings" },
                    { SystemLanguage.German, "Tastenanzeige-Einstellungen" },
                    { SystemLanguage.Korean, "키뷰어 설정" },
                }
            },
            {
                "dkv.toprow", new()
                {
                    { SystemLanguage.English, "Top Row" },
                    { SystemLanguage.German, "Obere Reihe" },
                    { SystemLanguage.Korean, "상단 줄" },
                }
            },
            {
                "dkv.bottomrow", new()
                {
                    { SystemLanguage.English, "Bottom Row" },
                    { SystemLanguage.German, "Untere Reihe" },
                    { SystemLanguage.Korean, "하단 줄" },
                }
            },
        };
        public float Size = 1;
        public float LocationX = 0;
        public float LocationY = 1;
        public bool FlipHorizontal = false;
        public bool ShareJipperResourcePack = true;

        [JsonIgnore]
        public bool KeyCodeJipperResourcePack = false;

        public KeyCode[] KeyCodes = new KeyCode[]
        {
            KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F,
            KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Semicolon,
            KeyCode.Z, KeyCode.X
        };


        public string GetLocalized(string key)
        {
            if (Localization.TryGetValue(key, out var langDict) &&
                langDict.TryGetValue(RDString.language, out var localizedText))
            {
                return localizedText;
            }

            return langDict?.TryGetValue(SystemLanguage.English, out var fallback) == true ? fallback : key;
        }

#if !BEPINEX
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            try
            {
                var filepath = GetPath(modEntry);
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                };
                var json = JsonConvert.SerializeObject(this, settings);
                File.WriteAllText(filepath, json);
            }
            catch (Exception e)
            {
                Main.Logger?.Log($"[Setting.Save] Exception: {e}");
            }
        }

        public override string GetPath(UnityModManager.ModEntry modEntry)
        {
            return Path.Combine(modEntry.Path, GetType().Name + ".json");
        }

        public static Setting Load(UnityModManager.ModEntry modEntry)
        {
            var filepath = Path.Combine(modEntry.Path, typeof(Setting).Name + ".json");

            if (!File.Exists(filepath))
            {
                return new Setting();
            }

            try
            {
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                };

                var json = File.ReadAllText(filepath);
                var setting = JsonConvert.DeserializeObject<Setting>(json, settings) ?? new Setting();

                // Optional external call
                //JipperResourcePackAPI.CheckJipperResourcePack(); // Remove or replace if needed

                return setting;
            }
            catch (Exception e)
            {
                Main.Logger?.Log($"[Setting.Load] Failed to load settings: {e}");
                return new Setting();
            }
        }
#endif
        //public void ShareJipperKeyCode(bool enable)
        //{
        //    if (enable)
        //    {
        //        KeyCodes = JipperResourcePackAPI.GetKey16(); // External call
        //        KeyCodeJipperResourcePack = true;
        //    }
        //    else
        //    {
        //        var keyCodes = new KeyCode[16];
        //        Array.Copy(KeyCodes, keyCodes, 16);
        //        KeyCodes = keyCodes;
        //        KeyCodeJipperResourcePack = false;
        //    }
        //}
    }
}