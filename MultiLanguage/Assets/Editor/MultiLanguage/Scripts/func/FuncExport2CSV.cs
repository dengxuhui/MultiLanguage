using System.Collections.Generic;
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

        public static void Start()
        {
            var rules = MultiLanguageAssetsManager.GetRules();
            _fullRawDir = FileTool.GetFullPath(rules.rawDirectory);
            _fullBuildDir = FileTool.GetFullPath(rules.buildDirectory);
            _fullSummaryDir = FileTool.GetFullPath(rules.summaryDirectory);
            //尝试创建目录
            FileTool.MakeDir(_fullRawDir);
            FileTool.MakeDir(_fullBuildDir);
            FileTool.MakeDir(_fullSummaryDir);

            #region check file

            //顺序执行
            var midwayUse = CheckSummaryUsingFile();
            CheckSummaryTranslatedFile(midwayUse);

            //TODO 差量更新，更新规则：
            //1.更新原始文件：原始文件有的，Using没有的，写进去，原始文件没有的，Using有的，从Using删除
            //2.更新翻译需求表：Using中有的，已翻译文件中没有的，需要翻译，已翻译文件中有的，Using没有的，从已翻译中删除（被弃用：将这个已翻译的字段放到DiscardCache文件中，用于后续有需求的话从里面找回）
            
            #endregion

            //最后刷新一下资源
            AssetDatabase.Refresh();
        }

        #region private method

        #region using csv 使用中的总表操作相关

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
            var rules = MultiLanguageAssetsManager.GetRules();
            var supports = rules.supports;

            bool midWayUse = false;

            #region 检查是否是中途导入

            var files = Directory.GetFiles(_fullBuildDir);
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

            var rules = MultiLanguageAssetsManager.GetRules();
            var supports = rules.supports;
            var saveTable = new CsvTable();
            for (var i = 0; i < supports.Length; i++)
            {
                var language = supports[i].language;
                var abbr = string.IsNullOrEmpty(supports[i].abbr) ? language.ToString() : supports[i].abbr;
                var fileName = string.Format(Config.BuildLanguageFormat, abbr);
                var fullPath = Path.Combine(_fullBuildDir, fileName);
                var singleTable = CsvOperater.ReadSingleFile(fullPath, language);
                for (int j = 0; j < singleTable.Count; j++)
                {
                    var fieldInfo = saveTable[j];
                    var singleFieldInfo = singleTable[j];
                    if (fieldInfo == null)
                    {
                        fieldInfo = new CsvFieldInfo {Name = singleFieldInfo.Name};
                        saveTable.AddField(fieldInfo);
                    }

                    singleFieldInfo.Contents.TryGetValue(language, out var content);
                    fieldInfo.Contents.Add(language, content);
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

            var rules = MultiLanguageAssetsManager.GetRules();
            var supports = rules.supports;
            var baseSupport = rules.baseLanguage;
            var rawFiles = Directory.GetFiles(_fullRawDir);
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
                    fieldInfo.Contents.TryGetValue(baseSupport.language, out var content);
                    //重复写入字段 与基础语言一致
                    for (int k = 0; k < supports.Length; k++)
                    {
                        var language = supports[k].language;
                        if (language == baseSupport.language)
                        {
                            continue;
                        }

                        fieldInfo.Contents.Add(language, content);
                    }
                }
            }

            CsvOperater.WriteSummaryFile(saveTable, filePath);
        }

        #endregion


        #region 已翻译的总表操作相关

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