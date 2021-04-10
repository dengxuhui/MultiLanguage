using System.IO;
using Editor.MultiLanguage.Scripts.tool;
using Config = Editor.MultiLanguage.Scripts.MultiLanguageConfig;

namespace Editor.MultiLanguage.Scripts.func
{
    /// <summary>
    /// 拷贝资源到运行时
    /// </summary>
    public static class FuncCopyAssets
    {
        public static void Start()
        {
            var rules = MultiLanguageAssetsManager.GetRules();
            var supports = rules.supports;
            for (var i = 0; i < supports.Length; i++)
            {
                var support = supports[i];
                var abbr = string.IsNullOrEmpty(support.abbr) ? support.language.ToString() : support.abbr;
                var name = string.Format(Config.BuildLanguageFormat, abbr);
                var srcPath = Path.Combine(FileTool.GetFullPath(rules.buildDirectory), name);
                var targetPath = Path.Combine(FileTool.GetFullPath(rules.runtimeAssetsDirectory), name);
                if (!File.Exists(srcPath))
                {
                    continue;
                }

                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                File.Copy(srcPath,targetPath);
            }
        }
    }
}