using UnityEditor;
using UnityEngine;

namespace Editor.MultiLanguage
{
    /// <summary>
    /// 多语言控制面板
    /// author by Aer @2021.04.02
    /// </summary>
    public class MultiLanguageWindow : EditorWindow
    {
        string myString = "Hello World";
        bool groupEnabled;
        bool myBool = true;
        float myFloat = 1.23f;
        Rect windowRect = new Rect(20, 20, 120, 50);

        /// <summary>
        /// 打开入口
        /// </summary>
        [MenuItem("Window/MultiLanguage", false, 0)]
        static void Init()
        {
            EditorWindow.GetWindow(typeof(MultiLanguageWindow));
        }

        private void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            myString = EditorGUILayout.TextField("Text Field", myString);

            groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            myBool = EditorGUILayout.Toggle("Toggle", myBool);
            myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
            EditorGUILayout.EndToggleGroup();

            var rules = MultiLanguageAssetsManager.GetRules();
            
            windowRect = GUILayout.Window(0, windowRect, DoMyWindow, "My Window");
        }

        // Make the contents of the window
        void DoMyWindow(int windowID)
        {
            // This button will size to fit the window
            if (GUILayout.Button("Hello World"))
            {
                // ("Got a click");
                Debug.Log("Got a click");
            }
        }
    }
}