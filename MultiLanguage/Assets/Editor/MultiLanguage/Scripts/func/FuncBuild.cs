using System.Collections.Generic;
using System.IO;
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
        /// <summary>
        /// 合并目录
        /// </summary>
        private static string _fullSummaryDir = "";

        /// <summary>
        /// 翻译中的目录
        /// </summary>
        private static string _fullTranslatingDir = "";

        /// <summary>
        /// 多语言规则设置文件数据
        /// </summary>
        private static MultiLanguageRules _rules;

        public static void Start(bool exportTranslate, bool updateTmp, bool updateFromPrefab, bool updateFromXlsx)
        {
            //执行检查
            Checker.DoCheck();

            _rules = MultiLanguageAssetsManager.GetRules();
            _fullSummaryDir = FileTool.GetFullPath(_rules.summaryDirectory);
            _fullTranslatingDir = FileTool.GetFullPath(_rules.translatingDirectory);

            if (updateFromPrefab)
            {
                CollectPrefabs.UpdateRawFile();
            }

            if (updateFromXlsx)
            {
                CollectXlsxs.UpdateRawFile();
            }

            //1.更新原始文件：原始文件有的，Using没有的，写进去，原始文件没有的，Using有的，从Using删除
            //2.更新翻译需求表：Using中有的，已翻译文件中没有的，需要翻译，已翻译文件中有的，Using没有的，从已翻译中删除（被弃用：将这个已翻译的字段放到DiscardCache文件中，用于后续有需求的话从里面找回）
            Progress(0.4f, "更新总表");
            var usingTbl = UpdateSummaryUsingFile();

            //更新翻译需求表
            if (exportTranslate)
            {
                UpdateSummaryTranslateFile(usingTbl);
            }

            AllLanguageBuilder.BuildAll(usingTbl);
            
            //如果需要更新tmp
            if (updateTmp)
            {
                TMP_AssetTool.UpdateTMP_Asset(usingTbl, Progress);
            }

            //最后刷新一下资源
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }

        #region private method

        private static void Progress(float progress, string info = "")
        {
            EditorUtility.DisplayProgressBar("Building Language", info, progress);
        }

        #region tool

        #endregion

        #region update 更新写入操作

        /// <summary>
        /// 更新使用
        /// </summary>
        private static CsvTable UpdateSummaryUsingFile()
        {
            //1.更新原始文件：原始文件有的，Using没有的，写进去，原始文件没有的，Using有的，从Using删除
            var rawFieldDic = CollectRawFiles.Collect();
            var filePath = Path.Combine(_fullSummaryDir, MultiLanguageConfig.CsvNameSummaryUsing);
            var usingTal = CsvOperater.ReadSummaryFile(filePath);
            //提取所有using中的key值
            var usingList = new List<string>();
            for (var i = 0; i < usingTal.Count; i++)
            {
                usingList.Add(usingTal[i].Name);
            }

            var supports = _rules.supports;
            int addCnt = 0;
            foreach (var kv in rawFieldDic)
            {
                //存在
                if (usingList.Remove(kv.Key))
                {
                    continue;
                }

                addCnt++;
                var fieldInfo = new CsvFieldInfo {Name = kv.Key};
                for (var i = 0; i < supports.Length; i++)
                {
                    fieldInfo.SetValue(supports[i].language, kv.Value);
                }

                usingTal.AddField(fieldInfo);
            }

            if (addCnt > 0)
            {
                CsvOperater.WriteSummaryFile(usingTal, filePath);
            }

            return usingTal;
        }

        /// <summary>
        /// 更新翻译表
        /// </summary>
        private static void UpdateSummaryTranslateFile(CsvTable usingTbl)
        {
            //2.更新翻译需求表：Using中有的，已翻译文件中没有的，需要翻译，已翻译文件中有的，Using没有的，从已翻译中删除（被弃用：将这个已翻译的字段放到DiscardCache文件中，用于后续有需求的话从里面找回）
            var needTransList = new List<CsvFieldInfo>();
            var discardList = new List<CsvFieldInfo>();
            var translatedPath = Path.Combine(_fullSummaryDir, MultiLanguageConfig.CsvNameSummaryTranslated);
            var translatedTbl = CsvOperater.ReadSummaryFile(translatedPath);
            for (var i = 0; i < usingTbl.Count; i++)
            {
                var usingField = usingTbl[i];
                if (!translatedTbl.Contains(usingField))
                {
                    needTransList.Add(usingField);
                }
            }

            for (var i = 0; i < translatedTbl.Count; i++)
            {
                var translatedField = translatedTbl[i];
                if (!usingTbl.Contains(translatedField))
                {
                    translatedTbl.RemoveAt(i);
                    ++i;
                    discardList.Add(translatedField);
                }
            }

            #region 写翻译需求表

            if (needTransList.Count > 0)
            {
                var files = FileTool.GetCSVs(_fullTranslatingDir);
                var translatingDic = new Dictionary<string, CsvFieldInfo>();
                if (files.Length > 0)
                {
                    for (var i = 0; i < files.Length; i++)
                    {
                        var csvTable = CsvOperater.ReadSummaryFile(files[i]);
                        for (var j = 0; j < csvTable.Count; j++)
                        {
                            if (!translatingDic.ContainsKey(csvTable[j].Name))
                            {
                                translatingDic.Add(csvTable[j].Name, csvTable[j]);
                            }
                        }
                    }
                }

                for (var i = 0; i < needTransList.Count; i++)
                {
                    if (translatingDic.ContainsKey(needTransList[i].Name))
                    {
                        needTransList.RemoveAt(i);
                        i--;
                    }
                }

                if (needTransList.Count > 0)
                {
                    var version = _rules.translateVersion;
                    var writeFilePath = Path.Combine(_fullTranslatingDir, MultiLanguageConfig.CsvNameSummaryTranslating);
                    writeFilePath = string.Format(writeFilePath, version);
                    if (!File.Exists(writeFilePath))
                    {
                        var writeTable = new CsvTable();
                        for (var i = 0; i < needTransList.Count; i++)
                        {
                            writeTable.AddField(needTransList[i]);
                        }

                        CsvOperater.WriteSummaryFile(writeTable, writeFilePath);
                        _rules.translateVersion++;
                    }
                    else
                    {
                        var content = $"翻译需求的version发生错误，已存在：{writeFilePath}文件，请检查，是否需要手动升级version";
                        EditorUtility.DisplayDialog("Version冲突，请检查", content, "OK");
                    }
                }
            }

            #endregion

            #region 写废弃表

            if (discardList.Count > 0)
            {
                CsvOperater.WriteSummaryFile(translatedTbl, translatedPath);

                var discardPath = Path.Combine(_fullSummaryDir, MultiLanguageConfig.CsvNameDiscardCache);
                var table = CsvOperater.ReadSummaryFile(discardPath);
                for (var i = 0; i < discardList.Count; i++)
                {
                    table.AddField(discardList[i]);
                }

                CsvOperater.WriteSummaryFile(table, discardPath);
            }

            #endregion
        }

        #endregion

        #endregion
    }
}