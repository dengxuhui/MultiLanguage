using System.Collections.Generic;
using System.IO;
using MultiLanguage.Scripts.tool;
using Config = MultiLanguage.Scripts.MultiLanguageConfig;

namespace MultiLanguage.Scripts.func.collector
{
    /// <summary>
    /// 原始raw文件字符收集器
    /// </summary>
    public static class CollectRawFiles
    {
        /// <summary>
        /// 收集
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> Collect()
        {
            var rawDic = new Dictionary<string, string>();
            var rules = MultiLanguageAssetsManager.GetRules();
            var dir = FileTool.GetFullPath(rules.rawDirectory);
            var baseSupport = rules.supports[rules.basicSupportIndex];
            var rawFiles = FileTool.GetCSVs(dir);
            for (var i = 0; i < rawFiles.Length; i++)
            {
                var tbl = CsvOperater.ReadSingleFile(rawFiles[i], baseSupport.language);
                var fileName = Path.GetFileNameWithoutExtension(rawFiles[i]);
                for (var i1 = 0; i1 < tbl.Count; i1++)
                {
                    var fieldInfo = tbl[i1];
                    var key = FileTool.FromRawKeyToSummaryKey(fileName, fieldInfo.Name);
                    if (!rawDic.ContainsKey(key))
                    {
                        rawDic.Add(key, fieldInfo.GetValue(baseSupport.language));
                    }
                }
            }

            return rawDic;
        }

        /// <summary>
        /// 将原始文件拷贝到使用中的总表文件
        /// </summary>
        /// <returns></returns>
        public static CsvTable CopyToSummaryUsingFile()
        {
            var rule = MultiLanguageAssetsManager.GetRules();
            var usingFilePath = Path.Combine(FileTool.GetFullPath(rule.summaryDirectory), Config.CsvNameSummaryUsing);
            var usingTbl = CsvOperater.ReadSummaryFile(usingFilePath);
            var usingDic = usingTbl.ToDictionary();

            var allRawFieldDic = Collect();
            var supports = rule.supports;

            var add = new List<CsvFieldInfo>();
            foreach (var kv in allRawFieldDic)
            {
                if (usingDic.ContainsKey(kv.Key))
                {
                    continue;
                }

                var a = new CsvFieldInfo {Name = kv.Key};
                for (var i = 0; i < supports.Length; i++)
                {
                    a.SetValue(supports[i].language, kv.Value);
                }

                add.Add(a);
            }

            var delete = new List<string>();
            foreach (var kv in usingDic)
            {
                if (allRawFieldDic.ContainsKey(kv.Key))
                {
                    continue;
                }

                delete.Add(kv.Key);
            }

            if (delete.Count <= 0 && add.Count <= 0)
            {
                return usingTbl;
            }

            if (delete.Count > 0)
            {
                for (var i = 0; i < delete.Count; i++)
                {
                    usingDic.Remove(delete[i]);
                }
            }

            if (add.Count > 0)
            {
                for (var i = 0; i < add.Count; i++)
                {
                    usingDic.Add(add[i].Name, add[i]);
                }
            }

            usingTbl = new CsvTable();
            foreach (var kv in usingDic)
            {
                usingTbl.AddField(kv.Value);
            }

            CsvOperater.WriteSummaryFile(usingTbl, usingFilePath);
            return usingTbl;
        }
    }
}