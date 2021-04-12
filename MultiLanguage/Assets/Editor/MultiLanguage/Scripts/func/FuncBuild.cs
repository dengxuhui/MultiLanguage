using System.IO;
using MultiLanguage.Scripts.func.builder;
using MultiLanguage.Scripts.func.checker;
using MultiLanguage.Scripts.func.collector;
using MultiLanguage.Scripts.tool;
using UnityEditor;
using UnityEngine;
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

            CollectCustomCsv();

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

        private static void CollectCustomCsv()
        {
            var rules = MultiLanguageAssetsManager.GetRules();
            var sDir = Path.GetDirectoryName(Application.dataPath);
            if (string.IsNullOrEmpty(sDir))
            {
                return;
            }

            var tDir = FileTool.GetFullPath(rules.rawDirectory);
            FileTool.TryMakeDir(tDir);
            //拷贝自定义csv
            for (var i = 0; i < rules.customCsvs.Length; i++)
            {
                var srcPath = rules.customCsvs[i];
                var extension = Path.GetExtension(srcPath);
                if (extension != ".csv")
                {
                    continue;
                }

                srcPath = Path.Combine(sDir, srcPath);
                if (!File.Exists(srcPath))
                {
                    continue;
                }

                var tarPath = Path.Combine(tDir, Path.GetFileName(srcPath));
                if (File.Exists(tarPath))
                {
                    File.Delete(tarPath);
                }
                File.Copy(srcPath,tarPath);
            }
        }
    }
}