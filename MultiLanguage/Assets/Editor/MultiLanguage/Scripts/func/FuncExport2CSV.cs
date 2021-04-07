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
            var fullRawDir = FileTool.GetFullPath(relativeRawDir);
            //尝试创建目录
            FileTool.MakeDir(fullRawDir);
            
            
            //最后刷新一下资源
            AssetDatabase.Refresh();
        }
    }
}