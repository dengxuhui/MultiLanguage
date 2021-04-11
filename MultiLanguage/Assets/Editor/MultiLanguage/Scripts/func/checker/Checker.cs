using System.Collections.Generic;
using System.IO;
using System.Text;
using Editor.MultiLanguage.Scripts.tool;
using UnityEditor;
using Config = Editor.MultiLanguage.Scripts.MultiLanguageConfig;

namespace Editor.MultiLanguage.Scripts.func.checker
{
    /// <summary>
    /// 检查器，主要检查总表文件以及已翻译文件
    /// </summary>
    public static class Checker
    {
        /// <summary>
        /// 执行检查
        /// </summary>
        public static void DoCheck()
        {
            CheckDir();
            var midwayUse = CheckSummaryUsingFile();
            CheckSummaryTranslatedFile(midwayUse);
        }

        #region private method

        /// <summary>
        /// 检查各个目录是否存在
        /// </summary>
        private static void CheckDir()
        {
            var rules = MultiLanguageAssetsManager.GetRules();
            FileTool.TryMakeDir(FileTool.GetFullPath(rules.rawDirectory));
            FileTool.TryMakeDir(FileTool.GetFullPath(rules.buildDirectory));
            FileTool.TryMakeDir(FileTool.GetFullPath(rules.summaryDirectory));
            FileTool.TryMakeDir(FileTool.GetFullPath(rules.translatingDirectory));
        }
        
        /// <summary>
        /// 检查当前正在使用的总表文件
        /// </summary>
        private static bool CheckSummaryUsingFile()
        {
            var rules = MultiLanguageAssetsManager.GetRules();
            var fullSummaryDir = FileTool.GetFullPath(rules.summaryDirectory);
            var filePath = Path.Combine(fullSummaryDir, Config.CsvNameSummaryUsing);
            if (File.Exists(filePath))
            {
                return false;
            }

            //合并档写文件 这里分两种情况：1.首次使用的时候直接把所有分表合并为一个使用表2.之前已经使用过一段时间存在翻译的文件，这里需要特殊处理把Single文件合并为正在使用的文件
            var supports = rules.supports;

            bool midWayUse = false;

            #region 检查是否是中途导入

            var fullBuildDir = FileTool.GetFullPath(rules.buildDirectory);
            var files = FileTool.GetCSVs(fullBuildDir);
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
        /// 检查已翻译表
        /// </summary>
        private static void CheckSummaryTranslatedFile(bool midwayUse)
        {
            var rule = MultiLanguageAssetsManager.GetRules();
            var fullSummaryDir = FileTool.GetFullPath(rule.summaryDirectory);
            var filePath = Path.Combine(fullSummaryDir, Config.CsvNameSummaryTranslated);
            if (File.Exists(filePath))
            {
                return;
            }

            if (midwayUse)
            {
                //将Using中的所有字段拷贝到Translated文件中
                var usingFilePath = Path.Combine(fullSummaryDir, Config.CsvNameSummaryUsing);
                var csvTable = CsvOperater.ReadSummaryFile(usingFilePath);
                CsvOperater.WriteSummaryFile(csvTable, filePath);
            }
            else
            {
                //生成一个空的Translated表
                CsvOperater.WriteEmptySummaryFile(filePath);
            }
        }
        
        /// <summary>
        /// 从已编译好的文件中反向生成Raw文件，一般用于中途使用改工具才会用到这个方法
        /// </summary>
        private static void WriteSummaryUsingFileFromBuiltFiles()
        {
            var rules = MultiLanguageAssetsManager.GetRules();
            var fullSummaryDir = FileTool.GetFullPath(rules.summaryDirectory);
            var filePath = Path.Combine(fullSummaryDir, Config.CsvNameSummaryUsing);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var fullBuildDir = FileTool.GetFullPath(rules.buildDirectory);
            var supports = rules.supports;
            var saveTable = new CsvTable();
            for (var i = 0; i < supports.Length; i++)
            {
                var language = supports[i].language;
                var abbr = string.IsNullOrEmpty(supports[i].abbr) ? language.ToString() : supports[i].abbr;
                var fileName = string.Format(Config.BuildLanguageFormat, abbr);
                var fullPath = Path.Combine(fullBuildDir, fileName);
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
                    fieldInfo.SetValue(language, content);
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
            var rules = MultiLanguageAssetsManager.GetRules();
            var fullSummaryDir = FileTool.GetFullPath(rules.summaryDirectory);
            var filePath = Path.Combine(fullSummaryDir, Config.CsvNameSummaryUsing);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var fullRawDir = FileTool.GetFullPath(rules.rawDirectory);
            var supports = rules.supports;
            var baseSupport = rules.supports[rules.basicSupportIndex];
            var rawFiles = FileTool.GetCSVs(fullRawDir);
            var saveTable = new CsvTable();
            for (var i = 0; i < rawFiles.Length; i++)
            {
                var fileName = Path.GetFileNameWithoutExtension(rawFiles[i]);
                var singleTable = CsvOperater.ReadSingleFile(rawFiles[i], baseSupport.language);
                for (var j = 0; j < singleTable.Count; j++)
                {
                    var fieldInfo = singleTable[j];
                    fieldInfo.Name = FileTool.FromRawKeyToSummaryKey(fileName, fieldInfo.Name);
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

                        fieldInfo.SetValue(language, content);
                    }
                }
            }

            CsvOperater.WriteSummaryFile(saveTable, filePath);
        }

        #endregion
    }
}