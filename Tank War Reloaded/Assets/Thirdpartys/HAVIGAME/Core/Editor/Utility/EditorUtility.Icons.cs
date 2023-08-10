using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HAVIGAME.Editor {
    public static partial class EditorUtility {

        public static class Icons {
            public class IconHolder {

                private string path;
                private Texture2D icon;

                public Texture2D Icon {
                    get {
                        if (icon != null) {
                            return icon;
                        }

                        icon = Resources.Load<Texture2D>(path);

                        return icon;
                    }
                }

                public IconHolder(string path) {
                    this.path = path;
                }
            }

            private static readonly Dictionary<string, IconHolder> iconHolders = new Dictionary<string, IconHolder>(16);

            public static Texture2D GetIcon(string path) {
                if (!string.IsNullOrEmpty(path)) {
                    if (iconHolders.TryGetValue(path, out IconHolder result)) {
                        return result.Icon;
                    }
                    else {
                        IconHolder iconHolder = new IconHolder(path);
                        iconHolders[path] = iconHolder;
                        return iconHolder.Icon;
                    }
                }
                return null;
            }
        }

        public static class IconMiner {
            [MenuItem("Window/HAVIGAME/Export Unity Icons", priority = -1001)]
            private static void ExportIcons() {
                UnityEditor.EditorUtility.DisplayProgressBar("Export Icons", "Exporting...", 0.0f);
                try {
                    var editorAssetBundle = GetEditorAssetBundle();
                    var iconsPath = GetIconsPath();
                    var count = 0;
                    foreach (var assetName in EnumerateIcons(editorAssetBundle, iconsPath)) {
                        var icon = editorAssetBundle.LoadAsset<Texture2D>(assetName);
                        if (icon == null)
                            continue;

                        var readableTexture = new Texture2D(icon.width, icon.height, icon.format, icon.mipmapCount > 1);

                        Graphics.CopyTexture(icon, readableTexture);

                        var folderPath = Path.GetDirectoryName(Path.Combine("icons/original/", assetName.Substring(iconsPath.Length)));
                        if (Directory.Exists(folderPath) == false)
                            Directory.CreateDirectory(folderPath);

                        var iconPath = Path.Combine(folderPath, icon.name + ".png");
                        File.WriteAllBytes(iconPath, readableTexture.EncodeToPNG());

                        count++;
                    }

                    Debug.Log($"{count} icons has been exported!");
                }
                finally {
                    UnityEditor.EditorUtility.ClearProgressBar();
                }
            }

            private static IEnumerable<string> EnumerateIcons(AssetBundle editorAssetBundle, string iconsPath) {
                foreach (var assetName in editorAssetBundle.GetAllAssetNames()) {
                    if (assetName.StartsWith(iconsPath, StringComparison.OrdinalIgnoreCase) == false)
                        continue;
                    if (assetName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) == false &&
                        assetName.EndsWith(".asset", StringComparison.OrdinalIgnoreCase) == false)
                        continue;

                    yield return assetName;
                }
            }

            private static AssetBundle GetEditorAssetBundle() {
                var editorGUIUtility = typeof(EditorGUIUtility);
                var getEditorAssetBundle = editorGUIUtility.GetMethod(
                    "GetEditorAssetBundle",
                    BindingFlags.NonPublic | BindingFlags.Static);

                return (AssetBundle)getEditorAssetBundle.Invoke(null, new object[] { });
            }

            private static string GetIconsPath() {
#if UNITY_2018_3_OR_NEWER
                return UnityEditor.Experimental.EditorResources.iconsPath;
#else
            var assembly = typeof(EditorGUIUtility).Assembly;
            var editorResourcesUtility = assembly.GetType("UnityEditorInternal.EditorResourcesUtility");

            var iconsPathProperty = editorResourcesUtility.GetProperty(
                "iconsPath",
                BindingFlags.Static | BindingFlags.Public);

            return (string)iconsPathProperty.GetValue(null, new object[] { });
#endif
            }
        }
    }
}


