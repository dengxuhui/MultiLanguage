using MultiLanguage.Scripts.func.builder;
using MultiLanguage.Scripts.func.checker;
using MultiLanguage.Scripts.func.collector;
using MultiLanguage.Scripts.tool;
using UnityEditor;
using Config = MultiLanguage.Scripts.MultiLanguageConfig;

namespace MultiLanguage.Scripts.func
{
    /// <summary>
    /// 导出翻译到csv文件
    /// </summary>
    public static class FuncBuild
    {
        public static void Start(bool exportTranslate, bool updateTmp, bool updateFromPrefab, bool updateFromXlsx)
        {
            //执行检查
            Checker.DoCheck();
            if (updateFromPrefab)
            {
                CollectPrefabs.UpdateRawFile();
            }

            if (updateFromXlsx)
            {
                CollectXlsxs.UpdateRawFile();
            }
            var usingTbl = CollectRawFiles.CopyToSummaryUsingFile();
            //更新翻译需求表
            if (exportTranslate)
            {
                TranslationNeedsBuilder.Build(usingTbl, null, null);
                CollectDiscardField.Collect(usingTbl, null);
            }

            AllLanguageBuilder.Build(usingTbl);

            //如果需要更新tmp
            if (updateTmp)
            {
                TMP_AssetTool.UpdateTMP_Asset(usingTbl, Progress);
            }

            //最后刷新一下资源
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }

        private static void Progress(float progress, string info = "")
        {
            EditorUtility.DisplayProgressBar("Building Language", info, progress);
        }
    }
}