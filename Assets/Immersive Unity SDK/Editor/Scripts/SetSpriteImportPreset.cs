using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

#if UNITY_2019_4_OR_NEWER

namespace Immersive.Presets
{
    /// <summary>
    /// Provides Menu Items to set the default Texture Importer for for 1080p or 1200p screens.
    /// </summary>
    public static class SetSpriteImportPreset
    {
        private const string spritePreset1080Name = "Sprite Preset 1080";
        private const string spritePreset1200Name = "Sprite Preset 1200";


        private static string spritePreset1080Path = null;
        private static string SpritePreset1080Path 
        {
            get
            {
                if (spritePreset1080Path == null)
                    spritePreset1080Path = GetTemplateAssetPath(spritePreset1080Name);
                return spritePreset1080Path;
            }
        }


        private static string spritePreset1200Path = null;
        private static string SpritePreset1200Path
        {
            get
            {
                if (spritePreset1200Path == null)
                    spritePreset1200Path = GetTemplateAssetPath(spritePreset1200Name);
                return spritePreset1200Path;
            }
        }
        private static string GetTemplateAssetPath(string fileName)
        {
            var guids = AssetDatabase.FindAssets(fileName);
            if (guids.Length == 1)
                return AssetDatabase.GUIDToAssetPath(guids[0]);
            return null;
        }

        [MenuItem("Immersive Interactive/Presets/Set Sprite 1080")]
        public static void SetSprite1080()
        {

            var preset = AssetDatabase.LoadAssetAtPath<Preset>(SpritePreset1080Path);
            SetAsDefault(preset);
        }

        [MenuItem("Immersive Interactive/Presets/Set Sprite 1080", true)]
        public static bool IsSprite1080()
        {
            var preset = AssetDatabase.LoadAssetAtPath<Preset>(SpritePreset1080Path);
            return !CheckIfCurrentlyDefault(preset);
        }

        [MenuItem("Immersive Interactive/Presets/Set Sprite 1200")]
        public static void SetSprite1200()
        {
            var preset = AssetDatabase.LoadAssetAtPath<Preset>(SpritePreset1200Path);

            SetAsDefault(preset);
        }

        [MenuItem("Immersive Interactive/Presets/Set Sprite 1200", true)]
        public static bool IsSprite1200()
        {
            var preset = AssetDatabase.LoadAssetAtPath<Preset>(SpritePreset1200Path);
            return !CheckIfCurrentlyDefault(preset);
        }

        private static void SetAsDefault(Preset preset)
        {
            var type = preset.GetPresetType();
            if (type.IsValidDefault())
            {
                Preset.SetDefaultPresetsForType(type, new DefaultPreset[] { new DefaultPreset(string.Empty, preset) });
            }
        }

        private static bool CheckIfCurrentlyDefault(Preset preset)
        {
            var type = preset.GetPresetType();
            if (type.IsValidDefault())
            {
                var presets = Preset.GetDefaultPresetsForType(type);
                if (presets.Length > 0)
                    return preset == presets[0].preset;
            }
            return false;
        }
    }
}

#endif