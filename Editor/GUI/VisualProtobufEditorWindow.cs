using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace VisualProtobuf.UIElements
{
    public class VisualProtobufEditorWindow : EditorWindow
    {
        const string kEditorAssetsPath = "Assets/VisualProtobuf/EditorAssets";

        public StyleSheet editorStyleSheet;

        [MenuItem("Window/General/Config %9", priority = 9)]
        public static void OpenConfigWindow()
        {
            var projectBrower = typeof(EditorWindow).Assembly.GetType("UnityEditor.ProjectBrowser");
            var w = EditorWindow.CreateWindow<VisualProtobufEditorWindow>(projectBrower);
            var iconPath = Path.Combine(kEditorAssetsPath, "Icon", EditorGUIUtility.isProSkin ? "Dark" : "Light", "Config.png");
            var iconTexture = AssetDatabase.LoadAssetAtPath<Texture>(iconPath);
            w.titleContent = new GUIContent("Config", iconTexture);
        }

        private void OnEnable()
        {
            var rootPath = Path.GetFullPath(Application.dataPath + "/../Config");
            ProtobufDatabase.Initialize(rootPath);
        }

        private void CreateGUI()
        {
            rootVisualElement.Add(CreateConfigGUI());
            if (editorStyleSheet == null) return;
            rootVisualElement.styleSheets.Add(editorStyleSheet);
        }

        VisualElement CreateConfigGUI()
        {
            return new ProtobufMainEditorView();
            //return new ProtobufInstanceEditorView();
        }
    }
}