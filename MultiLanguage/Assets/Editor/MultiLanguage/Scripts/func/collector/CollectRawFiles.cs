using System.Collections.Generic;
using System.IO;
using MultiLanguage.Scripts.tool;

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
    }
}