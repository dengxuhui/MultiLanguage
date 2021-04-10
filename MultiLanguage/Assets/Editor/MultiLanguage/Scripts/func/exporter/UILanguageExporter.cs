using System;
using System.Collections.Generic;
using System.IO;
using Editor.MultiLanguage.Scripts.tool;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Editor.MultiLanguage.Scripts.func.exporter
{
    /// <summary>
    /// ui语言导出工具
    /// </summary>
    public static class UILanguageExporter
    {
        public static Dictionary<string, string> Run(Action<float, string> progressCb = null)
        {
            var rules = MultiLanguageAssetsManager.GetRules();
            var directory = Path.GetDirectoryName(Application.dataPath);
            if (string.IsNullOrEmpty(directory) || string.IsNullOrEmpty(rules.uiPrefabDirectory))
            {
                return null;
            }

            var prefabDir = Path.Combine(directory, rules.uiPrefabDirectory);
            if (!Directory.Exists(prefabDir))
            {
                return null;
            }

            var uiStrDic = new Dictionary<string, string>();
            var uiFiles = FileTool.GetPrefabs(prefabDir);
            for (var i = 0; i < uiFiles.Length; i++)
            {
                var filePath = uiFiles[i];
                if (filePath == null) continue;
                var uiName = Path.GetFileNameWithoutExtension(filePath);
                var szBuildFileSrc = filePath.Replace(Application.dataPath, "Assets");
                var go = AssetDatabase.LoadAssetAtPath(szBuildFileSrc, typeof(object)) as GameObject;
                progressCb?.Invoke(0.6f, $"导出ui字符串中，检索:{uiName}...");

                if (go == null)
                {
                    Debug.LogErrorFormat("failed to load asset:{0}", szBuildFileSrc);
                    continue;
                }

                var tfs = go.GetComponentsInChildren<TMP_Text>(true);
                foreach (var t in tfs)
                {
                    if (t.name.Length > 2 &&
                        ((t.name.Substring(0, 2) == "m_") && (t.name.Substring(0, 3) != "m_z"))) continue;
                    var keyName = go.name + "_" + t.name;
                    if (string.IsNullOrEmpty(t.text)) continue;
                    t.text = t.text.Trim('\n', '\r');
                    if (!uiStrDic.ContainsKey(keyName))
                    {
                        uiStrDic.Add(keyName, t.text);
                        Debug.LogFormat("-> 搜集 {0} = {1}", keyName, t.text);
                    }
                    else
                    {
                        Debug.LogWarningFormat("-> 字符串名冲突 key = {0} , string = {1}", keyName, t.text);
                    }
                }
            }

            return uiStrDic;
        }
    }
}