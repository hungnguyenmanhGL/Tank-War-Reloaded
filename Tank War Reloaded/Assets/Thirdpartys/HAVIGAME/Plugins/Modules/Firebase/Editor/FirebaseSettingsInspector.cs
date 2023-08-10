#if FIREBASE

using UnityEngine;
using UnityEditor;
using HAVIGAME.Editor;
using System;

namespace HAVIGAME.Plugins.Firebases.Editor {
    [CustomEditor(typeof(FirebaseSettings))]
    public class FirebaseSettingsInspector : UnityEditor.Editor {
        private const float treeWidth = 125;
        private Vector2 treeScroll;
        private Vector2 itemSettingScroll;

        private FirebaseSettings settings;
        private FirebaseModuleTreeView treeView;
        private ModuleTreeViewItem itemView;

        private void OnEnable() {
            settings = (FirebaseSettings)target;

            treeView = new FirebaseModuleTreeView();
            treeView.onSelected = OnTreeViewItemSelected;
            treeView.Build();
        }

        public override void OnInspectorGUI() {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(treeWidth), GUILayout.ExpandHeight(true));
            treeScroll = GUILayout.BeginScrollView(treeScroll);
            DrawTreeView();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(HAVIGAME.Editor.EditorUtility.Styles.Box, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            DrawItemHeader();
            itemSettingScroll = GUILayout.BeginScrollView(itemSettingScroll);
            DrawItemSettings();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void DrawTreeView() {
            treeView.OnGUI();
        }

        private void DrawItemHeader() {
            if (itemView != null) {
                GUILayout.BeginHorizontal(HAVIGAME.Editor.EditorUtility.Styles.Box);

                GUILayout.Label(string.Format("{0} Settings", itemView.content), HAVIGAME.Editor.EditorUtility.Styles.WhiteLargeLabel);
                GUILayout.Space(EditorGUIUtility.singleLineHeight);

                if (!string.IsNullOrEmpty(itemView.DefineSymbol)) {
                    bool isEnabled = HAVIGAME.Editor.EditorUtility.DefineSymbols.HasDefineSymbols(itemView.DefineSymbol);

                    if (GUILayout.Button(isEnabled ? "Enabled" : "<color=red>Disabled</color>", HAVIGAME.Editor.EditorUtility.Styles.ToolbarbuttonRichText, GUILayout.Width(60))) {
                        if (isEnabled) {
                            HAVIGAME.Editor.EditorUtility.DefineSymbols.RemoveDefineSymbols(itemView.DefineSymbol);
                        }
                        else {
                            HAVIGAME.Editor.EditorUtility.DefineSymbols.AddDefineSymbols(itemView.DefineSymbol);
                        }
                    }
                }

                GUILayout.EndHorizontal();

                EditorGUILayout.HelpBox(itemView.Description, MessageType.Info);

                GUILayout.Button("", GUILayout.Height(3));
            }
        }

        private void DrawItemSettings() {
            
        }

        private void OnTreeViewItemSelected(ModuleTreeViewItem item) {
            itemView = item;
        }

        public class ModuleTreeViewItem : TreeViewItem {
            public string DefineSymbol { get; }
            public string Description { get; }

            public ModuleTreeViewItem(GUIContent content, string description, string defineSymbol) : base(content) {
                Description = description;
                DefineSymbol = defineSymbol;
            }
        }

        private class FirebaseModuleTreeView : HAVIGAME.Editor.TreeView {
            public const string Analytics = "Analytics";
            public const string Crashlytics = "Crashlytics";
            public const string RemoteConfig = "Remote Config";

            public Action<ModuleTreeViewItem> onSelected;

            protected override TreeViewItem BuildRoot() {
                TreeViewItem root = new TreeViewItem(new GUIContent("Root"));

                root.AddChild(new ModuleTreeViewItem(new GUIContent(Crashlytics), "", FirebaseManager.CRASHLYTICS_DEFINE_SYMBOL));
                root.AddChild(new ModuleTreeViewItem(new GUIContent(Analytics), "", FirebaseManager.ANALYTICS_DEFINE_SYMBOL));
                root.AddChild(new ModuleTreeViewItem(new GUIContent(RemoteConfig), "", FirebaseManager.REMOTE_CONFIG_DEFINE_SYMBOL));

                return root;
            }

            protected override void OnItemSelected(TreeViewItem item) {
                base.OnItemSelected(item);

                if (item is ModuleTreeViewItem viewItem) {
                    onSelected?.Invoke(viewItem);
                }
            }
        }
    }
}
#endif
