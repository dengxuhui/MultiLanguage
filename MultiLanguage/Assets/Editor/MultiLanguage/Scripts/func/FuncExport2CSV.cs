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
        /// 导出文件路径
        /// </summary>
        private static string _fullExportDir = "";

        /// <summary>
        /// 合并目录
        /// </summary>
        private static string _fullMergeDir = "";

        public static void Start()
        {
            var rules = MultiLanguageAssetsManager.GetRules();
            var relativeRawDir = rules.rawDirectory;
            var relativeExportDir = rules.exportDirectory;
            var relativeMergeDir = rules.mergeDirectory;
            _fullRawDir = FileTool.GetFullPath(relativeRawDir);
            _fullExportDir = FileTool.GetFullPath(relativeExportDir);
            _fullMergeDir = FileTool.GetFullPath(relativeMergeDir);
            //尝试创建目录
            FileTool.MakeDir(_fullRawDir);
            FileTool.MakeDir(_fullExportDir);
            FileTool.MakeDir(_fullMergeDir);

            #region check file

            //顺序执行
            CheckUsingRawFile();
            CheckTranslatedRawFile();

            #endregion

            //最后刷新一下资源
            AssetDatabase.Refresh();
        }

        #region private method

        #region using csv 使用中的总表操作相关

        /// <summary>
        /// 检查当前正在使用的总表文件
        /// </summary>
        private static void CheckUsingRawFile()
        {
            var filePath = Path.Combine(_fullMergeDir, Config.CsvNameMergeUsing);
            if (File.Exists(filePath))
            {
                return;
            }

            //合并档写文件 这里分两种情况：1.首次使用的时候直接把所有分表合并为一个使用表2.之前已经使用过一段时间存在翻译的文件，这里需要特殊处理把Single文件合并为正在使用的文件
            var rules = MultiLanguageAssetsManager.GetRules();
            var supports = rules.supports;

            bool midWayUse = false;

            #region 检查是否是中途导入

            var files = Directory.GetFiles(_fullExportDir);
            List<string> needFiles = new List<string>(supports.Length);
            for (var i = 0; i < supports.Length; i++)
            {
                var abbr = string.IsNullOrEmpty(supports[i].abbr) ? supports[i].language.ToString() : supports[i].abbr;
                var fileName = string.Format(Config.ExportLanguageFormat, abbr);
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
            if (needFiles.Count > 0)
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
                WriteUsingRawFileFromMidwayImport();
            }
            else
            {
                WriteUsingRawFileFromRawFiles();
            }
        }

        /// <summary>
        /// 从中途导入的文件写入总表
        /// </summary>
        private static void WriteUsingRawFileFromMidwayImport()
        {
            var filePath = Path.Combine(_fullMergeDir, Config.CsvNameMergeUsing);
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
                var fileName = string.Format(Config.ExportLanguageFormat, abbr);
                var fullPath = Path.Combine(_fullExportDir, fileName);
                var singleTable = CsvOperater.ReadSingleLangFile(fullPath, language);
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

            CsvOperater.WriteMergeLangFile(saveTable, filePath);
        }

        /// <summary>
        /// 从原始文件写入当前使用中的csv总表
        /// </summary>
        private static void WriteUsingRawFileFromRawFiles()
        {
        }

        #endregion


        #region 已翻译的总表操作相关

        /// <summary>
        /// 检查已翻译表
        /// </summary>
        private static void CheckTranslatedRawFile()
        {
            var filePath = Path.Combine(_fullMergeDir, Config.CsvNameMergeTranslated);
            if (File.Exists(filePath))
            {
                return;
            }
        }

        #endregion

        #endregion
    }
}