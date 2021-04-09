﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using Editor.MultiLanguage.Scripts.tool;
using UnityEditor;
using Config = Editor.MultiLanguage.Scripts.MultiLanguageConfig;

namespace Editor.MultiLanguage.Scripts.func
{
    /// <summary>
    /// 导出翻译到csv文件
    /// </summary>
    public static class FuncExport2Csv
    {
        /// <summary>
        /// 原始文件路径
        /// </summary>
        private static string _fullRawDir = "";

        /// <summary>
        /// 生成文件路径
        /// </summary>
        private static string _fullBuildDir = "";

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

        public static void Start(bool exportTranslate, bool updateTMP)
        {
            Progress(0,"处理数据");
            #region 初始化数据

            _rules = MultiLanguageAssetsManager.GetRules();
            _fullRawDir = FileTool.GetFullPath(_rules.rawDirectory);
            _fullBuildDir = FileTool.GetFullPath(_rules.buildDirectory);
            _fullSummaryDir = FileTool.GetFullPath(_rules.summaryDirectory);
            _fullTranslatingDir = FileTool.GetFullPath(_rules.translatingDirectory);

            #endregion

            Progress(0.05f,"创建目录");
            //尝试创建目录
            FileTool.MakeDir(_fullRawDir);
            FileTool.MakeDir(_fullBuildDir);
            FileTool.MakeDir(_fullSummaryDir);
            FileTool.MakeDir(_fullTranslatingDir);

            #region check file

            Progress(0.1f,"检查总表");
            //顺序执行
            var midwayUse = CheckSummaryUsingFile();
            CheckSummaryTranslatedFile(midwayUse);

            //1.更新原始文件：原始文件有的，Using没有的，写进去，原始文件没有的，Using有的，从Using删除
            //2.更新翻译需求表：Using中有的，已翻译文件中没有的，需要翻译，已翻译文件中有的，Using没有的，从已翻译中删除（被弃用：将这个已翻译的字段放到DiscardCache文件中，用于后续有需求的话从里面找回）
            Progress(0.4f,"更新总表");
            var usingTbl = UpdateSummaryUsingFile();
            if (exportTranslate)
            {
                UpdateSummaryTranslateFile(usingTbl);
            }

            #endregion

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

        /// <summary>
        /// 搜集所有
        /// </summary>
        private static Dictionary<string, string> CollectAllRawFilesToDic()
        {
            var rawDic = new Dictionary<string, string>();

            var rules = _rules;
            var baseSupport = rules.baseLanguage;
            var rawFiles = FileTool.GetAllCsvFiles(_fullRawDir);
            for (var i = 0; i < rawFiles.Length; i++)
            {
                var tbl = CsvOperater.ReadSingleFile(rawFiles[i], baseSupport.language);
                var fileName = Path.GetFileNameWithoutExtension(rawFiles[i]);
                for (var i1 = 0; i1 < tbl.Count; i1++)
                {
                    var fieldInfo = tbl[i1];
                    var key = FileTool.GenerateUniqueKeyByFileName(fileName, fieldInfo.Name);
                    if (!rawDic.ContainsKey(key))
                    {
                        rawDic.Add(key, fieldInfo.GetValue(baseSupport.language));
                    }
                }
            }

            return rawDic;
        }

        #endregion

        #region update 更新写入操作

        /// <summary>
        /// 更新使用
        /// </summary>
        private static CsvTable UpdateSummaryUsingFile()
        {
            //1.更新原始文件：原始文件有的，Using没有的，写进去，原始文件没有的，Using有的，从Using删除
            var rawFieldDic = CollectAllRawFilesToDic();
            var filePath = Path.Combine(_fullSummaryDir, Config.CsvNameSummaryUsing);
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
                    fieldInfo.Add(supports[i].language, kv.Value);
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
            var translatedPath = Path.Combine(_fullSummaryDir, Config.CsvNameSummaryTranslated);
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
                var files = FileTool.GetAllCsvFiles(_fullTranslatingDir);
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
                    var writeFilePath = Path.Combine(_fullTranslatingDir, Config.CsvNameSummaryTranslating);
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

                var discardPath = Path.Combine(_fullSummaryDir, Config.CsvNameDiscardCache);
                var table = CsvOperater.ReadSummaryFile(discardPath);
                for (var i = 0; i < discardList.Count; i++)
                {
                    table.AddField(discardList[i]);
                }

                CsvOperater.WriteSummaryFile(table, discardPath);
            }

            #endregion
        }

        /// <summary>
        /// build 语言表
        /// </summary>
        private static void BuildLanguageFiles()
        {
        }

        #endregion

        #region using csv 检查操作

        /// <summary>
        /// 检查当前正在使用的总表文件
        /// </summary>
        private static bool CheckSummaryUsingFile()
        {
            var filePath = Path.Combine(_fullSummaryDir, Config.CsvNameSummaryUsing);
            if (File.Exists(filePath))
            {
                return false;
            }

            //合并档写文件 这里分两种情况：1.首次使用的时候直接把所有分表合并为一个使用表2.之前已经使用过一段时间存在翻译的文件，这里需要特殊处理把Single文件合并为正在使用的文件
            var rules = _rules;
            var supports = rules.supports;

            bool midWayUse = false;

            #region 检查是否是中途导入

            var files = FileTool.GetAllCsvFiles(_fullBuildDir);
            List<string> needFiles = new List<string>(supports.Length);
            for (var i = 0; i < supports.Length; i++)
            {
                var abbr = string.IsNullOrEmpty(supports[i].abbr) ? supports[i].language.ToString() : supports[i].abbr;
                var fileName = string.Format(Config.BuildLanguageFormat, abbr);
                needFiles.Add(fileName);
            }

            for (var i = 0; i < files.Length; i++)
            {
                var fileInfo = new FileInfo(files[i]);
                var fileName = fileInfo.Name;
                needFiles.Remove(fileName);
            }

            if (needFiles.Count < supports.Length)
            {
                midWayUse = true;
            }

            //缺失文件
            if (midWayUse && needFiles.Count > 0)
            {
                var sb = new StringBuilder();
                for (var i = 0; i < needFiles.Count; i++)
                {
                    sb.Append(needFiles[i]);
                    sb.Append("\r\n");
                }

                EditorUtility.DisplayDialog("中途导入的文件缺失", sb.ToString(), "OK");
            }

            #endregion

            if (midWayUse)
            {
                WriteSummaryUsingFileFromBuiltFiles();
            }
            else
            {
                WriteSummaryUsingFileFromRawFiles();
            }

            return midWayUse;
        }

        /// <summary>
        /// 从已编译好的文件中反向生成Raw文件，一般用于中途使用改工具才会用到这个方法
        /// </summary>
        private static void WriteSummaryUsingFileFromBuiltFiles()
        {
            var filePath = Path.Combine(_fullSummaryDir, Config.CsvNameSummaryUsing);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var rules = _rules;
            var supports = rules.supports;
            var saveTable = new CsvTable();
            for (var i = 0; i < supports.Length; i++)
            {
                var language = supports[i].language;
                var abbr = string.IsNullOrEmpty(supports[i].abbr) ? language.ToString() : supports[i].abbr;
                var fileName = string.Format(Config.BuildLanguageFormat, abbr);
                var fullPath = Path.Combine(_fullBuildDir, fileName);
                var singleTable = CsvOperater.ReadSingleFile(fullPath, language);
                var index = 0;
                for (int j = 0; j < singleTable.Count; j++)
                {
                    var singleFieldInfo = singleTable[j];
                    if (Config.BlackRawKey.Contains(singleFieldInfo.Name))
                    {
                        continue;
                    }
                    var fieldInfo = saveTable[index];
                    if (fieldInfo == null)
                    {
                        fieldInfo = new CsvFieldInfo {Name = singleFieldInfo.Name};
                        saveTable.AddField(fieldInfo);
                    }

                    singleFieldInfo.TryGetValue(language, out var content);
                    fieldInfo.Add(language, content);
                    index++;
                }
            }

            CsvOperater.WriteSummaryFile(saveTable, filePath);
        }

        /// <summary>
        /// 从原始文件写入当前使用中的csv总表,正常用于初始都是走的这个文件~
        /// </summary>
        private static void WriteSummaryUsingFileFromRawFiles()
        {
            var filePath = Path.Combine(_fullSummaryDir, Config.CsvNameSummaryUsing);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var rules = _rules;
            var supports = rules.supports;
            var baseSupport = rules.baseLanguage;
            var rawFiles = FileTool.GetAllCsvFiles(_fullRawDir);
            var saveTable = new CsvTable();
            for (var i = 0; i < rawFiles.Length; i++)
            {
                var fileName = Path.GetFileNameWithoutExtension(rawFiles[i]);
                var singleTable = CsvOperater.ReadSingleFile(rawFiles[i], baseSupport.language);
                for (var j = 0; j < singleTable.Count; j++)
                {
                    var fieldInfo = singleTable[j];
                    fieldInfo.Name = FileTool.GenerateUniqueKeyByFileName(fileName, fieldInfo.Name);
                    saveTable.AddField(fieldInfo);
                    fieldInfo.TryGetValue(baseSupport.language, out var content);
                    //重复写入字段 与基础语言一致
                    for (int k = 0; k < supports.Length; k++)
                    {
                        var language = supports[k].language;
                        if (language == baseSupport.language)
                        {
                            continue;
                        }

                        fieldInfo.Add(language, content);
                    }
                }
            }

            CsvOperater.WriteSummaryFile(saveTable, filePath);
        }

        #endregion

        #region translated csv 检查操作

        /// <summary>
        /// 检查已翻译表
        /// </summary>
        private static void CheckSummaryTranslatedFile(bool midwayUse)
        {
            var filePath = Path.Combine(_fullSummaryDir, Config.CsvNameSummaryTranslated);
            if (File.Exists(filePath))
            {
                return;
            }

            if (midwayUse)
            {
                //将Using中的所有字段拷贝到Translated文件中
                var usingFilePath = Path.Combine(_fullSummaryDir, Config.CsvNameSummaryUsing);
                var csvTable = CsvOperater.ReadSummaryFile(usingFilePath);
                CsvOperater.WriteSummaryFile(csvTable, filePath);
            }
            else
            {
                //生成一个空的Translated表
                CsvOperater.WriteEmptySummaryFile(filePath);
            }
        }

        #endregion

        #endregion
    }
}