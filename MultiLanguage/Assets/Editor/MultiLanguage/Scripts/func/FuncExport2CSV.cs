using System.IO;
using Editor.MultiLanguage.Scripts.tool;
using UnityEditor;

namespace Editor.MultiLanguage.Scripts.func
{
    /// <summary>
    /// 导出翻译到csv文件
    /// </summary>
    public static class FuncExport2Csv
    {
        public static void Start()
        {
            var rules = MultiLanguageAssetsManager.GetRules();
            var relativeRawDir = rules.rawDirectory;
            var relativeExportDir = rules.exportDirectory;
            var fullRawDir = FileTool.GetFullPath(relativeRawDir);
            var fullExportDir = FileTool.GetFullPath(relativeExportDir);
            //尝试创建目录
            FileTool.MakeDir(fullRawDir);
            FileTool.MakeDir(fullExportDir);

            //step1 检查文件
            CheckAllLanguageRawFile(fullExportDir);


            //最后刷新一下资源
            AssetDatabase.Refresh();
        }

        private static void CheckAllLanguageRawFile(string fullExportDir)
        {
            var filePath = Path.Combine(fullExportDir, MultiLanguageConfig.AllLanguageRawFile);
            if (File.Exists(filePath))
            {
                return;
            }

            //如果不存在总表文件，就从现有的Export目录下导入到总表中，并将所有状态设置为已翻译
            var rules = MultiLanguageAssetsManager.GetRules();
            var supportLan = rules.supports;
            string exportFormat = Path.Combine(fullExportDir, MultiLanguageConfig.ExportLanguageFormat);
            for (var i = 0; i < supportLan.Length; i++)
            {
                var lang = supportLan[i];
                string path = string.Format(exportFormat,
                    string.IsNullOrEmpty(lang.abbr) ? lang.language.ToString() : lang.abbr);
                if (!File.Exists(path))
                {
                    continue;
                }

                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    
                }
            }
        }
    }
}