// ReSharper disable All

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using Config = Editor.MultiLanguage.Scripts.MultiLanguageConfig;

namespace Editor.MultiLanguage.Scripts.tool
{
    /// <summary>
    /// text mesh pro字体相关工具
    /// </summary>
    public static class TMP_AssetTool
    {
        /// <summary>
        /// 获取语言在设置中的字体文件设置
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public static TMP_Font GetTMP_Font(Language language)
        {
            var rules = MultiLanguageAssetsManager.GetRules();
            if (rules == null || rules.supports.Length <= 0)
            {
                Debug.LogError("Get sdf font config error,language rules config not exist supports language array");
                return TMP_Font.Common;
            }

            var supports = rules.supports;
            for (var i = 0; i < supports.Length; i++)
            {
                if (supports[i].language == language)
                {
                    return supports[i].tmpFont;
                }
            }

            Debug.LogError($"Can not find [{language.ToString()}] in multi language support language array!!!!");
            return TMP_Font.Common;
        }

        /// <summary>
        /// 更新TMP .asset资源
        /// </summary>
        /// <param name="usingTbl"></param>
        /// <param name="progressCallBack"></param>
        public static void UpdateTMP_Asset(CsvTable usingTbl, Action<float, string> progressCallBack)
        {
            if (usingTbl == null || usingTbl.Count <= 0)
            {
                return;
            }

            var rules = MultiLanguageAssetsManager.GetRules();

            #region 先收集并导出字符集
            var sdfFontDic = new Dictionary<TMP_Font, Dictionary<char, char>>();
            for (var i = 0; i < usingTbl.Count; i++)
            {
                var fieldInfo = usingTbl[i];
                fieldInfo.Walk((lang, content) =>
                {
                    var tmpFont = TMP_AssetTool.GetTMP_Font(lang);
                    sdfFontDic.TryGetValue(tmpFont, out var charDic);
                    if (charDic == null)
                    {
                        charDic = new Dictionary<char, char>();
                        sdfFontDic.Add(tmpFont, charDic);
                    }

                    var charArray = content.ToCharArray();
                    for (var i1 = 0; i1 < charArray.Length; i1++)
                    {
                        var cc = charArray[i1];
                        if (charDic.ContainsKey(cc)) continue;
                        charDic.Add(cc, cc);
                    }
                });
            }

            var saveFullPath = FileTool.GetFullPath(rules.fontDirectory);
            FileTool.TryMakeDir(saveFullPath);

            foreach (var kv in sdfFontDic)
            {
                Config.SdfCharFileNameDic.TryGetValue(kv.Key, out var f);
                var sdfFileName = $"{f}{Config.SdfCharFileExtension}";
                var sdfFp = Path.Combine(saveFullPath, sdfFileName);
                var writeStr = string.Join("", kv.Value.Values.ToArray());
                using (var sw = new StreamWriter(sdfFp, false, Encoding.Unicode))
                {
                    sw.WriteLine(writeStr);
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
            

            #endregion

            OpenAssetCreatorWindow();
        }

        private static void OpenAssetCreatorWindow()
        {
            var settings = new FontAssetCreationSettings();
            TMPro_FontAssetCreatorWindow.ShowFontAtlasCreatorWindow();
        }
    }
}